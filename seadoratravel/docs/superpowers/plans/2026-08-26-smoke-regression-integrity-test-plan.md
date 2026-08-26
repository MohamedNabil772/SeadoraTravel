# Seadora Travel Platform - Smoke, Regression & Integrity Test Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans to execute and maintain test coverage. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Establish and execute a full-suite automated regression, smoke, and API contract test harness across all Seadora Travel microservices (Content, Booking, Identity, FileServer, ApiGateway) and frontend web clients (`seadora-admin`, `seadora-website`).

**Architecture:** ASP.NET Core 9 Microservices behind Yarp Reverse Proxy (ApiGateway) running in Docker containers with PostgreSQL 16. Frontend clients built in Vue 3 + Vite + TypeScript + Pinia + TailwindCSS.

**Tech Stack:** .NET 9, MediatR, EF Core 9, Npgsql, PostgreSQL 16, Vue 3, Vite, TypeScript, Axios, PowerShell.

---

## Global Constraints & Service Architecture Map

| Service | Container Name | Internal Port | Gateway Route Prefix | Primary Responsibilities |
|---|---|---|---|---|
| **API Gateway** | `seadoratravel-api-gateway-1` | `8000` | `/` | Reverse proxy, path rewrite, CORS |
| **Content Service** | `seadoratravel-content-service-1` | `8080` | `/api/content` | Tours CRUD, Categories, Destinations, Currencies, Languages, Concierge AI |
| **Booking Service** | `seadoratravel-booking-service-1` | `8080` | `/api/booking` | Bookings, Availability, Feedbacks, Inquiries, Notifications, Reports |
| **Identity Service** | `seadoratravel-identity-service-1` | `8080` | `/api/auth` | JWT Auth, Login, Registration, OTP, Users |
| **File Server** | `seadoratravel-file-server-1` | `8080` | `/api/files` | Image uploads, Static asset serving |
| **Postgres DB** | `seadoratravel-postgres-1` | `5432` | N/A | Multi-schema persistence |

---

## Task Matrix & Test Breakdown

### Task 1: Content Service Tour Mutation & Query Health
**Files:**
- Modify: `src/Services/Content.Service/Seadora.Content.API/Controllers/ToursController.cs`
- Modify: `src/Services/Content.Service/Seadora.Content.Application/Tours/Commands/UpdateTourCommand.cs`
- Modify: `src/Services/Content.Service/Seadora.Content.Application/Tours/Commands/CreateTourCommand.cs`
- Test: `tests/full_system_smoke_test.ps1`

- [x] **Step 1: Verify Tour HTTP Methods in Controller**
  - Ensure `GET /api/tours`, `GET /api/tours/{id}`, `POST /api/tours`, `PUT /api/tours/{id}`, `DELETE /api/tours/{id}` are active and mapped.
- [x] **Step 2: Verify Partial DTO Serialization**
  - Allow nullable parameters (`Names`, `Descriptions`, `MediaUrls`, `Includes`, `Badge`, `Emoji`, `Price`).
- [x] **Step 3: Fallback Foreign Keys on Creation**
  - Automatically fallback `DestinationId` and `CategoryId` to available database entities if draft tour is submitted.
- [x] **Step 4: Execute Test Verification**
  - Test `PUT /api/content/api/tours/{id}` -> Returns `204 NoContent` and updates records properly.

---

### Task 2: Content Service Reference Entities (Categories, Destinations, Currencies, Languages)
**Files:**
- Modify: `src/Services/Content.Service/Seadora.Content.Application/Categories/Commands/CreateCategoryCommand.cs`
- Modify: `src/Services/Content.Service/Seadora.Content.Application/Categories/Commands/UpdateCategoryCommand.cs`
- Modify: `src/Services/Content.Service/Seadora.Content.Application/Destinations/Commands/CreateDestinationCommand.cs`
- Modify: `src/Services/Content.Service/Seadora.Content.Application/Destinations/Commands/UpdateDestinationCommand.cs`

- [x] **Step 1: Category Optional Fields**
  - Make `CoverImageUrl` optional during category creation/update.
- [x] **Step 2: Destination Optional Fields**
  - Make `ImageUrl` and `Highlights` optional with resilient fallbacks.
- [x] **Step 3: Currencies & Live Exchange Rates**
  - Test `GET /api/v1/currencies?includeInactive=true` and `POST /api/v1/currencies/sync-rates`.
- [x] **Step 4: Multi-Language & Translation Endpoints**
  - Verify `GET /api/v1/languages/all-translations` and `GET /api/v1/languages/en/translations`.

---

### Task 3: Booking Service Availability & Operational Workflows
**Files:**
- Modify: `src/Services/Booking.Service/Seadora.Booking.Application/Bookings/Queries/GetTourAvailabilityQuery.cs`
- Test: `tests/full_system_smoke_test.ps1`

- [x] **Step 1: Fix PostgreSQL UTC DateTime Comparison**
  - Convert `request.Date.Date` to UTC range `[targetDateUtc, targetDateUtc + 1 day)` to eliminate Npgsql `DateTimeKind.Unspecified` 500 error.
- [x] **Step 2: Booking Creation & Cash Cleanup**
  - Test booking placement, hotel pickup location binding, and auto-cancellation worker rules.
- [x] **Step 3: Feedbacks & Inquiries**
  - Test customer feedback submission with ratings and admin inquiries.
- [x] **Step 4: Analytics & Financial Ledger Reports**
  - Verify Dashboard stats, Supplier breakdown, Customer ledger, and Admin notifications endpoints.

---

### Task 4: Frontend Client Verification (`seadora-admin` & `seadora-website`)
**Files:**
- Inspect/Test: `src/Web/seadora-admin`
- Inspect/Test: `src/Web/seadora-website`

- [x] **Step 1: Vue Type-Check & Admin Bundle Build**
  - Run `npm run build` in `seadora-admin` -> Zero errors.
- [x] **Step 2: Vue Type-Check & Website Bundle Build**
  - Run `npm run build` in `seadora-website` -> Zero errors.
- [x] **Step 3: Admin Tour Edit & Tour Builder View Alignment**
  - Verify payloads sent by `TourEditView.vue` match Content Service commands.

---

## Test Execution Matrix & Results

```
================================================================
       SEADORA COMPLETE ENDPOINT HEALTH & REGRESSION SUITE       
================================================================
 Total Tests Executed: 41
 Passed:               41
 Failed:               0
 Success Rate:         100%
 Status:               ALL SYSTEMS HEALTHY AND FULLY OPERATIONAL
================================================================
```
