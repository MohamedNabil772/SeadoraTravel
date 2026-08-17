import { defineStore } from 'pinia'

export const useCurrencyStore = defineStore('currency', {
  state: () => ({
    selectedCurrency: localStorage.getItem('currency') || 'EUR'
  }),
  getters: {
    exchangeRate: (state) => {
      switch (state.selectedCurrency) {
        case 'USD': return 1.08
        case 'EGP': return 52.0
        default: return 1.0
      }
    },
    currencySymbol: (state) => {
      switch (state.selectedCurrency) {
        case 'USD': return '$'
        case 'EGP': return ' EGP'
        default: return '€'
      }
    }
  },
  actions: {
    setCurrency(currency: string) {
      this.selectedCurrency = currency
      localStorage.setItem('currency', currency)
    },
    convertPrice(priceInEur: number): number {
      return Math.round(priceInEur * this.exchangeRate)
    },
    formatPrice(priceInEur: number): string {
      const converted = this.convertPrice(priceInEur)
      if (this.selectedCurrency === 'EGP') {
        return `${converted}${this.currencySymbol}`
      }
      return `${this.currencySymbol}${converted}`
    }
  }
})
