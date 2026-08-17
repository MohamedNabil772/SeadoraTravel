# DESIGN.md

> **"A Sun-Drenched Journey"** — High-end editorial curation meets fluid interactivity, channeling the luxury of private Yachting on the Red Sea and the timelessness of ancient Egyptian history.

---

## 1. Visual Theme & Atmosphere

**Style**: Luxury Desert & Sea Editorial (Hybrid of Cream Editorial & Warm Professional)
**Keywords**: Yacht-Club, Sand Gold, Timeless Heritage, High-end Editorial, Breathable Elegance, Fluid Motion
**Tone**: Exquisite, magazine-like, curated, and deeply authentic — NOT sterile tech-industrial, NOT overly bright/playful, and NOT cluttered.
**Feel**: A warm golden afternoon on a private cruiser sailing the Red Sea, reading a beautifully printed boutique travel log with heavy textured pages.

**Interaction Tier**: L2 Fluid Interactive (Smooth scroll reveals, organic parallax layers, and micro-interactions)
**Dependencies**: CSS Transitions + Tailwind CSS v4 variables + Vanilla JS IntersectionObservers (No heavy WebGL/Three.js or GSAP required for L2)

### Egyptra-Inspired Luxury clean layout:
We draw inspiration from Egyptra's high-trust and high-conversion UX (floating search bar in hero, hero USP badges, tour ratings, price comparisons) but elevate it to a clean, luxurious editorial style. Emojis are deprecated in favor of clean thin-stroke vector SVGs.

---

## 2. Color Palette & Roles

```css
:root {
  /* Brand Foundations */
  --sea: #0a5c8a;                          /* Primary Ocean Blue */
  --sea-light: #1a8bc4;                    /* Shallow Reef Accent */
  --sea-deep: #063a5c;                     /* Deep Trench / Dark Mode text */
  --sun: #e8820a;                          /* Sunset Gold / Primary Call-to-Action */
  --sun-light: #f5a435;                    /* Radiant Orange / Hover Accent */
  --sun-pale: #fdf3e0;                     /* Warm Desert Mist / Soft Background */
  --grass: #2e7d4f;                        /* Oasis Green / Nature Accent */
  --grass-light: #4caf78;                  /* Palm Frond / Light Green Accent */
  --gold: #c9a84c;                         /* Luxury Pharaoh Gold / Elegant Outlines */
  --cream: #faf7f2;                        /* Primary Papyrus Background */
  --dark: #0d1f2d;                         /* Night Sky / Dark Section Backgrounds */
  --text: #2a3f4f;                         /* Charcoal Blue / Primary Body Text */
  --muted: #6b8a9a;                        /* Slate Mist / Secondary & Captions */
  --white: #ffffff;                        /* Absolute White */

  /* RGB variants for rgba() utility styling */
  --sea-rgb: 10, 92, 138;
  --sea-deep-rgb: 6, 58, 92;
  --sun-rgb: 232, 130, 10;
  --cream-rgb: 250, 247, 242;
  --dark-rgb: 13, 31, 45;
  --text-rgb: 42, 63, 79;
}
```

**Color Rules:**
1. **No Hardcoded Hexes**: All colors in components and templates MUST reference CSS variables.
2. **Contrast Guidelines**: Body text (`--text`) must always sit on light backgrounds (`--cream`, `--sun-pale`, `--white`). Dark sections (`--dark`, `--sea-deep`) must strictly use `--white` or `--sun-pale` for readability.
3. **Accent Restraint**: `--sun` and `--sun-light` are reserved exclusively for interactive elements (buttons, links, active tabs). Never use them for decorative backgrounds or border strokes.

---

## 3. Typography Rules

**Font Stack:**
```css
@import url('https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,400;0,600;0,700;0,900;1,400&family=Cormorant+Garamond:ital,wght@0,400;0,600;1,400;1,600&family=Jost:wght@300;400;500;600;700&display=swap');

:root {
  --font-serif-display: 'Playfair Display', Georgia, serif;
  --font-serif-accent: 'Cormorant Garamond', Georgia, serif;
  --font-sans: 'Jost', system-ui, -apple-system, sans-serif;
}
```

### Hierarchy

