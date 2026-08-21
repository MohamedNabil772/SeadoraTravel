import type { AxiosInstance } from 'axios'
import type { IDestinationRepository } from '../../core/repositories/IDestinationRepository'
import type { Destination } from '../../core/models/Destination'

export class ApiDestinationRepository implements IDestinationRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getDestinations(): Promise<Destination[]> {
    const res = await this.client.get('/api/content/api/destinations')
    return Array.isArray(res.data) ? res.data : (res.data?.items || [])
  }

  async getDestinationById(id: string): Promise<Destination> {
    const res = await this.client.get(`/api/content/api/destinations/${id}`)
    return res.data
  }
}