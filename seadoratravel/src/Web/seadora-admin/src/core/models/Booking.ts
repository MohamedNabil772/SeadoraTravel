export interface Booking {
  id: string
  tourId: string
  customerName: string
  customerEmail: string
  bookingDate: string
  status?: string
}

export interface BookingInput {
  tourId: string
  customerName: string
  customerEmail: string
  bookingDate: string
}