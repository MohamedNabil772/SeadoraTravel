# Seadora Travel — Business Cases

> End-to-end use cases derived from source and cross-checked against handlers/controllers/components. Each case lists Actor, Goal, Main flow (tied to real endpoints/handlers/views), Rules, Exceptions, and honest **Implementation status**. Companion: [BUSINESS_LOGIC.md](BUSINESS_LOGIC.md), [SOLUTION_STRUCTURE.md](SOLUTION_STRUCTURE.md), [FLOW_INTEGRITY_AND_QA.md](FLOW_INTEGRITY_AND_QA.md).

Status legend: **Implemented** · **Partial** · **Planned-only**.

---

## BC-1 · Browse tours & experiences
- **Actor:** Public visitor / customer
- **Goal:** Discover luxury tours by search, category, destination, price, language.
- **Flow:** `ToursView.vue` → `GET /api/content/api/{categories,destinations,tours}` → gateway strips `/api/content` → `ToursController.Get` → `GetToursQuery` → `TourSummaryDto`.
- **Rules:** localized by `language`; price min/max filtered in DB; text search over names/descriptions/destination/category/includes (in memory).
- **Exceptions:** backend failure → client logs and relies on already-loaded data; empty → "no tours" UI.
- **Status:** **Partial** — `startDate/endDate/destination/category` params are accepted by `GetToursQuery` but **not applied** in the handler.

## BC-2 · View tour details & reviews
- **Actor:** Public visitor / customer
- **Goal:** Inspect full details, media, itinerary, inclusions, add-ons, and feedback.
- **Flow:** `TourDetailsView.vue` → `GET /api/content/api/tours/{id}` → `GetTourByIdQuery` → `TourDto`; feedback via `GET/POST /api/booking/api/feedbacks`, `PUT .../feedbacks/{id}/visibility`.
- **Rules:** rating `0.5–5.0`; public query hides non-visible feedback unless `includeHidden=true`; **no verification the reviewer actually booked**.
- **Exceptions:** tour not found → `404`; out-of-range rating → throws.
- **Status:** **Partial** — detail + feedback implemented; booking-ownership rule not enforced. (Note: a **duplicate** `TourDetailsView.vue` exists under both `views/` and `features/tours/views/`.)

## BC-3 · Make a booking (single & multi-guest)
- **Actor:** Customer
- **Goal:** Reserve a tour with lead details and, for multiple guests, per-person identification.
- **Flow:** booking modal in `TourDetailsView.vue` → guest capture in `GuestInfoForm.vue` / `MultiGuestForm.vue` → `POST /api/booking/api/bookings` → `BookingsController.Create` → `CreateBookingCommand`.
- **Rules:** validates `TourId`, name (2–100), email regex, guest full names; sets `MissingIdentification` when passport file/number absent; stores `GuestsList`, add-ons, date, price; status `Pending`; missing ID does **not** block creation (blocks later confirmation).
- **Exceptions:** invalid input → `ArgumentException` → `400`; email/WhatsApp failures logged, booking still saved.
- **Status:** **Partial** — core booking works, but **no tour-existence check, no capacity/availability check, `TotalPrice` client-trusted, guest-count vs list not reconciled** (see [FLOW_INTEGRITY_AND_QA.md](FLOW_INTEGRITY_AND_QA.md)).

## BC-4 · Select add-ons
- **Actor:** Customer
- **Goal:** Add optional paid extras.
- **Flow:** admin defines add-ons (`TourEditor.vue`/`AddonsBuilder.vue` → `CreateTourCommand`); customer toggles in `TourDetailsView.vue`; POST includes `SelectedAddons` (`addonId/title/unitPrice/quantity`) → stored as snapshots on `Booking`.
- **Rules:** add-ons are **snapshots**, not live references; backend does **not** verify add-on IDs, recompute prices, or cap quantity.
- **Exceptions:** stale/tampered add-ons are stored verbatim.
- **Status:** **Partial** — capture/storage implemented; validation absent.

