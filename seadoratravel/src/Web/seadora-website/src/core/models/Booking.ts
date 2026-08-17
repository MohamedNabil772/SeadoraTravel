export interface Booking {
  id: string
  tourId: string
  customerName: string
  customerEmail: string
  bookingDate: string
  status?: "Pending" | "Confirmed" | "Completed" | "Cancelled"
}

export interface BookingInput {
  tourId: string
  customerName: string
  customerEmail: string
  bookingDate: string
}