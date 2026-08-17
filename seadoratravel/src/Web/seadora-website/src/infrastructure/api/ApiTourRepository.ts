import type { AxiosInstance } from 'axios'
import type { ITourRepository } from '../../core/repositories/ITourRepository'
import type { Tour } from '../../core/models/Tour'

export class ApiTourRepository implements ITourRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getTours(): Promise<Tour[]> {
    const res = await this.client.get('/api/content/api/tours')
    return res.data
  }

  async getTourById(id: string): Promise<Tour> {
    const res = await this.client.get(`/api/content/api/tours/${id}`)
    return res.data
  }
}