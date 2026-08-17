import type { Feedback, FeedbackInput } from '../models/Feedback'

export interface IFeedbackRepository {
  getFeedbackForTour(tourId: string): Promise<Feedback[]>
  submitFeedback(feedback: FeedbackInput): Promise<string>
}