# Seadora Travel — Target Architecture (Microservices, Scalable, Multi-Branch)

> Spec document. The implementation plan at
> `docs/superpowers/plans/2026-08-26-microservices-platform.md` argues from this spec.
> This is the "north star"; we migrate toward it in phases without a big-bang rewrite.

## 1. Business context

Seadora is a **tour/travel agency platform**. It sells **tours/trips** in **destinations**, each
made of **activities** (categories). Tours have **types** (group / private / VIP / corporate…) that
change **how a booking is arranged**. The platform must manage: the **public website**, an **admin
back-office**, **customers & profiles**, **bookings**, the **tour/supplier catalog**, **customer
support via a real email mailbox**, a **website chatbot**, and — later — a **financial model
(revenue, receivables, supplier payables, ledger)**.

**Explicit non-functional goal (from the owner):** stay on **microservices** so the system scales
and supports **multiple branches** as the business grows.

## 2. Guiding principles

1. **One bounded context = one service = one database.** No shared tables across services. A service
   owns its data; others get it via API or events, never by reaching into its DB.
2. **Async-first between services.** Cross-service writes and side effects flow through an **event
   bus with the transactional outbox pattern**. Synchronous calls are allowed only for
   **read-time queries** that need a fresh answer, and always through the gateway with resilience
   policies (timeout, retry, circuit breaker).