## BC-5 · Availability check
- **Actor:** Customer / concierge / admin
- **Goal:** Know booked guest count for a tour/date and avoid overbooking.
- **Flow:** `GET /api/booking/api/bookings/{tourId}/availability?date=YYYY-MM-DD` → `GetTourAvailabilityQuery` → sums non-cancelled `Guests` for that day → `{ tourId, date, bookedGuests }`.
- **Rules:** cancelled excluded; **guest count summed, not booking count**; **max capacity NOT checked**; create-booking never calls this.
- **Exceptions:** website `TourAvailabilityCalendar.vue` largely computes **synthetic** availability; admin `BookingsView.vue` computes capacity from booking count and `bookingDate` (not `tourDate`) → can misrepresent real trip capacity.
- **Status:** **Partial** — query works; **no enforcement anywhere**; UI partly faked.

## BC-6 · Concierge (conversational assistant)
- **Actor:** Public visitor / customer
- **Goal:** Ask about tours, availability, policy, payment, permits, pickup, support.
- **Flow:** `SeadoraConcierge.vue` → `POST /api/concierge/chat` → `ConciergeController` → `ProcessConciergeChatQuery` → `ConciergeService`: regex intent detection, real tour search, quick replies, optional `SuggestedTourDetails`; availability intent has a **hardcoded "Orange Bay" path** that calls Booking.
- **Rules:** policy/payment/passport answers are informational; tour search is real; availability is not generalized.
- **Exceptions:** backend failure → frontend canned replies / tour fetch fallback.
- **Status:** **Partial** — and the widget is **disabled in production** (`App.vue` renders it under `v-if="false"`) despite VIP-concierge language across the site (expectation mismatch).

## BC-7 · User authentication (OTP, social, email/password)
- **Actor:** Customer / admin user
- **Goal:** Authenticate and receive a JWT.
- **Flow:**
  - Email/password: admin `LoginView.vue` → `auth.ts` → `POST /api/auth/api/auth/login` → `LoginCommand`.
  - Register: `POST /api/auth/register` → `RegisterCommand` (assigns `Customer`).
  - WhatsApp OTP: `POST /api/auth/send-otp` → `SendWhatsAppOtpCommand` (6-digit, memory cache 5 min, mocked send); `POST /api/auth/verify-otp` → `VerifyWhatsAppOtpCommand`.
  - Social: `POST /api/auth/social-login` → `SocialLoginCommand`.
- **Rules:** admin SPA permits roles `SuperAdmin/Admin/BookingManager/OperationsManager/ConciergeSpecialist`; OTP expires 5 min; **social provider tokens are not verified**; website `AuthModal.vue` is largely UI-only (not wired to the auth store).
- **Exceptions:** invalid credentials/OTP → throw → `400`; **admin route protection may fail if gateway JWT issuer/audience ≠ Identity's**.
- **Status:** **Partial** — email/password + OTP flows work; social login unverified; website auth modal unwired; password policy/lockout/rate-limit missing.

## BC-8 · Admin content CRUD (tours, categories, destinations, languages, currencies, nationalities)
- **Actor:** Admin / content manager
- **Goal:** Manage catalog & localization master data.
- **Flow:** admin views (`features/{tours,categories,destinations,languages,currencies,nationalities}/...`) → `/api/admin/content/*` (guarded) **or** public-shaped `/api/content/api/*` → controllers → command handlers → `IContentDbContext`.
- **Rules:** tour name required; destination create/update FluentValidation; language/currency/nationality uniqueness via DB indexes; gateway admin routes require Admin/SuperAdmin, but **service controllers do not enforce auth when hit directly**.
- **Exceptions:** mismatched id → `400`; invalid validation → problem-json; Excel/PDF import-export available (`Admin\ExcelImportExportController`, `Admin\PdfCatalogController`).
- **Status:** **Implemented — with a critical security caveat**: writable via the unguarded public path (see [FLOW_INTEGRITY_AND_QA.md](FLOW_INTEGRITY_AND_QA.md)).

