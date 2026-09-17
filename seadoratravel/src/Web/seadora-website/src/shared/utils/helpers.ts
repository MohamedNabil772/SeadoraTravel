export function getSlug(text?: string): string {
  if (!text) return ''
  return text
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '') // remove diacritics
    .replace(/[–—]/g, '-') // replace en-dash and em-dash with hyphen
    .replace(/[^a-z0-9\s-]/g, '') // remove non-alphanumeric
    .trim()
    .replace(/\s+/g, '-') // convert spaces to hyphens
    .replace(/-+/g, '-') // collapse consecutive hyphens
}

export function getLocalized(obj: any, locale: string = 'en', fallback: string = 'en'): string {
  if (!obj) return ''
  if (typeof obj === 'string') return obj
  if (typeof obj === 'object') {
    return obj[locale] || obj[fallback] || obj['en'] || Object.values(obj)[0] || ''
  }
  return String(obj)
}

export const API_BASE_URL = import.meta.env.VITE_API_URL || (import.meta.env.PROD ? 'https://api.seadoratravel.com' : 'http://localhost:8000');

export function getFullImageUrl(url?: string): string {
  if (!url) return '';
  if (url.startsWith('http://') || url.startsWith('https://') || url.startsWith('blob:') || url.startsWith('data:')) {
    return url;
  }
  if (url.startsWith('/images/') || url.startsWith('images/')) {
    return url.startsWith('/') ? url : `/${url}`;
  }
  return `${API_BASE_URL}${url.startsWith('/') ? '' : '/'}${url}`;
}