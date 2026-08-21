export interface Destination {
  id: string
  names: Record<string, string>
  descriptions?: Record<string, string>
  highlights?: Record<string, string> | string[]
  imageUrl?: string
  flagEmoji?: string
  flag?: string
  tourCount?: number
}