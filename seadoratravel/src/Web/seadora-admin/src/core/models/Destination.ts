export interface Destination {
  id: string
  names: Record<string, string>
  descriptions?: Record<string, string>
  highlights?: Record<string, string>
  imageUrl?: string
  flag?: string
  flagEmoji?: string
  isFeatured?: boolean
  toursCount?: number
}