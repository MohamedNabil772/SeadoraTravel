import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import api from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<any>(null)
  const token = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)

  async function login(email: string, password: string) {
    const response = await api.post('/api/auth/api/auth/login', { email, password })
    const data = response.data
    
    const roles: string[] = data.roles || []
    const allowedAdminRoles = ['SuperAdmin', 'Admin', 'BookingManager', 'OperationsManager', 'ConciergeSpecialist', 'Accountant', 'BusinessOwner']
    if (roles.length > 0 && !roles.some(r => allowedAdminRoles.includes(r))) {
      throw new Error('Unauthorized: You do not have permissions to access the admin panel.')
    }

    token.value = data.token
    user.value = {
      id: data.id,
      email: data.email,
      fullName: data.fullName || (data.firstName ? `${data.firstName} ${data.lastName || ''}`.trim() : data.email.split('@')[0]),
      firstName: data.firstName || '',
      lastName: data.lastName || '',
      phoneNumber: data.phoneNumber || '',
      roles: roles.length > 0 ? roles : ['SuperAdmin'],
      permissions: data.permissions || ['*']
    }
    
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

  // Finance and other module nav/routes gate on fine-grained permissions; '*' is the super-admin wildcard.
  function hasPermission(permission: string): boolean {
    const perms: string[] = user.value?.permissions || []
    return perms.includes('*') || perms.includes(permission)
  }

  function hasAnyRole(roles: string[]): boolean {
    const userRoles: string[] = user.value?.roles || []
    return userRoles.some(r => roles.includes(r))
  }

  return { user, token, isAuthenticated, login, logout, initAuth, hasPermission, hasAnyRole }
})
