const fs = require('fs');
const path = require('path');

const projects = [
  'D:/Seadora Travel/seadoratravel/src/Web/seadora-website/src',
  'D:/Seadora Travel/seadoratravel/src/Web/seadora-admin/src'
];

const files = {
  'infrastructure/api/ApiTourRepository.ts': `import type { AxiosInstance } from 'axios'
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
    const res = await this.client.get(\`/api/content/api/tours/\${id}\`)
    return res.data
  }
}`,
  'infrastructure/api/ApiCategoryRepository.ts': `import type { AxiosInstance } from 'axios'
import type { ICategoryRepository } from '../../core/repositories/ICategoryRepository'
import type { Category } from '../../core/models/Category'

export class ApiCategoryRepository implements ICategoryRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getCategories(): Promise<Category[]> {
    const res = await this.client.get('/api/content/api/categories')
    return res.data
  }

  async getCategoryById(id: string): Promise<Category> {
    const res = await this.client.get(\`/api/content/api/categories/\${id}\`)
    return res.data
  }
}`,
  'infrastructure/api/ApiDestinationRepository.ts': `import type { AxiosInstance } from 'axios'
import type { IDestinationRepository } from '../../core/repositories/IDestinationRepository'
import type { Destination } from '../../core/models/Destination'

export class ApiDestinationRepository implements IDestinationRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getDestinations(): Promise<Destination[]> {
    const res = await this.client.get('/api/content/api/destinations')
    return res.data
  }

  async getDestinationById(id: string): Promise<Destination> {
    const res = await this.client.get(\`/api/content/api/destinations/\${id}\`)
    return res.data
  }
}`,
  'infrastructure/api/ApiBookingRepository.ts': `import type { AxiosInstance } from 'axios'
import type { IBookingRepository } from '../../core/repositories/IBookingRepository'
import type { Booking, BookingInput } from '../../core/models/Booking'

export class ApiBookingRepository implements IBookingRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getBookings(): Promise<Booking[]> {
    const res = await this.client.get('/api/booking/api/bookings')
    return res.data
  }

  async getBookingById(id: string): Promise<Booking> {
    const res = await this.client.get(\`/api/booking/api/bookings/\${id}\`)
    return res.data
  }

  async createBooking(booking: BookingInput): Promise<string> {
    const res = await this.client.post('/api/booking/api/bookings', booking)
    return res.data.id || res.data
  }
}`,
  'infrastructure/api/ApiFeedbackRepository.ts': `import type { AxiosInstance } from 'axios'
import type { IFeedbackRepository } from '../../core/repositories/IFeedbackRepository'
import type { Feedback, FeedbackInput } from '../../core/models/Feedback'

export class ApiFeedbackRepository implements IFeedbackRepository {
  private client: AxiosInstance;
  constructor(client: AxiosInstance) {
    this.client = client;
  }

  async getFeedbackForTour(tourId: string): Promise<Feedback[]> {
    const res = await this.client.get(\`/api/content/api/feedback/tour/\${tourId}\`)
    return res.data
  }

  async submitFeedback(feedback: FeedbackInput): Promise<string> {
    const res = await this.client.post('/api/content/api/feedback', feedback)
    return res.data.id || res.data
  }
}`
};

for (const proj of projects) {
  for (const [relPath, content] of Object.entries(files)) {
    const fullPath = path.join(proj, relPath);
    fs.writeFileSync(fullPath, content);
  }
}
console.log('Update complete.');
