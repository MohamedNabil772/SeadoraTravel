# 🌊 Seadora Travel — Frontend Optimization Plan

> [!IMPORTANT]
> **Owner/Executor**: The findings, architecture, and step-by-step implementations outlined in this plan are designed to be executed by the **Senior Frontend Developer** agent.

This document details the architectural findings, target folder structures, file migration maps, and implementation plan to refactor the Seadora front-end applications (`seadora-website` and `seadora-admin`) to follow Clean Architecture, DRY patterns, Feature-Oriented modules, and the Factory Pattern.

---

## 1. Architectural Investigation & Core Findings

During investigation of the current codebase (`seadora-website` and `seadora-admin`), several architectural anti-patterns and optimization opportunities were identified:

### 🔴 Core Gaps & Issues
1. **Network Layer Coupling**: UI views and components directly call raw `fetch` or Axios endpoints (e.g., `/api/content/api/tours` or `/api/booking/api/bookings`) with hardcoded environment URLs. This couples presentation elements directly to HTTP communication and routing details.
2. **Duplicated Logic (Violation of DRY)**:
   - **Data Fetching**: Category, Tour, and Destination fetching logic is repeated across multiple views (`ToursView.vue`, `TourDetailsView.vue`, `FeedbackView.vue`, `Trips.vue`).
   - **Helper Functions**: Helper functions like `getSlug()` and `getLocalized()` are defined inline within multiple view files.
   - **Type Schemas**: Type interfaces for domain entities (`Tour`, `Category`, `Destination`, `Booking`, `Feedback`) are declared multiple times in different files.
3. **Bloated Components**: UI components and views are massive (e.g., `TourDetailsView.vue` is ~93KB, `ToursView.vue` is ~75KB, `Trips.vue` is ~48KB) because they combine UI layout, inline SVG graphics, local state management, validation logic, and API calls.
4. **Anemic Pinia Usage**: Pinia stores exist (`auth`, `contact`, `currency`) but are bypassed for core business flows (e.g. tour searching, filtering, and booking).

---

## 2. Target Clean Architecture (Feature-Oriented)

We will restructure both Vue SPAs to separate concerns into four clean layers, utilizing a **Factory Pattern** for repository instantiation.

```
src/
├── core/                        # 1. CORE / DOMAIN LAYER
│   ├── models/                  # Domain entity schemas (TypeScript interfaces)
│   └── repositories/            # Repository contracts (Interface signatures)
│
├── infrastructure/              # 2. INFRASTRUCTURE LAYER
│   ├── api/                     # Concrete Axios Repository implementations
│   └── factories/               # RepositoryFactory for dependency injection/creation
│
├── features/                    # 3. FEATURE / PRESENTATION LAYER
│   ├── tours/                   # Tours Feature (views, sub-components, Pinia stores)
│   ├── destinations/            # Destinations Feature
│   ├── feedback/                # Feedback & Reviews Feature
│   ├── bookings/                # Bookings & Invoice Feature
│   └── auth/                    # Client/Admin Authentication Feature
│
└── shared/                      # 4. SHARED UI & UTILS LAYER
    ├── components/              # Global reusable UI widgets (Navbar, Footer, Hero)
    ├── utils/                   # Reusable functions (getSlug, getLocalized, formatters)
    └── styles/                  # Tailwind configurations & theme css
```

### Dependency Flow Rule
- **Core Domain** has zero dependencies on Vue, Axios, or Pinia.
- **Infrastructure** implements Core repository interfaces and depends on the API client.
- **Pinia Stores** depend on Core contracts and call repositories obtained via `RepositoryFactory`.
- **Views and Components** depend only on Pinia Stores and Domain Models.

---

## 3. Abstract Definitions & Factory Pattern

Below are the TypeScript definitions and factories to be created to encapsulate the data layer.

### 3.1 Core Models (`src/core/models/`)
Common data types representing the domain:

```typescript
// src/core/models/Tour.ts
export interface Tour {
  id: string
  categoryId: string
  destinationId: string
  price: number
  names: Record<string, string>
  descriptions: Record<string, string>
  duration: string
  emoji?: string
  bgGradient?: string
  imageUrl?: string
  mediaUrls?: string[]
  includes?: string[]
}

// src/core/models/Category.ts
export interface Category {
  id: string
  names: Record<string, string>
  icon?: string
}

// src/core/models/Destination.ts
export interface Destination {
  id: string
  names: Record<string, string>
  descriptions?: Record<string, string>
  imageUrl?: string
  flag?: string
}
```

### 3.2 Core Repository Interfaces (`src/core/repositories/`)
Contracts decoupling the app from the networking library:

