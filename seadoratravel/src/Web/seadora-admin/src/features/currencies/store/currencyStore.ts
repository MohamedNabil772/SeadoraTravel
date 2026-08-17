import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Currency {
  code: string
  name: string
  symbol: string
  exchangeRate: number
  isBase: boolean
  isActive: boolean
}

export const useCurrencyStore = defineStore('currency', () => {
  const currencies = ref<Currency[]>([
    { code: 'USD', name: 'US Dollar', symbol: '$', exchangeRate: 1, isBase: true, isActive: true },
    { code: 'EUR', name: 'Euro', symbol: '€', exchangeRate: 0.92, isBase: false, isActive: true },
    { code: 'GBP', name: 'British Pound', symbol: '£', exchangeRate: 0.79, isBase: false, isActive: true },
    { code: 'AED', name: 'Emirati Dirham', symbol: 'د.إ', exchangeRate: 3.67, isBase: false, isActive: true },
    { code: 'EGP', name: 'Egyptian Pound', symbol: '£', exchangeRate: 47.85, isBase: false, isActive: false },
  ])

  const baseCurrency = computed(() => currencies.value.find(c => c.isBase))

  const setBaseCurrency = (code: string) => {
    currencies.value.forEach(c => {
      c.isBase = c.code === code
      if (c.code === code) c.exchangeRate = 1
    })
  }

  const updateExchangeRate = (code: string, rate: number) => {
    const currency = currencies.value.find(c => c.code === code)
    if (currency && !currency.isBase) {
      currency.exchangeRate = rate
    }
  }

  const toggleStatus = (code: string) => {
    const currency = currencies.value.find(c => c.code === code)
    if (currency) {
      currency.isActive = !currency.isActive
    }
  }

  return { currencies, baseCurrency, setBaseCurrency, updateExchangeRate, toggleStatus }
})
