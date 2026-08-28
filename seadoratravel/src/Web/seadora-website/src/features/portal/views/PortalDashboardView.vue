<template>
  <div class="space-y-6">
    <div class="bg-white rounded-3xl p-8 border border-slate-200/80 shadow-sm relative overflow-hidden">
      <div class="absolute inset-0 bg-[url('/pattern.svg')] opacity-5 pointer-events-none"></div>
      <div class="relative z-10 flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
        <div>
          <h1 class="text-3xl font-bold text-slate-900 mb-2">Welcome back, {{ authStore.user?.name.split(' ')[0] || 'VIP' }}!</h1>
          <p class="text-slate-500">Your next adventure to <strong class="text-[#062d4d]">Luxor & Aswan</strong> begins in:</p>
          <div class="flex gap-4 mt-4">
            <div class="bg-slate-50 border border-slate-100 rounded-xl p-3 text-center min-w-[70px]">
              <div class="text-2xl font-bold text-[#c9a84c]">12</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest">Days</div>
            </div>
            <div class="bg-slate-50 border border-slate-100 rounded-xl p-3 text-center min-w-[70px]">
              <div class="text-2xl font-bold text-[#c9a84c]">08</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest">Hours</div>
            </div>
          </div>
        </div>
        <div class="bg-[#F8FAFC] border border-slate-200/80 text-[#062d4d] rounded-2xl p-6 w-full md:w-auto shadow-sm">
          <div class="text-sm font-bold opacity-80 mb-1">LUXOR, EGYPT</div>
          <div class="text-3xl font-bold mb-1">32°C</div>
          <div class="text-sm font-medium">Sunny ☀️</div>
        </div>
      </div>
    </div>

    <!-- CTA Section -->
    <div class="bg-white rounded-2xl p-6 border border-slate-200/80 shadow-sm flex flex-col md:flex-row justify-between items-center gap-4">
      <div>
        <h2 class="text-xl font-bold text-slate-900 mb-1">Ready for your next adventure?</h2>
        <p class="text-slate-500 text-sm">Discover our exclusive collection of luxury tours and experiences.</p>
      </div>
      <router-link to="/tours" class="px-6 py-3 bg-[#062d4d] text-white rounded-xl font-bold hover:bg-[#062d4d]/90 transition-colors whitespace-nowrap shadow-sm">
        Book a New Experience
      </router-link>
    </div>
  </div>
</template>
<script setup lang="ts">
import { useAuthStore } from '@/features/auth/store/auth';
const authStore = useAuthStore();
</script>