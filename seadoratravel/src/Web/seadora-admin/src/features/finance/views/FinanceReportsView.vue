<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'

type Col = { key: string; label: string; money?: boolean; align?: string }
type Report = {
  key: string
  label: string
  path: string
  rows: (d: any) => any[]
  cols: Col[]
  summary?: (d: any) => { label: string; value: string }[]
  exportable?: boolean
}

const money = (v: number) =>
  new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2 }).format(v || 0)
const shortDate = (v: string) => (v ? new Date(v).toLocaleDateString() : '')

const reports: Report[] = [
  {
    key: 'trial-balance', label: 'Trial Balance', path: '/api/finance/api/reports/trial-balance', exportable: true,
    rows: d => d, cols: [
      { key: 'accountCode', label: 'Code' }, { key: 'accountName', label: 'Account' },
      { key: 'accountType', label: 'Type' },
      { key: 'totalDebit', label: 'Debit', money: true, align: 'right' },
      { key: 'totalCredit', label: 'Credit', money: true, align: 'right' },
      { key: 'balance', label: 'Balance', money: true, align: 'right' }
    ]
  },
  {
    key: 'profit-and-loss', label: 'Profit & Loss', path: '/api/finance/api/reports/profit-and-loss',
    rows: d => d.byTourType || [], cols: [
      { key: 'key', label: 'Tour Type' },
      { key: 'net', label: 'Net Revenue', money: true, align: 'right' },
      { key: 'supplierCost', label: 'Supplier Cost', money: true, align: 'right' },
      { key: 'margin', label: 'Margin', money: true, align: 'right' }
    ],
    summary: d => [
      { label: 'Gross', value: money(d.gross) }, { label: 'Discounts', value: money(d.discounts) },
      { label: 'Net', value: money(d.net) }, { label: 'Supplier Cost', value: money(d.supplierCost) },
      { label: 'Refunds', value: money(d.refunds) }, { label: 'Net Profit', value: money(d.netProfit) }
    ]
  },
  {
    key: 'revenue', label: 'Revenue', path: '/api/finance/api/reports/revenue',
    rows: d => d.series || [], cols: [
      { key: 'day', label: 'Day' }, { key: 'currency', label: 'Ccy' },
      { key: 'recognized', label: 'Recognized', money: true, align: 'right' },
      { key: 'collected', label: 'Collected', money: true, align: 'right' }
    ],
    summary: d => [
      { label: 'Total Recognized', value: money(d.totalRecognized) },
      { label: 'Total Collected', value: money(d.totalCollected) }
    ]
  },
  {
    key: 'ar-aging', label: 'AR Aging', path: '/api/finance/api/reports/ar-aging', exportable: true,
    rows: d => d.items || [], cols: [
      { key: 'bookingId', label: 'Booking' }, { key: 'currency', label: 'Ccy' },
      { key: 'due', label: 'Due', money: true, align: 'right' },
      { key: 'ageDays', label: 'Age (d)', align: 'right' }, { key: 'bucket', label: 'Bucket' }
    ],
    summary: d => [
      { label: '0-30', value: money(d.bucket0_30) }, { label: '31-60', value: money(d.bucket31_60) },
      { label: '61-90', value: money(d.bucket61_90) }, { label: '90+', value: money(d.bucket90Plus) },
      { label: 'Total', value: money(d.total) }
    ]
  },
  {
    key: 'supplier-payables', label: 'Supplier Payables', path: '/api/finance/api/reports/supplier-payables', exportable: true,
    rows: d => d, cols: [
      { key: 'supplierId', label: 'Supplier' }, { key: 'periodStart', label: 'From' },
      { key: 'periodEnd', label: 'To' },
      { key: 'accrued', label: 'Accrued', money: true, align: 'right' },
      { key: 'paid', label: 'Paid', money: true, align: 'right' },
      { key: 'due', label: 'Due', money: true, align: 'right' }, { key: 'status', label: 'Status' }
    ]
  },
  {
    key: 'receipts', label: 'Receipts', path: '/api/finance/api/reports/receipts', exportable: true,
    rows: d => d.items || [], cols: [
      { key: 'receivedUtc', label: 'Date' }, { key: 'bookingId', label: 'Booking' },
      { key: 'amount', label: 'Amount', money: true, align: 'right' },
      { key: 'currency', label: 'Ccy' }, { key: 'method', label: 'Method' }, { key: 'reference', label: 'Reference' }
    ]
  },
  {
    key: 'refunds', label: 'Refunds', path: '/api/finance/api/reports/refunds',
    rows: d => d.series || [], cols: [
      { key: 'day', label: 'Day' }, { key: 'currency', label: 'Ccy' },
      { key: 'refunds', label: 'Refunds', money: true, align: 'right' }
    ],
    summary: d => [{ label: 'Total Refunds', value: money(d.total) }]
  },
  {
    key: 'tax', label: 'Tax', path: '/api/finance/api/reports/tax',
    rows: d => d.byBranch || [], cols: [
      { key: 'branchId', label: 'Branch' }, { key: 'currency', label: 'Ccy' },
      { key: 'taxCollected', label: 'Tax Collected', money: true, align: 'right' }
    ],
    summary: d => [{ label: 'Total Tax', value: money(d.total) }]
  },
  {
    key: 'general-ledger', label: 'General Ledger', path: '/api/finance/api/reports/general-ledger', exportable: true,
    rows: d => d.items || [], cols: [
      { key: 'occurredUtc', label: 'Date' }, { key: 'accountCode', label: 'Acct' },
      { key: 'accountName', label: 'Account' }, { key: 'description', label: 'Description' },
      { key: 'debit', label: 'Debit', money: true, align: 'right' },
      { key: 'credit', label: 'Credit', money: true, align: 'right' }
    ]
  }
]

