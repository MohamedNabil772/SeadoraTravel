<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold mb-6">Travel Documents</h1>
    <div class="grid md:grid-cols-2 gap-4">
      <div v-for="doc in ['E-Ticket', 'Travel Insurance', 'Passport Copy']" :key="doc" class="bg-[#062d4d]/40 backdrop-blur-md rounded-xl border border-white/10 p-5 flex items-center justify-between hover:border-[#c9a84c]/30 transition-all">
        <div class="flex items-center gap-4">
          <div class="w-10 h-10 rounded-lg bg-[#c9a84c]/10 text-[#c9a84c] flex items-center justify-center">
            ðŸ“„
          </div>
          <div>
            <div class="font-bold">{{ doc }}</div>
            <div class="text-xs text-white/50">Added Oct 1, 2026</div>
          </div>
        </div>
        <button class="p-2 text-white/50 hover:text-[#c9a84c] bg-white/5 rounded-lg hover:bg-white/10 transition-colors">
          â¬‡ï¸ 
        </button>
      </div>
    </div>
  </div>
</template>