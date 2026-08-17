# Seadora AI Concierge Architecture & Enhancements Plan

## 1. Squad Overview & Responsibilities
- **Frontend Squad (5 Agents)**:
  - **Agent 1 (UI/UX Pro Max)**: Responsive desktop floating window & mobile bottom sheet with drag-to-dismiss gesture.
  - **Agent 2 (Emil Kowalski)**: Spring popover physics, staggered message bubbles, 3-dot wave typing indicator, copy-to-clipboard actions.
  - **Agent 3 (Visual Design)**: Glassmorphism (`backdrop-blur-xl`), royal Red Sea palette (`#062d4d`, gold `#c9a84c`, emerald `#10b981`), rating stars, and luxury badges.
  - **Agent 4 (Interactive State)**: Horizontal momentum tour card slider, in-chat interactive date & slot picker, and auto-complete search.
  - **Agent 5 (QA & Build)**: Strict TypeScript interfaces, clean imports, and `npm run build` zero-error verification.

- **Backend Squad (5 Agents)**:
  - **Agent 1 (Ponytail Domain)**: `ConciergeService` intent detection engine without bloat.
  - **Agent 2 (Search Engine)**: Intelligent fuzzy tour search by title, destination, category, and budget.
  - **Agent 3 (API Controller)**: `POST /api/content/api/concierge/chat` MediatR endpoint & YARP Gateway configuration.
  - **Agent 4 (Policy & Availability)**: Real-time date allocation checking and policy accuracy (72h cancellation, cash/card, coast guard permits).
  - **Agent 5 (Testing & Verification)**: Smoke tests, `.NET` compilation, and health checks.

---

## 2. API Contract: `POST /api/content/api/concierge/chat`
```json
// Request
{
  "message": "Show me safari tours in Hurghada",
  "language": "en",
  "selectedDate": "2026-08-20",
  "tourId": null
}

// Response
{
  "replyText": "Here are our top luxury desert safari experiences in Hurghada:",
  "intent": "TourSearch",
  "suggestedTours": [
    {
      "id": "00000000-0000-0000-0000-000000000104",
      "slug": "hurghada-quad-bike-safari-desert-adventure",
      "title": "Hurghada Quad Bike Safari & Desert Adventure",
      "priceEur": 35.00,
      "rating": 4.9,
      "duration": "3 Hours",
      "destinationName": "Hurghada",
      "categoryName": "Desert Safari",
      "mainImage": "https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9?auto=format&fit=crop&w=800&q=80"
    }
  ],
  "quickReplies": [
    "📅 Check Weekend Availability",
    "💳 Payment & Cancellation",
    "💬 WhatsApp Support"
  ]
}
```
