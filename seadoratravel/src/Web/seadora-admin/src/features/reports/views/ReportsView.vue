<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue'
import api from '@/services/api'

// Tab state
const activeTab = ref('financials')

// Duration filter for Supplier Report
const supplierDuration = ref('all')

// Data states
const financialStats = ref<any>(null)
const supplierReports = ref<any[]>([])
const customerTrips = ref<any[]>([])
const ledgerData = ref<any[]>([])
const loading = ref(true)

// Ledger filters
const ledgerSearch = ref('')
const ledgerStatusFilter = ref('all')
const ledgerPage = ref(1)
const ledgerPageSize = 10

// Fetch helpers
onMounted(async () => {
  await fetchAllReports()
})

async function fetchAllReports() {
  loading.value = true
  try {
    const [finRes, custRes, ledgerRes] = await Promise.all([
      api.get('/api/booking/api/reports/dashboard'),
      api.get('/api/booking/api/reports/customers'),
      api.get('/api/booking/api/reports/ledger')
    ])
    financialStats.value = finRes.data
    customerTrips.value = custRes.data
    ledgerData.value = ledgerRes.data
    await fetchSupplierReport()
  } catch (err) {
    console.error('Error fetching reports:', err)
  } finally {
    loading.value = false
  }
}

async function fetchSupplierReport() {
  try {
    const res = await api.get(`/api/booking/api/reports/supplier?duration=${supplierDuration.value}`)
    supplierReports.value = res.data
  } catch (err) {
    console.error('Error fetching supplier report:', err)
  }
}

// Watch duration filter
watch(supplierDuration, async () => {
  await fetchSupplierReport()
})

// Computed filtered ledger list
const filteredLedger = computed(() => {
  return ledgerData.value.filter(item => {
    const searchLower = ledgerSearch.value.toLowerCase()
    const matchesSearch = 
      (item.customerName || '').toLowerCase().includes(searchLower) ||
      (item.customerEmail || '').toLowerCase().includes(searchLower) ||
      (item.tourName || '').toLowerCase().includes(searchLower) ||
      (item.supplierName || '').toLowerCase().includes(searchLower)

    const matchesStatus = ledgerStatusFilter.value === 'all' || item.status === ledgerStatusFilter.value
    return matchesSearch && matchesStatus
  })
})

const paginatedLedger = computed(() => {
  const start = (ledgerPage.value - 1) * ledgerPageSize
  return filteredLedger.value.slice(start, start + ledgerPageSize)
})

const ledgerTotalPages = computed(() => {
  return Math.ceil(filteredLedger.value.length / ledgerPageSize) || 1
})

// Reset page when filtering
watch([ledgerSearch, ledgerStatusFilter], () => {
  ledgerPage.value = 1
})

// Computed Profit & Loss Statement
const pnlStatement = computed(() => {
  if (!ledgerData.value || ledgerData.value.length === 0) {
    return {
      grossRevenue: 0,
      supplierPayouts: 0,
      netProfit: 0,
      margin: 0,
      confirmedCount: 0,
      aov: 0
    }
  }

  const confirmed = ledgerData.value.filter(item => item.status !== 'Cancelled')
  const grossRevenue = confirmed.reduce((acc, item) => acc + (item.grossRevenue || 0), 0)
  const supplierPayouts = confirmed.reduce((acc, item) => acc + (item.supplierShare || 0), 0)
  const netProfit = grossRevenue - supplierPayouts
  const margin = grossRevenue > 0 ? (netProfit / grossRevenue) * 100 : 0
  const aov = confirmed.length > 0 ? grossRevenue / confirmed.length : 0

  return {
    grossRevenue,
    supplierPayouts,
    netProfit,
    margin,
    confirmedCount: confirmed.length,
    aov
  }
})

// Max values for chart height scaling
const maxDailyRevenue = computed(() => {
  if (!financialStats.value?.daily) return 100
  const max = Math.max(...financialStats.value.daily.map((d: any) => d.revenue), 10)
  return max
})

