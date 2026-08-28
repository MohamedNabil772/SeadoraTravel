<template>
  <div class="space-y-6">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
      <h1 class="text-2xl font-bold text-slate-900 mb-0">My Bookings</h1>
      <router-link to="/tours" class="px-5 py-2.5 bg-[#062d4d] text-white rounded-xl font-bold shadow-sm hover:bg-[#062d4d]/90 transition-colors flex items-center gap-2">
        <span>✨</span> Book a New Experience
      </router-link>
    </div>
    <div class="grid gap-6">
      <div v-for="i in 2" :key="i" class="bg-white rounded-2xl border border-slate-200/80 p-6 flex flex-col md:flex-row gap-6 transition-all duration-300 hover:shadow-md group">
        <div class="w-full md:w-48 h-32 rounded-xl bg-slate-100 overflow-hidden relative">
          <img src="https://images.unsplash.com/photo-1539768942893-daf53e448371?q=80&w=400" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700" alt="Luxor" />
          <div class="absolute top-2 right-2 bg-green-100 text-green-700 border border-green-200 text-[10px] font-bold px-2 py-1 rounded-md tracking-wider">CONFIRMED</div>
        </div>
        <div class="flex-1 flex flex-col justify-between">
          <div>
            <div class="flex justify-between items-start mb-2">
              <h3 class="text-xl font-bold text-slate-900 group-hover:text-[#062d4d] transition-colors">Nile Cruise Premium</h3>
              <div class="text-right">
                <div class="text-lg font-bold text-slate-900">$2,450</div>
                <div class="text-xs text-green-600 font-medium">Balance Due: $0</div>
              </div>
            </div>
            <p class="text-slate-500 text-sm mb-4">Oct 15, 2026 - Oct 22, 2026 • 2 Guests</p>
          </div>
          <div class="flex gap-3 mt-4 md:mt-0">
            <router-link to="/portal/bookings/1" class="px-4 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm font-medium hover:border-[#062d4d] hover:text-[#062d4d] transition-colors text-slate-700">View Details</router-link>
            <button class="px-4 py-2 bg-secondary/10 text-secondary-text border border-secondary/20 rounded-lg text-sm font-bold shadow-sm hover:bg-secondary/20 transition-colors">Download Voucher</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>