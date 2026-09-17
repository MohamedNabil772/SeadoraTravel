import { API_URL } from '@/services/api'

export function getSlug(text?: string): string {
  if (!text) return ''
  return text.toLowerCase().replace(/\s+/g, '-').replace(/[^a-z0-9-]/g, '')
}

export function getLocalized(obj: Record<string, string> | undefined, locale: string = 'en', fallback: string = 'en'): string {
  if (!obj) return ''
  return obj[locale] || obj[fallback] || Object.values(obj)[0] || ''
}

export const DEFAULT_FALLBACK_IMAGE = 'https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=600&q=80'

export function resolveImageUrl(url?: string): string {
  if (!url) return DEFAULT_FALLBACK_IMAGE
  if (url.startsWith('blob:') || url.startsWith('data:')) return url
  if (url.startsWith('http://localhost:8000')) {
    return url.replace('http://localhost:8000', API_URL)
  }
  if (url.startsWith('http://') || url.startsWith('https://')) return url
  if (url.startsWith('/images/') || url.startsWith('images/')) {
    return url.startsWith('/') ? url : `/${url}`
  }
  if (url.startsWith('/api/files/') || url.startsWith('api/files/')) {
    const cleanPath = url.startsWith('/') ? url : `/${url}`
    return `${API_URL}${cleanPath}`
  }
  return `${API_URL}/${url.replace(/^\/+/, '')}`
}