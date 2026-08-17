import type { AxiosInstance } from 'axios'
import type { IFeedbackRepository } from '../../core/repositories/IFeedbackRepository'
import type { Feedback, FeedbackInput } from '../../core/models/Feedback'

export class ApiFeedbackRepository implements IFeedbackRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getFeedbackForTour(tourId: string): Promise<Feedback[]> {
    const res = await this.client.get(`/api/content/api/feedback/tour/${tourId}`)
    return res.data
  }

  async submitFeedback(feedback: FeedbackInput): Promise<string> {
    const res = await this.client.post('/api/content/api/feedback', feedback)
    return res.data.id || res.data
  }
}