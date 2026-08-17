import axios from 'axios'
import { ApiTourRepository } from '../api/ApiTourRepository'
import { ApiCategoryRepository } from '../api/ApiCategoryRepository'
import { ApiDestinationRepository } from '../api/ApiDestinationRepository'
import { ApiBookingRepository } from '../api/ApiBookingRepository'
import { ApiFeedbackRepository } from '../api/ApiFeedbackRepository'

import type { ITourRepository } from '../../core/repositories/ITourRepository'
import type { ICategoryRepository } from '../../core/repositories/ICategoryRepository'
import type { IDestinationRepository } from '../../core/repositories/IDestinationRepository'
import type { IBookingRepository } from '../../core/repositories/IBookingRepository'
import type { IFeedbackRepository } from '../../core/repositories/IFeedbackRepository'

const API_URL = import.meta.env?.VITE_API_URL || 'http://localhost:8000'

const apiClient = axios.create({
  baseURL: API_URL
})

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token') || localStorage.getItem('seadora_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export const RepositoryFactory = {
  getTourRepository(): ITourRepository {
    return new ApiTourRepository(apiClient)
  },
  getCategoryRepository(): ICategoryRepository {
    return new ApiCategoryRepository(apiClient)
  },
  getDestinationRepository(): IDestinationRepository {
    return new ApiDestinationRepository(apiClient)
  },
  getBookingRepository(): IBookingRepository {
    return new ApiBookingRepository(apiClient)
  },
  getFeedbackRepository(): IFeedbackRepository {
    return new ApiFeedbackRepository(apiClient)
  }
}
