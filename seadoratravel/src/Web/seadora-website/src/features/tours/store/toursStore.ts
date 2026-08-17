import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Tour } from '../../../core/models/Tour'
import { RepositoryFactory } from '../../../infrastructure/factories/RepositoryFactory'

export const useToursStore = defineStore('tours', () => {
  const tours = ref<Tour[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const tourRepo = RepositoryFactory.getTourRepository()

  async function fetchTours() {
    loading.value = true
    error.value = null
    try {
      const data = await tourRepo.getTours()
      tours.value = data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch tours'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  async function getTourById(id: string) {
    loading.value = true
    try {
      return await tourRepo.getTourById(id)
    } catch (err: any) {
      console.error(err)
      return null
    } finally {
      loading.value = false
    }
  }

  return {
    tours,
    loading,
    error,
    fetchTours,
    getTourById
  }
})
