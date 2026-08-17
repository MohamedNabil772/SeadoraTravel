export interface Destination {
  id: string
  names: Record<string, string>
  descriptions?: Record<string, string>
  imageUrl?: string
  flag?: string
}