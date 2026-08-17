import type { AxiosInstance } from 'axios'
import type { ICategoryRepository } from '../../core/repositories/ICategoryRepository'
import type { Category } from '../../core/models/Category'

export class ApiCategoryRepository implements ICategoryRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getCategories(): Promise<Category[]> {
    const res = await this.client.get('/api/content/api/categories')
    return res.data
  }

  async getCategoryById(id: string): Promise<Category> {
    const res = await this.client.get(`/api/content/api/categories/${id}`)
    return res.data
  }
}