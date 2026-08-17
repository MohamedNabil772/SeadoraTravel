export function getSlug(text?: string): string {
  if (!text) return ''
  return text.toLowerCase().replace(/\s+/g, '-').replace(/[^a-z0-9-]/g, '')
}

export function getLocalized(obj: Record<string, string> | undefined, locale: string = 'en', fallback: string = 'en'): string {
  if (!obj) return ''
  return obj[locale] || obj[fallback] || Object.values(obj)[0] || ''
}