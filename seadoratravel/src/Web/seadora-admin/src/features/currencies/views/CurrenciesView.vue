<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useCurrencyStore } from '../store/currencyStore'
import { CheckCircle2, Search, ArrowRightLeft, RefreshCw, Sparkles, RotateCcw } from 'lucide-vue-next'

const store = useCurrencyStore()
const searchQuery = ref('')

onMounted(() => {
  store.fetchCurrencies()
})

const filteredCurrencies = computed(() => {
  if (!searchQuery.value) return store.currencies
  const q = searchQuery.value.toLowerCase()
  return store.currencies.filter(c => c.name.toLowerCase().includes(q) || c.code.toLowerCase().includes(q))
})

const showAddModal = ref(false)
const newCurrency = ref({
  code: '',
  name: '',
  symbol: '',
  exchangeRate: 1
})

const handleAdd = async () => {
  if (!newCurrency.value.code || !newCurrency.value.name) return
  await store.addCurrency({ ...newCurrency.value })
  showAddModal.value = false
  newCurrency.value = { code: '', name: '', symbol: '', exchangeRate: 1 }
}

const handleRateChange = (code: string, event: Event) => {
  const target = event.target as HTMLInputElement
  if (target) {
    store.updateExchangeRate(code, Number(target.value))
  }
}
</script>

