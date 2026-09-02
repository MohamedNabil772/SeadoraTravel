import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '@/services/api'
import { useAuthStore } from '@/features/auth/store/auth'

export interface NotificationItem {
  id: string
  type: 'booking' | 'inquiry' | 'system'
  title: string
  message: string
  referenceId?: string
  isRead: boolean
  createdAt: string
}

export const useNotificationStore = defineStore('notifications', () => {
  const notifications = ref<NotificationItem[]>([])
  const isLoading = ref(false)
  const pollingTimer = ref<number | null>(null)
  const lastUpdated = ref<number>(Date.now())

  const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)
  const unreadInquiriesCount = computed(() => notifications.value.filter(n => !n.isRead && ((n.type || '').toLowerCase().includes('inquiry') || (n.type || '').toLowerCase().includes('contact'))).length)
  const unreadBookingsCount = computed(() => notifications.value.filter(n => !n.isRead && (n.type || '').toLowerCase().includes('booking')).length)

  const fetchNotifications = async () => {
    try {
      isLoading.value = true
      const { data } = await api.get('/api/booking/api/notifications')
      notifications.value = Array.isArray(data) ? data : (data.notifications || [])
      lastUpdated.value = Date.now()
    } catch (error) {
      console.error('Failed to fetch notifications', error)
    } finally {
      isLoading.value = false
    }
  }

  const startPolling = (intervalMs = 10000) => {
    const authStore = useAuthStore()
    authStore.initAuth()
    
    // Stop any existing timer first
    stopPolling()

    if (authStore.isAuthenticated) {
      fetchNotifications()
      pollingTimer.value = window.setInterval(() => {
        authStore.initAuth()
        if (authStore.isAuthenticated) {
          fetchNotifications()
        } else {
          stopPolling()
        }
      }, intervalMs)
    }
  }

  const stopPolling = () => {
    if (pollingTimer.value !== null) {
      window.clearInterval(pollingTimer.value)
      pollingTimer.value = null
    }
  }

  const markAsRead = async (id: string) => {
    // Optimistic update
    const item = notifications.value.find(n => n.id === id)
    if (item && !item.isRead) {
      item.isRead = true
      try {
        await api.put(`/api/booking/api/notifications/${id}/read`)
      } catch (error) {
        console.error('Failed to mark notification as read', error)
        // Revert on failure
        item.isRead = false
      }
    }
  }

  const markAllAsRead = async () => {
    // Optimistic update
    const unreadItems = notifications.value.filter(n => !n.isRead)
    unreadItems.forEach(n => n.isRead = true)
    
    try {
      await api.put('/api/booking/api/notifications/read-all')
    } catch (error) {
      console.error('Failed to mark all as read', error)
      // Revert on failure
      unreadItems.forEach(n => n.isRead = false)
    }
  }

  const deleteNotification = async (id: string) => {
    const index = notifications.value.findIndex(n => n.id === id)
    if (index !== -1) {
      const deletedItem = notifications.value[index]
      notifications.value.splice(index, 1)
      try {
        await api.delete(`/api/booking/api/notifications/${id}`)
      } catch (error) {
        console.error('Failed to delete notification', error)
        // Revert on failure
        notifications.value.splice(index, 0, deletedItem)
      }
    }
  }

  return {
    notifications,
    isLoading,
    lastUpdated,
    unreadCount,
    unreadInquiriesCount,
    unreadBookingsCount,
    fetchNotifications,
    startPolling,
    stopPolling,
    markAsRead,
    markAllAsRead,
    deleteNotification
  }
})
