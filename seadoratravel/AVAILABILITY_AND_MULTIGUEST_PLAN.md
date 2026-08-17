# Seadora Travel - Architecture & Feature Plan: Calendar Date Range, Availability & Multi-Guest Booking

## 1. Feature Specifications

### A. Calendar View with Booking Availability & Date Range
- **Filter Date Range**: Supports single day or start-to-end date range selection in `ToursView.vue` with quick presets (*Today, Tomorrow, This Weekend, Next 7 Days, Next 14 Days, This Month*).
- **Interactive Tour Availability**: Displays date-by-date availability badges (*Available*, *Low Stock / <5 spots left*, *Sold Out*) and starting price tiers in `TourDetailsView.vue`.

### B. Multi-Guest Information Capture (Per Person Records)
- **Coast Guard & Tourism Police Compliance**: When `guests > 1`, the booking modal dynamically collects individual records for each traveler:
  - **Guest 1 (Lead Traveler)**: Full Name, Email, WhatsApp, Hotel & Room #, Passport/ID Photo.
  - **Guest 2, 3, ... (Travelers)**: Full Name, Age Category (Adult / Child), Passport/ID Photo, Nationality.
  - Interactive accordions with instant drag-and-drop validation, file preview, and smooth spring transitions.

### C. Backend Domain & API Contracts
- **Booking Service**: Extended `Booking` entity with `List<GuestDetail> GuestsList` (stored in PostgreSQL `jsonb` column `"GuestsList"`).
- **Content Service**: Extended `GetToursQuery` to filter by `StartDate` and `EndDate`.
- **ApiGateway**: Verified endpoints and routes across services.

---

## 2. Agent Squad Assignments
- **Frontend Lead 1 (UI/UX Pro Max)**: UX architecture, date range selection logic, multi-guest stepper & dynamic accordions.
- **Frontend Lead 2 (Emil Kowalski)**: Micro-interactions, spring animations, date-range hover bands, and file upload polish.
- **Backend Lead 1 (Ponytail)**: Domain model simplicity, `GuestDetail` entity, and PostgreSQL `jsonb` mapping.
- **Backend Lead 2 (Superpowers)**: API contracts, DTO mappings, `CreateBookingCommandHandler`, validation, and gateway routing.
