<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold text-slate-900 mb-6">Travel Documents</h1>
    <div class="grid md:grid-cols-2 gap-4">
      <div v-for="doc in ['E-Ticket', 'Travel Insurance', 'Passport Copy']" :key="doc" class="bg-white rounded-xl border border-slate-200/80 p-5 flex items-center justify-between hover:border-[#062d4d]/20 hover:shadow-sm transition-all shadow-sm">
        <div class="flex items-center gap-4">
          <div class="w-10 h-10 rounded-lg bg-slate-50 text-[#c9a84c] border border-slate-100 flex items-center justify-center text-xl">
            📄
          </div>
          <div>
            <div class="font-bold text-slate-900">{{ doc }}</div>
            <div class="text-xs text-slate-500 font-medium">Added Oct 1, 2026</div>
          </div>
        </div>
        <button class="p-2 text-slate-400 hover:text-[#062d4d] bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
          ⬇️
        </button>
      </div>
    </div>
  </div>
</template>