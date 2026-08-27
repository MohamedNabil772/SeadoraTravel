# Phase 3 — Finance & Reporting Platform

**Status:** planning (awaiting sign-off)
**Owner:** Coordinator (orchestrator) + delegated implementers
**Depends on:** Phase 0–2 complete (Identity RBAC, Booking Money + outbox, Customer/CRM, messaging kernel)
**Constraints (locked):** microservice architecture; code-setup only (no docker, no live-DB migration, no
token generation at runtime — migration *generation* is allowed); additive only — must NOT break the
public website, admin SPA, email/HTML/WhatsApp templates; generic/data-driven so a new agency deploys
by config; every query branch-scoped.

---

## 1. Why (goal)

Give the business a real financial system with two audiences:

- **Business Owner** — high-signal **dashboards**: revenue, profit, growth, cash position, margins,
  per-branch and per-tour performance. Read-only, all branches.
- **Accountant** — **sufficient, detailed financial reports** and the operational tools behind them:
  general ledger / journal, trial balance, accounts-receivable aging, supplier payables/settlements,
  receipts & payments subledger, refunds, tax collected, P&L — all filterable and exportable.

Both roles must be first-class in RBAC, gated by permissions, and see data that is **correct** (from
the actual booking money + recorded payments), not recomputed guesses.

## 2. As-is (what exists today, and the gaps)

| Area | Today | Gap |
|---|---|---|
| Reports | `Booking.API/Controllers/ReportsController.cs` — `dashboard`, `supplier`, `customers`, `ledger` | Revenue is recomputed from `tour.Price` fetched over **HTTP from Content** (hardcoded URLs), **ignores** the booking's real `Money` (guests, addons, discount, tax, amount paid). Loads **all** bookings + tours into memory per call. "Ledger" is a derived view, **not persisted**. No branch scoping, mixes currencies, no role gating. |
| Money | `Booking.Domain/ValueObjects/Money.cs` — Subtotal, AddonsTotal, Discount, TaxTotal, Total, Currency, AmountPaid, BalanceDue | Solid foundation. Not yet the source of report revenue. |
| Payments | `UpdateBookingPaymentCommand` flips `IsPaid` bool + mirrors `Money.AmountPaid` to full/zero | No partial payments, no method/reference, no receipts, no audit trail. |
| Suppliers | `Content.Domain/{Supplier,PaymentAgreement}` — percentage + agreement name/frequency | No accrued payable, no settlement periods, no paid/unpaid tracking. |
| Cancellation/Refund | `Booking.Domain/Services/{CancellationPolicyService, Refunds/RefundProcessorFactory}` | Not reflected in any ledger. |
| RBAC | Identity: `Role`/`Permission`/`RolePermission` + `IdentitySeeder`. Permission IDs are `Module.Action` (e.g. `Bookings.View`). Roles: SuperAdmin, Admin, OperationsManager, ConciergeSpecialist, Customer. Admin manages RBAC via `RolesView.vue` (data-driven). | No **Accountant** / **BusinessOwner** roles, no `Finance.*` permissions. |
| Events | `BookingPlaced` published from Booking via outbox (Task 2.3); messaging/outbox/idempotency kernel in `Seadora.Common` | No payment/refund/revenue events; no finance consumer. |

**Decision:** we replace the naive in-Booking reporting with a dedicated **`Finance.Service`**
microservice that owns a persisted ledger + payments subledger + report/dashboard read-models, fed by
domain events. This matches the locked microservice direction and removes the cross-service HTTP
recompute. The old `ReportsController` stays alive read-only during transition, then the admin UI is
pointed at Finance and the old controller is retired (Task 3.9).

## 3. Non-goals (ponytail — explicitly out of scope)

- No full tax engine (we **store** TaxTotal from the booking; we do not compute jurisdictional tax).
- No bank/payment-gateway integration or automated bank-feed reconciliation (manual receipt entry +
  manual "reconciled" flag only). `// ponytail:` upgrade path noted in the Payment entity.
