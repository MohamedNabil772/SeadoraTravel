import type { AxiosInstance } from 'axios'
import type { IBookingRepository } from '../../core/repositories/IBookingRepository'
import type { Booking, BookingInput } from '../../core/models/Booking'

export class ApiBookingRepository implements IBookingRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getBookings(): Promise<Booking[]> {
    const res = await this.client.get('/api/booking/api/bookings')
    return Array.isArray(res.data) ? res.data : (res.data?.items || [])
  }

  async getBookingById(id: string): Promise<Booking> {
    const res = await this.client.get(`/api/booking/api/bookings/${id}`)
    return res.data
  }

  async createBooking(booking: BookingInput): Promise<string> {
    const res = await this.client.post('/api/booking/api/bookings', booking)
    return res.data.id || res.data
  }
}