```typescript
// src/core/repositories/ITourRepository.ts
import type { Tour } from '../models/Tour'

export interface ITourRepository {
  getTours(): Promise<Tour[]>
  getTourById(id: string): Promise<Tour>
}

// src/core/repositories/IBookingRepository.ts
export interface BookingInput {
  tourId: string
  customerName: string
  customerEmail: string
  bookingDate: string
}

export interface IBookingRepository {
  createBooking(booking: BookingInput): Promise<string> // Returns Booking ID/Ref
}
```

### 3.3 Infrastructure Concrete Implementations (`src/infrastructure/api/`)
API communication using a single Axios instance client:

```typescript
// src/infrastructure/api/ApiTourRepository.ts
import type { AxiosInstance } from 'axios'
import type { ITourRepository } from '../../core/repositories/ITourRepository'
import type { Tour } from '../../core/models/Tour'

export class ApiTourRepository implements ITourRepository {
  constructor(private client: AxiosInstance) {}

  async getTours(): Promise<Tour[]> {
    const res = await this.client.get('/api/content/api/tours')
    return res.data
  }

  async getTourById(id: string): Promise<Tour> {
    const res = await this.client.get(`/api/content/api/tours/${id}`)
    return res.data
  }
}
```

### 3.4 Repository Factory (`src/infrastructure/factories/RepositoryFactory.ts`)
Orchestrates dependency construction:

```typescript
// src/infrastructure/factories/RepositoryFactory.ts
import axios from 'axios'
import { ApiTourRepository } from '../api/ApiTourRepository'
import { ApiBookingRepository } from '../api/ApiBookingRepository'
import type { ITourRepository } from '../../core/repositories/ITourRepository'
import type { IBookingRepository } from '../../core/repositories/IBookingRepository'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'

const apiClient = axios.create({
  baseURL: API_URL
})

// Attach authorization headers dynamically if available
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token') || localStorage.getItem('seadora_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export const RepositoryFactory = {
  getTourRepository(): ITourRepository {
    return new ApiTourRepository(apiClient)
  },
  getBookingRepository(): IBookingRepository {
    return new ApiBookingRepository(apiClient)
  }
}
```

---

## 4. File Migration Maps

### 4.1 Customer Website Refactoring (`seadora-website`)

| Original Path | Target Path | Layer/Feature | Action |
|---|---|---|---|
| *New File* | `src/core/models/` | Core | Create models: `Tour.ts`, `Category.ts`, `Destination.ts`, `Booking.ts`, `Feedback.ts` |
| *New File* | `src/core/repositories/` | Core | Create interfaces: `ITourRepository.ts`, `ICategoryRepository.ts`, `IDestinationRepository.ts`, `IBookingRepository.ts`, `IFeedbackRepository.ts` |
| *New File* | `src/infrastructure/api/` | Infrastructure | Implement concrete API classes: `ApiTourRepository.ts`, `ApiCategoryRepository.ts`, `ApiDestinationRepository.ts`, `ApiBookingRepository.ts`, `ApiFeedbackRepository.ts` |
| *New File* | `src/infrastructure/factories/` | Infrastructure | Create `RepositoryFactory.ts` config with Axios setup |
| *New File* | `src/shared/utils/helpers.ts` | Shared | Extract `getSlug()`, `getLocalized()` to make them DRY |
| `src/store/auth.ts` | `src/features/auth/store/auth.ts` | Feature: Auth | Move & update imports |
| `src/store/contact.ts` | `src/features/contact/store/contact.ts` | Feature: Contact | Move & update imports |
| `src/components/Contact.vue` | `src/features/contact/components/Contact.vue` | Feature: Contact | Move & update imports |
| `src/components/Destinations.vue` | `src/features/destinations/components/Destinations.vue` | Feature: Destination | Move & refactor to use repository |
| `src/views/FeedbackView.vue` | `src/features/feedback/views/FeedbackView.vue` | Feature: Feedback | Move & refactor to use repository |
| `src/views/ToursView.vue` | `src/features/tours/views/ToursView.vue` | Feature: Tours | Move & refactor to use store & repository |
| `src/views/TourDetailsView.vue` | `src/features/tours/views/TourDetailsView.vue` | Feature: Tours | Move & refactor to use store & repository |
| `src/components/TourDetailsModal.vue` | `src/features/tours/components/TourDetailsModal.vue` | Feature: Tours | Move & refactor to use repository |
| `src/components/Trips.vue` | `src/features/tours/components/Trips.vue` | Feature: Tours | Move & refactor to use store |
| `src/components/Navbar.vue` | `src/shared/components/Navbar.vue` | Shared UI | Move & update layouts |
| `src/components/Footer.vue` | `src/shared/components/Footer.vue` | Shared UI | Move & update layouts |
| `src/components/Hero.vue` | `src/shared/components/Hero.vue` | Shared UI | Move & update layouts |
| `src/components/WhyChoose.vue` | `src/shared/components/WhyChoose.vue` | Shared UI | Move & update layouts |
| `src/components/Testimonials.vue` | `src/shared/components/Testimonials.vue` | Shared UI | Move & update layouts |