- No payroll / general accounts-payable beyond **supplier settlements**.
- One **reporting currency** for consolidated figures, using an **FX-rate snapshot per transaction**
  (seeded/manual `CurrencyRate` table). Pluggable rate provider later.
- Not full IFRS revenue recognition — we recognize revenue at booking placement (accrual) and track
  collected separately (cash). Good enough for an agency; upgrade path noted.

## 4. Personas & access model (RBAC)

New roles (seeded, data-driven — assignable/editable in the existing admin RolesView):

| Role | Finance permissions | Scope |
|---|---|---|
| **BusinessOwner** | `Finance.ViewDashboard`, `Finance.ViewReports` | Read-only, all branches |
| **Accountant** | `Finance.ViewDashboard`, `Finance.ViewReports`, `Finance.ManagePayments`, `Finance.PostAdjustments`, `Finance.Reconcile`, `Finance.Export` | Operate finance, all branches (or assigned branch) |
| **Admin / SuperAdmin** | inherit all `Finance.*` | — |

New `Finance.*` permissions (module `Finance`): `ViewDashboard`, `ViewReports`, `ManagePayments`,
`PostAdjustments`, `Reconcile`, `Export`. Enforced as **authorization policies** on the Finance service
(not just the blanket AdminPolicy). Requires the JWT to carry the caller's permissions (or roles the
Finance service maps to permissions) — verified/added in Task 3.1.

## 5. Target architecture

```
 Booking.Service ──(outbox)──▶ BookingRevenueRecognized ─┐
                              PaymentRecorded* ───────────┤        ┌─ JournalEntry/Line (double-entry, immutable)
                              BookingCancelled/RefundIssued┼──▶ Finance.Service ┼─ Payment subledger (partial, method, ref)
 (Finance owns payment capture; Booking consumes           │  (consumers post   ├─ SupplierSettlement (accrued payable)
  PaymentRecorded to keep IsPaid + website gate correct)    │   balanced entries ├─ CurrencyRate (FX snapshot)
                                                            │   + read-models)   └─ Read-models: RevenueDaily,
 ApiGateway  /api/finance/**  +  /api/admin/finance/**  ────┘                        ArAging, BookingFinancialSnapshot,
 (permission-gated policies)                                                          SupplierPayable, DashboardKpis
```

**Finance.Service** (4 projects, mirrors Customer scaffold): Domain, Application (CQRS + consumers +
report queries), Infrastructure (EF + messaging + outbox + idempotency), API (controllers +
permission policies + `/health`). Own Postgres DB `Seadora_Finance`. Listens `:8080`; gateway cluster
`http://finance-service:8080`.

**Payment ownership decision (recommended default):** Finance owns the **payments subledger** (accountants
record receipts with amount/method/reference; supports partial). On each receipt Finance emits
`PaymentRecorded`; **Booking consumes it** and sets `IsPaid = (cumulative paid ≥ Total)` + updates
`Money.AmountPaid`, so the existing confirm-gate and website stay correct and untouched in behavior.
**DECISION (locked 2026-08-27):** Finance owns payments. Cash receipts recorded via admin now; the future
online payment gateway's webhook calls the same Finance record-payment entry point → same `PaymentRecorded`
→ same Booking sync. Booking stays system-of-record for `IsPaid`; website + admin confirmation flows
unchanged. The existing `UpdateBookingPayment` endpoint stays live until Task 3.8/3.9 rewire admin to
Finance (no window where admin payment breaks).

## 6. Finance domain model

- **LedgerAccount** — chart of accounts, seeded: `Revenue`, `SupplierCostExpense`, `SupplierPayable`,
  `TaxPayable`, `Cash/Bank`, `Discounts`, `Refunds`, `AccountsReceivable`. (Id, Code, Name, Type:
  Asset/Liability/Income/Expense, Normal side.)
- **JournalEntry** (immutable, append-only) + **JournalLine** (AccountId, Debit, Credit, currency,
  reporting-currency amount, FX rate). Invariant: **Σ debits == Σ credits** (enforced in the factory;
  unit-tested). Source ref (event id, booking id, branch id, occurred date). Corrections via reversing
  entries — never mutate.
