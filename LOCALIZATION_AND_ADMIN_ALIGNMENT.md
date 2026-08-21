# 🌊 Seadora Travel — Localization, Admin & Website Alignment Reference

> **Purpose**: Single source of truth for synchronization between the **Customer Website**, **Admin Dashboard**, and **Microservices Backend (Content & Identity Services)**.
> **Date**: August 2026

---

## 1. Supported Languages & Localization Rules

### 1.1 Language Matrix
| Language Code | Language Name | Native Name | Flag Emoji | Direction | Website Status | Admin Status |
|---|---|---|---|---|---|---|
| `en` | English | English | 🇬🇧 | LTR | **Default / Primary** | Active |
| `de` | German | Deutsch | 🇩🇪 | LTR | Active | Active |
| `it` | Italian | Italiano | 🇮🇹 | LTR | Active | Active |
| `fr` | French | Français | 🇫🇷 | LTR | Active | Active |
| `ru` | Russian | Русский | 🇷🇺 | LTR | Active | Active |
| ~~`ar`~~ | ~~Arabic~~ | ~~العربية~~ | 🇸🇦 | RTL | **Disabled for now** | Excluded from active seed & selectors |

### 1.2 Localization Architecture
1. **Dynamic Translation Storage**:
   - Stored in backend PostgreSQL `Translations` table (`ContentDbContext`).
   - Fields: `Id`, `Key`, `Namespace`, `Values (jsonb: en, de, it, fr, ru)`, `UpdatedAt`.
2. **Backend Endpoints**:
   - `GET /api/v1/languages` → Lists active languages (`[en, de, it, fr, ru]`).
   - `GET /api/v1/languages/{code}/translations` → Returns flat key-value dictionary for requested language.
   - `GET /api/v1/languages/all-translations` → Returns full dictionary for Admin Translations Editor.
   - `PUT /api/v1/languages/{code}/translations` & `POST /api/v1/languages/translations/bulk` → Updates translations.
3. **Website Dynamic i18n Loading**:
   - Local bundled JSON (`locales/{lang}.json`) acts as instant initial / offline fallback.
   - On runtime / language change, `i18n.ts` fetches overrides from backend and merges into the Vue I18n store.
   - User language selection persists in `localStorage.getItem('seadora_lang')`.

---

## 2. Destinations Alignment

### 2.1 Domain & Schema Definition
```csharp
public class Destination
{
    public Guid Id { get; set; }
    public Dictionary<string, string> Names { get; set; } = new();         // "en", "de", "it", "fr", "ru"
    public Dictionary<string, string> Descriptions { get; set; } = new();  // "en", "de", "it", "fr", "ru"
    public Dictionary<string, string> Highlights { get; set; } = new();    // "en", "de", "it", "fr", "ru"
    public string ImageUrl { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;                  // e.g. "🤿", "🏺", "🏛️", "🏖️"
    public bool IsFeatured { get; set; }
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
```

### 2.2 Admin UI Specifications
- **Layout**: High-polish responsive **Grid View** (cards with image cover, flag emoji, 5-language name/desc, highlights pills, tour count badge, featured toggle) + optional **Table View**.
- **Actions**: "+ Add Destination" button, Edit button, Delete button (with tour dependency safeguard).
- **Modal Form**: Multi-language tabbed editor (`EN`, `DE`, `IT`, `FR`, `RU`), image URL input with live preview & file upload, flag emoji selector, highlights tag input.
- **Endpoints**:
  - `GET /api/content/api/destinations`
  - `POST /api/content/api/destinations`
  - `PUT /api/content/api/destinations/{id}`
  - `DELETE /api/content/api/destinations/{id}`

### 2.3 Website UI Specifications
- **Component**: `features/destinations/components/Destinations.vue`.
- **Behavior**: Dynamically fetches destinations from `/api/content/api/destinations`.
- **Card Rendering**: Localized name (`Names[locale]`), category/flag badge (`FlagEmoji`), localized highlights (`Highlights[locale]`), dynamic tour count (`toursCount[guid]`).
- **Interaction**: Clicking destination routes to `/tours?destination={id}`.

---

## 3. Categories Alignment

### 3.1 Domain & Schema Definition
```csharp
public class Category
{
    public Guid Id { get; set; }
    public Dictionary<string, string> Names { get; set; } = new();         // "en", "de", "it", "fr", "ru"
    public Dictionary<string, string> Descriptions { get; set; } = new();  // "en", "de", "it", "fr", "ru"
    public string? IconName { get; set; }                                  // Emoji or icon identifier
    public string? CustomIconUrl { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
```

