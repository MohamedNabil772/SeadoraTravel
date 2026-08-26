# Seadora Travel — Business Logic

> What the backend **actually enforces** today, service by service, verified against source. A ⚠️ marks a rule that is documented/modelled but **not enforced**, or an over-engineering/leanness note from the ponytail audit. Companion: [BUSINESS_CASES.md](BUSINESS_CASES.md), [SOLUTION_STRUCTURE.md](SOLUTION_STRUCTURE.md), [FLOW_INTEGRITY_AND_QA.md](FLOW_INTEGRITY_AND_QA.md).

## Legend
- ✅ **Enforced** in a handler/domain rule.
- 🟡 **Partial** — implemented but incomplete or bypassable.
- ⚠️ **Not enforced / draft** — exists as config, doc, or dead code only.

---

## 1. Identity Service

### Enforced
- **User model** extends ASP.NET Identity with `FirstName/LastName/FullName`, social IDs (`GoogleId/FacebookId/AppleId`), `AvatarUrl`, timestamps, `Roles`. (`Domain\Entities\User.cs`)
- **Roles & permissions**: `Role : IdentityRole<string>` with `RolePermissions`; permissions are rows keyed like `Tours.Create`. Seeded roles: `SuperAdmin, Admin, OperationsManager, ConciergeSpecialist, Customer`; SuperAdmin gets all permissions. (`Role.cs`, `Permission.cs`, `IdentitySeeder.cs`)
- **Registration** ✅ creates user from email/name/password, assigns `Customer`, returns JWT. (`RegisterCommand.cs:24-35`)
- **Login** ✅ email lookup + password check, returns JWT with current roles. (`LoginCommand.cs:25-36`)
- **JWT** contains subject/email/name/firstName/lastName/role; SuperAdmin gets `permission=*`, others get per-role permission claims; **fixed 60-minute expiry**. (`JwtTokenGenerator.cs:31-75`)
- **`/auth/me`** ✅ requires a valid JWT. `UsersController` requires `[Authorize(Roles="Admin")]` (note: **Admin**, not SuperAdmin). (`AuthController.cs:87-109`, `UsersController.cs:15`)

### WhatsApp OTP 🟡
- Generates a 6-digit code with **`new Random()`** (non-cryptographic), stores in **`IMemoryCache`** for 5 min under `OTP_{phone}`, and **logs the OTP**; delivery is mocked, not real Twilio. Verify checks cache equality, then finds/creates a phone user and issues a JWT + a dummy refresh token. (`SendWhatsAppOtpCommand.cs:25-48`, `VerifyWhatsAppOtpCommand.cs:36-88`)
- ⚠️ **No rate limiting**, no attempt counter, single-process cache (breaks under scale-out / restart).

### Social login 🟡
- Requires only `Email`; **does not verify the provider `IdToken`** with Google/Facebook/Apple. New users are email-confirmed and assigned `Customer`. (`SocialLoginCommand.cs:39-90`) — ⚠️ trust-boundary hole.

### Ponytail / leanness notes
- ⚠️ **Permission matrix is ahead of enforcement** — rich permission claims are minted, but backend authorization mostly uses roles or nothing. Upgrade path: enforce `permission` policies, or drop the matrix until used.
- ⚠️ **Bug**: JWT `name` claim is the literal string `"user.FirstName user.LastName"` instead of interpolated values. (`JwtTokenGenerator.cs:37`)
- ⚠️ **No password policy / lockout configured** — only `AddIdentity<User,Role>()`; login uses `CheckPasswordSignInAsync(..., lockoutOnFailure:false)`. (`DependencyInjection.cs:53-55`, `LoginCommand.cs:30`)
- ⚠️ Register does **not** pre-check duplicate email and throws a generic `Exception("Registration failed")`; Identity email index is not unique. (`RegisterCommand.cs:24-29`)

---

## 2. Booking Service — the core domain