const maxMonthlyRevenue = computed(() => {
  if (!financialStats.value?.monthly) return 100
  const max = Math.max(...financialStats.value.monthly.map((m: any) => m.revenue), 10)
  return max
})

function getDailyHeight(revenue: number): number {
  const max = maxDailyRevenue.value || 10
  return (revenue / max) * 120
}

function getMonthlyHeight(revenue: number): number {
  const max = maxMonthlyRevenue.value || 10
  return (revenue / max) * 120
}

// Format currency
function formatPrice(val: number) {
  return new Intl.NumberFormat('en-IE', { style: 'currency', currency: 'EUR' }).format(val)
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString('en-GB', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<template>
  <div class="reports-page">
    <!-- ponytail: legacy Booking-backed reports; superseded by the Finance module (double-entry ledger). Kept live for continuity. -->
    <div class="mb-4 flex items-start gap-3 rounded-lg border border-amber-300 bg-amber-50 px-4 py-3 text-sm text-amber-900">
      <span class="text-lg leading-none">ℹ️</span>
      <p class="flex-1">
        These are the legacy operational reports. Detailed accounting reports, owner dashboards and payment
        recording now live in the new
        <RouterLink to="/finance/dashboard" class="font-semibold underline hover:text-amber-700">Finance module</RouterLink>,
        backed by the double-entry ledger.
      </p>
    </div>
    <div class="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
      <div class="page-header">
        <h2>Reports & Financial Center</h2>
        <p>Monitor platform transaction earnings, supplier payouts, and guest manifests.</p>
      </div>
      <button @click="fetchAllReports" class="btn-refresh">
        🔄 Refresh Data
      </button>
    </div>

    <!-- Navigation Tabs -->
    <div class="tabs-nav">
      <button 
        @click="activeTab = 'financials'" 
        :class="['tab-btn', activeTab === 'financials' ? 'active' : '']"
      >
        💼 Financial Module
      </button>
      <button 
        @click="activeTab = 'suppliers'" 
        :class="['tab-btn', activeTab === 'suppliers' ? 'active' : '']"
      >
        🏢 Supplier Cost Analysis
      </button>
      <button 
        @click="activeTab = 'customers'" 
        :class="['tab-btn', activeTab === 'customers' ? 'active' : '']"
      >
        👥 Guest Lists per Trip
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Generating reports...</p>
    </div>

    <!-- Tab 1: Financial Module -->
    <div v-else-if="activeTab === 'financials' && financialStats" class="space-y-6">
      
      <!-- Income Statement (Structured P&L Sheet) -->
      <div class="card p-6 bg-white border border-stroke rounded-lg shadow-sm">
        <h3 class="font-bold text-dark text-lg mb-4 pb-2 border-b border-stroke">📊 Profit & Loss (P&L) Statement (Actuals)</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div>
            <table class="w-full text-sm">
              <tbody>
                <tr class="border-b border-stroke py-3 flex justify-between font-semibold text-body">
                  <span>Gross Operating Revenue (Sales)</span>
                  <span class="font-mono text-dark font-bold">{{ formatPrice(pnlStatement.grossRevenue) }}</span>
                </tr>
                <tr class="border-b border-stroke py-3 flex justify-between text-body">
                  <span>Less: Cost of Sales (Supplier Payouts)</span>
                  <span class="font-mono text-[#D34053] font-semibold">-{{ formatPrice(pnlStatement.supplierPayouts) }}</span>
                </tr>
                <tr class="border-b border-stroke py-3 flex justify-between font-bold text-base text-dark bg-meta-4/5 px-2 rounded mt-2">
                  <span>Net Operating Income (Platform Profit)</span>
                  <span class="font-mono text-[#10B981]">{{ formatPrice(pnlStatement.netProfit) }}</span>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="flex flex-col justify-between bg-[#F8FAFC] p-4 rounded-lg border border-stroke">
            <div class="grid grid-cols-2 gap-4 text-center">
              <div>
                <span class="text-xs text-body font-semibold block mb-1">Operating Profit Margin</span>
                <span class="text-2xl font-bold text-[#10B981] font-mono">{{ pnlStatement.margin.toFixed(1) }}%</span>
              </div>
              <div>
                <span class="text-xs text-body font-semibold block mb-1">Average Order Value (AOV)</span>
                <span class="text-2xl font-bold text-primary font-mono">{{ formatPrice(pnlStatement.aov) }}</span>
              </div>
              <div class="col-span-2 border-t border-stroke pt-2 mt-2">
                <span class="text-xs text-body block font-semibold">Total Revenue-Generating Bookings</span>
                <span class="text-xl font-bold text-dark font-mono">{{ pnlStatement.confirmedCount }} Confirmed Trips</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Cards Grid -->
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-icon">🎟️</div>
          <div class="stat-info">
            <div class="stat-num">{{ financialStats.totalBookings }}</div>
            <div class="stat-label">Total Bookings</div>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon bg-[rgba(60,80,224,0.08)] text-primary">💰</div>
          <div class="stat-info">
            <div class="stat-num text-primary">{{ formatPrice(financialStats.totalRevenue) }}</div>
            <div class="stat-label">Gross Sales Revenue</div>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon bg-[rgba(211,64,83,0.08)] text-[#D34053]">🏢</div>
          <div class="stat-info">
            <div class="stat-num text-[#D34053]">{{ formatPrice(financialStats.totalSupplierCost) }}</div>
            <div class="stat-label">Supplier Share Cost</div>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon bg-[rgba(16,185,129,0.08)] text-[#10B981]">📈</div>
          <div class="stat-info">
            <div class="stat-num text-[#10B981]">{{ formatPrice(financialStats.totalPlatformEarnings) }}</div>
            <div class="stat-label">Net Platform Earnings</div>
          </div>
        </div>
      </div>

      <!-- Historical Performance Charts -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Daily Earnings SVG Chart -->
        <div class="card p-6 bg-white border border-stroke rounded-lg shadow-sm">
          <h4 class="font-bold text-dark text-sm mb-2">📅 Daily Performance (Last 7 Days)</h4>
          <span class="text-xs text-body block mb-4">Legend: <span class="inline-block w-2.5 h-2.5 bg-[#3C50E0] rounded-sm mr-1"></span> Gross Sales | <span class="inline-block w-2.5 h-2.5 bg-[#10B981] rounded-sm mr-1"></span> Net Earnings</span>
          
          <div class="relative">
            <svg class="w-full h-48" viewBox="0 0 500 180">
              <!-- Grid lines -->
              <line x1="40" y1="20" x2="480" y2="20" stroke="#F1F5F9" stroke-dasharray="4 4" />
              <line x1="40" y1="70" x2="480" y2="70" stroke="#F1F5F9" stroke-dasharray="4 4" />
              <line x1="40" y1="120" x2="480" y2="120" stroke="#F1F5F9" stroke-dasharray="4 4" />
              <line x1="40" y1="150" x2="480" y2="150" stroke="#E2E8F0" />
              
              <!-- Bars -->
              <g v-for="(d, idx) in financialStats.daily" :key="d.date">
                <!-- Revenue Bar (Blue) -->
                <rect 
                  :x="55 + Number(idx) * 60" 
                  :y="150 - getDailyHeight(d.revenue)" 
                  width="16" 
                  :height="getDailyHeight(d.revenue)" 
                  rx="3"
                  fill="#3C50E0" 
                  class="transition-all duration-300 hover:opacity-85"
                >
                  <title>Revenue: {{ formatPrice(d.revenue) }}</title>
                </rect>
                <!-- Net Profit Bar (Green) -->
                <rect 
                  :x="73 + Number(idx) * 60" 
                  :y="150 - getDailyHeight(d.earnings)" 
                  width="16" 
                  :height="getDailyHeight(d.earnings)" 
                  rx="3"
                  fill="#10B981" 
                  class="transition-all duration-300 hover:opacity-85"
                >
                  <title>Net Profit: {{ formatPrice(d.earnings) }}</title>
                </rect>
                <!-- Date Label -->
                <text :x="72 + Number(idx) * 60" y="168" text-anchor="middle" class="text-[9px] font-semibold fill-body font-mono">
                  {{ d.date.split('-').slice(1).join('/') }}
                </text>
              </g>
            </svg>
          </div>
        </div>

        <!-- Monthly Earnings SVG Chart -->
        <div class="card p-6 bg-white border border-stroke rounded-lg shadow-sm">
          <h4 class="font-bold text-dark text-sm mb-2">📈 Monthly Trend (Last 6 Months)</h4>
          <span class="text-xs text-body block mb-4">Legend: <span class="inline-block w-2.5 h-2.5 bg-[#3C50E0] rounded-sm mr-1"></span> Gross Sales | <span class="inline-block w-2.5 h-2.5 bg-[#10B981] rounded-sm mr-1"></span> Net Earnings</span>

          <div class="relative">
            <svg class="w-full h-48" viewBox="0 0 500 180">
              <!-- Grid lines -->
              <line x1="40" y1="20" x2="480" y2="20" stroke="#F1F5F9" stroke-dasharray="4 4" />
              <line x1="40" y1="70" x2="480" y2="70" stroke="#F1F5F9" stroke-dasharray="4 4" />
              <line x1="40" y1="120" x2="480" y2="120" stroke="#F1F5F9" stroke-dasharray="4 4" />
              <line x1="40" y1="150" x2="480" y2="150" stroke="#E2E8F0" />
              
              <!-- Bars -->
              <g v-for="(m, idx) in financialStats.monthly" :key="m.month">
                <!-- Revenue Bar (Blue) -->
                <rect 
                  :x="50 + Number(idx) * 72" 
                  :y="150 - getMonthlyHeight(m.revenue)" 
                  width="20" 
                  :height="getMonthlyHeight(m.revenue)" 
                  rx="3"
                  fill="#3C50E0" 
                  class="transition-all duration-300 hover:opacity-85"
                >
                  <title>Revenue: {{ formatPrice(m.revenue) }}</title>
                </rect>
                <!-- Net Profit Bar (Green) -->
                <rect 
                  :x="72 + Number(idx) * 72" 
                  :y="150 - getMonthlyHeight(m.earnings)" 
                  width="20" 
                  :height="getMonthlyHeight(m.earnings)" 
                  rx="3"
                  fill="#10B981" 
                  class="transition-all duration-300 hover:opacity-85"
                >
                  <title>Net Profit: {{ formatPrice(m.earnings) }}</title>
                </rect>
                <!-- Month Label -->
                <text :x="71 + Number(idx) * 72" y="168" text-anchor="middle" class="text-[9px] font-semibold fill-body font-mono">
                  {{ m.month.split(' ')[0] }}
                </text>
              </g>
            </svg>
          </div>
        </div>
      </div>

      <!-- Transaction Ledger Section -->
      <div class="table-container">
        <div class="p-6 border-b border-stroke bg-[#F7F9FC] flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h3 class="font-bold text-dark text-lg">📁 General Transaction Ledger</h3>
            <p class="text-xs text-body mt-0.5">Audit trail of all excursion ticket sales, supplier costs, and net platform earnings.</p>
          </div>
          <div class="flex flex-wrap items-center gap-3">
            <input 
              v-model="ledgerSearch" 
              type="text" 
              aria-label="Search ledger"
              placeholder="Search ledger..." 
              class="px-3 py-1.5 border border-stroke rounded text-xs bg-white text-dark outline-none focus:border-primary w-48"
            />
            <select 
              v-model="ledgerStatusFilter" 
              aria-label="Filter by booking status"
              class="px-3 py-1.5 border border-stroke rounded text-xs bg-white text-dark outline-none cursor-pointer"
            >
              <option value="all">All Statuses</option>
              <option value="Confirmed">Confirmed</option>
              <option value="Completed">Completed</option>
              <option value="Pending">Pending</option>
              <option value="Cancelled">Cancelled</option>
            </select>
          </div>
        </div>

        <table class="data-table">
          <thead>
            <tr>
              <th>Tx Date</th>
              <th>Customer details</th>
              <th>Excursion Tour</th>
              <th>Supplier partner</th>
              <th>Gross Revenue</th>
              <th>Supplier Share</th>
              <th>Platform Profit</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="tx in paginatedLedger" :key="tx.bookingId" :class="{ 'bg-meta-1/5': tx.status === 'Cancelled' }">
              <td class="font-mono text-xs text-body">{{ formatDate(tx.bookingDate) }}</td>
              <td>
                <div class="font-semibold text-dark">{{ tx.customerName }}</div>
                <div class="text-[11px] text-body">{{ tx.customerEmail }}</div>
              </td>
              <td class="font-medium text-dark">{{ tx.tourName }}</td>
              <td class="font-medium text-dark">{{ tx.supplierName }}</td>
              <td class="font-bold text-dark font-mono">{{ formatPrice(tx.status === 'Cancelled' ? 0 : tx.grossRevenue) }}</td>
              <td class="font-bold text-[#D34053] font-mono">{{ formatPrice(tx.status === 'Cancelled' ? 0 : tx.supplierShare) }}</td>
              <td class="font-bold text-[#10B981] font-mono">{{ formatPrice(tx.status === 'Cancelled' ? 0 : tx.platformProfit) }}</td>
              <td>
                <span :class="['status-badge', tx.status.toLowerCase()]">
                  {{ tx.status }}
                </span>
              </td>
            </tr>
            <tr v-if="filteredLedger.length === 0">
              <td colspan="8" class="empty-state">
                No ledger transactions found matching the filter criteria.
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Pagination Controls -->
        <div v-if="ledgerTotalPages > 1" class="p-4 border-t border-stroke flex justify-between items-center bg-[#F7F9FC]">
          <span class="text-xs text-body font-semibold">
            Showing Page {{ ledgerPage }} of {{ ledgerTotalPages }}
          </span>
          <div class="flex gap-2">
            <button 
              @click="ledgerPage = Math.max(1, ledgerPage - 1)" 
              :disabled="ledgerPage === 1"
              class="px-3 py-1 border border-stroke rounded text-xs bg-white text-dark hover:bg-stroke disabled:opacity-50 transition-all cursor-pointer font-semibold"
            >
              Previous
            </button>
            <button 
              @click="ledgerPage = Math.min(ledgerTotalPages, ledgerPage + 1)" 
              :disabled="ledgerPage === ledgerTotalPages"
              class="px-3 py-1 border border-stroke rounded text-xs bg-white text-dark hover:bg-stroke disabled:opacity-50 transition-all cursor-pointer font-semibold"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Tab 2: Supplier Cost Analysis -->
    <div v-else-if="activeTab === 'suppliers'" class="space-y-6">
      <div class="filter-header-block">
        <div>
          <h3>Supplier Revenue Share Statement</h3>
          <p>Configure duration below to filter payouts based on trip date ranges.</p>
        </div>
        <div class="flex-filter">
          <label for="reports-supplier-duration">Duration:</label>
          <select id="reports-supplier-duration" v-model="supplierDuration" class="filter-select">
            <option value="all">All-Time Bookings</option>
            <option value="day">Last 24 Hours</option>
            <option value="week">Last 7 Days</option>
            <option value="month">Last 30 Days</option>
          </select>
        </div>
      </div>

      <div class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>English Name</th>
              <th>Arabic Name</th>
              <th>Payment Cycle</th>
              <th>Tours Booked</th>
              <th>Gross Revenue</th>
              <th>Supplier Share</th>
              <th>Platform Profit</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="sup in supplierReports" :key="sup.supplierId">
              <td class="font-semibold text-black">{{ sup.nameEn }}</td>
              <td class="font-semibold text-black rtl text-right">{{ sup.nameAr }}</td>
              <td>
                <span class="cycle-badge">
                  {{ sup.agreement }}
                </span>
              </td>
              <td class="font-mono font-bold text-black">{{ sup.bookingCount }}</td>
              <td class="font-bold text-black font-mono">{{ formatPrice(sup.totalRevenue) }}</td>
              <td class="font-bold text-[#D34053] font-mono">{{ formatPrice(sup.totalCost) }}</td>
              <td class="font-bold text-[#10B981] font-mono">{{ formatPrice(sup.totalRevenue - sup.totalCost) }}</td>
            </tr>
            <tr v-if="supplierReports.length === 0">
              <td colspan="7" class="empty-state">
                No active bookings found for the selected duration.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Tab 3: Customer Guest Lists -->
    <div v-else-if="activeTab === 'customers'" class="space-y-6">
      <div class="filter-header-block">
        <div>
          <h3>Customer Passenger Manifest per Excursion</h3>
          <p>Listing all available packages, total guest counts, and booked traveler directories.</p>
        </div>
      </div>

      <div class="space-y-4">
        <div v-for="trip in customerTrips" :key="trip.tourId" class="table-container">
          <div class="trip-manifest-header">
            <div>
              <span class="cycle-badge mb-1.5 inline-block">
                {{ trip.supplierName }}
              </span>
              <h4 class="font-bold text-black text-lg">{{ trip.tourNameEn }}</h4>
            </div>
            <div class="trip-manifest-info">
              <div class="text-right">
                <span class="label">Unit Price</span>
                <span class="val text-black">{{ formatPrice(trip.price) }}</span>
              </div>
              <div class="text-right">
                <span class="label">Gross Sales</span>
                <span class="val text-primary">{{ formatPrice(trip.price * trip.bookingCount) }}</span>
              </div>
              <div class="badge-count">
                {{ trip.bookingCount }} Passengers
              </div>
            </div>
          </div>

          <div class="p-6">
            <div v-if="trip.bookings.length > 0" class="overflow-x-auto">
              <table class="manifest-table">
                <thead>
                  <tr>
                    <th>Passenger Name</th>
                    <th>Passenger Email</th>
                    <th>Booking Date</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="b in trip.bookings" :key="b.id">
                    <td class="font-semibold text-black">{{ b.customerName }}</td>
                    <td class="font-mono text-xs text-body">{{ formatDate(b.bookingDate) }}</td>
                    <td class="text-body font-medium">{{ formatDate(b.bookingDate) }}</td>
                    <td>
                      <span :class="['status-badge', b.status.toLowerCase()]">
                        {{ b.status }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div v-else class="empty-manifest">
              No passengers have reserved this excursion yet.
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<style scoped>
.reports-page { color: #24303F; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #1C2434; margin-bottom: 4px; }
.page-header p { color: #64748B; font-size: 14px; }

.btn-refresh { padding: 10px 22px; background: #fff; border: 1px solid #E2E8F0; border-radius: 4px; color: #64748B; font-weight: 600; cursor: pointer; font-size: 14px; transition: all 0.2s; box-shadow: 0px 1px 2px rgba(0, 0, 0, 0.05); }
.btn-refresh:hover { background: #F7F9FC; color: #1C2434; }

.tabs-nav { display: flex; gap: 8px; border-bottom: 1px solid #E2E8F0; margin-bottom: 24px; }
.tab-btn { padding: 12px 20px; font-weight: 600; font-size: 14px; color: #64748B; background: none; border: none; border-bottom: 2px solid transparent; cursor: pointer; transition: all 0.2s; }
.tab-btn:hover { color: #3C50E0; }
.tab-btn.active { color: #3C50E0; border-bottom-color: #3C50E0; }

/* Stats grid */
.stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 20px; margin-bottom: 28px; }
.stat-card {
  background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07);
  padding: 24px; display: flex; align-items: center; gap: 20px;
}
.stat-icon { font-size: 28px; background: rgba(60, 80, 224, 0.08); width: 55px; height: 55px; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
.stat-num { font-size: 24px; font-weight: 700; color: #1C2434; }
.stat-label { font-size: 13px; color: #64748B; margin-top: 2px; }

.text-gold { color: #c9a84c; }
.text-rose { color: #ff6b6b; }
.text-emerald { color: #4caf78; }

.grid-columns-3 { display: grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 20px; margin-bottom: 28px; }
.performance-card { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; padding: 20px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); }
.performance-card h4 { font-size: 15px; font-weight: 700; color: #1C2434; margin-bottom: 16px; border-bottom: 1px solid #E2E8F0; padding-bottom: 12px; }
.performance-item { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #F1F5F9; padding-bottom: 8px; }

/* Tables */
.table-container { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; overflow: hidden; margin-bottom: 28px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); }
.table-header-block { padding: 20px; border-bottom: 1px solid #E2E8F0; background: #F7F9FC; }
.table-header-block h3 { font-size: 16px; font-weight: 700; color: #1C2434; }

.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 16px 24px; text-align: left; font-size: 12px; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748B; background: #F7F9FC; border-bottom: 1px solid #E2E8F0; }
.data-table td { padding: 16px 24px; border-bottom: 1px solid #E2E8F0; font-size: 14px; color: #24303F; }
.data-table tr:hover { background: #F9FAFB; }

.cycle-badge { padding: 4px 10px; background: rgba(60, 80, 224, 0.08); color: #3C50E0; border-radius: 4px; font-size: 12px; font-weight: 600; display: inline-block; }

.status-badge { padding: 4px 10px; border-radius: 20px; font-size: 10px; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; display: inline-block; }
.status-badge.pending { background: rgba(232,130,10,0.1); color: #e8820a; border: 1px solid rgba(232,130,10,0.2); }
.status-badge.confirmed { background: rgba(60,80,224,0.1); color: #3C50E0; border: 1px solid rgba(60,80,224,0.2); }
.status-badge.completed { background: rgba(16,185,129,0.1); color: #10B981; border: 1px solid rgba(16,185,129,0.2); }
.status-badge.cancelled { background: rgba(211,64,83,0.1); color: #D34053; border: 1px solid rgba(211,64,83,0.2); }

/* Filter header */
.filter-header-block { display: flex; justify-content: space-between; align-items: center; background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; padding: 20px; margin-bottom: 24px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); }
.filter-header-block h3 { font-size: 16px; font-weight: 700; color: #1C2434; }
.filter-header-block p { font-size: 13px; color: #64748B; margin-top: 2px; }
.flex-filter { display: flex; align-items: center; gap: 8px; }
.flex-filter label { font-size: 12px; font-weight: 700; text-transform: uppercase; color: #64748B; }
.filter-select { padding: 8px 12px; background: #FFFFFF; border: 1.5px solid #E2E8F0; border-radius: 4px; color: #24303F; font-size: 13px; outline: none; cursor: pointer; }
.filter-select:focus { border-color: #3C50E0; }

/* Manifests list */
.trip-manifest-header { padding: 20px; border-bottom: 1px solid #E2E8F0; background: #F7F9FC; display: flex; justify-content: space-between; align-items: center; }
.trip-manifest-info { display: flex; align-items: center; gap: 24px; }
.trip-manifest-info .label { display: block; font-size: 10px; color: #64748B; text-transform: uppercase; font-weight: 700; }
.trip-manifest-info .val { display: block; font-weight: 700; font-size: 14px; font-family: monospace; }
.badge-count { background: #3C50E0; color: #fff; font-size: 11px; font-weight: 700; padding: 6px 12px; border-radius: 4px; }

.manifest-table { width: 100%; border-collapse: collapse; }
.manifest-table th { padding: 10px 0; border-bottom: 1px solid #E2E8F0; text-align: left; font-size: 11px; text-transform: uppercase; color: #64748B; }
.manifest-table td { padding: 12px 0; border-bottom: 1px solid #F1F5F9; font-size: 13px; color: #24303F; }
.empty-manifest { text-align: center; padding: 24px; font-size: 13px; color: #64748B; font-style: italic; }

.loading { text-align: center; padding: 60px; color: #64748B; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(60, 80, 224, 0.1); border-top-color: #3C50E0; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state { text-align: center; padding: 48px; color: #64748B; }
</style>
