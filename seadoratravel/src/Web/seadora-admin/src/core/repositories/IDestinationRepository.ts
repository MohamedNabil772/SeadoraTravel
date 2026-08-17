import type { Destination } from '../models/Destination'

export interface IDestinationRepository {
  getDestinations(): Promise<Destination[]>
  getDestinationById(id: string): Promise<Destination>
}