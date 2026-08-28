<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store/auth'
import LuxuryIcons from '@/shared/components/LuxuryIcons.vue'
import ProfileSettingsModal from './ProfileSettingsModal.vue'

const authStore = useAuthStore()
const router = useRouter()
const showDropdown = ref(false)
const isProfileModalOpen = ref(false)

const userInitials = computed(() => {
  if (!authStore.user || !authStore.user.name) return 'VIP'
  const names = authStore.user.name.split(' ')
  return names.map((n: string) => n[0]).join('').toUpperCase().slice(0, 2)
})

const handleLogout = () => {
  authStore.logout()
  showDropdown.value = false
  router.push('/')
}
</script>

<template>
  <div class="relative inline-block text-left" v-click-outside="() => showDropdown = false">
    <!-- Trigger Button with Tactile Active Press -->
    <button 
      @click="showDropdown = !showDropdown" 
      class="flex items-center gap-3 p-1.5 rounded-full hover:bg-slate-100/80 active:scale-[0.97] transition-[background-color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] focus:outline-none focus-visible:ring-2 focus-visible:ring-[#c9a84c]"
      aria-haspopup="menu"
      :aria-expanded="showDropdown"
      aria-label="User profile options"
    >
      <div v-if="authStore.user?.avatarUrl" class="w-9 h-9 rounded-full overflow-hidden border-2 border-white shadow-sm shrink-0">
        <img :src="authStore.user.avatarUrl" alt="Profile" class="w-full h-full object-cover" />
      </div>
      <div v-else class="flex items-center justify-center w-9 h-9 rounded-full bg-[#062d4d] text-white font-semibold text-xs shadow-sm ring-2 ring-white shrink-0">
        {{ userInitials }}
      </div>
      <div class="hidden md:flex flex-col items-start text-left">
        <span class="text-xs font-semibold text-slate-800 leading-tight">{{ authStore.user?.name || 'VIP Guest' }}</span>
        <span class="text-[9px] font-bold text-[#a38030] uppercase tracking-wider bg-[#c9a84c]/15 px-1.5 py-0.5 rounded border border-[#c9a84c]/25 mt-0.5">
          {{ authStore.user?.role === 'VIP' ? 'VIP Elite' : 'VIP Traveler' }}
        </span>
      </div>
      <LuxuryIcons name="chevron-down" size="12" class="text-slate-400 hidden md:block transition-transform duration-300 ease-[cubic-bezier(0.16,1,0.3,1)]" :class="showDropdown ? 'rotate-180 text-slate-600' : ''" />
    </button>

    <!-- Dropdown Menu with Spring Popover Physics -->
    <Transition
      enter-active-class="transition duration-250 ease-[cubic-bezier(0.16,1,0.3,1)]"
      enter-from-class="transform opacity-0 scale-[0.96] translate-y-1"
      enter-to-class="transform opacity-100 scale-100 translate-y-0"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="transform opacity-100 scale-100 translate-y-0"
      leave-to-class="transform opacity-0 scale-[0.96] translate-y-1"
    >
      <div 
        v-if="showDropdown" 
        class="absolute right-0 mt-2 w-64 origin-top-right bg-white/98 backdrop-blur-xl border border-slate-200/80 rounded-2xl shadow-[0_12px_40px_rgba(0,0,0,0.08)] ring-1 ring-black/5 focus:outline-none z-50 overflow-hidden"
      >
        <!-- Header -->
        <div class="px-4 py-3.5 border-b border-slate-100 bg-[#F8FAFC]/80">
          <p class="text-xs font-bold text-slate-900 truncate">{{ authStore.user?.name || 'VIP Guest' }}</p>
          <p class="text-[11px] text-slate-500 truncate mt-0.5">{{ authStore.user?.email || 'traveler@seadora.com' }}</p>
        </div>

        <!-- Navigation Links -->
        <div class="p-1.5 space-y-0.5">
          <button 
            @click="isProfileModalOpen = true; showDropdown = false" 
            class="w-full group flex items-center px-3 py-2 rounded-xl text-xs font-medium text-slate-700 hover:bg-slate-50 hover:text-[#062d4d] active:scale-[0.98] transition-[background-color,color,transform] duration-150 text-left"
          >
            <LuxuryIcons name="user" size="14" class="mr-2.5 text-slate-400 group-hover:text-[#062d4d] transition-colors" />
            My Profile Settings
          </button>
          
          <router-link 
            to="/portal/support" 
            @click="showDropdown = false" 
            class="group flex items-center px-3 py-2 rounded-xl text-xs font-medium text-slate-700 hover:bg-slate-50 hover:text-[#062d4d] active:scale-[0.98] transition-[background-color,color,transform] duration-150"
          >
            <LuxuryIcons name="help-circle" size="14" class="mr-2.5 text-slate-400 group-hover:text-[#062d4d] transition-colors" />
            System Documentation / Help
          </router-link>
        </div>

        <!-- Logout -->
        <div class="border-t border-slate-100 p-1.5">
          <button 
            @click="handleLogout" 
            class="group flex w-full items-center px-3 py-2 rounded-xl text-xs font-semibold text-red-600 hover:bg-red-50/80 active:scale-[0.98] transition-[background-color,transform] duration-150 text-left"
          >
            <LuxuryIcons name="log-out" size="14" class="mr-2.5 text-red-500 group-hover:text-red-600 transition-colors" />
            Logout
          </button>
        </div>
      </div>
    </Transition>
    
    <!-- Profile Settings Modal -->
    <ProfileSettingsModal v-model:isOpen="isProfileModalOpen" />
  </div>
</template>
