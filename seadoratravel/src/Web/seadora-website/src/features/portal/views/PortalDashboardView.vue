<template>
  <div class="space-y-6">
    <!-- Welcome Countdown Hero Card -->
    <div class="bg-white rounded-3xl p-8 border border-slate-200/80 shadow-sm relative overflow-hidden">
      <div class="absolute inset-0 bg-[url('/pattern.svg')] opacity-5 pointer-events-none"></div>
      <div class="relative z-10 flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
        <div>
          <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-[#c9a84c]/10 text-[#a38030] text-xs font-bold uppercase tracking-wider mb-3">
            <span>✦</span> Seadora VIP Elite Member
          </div>
          <h1 class="text-3xl font-bold text-slate-900 mb-2">Welcome back, {{ authStore.user?.name || 'VIP Traveler' }}!</h1>
          <p class="text-slate-500 text-sm">Your next private luxury journey to <strong class="text-[#062d4d]">Luxor & Nile Valley</strong> begins in:</p>
          <div class="flex gap-4 mt-4">
            <div class="bg-slate-50 border border-slate-200/60 rounded-xl p-3 text-center min-w-[75px] shadow-xs">
              <div class="text-2xl font-bold text-[#c9a84c]">12</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-bold">Days</div>
            </div>
            <div class="bg-slate-50 border border-slate-200/60 rounded-xl p-3 text-center min-w-[75px] shadow-xs">
              <div class="text-2xl font-bold text-[#c9a84c]">08</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-bold">Hours</div>
            </div>
            <div class="bg-slate-50 border border-slate-200/60 rounded-xl p-3 text-center min-w-[75px] shadow-xs">
              <div class="text-2xl font-bold text-[#c9a84c]">45</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-bold">Mins</div>
            </div>
          </div>
        </div>
        <div class="bg-gradient-to-br from-slate-50 to-slate-100 border border-slate-200 text-[#062d4d] rounded-2xl p-6 w-full md:w-auto shadow-xs min-w-[220px]">
          <div class="text-xs font-bold text-slate-500 uppercase tracking-wider mb-1">CURRENT DESTINATION</div>
          <div class="text-2xl font-bold mb-1">Luxor, Egypt</div>
          <div class="text-3xl font-bold text-[#c9a84c] mb-1">32°C</div>
          <div class="text-xs font-medium text-slate-600">Sunny & Calm ☀️</div>
        </div>
      </div>
    </div>

    <!-- Quick Action Cards Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <router-link to="/tours" class="bg-white rounded-2xl p-5 border border-slate-200/80 shadow-xs hover:shadow-md hover:border-[#c9a84c]/50 transition-all group flex flex-col justify-between">
        <div>
          <div class="w-10 h-10 rounded-xl bg-[#062d4d] text-white flex items-center justify-center text-lg mb-3 group-hover:scale-105 transition-transform">
            ⛵
          </div>
          <h3 class="font-bold text-slate-900 text-sm">Browse Tours & Trips</h3>
          <p class="text-xs text-slate-500 mt-1">Explore all curated luxury Red Sea & Nile experiences.</p>
        </div>
        <span class="text-xs font-bold text-[#062d4d] mt-4 flex items-center gap-1 group-hover:translate-x-1 transition-transform">Book Experience →</span>
      </router-link>

      <router-link to="/portal/support" class="bg-gradient-to-br from-amber-50/50 to-amber-100/30 rounded-2xl p-5 border border-amber-200/80 shadow-xs hover:shadow-md hover:border-amber-300 transition-all group flex flex-col justify-between">
        <div>
          <div class="w-10 h-10 rounded-xl bg-[#c9a84c] text-[#062d4d] flex items-center justify-center text-lg mb-3 group-hover:scale-105 transition-transform">
            ✨
          </div>
          <h3 class="font-bold text-slate-900 text-sm">Send VIP Request</h3>
          <p class="text-xs text-slate-600 mt-1">Request custom yachts, private flights, or custom itineraries.</p>
        </div>
        <span class="text-xs font-bold text-[#a38030] mt-4 flex items-center gap-1 group-hover:translate-x-1 transition-transform">Request Concierge →</span>
      </router-link>

      <router-link to="/portal/bookings" class="bg-white rounded-2xl p-5 border border-slate-200/80 shadow-xs hover:shadow-md hover:border-[#c9a84c]/50 transition-all group flex flex-col justify-between">
        <div>
          <div class="w-10 h-10 rounded-xl bg-blue-50 text-[#062d4d] flex items-center justify-center text-lg mb-3 group-hover:scale-105 transition-transform">
            📅
          </div>
          <h3 class="font-bold text-slate-900 text-sm">My Bookings & Trips</h3>
          <p class="text-xs text-slate-500 mt-1">View vouchers, payment breakdowns, and trip manifests.</p>
        </div>
        <span class="text-xs font-bold text-[#062d4d] mt-4 flex items-center gap-1 group-hover:translate-x-1 transition-transform">Manage Bookings →</span>
      </router-link>

      <router-link to="/portal/documents" class="bg-white rounded-2xl p-5 border border-slate-200/80 shadow-xs hover:shadow-md hover:border-[#c9a84c]/50 transition-all group flex flex-col justify-between">
        <div>
          <div class="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-700 flex items-center justify-center text-lg mb-3 group-hover:scale-105 transition-transform">
            📄
          </div>
          <h3 class="font-bold text-slate-900 text-sm">Travel Document Vault</h3>
          <p class="text-xs text-slate-500 mt-1">Download official PDF vouchers and manage passenger passports.</p>
        </div>
        <span class="text-xs font-bold text-emerald-700 mt-4 flex items-center gap-1 group-hover:translate-x-1 transition-transform">Open Documents →</span>
      </router-link>
    </div>

    <!-- Booking CTA Banner -->
    <div class="bg-white rounded-2xl p-6 border border-slate-200/80 shadow-sm flex flex-col md:flex-row justify-between items-center gap-4">
      <div>
        <h2 class="text-xl font-bold text-slate-900 mb-1">Ready for your next adventure?</h2>
        <p class="text-slate-500 text-sm">Discover our exclusive collection of luxury tours, private charters, and historical expeditions.</p>
      </div>
      <div class="flex items-center gap-3">
        <router-link to="/tours" class="px-6 py-3 bg-[#062d4d] text-white rounded-xl font-bold hover:bg-[#062d4d]/90 transition-colors whitespace-nowrap shadow-sm text-sm">
          Book a New Experience
        </router-link>
        <router-link to="/portal/support" class="px-5 py-3 bg-[#c9a84c] text-[#062d4d] rounded-xl font-bold hover:bg-[#d8b85c] transition-colors whitespace-nowrap shadow-sm text-sm">
          ✨ Bespoke Request
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/features/auth/store/auth';
const authStore = useAuthStore();
</script>