- **Payment** — BookingId, BranchId, CustomerId?, Amount, Currency, Method (Cash/Card/Bank/Other),
  Reference, ReceivedUtc, ReconciledUtc?, CreatedBy. `// ponytail:` bank-feed auto-match later.
- **SupplierSettlement** — SupplierId, BranchId, period (from agreement frequency), AccruedAmount,
  PaidAmount, Status. Accrued from `BookingRevenueRecognized` using the supplier percentage snapshot.
- **CurrencyRate** — (FromCurrency, ToCurrency=reporting, Rate, AsOfUtc). Seeded/manual. Journal lines
  snapshot the rate used.
- **Read-models (projections, pre-aggregated for fast reports/dashboards):**
  `BookingFinancialSnapshot` (per booking: gross, discount, tax, net, supplierCost, margin, paid, due,
  status, branch, tourId, tourTypeCode, date), `RevenueDaily` (per branch/day: recognized, collected,
  refunds, supplierCost, margin), `ArAging` view (derived from snapshots), `SupplierPayable` (per
  supplier/period), `DashboardKpis` (rolling aggregates). Rebuildable from journal + events.

## 7. Event contracts (in `Seadora.Contracts`, dependency-free)

- **`BookingRevenueRecognized`** — published by Booking on create (full breakdown so Finance never calls
  back): BookingId, BranchId, CustomerId?, TourId, TourTypeCode?, Subtotal, AddonsTotal, Discount,
  TaxTotal, Total, Currency, SupplierId?, SupplierPercentage, OccurredUtc. (Distinct from `BookingPlaced`,
  which stays for CRM. Adding it is additive; existing consumers unaffected.)
- **`PaymentRecorded`** — published by Finance on each receipt: PaymentId, BookingId, BranchId, Amount,
  Currency, CumulativePaid, BookingTotal, Method, ReceivedUtc. Consumed by Booking (sync IsPaid/Money).
- **`BookingCancelled`** / **`RefundIssued`** — published by Booking: BookingId, BranchId, RefundAmount,
  Currency, Reason, OccurredUtc. Consumed by Finance (reverse accruals, post refund).

All consumers idempotent via the existing `IIdempotentConsumer` (keyed on `evt.Id`). Booking already has
outbox (Task 2.3); Finance gets outbox + idempotency wired at scaffold.

## 8. Reports catalogue (Accountant — detailed, permission `Finance.ViewReports`/`Export`)

Each is paged + filterable by **date range, branch, currency**, served from Finance read-models/journal
(no cross-service HTTP), CSV/Excel export:

1. **General Ledger / Journal** — every journal entry & line, filter by account.
2. **Trial Balance** — debit/credit balance per account for a period.
3. **Profit & Loss** — revenue − discounts − supplier cost − refunds = net; by period/branch/tour-type.
4. **Revenue report** — recognized vs collected, by period / tour / tour-type / branch.
5. **Accounts Receivable aging** — unpaid/partial bookings bucketed 0–30/31–60/61–90/90+.
6. **Supplier payables / settlement** — per supplier per agreement period: accrued vs paid, due.
7. **Receipts & payments** — payments subledger, by method/date, reconciled flag.
8. **Refunds report** — refunds by period/reason/tour.
9. **Tax collected** — TaxTotal aggregated by period/branch.

## 9. Owner dashboard (permission `Finance.ViewDashboard`)

KPI cards + trend charts (Chart.js already used in the admin design system):

- Revenue (recognized & collected), Net profit, Gross margin %, Outstanding AR, Cash collected.
- Growth: MoM / YoY, period-over-period deltas.
- Trend: revenue & margin by day/week/month/quarter.
- Top tours & tour-types by revenue and by margin.
- Supplier cost & margin split.
- Bookings count, average booking value, cancellation & refund rate.
- Per-branch breakdown (multi-branch aware).

## 10. Task breakdown

