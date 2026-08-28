<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/features/auth/store/auth'
import LuxuryIcons from '@/shared/components/LuxuryIcons.vue'
import ProfileSettingsModal from './ProfileSettingsModal.vue'

const { t } = useI18n()
const authStore = useAuthStore()
const router = useRouter()
const showDropdown = ref(false)
const isProfileModalOpen = ref(false)

const userInitials = computed(() => {
  if (!authStore.user || !authStore.user.name) return 'VIP'
  const names = authStore.user.name.split(' ')
  return names.map((n: string) => n[0]).join('').toUpperCase().slice(0, 2)
})

const roleBadge = computed(() => {
  if (authStore.user?.role === 'VIP' || authStore.user?.isVip) return t('portal.dropdown.vipElite')
  return t('portal.dropdown.vipGuest')
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
      type="button"
      @click="showDropdown = !showDropdown" 
      class="flex items-center gap-3 pl-2 md:pl-3 border-l border-slate-200/80 cursor-pointer group focus:outline-none"
      aria-haspopup="menu"
      :aria-expanded="showDropdown"
      aria-label="Open account menu"
    >
      <div class="text-right hidden sm:block group-hover:opacity-80 transition-opacity">
        <span class="block text-xs font-bold text-slate-800 leading-tight">{{ authStore.user?.name || t('portal.dropdown.vipGuest') }}</span>
        <div class="flex items-center justify-end gap-1 mt-0.5">
          <span class="inline-flex items-center rounded-full bg-[#c9a84c]/15 px-2 py-0.5 text-[9px] font-extrabold text-[#a38030] ring-1 ring-inset ring-[#c9a84c]/30 uppercase tracking-wider">
            {{ roleBadge }}
          </span>
        </div>
      </div>

      <!-- Avatar with Fallback Initials -->
      <div 
        class="w-9 h-9 rounded-full overflow-hidden border border-[#c9a84c]/30 flex items-center justify-center bg-gradient-to-tr from-[#062d4d] to-[#0a4575] text-white font-bold text-xs ring-2 ring-white shadow-sm transition-transform duration-300 group-hover:scale-105"
      >
        <img v-if="authStore.user?.avatarUrl" :src="authStore.user.avatarUrl" alt="Profile" class="w-full h-full object-cover" />
        <span v-else>{{ userInitials }}</span>
      </div>
      <LuxuryIcons name="chevron-down" size="12" class="text-slate-400 hidden md:block transition-transform duration-300 ease-[cubic-bezier(0.16,1,0.3,1)]" :class="showDropdown ? 'rotate-180 text-slate-600' : ''" />
    </button>

    <!-- Dropdown Menu with Spring Popover Physics -->
    <Transition
      enter-active-class="transition duration-250 ease-[cubic-bezier(0.34,1.56,0.64,1)]"
      enter-from-class="transform opacity-0 scale-95 -translate-y-2"
      enter-to-class="transform opacity-100 scale-100 translate-y-0"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="transform opacity-100 scale-100 translate-y-0"
      leave-to-class="transform opacity-0 scale-95 -translate-y-2"
    >
      <div 
        v-if="showDropdown" 
        class="absolute right-0 mt-3 w-64 origin-top-right bg-white/98 backdrop-blur-xl border border-slate-100 rounded-2xl shadow-2xl overflow-hidden z-50 ring-1 ring-black/5 focus:outline-none"
      >
        <!-- User Info Header -->
        <div class="px-4 py-3 border-b border-slate-100 bg-[#F8FAFC]">
          <p class="text-xs font-bold text-slate-900 truncate">{{ authStore.user?.name || t('portal.dropdown.vipGuest') }}</p>
          <p class="text-[11px] text-slate-500 truncate mt-0.5">{{ authStore.user?.email || 'traveler@seadora.com' }}</p>
        </div>

        <!-- Menu Items -->
        <div class="p-1.5 space-y-0.5 font-sans">
          <button 
            @click="isProfileModalOpen = true; showDropdown = false" 
            class="w-full group flex items-center gap-3 px-3 py-2.5 rounded-xl text-xs font-medium text-slate-700 hover:bg-slate-50 hover:text-[#062d4d] active:scale-[0.98] transition-[background-color,color,transform] duration-150 text-left cursor-pointer"
          >
            <LuxuryIcons name="settings" size="15" class="text-slate-400 group-hover:text-[#062d4d] transition-colors" />
            <span>{{ $t('portal.dropdown.profileSettings') }}</span>
          </button>
          
          <router-link 
            to="/portal/documents" 
            @click="showDropdown = false" 
            class="group flex items-center gap-3 px-3 py-2.5 rounded-xl text-xs font-medium text-slate-700 hover:bg-slate-50 hover:text-[#062d4d] active:scale-[0.98] transition-[background-color,color,transform] duration-150 cursor-pointer"
          >
            <LuxuryIcons name="file-text" size="15" class="text-slate-400 group-hover:text-[#062d4d] transition-colors" />
            <span>{{ $t('portal.dropdown.documentVault') }}</span>
          </router-link>

          <router-link 
            to="/portal/support" 
            @click="showDropdown = false" 
            class="group flex items-center gap-3 px-3 py-2.5 rounded-xl text-xs font-medium text-slate-700 hover:bg-slate-50 hover:text-[#062d4d] active:scale-[0.98] transition-[background-color,color,transform] duration-150 cursor-pointer"
          >
            <LuxuryIcons name="help-circle" size="15" class="text-slate-400 group-hover:text-[#062d4d] transition-colors" />
            <span>{{ $t('portal.dropdown.help') }}</span>
          </router-link>
        </div>

        <!-- Logout Section -->
        <div class="p-1.5 border-t border-slate-100">
          <button 
            @click="handleLogout" 
            class="w-full group flex items-center gap-3 px-3 py-2.5 rounded-xl text-xs font-bold text-red-600 hover:bg-red-50 active:scale-[0.98] transition-[background-color,transform] duration-150 text-left cursor-pointer"
          >
            <LuxuryIcons name="log-out" size="15" class="text-red-500 group-hover:text-red-600 transition-colors" />
            <span>{{ $t('portal.dropdown.logout') }}</span>
          </button>
        </div>
      </div>
    </Transition>

    <!-- Centered Profile Settings Modal -->
    <ProfileSettingsModal v-model:isOpen="isProfileModalOpen" />
  </div>
</template>
