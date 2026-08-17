export interface Tour {
  id: string
  categoryId: string
  destinationId: string
  price: number
  originalPrice: number | null
  discountPercentage: number | null
  currency: string
  
  names: Record<string, string>
  descriptions: Record<string, string>
  duration: string
  startTime: string
  
  emoji?: string
  bgGradient?: string
  imageUrl?: string
  mediaUrls?: string[]
  includes?: string[]
  badge?: string

  rating: number
  reviewCount: number

  isTopRated: boolean
  isBestseller: boolean
  isInHighDemand: boolean

  reserveAndPayLater: boolean
  hotelPickup: boolean
  freeCancellation: boolean
  isPrivateOption: boolean

  supplierId: string | null
  supplierPercentage: number
  maxAllocations: number
}