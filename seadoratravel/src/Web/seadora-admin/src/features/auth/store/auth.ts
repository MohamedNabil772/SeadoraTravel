import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import api from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<any>(null)
  const token = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)

  async function login(email: string, password: string) {
    const response = await api.post('/api/auth/api/Auth/login', { email, password })
    const data = response.data
    
    const roles: string[] = data.roles || []
    if (!roles.includes('Admin') && !roles.includes('BookingManager')) {
      throw new Error('Unauthorized: You do not have permissions to access the admin panel.')
    }

    token.value = data.token
    user.value = { email: data.email, roles }
    
    localStorage.setItem('token', token.value!)
    localStorage.setItem('user', JSON.stringify(user.value))
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  function initAuth() {
    const savedToken = localStorage.getItem('token')
    const savedUser = localStorage.getItem('user')
    if (savedToken && savedUser) {
      token.value = savedToken
      user.value = JSON.parse(savedUser)
    }
  }

  return { user, token, isAuthenticated, login, logout, initAuth }
})