| Role | Font | Size | Weight | Line Height | Letter Spacing |
|------|------|------|--------|-------------|----------------|
| **Hero H1** | `var(--font-serif-display)` | 4.5rem (72px) | 900 | 1.15 | -0.02em |
| **Section H2** | `var(--font-serif-display)` | 2.5rem (40px) | 700 | 1.25 | -0.01em |
| **Subheading H3** | `var(--font-serif-accent)` | 1.75rem (28px) | 600 (Italic) | 1.35 | 0.02em |
| **Body (Lead)** | `var(--font-sans)` | 1.125rem (18px) | 400 | 1.75 | 0.01em |
| **Body (Normal)** | `var(--font-sans)` | 1.0rem (16px) | 400 | 1.7 | 0.01em |
| **Eyebrow / Label**| `var(--font-sans)` | 0.8125rem (13px) | 600 | 1.4 | 0.15em (Caps) |

**Typography Rules:**
1. **Serif vs Sans**: All storytelling, emotional descriptions, and headers must use Serifs. All forms, tables, pricing numbers, UI controls, and utility text must use Jost (Sans-serif).
2. **Italics for Romance**: Use `Cormorant Garamond` (Italics) for testimonials, subheadings, and side annotations.
3. **NEVER use**: Comic Sans, Helvetica, system-ui (as a primary font), or monospace fonts for general layout.

**Text Decoration:**
*   **Hero H1**: Dark Editorial approach (No gradient fill to maintain luxury. Use soft shadow for depth):
    `color: var(--white); text-shadow: 0 2px 12px rgba(6, 58, 92, 0.35);`
*   **Section H2**: Dark text on cream background.
    `color: var(--sea-deep); text-shadow: 0 1px 4px rgba(0, 0, 0, 0.06);`

---

## 4. Component Stylings

### Buttons

```css
/* Primary Button (CTA) */
.btn-primary {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 12px 28px;
  font-family: var(--font-sans);
  font-size: 14px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: var(--white);
  background: linear-gradient(135deg, var(--sun), var(--sun-light));
  border: none;
  border-radius: 4px;
  cursor: pointer;
  box-shadow: 0 4px 14px rgba(232, 130, 10, 0.25);
  transition: transform 0.2s cubic-bezier(0.16, 1, 0.3, 1), 
              box-shadow 0.2s cubic-bezier(0.16, 1, 0.3, 1),
              filter 0.2s ease;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(232, 130, 10, 0.4);
  filter: brightness(1.05);
}

.btn-primary:active {
  transform: translateY(0) scale(0.97);
  box-shadow: 0 2px 8px rgba(232, 130, 10, 0.3);
}

.btn-primary:focus-visible {
  outline: 2px solid var(--sea-light);
  outline-offset: 3px;
}

/* Secondary Button (Outline / Elegant) */
.btn-secondary {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 11px 26px;
  font-family: var(--font-sans);
  font-size: 14px;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: var(--sea-deep);
  background: transparent;
  border: 1px solid var(--gold);
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
}

.btn-secondary:hover {
  background: var(--sea-deep);
  border-color: var(--sea-deep);
  color: var(--cream);
  transform: translateY(-1px);
}
```

### Egyptra-Inspired Search & Tour Page Layout

The tour page incorporates a high-trust horizontal filter layout and cards with detailed checklists:

#### 1. Search & Horizontal Filters Bar
- **Top Search Box**: Centered, rounded-full or rounded-lg search input using `--cream` background, gold strokes, and Jost typography. Includes a search icon on the left and reset on the right.
- **Filter Row**: Horizontal bar directly below search including:
  - **Date Selector**: Pill-shaped button `[ 📅 Select Dates ]` styled in luxury cream/dark.
  - **Filter Trigger**: Pill button showing settings slider icon and a badge count of active filters.
  - **Active Chips**: Removable tags (e.g., `Category: Sea & Diving x`) styled with a light background and gold border.
  - **Horizontal Scroll Pills**: Categories ("Sea & Diving", "Culture & History", "Safari & Adventure") and destinations ("Hurghada", "Luxor", "Cairo", "Sharm El-Sheikh") available as toggleable pills.
- **Summary Row**: Results counter (e.g., "12 results") on the left, sort selector ("Recommended") and "Clear All" reset link on the right, alongside Grid/List view mode toggle buttons.

