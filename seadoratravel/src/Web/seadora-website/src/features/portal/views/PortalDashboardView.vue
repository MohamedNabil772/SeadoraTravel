<template>
  <div class="space-y-8">
    <!-- Welcome & Journey Countdown Hero Card -->
    <div class="bg-white rounded-3xl p-8 md:p-10 border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] relative overflow-hidden">
      <div class="absolute -right-16 -top-16 w-80 h-80 bg-gradient-to-br from-[#c9a84c]/10 to-transparent rounded-full blur-3xl pointer-events-none"></div>
      
      <div class="relative z-10 flex flex-col lg:flex-row justify-between items-start lg:items-center gap-8">
        <div class="max-w-2xl">
          <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-[#c9a84c]/15 border border-[#c9a84c]/30 text-[#a38030] text-[11px] font-bold uppercase tracking-widest mb-3">
            <span>✦</span> Seadora VIP Elite Lounge
          </div>
          <h1 class="text-3xl md:text-4xl font-bold text-slate-900 tracking-tight leading-tight">
            Welcome back to Seadora, <span class="text-[#062d4d]">{{ customerFirstName }}</span>.
          </h1>
          <p class="text-slate-500 text-sm md:text-base mt-2 leading-relaxed">
            Anticipating your arrival in <strong class="text-slate-800">Luxor & the Nile Valley</strong> for your private Dahabiya voyage:
          </p>

          <!-- Countdown Timer Pills -->
          <div class="flex flex-wrap gap-3.5 mt-6">
            <div class="bg-[#F8FAFC] border border-slate-200/80 rounded-2xl px-4 py-3 text-center min-w-[80px] shadow-xs">
              <div class="text-2xl md:text-3xl font-bold text-[#c9a84c] tracking-tight">12</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-semibold mt-0.5">Days</div>
            </div>
            <div class="bg-[#F8FAFC] border border-slate-200/80 rounded-2xl px-4 py-3 text-center min-w-[80px] shadow-xs">
              <div class="text-2xl md:text-3xl font-bold text-[#c9a84c] tracking-tight">08</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-semibold mt-0.5">Hours</div>
            </div>
            <div class="bg-[#F8FAFC] border border-slate-200/80 rounded-2xl px-4 py-3 text-center min-w-[80px] shadow-xs">
              <div class="text-2xl md:text-3xl font-bold text-[#c9a84c] tracking-tight">45</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-semibold mt-0.5">Mins</div>
            </div>
          </div>
        </div>

        <!-- Destination Weather & Live Status Widget -->
        <div class="bg-[#062d4d] text-white rounded-3xl p-6 md:p-8 w-full lg:w-auto shadow-xl min-w-[260px] relative overflow-hidden flex flex-col justify-between">
          <div class="absolute -bottom-8 -right-8 w-32 h-32 bg-[#c9a84c]/20 rounded-full blur-2xl"></div>
          <div>
            <div class="text-[10px] font-bold text-[#c9a84c] uppercase tracking-widest mb-1">DESTINATION CLIMATE</div>
            <div class="text-xl font-bold">Luxor, Egypt</div>
            <div class="text-3xl md:text-4xl font-bold text-white mt-3 mb-1">32°C</div>
            <div class="text-xs text-white/70 font-medium">Clear Skies · Gentle Breeze ☀️</div>
          </div>
          <div class="mt-6 pt-4 border-t border-white/10 flex items-center justify-between text-xs text-white/80">
            <span>Water: 27°C</span>
            <span class="text-[#c9a84c] font-semibold">Optimal Sailing</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Asymmetrical Editorial Layout: Upcoming Journey vs Concierge Actions -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      
      <!-- Main Featured Trip Card (2 Columns) -->
      <div class="lg:col-span-2 bg-white rounded-3xl p-8 border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] flex flex-col justify-between">
        <div>
          <div class="flex items-center justify-between gap-4 mb-4">
            <div class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full bg-emerald-500 animate-pulse"></span>
              <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">Confirmed Active Journey</span>
            </div>
            <span class="font-mono text-xs font-bold text-[#062d4d] bg-slate-100 px-2.5 py-1 rounded-lg">#BK-10293</span>
          </div>

          <h2 class="text-xl md:text-2xl font-bold text-slate-900 tracking-tight mb-2">
            7-Night Private Dahabiya Expedition: Luxor to Aswan
          </h2>
          <p class="text-slate-500 text-sm leading-relaxed mb-6">
            Private luxury Nile sailing with onboard Egyptologist, private chef, fast-track temple access, and sunset felucca dining.
          </p>

          <!-- Key Journey Details Pills -->
          <div class="grid grid-cols-2 sm:grid-cols-3 gap-3 mb-6">
            <div class="bg-slate-50 border border-slate-100 rounded-2xl p-3.5">
              <div class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Dates</div>
              <div class="text-xs font-bold text-slate-800 mt-0.5">Oct 30 – Nov 6</div>
            </div>
            <div class="bg-slate-50 border border-slate-100 rounded-2xl p-3.5">
              <div class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Guests</div>
              <div class="text-xs font-bold text-slate-800 mt-0.5">2 VIP Guests</div>
            </div>
            <div class="bg-slate-50 border border-slate-100 rounded-2xl p-3.5">
              <div class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Passports</div>
              <div class="text-xs font-bold text-emerald-600 mt-0.5">Verified on File ✓</div>
            </div>
          </div>
        </div>

        <div class="pt-4 border-t border-slate-100 flex flex-wrap items-center justify-between gap-4">
          <router-link 
            to="/portal/bookings" 
            class="text-xs font-bold text-[#062d4d] hover:text-[#c9a84c] transition-colors flex items-center gap-1.5"
          >
            <span>View Full Itinerary & Manifest</span>
            <span>→</span>
          </router-link>
          
          <router-link 
            to="/portal/documents" 
            class="px-4 py-2 bg-slate-100 hover:bg-[#062d4d] hover:text-white active:scale-[0.97] text-slate-700 text-xs font-bold rounded-xl transition-[background-color,color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)]"
          >
            Download Digital Vouchers
          </router-link>
        </div>
      </div>

      <!-- Right Column: Bespoke Concierge & Quick Portals -->
      <div class="space-y-6 flex flex-col justify-between">
        
        <!-- Bespoke VIP Concierge Banner Card -->
        <div class="bg-gradient-to-br from-[#062d4d] to-[#0d3f66] text-white rounded-3xl p-7 shadow-lg flex-1 flex flex-col justify-between relative overflow-hidden">
          <div class="relative z-10">
            <div class="w-10 h-10 rounded-2xl bg-[#c9a84c] text-[#062d4d] flex items-center justify-center font-bold text-base mb-4 shadow-md">
              ✦
            </div>
            <h3 class="text-lg font-bold mb-1">Private Bespoke Concierge</h3>
            <p class="text-white/70 text-xs leading-relaxed">
              Require a private helicopter transfer, luxury yacht charter, or exclusive dinner reservations?
            </p>
          </div>
          <router-link 
            to="/portal/support" 
            class="mt-6 inline-flex items-center justify-center px-5 py-3 bg-[#c9a84c] hover:bg-[#d8b85c] text-[#062d4d] font-bold rounded-xl text-xs shadow-md active:scale-[0.97] transition-[background-color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)]"
          >
            Send Bespoke Request
          </router-link>
        </div>

        <!-- Curate New Journey CTA Card -->
        <div class="bg-white rounded-3xl p-7 border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] flex flex-col justify-between">
          <div>
            <h3 class="text-base font-bold text-slate-900 mb-1">Curate a New Journey</h3>
            <p class="text-slate-500 text-xs leading-relaxed">
              Explore our hand-crafted Red Sea diving safaris, Cairo antiquities, and oasis retreats.
            </p>
          </div>
          <router-link 
            to="/tours" 
            class="mt-4 inline-flex items-center justify-center px-5 py-2.5 bg-slate-100 hover:bg-slate-200 text-slate-800 font-bold rounded-xl text-xs active:scale-[0.97] transition-[background-color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)]"
          >
            Browse Experiences →
          </router-link>
        </div>

      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useAuthStore } from '@/features/auth/store/auth';

const authStore = useAuthStore();

const customerFirstName = computed(() => {
  if (!authStore.user?.name) return 'VIP Guest';
  return authStore.user.name.split(' ')[0];
});
</script>