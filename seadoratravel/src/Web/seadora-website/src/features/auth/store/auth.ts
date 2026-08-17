import { defineStore } from 'pinia'
import { authApi } from '../api/authApi'

export interface User {
  id?: string;
  name: string;
  email: string;
  phone?: string;
  avatar?: string;
  roles?: string[];
}

export type AuthModalMode = 'login' | 'register' | 'otp';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('seadora_token') || null as string | null,
    user: localStorage.getItem('seadora_user') ? JSON.parse(localStorage.getItem('seadora_user')!) as User : null as User | null,
    isAuthenticated: !!localStorage.getItem('seadora_token'),
    isAuthModalOpen: false,
    authModalMode: 'login' as AuthModalMode,
    favorites: localStorage.getItem('seadora_favorites') ? JSON.parse(localStorage.getItem('seadora_favorites')!) as string[] : [] as string[]
  }),
  getters: {
    isLoggedIn: (state) => state.isAuthenticated && state.user !== null
  },
  actions: {
    async sendWhatsAppOtp(phone: string) {
      return await authApi.sendWhatsAppOtp(phone)
    },
    async verifyWhatsAppOtp(phone: string, code: string) {
      const response = await authApi.verifyWhatsAppOtp(phone, code)
      this.setAuthData(response.data.token, response.data.user)
      return response
    },
    async socialLogin(provider: string, token: string, profile: any) {
      const response = await authApi.socialLogin(provider, token, profile)
      this.setAuthData(response.data.token, response.data.user)
      return response
    },
    async login(credentials: any) {
      const response = await authApi.login(credentials)
      this.setAuthData(response.data.token, response.data.user)
      return response
    },
    async register(userData: any) {
      const response = await authApi.register(userData)
      this.setAuthData(response.data.token, response.data.user)
      return response
    },
    setAuthData(token: string, user: User) {
      this.token = token
      this.user = user
      this.isAuthenticated = true
      localStorage.setItem('seadora_token', token)
      localStorage.setItem('seadora_user', JSON.stringify(user))
    },
    logout() {
      this.token = null
      this.user = null
      this.isAuthenticated = false
      this.favorites = []
      localStorage.removeItem('seadora_token')
      localStorage.removeItem('seadora_user')
      localStorage.removeItem('seadora_favorites')
    },
    openAuthModal(mode: AuthModalMode = 'login') {
      this.authModalMode = mode
      this.isAuthModalOpen = true
    },
    closeAuthModal() {
      this.isAuthModalOpen = false
    },
    toggleFavorite(tourId: string): boolean {
      if (!this.isLoggedIn) {
        this.openAuthModal('login')
        return false
      }
      const index = this.favorites.indexOf(tourId)
      if (index > -1) {
        this.favorites.splice(index, 1)
      } else {
        this.favorites.push(tourId)
      }
      localStorage.setItem('seadora_favorites', JSON.stringify(this.favorites))
      return true
    },
    isFavorite(tourId: string): boolean {
      return this.favorites.includes(tourId)
    }
  }
})
