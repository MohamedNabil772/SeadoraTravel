# Seadora Travel — Flow Integrity & QA Findings

> Output of the QA/testing team (dynamic build+test verification + systematic-debugging root-cause tracing), cross-checked with the analysis team. Every item is confirmed against source. Companion: [BUSINESS_LOGIC.md](BUSINESS_LOGIC.md), [BUSINESS_CASES.md](BUSINESS_CASES.md), [SOLUTION_STRUCTURE.md](SOLUTION_STRUCTURE.md).

## 0. Test inventory — the honest state
- **Automated tests: effectively zero.** Test projects exist (`tests\Seadora.UnitTests`, `Seadora.IntegrationTests`, `Seadora.Common.Tests`, `Services\Identity\Seadora.Identity.Application.Tests`) but every file is a **placeholder `Test1()` stub**. `dotnet test` reports "2/2 pass" — those 2 are stubs. Coverage ≈ **0%**.
- **Frontend tests: none** (no Vitest/Jest/Playwright/Cypress, no `*.spec.ts`/`*.test.ts`).
- **Validators: 3 total** (`CreateDestinationCommandValidator`, `UpdateDestinationCommandValidator`, `ProcessConciergeChatQueryValidator`) — none for booking, auth, or tour commands, despite FluentValidation being wired.

## 1. Build verification (actually run)
| Component | Result |
|---|---|
| Backend `SEADORA.sln` | ✅ builds — 1 warning (CS8622 nullability, `UpdateDestinationCommandValidator`) |
| Backend tests | ✅ 2/2 (placeholder stubs) |
| seadora-admin | ✅ builds clean (1,956 modules, ~210 kB main bundle) |
| **seadora-website** | 🔴 **BUILD FAILS** |

### 🔴 BLOCKER — website does not build
`src\Web\seadora-website\src\views\ComingSoonView.vue`, ~lines **544–557**: the `.luxury-cta::before` block is missing its closing `}`, so `@keyframes sunburstPulse` becomes an orphaned block → Vite `CssSyntaxError: Unclosed block`. **Fix:** add `}` after the `transform: skewX(-20deg);` line.

Also flagged (non-blocking): deprecated `vue-i18n@9` and `lucide-vue-next`; npm audit reports 3 (website) + 2 (admin) vulnerabilities.

## 2. Authorization — end-to-end verdict 🔴 CRITICAL
**Admin / state-changing operations are not protected in practice.**

- The gateway defines a real `AdminPolicy` (`ApiGateway\Program.cs:45-52`) and applies it to `/api/admin/*` (`appsettings.json:55-89`) — **but** the public catch-alls bypass it:
  - `/api/content/{**}` → Content **without policy** → exploitable as `/api/content/api/tours`, `/api/content/api/admin/tours`, `/api/content/api/categories`, `/api/content/api/currencies`, …
  - `/api/booking/{**}` → Booking **without policy** → `/api/booking/api/bookings/{id}/status|payment|attendance`
  - `/api/files/{**}` → FileServer **public**
- Services do not close the gap: Content `AdminPolicy` is **allow-all** (`Content.API\Program.cs:31-33`, `RequireAssertion(_ => true)`); Content/Booking/FileServer configure **no authentication**; controllers have **no `[Authorize]`**.
- **Issuer/audience risk:** gateway validates `Jwt:Issuer=SeadoraIdentity` / `Jwt:Audience=SeadoraGateway` (`ApiGateway\appsettings.json:8-11`) while Identity emits/validates `SeadoraTravel` / `SeadoraTravelUsers` (`Identity.Infrastructure\DependencyInjection.cs:75-76`, `JwtTokenGenerator.cs:73-74`). If not aligned via config, gateway admin auth **fails closed**.

**Smallest correct fix (closes the whole class):** align one shared JWT config; remove/restrict the public gateway catch-alls to explicit read-only routes; require gateway `AdminPolicy` on every unsafe route; **and** configure JWT + a real `AdminPolicy` in each service with `[Authorize(Policy="AdminPolicy")]` on all POST/PUT/PATCH/DELETE/admin actions.

