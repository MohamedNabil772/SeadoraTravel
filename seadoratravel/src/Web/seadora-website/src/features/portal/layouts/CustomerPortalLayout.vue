<template>
  <div class="min-h-screen bg-[#F8FAFC] flex flex-col md:flex-row text-slate-900 font-sans">
    <!-- Mobile Header -->
    <header class="md:hidden bg-white border-b border-slate-200/80 px-4 py-3 flex justify-between items-center sticky top-0 z-40 shadow-sm">
      <router-link to="/" class="flex items-center gap-3">
        <img src="/logo-emblem.png" alt="Seadora" class="w-8 h-8 object-contain" />
      </router-link>
      <div class="flex items-center gap-3">
        <CustomerProfileDropdown />
        <button @click="mobileMenuOpen = !mobileMenuOpen" class="text-slate-600 focus:outline-none">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/></svg>
        </button>
      </div>
    </header>

    <!-- Sidebar -->
    <aside 
      :class="[
        'fixed inset-y-0 left-0 z-50 w-64 bg-[#062d4d] border-r border-[#062d4d]/10 transform transition-transform duration-500 ease-[cubic-bezier(0.16,1,0.3,1)] md:relative md:translate-x-0 flex flex-col',
        mobileMenuOpen ? 'translate-x-0' : '-translate-x-full'
      ]"
    >
      <!-- Logo Area -->
      <router-link to="/" class="p-6 hidden md:flex items-center gap-3 border-b border-white/10 hover:bg-white/5 transition-colors group">
        <img src="/logo-emblem.png" alt="Seadora" class="w-10 h-10 object-contain drop-shadow-md group-hover:scale-105 transition-transform" />
        <div class="flex flex-col">
          <span class="font-bold tracking-widest text-lg text-white">SEADORA</span>
          <span class="text-[10px] text-[#c9a84c] tracking-[0.2em] uppercase">VIP Portal</span>
        </div>
      </router-link>

      <!-- Navigation -->
      <nav class="flex-1 px-4 py-6 flex flex-col gap-2 overflow-y-auto">
        <router-link to="/" class="flex items-center gap-4 px-4 py-3 rounded-xl transition-all duration-300 text-white/70 hover:text-white hover:bg-white/5 text-sm mb-2 font-medium">
          ← Return to Main Website
        </router-link>
        <router-link 
          v-for="item in navItems" 
          :key="item.path"
          :to="item.path"
          class="relative flex items-center gap-4 px-4 py-3 rounded-xl transition-all duration-300 group"
          :class="{
            'text-white bg-white/10 font-semibold shadow-sm': currentRoute === item.path,
            'text-white/70 hover:text-white hover:bg-white/5': currentRoute !== item.path
          }"
          @click="mobileMenuOpen = false"
        >
          <!-- Active Indicator -->
          <div v-if="currentRoute === item.path" class="absolute left-0 top-1/2 -translate-y-1/2 w-1.5 h-8 bg-[#c9a84c] rounded-r-md"></div>
          
          <component :is="item.icon" class="w-5 h-5 transition-transform duration-300 group-hover:scale-110" :class="{ 'text-[#c9a84c]': currentRoute === item.path }" />
          {{ item.label }}
        </router-link>
      </nav>

    </aside>

    <!-- Overlay for mobile sidebar -->
    <div 
      v-if="mobileMenuOpen" 
      @click="mobileMenuOpen = false" 
      class="fixed inset-0 bg-[#062d4d]/80 backdrop-blur-sm z-40 md:hidden transition-opacity"
    ></div>

    <!-- Main Content Area -->
    <div class="flex-1 flex flex-col relative h-screen overflow-hidden">
      <!-- Desktop Topbar -->
      <header class="hidden md:flex justify-end items-center px-8 py-4 bg-white/80 backdrop-blur-md border-b border-slate-200/80 sticky top-0 z-30 shadow-sm">
        <CustomerProfileDropdown />
      </header>

      <main class="flex-1 relative overflow-y-auto p-4 md:p-8">
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute } from 'vue-router';
import CustomerProfileDropdown from '@/shared/components/CustomerProfileDropdown.vue';

const route = useRoute();
const mobileMenuOpen = ref(false);

const currentRoute = computed(() => route.path);

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
  transition: opacity 0.3s ease, transform 0.3s ease;
}
.portal-fade-enter-from {
  opacity: 0;
  transform: translateY(5px);
}
.portal-fade-leave-to {
  opacity: 0;
  transform: translateY(-5px);
}
</style>
