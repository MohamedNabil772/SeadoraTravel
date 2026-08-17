import type { Tour } from '../models/Tour'

export interface ITourRepository {
  getTours(): Promise<Tour[]>
  getTourById(id: string): Promise<Tour>
}