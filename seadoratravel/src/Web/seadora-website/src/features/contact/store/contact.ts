import { defineStore } from 'pinia'

export const useContactStore = defineStore('contact', {
  state: () => ({
    loading: false,
    success: false,
    error: null as string | null
  }),
  actions: {
    async submitForm(formData: any) {
      this.loading = true
      this.error = null
      this.success = false
      
      console.log('Submitting contact form:', formData)
      
      // Simulate API call
      try {
        await new Promise(resolve => setTimeout(resolve, 1500))
        this.success = true
        console.log('Form submitted successfully')
      } catch (err) {
        this.error = 'Failed to submit form'
        console.error(err)
      } finally {
        this.loading = false
      }
    }
  }
})
