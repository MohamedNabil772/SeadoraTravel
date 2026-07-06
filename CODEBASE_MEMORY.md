# 🌊 Seadora Travel — Codebase Memory & Identification

> **Purpose**: This file serves as a living reference document for the Seadora Travel codebase.
> It is the single source of truth for understanding the system architecture, current state,
> and known issues. **Update this file whenever changes are made.**

> **Last Updated**: 2026-07-06

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Workspace Layout](#2-workspace-layout)
3. [Technology Stack](#3-technology-stack)
4. [Architecture Overview](#4-architecture-overview)
5. [Static Landing Page](#5-static-landing-page)
6. [Microservices Backend](#6-microservices-backend)
   - [6.1 Identity Service](#61-identity-service)
   - [6.2 Content Service](#62-content-service)
   - [6.3 Booking Service](#63-booking-service)
   - [6.4 File Server](#64-file-server)
7. [Shared Library: Seadora.Common](#7-shared-library-seadoracommon)
8. [API Gateway](#8-api-gateway)
9. [Web Frontends](#9-web-frontends)
   - [9.1 Customer Website](#91-customer-website)
   - [9.2 Admin Dashboard](#92-admin-dashboard)
10. [Docker Compose Topology](#10-docker-compose-topology)
11. [Test Projects](#11-test-projects)
12. [Business Rules](#12-business-rules)
13. [Seed Data Reference](#13-seed-data-reference)
14. [Known Issues & Gaps](#14-known-issues--gaps)
15. [Change Log](#15-change-log)

---

## 1. Project Overview

| Field | Value |
|---|---|
| **Brand Name** | SeeDora Travel / Seadora Travel |
| **Business** | Luxury Egyptian travel agency |
| **Base** | Hurghada, Red Sea, Egypt |
| **Target Audience** | European travellers (DE, IT, FR, RU, EN) |
| **Phone** | +20 100 129 6641 |
| **Email** | info@sedoratravel.com |
| **Languages** | English, German, Italian, French, Russian |

The solution has **two independent parts**:
1. **Static landing page** — `index.html` at workspace root (standalone marketing site)
2. **Microservices platform** — `seadoratravel/` directory (full-stack booking system)

---

## 2. Workspace Layout

```
D:\Seadora Travel\
├── index.html                          # Static marketing landing page (82 KB, 1556 lines)
├── CODEBASE_MEMORY.md                  # ← This file
│
└── seadoratravel\                      # Microservices platform
    ├── SEADORA.sln                     # Visual Studio solution (all projects)
    ├── BUSINESS_RULES.md               # Documented business policies (DRAFT)
    ├── docker-compose.yml              # 8-service Docker topology
    │
    ├── src\
    │   ├── ApiGateway\                 # YARP reverse proxy
    │   │   └── Seadora.ApiGateway\
    │   │
    │   ├── Services\
    │   │   ├── Common\
    │   │   │   └── Seadora.Common\     # Shared library
    │   │   │
    │   │   ├── Identity.Service\       # Auth & user management
    │   │   │   ├── Seadora.Identity.API\
    │   │   │   ├── Seadora.Identity.Application\
    │   │   │   ├── Seadora.Identity.Domain\
    │   │   │   └── Seadora.Identity.Infrastructure\
    │   │   │
    │   │   ├── Content.Service\        # Destinations, tours, categories
    │   │   │   ├── Seadora.Content.API\
    │   │   │   ├── Seadora.Content.Application\
    │   │   │   ├── Seadora.Content.Domain\
    │   │   │   └── Seadora.Content.Infrastructure\
    │   │   │
    │   │   ├── Booking.Service\        # Bookings & feedback
    │   │   │   ├── Seadora.Booking.API\
    │   │   │   ├── Seadora.Booking.Application\
    │   │   │   ├── Seadora.Booking.Domain\
    │   │   │   └── Seadora.Booking.Infrastructure\
    │   │   │
    │   │   └── FileServer\             # File uploads/downloads
    │   │       └── Seadora.FileServer.API\
    │   │
    │   └── Web\
    │       ├── seadora-website\        # Vue 3 customer-facing SPA
    │       └── seadora-admin\          # Vue 3 admin dashboard SPA
    │
    └── tests\
        ├── Seadora.Common.Tests\
        ├── Seadora.UnitTests\
        ├── Seadora.IntegrationTests\
        └── Services\
            └── Identity\
                └── Seadora.Identity.Application.Tests\
```

---

## 3. Technology Stack

| Layer | Technology | Version |
|---|---|---|
| **Backend Runtime** | .NET | 9.0 |
| **Web Framework** | ASP.NET Core Minimal APIs + Controllers | 9.0 |
| **CQRS / Mediator** | MediatR | 14.1 |
| **ORM** | Entity Framework Core | 9.0 |
| **Database** | PostgreSQL (Npgsql) | 15 Alpine |
| **Auth** | ASP.NET Identity + JWT Bearer (HMAC-SHA256) | — |
| **API Gateway** | YARP (Yet Another Reverse Proxy) | 2.3.0 |
| **Logging** | Serilog + Serilog.AspNetCore | 4.2 / 9.0 |
| **Frontend Framework** | Vue | 3.5 |
| **Frontend Language** | TypeScript | 6.0 |
| **Build Tool** | Vite | 8.0 |
| **State Management** | Pinia | 3.0 |
| **Routing** | Vue Router | 4.6 |
| **i18n** | Vue I18n | 9.14 |
| **CSS (Website)** | Tailwind CSS | 4.3 |
| **CSS (Admin)** | Tailwind CSS | 3.4 |
| **CSS (Landing Page)** | Vanilla CSS | — |
| **Containerization** | Docker Compose | — |
| **Testing** | xUnit + Moq + FluentAssertions | 2.9.3 / 4.20 / 8.10 |

---

## 4. Architecture Overview

**Pattern**: Microservices with Clean Architecture per service

Each backend service follows 4-layer Clean Architecture:
```
API (Controllers/Endpoints) → Application (Commands/Queries via MediatR) → Domain (Entities) → Infrastructure (EF Core, PostgreSQL)
```

**Communication**:
- Frontends → API Gateway (YARP on `:8000`) → Individual microservices
- Database-per-service (each service has its own PostgreSQL database)
- No inter-service communication (no message bus, no gRPC, no REST cross-calls)
- Cross-service references are denormalized (e.g., `TourId` stored in Booking without validation)

**Data Flow**:
```
Vue 3 SPA ──HTTP──► YARP Gateway (:8000) ──proxy──► Microservice ──EF Core──► PostgreSQL
```

---

## 5. Static Landing Page

**File**: `index.html` (root of workspace)
**Size**: 82 KB, 1,556 lines
**Type**: Self-contained single HTML file (inline CSS + inline JS)

### Sections
| Section | Description |
|---|---|
| Language Bar | 5-language switcher (EN, DE, IT, FR, RU) using `data-lang` attributes + CSS class toggle |
| Navbar | Sticky, glass-blur backdrop, gold-accented logo, nav links, orange CTA |
| Hero | Full-viewport gradient (sea-deep blues → greens), animated stats sidebar |
| Destinations | 4-column responsive grid: Hurghada, Cairo, Luxor, Sharm El-Sheikh |
| Trips | Tab-filtered grid (All/Sea/Desert/Cultural/Cruise), 3-column cards |
| Why Choose Us | 6 icon feature cards |
| Testimonials | 3 multilingual review cards with emoji country flags |
| Contact | 2-column: info + form (mock submit — no backend connection) |
| Footer | 4-column: brand, destinations, tours, contact |

### Fonts
- **Playfair Display** (serif, headings)
- **Cormorant Garamond** (serif, body accent)
- **Jost** (sans-serif, body text)

### Color Palette (CSS custom properties)
| Variable | Hex | Usage |
|---|---|---|
| `--sea` | `#0a5c8a` | Primary blue |
| `--sea-light` | `#1a8bc4` | Light blue |
| `--sea-deep` | `#063a5c` | Dark blue (nav, hero) |
| `--sun` | `#e8820a` | Primary orange (CTAs) |
| `--sun-light` | `#f5a435` | Light orange |
| `--grass` | `#2e7d4f` | Green accents |
| `--gold` | `#c9a84c` | Gold accents |
| `--cream` | `#faf7f2` | Background |
| `--dark` | `#0d1f2d` | Dark sections |

### JavaScript Features
- `setLang(lang, btn)` — language switcher via CSS class on `<body>`
- `filterTrips(cat, btn)` — tab-based trip category filter
- `handleSubmit(btn)` — mock form submission (visual feedback only)
- `IntersectionObserver` — scroll-reveal animations on cards

> **Note**: The landing page is completely standalone. It does NOT connect to any backend service.

---

## 6. Microservices Backend

### 6.1 Identity Service

**Path**: `seadoratravel/src/Services/Identity.Service/`
**Database**: `Seadora_Identity` (PostgreSQL)
**Ports**: HTTP `5062`, HTTPS `7002`

#### Domain Entities

**User** (extends `IdentityUser<string>`):
| Property | Type | Notes |
|---|---|---|
| `Id` | `string` | Auto-generated GUID string |
| `FirstName` | `string` | — |
| `LastName` | `string` | — |
| `Roles` | `List<Role>` | Navigation (many-to-many) |
| *(inherited)* | — | UserName, Email, PasswordHash, PhoneNumber, etc. |

**Role** (extends `IdentityRole<string>`):
| Property | Type | Notes |
|---|---|---|
| `Id` | `string` | Auto-generated GUID string |
| `Users` | `List<User>` | Navigation (many-to-many) |

#### CQRS (MediatR)

| Type | Name | Input | Output |
|---|---|---|---|
| Command | `RegisterCommand` | `(FirstName, LastName, Email, Password)` | `AuthResponse` |
| Command | `LoginCommand` | `(Email, Password)` | `AuthResponse` |

**AuthResponse** DTO: `record AuthResponse(string Token, string Email)`

No Queries exist — no user profile or user listing endpoints.

#### API Endpoints (Controller-based)

| Method | Route | Auth | Handler |
|---|---|---|---|
| `POST` | `/api/Auth/register` | Anonymous | `RegisterCommand` |
| `POST` | `/api/Auth/login` | Anonymous | `LoginCommand` |
| `GET` | `/WeatherForecast` | Anonymous | ⚠️ Scaffold leftover |

#### JWT Configuration

| Setting | Value | Source |
|---|---|---|
| Algorithm | HMAC-SHA256 | Hardcoded |
| Expiry | **7 days** | Hardcoded (ignores `ExpiryMinutes` config) |
| Issuer | `"SeadoraTravel"` | Fallback default |
| Audience | `"SeadoraTravelUsers"` | Fallback default |
| Secret | `"YourSuperSecretKeyHereYourSuperSecretKeyHere"` | ⚠️ Hardcoded fallback |
| Roles in JWT | **NOT INCLUDED** | ⚠️ Role-based auth won't work |

#### Infrastructure
- **DbContext**: `SeadoraIdentityDbContext` — defined inside `DependencyInjection.cs` (not its own file)
- **Schema**: `EnsureCreatedAsync()` — no EF migrations
- **Seeder**: Runs on startup, creates 3 roles + 3 users
- **Validation**: FluentValidation package referenced but **commented out**
- **Email**: Not implemented
- **CORS**: `AllowAll` (any origin, method, header)

#### Seed Data
| Role | User | Email | Password |
|---|---|---|---|
| Admin | System Admin | `admin@seadoratravel.com` | `Admin123!` |
| BookingManager | Booking Manager | `manager@seadoratravel.com` | `Manager123!` |
| Customer | John Doe | `customer@gmail.com` | `Customer123!` |

---

### 6.2 Content Service

**Path**: `seadoratravel/src/Services/Content.Service/`
**Database**: `Seadora_Content` (PostgreSQL)
**Ports**: HTTP `5190`, HTTPS `7030`
**Nature**: **Read-only** — no write endpoints exist

#### Domain Entities

**Category**:
| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | PK |
| `Names` | `Dictionary<string, string>` | Localized (jsonb) |
| `Icon` | `string` | Emoji or CSS class |
| `Tours` | `ICollection<Tour>` | Navigation (1:M) |

**Destination**:
| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | PK |
| `Names` | `Dictionary<string, string>` | Localized (jsonb) |
| `Descriptions` | `Dictionary<string, string>` | Localized (jsonb) |
| `ImageUrl` | `string` | — |
| `Flag` | `string` | Emoji flag |
| `Tours` | `ICollection<Tour>` | Navigation (1:M) |

**Tour**:
| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | PK |
| `Names` | `Dictionary<string, string>` | Localized (jsonb) |
| `Descriptions` | `Dictionary<string, string>` | Localized (jsonb) |
| `Price` | `decimal` | — |
| `Duration` | `string` | Free-form: `"fullDay"`, `"halfDay"`, `"twoDays"`, `"fiveDays"`, `"oneDay"`, `"threeHours"`, `"evening"` |
| `Includes` | `List<string>` | e.g. `["🚌 Transfer", "🥗 Lunch"]` |
| `ImageUrl` | `string` | — |
| `Emoji` | `string` | — |
| `BgGradient` | `string` | CSS gradient for card |
| `Badge` | `string` | — |
| `DestinationId` | `Guid` | FK → Destination |
| `CategoryId` | `Guid` | FK → Category |

**Relationships**: Tour → Destination (M:1), Tour → Category (M:1)

#### CQRS (MediatR)

| Type | Name | Handler |
|---|---|---|
| Query | `GetDestinationsQuery` | Returns `List<Destination>` via DbContext |
| Query | `GetToursQuery` | Returns `List<Tour>` (includes Destination) |

No Commands exist — service is purely read-only.

#### API Endpoints (Controller-based)

| Method | Route | Pattern | Returns |
|---|---|---|---|
| `GET` | `/api/categories` | Direct DbContext (no MediatR) | `List<Category>` |
| `GET` | `/api/destinations` | MediatR Query | `List<Destination>` |
| `GET` | `/api/tours` | MediatR Query | `List<Tour>` (with Destination) |

**No DTOs** — domain entities returned directly. Circular references handled by `ReferenceHandler.IgnoreCycles`.

#### Infrastructure
- **DbContext**: `ContentDbContext` — jsonb configuration for all `Dictionary<string, string>` properties
- **Schema**: `EnsureDeletedAsync()` + `EnsureCreatedAsync()` — **destructive re-creation every startup**
- **No migrations**
- **CORS**: `AllowAll`
- **JSON**: `EnableDynamicJson()` on Npgsql data source

#### Seed Data
- **3 Categories**: Sea & Diving, Culture & History, Safari & Adventure
- **4 Destinations**: Hurghada, Luxor, Cairo, Sharm El-Sheikh
- **9 Tours**: Each with full 5-language localization (en, de, it, fr, ru)

---

### 6.3 Booking Service

**Path**: `seadoratravel/src/Services/Booking.Service/`
**Database**: `Seadora_Booking` (PostgreSQL)
**Ports**: HTTP `5076`, HTTPS `7286`

#### Domain Entities

**Booking** (anemic POCO):
| Property | Type | Default |
|---|---|---|
| `Id` | `Guid` | — |
| `TourId` | `Guid` | — |
| `CustomerName` | `string` | `""` |
| `CustomerEmail` | `string` | `""` |
| `BookingDate` | `DateTime` | — |
| `Status` | `string` | `"Pending"` |

**Feedback** (anemic POCO):
| Property | Type | Default |
|---|---|---|
| `Id` | `Guid` | — |
| `TourId` | `Guid` | — |
| `Rating` | `double` | — |
| `Comment` | `string` | `""` |
| `CustomerName` | `string` | `""` |
| `CustomerEmail` | `string` | `""` |
| `CreatedAt` | `DateTime` | — |
| `IsVisible` | `bool` | `true` |

#### CQRS (MediatR)

| Type | Name | Input | Output |
|---|---|---|---|
| Command | `CreateBookingCommand` | `(TourId, CustomerName, CustomerEmail)` | `Guid` |
| Command | `CreateFeedbackCommand` | `(TourId, Rating, Comment, CustomerName, CustomerEmail)` | `Feedback` |
| Command | `UpdateFeedbackVisibilityCommand` | `(Id, IsVisible)` | `Unit` |
| Query | `GetFeedbacksQuery` | `(TourId?, IncludeHidden)` | `List<Feedback>` |

Inline validation in handlers (no FluentValidation):
- Booking: TourId required, name 2–100 chars, email regex
- Feedback: Rating between 0.5–5.0 (bug: message says "1 and 5")

#### API Endpoints (Controller-based)

| Method | Route | Handler | Returns |
|---|---|---|---|
| `POST` | `/api/bookings` | `CreateBookingCommand` | `Guid` |
| `POST` | `/api/feedbacks` | `CreateFeedbackCommand` | `Feedback` |
| `GET` | `/api/feedbacks?tourId={guid}&includeHidden={bool}` | `GetFeedbacksQuery` | `List<Feedback>` |
| `PUT` | `/api/feedbacks/{id}/visibility` | `UpdateFeedbackVisibilityCommand` | `NoContent` |

**Missing**: No GET/PUT/DELETE for bookings. No booking detail or listing endpoint.

#### Domain Service (DRAFT — NOT ACTIVE)
`CancellationPolicyService` in `Domain/Services/`:
- `CalculateRefundAmount()` — **commented out**, currently returns full amount
- `IsCashReservationValid()` — **commented out**, currently returns `true`
- **Not registered in DI**, not used anywhere

#### Infrastructure
- **DbContext**: `BookingDbContext` — no `OnModelCreating`, no Fluent API, no indexes
- **Schema**: `EnsureDeletedAsync()` + `EnsureCreatedAsync()` — destructive every startup
- **No migrations**
- **CORS**: `AllowAll`

#### Seed Data
- **0 bookings** seeded
- **19 feedbacks** seeded (7 named + 12 generic) for TourIds `00000000-0000-0000-0000-000000000101` through `109`

---

### 6.4 File Server

**Path**: `seadoratravel/src/Services/FileServer/Seadora.FileServer.API/`
**Storage**: Local disk (`uploads/` directory, Docker volume `seadora-uploads`)

#### API Endpoints (Controller-based)

| Method | Route | Purpose | Returns |
|---|---|---|---|
| `POST` | `/api/files` | Upload file | `{ FileId }` |
| `GET` | `/api/files/{fileId}` | Download file | File stream (`application/octet-stream`) |
| `DELETE` | `/api/files/{fileId}` | Delete file | 204 No Content |

- Files renamed to `{GUID}{extension}` on disk
- Uses `LocalStorageService` from `Seadora.Common`
- Config: `StorageSettings:Path = "uploads"`

---

## 7. Shared Library: Seadora.Common

**Path**: `seadoratravel/src/Services/Common/Seadora.Common/`
**Target**: .NET 9.0 class library
**Dependencies**: `Microsoft.AspNetCore.Http.Abstractions 2.2.0`, `Serilog 4.2.0`, `Serilog.AspNetCore 9.0.0`

### Storage Abstraction
| Class | Description |
|---|---|
| `IStorageService` | Interface: `UploadFileAsync`, `GetFileAsync`, `DeleteFileAsync` |
| `LocalStorageService` | Writes to local directory, `{GUID}{ext}` naming |
| `RemoteStorageService` | HTTP proxy to FileServer via `HttpClient` |
| `StorageDependencyInjection` | `AddSeadoraStorage()` — reads `StorageSettings:Type` config (`"Remote"` or local) |

### Logging
| Class | Description |
|---|---|
| `CorrelationIdMiddleware` | Reads/generates `X-Correlation-ID` header, pushes to Serilog `LogContext` |

---

## 8. API Gateway

**Path**: `seadoratravel/src/ApiGateway/`
**Technology**: YARP 2.3.0
**Port**: `:8000` (Docker: `8000→8080`)
**CORS**: `AllowAll`

### Route Table

| Route Pattern | Backend Cluster | Backend Address | Path Transform |
|---|---|---|---|
| `/api/auth/{**catch-all}` | `identity-cluster` | `http://identity-service:8080` | Strip `/api/auth` |
| `/api/content/{**catch-all}` | `content-cluster` | `http://content-service:8080` | Strip `/api/content` |
| `/api/booking/{**catch-all}` | `booking-cluster` | `http://booking-service:8080` | Strip `/api/booking` |
| `/api/files/{**catch-all}` | `file-cluster` | `http://file-server:8080` | Strip `/api/files` |

**Example request flow**:
```
Frontend: GET /api/content/api/tours
Gateway strips "/api/content" → forwards to content-service:8080 as GET /api/tours
```

---

## 9. Web Frontends

### 9.1 Customer Website

**Path**: `seadoratravel/src/Web/seadora-website/`
**Stack**: Vue 3.5 + TypeScript 6.0 + Vite 8.0 + Pinia 3.0 + Vue Router 4.6 + Vue I18n 9.14 + Tailwind CSS 4.3
**Docker**: Multi-stage → Nginx Alpine (`:3000→80`), custom `nginx.conf` with SPA fallback

#### Routes
| Path | View | Description |
|---|---|---|
| `/` | `HomeView` | Hero, Destinations, Trips, Testimonials, WhyChoose, Contact |
| `/tours` | `ToursView` | Full tour listing (46 KB — feature-rich) |
| `/tour/:slug` | `TourDetailsView` | Tour detail page (90 KB — very detailed) |
| `/feedback` | `FeedbackView` | Feedback/reviews page (17 KB) |

#### Components (10)
`Navbar`, `Hero`, `Destinations`, `Trips`, `TourDetailsModal`, `Contact`, `Testimonials`, `WhyChoose`, `Footer`, `HelloWorld`

#### i18n
5 language locale files (~7–9 KB each): `en.json`, `de.json`, `it.json`, `fr.json`, `ru.json`

#### Tailwind Theme
Custom colors: `sea` (blues), `sun` (oranges), `grass` (greens), `gold`, `cream`, `dark`, `text`, `muted`
Fonts: Playfair Display, Cormorant Garamond, Jost

#### State
Pinia store `contact.ts` — contact form with loading/success/error states (simulates API call via `setTimeout`)

---

### 9.2 Admin Dashboard

**Path**: `seadoratravel/src/Web/seadora-admin/`
**Stack**: Vue 3.5 + TypeScript 6.0 + Vite 8.0 + Pinia 3.0 + Vue Router 4.6 + Vue I18n 9.14 + Tailwind CSS 3.4
**Docker**: Multi-stage → Nginx Alpine (`:3001→80`)

#### Routes
| Path | View | Notes |
|---|---|---|
| `/login` | `LoginView` | Standalone login view with async auth and error alert |
| `/` | `DashboardView` | Summary cards showing active tours, destinations, categories, total bookings, recent bookings |
| `/tours` | `ToursView` | Complete CRUD for tours with localized tab inputs |
| `/destinations` | `DestinationsView` | Complete CRUD for destinations with localized tab inputs |
| `/categories` | `CategoriesView` | Complete CRUD for categories with localized tab inputs |
| `/bookings` | `BookingsView` | Table of bookings with status badges and Confirm/Complete/Cancel actions |
| `/feedback` | `FeedbackView` | Star ratings and comments of customer feedbacks |
| `/users` | `UsersView` | Simple placeholder |

#### Layout
`DashboardLayout.vue` — sidebar (Dashboard, Tours, Destinations, Categories, Bookings, Feedback, Users, Logout) + header with route name + admin avatar

#### State
Pinia store `auth.ts` — real async API auth on POST `/api/auth/api/Auth/login`, verification guards, localStorage persistence, `login()`/`logout()`/`initAuth()`

#### i18n
5 languages via separate `i18n/` directory

#### Status: Fully Functional
- `App.vue` fully wires routing via `<RouterView />`
- `main.ts` successfully imports and registers Pinia, router, and i18n
- Fully connected and calling backend microservices through YARP Gateway interceptor

---

## 10. Docker Compose Topology

**File**: `seadoratravel/docker-compose.yml`
**Services**: 8

| Service | Image/Build | Port Mapping | Depends On | Database |
|---|---|---|---|---|
| `postgres` | `postgres:15-alpine` | `5432:5432` | — | — |
| `api-gateway` | Build from `src/ApiGateway/Dockerfile` | `8000:8080` | identity, content, booking, file-server | — |
| `identity-service` | Build from Identity.API Dockerfile | Internal | postgres (healthy) | `Seadora_Identity` |
| `content-service` | Build from Content.API Dockerfile | Internal | postgres (healthy) | `Seadora_Content` |
| `booking-service` | Build from Booking.API Dockerfile | Internal | postgres (healthy) | `Seadora_Booking` |
| `file-server` | Build from FileServer.API Dockerfile | Internal | — | — |
| `seadora-website` | Build from `src/Web/seadora-website` | `3000:80` | — | — |
| `seadora-admin` | Build from `src/Web/seadora-admin` | `3001:80` | — | — |

**Volumes**:
- `seadora-db-data` → PostgreSQL data
- `seadora-uploads` → File Server uploads

**Environment Variables** (from docker-compose):
- PostgreSQL: `POSTGRES_USER=postgres`, `POSTGRES_PASSWORD=postgres`
- Identity: `JwtSettings__Secret=YourSuperSecretKeyHereYourSuperSecretKeyHere`
- All services: `ASPNETCORE_ENVIRONMENT=Development`
- Connection strings provided via env vars (not in appsettings)
- Frontends: `VITE_API_URL=http://localhost:8000`

---

## 11. Test Projects

| Project | Path | Deps | Reference | Status |
|---|---|---|---|---|
| `Seadora.Common.Tests` | `tests/Seadora.Common.Tests/` | xUnit + Moq + FluentAssertions | `Seadora.Common` | ⬜ Empty scaffold |
| `Seadora.Identity.Application.Tests` | `tests/Services/Identity/...` | xUnit + Moq + FluentAssertions | `Seadora.Identity.Application` | ⬜ Empty scaffold |
| `Seadora.UnitTests` | `tests/Seadora.UnitTests/` | xUnit + coverlet | None | ⬜ Empty scaffold |
| `Seadora.IntegrationTests` | `tests/Seadora.IntegrationTests/` | xUnit + coverlet | None | ⬜ Empty scaffold |

All test projects contain only a single empty `Test1()` method. No actual tests written.

---

## 12. Business Rules

**Source**: `seadoratravel/BUSINESS_RULES.md`
**Status**: DRAFT — **NOT IMPLEMENTED**

### 1. Cash Payment Deadline
- Cash bookings must be confirmed 48 hours before `BookingDate`
- Auto-cancel if not confirmed in time

### 2. Cancellation & Refund Tiers (Online Payments)
| Tier | Window | Penalty |
|---|---|---|
| Free Cancellation | >72 hours before | 0% (full refund) |
| Late Cancellation | 48–72 hours before | 25% retained |
| Last-Minute | <24 hours before | 50% retained |

### Implementation Notes
- Need `ICancellationPolicyService` in Booking.Service domain
- Need background worker (Hangfire/Quartz) for cash booking pruning
- `CancellationPolicyService.cs` exists but all logic is **commented out**

---

## 13. Seed Data Reference

### Identity Service
| Role | Email | Password |
|---|---|---|
| Admin | `admin@seadoratravel.com` | `Admin123!` |
| BookingManager | `manager@seadoratravel.com` | `Manager123!` |
| Customer | `customer@gmail.com` | `Customer123!` |

### Content Service
**Categories**: Sea & Diving, Culture & History, Safari & Adventure

**Destinations**: Hurghada, Luxor, Cairo, Sharm El-Sheikh

**Tours** (9 total, all 5-language localized):
Glass Boat, Desert Safari Quad, Luxor Full Day, Pyramids of Giza, Red Sea Diving, Nile Dinner Cruise, and 3 more

### Booking Service
**Bookings**: None seeded
**Feedbacks**: 19 records for TourIds `00000000-0000-0000-0000-000000000101` through `109`

---

## 14. Known Issues & Gaps

### 🔴 Critical
| # | Issue | Service | Detail |
|---|---|---|---|
| 1 | **Destructive DB on startup** | All services | `EnsureDeletedAsync()` + `EnsureCreatedAsync()` wipes all data on every restart |
| 2 | **No EF migrations** | All services | No migration history — impossible to evolve schema safely |
| 3 | **Missing config** | Identity | No connection string or JWT settings in appsettings.json — relies on env vars |

### 🟠 High
| # | Issue | Service | Detail |
|---|---|---|---|
| 4 | **Roles not in JWT** | Identity | **RESOLVED**: Roles added to claims in JwtTokenGenerator and handlers. |
| 5 | **No auth on endpoints** | Content, Booking | No `[Authorize]` attributes — APIs fully open |
| 6 | **Booking: minimal CRUD** | Booking | **RESOLVED**: Exposed GET (all), GET (by id), and PUT (status update) endpoints. |
| 7 | **Admin app not wired** | Admin Frontend | **RESOLVED**: App is fully wired up with Pinia, Router, i18n, and auth guard. |

### 🟡 Medium
| # | Issue | Service | Detail |
|---|---|---|---|
| 8 | **JWT expiry hardcoded** | Identity | **RESOLVED**: Configurable expiry read from ExpiryMinutes config setting. |
| 9 | **No refresh tokens** | Identity | Only access tokens issued |
| 10 | **No validation pipeline** | Identity | FluentValidation referenced but commented out |
| 11 | **Status as magic string** | Booking | `Booking.Status` is `string`, not enum — no state machine |
| 12 | **Business rules inactive** | Booking | Cancellation policy logic commented out |
| 13 | **Content: read-only** | Content | **RESOLVED**: Exposed full Create, Update, Delete CRUD commands and endpoints. |
| 14 | **Content: no DTOs** | Content | Domain entities exposed directly in API responses |
| 15 | **All tests empty** | All | 4 test projects are scaffolds with empty `Test1()` |
| 16 | **CORS wide open** | All services | `AllowAll` — too permissive for production |
| 17 | **Rating validation bug** | Booking | Error says "1 to 5" but code allows 0.5 |
| 18 | **Payment not implemented** | — | Stripe/PayPal mentioned but not coded |
| 19 | **Email not implemented** | — | No email service exists |
| 20 | **Contact form mock** | Website | Uses `setTimeout` — not connected to backend |

### 🔵 Low
| # | Issue | Service | Detail |
|---|---|---|---|
| 21 | **Inconsistent CQRS** | Content | CategoriesController bypasses MediatR, others use it |
| 22 | **Scaffold artifacts** | Identity, Booking | WeatherForecast, Class1.cs, HelloWorld still present |
| 23 | **Tailwind version mismatch** | Frontends | Website uses v4.3, admin uses v3.4 |
| 24 | **DbContext in wrong file** | Identity | Defined inside `DependencyInjection.cs` |
| 25 | **Duration as string** | Content | Free-form strings instead of enum |

---

## 15. Change Log

| Date | Change | Author |
|---|---|---|
| 2026-07-06 | Implemented feedback visibility control toggles on admin panel and API backend | AI Assistant |
| 2026-07-06 | Implemented full admin dashboard routing, auth store, CRUD endpoints, and views | AI Assistant |
| 2026-07-06 | Initial codebase memory document created from full investigation | AI Assistant |

---

> **Maintenance Note**: This file should be updated whenever:
> - New endpoints are added or modified
> - Entity schemas change
> - New services or frontends are added
> - Issues from Section 14 are resolved
> - New issues are discovered
> - Configuration changes are made
> - Dependencies are upgraded