### 3.2 Admin UI Specifications
- **Layout**: Grid & Table views with drag-and-drop ordering.
- **Actions**: "+ Add Category", Edit, Delete, Reorder.
- **Modal Form**: Localized tabs (`EN`, `DE`, `IT`, `FR`, `RU`), icon selector, cover image preview, display order.
- **Endpoints**:
  - `GET /api/content/api/categories`
  - `POST /api/content/api/categories`
  - `PUT /api/content/api/categories/{id}`
  - `DELETE /api/content/api/categories/{id}`
  - `POST /api/content/api/categories/reorder`

### 3.3 Website UI Specifications
- **Component**: `components/Trips.vue` & `features/tours/components/Trips.vue`.
- **Behavior**: Category filter tabs populated dynamically from `/api/content/api/categories` using `Names[locale]`.

---

## 4. Tour Details Alignment

### 4.1 Domain & Schema Specification
```csharp
public class Tour
{
    public Guid Id { get; set; }
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new();
    public string Emoji { get; set; } = string.Empty;
    public string BgGradient { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public Guid DestinationId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal SupplierPercentage { get; set; }
    public int MaxAllocations { get; set; } = 20;

    // Badges & Flags
    public bool IsTopRated { get; set; }
    public bool IsBestseller { get; set; }
    public bool IsInHighDemand { get; set; }
    public bool ReserveAndPayLater { get; set; } = true;
    public bool HotelPickup { get; set; } = true;
    public bool FreeCancellation { get; set; } = true;
    public bool IsPrivateOption { get; set; }

    // Nested Models (jsonb)
    public List<TourPackage> Packages { get; set; } = new();
    public string PickupTimeType { get; set; } = "FixedSlots";
    public List<string> AvailablePickupTimes { get; set; } = new();
    public Dictionary<string, string> Highlights { get; set; } = new();
    public List<TourItinerary> Itinerary { get; set; } = new();
    public List<TourInclusion> Inclusions { get; set; } = new();
    public List<TourInclusion> Exclusions { get; set; } = new();
    public ImportantInfo ImportantInformation { get; set; } = new();
    public List<TourFaq> Faqs { get; set; } = new();
    public List<TourAddon> Addons { get; set; } = new();
    public List<TourMedia> Media { get; set; } = new();
}
```

### 4.2 Admin Tour Editor Sections
1. **Basic Info**: Title & Description in 5 languages (en, de, it, fr, ru), Destination & Category selectors.
2. **Pricing & Timing**: Base price, original price, discount %, duration, start time, pickup time type & slots.
3. **Features & Badges**: Top Rated, Bestseller, High Demand, Hotel Pickup, Free Cancellation, Pay Later toggles.
4. **Packages / Variants**: Option tiers with title, price, descriptions, features.
5. **Highlights & Itinerary**: Multi-language day/time itinerary timeline.
6. **Inclusions & Exclusions**: What's included / excluded lists.
7. **Important Info & FAQs**: What to bring, Not suitable for, Notes, Accordion FAQs in 5 languages.
8. **Add-ons & Media**: Upgrades/extras per person/booking, photo gallery with captions.

### 4.3 Website Tour Details Rendering
- Fully dynamic binding of all above fields with 5-language reactivity:
  - Header badges & dynamic discount calculations.
  - Interactive package variant selection.
  - Interactive itinerary timeline.
  - Collapsible inclusions/exclusions.
  - Important info tabs.
  - Interactive FAQ accordion.
  - Add-on selector with dynamic total price computation.
  - Direct booking modal integration.

---

## 5. API Gateway (YARP) Route Map
| Route Pattern | Target Cluster | Internal URL | Path Transform |
|---|---|---|---|
| `/api/auth/{**catch-all}` | `identity-cluster` | `http://identity-service:8080` | Strip `/api/auth` |
| `/api/content/{**catch-all}` | `content-cluster` | `http://content-service:8080` | Strip `/api/content` |
| `/api/booking/{**catch-all}` | `booking-cluster` | `http://booking-service:8080` | Strip `/api/booking` |
| `/api/files/{**catch-all}` | `file-cluster` | `http://file-server:8080` | Strip `/api/files` |
