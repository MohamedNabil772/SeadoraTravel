import type { Category } from '../models/Category'

export interface ICategoryRepository {
  getCategories(): Promise<Category[]>
  getCategoryById(id: string): Promise<Category>
}