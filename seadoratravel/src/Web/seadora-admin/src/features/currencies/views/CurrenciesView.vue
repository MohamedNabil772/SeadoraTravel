<script setup lang="ts">
import { ref, computed } from 'vue'
import { useCurrencyStore } from '../store/currencyStore'
import { CheckCircle2, Search, ArrowRightLeft } from 'lucide-vue-next'

const store = useCurrencyStore()
const searchQuery = ref('')

const filteredCurrencies = computed(() => {
  if (!searchQuery.value) return store.currencies
  const q = searchQuery.value.toLowerCase()
  return store.currencies.filter(c => c.name.toLowerCase().includes(q) || c.code.toLowerCase().includes(q))
})
</script>

<template>
  <div class="space-y-6 max-w-6xl mx-auto">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-2xl font-bold tracking-tight text-gray-900">Currencies</h1>
        <p class="text-sm text-gray-500 mt-1">Manage supported currencies and exchange rates.</p>
      </div>
      <div class="flex items-center space-x-3 w-full sm:w-auto">
        <div class="relative w-full sm:w-64">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search currencies..."
            class="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all duration-200"
          />
        </div>
      </div>
    </div>

    <div class="bg-white rounded-2xl border border-gray-100 shadow-[0_1px_3px_0_rgb(0,0,0,0.02)] overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left text-sm whitespace-nowrap">
          <thead class="bg-gray-50/50 border-b border-gray-100">
            <tr>
              <th class="px-6 py-4 font-medium text-gray-500">Currency</th>
              <th class="px-6 py-4 font-medium text-gray-500">Code</th>
              <th class="px-6 py-4 font-medium text-gray-500">Exchange Rate</th>
              <th class="px-6 py-4 font-medium text-gray-500">Status</th>
              <th class="px-6 py-4 font-medium text-gray-500 text-right">Base Currency</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-50">
            <tr v-for="currency in filteredCurrencies" :key="currency.code" class="group hover:bg-gray-50/50 transition-colors duration-200">
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-full bg-gray-100 flex items-center justify-center text-gray-600 font-medium">
                    {{ currency.symbol }}
                  </div>
                  <span class="font-medium text-gray-900">{{ currency.name }}</span>
                </div>
              </td>
              <td class="px-6 py-4 text-gray-600 font-medium">{{ currency.code }}</td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2">
                  <span v-if="!currency.isBase" class="text-gray-400 font-medium">1 {{ store.baseCurrency?.code }} =</span>
                  <input
                    v-if="!currency.isBase"
                    type="number"
                    step="0.0001"
                    :value="currency.exchangeRate"
                    @change="(e) => store.updateExchangeRate(currency.code, Number((e.target as HTMLInputElement).value))"
                    class="w-24 px-3 py-1.5 text-sm bg-white border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all shadow-sm"
                  />
                  <span v-else class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-blue-50 text-blue-700 text-xs font-semibold tracking-wide uppercase">
                    <ArrowRightLeft class="w-3.5 h-3.5" /> Base
                  </span>
                </div>
              </td>
              <td class="px-6 py-4">
                <button
                  @click="store.toggleStatus(currency.code)"
                  class="relative inline-flex h-5 w-9 shrink-0 cursor-pointer items-center justify-center rounded-full focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500/50"
                  role="switch"
                  :aria-checked="currency.isActive"
                >
                  <div class="absolute inset-0 rounded-full transition-colors duration-300 ease-in-out" :class="currency.isActive ? 'bg-blue-500' : 'bg-gray-200'" />
                  <div
                    class="absolute left-0 inline-block h-4 w-4 transform rounded-full bg-white shadow-sm ring-0 transition-transform duration-500 ease-[cubic-bezier(0.34,1.56,0.64,1)]"
                    :class="currency.isActive ? 'translate-x-[18px]' : 'translate-x-[2px]'"
                  />
                </button>
              </td>
              <td class="px-6 py-4 text-right">
                <button
                  v-if="!currency.isBase"
                  @click="store.setBaseCurrency(currency.code)"
                  class="inline-flex items-center gap-1.5 text-xs font-medium text-gray-500 hover:text-blue-600 transition-colors opacity-0 group-hover:opacity-100"
                >
                  Set as Base
                </button>
                <div v-else class="inline-flex justify-end w-full">
                  <CheckCircle2 class="w-5 h-5 text-blue-500" />
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