## BC-9 · Admin booking & status management (payment / identification gates)
- **Actor:** Admin / operations manager
- **Goal:** Review bookings; manage payment; confirm/cancel/complete; handle attendance.
- **Flow:** `BookingsView.vue` → `GET /api/booking/api/bookings`, `GET /api/content/api/tours`; actions → `PUT /api/booking/api/bookings/{id}/status|payment|attendance` → `UpdateBookingStatusCommand` / `UpdateBookingPaymentCommand`.
- **Rules:** ✅ `Confirmed` requires `IsPaid && !MissingIdentification`; ⚠️ **no gate on `Completed`**, no transition validation, payment is a bare boolean.
- **Exceptions:** none for invalid transitions — they are allowed.
- **Status:** **Partial** — confirm gate works; completion/transition/payment-detail gaps; **endpoints are publicly callable** (unauthenticated payment/status mutation).

## BC-10 · Localization / multi-language
- **Actor:** Visitor / admin / system
- **Goal:** Serve content and UI in EN/DE/IT/FR/RU (Arabic partially referenced).
- **Flow:** website `i18n.ts` loads backend translations (`GET /api/content/api/v1/languages/{lang}/translations`) and deep-merges over bundled JSON (fallback `en`); localized tour fields resolved requested → `en` → first value in `GetToursQuery`.
- **Rules:** website fallback chain works; **translation dictionary & detail/category/destination endpoints don't fully fall back server-side**; admin UI largely hardcoded English; `html lang`/`dir` not consistently updated; Arabic/RTL referenced in code but not offered in switchers.
- **Exceptions:** missing translation endpoint → silent continue with local JSON (no timeout/error surfaced).
- **Status:** **Partial** — website robust; admin not localized; RTL not ready.

## BC-11 · Feedback / reviews management
- **Actor:** Customer (submit), admin (moderate)
- **Goal:** Collect and moderate tour reviews.
- **Flow:** `POST /api/booking/api/feedbacks` → `CreateFeedbackCommand`; admin toggles visibility via `UpdateFeedbackVisibilityCommand`.
- **Rules:** rating `0.5–5.0`; visibility boolean; **no booking-ownership check, no email validation**; error text/range inconsistent ("1 and 5" vs `0.5`).
- **Status:** **Partial** — works but unverified/anti-spam gaps.

## BC-12 · Reports / dashboard
- **Actor:** Admin
- **Goal:** View operational metrics.
- **Flow:** admin `DashboardView.vue`/`ReportsView.vue` → `GET /api/booking/api/reports/dashboard` → `ReportsController` (makes synchronous HTTP calls to Content for tour data).
- **Status:** **Partial** — endpoint + cross-service read exist; report detail/metric coverage unclear; no auth on the booking service.

## BC-13 · Cash reservation auto-cleanup (system)
- **Actor:** System (background)
- **Goal:** Auto-cancel stale unpaid cash reservations.
- **Flow:** `CashReservationCleanupWorker` (hourly) → loads `Pending && !IsPaid` → `ICancellationPolicyService.IsCashReservationValid` → cancels if invalid.
- **Rules:** intended 48h-before-tour rule from `BUSINESS_RULES.md`.
- **Status:** **Partial / buggy** — wired, but keyed off `BookingDate` (creation time), not `TourDate`, so it likely cancels fresh bookings. See [BUSINESS_LOGIC.md](BUSINESS_LOGIC.md) §2.

## BC-14 · File upload (documents/images)
- **Actor:** Customer (passport docs), admin (media)
- **Goal:** Upload/serve files.
- **Flow:** `POST /api/files` → `FilesController.Upload` → `LocalStorageService` (`{Guid}{ext}`); download `GET /api/files/{fileId}`; delete `DELETE /api/files/{fileId}`.
- **Rules:** whitelist + 15 MB; extension-only validation.
- **Status:** **Implemented — insecure**: no auth on upload/delete, SVG allowed, `fileId` not validated on read/delete.

---

## Planned-only / not-wired
- **Cancellation & refunds** (tiers 72h/48h/24h) — service/factory exist, **no cancel flow calls them**.
- **Online immediate payment capture** (Stripe/PayPal) — not implemented; `IsPaid` is manual.
- **Granular RBAC permissions** — permission claims minted but not enforced at endpoints.
- **Arabic / RTL** — referenced in code, not offered to users.
