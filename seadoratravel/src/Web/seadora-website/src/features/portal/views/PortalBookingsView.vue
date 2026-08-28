<template>
  <div class="space-y-6">
    <!-- Header with Action -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-2">
      <div>
        <h1 class="text-2xl font-bold text-slate-900 mb-0">{{ $t('portal.bookings.title') }}</h1>
        <p class="text-xs text-slate-500 mt-1">{{ $t('portal.bookings.subtitle') }}</p>
      </div>
      <router-link 
        to="/tours" 
        class="px-5 py-2.5 bg-[#062d4d] text-white rounded-xl text-xs font-bold shadow-sm hover:bg-[#062d4d]/90 active:scale-[0.97] transition-[background-color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] flex items-center gap-2"
      >
        <span>✦</span>
        <span>{{ $t('portal.bookings.curateNew') }}</span>
      </router-link>
    </div>

    <!-- Journeys Cards List -->
    <div class="grid gap-6">
      <!-- Active Journey 1 -->
      <div class="bg-white rounded-3xl border border-slate-200/80 p-6 md:p-7 flex flex-col md:flex-row gap-6 shadow-[0_8px_30px_rgb(0,0,0,0.04)] hover:shadow-md transition-[box-shadow,transform] duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] active:scale-[0.99] group">
        <div class="w-full md:w-56 h-40 rounded-2xl bg-slate-100 overflow-hidden relative shrink-0">
          <img 
            src="https://images.unsplash.com/photo-1539768942893-daf53e448371?q=80&w=600" 
            class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700 ease-out" 
            alt="7-Night Private Dahabiya Expedition" 
          />
          <div class="absolute top-3 right-3 bg-emerald-500/90 backdrop-blur-md text-white text-[10px] font-bold px-2.5 py-1 rounded-full tracking-wider shadow-sm">
            {{ $t('portal.bookings.confirmed') }}
          </div>
        </div>

        <div class="flex-1 flex flex-col justify-between">
          <div>
            <div class="flex flex-col sm:flex-row justify-between items-start gap-2 mb-2">
              <div>
                <span class="text-[11px] font-bold text-[#c9a84c] tracking-widest uppercase">Nile Valley Expedition</span>
                <h3 class="text-xl font-bold text-slate-900 group-hover:text-[#062d4d] transition-colors mt-0.5">
                  7-Night Private Dahabiya: Luxor to Aswan
                </h3>
              </div>
              <div class="text-left sm:text-right">
                <div class="text-lg font-bold text-slate-900">€2,450</div>
                <div class="text-xs text-emerald-600 font-semibold">{{ $t('portal.bookings.fullyPaid') }} ✓</div>
              </div>
            </div>
            <p class="text-slate-500 text-xs mb-4">
              Oct 30, 2026 – Nov 6, 2026 • 2 VIP Guests • Private Suite Deck • Full Board Catering
            </p>
          </div>

          <div class="flex flex-wrap items-center gap-3 pt-4 border-t border-slate-100">
            <router-link 
              to="/portal/bookings/1" 
              class="px-4 py-2 bg-slate-100 hover:bg-slate-200/80 active:scale-[0.97] rounded-xl text-xs font-bold text-slate-800 transition-[background-color,transform] duration-200"
            >
              {{ $t('portal.bookings.viewItinerary') }}
            </router-link>
            <router-link 
              to="/portal/documents" 
              class="px-4 py-2 bg-[#c9a84c]/15 text-[#a38030] hover:bg-[#c9a84c]/25 active:scale-[0.97] border border-[#c9a84c]/30 rounded-xl text-xs font-bold transition-[background-color,transform] duration-200"
            >
              {{ $t('portal.bookings.downloadVoucher') }}
            </router-link>
          </div>
        </div>
      </div>

      <!-- Past Completed Journey 2 -->
      <div class="bg-white rounded-3xl border border-slate-200/80 p-6 md:p-7 flex flex-col md:flex-row gap-6 shadow-[0_8px_30px_rgb(0,0,0,0.04)] hover:shadow-md transition-[box-shadow,transform] duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] active:scale-[0.99] group opacity-90">
        <div class="w-full md:w-56 h-40 rounded-2xl bg-slate-100 overflow-hidden relative shrink-0">
          <img 
            src="https://images.unsplash.com/photo-1544551763-46a013bb70d5?q=80&w=600" 
            class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700 ease-out" 
            alt="Red Sea Private Yacht Charter" 
          />
          <div class="absolute top-3 right-3 bg-slate-700/80 backdrop-blur-md text-white text-[10px] font-bold px-2.5 py-1 rounded-full tracking-wider shadow-sm">
            {{ $t('portal.bookings.completed') }}
          </div>
        </div>

        <div class="flex-1 flex flex-col justify-between">
          <div>
            <div class="flex flex-col sm:flex-row justify-between items-start gap-2 mb-2">
              <div>
                <span class="text-[11px] font-bold text-slate-400 tracking-widest uppercase">Red Sea Marine Sanctuary</span>
                <h3 class="text-xl font-bold text-slate-900 group-hover:text-[#062d4d] transition-colors mt-0.5">
                  Private Yacht Charter & Dolphin Reef Snorkeling
                </h3>
              </div>
              <div class="text-left sm:text-right">
                <div class="text-lg font-bold text-slate-700">€850</div>
                <div class="text-xs text-slate-400 font-medium">Completed on May 12, 2026</div>
              </div>
            </div>
            <p class="text-slate-500 text-xs mb-4">
              May 12, 2026 • 4 VIP Guests • 55ft Majesty Luxury Cruiser • Private Chef
            </p>
          </div>

          <div class="flex flex-wrap items-center gap-3 pt-4 border-t border-slate-100">
            <router-link 
              to="/portal/documents" 
              class="px-4 py-2 bg-slate-100 hover:bg-slate-200/80 active:scale-[0.97] rounded-xl text-xs font-bold text-slate-800 transition-[background-color,transform] duration-200"
            >
              {{ $t('portal.bookings.viewReceipt') }}
            </router-link>
            <router-link 
              to="/tours" 
              class="px-4 py-2 bg-slate-100 hover:bg-slate-200/80 active:scale-[0.97] rounded-xl text-xs font-bold text-[#062d4d] transition-[background-color,transform] duration-200"
            >
              {{ $t('portal.bookings.rebook') }}
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Portal Bookings View
</script>