export interface Tour {
  id: string
  categoryId: string
  destinationId: string
  price: number
  names: Record<string, string>
  descriptions: Record<string, string>
  duration: string
  emoji?: string
  bgGradient?: string
  imageUrl?: string
  mediaUrls?: string[]
  includes?: string[]
}