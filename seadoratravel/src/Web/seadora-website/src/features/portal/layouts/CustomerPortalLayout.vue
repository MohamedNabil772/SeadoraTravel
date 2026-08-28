<template>
  <div class="min-h-screen bg-[#041a2e] flex flex-col md:flex-row text-white overflow-hidden font-sans">
    <!-- Mobile Header -->
    <header class="md:hidden bg-[#062d4d]/80 backdrop-blur-md border-b border-white/10 p-4 flex justify-between items-center sticky top-0 z-40">
      <div class="flex items-center gap-3">
        <img src="/logo-emblem.png" alt="Seadora" class="w-8 h-8 object-contain" />
        <span class="font-bold tracking-widest text-[#c9a84c]">PORTAL</span>
      </div>
      <button @click="mobileMenuOpen = !mobileMenuOpen" class="text-white">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/></svg>
      </button>
    </header>

    <!-- Sidebar -->
    <aside 
      :class="[
        'fixed inset-y-0 left-0 z-50 w-64 bg-[#062d4d]/90 backdrop-blur-xl border-r border-white/10 transform transition-transform duration-500 ease-[cubic-bezier(0.16,1,0.3,1)] md:relative md:translate-x-0 flex flex-col',
        mobileMenuOpen ? 'translate-x-0' : '-translate-x-full'
      ]"
    >
      <!-- Logo Area -->
      <div class="p-6 hidden md:flex items-center gap-3 border-b border-white/5">
        <img src="/logo-emblem.png" alt="Seadora" class="w-10 h-10 object-contain drop-shadow-md" />
        <div class="flex flex-col">
          <span class="font-bold tracking-widest text-lg text-white">SEADORA</span>
          <span class="text-[10px] text-[#c9a84c] tracking-[0.2em] uppercase">VIP Portal</span>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 px-4 py-6 flex flex-col gap-2 overflow-y-auto">
        <router-link 
          v-for="item in navItems" 
          :key="item.path"
          :to="item.path"
          class="flex items-center gap-4 px-4 py-3 rounded-xl transition-all duration-300 group"
          :class="{
            'bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-semibold shadow-lg shadow-[#c9a84c]/20': currentRoute === item.path,
            'text-white/70 hover:text-white hover:bg-white/5': currentRoute !== item.path
          }"
          @click="mobileMenuOpen = false"
        >
          <component :is="item.icon" class="w-5 h-5 transition-transform duration-300 group-hover:scale-110" />
          {{ item.label }}
        </router-link>
      </nav>

      <!-- User Profile Summary -->
      <div class="p-4 m-4 rounded-2xl bg-gradient-to-br from-white/10 to-transparent border border-white/10 backdrop-blur-md">
        <div class="flex items-center gap-3 mb-3">
          <div class="w-10 h-10 rounded-full bg-gradient-to-tr from-[#c9a84c] to-white/20 flex items-center justify-center text-[#062d4d] font-bold shadow-inner">
            {{ userInitials }}
          </div>
          <div>
            <div class="text-sm font-semibold truncate">{{ authStore.user?.name || 'VIP Guest' }}</div>
            <div class="text-[10px] text-[#c9a84c] uppercase tracking-wider flex items-center gap-1">
              <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/></svg>
              Gold Member
            </div>
          </div>
        </div>
        <button @click="handleLogout" class="w-full py-2 text-xs text-white/50 hover:text-[#c9a84c] hover:bg-white/5 rounded-lg transition-colors flex items-center justify-center gap-2">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/></svg>
          Sign Out
        </button>
      </div>
    </aside>

    <!-- Overlay for mobile sidebar -->
    <div 
      v-if="mobileMenuOpen" 
      @click="mobileMenuOpen = false" 
      class="fixed inset-0 bg-[#041a2e]/80 backdrop-blur-sm z-40 md:hidden transition-opacity"
    ></div>

    <!-- Main Content Area -->
    <main class="flex-1 relative overflow-y-auto overflow-x-hidden p-4 md:p-8">
      <router-view v-slot="{ Component }">
        <transition 
          name="portal-fade"
          mode="out-in"
        >
          <component :is="Component" />
        </transition>
      </router-view>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/features/auth/store/auth';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const mobileMenuOpen = ref(false);

const currentRoute = computed(() => route.path);

const userInitials = computed(() => {
  const name = authStore.user?.name || 'V';
  return name.charAt(0).toUpperCase();
});

const handleLogout = () => {
  authStore.logout();
  router.push('/');
};

// Icons (inline SVG components for simplicity)
import { h } from 'vue';
const IconDashboard = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('rect', { x: '3', y: '3', width: '7', height: '9' }), h('rect', { x: '14', y: '3', width: '7', height: '5' }), h('rect', { x: '14', y: '12', width: '7', height: '9' }), h('rect', { x: '3', y: '16', width: '7', height: '5' })]);
const IconBookings = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M19 4H5a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2V6a2 2 0 00-2-2z' }), h('path', { d: 'M16 2v4' }), h('path', { d: 'M8 2v4' }), h('path', { d: 'M3 10h18' })]);
const IconDocuments = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z' }), h('polyline', { points: '14 2 14 8 20 8' }), h('line', { x1: '16', y1: '13', x2: '8', y2: '13' }), h('line', { x1: '16', y1: '17', x2: '8', y2: '17' }), h('polyline', { points: '10 9 9 9 8 9' })]);
const IconProfile = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2' }), h('circle', { cx: '12', cy: '7', r: '4' })]);
const IconSupport = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M21 11.5a8.38 8.38 0 01-.9 3.8 8.5 8.5 0 01-7.6 4.7 8.38 8.38 0 01-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 01-.9-3.8 8.5 8.5 0 014.7-7.6 8.38 8.38 0 013.8-.9h.5a8.48 8.48 0 018 8v.5z' })]);

const navItems = [
  { path: '/portal', label: 'Dashboard', icon: IconDashboard },
  { path: '/portal/bookings', label: 'My Bookings', icon: IconBookings },
  { path: '/portal/documents', label: 'Documents', icon: IconDocuments },
  { path: '/portal/profile', label: 'Profile', icon: IconProfile },
  { path: '/portal/support', label: 'Support & Concierge', icon: IconSupport }
];
</script>

<style scoped>
.portal-fade-enter-active,
.portal-fade-leave-active {
  transition: opacity 0.4s cubic-bezier(0.16, 1, 0.3, 1), transform 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}
.portal-fade-enter-from {
  opacity: 0;
  transform: translateY(10px) scale(0.99);
}
.portal-fade-leave-to {
  opacity: 0;
  transform: translateY(-10px) scale(0.99);
}
</style>