## 3. Overbooking race 🔴 CRITICAL
`CreateBookingCommandHandler` (`CreateBookingCommand.cs:115-150`) persists a booking with **no capacity check** and no transaction/lock. `GetTourAvailabilityQuery` (`:27-32`) is an advisory read-only sum and is **not called** during creation. `Tour.MaxAllocations` lives in Content (`Tour.cs:33`) with no atomic boundary to Booking. → Two concurrent creates can exceed capacity.
**Fix:** a single capacity guard in `CreateBookingCommandHandler`, inside a serializable transaction or per-tour/date lock, before `_context.Bookings.Add`.

## 4. Payment / identification / status gates 🔴
`UpdateBookingStatusCommand` enforces the payment + identification gate **only for `Confirmed`** (`:40-52`); `Completed` has no gate and any status can be assigned directly (`:54-55`), including backwards (`Completed → Pending`). Because the endpoints are public (§2), anyone can drive these transitions and even mark a booking paid via `UpdateBookingPaymentCommand` (`:31`).
**Fix:** a central status-transition validator requiring `IsPaid && !MissingIdentification` for both `Confirmed` and `Completed`, plus allowed-transition checks; and authorize the endpoints.

## 5. Pricing integrity 🟠 HIGH
`CreateBookingCommand` stores the **client-supplied `TotalPrice`** (`:131`) and **client-supplied add-on prices/quantities** (`:134`) verbatim — no server-side recompute against Content. A tampered request underpays.
**Fix:** recompute price server-side from tour + validated add-ons; ignore client totals.

## 6. Auth hardening 🟠 HIGH
- **Register:** no duplicate-email pre-check, generic `Exception`, Identity email index not unique, `RequireUniqueEmail` not set. (`RegisterCommand.cs:24-29`, `DependencyInjection.cs:53-55`)
- **Login:** generic exceptions; **lockout disabled** (`CheckPasswordSignInAsync(..., false)`). No password policy configured.
- **WhatsApp OTP:** no rate limit / attempt counter; **non-crypto `Random`**; **OTP value is logged**; process-local `IMemoryCache` (breaks on restart / scale-out). (`SendWhatsAppOtpCommand.cs:25-33`, `VerifyWhatsAppOtpCommand.cs:38-45`)
- **Social login:** provider `IdToken` **not verified** server-side. (`SocialLoginCommand.cs:39-90`)
**Fix:** `RequireUniqueEmail`, explicit password + lockout policy, per-phone/IP OTP rate limits + attempt counters + `RandomNumberGenerator` + never log codes, and server-side provider-token verification.

## 7. File upload 🟠 HIGH
Direct output-path traversal from `file.FileName` is **refuted** (storage generates `{Guid}{ext}`, `LocalStorageService.cs:25-26`). Confirmed risks: **no auth** on upload/delete (`FilesController.cs:17-18,52-53`); **`fileId` unsanitized** on read/delete before `Path.Combine` (`LocalStorageService.cs:38,46`); **public SVG upload** allowed (`:23`).
**Fix:** require auth; validate `fileId` matches `{guid}.{allowedExt}`; block or sanitize SVG.

## 8. Localization fallback 🟡 MEDIUM
Backend tour **list** falls back locale → `en` → first → empty (`GetToursQuery.cs:76-86`), but `GetTranslationsQuery` returns only requested-language keys (missing keys omitted, `:24-31`) and detail/category/destination queries return raw dictionaries with no server fallback. Frontend compensates in several helpers (inconsistent).
**Fix:** one shared server-side locale resolver used by all localized DTOs and the translations endpoint.

## 9. Cash-cleanup date bug 🟡 MEDIUM
`CashReservationCleanupWorker` + `CancellationPolicyService` key off `booking.BookingDate`, but that field is set to **creation time** (`CreateBookingCommand.cs:127-136`), not `TourDate` → the worker likely cancels **fresh** unpaid bookings and refund tiers compute against the wrong date.
**Fix:** use `TourDate` for all deadline/refund math; store payment method to scope the cash rule.

