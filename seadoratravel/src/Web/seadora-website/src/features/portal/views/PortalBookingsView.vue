<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold mb-6 text-white">My Bookings</h1>
    <div class="grid gap-6">
      <div v-for="i in 2" :key="i" class="bg-[#062d4d]/40 backdrop-blur-md rounded-2xl border border-white/10 p-6 flex flex-col md:flex-row gap-6 transition-all duration-300 hover:border-[#c9a84c]/50 hover:shadow-lg hover:shadow-[#c9a84c]/10 group">
        <div class="w-full md:w-48 h-32 rounded-xl bg-[#041a2e] overflow-hidden relative">
          <img src="https://images.unsplash.com/photo-1539768942893-daf53e448371?q=80&w=400" class="w-full h-full object-cover opacity-80 group-hover:scale-105 transition-transform duration-700" alt="Luxor" />
          <div class="absolute top-2 right-2 bg-green-500/90 backdrop-blur text-white text-[10px] font-bold px-2 py-1 rounded-md tracking-wider">CONFIRMED</div>
        </div>
        <div class="flex-1 flex flex-col justify-between">
          <div>
            <div class="flex justify-between items-start mb-2">
              <h3 class="text-xl font-bold text-white group-hover:text-[#c9a84c] transition-colors">Nile Cruise Premium</h3>
              <div class="text-right">
                <div class="text-lg font-bold text-white">$2,450</div>
                <div class="text-xs text-[#c9a84c]">Balance Due: $0</div>
              </div>
            </div>
            <p class="text-white/60 text-sm mb-4">Oct 15, 2026 - Oct 22, 2026 â€¢ 2 Guests</p>
          </div>
          <div class="flex gap-3 mt-4 md:mt-0">
            <router-link to="/portal/bookings/1" class="px-4 py-2 bg-[#041a2e] border border-white/10 rounded-lg text-sm font-medium hover:border-[#c9a84c] hover:text-[#c9a84c] transition-colors">View Details</router-link>
            <button class="px-4 py-2 bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] rounded-lg text-sm font-bold shadow-lg hover:shadow-[#c9a84c]/20 transition-shadow">Download Voucher</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>