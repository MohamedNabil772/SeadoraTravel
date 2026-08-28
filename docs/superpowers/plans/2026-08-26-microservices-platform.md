# Seadora Microservices Platform — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development
> (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use
> checkbox (`- [ ]`) syntax for tracking.

**Goal:** Evolve Seadora from a synchronous distributed monolith into a properly decoupled,
event-driven, multi-branch microservices platform — without a big-bang rewrite.

**Architecture:** Bounded-context-per-service with DB-per-service; async integration events over
RabbitMQ using the transactional **outbox** pattern; read-only local **projections** instead of
hand-duplicated data; synchronous calls only for fresh reads via the gateway with Polly resilience.
Every business row is branch-scoped (`BranchId`) for multi-branch scale.

**Tech Stack:** .NET 9, EF Core + PostgreSQL, MediatR (CQRS), MassTransit + RabbitMQ, Polly, YARP
gateway, Vue 3 SPAs, xUnit + FluentAssertions + Testcontainers (integration).

**Spec:** `docs/architecture/TARGET_ARCHITECTURE.md`

## Global Constraints

- **Target framework:** `net9.0` for all backend projects (match existing).
- **No shared tables across services.** A service reads another's data only via API or events.
- **Every new tenant-scoped table has a non-null `BranchId` (uuid) with an index.**
- **Every integration event is published via the outbox** (same DB transaction as the state change).
- **Every event consumer is idempotent** (dedupe by message id).
- **Money type:** `decimal` with explicit ISO 4217 `Currency`; never `float`/`double`.
- **Every phase ends green:** `dotnet build SEADORA.sln` + touched-service tests pass, and
  `docker compose up -d` brings all services `Up`.
- **Migrations, not `EnsureCreated`.** Schema changes ship as EF migrations. Dummy dev data may be
  wiped/reseeded, but the seeders must repopulate it.
- **Commit frequently**, one deliverable per commit, with the repo's co-author trailer.

---

## File Structure (where new code lives)

- Messaging kernel → `src/Services/Common/Seadora.Common/Messaging/` (shared by all services).
- New services follow the existing 4-project layout used by Content/Booking:
  `<Name>.Service/Seadora.<Name>.{Domain,Application,Infrastructure,API}`.
- Gateway routes → `src/ApiGateway/appsettings.json` (YARP `Clusters`/`Routes`).
- Compose infra → `seadoratravel/docker-compose.yml`.

---

# PHASE 0 — Foundations (rails only, no behavior change)

**Outcome:** RabbitMQ running; `Seadora.Common` can publish/consume events via an outbox; idempotency
helper exists; `BranchId` + branch claim plumbed. Nothing changes functionally yet.

### Task 0.1: Add RabbitMQ to the stack

**Files:**
- Modify: `seadoratravel/docker-compose.yml`

**Interfaces:**
- Produces: a `rabbitmq` service reachable at `rabbitmq:5672` (AMQP) and `:15672` (management UI),
  env `RABBITMQ_DEFAULT_USER=seadora`, `RABBITMQ_DEFAULT_PASS=seadora`.

- [ ] **Step 1:** Add a `rabbitmq:3-management` service with a healthcheck (`rabbitmq-diagnostics
  -q ping`), a named volume, and put it on the existing default network.
- [ ] **Step 2:** Add `depends_on: rabbitmq: {condition: service_healthy}` to identity/content/
  booking services (they will publish/consume later).
- [ ] **Step 3:** `docker compose up -d rabbitmq`; verify management UI at `http://localhost:15672`
  responds and the container is `healthy`.
- [ ] **Step 4:** Commit.

### Task 0.2: Messaging kernel in `Seadora.Common`

**Files:**
- Create: `src/Services/Common/Seadora.Common/Messaging/IIntegrationEvent.cs`
- Create: `src/Services/Common/Seadora.Common/Messaging/IntegrationEvent.cs` (base: `Guid Id`,
  `DateTime OccurredUtc`)
- Create: `src/Services/Common/Seadora.Common/Messaging/IEventPublisher.cs`
  (`Task PublishAsync(IIntegrationEvent evt, CancellationToken ct)`)
- Create: `src/Services/Common/Seadora.Common/Messaging/MassTransitEventPublisher.cs`
- Create: `src/Services/Common/Seadora.Common/Messaging/MessagingDependencyInjection.cs`
  (`AddSeadoraMessaging(this IServiceCollection, IConfiguration)` — registers MassTransit + RabbitMQ
  from `RabbitMq:Host/User/Pass` config)
- Modify: `src/Services/Common/Seadora.Common/Seadora.Common.csproj` (add `MassTransit.RabbitMQ`)
- Test: `src/Services/Common/Seadora.Common.Tests/Messaging/EventPublisherTests.cs`

**Interfaces:**
- Produces: `IEventPublisher.PublishAsync`, `AddSeadoraMessaging(services, config)`,
  `IntegrationEvent` base type.

- [ ] **Step 1:** Write a failing test: a fake bus captures a published `IntegrationEvent` and
  asserts `Id`/`OccurredUtc` are set and the message reaches the bus.
- [ ] **Step 2:** Run it; expect FAIL (types not defined).
- [ ] **Step 3:** Implement the interfaces + `MassTransitEventPublisher` (wraps
  `IPublishEndpoint.Publish`) + DI registration reading `RabbitMq:*`.
- [ ] **Step 4:** Run the test; expect PASS.
- [ ] **Step 5:** Commit.

### Task 0.3: Transactional outbox (EF Core)

**Files:**
- Create: `src/Services/Common/Seadora.Common/Messaging/Outbox/OutboxMessage.cs`
  (`Id, Type, Payload(jsonb), OccurredUtc, ProcessedUtc?`)
- Create: `src/Services/Common/Seadora.Common/Messaging/Outbox/IOutboxWriter.cs`
  (`void Enqueue(IIntegrationEvent evt)` — writes an `OutboxMessage` to the current DbContext)
- Create: `src/Services/Common/Seadora.Common/Messaging/Outbox/OutboxDispatcher.cs`
  (a `BackgroundService` that polls unprocessed rows, publishes via `IEventPublisher`, marks
  `ProcessedUtc`)
- Create: `src/Services/Common/Seadora.Common/Messaging/Outbox/OutboxDependencyInjection.cs`
- Test: `src/Services/Common/Seadora.Common.Tests/Messaging/OutboxDispatcherTests.cs`

**Interfaces:**
- Consumes: `IEventPublisher` (Task 0.2).
- Produces: `IOutboxWriter.Enqueue`, `OutboxMessage` entity, `AddSeadoraOutbox(services)`,
  `OutboxDispatcher`.

- [ ] **Step 1:** Failing test: enqueue an event + save; dispatcher run publishes it once and sets
  `ProcessedUtc`; a second run publishes nothing (idempotent relay).
- [ ] **Step 2:** Run; expect FAIL.
- [ ] **Step 3:** Implement `OutboxMessage`, `IOutboxWriter` (serialize to jsonb), and
  `OutboxDispatcher` (batch poll, publish, mark processed, honor cancellation).
- [ ] **Step 4:** Run; expect PASS.
- [ ] **Step 5:** Commit.

### Task 0.4: Idempotent consumer helper

**Files:**
- Create: `src/Services/Common/Seadora.Common/Messaging/Idempotency/ProcessedMessage.cs`
  (`MessageId (pk), ConsumerName, ProcessedUtc`)
- Create: `src/Services/Common/Seadora.Common/Messaging/Idempotency/IdempotentConsumer.cs`
  (`Task<bool> AlreadyProcessed(Guid messageId, string consumer)` + `Task MarkProcessed(...)`)
- Test: `src/Services/Common/Seadora.Common.Tests/Messaging/IdempotencyTests.cs`

**Interfaces:**
- Produces: `IdempotentConsumer.AlreadyProcessed / MarkProcessed`, `ProcessedMessage` entity.

- [ ] **Step 1:** Failing test: first call for a `(messageId, consumer)` returns `false`; after
  `MarkProcessed`, returns `true`.
- [ ] **Step 2:** Run; expect FAIL.
- [ ] **Step 3:** Implement using a unique key on `(MessageId, ConsumerName)`.
- [ ] **Step 4:** Run; expect PASS.
- [ ] **Step 5:** Commit.

### Task 0.5: BranchId groundwork + branch claim

**Files:**
- Create: `src/Services/Common/Seadora.Common/Tenancy/ICurrentBranch.cs` (`Guid BranchId { get; }`)
- Create: `src/Services/Common/Seadora.Common/Tenancy/CurrentBranchAccessor.cs` (reads `branch`
  claim from `HttpContext.User`)
- Modify: Identity token generation to include a `branch` claim (default single "Head Office" branch
  seeded now). Path: `src/Services/Identity.Service/Seadora.Identity.Infrastructure/**` token service
  + `IdentitySeeder.cs` (seed a default Branch id constant).
- Test: `src/Services/Common/Seadora.Common.Tests/Tenancy/CurrentBranchAccessorTests.cs`

**Interfaces:**
- Produces: `ICurrentBranch.BranchId`; a well-known default `BranchId` GUID constant reused by all
  seeders until the Organization service exists (Phase 6).

- [ ] **Step 1:** Failing test: given a principal with a `branch` claim, `CurrentBranchAccessor`
  returns that GUID; missing claim → falls back to the default Head-Office GUID.
- [ ] **Step 2:** Run; expect FAIL.
- [ ] **Step 3:** Implement accessor; add `branch` claim to the identity token; seed default branch
  constant.
- [ ] **Step 4:** Run; expect PASS. Verify login still works via `docker compose` smoke test
  (identity `Up`, `/api/auth/login` issues a token containing `branch`).
- [ ] **Step 5:** Commit.

**Phase 0 gate:** `dotnet build SEADORA.sln` green; `Seadora.Common.Tests` green; `docker compose
up -d` all `Up` incl. rabbitmq; no functional change to existing endpoints.

---

# PHASE 1 — Fix the core (P0 correctness)

**Outcome:** TourType actually drives arrangement; Booking reads catalog via an event-fed projection
(no hand-duplication); overbooking is impossible; bookings carry a real money breakdown. This is the
highest-value phase.

### Task 1.1: TourType arrangement policy + event (Content)

**Files:**
- Modify: `src/Services/Content.Service/Seadora.Content.Domain/Entities/TourType.cs` (add
  `AllocationModel` enum, `DefaultMinCapacity`, `DefaultMaxCapacity`, `RequiresGuestDetails`,
  `RequiresPassport`, `PayLaterAllowed`)
- Create: `src/Services/Content.Service/Seadora.Content.Domain/Enums/AllocationModel.cs`
  (`Shared`, `WholeUnit`)
- Modify: `.../Seadora.Content.Infrastructure/Persistence/ContentDbContext.cs` (map new columns)
- Create: EF migration `AddTourTypePolicy`
- Create: `.../Seadora.Common/Contracts/Content/TourTypePolicyChanged.cs` **or** a shared
  `Seadora.Contracts` project (see note) — event with `TourTypeId, Code, AllocationModel, Min, Max,
  RequiresGuestDetails, RequiresPassport, PayLaterAllowed, BranchId?`
- Modify: TourType update command handler to `Enqueue` the event on the outbox.
- Test: `.../Seadora.Content.Application.Tests/TourTypes/UpdatePolicyTests.cs`

**Interfaces:**
- Consumes: `IOutboxWriter` (0.3), `AllocationModel`.
- Produces: `TourTypePolicyChanged` contract, `TourType` policy fields, `AllocationModel` enum.

> **Contracts note:** create one `src/Services/Common/Seadora.Contracts` project holding integration
> event DTOs shared by publisher + consumer, referenced by every service. Add it here (first event)
> and reuse for all later events. Keep it dependency-free (POCOs only).

- [ ] **Step 1:** Failing test: updating a TourType's policy persists the fields and enqueues one
  `TourTypePolicyChanged` with matching values.
- [ ] **Step 2:** Run; expect FAIL.
- [ ] **Step 3:** Add enum + fields + mapping + migration; enqueue event in the handler.
- [ ] **Step 4:** Run; expect PASS. Apply migration in a `docker compose` reseed and confirm
  content-service `Up`.
- [ ] **Step 5:** Commit.

### Task 1.2: Tour projection in Booking (consume catalog events)

**Files:**
- Create: `.../Booking.Service/Seadora.Booking.Domain/Entities/TourProjection.cs`
  (`TourId (pk), BranchId, TourTypeCode, AllocationModel, MinCapacity, MaxCapacity,
  RequiresGuestDetails, RequiresPassport, PayLaterAllowed, PriceFrom, Currency, UpdatedUtc`)
- Modify: `.../Seadora.Booking.Infrastructure/Persistence/BookingDbContext.cs` (add DbSet + mapping)
- Create: EF migration `AddTourProjection`
- Create: `.../Seadora.Booking.Application/Integration/TourProjectionConsumers.cs`
  (consume `TourPublished`, `TourUpdated`, `TourTypePolicyChanged`; upsert projection; idempotent)
- Modify: Content to also publish `TourPublished`/`TourUpdated` (create/update tour handlers enqueue
  events carrying capacity + resolved type policy + price-from + branch).
- Test: `.../Seadora.Booking.Application.Tests/Integration/TourProjectionConsumerTests.cs`

**Interfaces:**
- Consumes: `IdempotentConsumer` (0.4), Content events (1.1 + new `TourPublished/TourUpdated`).
- Produces: `TourProjection` read model used by availability/allocation (1.3).

- [ ] **Step 1:** Failing test: consuming `TourUpdated` upserts a `TourProjection`; re-consuming the
  same message id is a no-op (idempotent).
- [ ] **Step 2:** Run; expect FAIL.
- [ ] **Step 3:** Add projection entity + migration + consumers; add Content publishers.
- [ ] **Step 4:** Run; expect PASS. End-to-end smoke: edit a tour in admin → booking DB
  `TourProjection` row updates (via `docker compose` + RabbitMQ).
- [ ] **Step 5:** Commit.

### Task 1.3: Departures + allocation with concurrency (no overbooking)

**Files:**
- Create: `.../Seadora.Booking.Domain/Entities/Departure.cs`
  (`Id, BranchId, TourId, StartUtc, TimeSlot, Capacity, AllocationModel, Version (rowversion)`)
- Modify: `BookingDbContext.cs` (map `Departure`; configure `Version` as concurrency token)
- Create: EF migration `AddDepartures`
- Modify: `.../Seadora.Booking.Application/Bookings/Queries/GetTourAvailabilityQuery.cs` — return
  **remaining capacity** = `Departure.Capacity − Σ non-cancelled guests`, not just booked count.
- Modify: `.../Seadora.Booking.Application/Bookings/Commands/CreateBooking/CreateBookingCommand.cs` —
  in one transaction: load departure `FOR UPDATE`/optimistic `Version`, check remaining ≥ guests
  (respecting `AllocationModel`: `WholeUnit` ⇒ blocks the slot), insert booking, bump `Version`;
  on concurrency conflict, retry/`409`.
- Test: `.../Seadora.Booking.Application.Tests/Bookings/AllocationConcurrencyTests.cs` (Testcontainers
  Postgres; two concurrent bookings on a capacity-1 departure → exactly one succeeds).

**Interfaces:**
- Consumes: `TourProjection` (1.2) for capacity/allocation defaults.
- Produces: `Departure` entity; corrected availability semantics.

- [ ] **Step 1:** Failing test: capacity-1 departure, two parallel `CreateBooking` → one `Created`,
  one rejected; final booked guests == capacity (no overbooking).
- [ ] **Step 2:** Run; expect FAIL.
- [ ] **Step 3:** Add `Departure` + concurrency token + migration; rewrite availability; make
  `CreateBooking` transactional with the `Version` check + retry.
- [ ] **Step 4:** Run; expect PASS.
- [ ] **Step 5:** Commit.

### Task 1.4: Booking money breakdown + type snapshot

**Files:**
- Create: `.../Seadora.Booking.Domain/ValueObjects/Money.cs` (`Subtotal, AddonsTotal, Discount,
  TaxTotal, Total, Currency, AmountPaid, BalanceDue` — with an invariant `Total = Subtotal +
  AddonsTotal − Discount + TaxTotal` and `BalanceDue = Total − AmountPaid`)
- Modify: `.../Seadora.Booking.Domain/Entities/Booking.cs` — add `BranchId`, `CustomerId (Guid?)`,
  `TourTypeCode`; embed `Money` (owned type, jsonb or columns); replace `TripType` string usage with
  `TourTypeCode`; make `IsPaid` a computed `=> Money.BalanceDue <= 0` (drop the stored bool via
  migration).
- Modify: `CreateBookingCommand.cs` — compute `Money` from projection price + add-ons + discount +
  tax; snapshot `TourTypeCode` from `TourProjection`; enforce `RequiresGuestDetails`/`RequiresPassport`
  from the projection.
- Modify: `.../Bookings/Commands/UpdateBookingPayment/UpdateBookingPaymentCommand.cs` — set
  `AmountPaid` (money), not a boolean.
- Create: EF migration `AddBookingMoneyAndType`
- Test: `.../Seadora.Booking.Domain.Tests/MoneyInvariantTests.cs` (+ a create-booking test asserting
  the computed breakdown and required-doc enforcement).

**Interfaces:**
- Consumes: `TourProjection` (1.2).
- Produces: `Money` value object; `Booking.BranchId/CustomerId/TourTypeCode`.

- [ ] **Step 1:** Failing tests: `Money` rejects a breakdown where `Total` ≠ components;
  `BalanceDue` derives correctly; create-booking with `RequiresPassport` and no passport → rejected.
- [ ] **Step 2:** Run; expect FAIL.
- [ ] **Step 3:** Implement `Money` + entity changes + migration + command logic.
- [ ] **Step 4:** Run; expect PASS. `docker compose` reseed; confirm booking-service `Up` and a
  public booking create returns a correct money breakdown.
- [ ] **Step 5:** Commit.

**Phase 1 gate:** `dotnet build SEADORA.sln` green; booking + content test suites green; overbooking
test proves the allocation lock; editing a tour propagates to Booking's projection over RabbitMQ; a
booking persists a money breakdown and a snapshotted `TourTypeCode`.

---

# PHASES 2–6 — Scoped epics

> These phases are defined to task granularity here; their bite-sized TDD steps are expanded
> (same structure as Phases 0–1) at the start of each phase, because their detail depends on
> decisions locked by the phase before them (e.g. the Customer contract shapes Support/Finance
> links). This is a sequencing decision, not a placeholder — each task below has a concrete
> deliverable and acceptance test.

## Phase 2 — Customer (CRM) service

- **Task 2.1 — Scaffold `Customer.Service`** (4-project layout, DB `Seadora_Customer`, compose entry,
  gateway route `/api/customer/**`, fail-closed auth + `[AllowAnonymous]` allow-list, health check).
  *Accept:* service `Up`, migrations applied, `/health` 200, admin route reachable behind JWT.
- **Task 2.2 — `Customer` + `CustomerDocument` aggregates** with `BranchId`, unique email per branch,
  `MarketingConsent`/`ConsentUpdatedUtc`; documents reference FileServer, encrypted-at-rest, retention
  policy. *Accept:* CRUD + consent update tests; document access is branch+role gated.
- **Task 2.3 — Link bookings to customers.** On `BookingPlaced`, Customer service resolves/creates a
  customer by (branch, email) and stores a **BookingHistory** projection; Booking stamps the returned
  `CustomerId`. *Accept:* placing a booking creates/links exactly one customer; history row appears;
  idempotent on redelivery.
- **Task 2.4 — Admin CRM views** (customers list, profile with booking history + lifetime value).
  *Accept:* admin SPA lists/searches customers and opens a profile; a11y + dialog patterns from the
  earlier admin work reused.
- **Task 2.5 — Customer Identity & Registration API** (`POST /api/auth/register-customer` in `Identity.Service`
  with automatic Customer role assignment, `CustomerRegistered` event publishing, and WhatsApp OTP verification).
- **Task 2.6 — Customer Self-Service Portal API** (`CustomerPortalController` in `Customer.Service` for
  `/portal/me`, `/portal/bookings`, `/portal/documents`, and `/portal/profile`).
- **Task 2.7 — Website Luxury Auth Modal & State** (`AuthModal.vue` + Pinia store in `seadora-website`
  supporting Login, Register, WhatsApp OTP, and session persistence).
- **Task 2.8 — Website Customer Portal Views** (Personalized portal hub: `PortalDashboardView`,
  `PortalBookingsView`, `PortalBookingDetailView`, `PortalDocumentsView`, `PortalProfileView`, and `PortalSupportView`).
  *Detailed Spec:* `seadoratravel/docs/superpowers/plans/2026-08-28-customer-portal-and-auth.md`.

## Phase 3 — Support service (email mailbox + tickets)

- **Task 3.1 — Scaffold `Support.Service`** (DB `Seadora_Support`, compose, gateway `/api/support/**`).
- **Task 3.2 — `Ticket` + `Message` aggregates** (channel, status, priority, SLA, threading by
  email `Message-ID`), branch-scoped. *Accept:* ticket lifecycle + threading tests.
- **Task 3.3 — Inbound mailbox poller** (IMAP or Microsoft Graph / SES-inbound) → normalize →
  `Message(In)` → open/append ticket → publish `InquiryReceived`; link to Customer/Booking by email.
  *Accept:* a fetched email creates/updates a ticket; duplicate fetch is idempotent.
- **Task 3.4 — Outbound replies via Notifications;** migrate existing `ContactInquiry` (Booking) into
  Support tickets, then retire the old inquiry path. *Accept:* replying emails the customer and
  appends `Message(Out)`; legacy inquiries visible as tickets.
- **Task 3.5 — Centralize Notifications** (move SMTP + Twilio WhatsApp behind a `Notifications`
  module/service with templates + delivery status). *Accept:* booking confirmation + support reply
  both flow through Notifications with a recorded delivery status.

## Phase 4 — Concierge chatbot

- **Task 4.1 — `Concierge.Service`** with conversation sessions persisted per (branch, visitor).
- **Task 4.2 — Retrieval index over the catalog** kept fresh by Content events (reuse the projection
  pattern). *Accept:* index updates when a tour changes.
- **Task 4.3 — LLM answer with guardrails + catalog RAG**, returning suggested tours + quick replies
  (supersedes today's regex `ConciergeService`). *Accept:* grounded answers cite real tours; refuses
  out-of-scope asks.
- **Task 4.4 — Human handoff → Support ticket** with the conversation transcript; website widget
  wired. *Accept:* "talk to a human" opens a Support ticket linked to the session/customer.

## Phase 5 — Finance & ledger

- **Task 5.1 — `Finance.Service`** (DB `Seadora_Finance`, compose, gateway `/api/finance/**`).
- **Task 5.2 — PSP integration** (Stripe/Adyen) with webhooks, **idempotency keys**, charge/refund;
  `PaymentSettled`/`PaymentRefunded` events. *Accept:* a webhook settles a payment exactly once under
  retries.
- **Task 5.3 — Double-entry ledger** (`LedgerAccount`, `JournalEntry`): receivable on `BookingPlaced`,
  cash/PSP on `PaymentSettled`, supplier payable from `SupplierAgreement` net rate, commission/margin,
  tax. Every movement balances. *Accept:* trial balance nets to zero across a booking→pay→refund
  cycle.
- **Task 5.4 — Supplier settlement** (flesh out `SupplierAgreement`: net rate, commission %, terms,
  currency) + payables report. *Accept:* payables computed from agreements match ledger.
- **Task 5.5 — Revenue reporting read model** fed by Finance + Booking events; admin dashboards read
  it (retire cross-service `ReportsController` joins). *Accept:* revenue/AR/AP dashboards match ledger
  totals.

## Phase 6 — Organization / branches hardening

- **Task 6.1 — `Organization.Service`** owns `Branch`, staff↔branch, branch settings
  (currency/locale/mailbox); replaces the seeded default-branch constant. *Accept:* branches CRUD;
  Identity issues real branch claims from staff assignments.
- **Task 6.2 — Branch-aware catalog** (`Tour`/`Supplier` optional `BranchId`; null = global). *Accept:*
  a branch sees global + its own tours; cannot see another branch's private tours.
- **Task 6.3 — Cross-branch admin & reporting** (head-office role sees all branches; branch role
  scoped). *Accept:* authorization tests prove branch isolation; head office aggregates across
  branches.
- **Task 6.4 — Isolation upgrade path documented/validated** (move one branch's rows to a dedicated
  schema without caller changes). *Accept:* a rehearsal migration moves a test branch and all flows
  still pass.

---

## Self-Review

- **Spec coverage:** every §11 phase maps to a phase here; the four named business capabilities —
  bookings/inventory (Phase 1), customers (Phase 2), support mailbox (Phase 3), chatbot (Phase 4),
  finance/ledger (Phase 5), multi-branch (Phase 0 groundwork → Phase 6) — each have tasks.
- **Sequencing:** Phase 0 rails precede all event work; Booking projection (1.2) precedes allocation
  (1.3) and money (1.4) which both read it; Customer contract (2.x) precedes Support/Finance links.
- **Types are consistent** across tasks: `IEventPublisher`/`IOutboxWriter`/`IdempotentConsumer`/
  `TourProjection`/`Money`/`AllocationModel`/`Departure` are defined once and referenced by exact
  name downstream.

## Execution Handoff

**Plan complete and saved to `docs/superpowers/plans/2026-08-26-microservices-platform.md`. Two
execution options:**

**1. Subagent-Driven (recommended)** — dispatch a fresh implementer per task (via the Copilot CLI
delegate loop already used in this repo), review + re-run gates between tasks.

**2. Inline Execution** — execute tasks in this session with executing-plans, batching with
checkpoints for review.

**Which approach?**