3. **Own your reference data.** A service that needs another's data keeps a **read-only local
   projection** kept current by events — it does **not** duplicate by hand or call synchronously in
   hot paths (fixes today's Booking↔Content coupling).
4. **Money and inventory are sacred.** Overbooking and accounting errors are business-ending.
   Inventory decrements are **transactional with concurrency control**; money is a **double-entry
   ledger of immutable journal entries**, never a boolean.
5. **Multi-branch = tenancy dimension, not a fork.** Every business row carries a `BranchId`.
   Services are branch-aware from day one; we never copy a service per branch.
6. **Ship in slices.** Each phase leaves the system **working and deployable**. We add the new path,
   dual-run, then retire the old one.

## 3. Multi-branch (tenancy) model

- A **Branch** (a.k.a. agency location) is a first-class concept owned by a new **Organization
  service** (branches, staff-to-branch assignment, branch settings, currency/locale defaults).
- **Row-level tenancy** inside each service DB: every tenant-scoped table has a non-null `BranchId`
  with an index; all queries filter by branch; the JWT carries the caller's branch claim(s).
- **Shared schema, shared DB per service** (not DB-per-branch). This scales to many branches without
  multiplying infrastructure and keeps cross-branch reporting a simple query. Upgrade path noted in
  §9 if a branch ever needs physical isolation.
- Catalog data can be **branch-scoped or global**: a Tour/Supplier has an optional `BranchId`
  (null = available to all branches) so head office can publish shared inventory while a branch adds
  its own.

## 4. Target service map

Existing (evolve in place):

| Service | Bounded context | Owns |
|---|---|---|
| **Identity** | AuthN/AuthZ | users, roles, permissions, social login, **branch claims** |
| **Content (Catalog)** | Product catalog | tours, tour types (+ arrangement policy), categories, destinations, suppliers, pricing/rate plans |
| **Booking** | Reservations & inventory | bookings, **departures/availability**, allocation, cancellation/refund policy |
| **FileServer** | Binary storage | uploads, documents (passports), signed URLs |
| **ApiGateway** | Edge | YARP routing, JWT validation, rate limiting, correlation |

New (introduced by phases):

| Service | Bounded context | Owns |
|---|---|---|
| **Organization** | Tenancy | branches, staff↔branch, branch settings |
| **Customer (CRM)** | Customer master | customer profiles, consent/GDPR, documents, booking history projection, lifetime value |
| **Support** | Service desk | tickets, message threads, **inbound/outbound email mailbox**, channels (email/web/whatsapp/chat), SLA |
| **Concierge** | Website chatbot | conversation sessions, RAG over catalog, human-handoff → Support ticket |
| **Finance (Ledger)** | Money | payments (PSP), receivables, supplier payables, commissions, refunds, taxes, **double-entry ledger**, revenue reports |
| **Notifications** | Delivery | email/WhatsApp/push channels, templates, delivery status (centralizes today's SMTP + Twilio) |

**Cross-cutting infra:** message broker (**RabbitMQ + MassTransit**), a **read/reporting store**
for cross-service analytics, and the existing **`Seadora.Common`** shared kernel extended with
messaging + outbox building blocks.

## 5. Communication patterns

- **Commands within a service:** MediatR (already in use). Keep it.
- **Between services (writes/side effects):** **integration events** over RabbitMQ via MassTransit,
  published through a **transactional outbox** (event persisted in the same DB transaction as the
  state change; a dispatcher relays it). Consumers are **idempotent** (dedupe by message id).
- **Between services (fresh reads):** synchronous HTTP **through the gateway**, wrapped in
  **Polly** (timeout + retry + circuit breaker). Used sparingly; prefer local projections.
- **Edge → services:** gateway routes `/api/{service}/...`, validates JWT (already aligned), adds
  correlation id (already in `Seadora.Common`), enforces rate limits.

### Canonical event flows

- **Catalog change** → Content publishes `TourPublished/TourUpdated/TourTypePolicyChanged` →
  Booking updates its **tour projection** (capacity, type policy, price snapshot inputs); Concierge
  refreshes its retrieval index.
- **Booking placed** → Booking transactionally decrements a **Departure** allocation and publishes
  `BookingPlaced` → Customer appends to history; Finance opens a **receivable**; Notifications sends
  confirmation.
- **Payment settled** (PSP webhook → Finance) → `PaymentSettled` → Booking marks paid; Notifications
  receipts; ledger posts journal entries.
- **Inbound email** (mailbox poll in Support) → `InquiryReceived` → Support ticket created, linked to
  Customer/Booking by email match; Notifications/agents reply; thread tracked.

## 6. Core domain models (target shape)

Only the **new or materially-changed** aggregates are listed; existing catalog entities stay.

### Booking service — fix inventory & money
- **Departure** (new): `Id, BranchId, TourId, StartUtc, TimeSlot, Capacity, AllocationModel
  (Shared|WholeUnit), Version (rowversion)`. The unit availability is decremented against.
- **Allocation** is enforced in the same DB transaction as the booking insert using the `Version`
  optimistic token (no overbooking).
- **TourProjection** (new, read-only): `TourId, BranchId, TourTypeCode, AllocationModel,
  MinCapacity, MaxCapacity, RequiresGuestDetails, RequiresPassport, PayLaterAllowed, priceInputs` —
  populated from Content events. Replaces hand-duplicated Tour fields.
- **Booking** (change): add `BranchId`, `CustomerId`, `TourTypeCode` (snapshot), and a **money
  breakdown** value object `Money { Subtotal, AddonsTotal, Discount, TaxTotal, Total, Currency,
  AmountPaid, BalanceDue }`. Replace `TripType` free-text with `TourTypeCode`; replace `IsPaid`
  boolean with derived state from `BalanceDue`.
- **GuestDetail** supports **age bands** (`Adult|Child|Infant`) for per-pax pricing.

### Content service — make TourType drive arrangement
- **TourType** (change): add arrangement policy fields `AllocationModel, DefaultMinCapacity,
  DefaultMaxCapacity, RequiresGuestDetails, RequiresPassport, PayLaterAllowed`. Tour keeps override
  fields; the default lives once per type. Emit `TourTypePolicyChanged` on edit.
- **Pricing** (change): introduce **RatePlan** (per-pax vs per-unit, age-band prices, seasonal
  windows, currency) rather than a single `Tour.Price`. `Tour.Price` becomes the "from" display
  price derived from the active rate plan.
- **Supplier / SupplierAgreement** (change): flesh out `PaymentAgreement` (today just `Id+Name`) into
  net rate, commission %, payment terms, settlement currency — the basis of payables.

### Customer service (new)
- **Customer**: `Id, BranchId, IdentityUserId?, FullName, Email (unique per branch), Phone,
  Nationality, MarketingConsent, ConsentUpdatedUtc, CreatedUtc`.
- **CustomerDocument**: `Id, CustomerId, Type (Passport|Id), FileRef (FileServer), ExpiryUtc,
  encrypted-at-rest`. GDPR retention + access-controlled.
- **BookingHistory** (projection from `BookingPlaced`/`BookingUpdated`): lightweight rows for CRM
  views + lifetime value.

### Support service (new)
- **Ticket**: `Id, BranchId, CustomerId?, BookingId?, Channel (Email|Web|WhatsApp|Chat), Status,
  Priority, AssignedAgentId?, SlaDueUtc, CreatedUtc`.
- **Message**: `Id, TicketId, Direction (In|Out), Channel, FromAddress, Body, SentUtc,
  ExternalMessageId (email Message-ID for threading)`.
- **Mailbox ingestion**: poller (IMAP or Microsoft Graph / SES-inbound) → normalize → `Message`
  (in) → open/append `Ticket` → publish `InquiryReceived`.

### Finance service (new)
- **Payment**: PSP charge/refund with `IdempotencyKey`, provider, status; driven by webhooks.
- **LedgerAccount** + **JournalEntry** (double-entry): receivables (customer owes), payables
  (agency owes supplier net), commission/margin, tax, cash/PSP. Every money movement is a balanced
  entry; `IsPaid` and revenue are **derived**, never stored as truth.
- **Reconciliation**: match PSP settlements to receivables.

## 7. Cross-cutting concerns

- **Security/PII (P0 compliance):** passports + nationality are sensitive. Encrypt documents at rest,
  access-control by branch + role, log access, enforce retention/consent. Fail-closed auth is already
  in place from earlier work — keep the gateway as the single validation point.
- **Idempotency:** booking creation and all event consumers use idempotency keys / dedupe tables to
  survive retries and double-submits.
- **Observability:** correlation id (exists) propagated over HTTP **and** message headers; structured
  logs; health checks per service; readiness gates in compose/orchestrator.
- **Config:** per-service settings via env; secrets out of source (today's JWT secret should move to
  a secret store before production).

## 8. Data & reporting

- **No cross-service joins.** Reporting (today's `ReportsController` rebuilds a Tour DTO) moves to a
  **reporting read model** fed by events (Booking, Finance, Content projections), queried by admin.
- Each service keeps its own migrations; the reporting store is rebuildable from events.

## 9. Scaling & multi-branch growth path

- **Stateless services** scale horizontally behind the gateway; Postgres scales via read replicas for
  read-heavy services (Content, reporting).
- **Branches**: row-level `BranchId` covers dozens–hundreds of branches on shared infra. If a
  specific branch ever needs physical isolation (regulatory), the seam is already there — that
  branch's rows can be migrated to a dedicated DB/schema without code changes to callers.
- **Broker**: RabbitMQ clustering; competing consumers per service for throughput.

## 10. What we explicitly keep (don't rewrite)

- YARP gateway, JWT alignment + fail-closed auth (done earlier), `Seadora.Common` cross-cutting,
  CQRS/MediatR, EF Core + Postgres, jsonb localization, the two Vue SPAs, refund/cancellation policy
  services and the booking status-transition validator.

## 11. Phase overview (detail in the implementation plan)

- **Phase 0 — Foundations:** messaging + outbox in `Seadora.Common`, RabbitMQ in compose, idempotency
  helpers, `BranchId` groundwork + branch claim in JWT. *No behavior change; rails only.*
- **Phase 1 — Fix the core (P0 correctness):** TourType arrangement policy + event; Booking
  **TourProjection** via events (kill hand-duplication); **Departure/Allocation** with concurrency
  (stop overbooking); booking **money breakdown**.
- **Phase 2 — Customer (CRM):** Customer service + profiles + documents; link bookings to customers;
  booking-history projection.
- **Phase 3 — Support mailbox:** Support service; inbound/outbound email; migrate `ContactInquiry`
  → tickets; centralize Notifications.
- **Phase 4 — Concierge chatbot:** conversation sessions, RAG over catalog, human handoff → Support.
- **Phase 5 — Finance & ledger:** PSP integration, double-entry ledger, receivables/payables,
  supplier settlement, revenue reporting read model.
- **Phase 6 — Organization/branches hardening:** Organization service, branch admin, per-branch
  settings, cross-branch reporting.

Each phase is independently shippable and leaves every service green.
