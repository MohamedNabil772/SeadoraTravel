<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { Settings, HelpCircle, LogOut } from 'lucide-vue-next'
import { useAuthStore } from '@/features/auth/store/auth'
import ProfileSettingsModal from './ProfileSettingsModal.vue'

const auth = useAuthStore()
const emit = defineEmits(['logout'])

const isOpen = ref(false)
const dropdownRef = ref<HTMLElement | null>(null)
const showSettings = ref(false)

const toggle = () => {
  isOpen.value = !isOpen.value
}

const handleClickOutside = (e: MouseEvent) => {
  if (dropdownRef.value && !dropdownRef.value.contains(e.target as Node)) {
    isOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

const userInitials = computed(() => {
  const name = auth.user?.fullName || 'Administrator'
  return name.substring(0, 2).toUpperCase()
})

const handleLogout = () => {
  isOpen.value = false
  emit('logout')
}

const openSettings = () => {
  isOpen.value = false
  showSettings.value = true
}

const roleBadge = computed(() => {
  if (auth.user?.roles?.includes('Admin')) return 'Super Admin'
  if (auth.user?.roles?.includes('BookingManager')) return 'Booking Mgr'
  return 'Administrator'
})
</script>

<template>
  <div class="relative" ref="dropdownRef">
    <button 
      type="button"
      @click="toggle"
      :aria-expanded="isOpen"
      aria-haspopup="menu"
      aria-label="Open account menu"
      class="flex items-center gap-3 pl-2 md:pl-4 border-l border-border/60 cursor-pointer group"
    >
      <div class="text-right hidden sm:block group-hover:opacity-80 transition-opacity">
        <span class="block text-sm font-semibold text-text-main leading-tight">{{ auth.user?.fullName || 'Administrator' }}</span>
        <div class="flex items-center justify-end gap-1.5 mt-0.5">
          <span class="inline-flex items-center rounded-full bg-secondary/10 px-1.5 py-0.5 text-[9px] font-bold text-secondary-text ring-1 ring-inset ring-secondary/20">
            {{ roleBadge }}
          </span>
        </div>
      </div>
      <div 
        class="w-9 h-9 rounded-full bg-gradient-to-tr from-secondary/20 to-secondary/5 border border-secondary/20 flex items-center justify-center text-secondary-text font-bold text-sm ring-2 ring-white shadow-sm transition-transform duration-300 group-hover:scale-105"
      >
        {{ userInitials }}
      </div>
    </button>

    <!-- Dropdown Menu -->
    <Transition name="staggered-dropdown">
      <div 
        v-if="isOpen" 
        class="absolute right-0 mt-3 w-64 bg-white/95 backdrop-blur-md rounded-xl shadow-2xl border border-gray-100 overflow-hidden z-50 origin-top-right"
      >
        <div class="px-4 py-3 border-b border-gray-100 bg-gray-50/50">
          <p class="text-sm font-medium text-gray-900 truncate">{{ auth.user?.fullName || 'Administrator' }}</p>
          <p class="text-xs text-gray-500 truncate mt-0.5">{{ auth.user?.email || 'admin@seadora.com' }}</p>
        </div>

        <div class="p-1.5">
          <button 
            @click="openSettings"
            class="w-full flex items-center gap-3 px-3 py-2.5 text-sm text-gray-700 rounded-lg hover:bg-gray-50 hover:text-primary transition-colors text-left"
          >
            <Settings class="w-4 h-4" />
            <span>My Profile Settings</span>
          </button>
          
          <button 
            class="w-full flex items-center gap-3 px-3 py-2.5 text-sm text-gray-700 rounded-lg hover:bg-gray-50 hover:text-primary transition-colors text-left"
          >
            <HelpCircle class="w-4 h-4" />
            <span>System Documentation / Help</span>
          </button>
        </div>

        <div class="p-1.5 border-t border-gray-100">
          <button 
            @click="handleLogout"
            class="w-full flex items-center gap-3 px-3 py-2.5 text-sm text-red-600 rounded-lg hover:bg-red-50 transition-colors text-left font-medium"
          >
            <LogOut class="w-4 h-4" />
            <span>Logout</span>
          </button>
        </div>
      </div>
    </Transition>

    <ProfileSettingsModal :is-open="showSettings" @close="showSettings = false" />
  </div>
</template>

<style scoped>
.staggered-dropdown-enter-active {
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.staggered-dropdown-leave-active {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}
.staggered-dropdown-enter-from {
  opacity: 0;
  transform: scale(0.95) translateY(-10px);
}
.staggered-dropdown-leave-to {
  opacity: 0;
  transform: scale(0.95) translateY(-10px);
}
</style>
