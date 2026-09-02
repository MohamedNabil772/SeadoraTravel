# SEO Optimization — Seadora Travel Website

> Implemented: full technical SEO overhaul of `seadoratravel/src/Web/seadora-website`.
> Domain: https://seadoratravel.com · Locales: en (default), fr, de, it, ru

## 1. What was implemented

### Phase 1 — Critical on-page foundation
| Item | Where |
|---|---|
| Complete `<head>`: meta description, robots (`index, follow, max-image-preview:large`), canonical, absolute OG + Twitter card tags, theme-color, `<noscript>` SEO fallback | `seadora-website/index.html` |
| Full hreflang set (en/fr/de/it/ru + x-default) in static head **and** dynamically per route | `index.html` + `src/shared/utils/seo.ts` |
| Per-route `<title>`, description, canonical, OG/Twitter via router `afterEach` | `src/router/index.ts`, `src/shared/utils/seo.ts` (`applyRouteSeo`) |
| Dynamic per-tour SEO: localized title/description, gallery image, `Product` JSON-LD (price, currency, aggregateRating, offer URL) + `BreadcrumbList` | `src/features/tours/views/TourDetailsView.vue` (`applyTourSeo`) |
| Static site-wide JSON-LD: `TravelAgency` (with `sameAs` socials) + `WebSite` (SearchAction → `/tours?search=`) injected on every public route | `src/shared/utils/seo.ts` |
| `robots.txt` (allows public site, disallows `/portal`, `/api`) | `public/robots.txt` |
| Build-time `sitemap.xml` generator with per-locale tour slugs + hreflang alternates; graceful static fallback when API is down | `scripts/generate-sitemap.mjs` (wired into `npm run build`) |
| Portal (`/portal/*`) and feedback page set to `noindex` | router + `seo.ts` |

### Phase 2 — Performance / Core Web Vitals
| Item | Where |
|---|---|
| **Deleted unreferenced 96 MB `hero-sea-video.mp4`** (was shipped in every deploy, never used) | `public/` |
| LCP hints on the hero image: `fetchpriority="high"`, `loading="eager"`, `decoding="async"` | `src/shared/components/Hero.vue` |
| nginx: gzip compression, `server_name`, long-cache for media (real 404 for missing files), cache rules for `robots.txt`/`sitemap.xml`, `X-Robots-Tag` header | `nginx.conf` |
| Branded 1200×630 OG share image | `public/og-share.jpg` |

### Phase 3 — International SEO
| Item | Where |
|---|---|
| Locale-prefix URLs: `/fr/`, `/de/`, `/it/`, `/ru/` (+ bare paths = English default) via optional route param | `src/router/index.ts` |
| Language switchers keep the URL localized (e.g. switch to FR on `/tours` → `/fr/tours`) | `Navbar.vue`, `features/tours/views/TourDetailsView.vue` |
| `<html lang>` synced to active locale | `src/i18n.ts` |
| Localized per-route metadata (titles/descriptions) via new `seo.*` i18n keys in all 5 locales | `src/locales/*.json` |
| Restored 4 missing `de.json` keys (`footer.sections.contact`, `footer.rights`, `footer.privacy`, `footer.terms`) — de is now at full key parity | `src/locales/de.json` |

### Phase 4 — Content & brand consistency
| Item | Where |
|---|---|
| Tour-detail section headings fixed from `h3` → `h2` (hierarchy: one `h1` → `h2` sections) | `features/tours/views/TourDetailsView.vue` |
| Gallery/hero alt-text fallbacks (`img.title || tourTitle`) so images never have empty alt | `TourDetailsView.vue` |
| Deleted dead scaffold `HelloWorld.vue` (stray h1) and the stale duplicate `src/views/TourDetailsView.vue` | `src/` |
| Archived the legacy "SeeDora Travel" standalone page (wrong brand, mojibake, dead links) out of the deploy root | moved to `docs/legacy/seedora-legacy-landing.html` |

## 2. URL & hreflang scheme

- English (default): `https://seadoratravel.com/`, `/tours`, `/tour/<slug>`
- Other locales: `https://seadoratravel.com/fr/…`, `/de/…`, `/it/…`, `/ru/…`
- Every page emits `link rel="alternate" hreflang` for all 5 locales + `x-default` → English.
- Sitemap follows the same scheme with `xhtml:link` alternates.

## 3. Ops notes / follow-ups (recommended, not blocking)

1. **Search Console**: submit `https://seadoratravel.com/sitemap.xml` and verify JS rendering via URL Inspection (content depends on the Content API being reachable by Googlebot).
2. **Image compression**: `public/` still contains heavy originals (hero ~890 KB, destination jpgs ~900–970 KB each, ~90 tour JPGs 100–600 KB). Recommended: convert to WebP (quality ~80) + `srcset`; ImageMagick/`cwebp` was not available on this machine, so this was not done in-place. Example: `cwebp -q 80 hero-egypt-majestic.jpg -o hero-egypt-majestic.webp` + `<picture>` markup.
3. **Canonical host**: nginx note added — configure www→apex + HTTPS 301 at the edge proxy (or add the 443 block per the comment in `nginx.conf`).
4. **Video hero (optional)**: if a hero video is wanted later, re-encode a 6–10 s loop (H.264+WebM, CRF 26–28, ≤3 MB), serve with poster + `preload="none"` after LCP — never bundle >5 MB media in `public/`.
5. **Structured data validation**: run Rich Results Test on `/` and one `/tour/<slug>` URL after deploy.
6. **Facebook profile URL**: footer currently links a tracking share URL; replace with the canonical page URL and add it to `organizationSchema().sameAs` in `seo.ts`.