> Delegated per the established loop (brief → dispatch Copilot implementer → I re-run all gates → commit).
> Skills: **ponytail** + **superpowers/TDD** for backend; **ui-ux-pro-max** + **emil-design-eng** for 3.8 UI.
> Gates every task: `dotnet build SEADORA.sln` 0/0; `dotnet test tests/Seadora.UnitTests`; `dotnet test
> tests/Seadora.Common.Tests` 7/7; plus `npm run build` in seadora-admin for UI tasks.

| # | Task | Depends on | Skills |
|---|---|---|---|
| **3.1** | **Identity RBAC**: seed `Accountant`+`BusinessOwner` roles + `Finance.*` permissions; ensure JWT carries permissions/roles; migration. No UI change (RolesView is data-driven). TDD: seeding idempotent, role→permission mapping. | — | ponytail, superpowers |
| **3.2** | **Scaffold Finance.Service** (4 projects, `Seadora_Finance` DB, gateway `/api/finance/**` + `/api/admin/finance/**` with `Finance.*` policies, compose entry, sln). Mirror Customer 2.1. | 3.1 | ponytail |
| **3.3** | **Finance domain**: chart of accounts (seeded), JournalEntry/Line (balanced invariant), Payment, SupplierSettlement, CurrencyRate, read-model tables; `InitialCreate` migration. TDD: balanced-entry invariant, FX conversion, AR balance math. | 3.2 | ponytail, superpowers |
| **3.4** | **Event contracts + Booking publisher**: add `BookingRevenueRecognized`, `PaymentRecorded`, `BookingCancelled`/`RefundIssued`; Booking publishes revenue+cancel via outbox; Booking consumes `PaymentRecorded` → sync `IsPaid`/`Money` (keeps confirm-gate + website correct). TDD. | 3.3 | ponytail, superpowers |
| **3.5** | **Finance consumers + posting engine**: consume revenue/payment/refund events → post balanced journal entries + update read-models + accrue supplier settlements (idempotent). TDD posting rules per event. | 3.4 | ponytail, superpowers |
| **3.6** | **Reports API** (accountant): the 9 reports (§8), paged, date/branch/currency filters, permission-gated, CSV/Excel export — from read-models/journal. TDD query correctness. | 3.5 | ponytail, superpowers |
| **3.7** | **Dashboard API** (owner): KPI + trend aggregates (§9), permission-gated. TDD aggregates. | 3.5 | ponytail, superpowers |
| **3.8** | **Admin Finance UI**: Finance section — Owner **Dashboard** (KPI cards + Chart.js trends) + Accountant **Reports** views (tables, filters, exports) + **Payments** management. RBAC-gated nav/routes. Mirror existing luxury design system. | 3.6, 3.7 | ui-ux-pro-max, emil-design-eng, ponytail |
| **3.9** | **Retire old reporting**: point admin ReportsView at Finance; remove/deprecate Booking `ReportsController` + its cross-service HTTP; confirm website/email untouched. | 3.8 | ponytail |

## 11. Risks & mitigations

- **Confirm-gate / website regression** (payments move to Finance): Booking remains system-of-record for
  `IsPaid`; Finance only *drives* it via `PaymentRecorded`. Behavior of the existing gate is preserved and
  covered by existing tests — re-run them every task.
- **Mixed currencies in consolidated figures**: FX snapshot per journal line; reports show both
  transaction and reporting currency. Single reporting currency per deployment (config).
- **Double-counting on redelivery**: all consumers idempotent (`evt.Id`); journal entries carry the
  source event id with a unique guard.
- **Data migration**: dummy data — read-models are rebuildable by replaying events / a one-off backfill
  command; no production data at risk.
- **Multi-company**: Finance is generic + branch-scoped; chart of accounts + reporting currency seeded
  from config, so a new agency deploys unchanged.

## 12. Definition of done (phase)

Accountant logs in → sees all 9 reports with correct, branch-scoped, exportable figures sourced from a
persisted ledger. Business owner logs in → sees dashboards with revenue/profit/growth/margins/AR/cash.
Non-finance roles see no finance menu. All gates green; website, admin (non-finance), email/WhatsApp
templates unchanged.
