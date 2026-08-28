<template>
  <div class="space-y-6">
    <router-link to="/portal/bookings" class="text-sm text-slate-500 hover:text-[#062d4d] flex items-center gap-2 mb-4 font-medium">
      ← {{ $t('portal.bookingDetail.back') }}
    </router-link>
    <div class="bg-white rounded-2xl border border-slate-200/80 p-6 shadow-sm">
      <h1 class="text-2xl font-bold text-slate-900 mb-2">7-Night Private Dahabiya: Luxor to Aswan</h1>
      <p class="text-[#c9a84c] font-medium">Booking #BK-10293</p>
      
      <div class="mt-8 border-t border-slate-100 pt-8">
        <h3 class="text-lg font-bold text-slate-900 mb-6">{{ $t('portal.bookingDetail.itineraryTimeline') }}</h3>
        <div class="space-y-6 relative before:absolute before:inset-0 before:ml-5 before:-translate-x-px md:before:mx-auto md:before:translate-x-0 before:h-full before:w-0.5 before:bg-gradient-to-b before:from-transparent before:via-slate-200 before:to-transparent">
          <div v-for="day in 3" :key="day" class="relative flex items-center justify-between md:justify-normal md:odd:flex-row-reverse group is-active">
            <div class="flex items-center justify-center w-10 h-10 rounded-full border border-slate-200 bg-white text-[#c9a84c] shrink-0 md:order-1 md:group-odd:-translate-x-1/2 md:group-even:translate-x-1/2 shadow-sm z-10 font-bold">
              {{ day }}
            </div>
            <div class="w-[calc(100%-4rem)] md:w-[calc(50%-2.5rem)] p-4 rounded-xl border border-slate-200/80 bg-slate-50 transition-all duration-300 hover:border-[#062d4d]/20 hover:shadow-sm">
              <div class="flex items-center justify-between mb-1">
                <div class="font-bold text-[#c9a84c]">{{ $t('portal.bookingDetail.day') }} {{ day }}</div>
                <div class="text-xs text-slate-400 font-medium">Oct {{ 29 + day }}, 2026</div>
              </div>
              <div class="font-semibold text-slate-900 mb-2">Karnak & Luxor Royal Temples</div>
              <p class="text-sm text-slate-500">{{ $t('portal.bookingDetail.guidedTour') }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Portal Booking Detail View
</script>