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

export function resolveImageUrl(url?: string | null): string {
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

export const CATEGORY_ICON_MAP: Record<string, string> = {
  boat: '⛵',
  dolphin: '🐬',
  diving: '🤿',
  safari: '🐪',
  spa: '🧖',
  culture: '🏛️',
  beach: '🏖️',
  hotel: '🏨',
  island: '🏝️',
  cruise: '🛳️',
  ship: '🛳️',
  palm: '🌴',
  castle: '🏰',
  wine: '🍷',
  gem: '💎',
  plane: '✈️',
  flight: '✈️',
  sunset: '🌅',
  cocktail: '🍹',
  surf: '🏄',
  map: '🗺️',
  bell: '🛎️',
  cheers: '🥂',
  monument: '🏛️',
  star: '🌟',
  lobster: '🦞',
  pyramid: '🏛️',
  aquapark: '🌊',
  watersport: '🏄'
}

export function isImageUrl(str?: string | null): boolean {
  if (!str) return false
  return (
    str.startsWith('http://') ||
    str.startsWith('https://') ||
    str.startsWith('data:image/') ||
    str.startsWith('/api/files/') ||
    str.startsWith('api/files/') ||
    str.startsWith('/images/') ||
    str.startsWith('images/') ||
    /\.(png|jpe?g|svg|webp|gif)(\?.*)?$/i.test(str)
  )
}

export function getCategoryDisplayIcon(icon?: string | null): string {
  if (!icon) return '🏷️'
  const key = icon.toLowerCase().trim()
  return CATEGORY_ICON_MAP[key] || icon
}