### Domain model
- `Booking` (mutable EF entity): `TourId`, customer contact, optional hotel/passport/trip fields, `TourDate`, `Guests`, `TotalPrice`, `Language`, `MissingIdentification`, `SelectedAddons` (jsonb), `GuestsList` (jsonb). Defaults `Status=Pending`, `IsPaid=false`, `Attendance="Pending"`. (`Booking.cs:6-33`)
- `GuestDetail`: `HasIdentification` derived from passport file **or** number; age category defaults `Adult`. (`GuestDetail.cs:5-17`)
- **No DB constraints** for capacity, guest count, payment, or identification — all invariants live in handler code (or don't). (`BookingDbContext.cs:17-27`)

### Create booking ✅ (with gaps)
Validated in `CreateBookingCommand.cs`:
- ✅ `TourId` non-empty; customer name required, length **2–100**; email required + **regex-valid**. (`:47-72`)
- ✅ Each provided guest must have `FullName`. (`:77-84`)
- ✅ **Identification flag**: a guest is "identified" if passport file **or** number present; any missing → booking `MissingIdentification = true`. With no guest list, missing main passport file → flag set. (`:86-90`, `:106-112`)
- 🟡 **Guest count rule**: if `request.Guests > 0` that wins; else guest-list count or `1`. **No rule enforces `Guests == GuestsList.Count`** — a mismatch (e.g. `Guests=5`, list of 3) is accepted. (`:127-135`)
- Booking saved as `Pending`; **`BookingDate` = creation time**, not travel date. (`:115-138`)
- Side effects (best-effort, non-transactional): internal notification, WhatsApp confirmation (if number), email receipt (always attempted); failures are logged and **do not roll back**. (`:140-172`)

⚠️ **Not enforced at create time** (all confirmed by flow tracing):
- ❌ **No availability / capacity check** → overbooking is possible (see below).
- ❌ **No `TourId` existence check** against Content.
- ❌ **Add-ons taken as-is** — no validation of add-on IDs against the tour, no server-side price recompute, no quantity check. (`:134`)
- ❌ **`TotalPrice` is client-supplied and trusted** — a tampered price is stored verbatim. (`:131`) — pricing-integrity risk.

### Availability 🟡 (advisory only)
- `GetTourAvailabilityQuery` returns the **sum of booked guests** for a `TourId` on a given UTC day, excluding `Cancelled`. It does **not** read `Tour.MaxAllocations`, does not reserve seats, and does not block anything. (`GetTourAvailabilityQuery.cs:22-32`)
- ⚠️ **Ceiling**: capacity lives in Content (`Tour.MaxAllocations`), bookings are written in Booking, and there is **no atomic boundary** between the two. Two concurrent creates can exceed capacity. Upgrade path: a transactional capacity check inside `CreateBookingCommandHandler` (serializable transaction or per-tour/date lock) using a canonical capacity value.

### Status & payment gates 🟡
- `UpdateBookingPaymentCommand` only toggles the `IsPaid` boolean — no amount, provider, method, deadline, or transaction id. (`UpdateBookingPaymentCommand.cs:21-35`)
- ✅ `UpdateBookingStatusCommand` blocks transition to **`Confirmed`** unless `IsPaid == true` **and** `MissingIdentification == false`. (`:40-52`)
- ⚠️ **No gate on `Completed`**, and **no status-transition state machine** — any status can be set directly, e.g. `Pending → Completed` or backwards `Completed → Pending`. (`:54-55`)
- On `Confirmed`: WhatsApp + templated email confirmation. On `Completed`: feedback-request email + WhatsApp. Failures logged only. (`:57-115`)

### Cash cleanup / cancellation / refunds
- ✅ `CashReservationCleanupWorker` runs **hourly**, loads `Pending && !IsPaid` bookings, and cancels those where `ICancellationPolicyService.IsCashReservationValid` is false. (`CashReservationCleanupWorker.cs:30-69`, `DependencyInjection.cs:28-35`)
- ⚠️ **Bug**: both cash validity and refund tiers key off `booking.BookingDate`, but `BookingDate` is **creation time**, not the scheduled `TourDate` → the worker likely **cancels fresh unpaid bookings** and refund math is wrong. (`CancellationPolicyService.cs:17-45` vs `CreateBookingCommand.cs:127-136`)
- ⚠️ **Dead code**: `RefundProcessorFactory` + refund processors exist but **no cancellation endpoint/handler calls them**. Ponytail: delete until a real cancel/refund flow exists. (`RefundProcessorFactory.cs:7-62`)

### Feedback 🟡
- Accepts **any** `TourId`, rating `0.5..5.0`, free text/name/email; **no booking-ownership check** and no email validation. Error text says "between 1 and 5" while code allows `0.5`. Visibility is a boolean toggle. (`CreateFeedbackCommand.cs:29-50`, `UpdateFeedbackVisibilityCommand.cs:21-34`)

### BUSINESS_RULES.md cross-check
The documented policies in `seadoratravel\BUSINESS_RULES.md` are explicitly **DRAFT**. Reality:
| Documented rule | Status |
|---|---|
| Cash payment must be confirmed 48h before booking date, else auto-cancel | ⚠️ **Partly wired but miswired** — worker exists, but there is no payment-method concept and it uses creation date, not tour date |
| Online payment immediate capture | ⚠️ **Not enforced** — `IsPaid` is a manual boolean toggle |
| Cancellation refund tiers (72h/48h/24h) | ⚠️ **Modelled but not wired** — `CancellationPolicyService` exists, no cancel flow invokes it, and it uses the wrong date |

---

## 3. Content Service

### Enforced
- **`Tour`** is a large content aggregate: localized `Names/Descriptions/Highlights`, price/currency, destination/category/type/supplier, capacity fields (`MaxAllocations`, `GroupMinCapacity`, `GroupMaxCapacity`), marketing flags, pickup config, packages, itinerary, inclusions/exclusions, FAQs, add-ons, media. (`Tour.cs:3-72`)
- **Create tour** ✅ requires non-empty `Names`; if `DestinationId`/`CategoryId` are empty it **silently picks the first** destination/category. Defaults: currency `EUR`, max allocations `20`, group min `1`/max `20`, pickup `FixedSlots`. (`CreateTourCommand.cs:68-179`)
- **Update tour** is patch-like but value-based: price/rating/review-count only update when `> 0`; booleans always overwrite; supplier id always overwritten. (`UpdateTourCommand.cs:69-105`)
- **Public tour search** loads tours then filters **text in memory** (names/descriptions/destination/category/includes); price filters run in DB. Localized display falls back requested-locale → `en` → first value. (`GetToursQuery.cs:34-99`)
- **Uniqueness** via DB indexes: translation `(Key,Namespace)`, language code, currency code, nationality code, tour-type code. (`ContentDbContext.cs:26-35,112-115`)
- **Languages**: creating a default language clears other defaults first. (`CreateLanguageCommand.cs:19-47`)
- **Currencies**: create, set-base (clears other bases, rate→1), manual-rate (`IsManualRate=true`), live-sync (updates `ExchangeRate` only when not manual). (`Create/SetBase/UpdateCurrencyRate/SyncLiveExchangeRates`)
- **Nationalities**: code uppercased/trimmed on create. (`CreateNationalityCommand.cs:18-33`)
- **Excel import/export** for `tours/destinations/categories` (`.xlsx`, Kestrel 100 MB). Tour import matches by **English title**, defaults currency/duration/emoji, falls back to first category/destination on no match. Separate translation Excel service handles `Names/Descriptions/Highlights` for hardcoded locales `en/ar/de/fr/it/es/ru`. (`ExcelImportExportController.cs`, `ExcelLocalizationService.cs`)

### Ponytail / leanness notes
- ⚠️ **`Tour` is doing too much** — CMS + pricing + inventory hints + supplier terms + media + itinerary + add-ons + localization in one JSON-heavy blob. Fine if the admin UI needs all fields, but many are **catalog metadata, not enforced policy**.
- ⚠️ **Capacity/cancellation/payment flags** (`MaxAllocations`, `ReserveAndPayLater`, `FreeCancellation`, package capacity) are **display/config only** — Booking never consumes them.
- ⚠️ **Two overlapping Excel systems** (entity import/export vs translation template) — duplication to consolidate.
- 🟡 **Localization dictionary endpoints don't fully fall back**: `GetTranslationsQuery` only returns keys for the requested language (missing keys omitted, not filled from English); `GetTourByIdQuery`/`GetCategoriesQuery`/`GetDestinationsQuery` return raw dictionaries without server-side locale fallback (frontend compensates).

---

## 4. File Server

### Enforced
- ✅ Rejects null/empty files; **extension whitelist** (`.jpg .jpeg .png .webp .svg .pdf .xlsx .xls .csv`); **15 MB** max; stores as `{Guid}{ext}` under configured path (default `uploads`); download content-type inferred from extension (fallback `application/octet-stream`). (`FilesController.cs:17-76`, `LocalStorageService.cs:18-52`)

### Notes
- ⚠️ **No auth** on upload/delete (see [FLOW_INTEGRITY_AND_QA.md](FLOW_INTEGRITY_AND_QA.md)).
- ⚠️ Extension-only validation — MIME/content is trusted; **SVG upload allowed** (stored-XSS vector if served inline). Upgrade path: magic-byte sniffing + SVG sanitization.
- ⚠️ `RemoteStorageService` abstraction is unused by FileServer itself (always local) — keep only if another consumer needs it.

---

## 5. Business-logic summary (cross-cutting)

1. **The single strongest real business gate** is booking confirmation: it requires `IsPaid` **and** no missing identification.
2. **Availability is informational only** — it never blocks a booking; overbooking is currently possible.
3. **Pricing is client-trusted** — `TotalPrice` and add-on prices are stored as submitted; no server recompute.
4. **Content capacity / cancellation / payment flags are mostly catalog metadata**, not enforced policy.
5. **OTP and social login are prototype-grade** trust boundaries (no rate limit, mocked delivery, unverified provider tokens).
6. **`BUSINESS_RULES.md` is a brochure, not behaviour** — cash cleanup is the only wired rule and it is keyed off the wrong date.
7. **Notable dead/ahead-of-need code**: refund processor factory (no caller), permission matrix (barely enforced), duplicate Excel paths.