const activeKey = ref('trial-balance')
const active = computed(() => reports.find(r => r.key === activeKey.value)!)
const from = ref('')
const to = ref('')
const currency = ref('')
const loading = ref(false)
const error = ref('')
const raw = ref<any>(null)

function queryString() {
  const p = new URLSearchParams()
  if (from.value) p.append('from', from.value)
  if (to.value) p.append('to', to.value)
  if (currency.value) p.append('currency', currency.value)
  const s = p.toString()
  return s ? `?${s}` : ''
}

async function load() {
  loading.value = true
  error.value = ''
  raw.value = null
  try {
    const res = await api.get(active.value.path + queryString())
    raw.value = res.data
  } catch (e: any) {
    error.value = e?.response?.status === 403
      ? 'You do not have permission to view this report.'
      : 'Failed to load report.'
  } finally {
    loading.value = false
  }
}

function switchTab(key: string) {
  activeKey.value = key
  load()
}

const rows = computed(() => (raw.value ? active.value.rows(raw.value) : []))
const summary = computed(() => (raw.value && active.value.summary ? active.value.summary(raw.value) : []))

function cell(row: any, col: Col) {
  const v = row[col.key]
  if (col.money) return money(v)
  if (col.key.toLowerCase().includes('utc') || col.key === 'periodStart' || col.key === 'periodEnd' || col.key === 'day')
    return shortDate(v)
  if (typeof v === 'string' && v.length === 36) return v.slice(0, 8)
  return v ?? ''
}

async function exportCsv() {
  try {
    const res = await api.get(`/api/finance/api/reports/export/${active.value.key}${queryString()}`, { responseType: 'blob' })
    const url = window.URL.createObjectURL(new Blob([res.data]))
    const a = document.createElement('a')
    a.href = url
    a.download = `${active.value.key}.csv`
    a.click()
    window.URL.revokeObjectURL(url)
  } catch {
    error.value = 'Export failed (permission or server error).'
  }
}

onMounted(load)
</script>

<template>
  <div class="p-6 space-y-5">
    <div>
      <h1 class="text-2xl font-bold text-dark">Financial Reports</h1>
      <p class="text-body text-sm mt-1">Detailed, exportable accounting reports sourced from the ledger.</p>
    </div>

    <!-- Tabs -->
    <div class="flex flex-wrap gap-2 border-b border-stroke pb-3">
      <button
        v-for="r in reports" :key="r.key" @click="switchTab(r.key)"
        class="px-3 py-1.5 rounded-md text-sm font-medium transition-colors duration-200"
        :class="activeKey === r.key ? 'bg-dark text-white' : 'text-body hover:bg-slate-100'"
      >{{ r.label }}</button>
    </div>

    <!-- Filters -->
    <div class="flex flex-wrap items-end gap-3 bg-white border border-stroke rounded-lg p-4">
      <label class="flex flex-col text-xs text-body font-semibold">From
        <input type="date" v-model="from" class="mt-1 border border-stroke rounded-md px-2 py-1 text-dark" />
      </label>
      <label class="flex flex-col text-xs text-body font-semibold">To
        <input type="date" v-model="to" class="mt-1 border border-stroke rounded-md px-2 py-1 text-dark" />
      </label>
      <label class="flex flex-col text-xs text-body font-semibold">Currency
        <input type="text" v-model="currency" placeholder="USD" maxlength="3"
               class="mt-1 border border-stroke rounded-md px-2 py-1 text-dark uppercase w-20" />
      </label>
      <button @click="load"
        class="px-4 py-1.5 rounded-md bg-amber-500 text-white text-sm font-semibold hover:bg-amber-600 transition-colors">
        Apply
      </button>
      <button v-if="active.exportable" @click="exportCsv"
        class="px-4 py-1.5 rounded-md border border-stroke text-dark text-sm font-semibold hover:bg-slate-50 transition-colors">
        ⬇ Export CSV
      </button>
    </div>

    <!-- Summary cards -->
    <div v-if="summary.length" class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3">
      <div v-for="(s, i) in summary" :key="i" class="bg-white border border-stroke rounded-lg p-4">
        <p class="text-[11px] uppercase tracking-wider text-body font-semibold">{{ s.label }}</p>
        <p class="text-lg font-bold text-dark mt-1">{{ s.value }}</p>
      </div>
    </div>

    <!-- Table -->
    <div class="bg-white border border-stroke rounded-lg shadow-sm overflow-x-auto">
      <div v-if="loading" class="py-16 text-center text-body animate-pulse">Loading {{ active.label }}…</div>
      <div v-else-if="error" class="py-8 px-4 text-red-600 text-sm">{{ error }}</div>
      <table v-else class="w-full text-sm">
        <thead>
          <tr class="text-left text-body border-b border-stroke bg-slate-50/60">
            <th v-for="c in active.cols" :key="c.key" class="py-2.5 px-3 font-semibold"
                :class="c.align === 'right' ? 'text-right' : ''">{{ c.label }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(row, i) in rows" :key="i" class="border-b border-stroke/50 hover:bg-slate-50">
            <td v-for="c in active.cols" :key="c.key" class="py-2 px-3 text-dark"
                :class="c.align === 'right' ? 'text-right tabular-nums' : ''">{{ cell(row, c) }}</td>
          </tr>
          <tr v-if="!rows.length">
            <td :colspan="active.cols.length" class="py-10 text-center text-body">No records for this filter.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
