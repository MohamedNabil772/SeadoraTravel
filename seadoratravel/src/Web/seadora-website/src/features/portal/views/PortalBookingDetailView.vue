<template>
  <div class="space-y-6">
    <router-link to="/portal/bookings" class="text-sm text-white/50 hover:text-[#c9a84c] flex items-center gap-2 mb-4">
      â†  Back to Bookings
    </router-link>
    <div class="bg-[#062d4d]/40 backdrop-blur-md rounded-2xl border border-white/10 p-6">
      <h1 class="text-2xl font-bold mb-2">Nile Cruise Premium</h1>
      <p class="text-[#c9a84c]">Booking #SD-2026-8943</p>
      
      <div class="mt-8 border-t border-white/10 pt-8">
        <h3 class="text-lg font-bold mb-6">Itinerary Timeline</h3>
        <div class="space-y-6 relative before:absolute before:inset-0 before:ml-5 before:-translate-x-px md:before:mx-auto md:before:translate-x-0 before:h-full before:w-0.5 before:bg-gradient-to-b before:from-transparent before:via-white/20 before:to-transparent">
          <div v-for="day in 3" :key="day" class="relative flex items-center justify-between md:justify-normal md:odd:flex-row-reverse group is-active">
            <div class="flex items-center justify-center w-10 h-10 rounded-full border border-white/20 bg-[#041a2e] text-[#c9a84c] shrink-0 md:order-1 md:group-odd:-translate-x-1/2 md:group-even:translate-x-1/2 shadow-lg z-10 font-bold">
              {{ day }}
            </div>
            <div class="w-[calc(100%-4rem)] md:w-[calc(50%-2.5rem)] p-4 rounded-xl border border-white/10 bg-[#062d4d]/50 backdrop-blur transition-all duration-300 hover:border-[#c9a84c]/30">
              <div class="flex items-center justify-between mb-1">
                <div class="font-bold text-[#c9a84c]">Day {{ day }}</div>
                <div class="text-xs text-white/50">Oct {{ 14 + day }}, 2026</div>
              </div>
              <div class="font-semibold mb-2">Luxor Temple Visit</div>
              <p class="text-sm text-white/70">Guided tour through the ancient temples.</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>