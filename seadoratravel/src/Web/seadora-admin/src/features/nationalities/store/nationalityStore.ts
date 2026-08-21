import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '@/services/api'

export interface Nationality {
  id: string
  code: string
  countryName: string
  nationalityName: string
  flagEmoji?: string
  flagCode?: string
  isActive: boolean
}

export const useNationalityStore = defineStore('nationality', () => {
  const nationalities = ref<Nationality[]>([])
  const isLoading = ref(false)

  const activeNationalities = computed(() => nationalities.value.filter(n => n.isActive))

  const fetchNationalities = async (includeInactive = true) => {
    isLoading.value = true
    try {
      const response = await api.get(`/api/content/api/v1/nationalities?includeInactive=${includeInactive}`)
      if (Array.isArray(response.data) && response.data.length > 0) {
        nationalities.value = response.data.map(n => ({
          ...n,
          flagCode: n.code || n.flagCode
        }))
      }
    } catch (e) {
      console.warn('Failed to fetch nationalities from backend', e)
    } finally {
      isLoading.value = false
    }
  }

  const toggleStatus = async (id: string) => {
    const nat = nationalities.value.find(n => n.id === id)
    if (nat) {
      nat.isActive = !nat.isActive
      try {
        await api.patch(`/api/content/api/v1/nationalities/${id}/toggle-active`, { id, isActive: nat.isActive })
      } catch (e) {
        console.warn('Failed to toggle nationality status on backend', e)
      }
    }
  }

  const addNationality = async (nat: Omit<Nationality, 'id'>) => {
    try {
      const response = await api.post('/api/content/api/v1/nationalities', {
        code: (nat.code || nat.flagCode || '').toUpperCase().trim(),
        countryName: nat.countryName,
        nationalityName: nat.nationalityName,
        flagEmoji: nat.flagEmoji || '',
        isActive: nat.isActive ?? true
      })
      if (response.data) {
        await fetchNationalities()
        return
      }
    } catch (e) {
      console.warn('Failed to add nationality on backend', e)
    }
    nationalities.value.push({ ...nat, id: Math.random().toString(36).substr(2, 9), flagCode: nat.code || nat.flagCode })
  }

  const updateNationality = async (id: string, updates: Partial<Nationality>) => {
    const idx = nationalities.value.findIndex(n => n.id === id)
    if (idx !== -1) {
      nationalities.value[idx] = { ...nationalities.value[idx], ...updates }
      try {
        await api.put(`/api/content/api/v1/nationalities/${id}`, {
          id,
          code: (updates.code || updates.flagCode || nationalities.value[idx].code || '').toUpperCase().trim(),
          countryName: updates.countryName || nationalities.value[idx].countryName,
          nationalityName: updates.nationalityName || nationalities.value[idx].nationalityName,
          flagEmoji: updates.flagEmoji || nationalities.value[idx].flagEmoji || '',
          isActive: updates.isActive ?? nationalities.value[idx].isActive
        })
      } catch (e) {
        console.warn('Failed to update nationality on backend', e)
      }
    }
  }

  const deleteNationality = async (id: string) => {
    try {
      await api.delete(`/api/content/api/v1/nationalities/${id}`)
      nationalities.value = nationalities.value.filter(n => n.id !== id)
    } catch (e) {
      console.warn('Failed to delete nationality on backend', e)
    }
  }

  return { 
    nationalities, 
    activeNationalities, 
    isLoading, 
    fetchNationalities, 
    toggleStatus, 
    addNationality, 
    updateNationality, 
    deleteNationality 
  }
})
