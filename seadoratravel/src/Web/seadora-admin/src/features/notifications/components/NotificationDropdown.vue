<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useNotificationStore } from '../store/notificationStore'
import { 
  Bell, 
  Calendar, 
  Mail, 
  AlertCircle, 
  Check, 
  Trash2, 
  SearchX 
} from 'lucide-vue-next'

const router = useRouter()
const store = useNotificationStore()

const isOpen = ref(false)
const dropdownRef = ref<HTMLElement | null>(null)
const activeTab = ref<'all' | 'unread' | 'bookings' | 'inquiries'>('all')

const handleClickOutside = (event: MouseEvent) => {
  if (isOpen.value && dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
    isOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('mousedown', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('mousedown', handleClickOutside)
})

const toggleDropdown = () => {
  isOpen.value = !isOpen.value
}

const isBooking = (type: string) => type.toLowerCase().includes('booking')
const isInquiry = (type: string) => type.toLowerCase().includes('inquiry') || type.toLowerCase().includes('contact')

const filteredNotifications = computed(() => {
  let list = store.notifications
  
  if (activeTab.value === 'unread') {
    list = list.filter(n => !n.isRead)
  } else if (activeTab.value === 'bookings') {
    list = list.filter(n => isBooking(n.type))
  } else if (activeTab.value === 'inquiries') {
    list = list.filter(n => isInquiry(n.type))
  }
  
  return list.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
})

const handleNotificationClick = async (item: any) => {
  if (!item.isRead) {
    await store.markAsRead(item.id)
  }
  
  isOpen.value = false
  
  if (isBooking(item.type)) {
    if (item.referenceId) router.push(`/bookings/${item.referenceId}/details`)
    else router.push(`/bookings`)
  } else if (isInquiry(item.type)) {
    if (item.referenceId) router.push(`/inquiries?id=${item.referenceId}`)
    else router.push('/inquiries')
  }
}

const formatTimeAgo = (dateString: string) => {
  const date = new Date(dateString)
  const now = new Date()
  const seconds = Math.floor((now.getTime() - date.getTime()) / 1000)
  
  if (seconds < 60) return 'Just now'
  
  const minutes = Math.floor(seconds / 60)
  if (minutes < 60) return `${minutes}m ago`
  
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return `${hours}h ago`
  
  const days = Math.floor(hours / 24)
  if (days < 7) return `${days}d ago`
  
  return date.toLocaleDateString()
}

const getIconForType = (type: string) => {
  if (isBooking(type)) return Calendar
  if (isInquiry(type)) return Mail
  return AlertCircle
}

const getIconColorForType = (type: string) => {
  if (isBooking(type)) return 'text-amber-500 bg-amber-500/10'
  if (isInquiry(type)) return 'text-emerald-500 bg-emerald-500/10'
  return 'text-blue-500 bg-blue-500/10'
}

const getTagForType = (type: string) => {
  if (isBooking(type)) return { label: 'VIP Booking', class: 'bg-amber-100 text-amber-700 border-amber-200' }
  if (isInquiry(type)) return { label: 'VIP Inquiry', class: 'bg-emerald-100 text-emerald-700 border-emerald-200' }
  return { label: 'System', class: 'bg-blue-100 text-blue-700 border-blue-200' }
}
</script>

<template>
  <div class="relative" ref="dropdownRef">
    <!-- Bell Trigger -->
    <button 
      @click="toggleDropdown"
      class="relative p-2 text-text-muted hover:text-text-main hover:bg-black/5 rounded-full transition-all duration-300 ease-out focus:outline-none"
      :class="{ 'bg-black/5 text-text-main scale-105': isOpen }"
    >
      <Bell class="w-5 h-5 transition-transform duration-300" :class="{ 'scale-110': isOpen }" />
      
      <!-- Unread Badge Ping (Luxury Gold/Amber) -->
      <div v-if="store.unreadCount > 0" class="absolute top-1.5 right-1.5">
        <span class="absolute inline-flex h-full w-full rounded-full bg-amber-400 opacity-75 animate-ping"></span>
        <span class="relative inline-flex rounded-full h-2.5 w-2.5 bg-gradient-to-tr from-amber-500 to-yellow-400 ring-2 ring-white"></span>
      </div>
    </button>

    <!-- Dropdown Panel (Glassmorphism + Spring Transition) -->
    <transition
      enter-active-class="transition-[opacity,transform] duration-300 ease-[cubic-bezier(0.23,1,0.32,1)]"
      enter-from-class="opacity-0 translate-y-2 scale-95"
      enter-to-class="opacity-100 translate-y-0 scale-100"
      leave-active-class="transition-[opacity,transform] duration-200 ease-in"
      leave-from-class="opacity-100 translate-y-0 scale-100"
      leave-to-class="opacity-0 translate-y-1 scale-95"
    >
      <div 
        v-if="isOpen"
        class="absolute right-0 mt-2 w-80 sm:w-96 bg-white/95 backdrop-blur-xl border border-border/60 rounded-2xl shadow-[0_8px_40px_-12px_rgba(0,0,0,0.1)] overflow-hidden z-50 origin-top-right flex flex-col max-h-[85vh]"
      >
        <!-- Header -->
        <div class="p-4 border-b border-border/40 flex items-center justify-between bg-surface-sunken/50">
          <div class="flex items-center gap-2">
            <h3 class="font-semibold text-text-main tracking-tight text-lg">Notifications</h3>
            <span v-if="store.unreadCount > 0" class="px-2 py-0.5 rounded-full bg-amber-100 text-amber-700 text-xs font-bold">
              {{ store.unreadCount }} new
            </span>
          </div>
          <button 
            v-if="store.unreadCount > 0"
            @click="store.markAllAsRead"
            class="text-xs font-medium text-primary hover:text-primary-dark transition-colors flex items-center gap-1"
          >
            <Check class="w-3.5 h-3.5" />
            Mark all read
          </button>
        </div>

        <!-- Filters -->
        <div class="flex p-2 gap-1 border-b border-border/30 bg-surface-sunken/30">
          <button 
            v-for="tab in ['all', 'unread', 'bookings', 'inquiries'] as const" 
            :key="tab"
            @click="activeTab = tab"
            class="flex-1 py-1.5 px-2 text-xs font-medium rounded-lg transition-all capitalize"
            :class="activeTab === tab ? 'bg-white shadow-sm text-text-main' : 'text-text-muted hover:text-text-main hover:bg-black/5'"
          >
            {{ tab }}
          </button>
        </div>

        <!-- List -->
        <div class="overflow-y-auto flex-1 p-2 space-y-1 no-scrollbar min-h-[300px]">
          <template v-if="filteredNotifications.length > 0">
            <div 
              v-for="item in filteredNotifications" 
              :key="item.id"
              role="button"
              tabindex="0"
              :aria-label="`${item.title}. ${item.message}`"
              @click="handleNotificationClick(item)"
              @keydown.enter.prevent="handleNotificationClick(item)"
              @keydown.space.prevent="handleNotificationClick(item)"
              class="group relative flex gap-3 p-3 rounded-xl hover:bg-black/[0.03] transition-colors cursor-pointer select-none focus:outline-none focus-visible:ring-2 focus-visible:ring-primary/30"
              :class="{ 'bg-blue-50/30': !item.isRead }"
            >
              <!-- Unread Indicator -->
              <div v-if="!item.isRead" class="absolute left-1.5 top-1/2 -translate-y-1/2 w-1.5 h-1.5 rounded-full bg-amber-500"></div>
              
              <!-- Icon -->
              <div 
                class="flex-shrink-0 w-10 h-10 rounded-full flex items-center justify-center transition-transform group-hover:scale-105"
                :class="getIconColorForType(item.type)"
              >
                <component :is="getIconForType(item.type)" class="w-5 h-5" />
              </div>
              
              <!-- Content -->
              <div class="flex-1 min-w-0 pr-6">
                <div class="flex items-baseline justify-between gap-2 mb-0.5">
                  <h4 class="text-sm font-medium text-text-main truncate" :class="{ 'font-semibold': !item.isRead }">
                    {{ item.title }}
                  </h4>
                  <span class="text-[10px] text-text-muted whitespace-nowrap flex-shrink-0">
                    {{ formatTimeAgo(item.createdAt) }}
                  </span>
                </div>
                <div class="mb-1">
                  <span 
                    class="text-[9px] font-bold px-1.5 py-0.5 rounded-full border"
                    :class="getTagForType(item.type).class"
                  >
                    {{ getTagForType(item.type).label }}
                  </span>
                </div>
                <p class="text-xs text-text-muted line-clamp-2 leading-relaxed mt-1">
                  {{ item.message }}
                </p>
              </div>

              <!-- Delete Action -->
              <button 
                type="button"
                @click.stop="store.deleteNotification(item.id)"
                :aria-label="`Delete notification: ${item.title}`"
                class="absolute right-3 top-1/2 -translate-y-1/2 p-1.5 text-text-muted/0 group-hover:text-red-400 focus-visible:text-red-400 hover:bg-red-50 rounded-md transition-all duration-200"
              >
                <Trash2 class="w-4 h-4" />
              </button>
            </div>
          </template>

          <!-- Empty State -->
          <div v-else class="h-full flex flex-col items-center justify-center text-center p-6 mt-8">
            <div class="w-16 h-16 rounded-full bg-surface-sunken flex items-center justify-center mb-4 shadow-inner">
              <SearchX class="w-8 h-8 text-text-muted/50" />
            </div>
            <h4 class="text-sm font-medium text-text-main mb-1">All caught up</h4>
            <p class="text-xs text-text-muted max-w-[200px]">
              You don't have any {{ activeTab !== 'all' ? activeTab : '' }} notifications at the moment.
            </p>
          </div>
        </div>

        <!-- Footer -->
        <div class="p-3 text-center border-t border-border/40 bg-surface-sunken/50">
          <button class="text-xs font-medium text-text-muted hover:text-primary transition-colors">
            View Notification Settings
          </button>
        </div>
      </div>
    </transition>
  </div>
</template>

<style scoped>
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
