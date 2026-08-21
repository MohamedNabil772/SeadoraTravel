<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store/auth'
import { useNotificationStore } from '@/features/notifications/store/notificationStore'
import NotificationDropdown from '@/features/notifications/components/NotificationDropdown.vue'
import { Toaster } from 'vue-sonner'
import { Search, Menu } from 'lucide-vue-next'

const auth = useAuthStore()
const notificationStore = useNotificationStore()
const router = useRouter()

onMounted(() => {
  notificationStore.startPolling(15000) // Poll every 15s
})

onUnmounted(() => {
  notificationStore.stopPolling()
})

const isSidebarOpen = ref(false)

function toggleSidebar() {
  isSidebarOpen.value = !isSidebarOpen.value
}

function handleLogout() {
  auth.logout()
  router.push('/')
}
</script>

<template>
  <div class="flex h-screen bg-surface-sunken overflow-hidden font-sans text-text-main selection:bg-secondary/30">
    <!-- Sidebar Overlay for Mobile -->
    <div 
      v-if="isSidebarOpen" 
      @click="isSidebarOpen = false"
      class="fixed inset-0 bg-black/50 z-20 md:hidden backdrop-blur-sm transition-opacity"
    ></div>

    <!-- Sidebar (Luxury Deep Navy) -->
    <aside 
      :class="[
        'w-64 bg-primary text-text-inverse flex-shrink-0 flex flex-col duration-300 ease-linear border-r border-primary-light absolute md:relative z-30 h-full transform md:transform-none',
        isSidebarOpen ? 'translate-x-0' : '-translate-x-full'
      ]"
    >
      <!-- Sidebar Header / Logo -->
      <div class="flex items-center justify-between gap-2 px-6 py-6">
        <RouterLink to="/dashboard" class="flex items-center gap-2.5 transition-transform hover:scale-105 duration-300 ease-out">
          <img src="/logo-emblem.png" alt="Seadora" class="w-10 h-10 object-contain drop-shadow-sm" />
          <span class="text-xl font-bold font-serif tracking-widest text-secondary">SEADORA <span class="text-white text-sm tracking-widest font-sans opacity-80">ADMIN</span></span>
        </RouterLink>
      </div>

      <!-- Navigation Links -->
      <nav class="mt-4 flex-1 px-4 space-y-6 overflow-y-auto no-scrollbar pb-6">
        
        <!-- CORE -->
        <div>
          <div class="px-4 mb-2 text-[10px] font-bold text-white/40 uppercase tracking-widest">Core</div>
          <div class="space-y-1">
            <RouterLink 
              to="/dashboard" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
              exact
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">📊</span>
              <span>Dashboard</span>
            </RouterLink>
          </div>
        </div>

        <!-- COMMERCE -->
        <div>
          <div class="px-4 mb-2 text-[10px] font-bold text-white/40 uppercase tracking-widest">Commerce</div>
          <div class="space-y-1">
            <RouterLink 
              to="/tours" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">⛵</span>
              <span>Tours</span>
            </RouterLink>
            <RouterLink 
              to="/bookings" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">📅</span>
              <span>Bookings</span>
            </RouterLink>
            <RouterLink 
              to="/inquiries" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">✉️</span>
              <span class="flex-1">VIP Inquiries</span>
              <span v-if="notificationStore.unreadInquiriesCount > 0" class="bg-amber-500 text-white text-[10px] font-bold px-1.5 py-0.5 rounded-full">{{ notificationStore.unreadInquiriesCount }}</span>
            </RouterLink>
          </div>
        </div>

        <!-- TAXONOMY -->
        <div>
          <div class="px-4 mb-2 text-[10px] font-bold text-white/40 uppercase tracking-widest">Taxonomy</div>
          <div class="space-y-1">
            <RouterLink 
              to="/destinations" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">🗺️</span>
              <span>Destinations</span>
            </RouterLink>
            <RouterLink 
              to="/categories" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">🏷️</span>
              <span>Categories</span>
            </RouterLink>
          </div>
        </div>

        <!-- SETTINGS & SYSTEM -->
        <div>
          <div class="px-4 mb-2 text-[10px] font-bold text-white/40 uppercase tracking-widest">System</div>
          <div class="space-y-1">
            <RouterLink 
              to="/settings/languages" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">🌍</span>
              <span>Languages</span>
            </RouterLink>
            <RouterLink 
              to="/settings/currencies" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">💰</span>
              <span>Currencies</span>
            </RouterLink>
            <RouterLink 
              to="/settings/nationalities" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">🏳️</span>
              <span>Nationalities</span>
            </RouterLink>
            <RouterLink 
              to="/users" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">👥</span>
              <span>Users</span>
            </RouterLink>
          </div>
        </div>
      </nav>

      <!-- Sidebar Footer / Logout -->
      <div class="p-4 border-t border-white/5">
        <button 
          @click="handleLogout" 
          class="w-full group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-red-500/10 hover:text-red-400 text-left cursor-pointer"
        >
          <span class="opacity-70 group-hover:opacity-100">🚪</span>
          <span>Logout</span>
        </button>
      </div>
    </aside>

    <!-- Main Shell -->
    <div class="flex-1 flex flex-col overflow-hidden relative">
      <!-- Header -->
      <header class="h-16 bg-white/80 backdrop-blur-md border-b border-border/60 flex items-center justify-between px-4 md:px-8 z-10">
        <div class="flex items-center gap-4">
          <button @click="toggleSidebar" class="md:hidden p-2 -ml-2 text-text-muted hover:text-text-main hover:bg-black/5 rounded-md transition-colors">
            <Menu class="w-5 h-5" />
          </button>
          <h2 class="text-xl font-medium text-text-main tracking-tight animate-fade-in capitalize">
            {{ $route.meta?.title || $route.name?.toString().replace('-', ' ') || 'Dashboard' }}
          </h2>
        </div>
        
        <div class="flex items-center gap-4 md:gap-6">
          
          <div class="relative hidden md:block">
            <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-text-muted" />
            <input 
              type="text" 
              placeholder="Search..." 
              class="pl-9 pr-4 py-1.5 text-sm bg-surface-sunken border border-border/60 rounded-full focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary/30 transition-all w-48 lg:w-64" 
            />
          </div>

          <NotificationDropdown />

          <div class="flex items-center gap-3 pl-2 md:pl-4 border-l border-border/60">
            <div class="text-right hidden sm:block">
              <span class="block text-sm font-semibold text-text-main leading-tight">Administrator</span>
              <span class="block text-[11px] text-text-muted font-sans mt-0.5">{{ auth.user?.email || 'admin@seadora.com' }}</span>
            </div>
            <div class="w-9 h-9 rounded-full bg-gradient-to-tr from-secondary/20 to-secondary/5 border border-secondary/20 flex items-center justify-center text-secondary-dark font-bold text-sm ring-2 ring-white shadow-sm cursor-pointer transition-transform hover:scale-105">
              AD
            </div>
          </div>
        </div>
      </header>

      <!-- Content Area with simple fade transition & persistent trademark footer -->
      <main class="flex-1 overflow-x-hidden overflow-y-auto flex flex-col justify-between bg-surface-sunken">
        <div class="p-6 md:p-8 2xl:p-10 flex-1">
          <router-view v-slot="{ Component }">
            <transition name="fade" mode="out-in">
              <component :is="Component" />
            </transition>
          </router-view>
        </div>

        <!-- Professional Admin Footer with Trademark -->
        <footer class="mt-auto py-4 px-6 md:px-8 border-t border-border/70 bg-white/80 backdrop-blur-md flex flex-col sm:flex-row items-center justify-between gap-3 text-xs text-text-muted select-none">
          <div class="flex items-center gap-2">
            <span class="font-bold text-primary tracking-wider font-serif">SEADORA LUXURY TRAVEL</span>
            <span class="text-text-muted/40">•</span>
            <span>&copy; {{ new Date().getFullYear() }} All Rights Reserved</span>
          </div>
          
          <div class="flex items-center gap-2">
            <span class="text-text-muted text-[11px]">System Architecture & Development by</span>
            <span class="inline-flex items-center gap-1 font-bold text-primary tracking-wide text-xs">
              <span class="text-secondary font-serif text-sm">✦</span> TIM SOLUTIONS<sup class="text-[9px] text-secondary font-bold">®</sup>
            </span>
          </div>
        </footer>
      </main>
    </div>
    <!-- Sonner Toaster for global elegant toasts -->
    <Toaster position="top-right" richColors />
  </div>
</template>

<style>
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

/* Page Transitions */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease-out, transform 0.2s ease-out;
}
.fade-enter-from {
  opacity: 0;
  transform: translateY(4px);
}
.fade-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>
