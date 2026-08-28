<template>
  <div class="space-y-6">
    <div class="bg-gradient-to-r from-[#062d4d] to-[#041a2e] rounded-3xl p-8 border border-white/10 shadow-2xl relative overflow-hidden">
      <div class="absolute inset-0 bg-[url('/pattern.svg')] opacity-5 pointer-events-none"></div>
      <div class="relative z-10 flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
        <div>
          <h1 class="text-3xl font-bold text-white mb-2">Welcome back, {{ authStore.user?.name.split(' ')[0] || 'VIP' }}!</h1>
          <p class="text-white/70">Your next adventure to <strong class="text-[#c9a84c]">Luxor & Aswan</strong> begins in:</p>
          <div class="flex gap-4 mt-4">
            <div class="bg-[#041a2e]/50 backdrop-blur-sm border border-white/10 rounded-xl p-3 text-center min-w-[70px]">
              <div class="text-2xl font-bold text-[#c9a84c]">12</div>
              <div class="text-[10px] text-white/50 uppercase tracking-widest">Days</div>
            </div>
            <div class="bg-[#041a2e]/50 backdrop-blur-sm border border-white/10 rounded-xl p-3 text-center min-w-[70px]">
              <div class="text-2xl font-bold text-[#c9a84c]">08</div>
              <div class="text-[10px] text-white/50 uppercase tracking-widest">Hours</div>
            </div>
          </div>
        </div>
        <div class="bg-gradient-to-br from-[#c9a84c] to-[#a38030] text-[#062d4d] rounded-2xl p-6 w-full md:w-auto shadow-lg shadow-[#c9a84c]/20">
          <div class="text-sm font-bold opacity-80 mb-1">LUXOR, EGYPT</div>
          <div class="text-3xl font-bold mb-1">32°C</div>
          <div class="text-sm font-medium">Sunny â˜€ï¸ </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { useAuthStore } from '@/features/auth/store/auth';
const authStore = useAuthStore();
</script>