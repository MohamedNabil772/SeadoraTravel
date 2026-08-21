import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '@/services/api'

export interface Currency {
  id?: string
  code: string
  name: string
  symbol: string
  exchangeRate: number
  liveExchangeRate?: number
  isBase: boolean
  isManualRate: boolean
  lastRateSyncAt?: string
  isActive: boolean
}

export const useCurrencyStore = defineStore('currency', () => {
  const currencies = ref<Currency[]>([
    { code: 'EUR', name: 'Euro', symbol: '€', exchangeRate: 1.0, liveExchangeRate: 1.0, isBase: true, isManualRate: false, isActive: true },
    { code: 'USD', name: 'US Dollar', symbol: '$', exchangeRate: 1.085, liveExchangeRate: 1.085, isBase: false, isManualRate: false, isActive: true },
    { code: 'EGP', name: 'Egyptian Pound', symbol: 'E£', exchangeRate: 52.5, liveExchangeRate: 52.5, isBase: false, isManualRate: false, isActive: true },
  ])
  const isLoading = ref(false)
  const isSyncing = ref(false)

  const baseCurrency = computed(() => currencies.value.find(c => c.isBase) || currencies.value[0])
  const activeCurrencies = computed(() => currencies.value.filter(c => c.isActive))

  const fetchCurrencies = async (includeInactive = true) => {
    isLoading.value = true
    try {
      const response = await api.get(`/api/content/api/v1/currencies?includeInactive=${includeInactive}`)
      if (Array.isArray(response.data) && response.data.length > 0) {
        currencies.value = response.data
      }
    } catch (e) {
      console.warn('Failed to fetch currencies from backend', e)
    } finally {
      isLoading.value = false
    }
  }

  const syncLiveRates = async () => {
    isSyncing.value = true
    try {
      const response = await api.post('/api/content/api/v1/currencies/sync-rates')
      if (Array.isArray(response.data) && response.data.length > 0) {
        currencies.value = response.data
      }
    } catch (e) {
      console.warn('Failed to sync live exchange rates', e)
    } finally {
      isSyncing.value = false
    }
  }

  const updateExchangeRate = async (code: string, rate: number) => {
    const currency = currencies.value.find(c => c.code === code)
    if (currency && !currency.isBase) {
      currency.exchangeRate = rate
      currency.isManualRate = true
      if (currency.id) {
        try {
          await api.patch(`/api/content/api/v1/currencies/${currency.id}/rate`, { id: currency.id, exchangeRate: rate })
        } catch (e) {
          console.warn('Failed to persist exchange rate to backend', e)
        }
      }
    }
  }

  const resetToLiveRate = async (code: string) => {
    const currency = currencies.value.find(c => c.code === code)
    if (currency && currency.id) {
      try {
        await api.post(`/api/content/api/v1/currencies/${currency.id}/reset-live-rate`)
        if (currency.liveExchangeRate) {
          currency.exchangeRate = currency.liveExchangeRate
          currency.isManualRate = false
        }
      } catch (e) {
        console.warn('Failed to reset to live rate', e)
      }
    }
  }

  const setBaseCurrency = async (code: string) => {
    const target = currencies.value.find(c => c.code === code)
    if (target) {
      currencies.value.forEach(c => {
        c.isBase = c.code === code
        if (c.code === code) {
          c.exchangeRate = 1
          c.isManualRate = false
          c.isActive = true
        }
      })
      if (target.id) {
        try {
          await api.post(`/api/content/api/v1/currencies/${target.id}/set-base`)
        } catch (e) {
          console.warn('Failed to set base currency', e)
        }
      }
    }
  }

  const toggleStatus = async (code: string) => {
    const currency = currencies.value.find(c => c.code === code)
    if (currency && !currency.isBase) {
      currency.isActive = !currency.isActive
      if (currency.id) {
        try {
          await api.patch(`/api/content/api/v1/currencies/${currency.id}/toggle-active`, { id: currency.id, isActive: currency.isActive })
        } catch (e) {
          console.warn('Failed to toggle currency active status', e)
        }
      }
    }
  }

  const addCurrency = async (currency: Omit<Currency, 'isBase' | 'isActive' | 'isManualRate'>) => {
    const newCurr: Currency = {
      ...currency,
      isBase: false,
      isManualRate: false,
      isActive: true
    }
    try {
      const response = await api.post('/api/content/api/v1/currencies', newCurr)
      if (response.data) {
        currencies.value.push(response.data)
        return
      }
    } catch (e) {
      console.warn('Failed to add currency to backend', e)
    }
    currencies.value.push(newCurr)
  }

  return { 
    currencies, 
    isLoading,
    isSyncing,
    baseCurrency, 
    activeCurrencies,
    fetchCurrencies,
    syncLiveRates,
    setBaseCurrency, 
    updateExchangeRate, 
    resetToLiveRate,
    toggleStatus, 
    addCurrency 
  }
})
