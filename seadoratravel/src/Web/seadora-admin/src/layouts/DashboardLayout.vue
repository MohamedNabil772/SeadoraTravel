<script setup lang="ts">
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const router = useRouter()

function handleLogout() {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="flex h-screen bg-cream overflow-hidden">
    <!-- Sidebar -->
    <aside class="w-64 bg-sea-deep text-white flex-shrink-0 hidden md:flex flex-col">
      <div class="p-6">
        <h1 class="text-2xl font-serif font-bold text-sun">Seadora Admin</h1>
      </div>
      <nav class="mt-6 flex-1 px-4 space-y-2 overflow-y-auto">
        <RouterLink 
          to="/" 
          class="flex items-center px-4 py-3 rounded-lg hover:bg-sea transition-colors"
          active-class="bg-sea"
        >
          <span class="ml-3">Dashboard 📊</span>
        </RouterLink>
        <RouterLink 
          to="/tours" 
          class="flex items-center px-4 py-3 rounded-lg hover:bg-sea transition-colors"
          active-class="bg-sea"
        >
          <span class="ml-3">Tours ⛵</span>
        </RouterLink>
        <RouterLink 
          to="/destinations" 
          class="flex items-center px-4 py-3 rounded-lg hover:bg-sea transition-colors"
          active-class="bg-sea"
        >
          <span class="ml-3">Destinations 🗺️</span>
        </RouterLink>
        <RouterLink 
          to="/categories" 
          class="flex items-center px-4 py-3 rounded-lg hover:bg-sea transition-colors"
          active-class="bg-sea"
        >
          <span class="ml-3">Categories 🏷️</span>
        </RouterLink>
        <RouterLink 
          to="/bookings" 
          class="flex items-center px-4 py-3 rounded-lg hover:bg-sea transition-colors"
          active-class="bg-sea"
        >
          <span class="ml-3">Bookings 📅</span>
        </RouterLink>
        <RouterLink 
          to="/feedback" 
          class="flex items-center px-4 py-3 rounded-lg hover:bg-sea transition-colors"
          active-class="bg-sea"
        >
          <span class="ml-3">Feedback ⭐</span>
        </RouterLink>
        <RouterLink 
          to="/users" 
          class="flex items-center px-4 py-3 rounded-lg hover:bg-sea transition-colors"
          active-class="bg-sea"
        >
          <span class="ml-3">Users 👥</span>
        </RouterLink>
      </nav>
      <div class="p-4 border-t border-sea">
        <button @click="handleLogout" class="w-full px-4 py-2 text-sm text-left hover:bg-sea rounded-lg transition-colors cursor-pointer">
          Logout 🚪
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <header class="h-16 bg-white border-b border-gray-200 flex items-center justify-between px-8">
        <h2 class="text-xl font-semibold text-sea-deep capitalize">
          {{ $route.name }}
        </h2>
        <div class="flex items-center space-x-4">
          <span class="text-sm text-muted">{{ auth.user?.email || 'Admin User' }}</span>
          <div class="w-8 h-8 rounded-full bg-sun-light flex items-center justify-center text-white font-bold">
            A
          </div>
        </div>
      </header>
      <main class="flex-1 overflow-x-hidden overflow-y-auto p-8 bg-gray-50">
        <RouterView />
      </main>
    </div>
  </div>
</template>
