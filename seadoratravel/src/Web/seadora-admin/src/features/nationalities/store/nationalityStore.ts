import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface Nationality {
  id: string
  countryName: string
  nationalityName: string
  flagCode: string
  isActive: boolean
}

export const useNationalityStore = defineStore('nationality', () => {
  const nationalities = ref<Nationality[]>([
    { id: '1', countryName: 'United States', nationalityName: 'American', flagCode: 'US', isActive: true },
    { id: '2', countryName: 'United Kingdom', nationalityName: 'British', flagCode: 'GB', isActive: true },
    { id: '3', countryName: 'United Arab Emirates', nationalityName: 'Emirati', flagCode: 'AE', isActive: true },
    { id: '4', countryName: 'Egypt', nationalityName: 'Egyptian', flagCode: 'EG', isActive: false },
    { id: '5', countryName: 'Germany', nationalityName: 'German', flagCode: 'DE', isActive: true },
  ])

  const toggleStatus = (id: string) => {
    const nat = nationalities.value.find(n => n.id === id)
    if (nat) {
      nat.isActive = !nat.isActive
    }
  }
  
  const addNationality = (nat: Omit<Nationality, 'id'>) => {
    nationalities.value.push({ ...nat, id: Math.random().toString(36).substr(2, 9) })
  }

  const updateNationality = (id: string, updates: Partial<Nationality>) => {
    const idx = nationalities.value.findIndex(n => n.id === id)
    if (idx !== -1) {
      nationalities.value[idx] = { ...nationalities.value[idx], ...updates }
    }
  }

  return { nationalities, toggleStatus, addNationality, updateNationality }
})
