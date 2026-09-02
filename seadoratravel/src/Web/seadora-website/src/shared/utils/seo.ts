/**
 * SEO head management — dependency-free per-route metadata, canonical,
 * hreflang, Open Graph / Twitter cards and JSON-LD structured data.
 */
import type { RouteLocationNormalizedGeneric } from 'vue-router'
import { i18n } from '@/i18n'

export const SITE_URL = 'https://seadoratravel.com'
export const SITE_NAME = 'Seadora Travel'
export const DEFAULT_LOCALE = 'en'
export const SUPPORTED_LOCALES = ['en', 'fr', 'de', 'it', 'ru'] as const

/** BCP47 tag used for og:locale */
const OG_LOCALES: Record<string, string> = {
  en: 'en_US',
  fr: 'fr_FR',
  de: 'de_DE',
  it: 'it_IT',
  ru: 'ru_RU'
}

export interface PageSeo {
  title: string
  description: string
  /** Canonical path WITHOUT locale prefix (e.g. '/tours' or '/tour/slug') */
  path?: string
  /** Absolute or root-relative image URL for OG/Twitter */
  image?: string
  /** Open Graph type, defaults to 'website' */
  type?: string
  /** robots content, e.g. 'noindex, follow' */
  robots?: string
  /** JSON-LD objects to inject for this page */
  jsonLd?: object[]
}

/* ------------------------------------------------------------------ */
/* Locale path helpers                                                 */
/* ------------------------------------------------------------------ */

/** Remove a supported locale prefix from a path (en/fr/de/it/ru). */
export function stripLocalePrefix(path: string): string {
  const m = path.match(/^\/(en|fr|de|it|ru)(?=\/|$)/)
  return m ? path.slice(m[1].length + 1) || '/' : path
}

/** Build the canonical URL for the given locale + base path. */
export function buildCanonical(path: string, locale: string): string {
  return locale === DEFAULT_LOCALE
    ? `${SITE_URL}${path}`
    : `${SITE_URL}/${locale}${path === '/' ? '' : path}`
}

/** Full alternate list (hreflang) for a base path. */
export function buildAlternates(path: string): { hreflang: string; href: string }[] {
  const list: { hreflang: string; href: string }[] = SUPPORTED_LOCALES.map(l => ({
    hreflang: l,
    href: buildCanonical(path, l)
  }))
  // x-default points at the default (English) URL
  list.push({ hreflang: 'x-default', href: buildCanonical(path, DEFAULT_LOCALE) })
  return list
}

/** URL for a localized route navigation (used by language switchers). */
export function localizedPath(basePath: string, locale: string): string {
  return buildCanonical(basePath, locale).replace(SITE_URL, '') || '/'
}

/* ------------------------------------------------------------------ */
/* Head element helpers                                                */
/* ------------------------------------------------------------------ */

function upsertMeta(attr: 'name' | 'property', key: string, content: string) {
  let el = document.head.querySelector<HTMLMetaElement>(`meta[${attr}="${key}"]`)
  if (!el) {
    el = document.createElement('meta')
    el.setAttribute(attr, key)
    document.head.appendChild(el)
  }
  el.setAttribute('content', content)
}

function upsertLink(rel: string, href: string, hreflang?: string) {
  const selector = hreflang
    ? `link[rel="${rel}"][hreflang="${hreflang}"]`
    : `link[rel="${rel}"]`
  let el = document.head.querySelector<HTMLLinkElement>(selector)
  if (!el) {
    el = document.createElement('link')
    el.setAttribute('rel', rel)
    if (hreflang) el.setAttribute('hreflang', hreflang)
    document.head.appendChild(el)
  }
  el.setAttribute('href', href)
}

/* ------------------------------------------------------------------ */
/* JSON-LD                                                             */
/* ------------------------------------------------------------------ */

export function clearJsonLd() {
  document.head
    .querySelectorAll('script[data-seo-jsonld="true"]')
    .forEach(el => el.remove())
}

export function setJsonLd(objects: object[]) {
  clearJsonLd()
  objects.forEach(obj => {
    const script = document.createElement('script')
    script.type = 'application/ld+json'
    script.setAttribute('data-seo-jsonld', 'true')
    script.textContent = JSON.stringify(obj)
    document.head.appendChild(script)
  })
}

/* ------------------------------------------------------------------ */
/* Page meta entry point                                               */
/* ------------------------------------------------------------------ */

export function setPageMeta(seo: PageSeo) {
  if (typeof document === 'undefined') return

  const locale = (i18n.global.locale as any).value || DEFAULT_LOCALE
  const basePath = seo.path ?? stripLocalePrefix(
    typeof window !== 'undefined' ? window.location.pathname : '/'
  )
  const canonical = buildCanonical(basePath, locale)
  const image = seo.image?.startsWith('http')
    ? seo.image
    : `${SITE_URL}${seo.image ?? '/og-share.jpg'}`

  document.title = seo.title
  upsertMeta('name', 'description', seo.description)
  upsertMeta('name', 'robots', seo.robots ?? 'index, follow, max-image-preview:large')
  upsertLink('canonical', canonical)

  // hreflang alternates
  for (const alt of buildAlternates(basePath)) {
    upsertLink('alternate', alt.href, alt.hreflang)
  }

  // Open Graph
  upsertMeta('property', 'og:title', seo.title)
  upsertMeta('property', 'og:description', seo.description)
  upsertMeta('property', 'og:url', canonical)
  upsertMeta('property', 'og:type', seo.type ?? 'website')
  upsertMeta('property', 'og:image', image)
  upsertMeta('property', 'og:image:width', '1200')
  upsertMeta('property', 'og:image:height', '630')
  upsertMeta('property', 'og:site_name', SITE_NAME)
  upsertMeta('property', 'og:locale', OG_LOCALES[locale] ?? 'en_US')
  for (const l of SUPPORTED_LOCALES) {
    if (l !== locale) upsertMeta('property', 'og:locale:alternate', OG_LOCALES[l] ?? l)
  }

  // Twitter card
  upsertMeta('name', 'twitter:card', 'summary_large_image')
  upsertMeta('name', 'twitter:title', seo.title)
  upsertMeta('name', 'twitter:description', seo.description)
  upsertMeta('name', 'twitter:image', image)
  upsertMeta('name', 'twitter:site', '@seadora.travel.eg')

  // Structured data
  if (seo.jsonLd) setJsonLd(seo.jsonLd)
  else clearJsonLd()
}

