/**
 * Build-time sitemap generator for Seadora Travel.
 *
 * - Emits static routes for every supported locale (+ hreflang alternates).
 * - Fetches published tours from the Content API and emits a URL per tour
 *   per locale, using the same slug normalization as the website
 *   (shared/utils/helpers.ts → getSlug).
 * - Falls back to a static-only sitemap when the API is unreachable, so
 *   `npm run build` never fails because of the sitemap.
 *
 * Usage: node scripts/generate-sitemap.mjs
 * Env:   SITEMAP_API_URL (default https://api.seadoratravel.com)
 *        SITEMAP_SITE_URL (default https://seadoratravel.com)
 */
import { writeFileSync } from 'node:fs'
import { fileURLToPath } from 'node:url'
import { dirname, join } from 'node:path'

const SITE_URL = process.env.SITEMAP_SITE_URL || 'https://seadoratravel.com'
const API_URL = process.env.SITEMAP_API_URL || 'https://api.seadoratravel.com'
const LOCALES = ['en', 'fr', 'de', 'it', 'ru']
const DEFAULT_LOCALE = 'en'

const __dirname = dirname(fileURLToPath(import.meta.url))
const OUT = join(__dirname, '..', 'public', 'sitemap.xml')

function getSlug(text) {
  if (!text) return ''
  return text
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[–—]/g, '-')
    .replace(/[^a-z0-9\s-]/g, '')
    .trim()
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
}

function pagePath(path, locale) {
  return locale === DEFAULT_LOCALE ? path : `/${locale}${path === '/' ? '' : path}`
}

function alternatesXml(path) {
  const entries = LOCALES.map(
    l => `    <xhtml:link rel="alternate" hreflang="${l}" href="${SITE_URL}${pagePath(path, l)}"/>`
  )
  entries.push(
    `    <xhtml:link rel="alternate" hreflang="x-default" href="${SITE_URL}${pagePath(path, DEFAULT_LOCALE)}"/>`
  )
  return entries.join('\n')
}

function staticEntry(path, priority) {
  return `  <url>
    <loc>${SITE_URL}${pagePath(path, DEFAULT_LOCALE)}</loc>
${alternatesXml(path)}
    <priority>${priority}</priority>
  </url>`
}

function tourEntries(tour) {
  const urls = new Map()
  for (const locale of LOCALES) {
    const name = tour.names?.[locale] || tour.names?.en
    if (!name) continue
    const slug = getSlug(name) || tour.id
    urls.set(locale, `/tour/${slug}`)
  }
  if (urls.size === 0) return ''

  const ref = urls.get(DEFAULT_LOCALE) || urls.values().next().value
  const altXml = LOCALES.filter(l => urls.has(l) || l === DEFAULT_LOCALE)
    .map(l => {
      const p = urls.get(l) || urls.get(DEFAULT_LOCALE)
      return `    <xhtml:link rel="alternate" hreflang="${l}" href="${SITE_URL}${pagePath(p, l)}"/>`
    })
  altXml.push(
    `    <xhtml:link rel="alternate" hreflang="x-default" href="${SITE_URL}${pagePath(urls.get(DEFAULT_LOCALE) || ref, DEFAULT_LOCALE)}"/>`
  )

  return `  <url>
    <loc>${SITE_URL}${pagePath(ref, DEFAULT_LOCALE)}</loc>
${altXml.join('\n')}
    <priority>0.8</priority>
  </url>`
}

async function fetchTours() {
  try {
    const res = await fetch(`${API_URL}/api/content/api/tours`, {
      signal: AbortSignal.timeout(10_000)
    })
    if (!res.ok) throw new Error(`HTTP ${res.status}`)
    const data = await res.json()
    if (!Array.isArray(data)) throw new Error('Unexpected API response')
    console.log(`Sitemap: fetched ${data.length} tour(s) from ${API_URL}`)
    return data
  } catch (err) {
    console.log(`Sitemap: could not fetch tours (${err.message}) — using static routes only`)
    return []
  }
}

const tours = await fetchTours()
const urls = [
  staticEntry('/', '1.0'),
  staticEntry('/tours', '0.9'),
  ...tours.map(tourEntries).filter(Boolean)
]

const xml = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9" xmlns:xhtml="http://www.w3.org/1999/xhtml">
${urls.join('\n')}
</urlset>
`

writeFileSync(OUT, xml, 'utf8')
console.log(`Sitemap written: ${OUT} (${urls.length} URLs)`)