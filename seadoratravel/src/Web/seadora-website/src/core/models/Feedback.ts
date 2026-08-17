export interface Feedback {
  id: string
  tourId: string
  customerName: string
  rating: number
  comment: string
  date: string
}

export interface FeedbackInput {
  tourId: string
  customerName: string
  rating: number
  comment: string
}