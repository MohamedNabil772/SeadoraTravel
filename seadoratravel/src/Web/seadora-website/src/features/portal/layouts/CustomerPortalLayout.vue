<template>
  <div class="min-h-screen bg-[#F8FAFC] flex flex-col md:flex-row text-slate-900 font-sans selection:bg-[#c9a84c]/20">
    <!-- Mobile Header -->
    <header class="md:hidden bg-white/95 backdrop-blur-md border-b border-slate-200/80 px-4 py-3 flex justify-between items-center sticky top-0 z-40 shadow-xs">
      <router-link to="/" class="flex items-center gap-2.5 active:scale-95 transition-transform">
        <img src="/logo-emblem.png" alt="Seadora" class="w-8 h-8 object-contain" />
        <span class="font-bold tracking-widest text-base text-[#062d4d]">SEADORA</span>
      </router-link>
      <div class="flex items-center gap-3">
        <CustomerProfileDropdown />
        <button 
          @click="mobileMenuOpen = !mobileMenuOpen" 
          class="p-2 rounded-xl text-slate-600 hover:bg-slate-100 active:scale-95 transition-all focus:outline-none"
          aria-label="Toggle mobile menu"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/></svg>
        </button>
      </div>
    </header>

    <!-- Sidebar with Luxury Deep Navy Theme -->
    <aside 
      :class="[
        'fixed inset-y-0 left-0 z-50 w-64 bg-[#062d4d] border-r border-white/10 transform transition-transform duration-400 ease-[cubic-bezier(0.16,1,0.3,1)] md:relative md:translate-x-0 flex flex-col shadow-2xl md:shadow-none',
        mobileMenuOpen ? 'translate-x-0' : '-translate-x-full'
      ]"
    >
      <!-- Logo & Brand Lounge Header -->
      <router-link to="/" class="p-6 hidden md:flex items-center gap-3 border-b border-white/10 hover:bg-white/5 transition-colors group">
        <img src="/logo-emblem.png" alt="Seadora" class="w-10 h-10 object-contain drop-shadow-md group-hover:scale-105 transition-transform duration-300 ease-[cubic-bezier(0.16,1,0.3,1)]" />
        <div class="flex flex-col">
          <span class="font-bold tracking-widest text-lg text-white">SEADORA</span>
          <span class="text-[9px] text-[#c9a84c] tracking-[0.25em] uppercase font-semibold">{{ $t('portal.dashboard.vipLounge') }}</span>
        </div>
      </router-link>

      <!-- Back to Main Website Action -->
      <div class="px-4 pt-4 pb-2">
        <router-link 
          to="/" 
          class="flex items-center gap-2.5 px-3.5 py-2.5 rounded-xl text-xs font-medium text-white/60 hover:text-white hover:bg-white/10 active:scale-[0.97] transition-[background-color,color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)]"
        >
          <span>←</span>
          <span>{{ $t('portal.nav.returnHome') }}</span>
        </router-link>
      </div>

      <!-- Navigation Links -->
      <nav class="flex-1 px-4 py-2 flex flex-col gap-1.5 overflow-y-auto">
        <router-link 
          v-for="item in navItems" 
          :key="item.path"
          :to="item.path"
          class="relative flex items-center gap-3.5 px-4 py-3 rounded-xl text-xs font-semibold tracking-wide transition-[background-color,color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] active:scale-[0.97] group"
          :class="{
            'text-white bg-white/12 shadow-sm ring-1 ring-white/10': currentRoute === item.path,
            'text-white/70 hover:text-white hover:bg-white/5': currentRoute !== item.path
          }"
          @click="mobileMenuOpen = false"
        >
          <!-- Active Indicator -->
          <div v-if="currentRoute === item.path" class="absolute left-0 top-1/2 -translate-y-1/2 w-1.5 h-6 bg-[#c9a84c] rounded-r-md"></div>
          
          <component :is="item.icon" class="w-4 h-4 transition-transform duration-300 group-hover:scale-110" :class="{ 'text-[#c9a84c]': currentRoute === item.path }" />
          <span>{{ item.label }}</span>
        </router-link>
      </nav>

      <!-- Dedicated Concierge Badge in Sidebar Footer -->
      <div class="p-4 border-t border-white/10 m-4 rounded-2xl bg-white/5 backdrop-blur-sm">
        <div class="flex items-center gap-2.5">
          <div class="w-7 h-7 rounded-lg bg-[#c9a84c]/20 flex items-center justify-center text-[#c9a84c] text-xs font-bold">
            ✦
          </div>
          <div>
            <div class="text-[11px] font-bold text-white">{{ $t('portal.nav.vipBadge') }}</div>
            <div class="text-[10px] text-white/50">{{ $t('portal.nav.vipPriority') }}</div>
          </div>
        </div>
      </div>
    </aside>

    <!-- Mobile Drawer Overlay -->
    <div 
      v-if="mobileMenuOpen" 
      @click="mobileMenuOpen = false" 
      class="fixed inset-0 bg-[#062d4d]/70 backdrop-blur-sm z-40 md:hidden transition-opacity duration-300"
    ></div>

    <!-- Main Shell Area -->
    <div class="flex-1 flex flex-col relative h-screen overflow-hidden">
      <!-- Desktop Topbar -->
      <header class="hidden md:flex justify-between items-center px-8 py-4 bg-white/85 backdrop-blur-xl border-b border-slate-200/80 sticky top-0 z-30 shadow-xs">
        <div class="flex items-center gap-4">
          <router-link 
            to="/tours" 
            class="px-4 py-2 bg-slate-100 hover:bg-slate-200/80 active:scale-[0.97] text-slate-700 text-xs font-bold rounded-xl transition-[background-color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] flex items-center gap-2"
          >
            <span>⛵</span>
            <span>{{ $t('portal.nav.exploreAll') }}</span>
          </router-link>
        </div>
        <div class="flex items-center gap-4">
          <CustomerProfileDropdown />
        </div>
      </header>

      <!-- Dynamic View Area -->
      <main class="flex-1 relative overflow-y-auto p-4 md:p-8 2xl:p-10">
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
import { useI18n } from 'vue-i18n';
import CustomerProfileDropdown from '@/shared/components/CustomerProfileDropdown.vue';

