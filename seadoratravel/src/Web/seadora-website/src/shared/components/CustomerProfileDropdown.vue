<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store/auth'
import LuxuryIcons from '@/shared/components/LuxuryIcons.vue'

const authStore = useAuthStore()
const router = useRouter()
const showDropdown = ref(false)

const userInitials = computed(() => {
  if (!authStore.user) return 'VIP'
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
    <!-- Trigger -->
    <button 
      @click="showDropdown = !showDropdown" 
      class="flex items-center gap-3 p-1 rounded-full hover:bg-slate-100 transition-colors focus:outline-none"
    >
      <div class="flex items-center justify-center w-9 h-9 rounded-full bg-[#062d4d] text-white font-semibold text-sm shadow-sm ring-2 ring-white">
        {{ userInitials }}
      </div>
      <div class="hidden md:flex flex-col items-start">
        <span class="text-sm font-medium text-slate-700 leading-tight">{{ authStore.user?.name || 'Guest' }}</span>
        <span class="text-[10px] font-bold text-[#c9a84c] uppercase tracking-wider bg-[#c9a84c]/10 px-1.5 py-0.5 rounded border border-[#c9a84c]/20">
          {{ authStore.user?.role === 'VIP' ? 'VIP Guest' : 'Traveler' }}
        </span>
      </div>
      <LuxuryIcons name="chevron-down" size="14" class="text-slate-400 hidden md:block transition-transform duration-200" :class="showDropdown ? 'rotate-180' : ''" />
    </button>

    <!-- Dropdown Menu -->
    <Transition
      enter-active-class="transition ease-out duration-100"
      enter-from-class="transform opacity-0 scale-95"
      enter-to-class="transform opacity-100 scale-100"
      leave-active-class="transition ease-in duration-75"
      leave-from-class="transform opacity-100 scale-100"
      leave-to-class="transform opacity-0 scale-95"
    >
      <div v-if="showDropdown" class="absolute right-0 mt-2 w-64 origin-top-right bg-white border border-slate-200/80 rounded-2xl shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none z-50 overflow-hidden">
        
        <!-- Header -->
        <div class="px-4 py-3 border-b border-slate-100 bg-[#F8FAFC]">
          <p class="text-sm font-medium text-slate-900 truncate">{{ authStore.user?.name || 'Guest' }}</p>
          <p class="text-xs text-slate-500 truncate">{{ authStore.user?.email || 'guest@example.com' }}</p>
        </div>

        <!-- Links -->
        <div class="py-1">
          <router-link to="/portal/profile" @click="showDropdown = false" class="group flex items-center px-4 py-2.5 text-sm text-slate-700 hover:bg-slate-50 hover:text-[#062d4d] transition-colors">
            <LuxuryIcons name="user" size="16" class="mr-3 text-slate-400 group-hover:text-[#062d4d] transition-colors" />
            My Profile Settings
          </router-link>
          
          <router-link to="/portal/documents" @click="showDropdown = false" class="group flex items-center px-4 py-2.5 text-sm text-slate-700 hover:bg-slate-50 hover:text-[#062d4d] transition-colors">
            <LuxuryIcons name="file-text" size="16" class="mr-3 text-slate-400 group-hover:text-[#062d4d] transition-colors" />
            Travel Documents
          </router-link>

          <router-link to="/portal/support" @click="showDropdown = false" class="group flex items-center px-4 py-2.5 text-sm text-slate-700 hover:bg-slate-50 hover:text-[#062d4d] transition-colors">
            <LuxuryIcons name="headphones" size="16" class="mr-3 text-slate-400 group-hover:text-[#062d4d] transition-colors" />
            Support & Requests
          </router-link>
        </div>

        <!-- Logout -->
        <div class="border-t border-slate-100 py-1">
          <button @click="handleLogout" class="group flex w-full items-center px-4 py-2.5 text-sm text-red-600 hover:bg-red-50 transition-colors">
            <LuxuryIcons name="log-out" size="16" class="mr-3 text-red-500 group-hover:text-red-600 transition-colors" />
            Logout
          </button>
        </div>

      </div>
    </Transition>
  </div>
</template>