<template>
  <div class="space-y-6 max-w-6xl mx-auto">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-2xl font-bold tracking-tight text-gray-900">Currencies & Exchange Rates</h1>
        <p class="text-sm text-gray-500 mt-1">Live market exchange rates with automatic sync & custom manual override control.</p>
      </div>
      <div class="flex items-center space-x-3 w-full sm:w-auto flex-wrap gap-2">
        <div class="relative w-full sm:w-56">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search currencies..."
            class="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all duration-200"
          />
        </div>

        <button 
          @click="store.syncLiveRates" 
          :disabled="store.isSyncing"
          class="inline-flex items-center gap-2 rounded-xl bg-blue-50 border border-blue-200/60 px-3.5 py-2 text-sm font-medium text-blue-700 hover:bg-blue-100 transition-all shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500/30 disabled:opacity-50"
          title="Fetch latest market rates from Free Exchange Rate API"
        >
          <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': store.isSyncing }" />
          <span>{{ store.isSyncing ? 'Syncing...' : 'Sync Live Rates' }}</span>
        </button>

        <button @click="showAddModal = true" class="inline-flex items-center justify-center rounded-xl bg-gray-900 px-4 py-2 text-sm font-medium text-white hover:bg-gray-800 transition-colors shadow-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:ring-offset-2">
          Add Currency
        </button>
      </div>
    </div>

    <!-- Currencies Table -->
    <div class="bg-white rounded-2xl border border-gray-100 shadow-[0_1px_3px_0_rgb(0,0,0,0.02)] overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left text-sm whitespace-nowrap">
          <thead class="bg-gray-50/60 border-b border-gray-100">
            <tr>
              <th class="px-6 py-4 font-medium text-gray-500">Currency</th>
              <th class="px-6 py-4 font-medium text-gray-500">Code</th>
              <th class="px-6 py-4 font-medium text-gray-500">Exchange Rate</th>
              <th class="px-6 py-4 font-medium text-gray-500">Rate Mode</th>
              <th class="px-6 py-4 font-medium text-gray-500">Status</th>
              <th class="px-6 py-4 font-medium text-gray-500 text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-50">
            <tr v-for="currency in filteredCurrencies" :key="currency.code" class="group hover:bg-gray-50/50 transition-colors duration-200">
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-full bg-gray-100 flex items-center justify-center text-gray-700 font-bold text-base shadow-inner">
                    {{ currency.symbol }}
                  </div>
                  <div>
                    <span class="font-medium text-gray-900 block">{{ currency.name }}</span>
                    <span v-if="currency.lastRateSyncAt" class="text-[11px] text-gray-400 block mt-0.5">
                      Live sync: {{ new Date(currency.lastRateSyncAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) }}
                    </span>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 text-gray-600 font-semibold tracking-wide">{{ currency.code }}</td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2">
                  <span v-if="!currency.isBase" class="text-gray-400 text-xs font-medium">1 {{ store.baseCurrency?.code }} =</span>
                  <input
                    v-if="!currency.isBase"
                    type="number"
                    step="0.0001"
                    :value="currency.exchangeRate"
                    @change="handleRateChange(currency.code, $event)"
                    class="w-28 px-3 py-1.5 text-sm bg-white border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all shadow-sm font-mono"
                  />
                  <span v-else class="inline-flex items-center gap-1.5 px-3 py-1 rounded-md bg-blue-50 text-blue-700 text-xs font-semibold tracking-wide uppercase">
                    <ArrowRightLeft class="w-3.5 h-3.5" /> 1.0000 (Base)
                  </span>
                </div>
              </td>
              <td class="px-6 py-4">
                <span v-if="currency.isBase" class="inline-flex items-center gap-1 text-xs font-medium text-blue-600">
                  <CheckCircle2 class="w-3.5 h-3.5" /> Base Currency
                </span>
                <span v-else-if="currency.isManualRate" class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full text-xs font-medium bg-amber-50 text-amber-700 border border-amber-200/60">
                  <span>✍️</span> Manual Override
                </span>
                <span v-else class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full text-xs font-medium bg-emerald-50 text-emerald-700 border border-emerald-200/60">
                  <Sparkles class="w-3.5 h-3.5" /> Live Auto Rate
                </span>
              </td>
              <td class="px-6 py-4">
                <button
                  @click="store.toggleStatus(currency.code)"
                  :disabled="currency.isBase"
                  class="relative inline-flex h-5 w-9 shrink-0 cursor-pointer items-center justify-center rounded-full focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500/50 disabled:opacity-50 disabled:cursor-not-allowed"
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
                <div class="flex items-center justify-end gap-2">
                  <button
                    v-if="!currency.isBase && currency.isManualRate && currency.liveExchangeRate"
                    @click="store.resetToLiveRate(currency.code)"
                    class="inline-flex items-center gap-1 text-xs font-medium text-blue-600 hover:text-blue-800 bg-blue-50 px-2.5 py-1 rounded-md transition-colors"
                    title="Reset to live market rate from API"
                  >
                    <RotateCcw class="w-3.5 h-3.5" /> Reset to Live
                  </button>

                  <button
                    v-if="!currency.isBase"
                    @click="store.setBaseCurrency(currency.code)"
                    class="inline-flex items-center gap-1.5 text-xs font-medium text-gray-500 hover:text-blue-600 transition-colors opacity-0 group-hover:opacity-100"
                  >
                    Set as Base
                  </button>
                  <div v-else class="inline-flex justify-end">
                    <CheckCircle2 class="w-5 h-5 text-blue-500" />
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add Currency Modal -->
    <div v-if="showAddModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-0">
      <div class="fixed inset-0 bg-gray-900/40 backdrop-blur-sm transition-opacity" @click="showAddModal = false"></div>
      <div class="relative transform overflow-hidden rounded-2xl bg-white p-6 text-left shadow-2xl transition-all sm:w-full sm:max-w-md border border-gray-200 animate-in fade-in zoom-in-95 duration-200 ease-out">
        <div class="flex items-center justify-between mb-5">
          <h3 class="text-lg font-semibold text-gray-900">Add New Currency</h3>
          <button @click="showAddModal = false" class="text-gray-400 hover:text-gray-500 transition-colors">
            <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
          </button>
        </div>
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Currency Name</label>
            <input v-model="newCurrency.name" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. Japanese Yen" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Code (ISO)</label>
            <input v-model="newCurrency.code" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. JPY" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Symbol</label>
            <input v-model="newCurrency.symbol" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. ¥" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Exchange Rate (to Base)</label>
            <input v-model.number="newCurrency.exchangeRate" type="number" step="0.0001" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" />
          </div>
        </div>
        <div class="mt-6 flex justify-end gap-3 pt-4 border-t border-gray-100">
          <button @click="showAddModal = false" class="rounded-lg px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-gray-200">Cancel</button>
          <button @click="handleAdd" :disabled="!newCurrency.code || !newCurrency.name" class="rounded-lg bg-gray-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-gray-900 focus:ring-offset-2">Add Currency</button>
        </div>
      </div>
    </div>
  </div>
</template>