---

### 4.2 Admin Dashboard Refactoring (`seadora-admin`)

| Original Path | Target Path | Layer/Feature | Action |
|---|---|---|---|
| `src/services/api.ts` | `src/infrastructure/factories/RepositoryFactory.ts` | Infrastructure | Merge Axios configuration into RepositoryFactory |
| *New File* | `src/infrastructure/api/` | Infrastructure | Create Admin Concrete API repositories (incorporating Category/Destination/Supplier/User updates) |
| `src/views/LoginView.vue` | `src/features/auth/views/LoginView.vue` | Feature: Auth | Move & refactor |
| `src/stores/auth.ts` | `src/features/auth/store/auth.ts` | Feature: Auth | Move & update endpoints |
| `src/views/ToursView.vue` | `src/features/tours/views/ToursView.vue` | Feature: Tours | Move & refactor to use repo |
| `src/views/TourEditView.vue` | `src/features/tours/views/TourEditView.vue` | Feature: Tours | Move & refactor to use repo & upload endpoints |
| `src/views/DestinationsView.vue` | `src/features/destinations/views/DestinationsView.vue` | Feature: Destination | Move & refactor to use repo |
| `src/views/CategoriesView.vue` | `src/features/categories/views/CategoriesView.vue` | Feature: Categories | Move & refactor to use repo |
| `src/views/BookingsView.vue` | `src/features/bookings/views/BookingsView.vue` | Feature: Bookings | Move & refactor to use repo |
| `src/views/BookingDetailsView.vue` | `src/features/bookings/views/BookingDetailsView.vue` | Feature: Bookings | Move & refactor to use repo |
| `src/views/FeedbackView.vue` | `src/features/feedback/views/FeedbackView.vue` | Feature: Feedback | Move & refactor to use repo |
| `src/views/UsersView.vue` | `src/features/users/views/UsersView.vue` | Feature: Users | Move & refactor to use repo |
| `src/views/ReportsView.vue` | `src/features/reports/views/ReportsView.vue` | Feature: Reports | Move & refactor to use repo |
| `src/views/SuppliersView.vue` | `src/features/suppliers/views/SuppliersView.vue` | Feature: Suppliers | Move & refactor to use repo |

---

## 5. Step-by-Step Implementation Tasks

When executing this plan in an active coding session, complete the tasks in this exact order:

### Phase 1: Core Domain Setup
- [ ] **Task 1.1**: Define TypeScript schemas/interfaces in both web applications inside `src/core/models/`.
- [ ] **Task 1.2**: Write the TypeScript interface definitions for each repository class under `src/core/repositories/`.

### Phase 2: Infrastructure Configuration
- [ ] **Task 2.1**: Install Axios on `seadora-website` project if not already present.
- [ ] **Task 2.2**: Write concrete Repository implementations in `src/infrastructure/api/` calling endpoints.
- [ ] **Task 2.3**: Establish `RepositoryFactory.ts` under `src/infrastructure/factories/` returning interface types.
- [ ] **Task 2.4**: Create `helpers.ts` under `src/shared/utils/` containing `getSlug` and `getLocalized`.

### Phase 3: Website Reorganization (`seadora-website`)
- [ ] **Task 3.1**: Reorganize the folder structure of `/src` to introduce `/features` and `/shared`.
- [ ] **Task 3.2**: Create a central Pinia store for tours (`toursStore.ts` under `src/features/tours/store/`) handling the fetching, filtering (search, categories, destinations, dates), and caching.
- [ ] **Task 3.3**: Refactor UI components (`Trips.vue`, `TourDetailsModal.vue`, `Contact.vue`, `Destinations.vue`) and Views to fetch data only through Pinia stores or the Repository Factory.
- [ ] **Task 3.4**: Clean up import paths in `router/index.ts` and `App.vue`.

### Phase 4: Admin Reorganization (`seadora-admin`)
- [ ] **Task 4.1**: Set up `/features` and `/shared` directories in `seadora-admin`.
- [ ] **Task 4.2**: Refactor views to interact with concrete CRUD repositories via the Factory.
- [ ] **Task 4.3**: Clean up import paths in layout and router.

### Phase 5: Verification & Cleanup
- [ ] **Task 5.1**: Remove deprecated/unreferenced duplicate helper imports and inline functions.
- [ ] **Task 5.2**: Perform local development build checks:
  ```powershell
  # Check Customer Website
  cd seadoratravel/src/Web/seadora-website
  npm run build
  
  # Check Admin Dashboard
  cd ../seadora-admin
  npm run build
  ```
- [ ] **Task 5.3**: Verify Docker compose runs the fully optimized SPAs.