#### 2. Egyptra Tour Card Elements
- **Image Actions**: Floating circular buttons at the top right of the card:
  - **Heart Icon (Favorite)**: Saves to client-side favorites via `useAuthStore` (turns red/gold on active).
  - **Share Icon (Copy Link)**: Copies the SEO-friendly URL to clipboard with a toast notification.
- **Content Section**:
  - **Meta line**: Clock icon + duration (e.g., `⏱️ 3 Hours`) and status checkmark (e.g., `Instant Confirmation ✓` or `Today 08:30 ✓`).
  - **Title**: Bold title in Jost/Playfair Display.
  - **Rating row**: Green text (using `--grass`) showing average star score, number of reviews, and a yellow `BESTSELLER` tag.
  - **Bullet Point Checks**: Green checkmarks for booking reassurance:
    - `✓ Reserve & pay later`
    - `✓ Hotel pickup`
  - **Price & Booking CTA**: Transparent price ("From €25 / person") positioned next to action buttons ("Details" / "Book Now").

#### 3. View Mode Toggle (Grid vs List)
- **Grid Layout**: 3-column (desktop) or 2-column (tablet) cards.
- **List Layout**: 1-column layout, turning card into horizontal layout (image on left, details and booking buttons on the right) for easy scanning.


---

## 5. Layout Principles

**Grid & Alignment Systems:**
*   **Grid Base**: 12-column grid system for primary sections.
*   **Max Width**: Standard viewport container limit `max-width: 1280px` with responsive padding.
*   **Narrow Container**: `max-width: 720px` for reading ease.

**Spacing Scale:**
*   Section Gaps: 100px (Desktop), 80px (Tablet), 60px (Mobile).
*   Grid Gaps: 32px (Desktop), 24px (Mobile).

---

## 6. Depth & Elevation

| Level | Treatment | Use Case |
|-------|-----------|----------|
| **Flat** | No shadow, crisp solid boundaries or soft `1px solid var(--gold)` lines. | Input fields, table rows, tag badges. |
| **Subtle** | `box-shadow: 0 4px 20px rgba(13, 31, 45, 0.03)` | Default cards, filter panels. |
| **Elevated** | `box-shadow: 0 12px 32px rgba(13, 31, 45, 0.08)` | Hover card states, sticky navigation menus. |
| **Overlay** | `box-shadow: 0 20px 48px rgba(6, 58, 92, 0.18)` | Lightboxes, booking modals, hamburger drawers. |

---

## 7. Animation & Interaction (L2 Fluid Interactive)

**Motion Philosophy**: Quiet elegance. Animations must feel smooth, organic, and serve to frame information as the user scrolls.

### Base Setup (IntersectionObserver)
Used in Vue via `v-reveal` directive to apply `.in-view` scroll classes.

### Reduced Motion Guard
```css
@media (prefers-reduced-motion: reduce) {
  *, *::before, *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
    scroll-behavior: auto !important;
    transition: none !important;
  }
}
```

---

## 8. Do's and Don'ts

### Do
1. **Always use CSS variables** for standard colors and font families.
2. **Prioritize whitespace**: Maintain comfortable margins (`100px` on desktop) between page sections to allow the design to "breathe".
3. **Use custom vector SVGs** instead of emoji character placeholders for UI icons.
4. **Implement smooth transitions** on all hover, active, and focus states.
5. **Ensure a crisp tactile snap** on button press via active scaling (e.g. `scale(0.97)`).

### Don't
1. ❌ **Don't hardcode Hex colors** inside templates or custom Vue components.
2. ❌ **Don't use emoji placeholders** for destination icons or categories.
3. ❌ **Don't use aggressive gradient overlays** on luxury typography.
4. ❌ **Don't allow horizontal overflow** on mobile screens.
5. ❌ **Don't use low-quality pixelated placeholders**; use high-resolution optimized SVG illustrations or Unsplash.
6. ❌ **Don't write cluttered buttons**; maintain elegant capitalization and letter spacing.
7. ❌ **Don't use custom scroll jacking** (like forced scroll-to-section) for L2 interactions; stick to native smooth scrolling.

---

## 9. Responsive Behavior

**Breakpoints:**
*   **Desktop**: `>= 1024px`
*   **Tablet**: `768px - 1023px`
*   **Mobile**: `< 768px`

**Touch Target Standards**: All links, buttons, and close buttons on mobile devices must have a minimum interactive surface area of `44px x 44px`.