## 10. Other loose ends
- **CORS `AllowAnyOrigin/Method/Header`** in Content (`Program.cs:15-19`).
- **Guest count vs `GuestsList`** not reconciled (`Guests=5`, list of 3 accepted).
- **Feedback** accepts any `TourId`, no booking-ownership/email checks; range text mismatch (`0.5` vs "1–5").
- **Refund processor factory / permission matrix** are dead/ahead-of-need code.
- **Duplicate frontend components** (`TourDetailsView.vue`, `Trips.vue`, `PdfPreviewModal.vue`) cause drift.
- **Notification failures** swallowed with logging only (acceptable, but no retry/outbox).
- **`TourId` existence** never validated at booking time (possible silent FK/orphan).

## 11. Frontend UX / accessibility (design-eng + UI/UX review)
Grades: design-system **C+**, accessibility **C-**, responsive **B-**.
- **P0 motion:** 600ms route fade/scale on every navigation makes the app feel slow (`App.vue:14-35`) — cut to ~180–220ms opacity.
- **P0 modals:** public/admin modals lack `role="dialog"`, focus trap, Esc, return-focus (`AuthModal.vue`, `TourDetailsView.vue`, `CategoryModalForm.vue`).
- **P0 errors:** failures use native `alert()` or silent `console.error` (`ToursView.vue`, `Trips.vue`) — replace with inline/toast + retry, preserving input.
- **P0 tokens:** `DESIGN.md` says "no hardcoded hex" but ~2,601 color/layout literals across 53 files; gold text (`#c9a84c`/`#D4AF37`) often fails WCAG AA contrast.
- **A11y:** labels not associated via `for/id`; clickable `<div>`s break keyboard nav; icon-only buttons lack `aria-label`; touch targets unaudited; `html lang/dir` not updated on language change.
- **Unwired luxury features:** login button hidden (`Navbar.vue`), concierge disabled (`App.vue v-if="false"`), `ComingSoon` CTA navigates nowhere — expectation mismatch with the premium positioning.
- **Reduced motion:** website has global CSS guard, but JS/spring motion and admin ignore `prefers-reduced-motion`.
- **Structure:** deduplicate the twin `TourDetailsView`/`Trips`/`PdfPreviewModal`; extract bloated views (`TourDetailsView.vue` ~93 kB).

## 12. Priority fix list (recommended order)
| # | Item | Severity | Where |
|---|---|---|---|
| 1 | Website build blocker (unclosed CSS) | 🔴 blocker | `ComingSoonView.vue:544-557` |
| 2 | Real authorization on admin/state-changing ops | 🔴 critical | gateway + all services |
| 3 | Overbooking capacity guard + transaction | 🔴 critical | `CreateBookingCommand` |
| 4 | Payment/ID gate on Completed + transition validator | 🔴 critical | `UpdateBookingStatusCommand` |
| 5 | Server-side price recompute (ignore client totals) | 🟠 high | `CreateBookingCommand` |
| 6 | Auth hardening (unique email, lockout, OTP rate-limit, verify social tokens) | 🟠 high | Identity |
| 7 | File upload auth + `fileId` validation + SVG handling | 🟠 high | FileServer |
| 8 | Cash-cleanup/refund date bug (`TourDate`) | 🟡 med | Booking policy |
| 9 | Shared localization fallback resolver | 🟡 med | Content |
| 10 | Minimum viable test suite for #2–#6 | 🟡 med | tests\ |
| 11 | Frontend UX/a11y P0s (modals, errors, tokens, motion) | 🟡 med | both SPAs |

### Smallest test set that would catch the top risks
Overbooking (concurrent creates vs capacity) · admin-endpoint authorization (401/403) · booking email validation · register duplicate-email + password policy · OTP rate-limit · payment/identification confirmation gate. No frameworks beyond the already-installed xUnit + FluentValidation.
