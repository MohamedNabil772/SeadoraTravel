import type { Booking, BookingInput } from '../models/Booking'

export interface IBookingRepository {
  getBookings(): Promise<Booking[]>
  getBookingById(id: string): Promise<Booking>
  createBooking(booking: BookingInput): Promise<string>
}