/* ------------------------------------------------------------------ */
/* Static site-wide schema blocks                                      */
/* ------------------------------------------------------------------ */

export function organizationSchema(): object {
  return {
    '@context': 'https://schema.org',
    '@type': 'TravelAgency',
    name: SITE_NAME,
    url: SITE_URL,
    logo: `${SITE_URL}/logo-horizontal.png`,
    image: `${SITE_URL}/logo-full.png`,
    description:
      'Seadora Travel crafts luxury Egypt experiences — Red Sea cruises, diving, Nile voyages, Cairo and Luxor private tours with VIP concierge service.',
    email: 'info@seadoratravel.com',
    telephone: '+201068940967',
    address: { '@type': 'PostalAddress', addressCountry: 'EG', addressLocality: 'Hurghada' },
    sameAs: [
      'https://www.facebook.com/share/1ERTGyUJvs/',
      'https://www.instagram.com/seadora.travel.egypt',
      'https://www.tiktok.com/@seadora.travel.eg',
      'https://wa.me/201068940967'
    ]
  }
}

export function websiteSchema(): object {
  return {
    '@context': 'https://schema.org',
    '@type': 'WebSite',
    name: SITE_NAME,
    url: SITE_URL,
    inLanguage: SUPPORTED_LOCALES,
    potentialAction: {
      '@type': 'SearchAction',
      target: {
        '@type': 'EntryPoint',
        urlTemplate: `${SITE_URL}/tours?search={search_term_string}`
      },
      'query-input': 'required name=search_term_string'
    }
  }
}

export function breadcrumbSchema(items: { name: string; path: string }[]): object {
  return {
    '@context': 'https://schema.org',
    '@type': 'BreadcrumbList',
    itemListElement: items.map((item, i) => ({
      '@type': 'ListItem',
      position: i + 1,
      name: item.name,
      item: buildCanonical(item.path, (i18n.global.locale as any).value || DEFAULT_LOCALE)
    }))
  }
}

/* ------------------------------------------------------------------ */
/* Route-driven defaults                                               */
/* ------------------------------------------------------------------ */

const t = (key: string, fallback: string): string => {
  const msg = (i18n.global as any).t(key)
  return msg === key ? fallback : msg
}

/**
 * Apply default SEO metadata for a route. Views with dynamic content
 * (e.g. tour details) call setPageMeta themselves after data loads.
 */
export function applyRouteSeo(to: RouteLocationNormalizedGeneric) {
  const name = String(to.name ?? '')

  // Private customer area: never index
  if (name.startsWith('portal') || to.path.startsWith('/portal')) {
    setPageMeta({
      title: `My Portal | ${SITE_NAME}`,
      description: 'Your private Seadora Travel customer portal.',
      robots: 'noindex, nofollow'
    })
    return
  }

  const basePath = stripLocalePrefix(to.path)
  const staticSchemas = [organizationSchema(), websiteSchema()]

  switch (name) {
    case 'home':
      setPageMeta({
        title: t('seo.home.title', 'Seadora Travel – Luxury Egypt Experiences & Red Sea Tours'),
        description: t('seo.home.description',
          'Luxury Egypt tours with Seadora Travel: Red Sea cruises, diving, Nile voyages and private Cairo & Luxor experiences. VIP concierge, hotel pickup and expert local guides.'),
        path: basePath,
        jsonLd: staticSchemas
      })
      break
    case 'tours':
      setPageMeta({
        title: t('seo.tours.title', 'Egypt Tours & Excursions | Seadora Travel'),
        description: t('seo.tours.description',
          'Browse curated Egypt tours: snorkeling & diving in Hurghada, Pyramids day trips, Nile cruises, desert safaris and private luxury experiences.'),
        path: basePath,
        jsonLd: staticSchemas
      })
      break
    case 'coming-soon':
      setPageMeta({
        title: t('seo.comingSoon.title', 'Something Extraordinary Is Coming | Seadora Travel'),
        description: t('seo.comingSoon.description',
          'A new luxury travel experience from Seadora Travel is on the way. Stay tuned.'),
        path: basePath,
        robots: 'noindex, follow',
        jsonLd: staticSchemas
      })
      break
    case 'feedback':
      setPageMeta({
        title: t('seo.feedback.title', 'Share Your Experience | Seadora Travel'),
        description: t('seo.feedback.description',
          'Tell us about your Seadora Travel tour. Your feedback helps us craft extraordinary journeys.'),
        path: basePath,
        robots: 'noindex, follow'
      })
      break
    case 'tour-details':
      // Dynamic meta is applied by TourDetailsView once the tour loads;
      // clear stale JSON-LD from the previous page meanwhile.
      clearJsonLd()
      break
    default:
      setPageMeta({
        title: `${SITE_NAME} – Where the Red Sea Becomes Your Story`,
        description: t('seo.home.description',
          'Luxury Egypt tours with Seadora Travel: Red Sea cruises, diving, Nile voyages and private Cairo & Luxor experiences.'),
        path: basePath
      })
  }
}