const { t } = useI18n();
const route = useRoute();
const mobileMenuOpen = ref(false);

const currentRoute = computed(() => route.path);

// Inline SVG Icon components
import { h } from 'vue';
const IconDashboard = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('rect', { x: '3', y: '3', width: '7', height: '9' }), h('rect', { x: '14', y: '3', width: '7', height: '5' }), h('rect', { x: '14', y: '12', width: '7', height: '9' }), h('rect', { x: '3', y: '16', width: '7', height: '5' })]);
const IconBookings = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M19 4H5a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2V6a2 2 0 00-2-2z' }), h('path', { d: 'M16 2v4' }), h('path', { d: 'M8 2v4' }), h('path', { d: 'M3 10h18' })]);
const IconDocuments = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z' }), h('polyline', { points: '14 2 14 8 20 8' }), h('line', { x1: '16', y1: '13', x2: '8', y2: '13' }), h('line', { x1: '16', y1: '17', x2: '8', y2: '17' }), h('polyline', { points: '10 9 9 9 8 9' })]);
const IconProfile = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2' }), h('circle', { cx: '12', cy: '7', r: '4' })]);
const IconFavorites = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z' })]);
const IconSupport = () => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round' }, [h('path', { d: 'M21 11.5a8.38 8.38 0 01-.9 3.8 8.5 8.5 0 01-7.6 4.7 8.38 8.38 0 01-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 01-.9-3.8 8.5 8.5 0 014.7-7.6 8.38 8.38 0 013.8-.9h.5a8.48 8.48 0 018 8v.5z' })]);

const navItems = computed(() => [
  { path: '/portal', label: t('portal.nav.dashboard'), icon: IconDashboard },
  { path: '/portal/favorites', label: t('portal.nav.favorites') || 'Saved Favorites', icon: IconFavorites },
  { path: '/portal/bookings', label: t('portal.nav.bookings'), icon: IconBookings },
  { path: '/portal/documents', label: t('portal.nav.documents'), icon: IconDocuments },
  { path: '/portal/profile', label: t('portal.nav.profile'), icon: IconProfile },
  { path: '/portal/support', label: t('portal.nav.support'), icon: IconSupport }
]);
</script>

<style scoped>
.portal-fade-enter-active,
.portal-fade-leave-active {
  transition: opacity 0.4s cubic-bezier(0.16, 1, 0.3, 1), transform 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}
.portal-fade-enter-from {
  opacity: 0;
  transform: translateY(8px) scale(0.99);
}
.portal-fade-leave-to {
  opacity: 0;
  transform: translateY(-8px) scale(0.99);
}
</style>
