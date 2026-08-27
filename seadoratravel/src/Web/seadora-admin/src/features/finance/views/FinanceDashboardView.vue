<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'

const loading = ref(true)
const error = ref('')
const data = ref<any>(null)
const granularity = ref<'day' | 'week' | 'month' | 'quarter'>('month')

async function load() {
  loading.value = true
  error.value = ''
  try {
    const res = await api.get(`/api/finance/api/dashboard?granularity=${granularity.value}`)
    data.value = res.data
  } catch (e: any) {
    error.value = e?.response?.status === 403
      ? 'You do not have permission to view the finance dashboard.'
      : 'Failed to load dashboard.'
  } finally {
    loading.value = false
  }
}

onMounted(load)

const money = (v: number) =>
  new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 }).format(v || 0)
const pct = (v: number) => `${(v ?? 0).toFixed(1)}%`

const kpis = computed(() => data.value?.kpis)
const trend = computed<any[]>(() => data.value?.trend || [])
const maxTrend = computed(() => Math.max(1, ...trend.value.map(t => Math.max(t.recognized, t.collected))))
const growth = computed(() => data.value?.growth)
</script>

<template>
  <div class="p-6 space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-dark">Financial Overview</h1>
        <p class="text-body text-sm mt-1">Revenue, profit and cash position across all branches.</p>
      </div>
      <div class="flex items-center gap-2">
        <select
          v-model="granularity"
          @change="load"
          class="border border-stroke rounded-lg px-3 py-2 text-sm text-dark focus:outline-none focus:ring-2 focus:ring-amber-400/40"
        >
          <option value="day">Daily</option>
          <option value="week">Weekly</option>
          <option value="month">Monthly</option>
          <option value="quarter">Quarterly</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="text-body py-20 text-center animate-pulse">Loading financials…</div>
    <div v-else-if="error" class="bg-red-50 border border-red-200 text-red-700 rounded-lg p-4">{{ error }}</div>

    <template v-else-if="kpis">
      <!-- KPI cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-5 hover:shadow-md transition-shadow duration-300">
          <p class="text-[11px] uppercase tracking-wider text-body font-semibold">Revenue Recognized</p>
          <p class="text-2xl font-bold text-dark mt-2">{{ money(kpis.revenueRecognized) }}</p>
          <p class="text-xs mt-1" :class="growth?.revenueMoMPct >= 0 ? 'text-emerald-600' : 'text-red-600'">
            {{ growth?.revenueMoMPct >= 0 ? '▲' : '▼' }} {{ pct(Math.abs(growth?.revenueMoMPct || 0)) }} vs prev
          </p>
        </div>
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-5 hover:shadow-md transition-shadow duration-300">
          <p class="text-[11px] uppercase tracking-wider text-body font-semibold">Cash Collected</p>
          <p class="text-2xl font-bold text-dark mt-2">{{ money(kpis.revenueCollected) }}</p>
          <p class="text-xs text-body mt-1">of {{ money(kpis.revenueRecognized) }} recognized</p>
        </div>
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-5 hover:shadow-md transition-shadow duration-300">
          <p class="text-[11px] uppercase tracking-wider text-body font-semibold">Net Profit</p>
          <p class="text-2xl font-bold text-dark mt-2">{{ money(kpis.netProfit) }}</p>
          <p class="text-xs text-body mt-1">Gross margin {{ pct(kpis.grossMarginPct) }}</p>
        </div>
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-5 hover:shadow-md transition-shadow duration-300">
          <p class="text-[11px] uppercase tracking-wider text-body font-semibold">Outstanding AR</p>
          <p class="text-2xl font-bold text-dark mt-2">{{ money(kpis.outstandingAr) }}</p>
          <p class="text-xs text-body mt-1">{{ kpis.bookings }} active bookings</p>
        </div>
      </div>

      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-4">
          <p class="text-[11px] uppercase tracking-wider text-body font-semibold">Supplier Cost</p>
          <p class="text-lg font-bold text-dark mt-1">{{ money(kpis.supplierCost) }}</p>
        </div>
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-4">
          <p class="text-[11px] uppercase tracking-wider text-body font-semibold">Refunds</p>
          <p class="text-lg font-bold text-dark mt-1">{{ money(kpis.refunds) }}</p>
        </div>
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-4">
          <p class="text-[11px] uppercase tracking-wider text-body font-semibold">Avg Booking</p>
          <p class="text-lg font-bold text-dark mt-1">{{ money(kpis.averageBookingValue) }}</p>
        </div>
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-4">
          <p class="text-[11px] uppercase tracking-wider text-body font-semibold">Cancel / Refund Rate</p>
          <p class="text-lg font-bold text-dark mt-1">{{ pct(kpis.cancellationRatePct) }} / {{ pct(kpis.refundRatePct) }}</p>
        </div>
      </div>

      <!-- Trend chart (recognized vs collected) -->
      <div class="bg-white border border-stroke rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="font-bold text-dark">Revenue Trend</h3>
          <div class="flex items-center gap-4 text-xs text-body">
            <span class="flex items-center gap-1"><span class="w-3 h-3 rounded-sm bg-amber-500 inline-block"></span> Recognized</span>
            <span class="flex items-center gap-1"><span class="w-3 h-3 rounded-sm bg-slate-400 inline-block"></span> Collected</span>
          </div>
        </div>
        <div v-if="trend.length" class="flex items-end gap-2 h-56">
          <div v-for="(t, i) in trend" :key="i" class="flex-1 flex flex-col items-center justify-end gap-1 group">
            <div class="w-full flex items-end justify-center gap-1 h-full">
              <div
                class="w-1/2 bg-amber-500 rounded-t transition-all duration-500 group-hover:bg-amber-600"
                :style="{ height: `${(t.recognized / maxTrend) * 100}%` }"
                :title="`Recognized ${money(t.recognized)}`"
              ></div>
              <div
                class="w-1/2 bg-slate-400 rounded-t transition-all duration-500 group-hover:bg-slate-500"
                :style="{ height: `${(t.collected / maxTrend) * 100}%` }"
                :title="`Collected ${money(t.collected)}`"
              ></div>
            </div>
            <span class="text-[10px] text-body truncate w-full text-center">{{ t.period }}</span>
          </div>
        </div>
        <p v-else class="text-body text-sm py-10 text-center">No revenue in this period yet.</p>
      </div>

      <!-- Top tour-types + branch split -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-6">
          <h3 class="font-bold text-dark mb-4">Top Tour Types</h3>
          <table class="w-full text-sm">
            <thead>
              <tr class="text-left text-body border-b border-stroke">
                <th class="py-2 font-semibold">Type</th>
                <th class="py-2 font-semibold text-right">Revenue</th>
                <th class="py-2 font-semibold text-right">Margin</th>
                <th class="py-2 font-semibold text-right">Bookings</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(r, i) in data.topTourTypes" :key="i" class="border-b border-stroke/60 hover:bg-slate-50">
                <td class="py-2 text-dark font-medium">{{ r.key }}</td>
                <td class="py-2 text-right text-dark">{{ money(r.revenue) }}</td>
                <td class="py-2 text-right text-emerald-600">{{ money(r.margin) }}</td>
                <td class="py-2 text-right text-body">{{ r.bookings }}</td>
              </tr>
              <tr v-if="!data.topTourTypes?.length"><td colspan="4" class="py-6 text-center text-body">No data.</td></tr>
            </tbody>
          </table>
        </div>
        <div class="bg-white border border-stroke rounded-lg shadow-sm p-6">
          <h3 class="font-bold text-dark mb-4">Per-Branch Split</h3>
          <table class="w-full text-sm">
            <thead>
              <tr class="text-left text-body border-b border-stroke">
                <th class="py-2 font-semibold">Branch</th>
                <th class="py-2 font-semibold text-right">Revenue</th>
                <th class="py-2 font-semibold text-right">Supplier Cost</th>
                <th class="py-2 font-semibold text-right">Margin</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(r, i) in data.byBranch" :key="i" class="border-b border-stroke/60 hover:bg-slate-50">
                <td class="py-2 text-dark font-mono text-xs">{{ r.branchId.slice(0, 8) }}</td>
                <td class="py-2 text-right text-dark">{{ money(r.revenue) }}</td>
                <td class="py-2 text-right text-body">{{ money(r.supplierCost) }}</td>
                <td class="py-2 text-right text-emerald-600">{{ money(r.margin) }}</td>
              </tr>
              <tr v-if="!data.byBranch?.length"><td colspan="4" class="py-6 text-center text-body">No data.</td></tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>
  </div>
</template>
