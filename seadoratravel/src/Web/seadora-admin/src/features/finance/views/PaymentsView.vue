<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import api from '@/services/api'
import { toast } from 'vue-sonner'

interface Booking {
  id: string
  tourId: string
  customerName: string
  customerEmail: string
  bookingDate: string
  status: string
  whatsApp?: string
  hotelName?: string
  roomNumber?: string
  totalPrice?: number
  price?: number
  currency?: string
}

interface Tour {
  id: string
  title?: string
  names?: Record<string, string>
  price?: number
}

interface CurrencyItem {
  id: string
  code: string
  name: string
  symbol: string
  exchangeRate: number
  liveExchangeRate?: number
  isBase: boolean
  isActive: boolean
}

interface Payment {
  id: string
  bookingId: string
  amount: number
  currency: string
  exchangeRate?: number
  settledAmount?: number
  method: string
  reference?: string
  receivedUtc: string
  createdBy?: string
}

// Configurable Payment Methods
const availablePaymentMethods = ref<string[]>([
  'Credit / Debit Card',
  'Bank Wire Transfer',
  'Cash (EUR)',
  'Cash (USD)',
  'Cash (EGP)',
  'InstaPay (Egypt)',
  'Vodafone Cash / Smart Wallet',
  'Stripe Online Checkout',
  'POS Machine Terminal',
  'PayPal',
  'Crypto / USDT',
  'Other / Custom'
])

// State
const bookings = ref<Booking[]>([])
const tours = ref<Tour[]>([])
const currencies = ref<CurrencyItem[]>([])
const paymentsMap = ref<Record<string, Payment[]>>({})
const loading = ref(true)
const loadingPayments = ref(false)
const recordingPayment = ref(false)
const selectedBookingId = ref<string>('')

// Filter & Search states
const searchQuery = ref('')
const selectedTourFilter = ref<string>('ALL')
const selectedStatusFilter = ref<string>('ALL')
const datePreset = ref<string>('ALL')
const customDateStart = ref<string>('')
const customDateEnd = ref<string>('')

// Payment Recording Form
const form = ref({
  amount: '', // Amount paid in Customer Currency
  currency: 'EUR', // Selected Payment Currency
  exchangeRate: 1.0, // Conversion Rate against Booking Currency (e.g. 53.50 EGP/EUR)
  settledAmount: 0, // Calculated amount deducted from Booking (in Booking Currency)
  method: 'Credit / Debit Card',
  customMethod: '',
  reference: '',
  receivedUtc: new Date().toISOString().slice(0, 16)
})

// Currency Formatter
const formatMoney = (val: number, currencyCode: string = 'EUR') => {
  try {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currencyCode || 'EUR',
      minimumFractionDigits: 2
    }).format(val || 0)
  } catch {
    return `${(val || 0).toFixed(2)} ${currencyCode}`
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}

const formatDateTime = (dateStr: string) => {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// Load Initial Data
async function loadInitialData() {
  loading.value = true
  try {
    const [bookingsRes, toursRes, currenciesRes] = await Promise.all([
      api.get('/api/booking/api/bookings'),
      api.get('/api/content/api/tours'),
      api.get('/api/content/api/currencies').catch(() => ({ data: [] }))
    ])
    
    const rawBookings = bookingsRes.data
    bookings.value = Array.isArray(rawBookings) ? rawBookings : (rawBookings?.items || [])
    tours.value = Array.isArray(toursRes.data) ? toursRes.data : []
    
    if (Array.isArray(currenciesRes.data) && currenciesRes.data.length > 0) {
      currencies.value = currenciesRes.data
    } else {
      currencies.value = [
        { id: '1', code: 'EUR', name: 'Euro', symbol: '€', exchangeRate: 1.0, isBase: true, isActive: true },
        { id: '2', code: 'USD', name: 'US Dollar', symbol: '$', exchangeRate: 1.08, isBase: false, isActive: true },
        { id: '3', code: 'GBP', name: 'British Pound', symbol: '£', exchangeRate: 0.85, isBase: false, isActive: true },
        { id: '4', code: 'EGP', name: 'Egyptian Pound', symbol: 'E£', exchangeRate: 53.50, isBase: false, isActive: true },
        { id: '5', code: 'AED', name: 'UAE Dirham', symbol: 'د.إ', exchangeRate: 3.97, isBase: false, isActive: true },
        { id: '6', code: 'SAR', name: 'Saudi Riyal', symbol: '﷼', exchangeRate: 4.05, isBase: false, isActive: true }
      ]
    }

    if (bookings.value.length > 0 && !selectedBookingId.value) {
      selectBooking(bookings.value[0].id)
    }
  } catch (err) {
    console.error('Failed to load payments workspace data', err)
    toast.error('Failed to load bookings data.')
  } finally {
    loading.value = false
  }
}

// Load Payments for a specific booking
async function loadBookingPayments(bId: string) {
  if (!bId) return
  loadingPayments.value = true
  try {
    const res = await api.get(`/api/finance/api/payments/booking/${bId}`)
    paymentsMap.value[bId] = Array.isArray(res.data) ? res.data : []
  } catch (err: any) {
    if (err?.response?.status === 404) {
      paymentsMap.value[bId] = []
    } else {
      console.warn('Could not load payment ledger for booking', bId, err)
    }
  } finally {
    loadingPayments.value = false
  }
}

function selectBooking(id: string) {
  selectedBookingId.value = id
  if (id && !paymentsMap.value[id]) {
    loadBookingPayments(id)
  }
  
  const b = selectedBooking.value
  if (b) {
    const bookingCurr = b.currency || 'EUR'
    form.value.currency = bookingCurr
    updateRateForCurrency(bookingCurr)
    
    const remaining = getBookingRemaining(b)
    recalculateFromBaseDue(remaining)
    form.value.reference = ''
    form.value.receivedUtc = new Date().toISOString().slice(0, 16)
  }
}

// Calculate rate between payment currency and booking base currency
function updateRateForCurrency(targetCurrency: string) {
  const b = selectedBooking.value
  const bookingCurr = (b?.currency || 'EUR').toUpperCase()
  const payCurr = targetCurrency.toUpperCase()

  if (payCurr === bookingCurr) {
    form.value.exchangeRate = 1.0
    return
  }

  // Find currencies in rate store
  const targetObj = currencies.value.find(c => c.code.toUpperCase() === payCurr)
  const baseObj = currencies.value.find(c => c.code.toUpperCase() === bookingCurr)

  const targetRate = targetObj?.exchangeRate || (payCurr === 'EGP' ? 53.50 : payCurr === 'USD' ? 1.08 : 1.0)
  const baseRate = baseObj?.exchangeRate || 1.0

  // Relative rate: how many target units per 1 base unit
  const calculatedRate = targetRate / baseRate
  form.value.exchangeRate = parseFloat(calculatedRate.toFixed(4))
}

// Watch payment currency changes
watch(() => form.value.currency, (newCurr) => {
  updateRateForCurrency(newCurr)
  const b = selectedBooking.value
  if (b) {
    const remaining = getBookingRemaining(b)
    recalculateFromBaseDue(remaining)
  }
})

// Watch rate adjustments
watch(() => form.value.exchangeRate, () => {
  recalculateSettledAmount()
})

// Recalculate customer paid amount when given a target base settled amount
function recalculateFromBaseDue(baseAmount: number) {
  const rate = form.value.exchangeRate || 1.0
  const custAmount = baseAmount * rate
  form.value.amount = custAmount > 0 ? custAmount.toFixed(2) : ''
  form.value.settledAmount = parseFloat(baseAmount.toFixed(2))
}

// Recalculate settled amount from user input
function onCustomerAmountInput() {
  recalculateSettledAmount()
}

function recalculateSettledAmount() {
  const raw = parseFloat(form.value.amount) || 0
  const rate = form.value.exchangeRate || 1.0
  form.value.settledAmount = parseFloat((raw / rate).toFixed(2))
}

// Helpers
function getTourName(tourId: string) {
  const t = tours.value.find(item => item.id === tourId)
  return t ? (t.names?.en || t.title || 'Luxury Tour') : 'Luxury Tour'
}

function getBookingTotal(b: Booking): number {
  return b.totalPrice ?? b.price ?? 0
}

function getBookingPaid(bId: string): number {
  const pList = paymentsMap.value[bId] || []
  return pList.reduce((sum, p) => sum + (p.settledAmount ?? p.amount ?? 0), 0)
}

function getBookingRemaining(b: Booking): number {
  const total = getBookingTotal(b)
  const paid = getBookingPaid(b.id)
  return Math.max(0, total - paid)
}

function getPaymentStatus(b: Booking): 'PAID' | 'PARTIAL' | 'UNPAID' {
  const total = getBookingTotal(b)
  const paid = getBookingPaid(b.id)
  if (paid >= total && total > 0) return 'PAID'
  if (paid > 0 && paid < total) return 'PARTIAL'
  return 'UNPAID'
}

// Selected Booking Computed
const selectedBooking = computed(() => {
  return bookings.value.find(b => b.id === selectedBookingId.value) || null
})

const selectedPayments = computed(() => {
  if (!selectedBookingId.value) return []
  return paymentsMap.value[selectedBookingId.value] || []
})

// Filtered Bookings Computed
const filteredBookings = computed(() => {
  let list = [...bookings.value]

  // Search Filter
  if (searchQuery.value.trim()) {
    const q = searchQuery.value.trim().toLowerCase()
    list = list.filter(b => 
      b.id.toLowerCase().includes(q) ||
      (b.customerName && b.customerName.toLowerCase().includes(q)) ||
      (b.customerEmail && b.customerEmail.toLowerCase().includes(q)) ||
      (b.whatsApp && b.whatsApp.toLowerCase().includes(q)) ||
      (b.hotelName && b.hotelName.toLowerCase().includes(q)) ||
      getTourName(b.tourId).toLowerCase().includes(q)
    )
  }

  // Tour Filter
  if (selectedTourFilter.value !== 'ALL') {
    list = list.filter(b => b.tourId === selectedTourFilter.value)
  }

  // Date Filter
  if (datePreset.value !== 'ALL') {
    const now = new Date()
    list = list.filter(b => {
      if (!b.bookingDate) return false
      const d = new Date(b.bookingDate)
      if (datePreset.value === 'TODAY') {
        return d.toDateString() === now.toDateString()
      }
      if (datePreset.value === 'THIS_WEEK') {
        const startOfWeek = new Date(now)
        startOfWeek.setDate(now.getDate() - now.getDay())
        startOfWeek.setHours(0, 0, 0, 0)
        return d >= startOfWeek
      }
      if (datePreset.value === 'THIS_MONTH') {
        return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear()
      }
      if (datePreset.value === 'CUSTOM') {
        const start = customDateStart.value ? new Date(customDateStart.value) : null
        const end = customDateEnd.value ? new Date(customDateEnd.value) : null
        if (start && d < start) return false
        if (end && d > end) return false
        return true
      }
      return true
    })
  }

  // Payment Status Filter
  if (selectedStatusFilter.value !== 'ALL') {
    list = list.filter(b => getPaymentStatus(b) === selectedStatusFilter.value)
  }

  return list
})

// Metrics
const totalBookingsCount = computed(() => bookings.value.length)
const totalVolumeAmount = computed(() => bookings.value.reduce((sum, b) => sum + getBookingTotal(b), 0))
const totalCollectedAmount = computed(() => {
  return Object.values(paymentsMap.value).flat().reduce((sum, p) => sum + (p.settledAmount ?? p.amount ?? 0), 0)
})
const totalUnpaidAmount = computed(() => Math.max(0, totalVolumeAmount.value - totalCollectedAmount.value))

// Quick Fill Payment Helpers
function setQuickAmount(ratio: number) {
  if (!selectedBooking.value) return
  const remaining = getBookingRemaining(selectedBooking.value)
  const total = getBookingTotal(selectedBooking.value)
  const targetBase = ratio === 1 ? remaining : total * ratio
  recalculateFromBaseDue(Math.min(targetBase, remaining > 0 ? remaining : total))
}

// Record Payment Handler
async function handleRecordPayment() {
  if (!selectedBooking.value) {
    toast.error('Please select a booking first.')
    return
  }
  const amount = parseFloat(form.value.amount)
  if (!amount || amount <= 0) {
    toast.error('Please enter a valid payment amount.')
    return
  }

  const finalMethod = form.value.method === 'Other / Custom' && form.value.customMethod.trim() 
    ? form.value.customMethod.trim() 
    : form.value.method

  recordingPayment.value = true
  try {
    await api.post(`/api/finance/api/payments/booking/${selectedBooking.value.id}`, {
      amount,
      currency: form.value.currency,
      exchangeRate: form.value.exchangeRate,
      settledAmount: form.value.settledAmount,
      method: finalMethod,
      reference: form.value.reference?.trim() || `${finalMethod.toUpperCase()}-${Date.now().toString().slice(-6)}`,
      receivedUtc: form.value.receivedUtc ? new Date(form.value.receivedUtc).toISOString() : new Date().toISOString()
    })

    toast.success(`Payment of ${formatMoney(amount, form.value.currency)} (Settling ${formatMoney(form.value.settledAmount, selectedBooking.value.currency || 'EUR')}) recorded successfully!`)
    
    // Refresh payments for this booking
    await loadBookingPayments(selectedBooking.value.id)
    
    // Reset amount
    const remaining = getBookingRemaining(selectedBooking.value)
    recalculateFromBaseDue(remaining)
    form.value.reference = ''
    form.value.customMethod = ''
  } catch (err: any) {
    console.error('Failed to record payment', err)
    const msg = err?.response?.data?.message || err?.response?.data?.detail || 'Failed to record payment.'
    toast.error(msg)
  } finally {
    recordingPayment.value = false
  }
}

function copyToClipboard(text: string) {
  navigator.clipboard.writeText(text)
  toast.success('Copied to clipboard!')
}

onMounted(loadInitialData)
</script>

<template>
  <div class="payments-workspace flex flex-col gap-6">
    
    <!-- Top Header & Financial KPI Overview -->
    <div class="bg-white rounded-2xl border border-slate-200/80 p-6 shadow-xs flex flex-col lg:flex-row justify-between items-start lg:items-center gap-6">
      <div>
        <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-emerald-500/10 border border-emerald-500/20 text-emerald-700 text-xs font-bold uppercase tracking-wider mb-2">
          <span>💳</span> Multi-Currency Financial Ledger & Settlement
        </div>
        <h1 class="text-2xl md:text-3xl font-bold text-slate-900 tracking-tight">Payments & Settlement</h1>
        <p class="text-xs md:text-sm text-slate-500 mt-1">Accept payments in any customer currency with live exchange conversion and flexible settlement methods.</p>
      </div>

      <!-- Financial Metrics Ribbon -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 w-full lg:w-auto">
        <div class="bg-slate-50 border border-slate-100 rounded-xl p-3.5 min-w-[120px]">
          <span class="text-[10px] uppercase font-bold text-slate-400 block tracking-wider">Total Bookings</span>
          <span class="text-lg font-black text-slate-900 mt-0.5 block">{{ totalBookingsCount }}</span>
        </div>
        <div class="bg-slate-50 border border-slate-100 rounded-xl p-3.5 min-w-[120px]">
          <span class="text-[10px] uppercase font-bold text-slate-400 block tracking-wider">Gross Volume</span>
          <span class="text-lg font-black text-slate-900 mt-0.5 block">{{ formatMoney(totalVolumeAmount) }}</span>
        </div>
        <div class="bg-emerald-50 border border-emerald-100 rounded-xl p-3.5 min-w-[120px]">
          <span class="text-[10px] uppercase font-bold text-emerald-600 block tracking-wider">Collected</span>
          <span class="text-lg font-black text-emerald-700 mt-0.5 block">{{ formatMoney(totalCollectedAmount) }}</span>
        </div>
        <div class="bg-amber-50 border border-amber-100 rounded-xl p-3.5 min-w-[120px]">
          <span class="text-[10px] uppercase font-bold text-amber-600 block tracking-wider">Outstanding</span>
          <span class="text-lg font-black text-amber-700 mt-0.5 block">{{ formatMoney(totalUnpaidAmount) }}</span>
        </div>
      </div>
    </div>

    <!-- Main Workspace Split Layout (5 Cols List / 7 Cols Detail & Ledger) -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 items-start">
      
      <!-- LEFT COLUMN: Search, Filters & Bookings Master List (5 cols) -->
      <div class="lg:col-span-5 space-y-4">
        
        <!-- Search & Filter Controls Card -->
        <div class="bg-white rounded-2xl border border-slate-200/80 p-4 shadow-xs space-y-3">
          <!-- Search Bar -->
          <div class="relative">
            <span class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-slate-400">
              🔍
            </span>
            <input 
              v-model="searchQuery" 
              type="text" 
              placeholder="Search by customer, email, booking ref, hotel..." 
              class="w-full pl-9 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-medium text-slate-900 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-[#062d4d] focus:bg-white transition-all"
            />
            <button 
              v-if="searchQuery" 
              @click="searchQuery = ''"
              class="absolute inset-y-0 right-0 pr-3 flex items-center text-slate-400 hover:text-slate-600 text-xs"
            >
              ✕
            </button>
          </div>

          <!-- Secondary Filters Row -->
          <div class="grid grid-cols-2 gap-2">
            <!-- Tour Filter -->
            <select 
              v-model="selectedTourFilter" 
              class="w-full px-2.5 py-1.5 bg-slate-50 border border-slate-200 rounded-lg text-[11px] font-semibold text-slate-700 focus:outline-none focus:ring-1 focus:ring-[#062d4d]"
            >
              <option value="ALL">All Tours & Journeys</option>
              <option v-for="t in tours" :key="t.id" :value="t.id">
                {{ t.names?.en || t.title || 'Tour' }}
              </option>
            </select>

            <!-- Date Preset Filter -->
            <select 
              v-model="datePreset" 
              class="w-full px-2.5 py-1.5 bg-slate-50 border border-slate-200 rounded-lg text-[11px] font-semibold text-slate-700 focus:outline-none focus:ring-1 focus:ring-[#062d4d]"
            >
              <option value="ALL">All Dates</option>
              <option value="TODAY">Today's Bookings</option>
              <option value="THIS_WEEK">This Week</option>
              <option value="THIS_MONTH">This Month</option>
              <option value="CUSTOM">Custom Date Range</option>
            </select>
          </div>

          <!-- Custom Date Range Picker (Conditional) -->
          <div v-if="datePreset === 'CUSTOM'" class="grid grid-cols-2 gap-2 pt-1">
            <input 
              v-model="customDateStart" 
              type="date" 
              class="w-full px-2 py-1 bg-slate-50 border border-slate-200 rounded-lg text-[10px] text-slate-700" 
            />
            <input 
              v-model="customDateEnd" 
              type="date" 
              class="w-full px-2 py-1 bg-slate-50 border border-slate-200 rounded-lg text-[10px] text-slate-700" 
            />
          </div>

          <!-- Payment Status Tabs -->
          <div class="flex items-center gap-1.5 pt-1 border-t border-slate-100 overflow-x-auto no-scrollbar">
            <button 
              @click="selectedStatusFilter = 'ALL'"
              :class="selectedStatusFilter === 'ALL' ? 'bg-[#062d4d] text-white shadow-xs' : 'bg-slate-100 text-slate-600 hover:bg-slate-200'"
              class="px-2.5 py-1 rounded-lg text-[10px] font-bold uppercase tracking-wider whitespace-nowrap transition-all"
            >
              All ({{ bookings.length }})
            </button>
            <button 
              @click="selectedStatusFilter = 'UNPAID'"
              :class="selectedStatusFilter === 'UNPAID' ? 'bg-amber-500 text-white shadow-xs' : 'bg-amber-50 text-amber-700 hover:bg-amber-100'"
              class="px-2.5 py-1 rounded-lg text-[10px] font-bold uppercase tracking-wider whitespace-nowrap transition-all"
            >
              Unpaid
            </button>
            <button 
              @click="selectedStatusFilter = 'PARTIAL'"
              :class="selectedStatusFilter === 'PARTIAL' ? 'bg-blue-600 text-white shadow-xs' : 'bg-blue-50 text-blue-700 hover:bg-blue-100'"
              class="px-2.5 py-1 rounded-lg text-[10px] font-bold uppercase tracking-wider whitespace-nowrap transition-all"
            >
              Partial
            </button>
            <button 
              @click="selectedStatusFilter = 'PAID'"
              :class="selectedStatusFilter === 'PAID' ? 'bg-emerald-600 text-white shadow-xs' : 'bg-emerald-50 text-emerald-700 hover:bg-emerald-100'"
              class="px-2.5 py-1 rounded-lg text-[10px] font-bold uppercase tracking-wider whitespace-nowrap transition-all"
            >
              Settled
            </button>
          </div>
        </div>

        <!-- Bookings Master List -->
        <div class="space-y-2.5 max-h-[700px] overflow-y-auto pr-1">
          <!-- Loading Skeletons -->
          <div v-if="loading" class="space-y-3">
            <div v-for="i in 4" :key="i" class="bg-white p-4 rounded-xl border border-slate-200/60 animate-pulse space-y-2">
              <div class="h-3 bg-slate-200 rounded w-1/3"></div>
              <div class="h-4 bg-slate-200 rounded w-2/3"></div>
              <div class="h-3 bg-slate-200 rounded w-1/2"></div>
            </div>
          </div>

          <!-- Empty Search State -->
          <div v-else-if="filteredBookings.length === 0" class="bg-white rounded-2xl border border-slate-200/80 p-8 text-center">
            <span class="text-3xl block mb-2">🔍</span>
            <h4 class="text-sm font-bold text-slate-800">No Bookings Found</h4>
            <p class="text-xs text-slate-400 mt-1">Try adjusting your filters or search keywords.</p>
            <button 
              @click="searchQuery = ''; selectedTourFilter = 'ALL'; selectedStatusFilter = 'ALL'; datePreset = 'ALL'"
              class="mt-3 text-xs font-bold text-[#062d4d] hover:underline"
            >
              Clear All Filters
            </button>
          </div>

          <!-- Booking Interactive Card -->
          <div 
            v-for="b in filteredBookings" 
            :key="b.id"
            @click="selectBooking(b.id)"
            :class="[
              selectedBookingId === b.id 
                ? 'border-[#062d4d] bg-sky-50/40 ring-2 ring-[#062d4d]/10 shadow-md translate-x-1' 
                : 'border-slate-200/80 bg-white hover:border-slate-300 hover:shadow-xs'
            ]"
            class="p-4 rounded-2xl border transition-all duration-200 cursor-pointer flex flex-col gap-2.5 active:scale-[0.99]"
          >
            <div class="flex items-start justify-between gap-2">
              <div>
                <div class="flex items-center gap-1.5">
                  <span class="font-bold text-xs text-slate-900">{{ b.customerName || 'VIP Guest' }}</span>
                  <span class="text-[10px] text-slate-400 font-mono">#{{ b.id.slice(0, 8) }}</span>
                </div>
                <div class="text-[11px] text-slate-500 truncate max-w-[200px]">{{ b.customerEmail }}</div>
              </div>

              <!-- Settlement Status Badge -->
              <span 
                v-if="getPaymentStatus(b) === 'PAID'" 
                class="px-2 py-0.5 rounded-md bg-emerald-100 text-emerald-800 text-[10px] font-extrabold uppercase tracking-wider"
              >
                Settled ✓
              </span>
              <span 
                v-else-if="getPaymentStatus(b) === 'PARTIAL'" 
                class="px-2 py-0.5 rounded-md bg-blue-100 text-blue-800 text-[10px] font-extrabold uppercase tracking-wider"
              >
                Partial ({{ Math.round((getBookingPaid(b.id) / (getBookingTotal(b) || 1)) * 100) }}%)
              </span>
              <span 
                v-else 
                class="px-2 py-0.5 rounded-md bg-amber-100 text-amber-800 text-[10px] font-extrabold uppercase tracking-wider"
              >
                Unpaid
              </span>
            </div>

            <div class="pt-2 border-t border-slate-100 flex items-center justify-between text-xs">
              <div class="flex items-center gap-1 text-slate-600 truncate max-w-[180px]">
                <span>⛵</span>
                <span class="truncate font-medium">{{ getTourName(b.tourId) }}</span>
              </div>
              <div class="text-right">
                <span class="font-bold text-slate-900">{{ formatMoney(getBookingTotal(b), b.currency || 'EUR') }}</span>
                <div v-if="getBookingRemaining(b) > 0" class="text-[10px] text-amber-600 font-semibold">
                  Due: {{ formatMoney(getBookingRemaining(b), b.currency || 'EUR') }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- RIGHT COLUMN: Linked Booking Financial Dossier & Payment Ledger (7 cols) -->
      <div class="lg:col-span-7 space-y-6">
        
        <!-- Empty Selection State -->
        <div v-if="!selectedBooking" class="bg-white rounded-2xl border border-slate-200/80 p-12 text-center shadow-xs">
          <div class="w-16 h-16 bg-slate-100 rounded-full flex items-center justify-center text-3xl mx-auto mb-3">
            📋
          </div>
          <h3 class="text-base font-bold text-slate-900">No Booking Selected</h3>
          <p class="text-xs text-slate-500 max-w-sm mx-auto mt-1">Select a booking from the left list to review its financial snapshot, record multi-currency transactions, and print receipts.</p>
        </div>

        <!-- Linked Booking Details Card -->
        <div v-else class="space-y-6">
          
          <!-- Dossier Banner Header -->
          <div class="bg-white rounded-2xl border border-slate-200/80 p-6 shadow-xs space-y-5">
            <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
              <div>
                <div class="flex items-center gap-2">
                  <span class="text-xs font-bold text-[#062d4d] uppercase tracking-wider bg-sky-50 px-2.5 py-0.5 rounded-lg border border-sky-100">
                    Linked Financial Dossier
                  </span>
                  <button 
                    @click="copyToClipboard(selectedBooking.id)"
                    class="text-[11px] text-slate-400 hover:text-slate-600 font-mono flex items-center gap-1"
                    title="Click to copy full GUID"
                  >
                    <span>ID: {{ selectedBooking.id }}</span>
                    <span>📋</span>
                  </button>
                </div>
                <h2 class="text-xl font-bold text-slate-900 mt-2">{{ selectedBooking.customerName }}</h2>
                <div class="flex flex-wrap items-center gap-3 text-xs text-slate-500 mt-1">
                  <span>✉️ {{ selectedBooking.customerEmail }}</span>
                  <span v-if="selectedBooking.whatsApp">📱 {{ selectedBooking.whatsApp }}</span>
                  <span v-if="selectedBooking.hotelName">🏨 {{ selectedBooking.hotelName }} (Rm {{ selectedBooking.roomNumber || '—' }})</span>
                </div>
              </div>

              <!-- Direct Tour Link Badge -->
              <div class="text-left sm:text-right bg-slate-50 border border-slate-100 rounded-xl p-3">
                <span class="text-[10px] uppercase font-bold text-slate-400 block tracking-wider">Experience</span>
                <span class="text-xs font-bold text-slate-900 block max-w-[200px] truncate">{{ getTourName(selectedBooking.tourId) }}</span>
                <span class="text-[11px] text-slate-500 block mt-0.5">📅 {{ formatDate(selectedBooking.bookingDate) }}</span>
              </div>
            </div>

            <!-- Financial Settlement Progress Bar -->
            <div class="pt-4 border-t border-slate-100 space-y-2">
              <div class="flex justify-between items-baseline text-xs">
                <div class="space-x-2">
                  <span class="text-slate-500">Booking Total: <strong class="text-slate-900 font-bold">{{ formatMoney(getBookingTotal(selectedBooking), selectedBooking.currency || 'EUR') }}</strong></span>
                  <span class="text-slate-300">|</span>
                  <span class="text-emerald-700 font-medium">Paid: <strong>{{ formatMoney(getBookingPaid(selectedBooking.id), selectedBooking.currency || 'EUR') }}</strong></span>
                </div>
                <div>
                  <span class="text-xs font-bold text-amber-700">
                    Remaining Due: {{ formatMoney(getBookingRemaining(selectedBooking), selectedBooking.currency || 'EUR') }}
                  </span>
                </div>
              </div>

              <!-- Visual Progress Bar -->
              <div class="w-full h-3 bg-slate-100 rounded-full overflow-hidden flex">
                <div 
                  class="bg-gradient-to-r from-emerald-500 to-teal-500 transition-all duration-500 rounded-full"
                  :style="{ width: `${Math.min(100, (getBookingPaid(selectedBooking.id) / (getBookingTotal(selectedBooking) || 1)) * 100)}%` }"
                ></div>
              </div>
            </div>
          </div>

          <!-- Two-Card Action Grid: Inline Multi-Currency Payment Recording & Transaction Ledger -->
          <div class="grid grid-cols-1 md:grid-cols-12 gap-6">
            
            <!-- RECORD PAYMENT FORM (5 cols) -->
            <div class="md:col-span-5 bg-white rounded-2xl border border-slate-200/80 p-5 shadow-xs flex flex-col justify-between space-y-4">
              <div>
                <div class="flex items-center justify-between mb-3">
                  <h3 class="font-bold text-sm text-slate-900 flex items-center gap-1.5">
                    <span>💵</span> Record Multi-Currency Payment
                  </h3>
                  <span class="text-[10px] text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded font-bold uppercase">Dynamic FX</span>
                </div>

                <!-- Quick-Fill Chips -->
                <div class="flex items-center gap-1.5 mb-3 flex-wrap">
                  <button 
                    @click="setQuickAmount(1)" 
                    class="px-2 py-1 bg-emerald-50 hover:bg-emerald-100 text-emerald-800 text-[10px] font-bold rounded-lg transition-all"
                  >
                    Full Balance
                  </button>
                  <button 
                    @click="setQuickAmount(0.5)" 
                    class="px-2 py-1 bg-sky-50 hover:bg-sky-100 text-sky-800 text-[10px] font-bold rounded-lg transition-all"
                  >
                    50% Deposit
                  </button>
                  <button 
                    @click="setQuickAmount(0.25)" 
                    class="px-2 py-1 bg-slate-100 hover:bg-slate-200 text-slate-700 text-[10px] font-bold rounded-lg transition-all"
                  >
                    25% Deposit
                  </button>
                </div>

                <div class="space-y-3">
                  
                  <!-- Currency Selector & Exchange Rate -->
                  <div class="grid grid-cols-2 gap-2">
                    <div>
                      <label class="block text-[10px] font-bold uppercase tracking-wider text-slate-600 mb-1">
                        Payment Currency
                      </label>
                      <select 
                        v-model="form.currency" 
                        class="w-full px-2.5 py-1.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-bold text-slate-900 focus:outline-none focus:ring-2 focus:ring-[#062d4d]"
                      >
                        <option v-for="c in currencies" :key="c.code" :value="c.code">
                          {{ c.code }} ({{ c.symbol }}) - {{ c.name }}
                        </option>
                      </select>
                    </div>

                    <div>
                      <label class="block text-[10px] font-bold uppercase tracking-wider text-slate-600 mb-1">
                        Exchange Rate (vs {{ selectedBooking.currency || 'EUR' }})
                      </label>
                      <input 
                        v-model.number="form.exchangeRate" 
                        type="number" 
                        step="0.0001" 
                        min="0.0001"
                        class="w-full px-2.5 py-1.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-slate-900 focus:outline-none focus:ring-2 focus:ring-[#062d4d]"
                      />
                    </div>
                  </div>

                  <!-- Amount Input in Customer Currency -->
                  <div>
                    <label class="block text-[10px] font-bold uppercase tracking-wider text-slate-600 mb-1">
                      Customer Pays Amount ({{ form.currency }})
                    </label>
                    <div class="relative">
                      <span class="absolute inset-y-0 left-0 pl-3 flex items-center text-slate-400 text-xs font-bold">
                        {{ form.currency }}
                      </span>
                      <input 
                        v-model="form.amount" 
                        @input="onCustomerAmountInput"
                        type="number" 
                        min="0.01" 
                        step="0.01" 
                        placeholder="0.00" 
                        class="w-full pl-12 pr-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm font-bold text-slate-900 focus:outline-none focus:ring-2 focus:ring-[#062d4d] focus:bg-white"
                      />
                    </div>
                  </div>

                  <!-- Multi-Currency Settlement Live Calculation Box -->
                  <div class="p-2.5 bg-sky-50/80 border border-sky-200/60 rounded-xl text-xs space-y-1">
                    <div class="flex items-center justify-between text-slate-600">
                      <span>Rate conversion:</span>
                      <span class="font-mono font-bold text-slate-900">
                        1 {{ selectedBooking.currency || 'EUR' }} = {{ form.exchangeRate }} {{ form.currency }}
                      </span>
                    </div>
                    <div class="flex items-center justify-between text-[#062d4d] font-bold pt-1 border-t border-sky-200/40">
                      <span>Settles in Booking:</span>
                      <span class="text-sm font-black text-emerald-700">
                        {{ formatMoney(form.settledAmount, selectedBooking.currency || 'EUR') }}
                      </span>
                    </div>
                  </div>

                  <!-- Configurable Payment Method -->
                  <div>
                    <label class="block text-[10px] font-bold uppercase tracking-wider text-slate-600 mb-1">
                      Payment Method / Channel
                    </label>
                    <select 
                      v-model="form.method" 
                      class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#062d4d]"
                    >
                      <option v-for="m in availablePaymentMethods" :key="m" :value="m">
                        {{ m }}
                      </option>
                    </select>
                  </div>

                  <!-- Custom Payment Method Name Input (Conditional) -->
                  <div v-if="form.method === 'Other / Custom'">
                    <label class="block text-[10px] font-bold uppercase tracking-wider text-slate-600 mb-1">
                      Custom Method Name
                    </label>
                    <input 
                      v-model="form.customMethod" 
                      type="text" 
                      placeholder="e.g. Corporate Cheque, VIP Voucher #12..." 
                      class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#062d4d] focus:bg-white"
                    />
                  </div>

                  <!-- Reference / Wire ID -->
                  <div>
                    <label class="block text-[10px] font-bold uppercase tracking-wider text-slate-600 mb-1">
                      Reference / Wire ID / TXN
                    </label>
                    <input 
                      v-model="form.reference" 
                      type="text" 
                      placeholder="e.g. TXN-998822, Wire ref, Instapay ref..." 
                      class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#062d4d] focus:bg-white"
                    />
                  </div>

                  <!-- Received Date -->
                  <div>
                    <label class="block text-[10px] font-bold uppercase tracking-wider text-slate-600 mb-1">
                      Received Date & Time
                    </label>
                    <input 
                      v-model="form.receivedUtc" 
                      type="datetime-local" 
                      class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#062d4d] focus:bg-white"
                    />
                  </div>
                </div>
              </div>

              <!-- Submit Button -->
              <button 
                @click="handleRecordPayment" 
                :disabled="recordingPayment || !form.amount"
                class="w-full py-2.5 bg-[#062d4d] hover:bg-[#0a3d66] active:scale-[0.98] text-white font-bold text-xs rounded-xl shadow-md transition-all flex items-center justify-center gap-2 disabled:opacity-50 cursor-pointer mt-2"
              >
                <span v-if="recordingPayment" class="w-4 h-4 border-2 border-white/20 border-t-white rounded-full animate-spin"></span>
                <span>{{ recordingPayment ? 'Recording...' : 'Record Payment →' }}</span>
              </button>
            </div>

            <!-- PAYMENT HISTORY LEDGER (7 cols) -->
            <div class="md:col-span-7 bg-white rounded-2xl border border-slate-200/80 p-5 shadow-xs flex flex-col justify-between">
              <div>
                <div class="flex items-center justify-between mb-4">
                  <h3 class="font-bold text-sm text-slate-900 flex items-center gap-1.5">
                    <span>📜</span> Transaction Ledger
                  </h3>
                  <span class="text-xs font-bold text-emerald-700 bg-emerald-50 px-2.5 py-1 rounded-lg border border-emerald-100">
                    Total Settled: {{ formatMoney(getBookingPaid(selectedBooking.id), selectedBooking.currency || 'EUR') }}
                  </span>
                </div>

                <!-- Loading State -->
                <div v-if="loadingPayments" class="py-12 text-center text-slate-400 text-xs">
                  <div class="w-6 h-6 border-2 border-[#062d4d]/20 border-t-[#062d4d] rounded-full animate-spin mx-auto mb-2"></div>
                  Loading payments...
                </div>

                <!-- Empty Ledger State -->
                <div v-else-if="selectedPayments.length === 0" class="bg-slate-50 rounded-xl p-8 text-center border border-slate-100 my-4">
                  <span class="text-2xl block mb-1">💸</span>
                  <h4 class="text-xs font-bold text-slate-700">No Payments Recorded Yet</h4>
                  <p class="text-[11px] text-slate-400 mt-0.5">Use the form on the left to record multi-currency deposits or settlements.</p>
                </div>

                <!-- Payments Table / List -->
                <div v-else class="space-y-2.5 max-h-[360px] overflow-y-auto pr-1">
                  <div 
                    v-for="p in selectedPayments" 
                    :key="p.id" 
                    class="p-3 bg-slate-50 border border-slate-100 rounded-xl flex items-center justify-between text-xs hover:bg-slate-100/70 transition-colors"
                  >
                    <div class="flex items-center gap-2.5">
                      <span class="w-8 h-8 rounded-lg bg-emerald-100 text-emerald-800 font-bold flex items-center justify-center text-sm">
                        ✓
                      </span>
                      <div>
                        <div class="font-bold text-slate-900">{{ p.method }}</div>
                        <div class="text-[10px] text-slate-400 font-mono">{{ p.reference || 'No Reference' }}</div>
                        <div v-if="p.currency && p.currency !== (selectedBooking.currency || 'EUR')" class="text-[10px] text-sky-700 font-medium mt-0.5">
                          Paid: {{ formatMoney(p.amount, p.currency) }} (Rate: {{ p.exchangeRate || '1.0' }})
                        </div>
                      </div>
                    </div>

                    <div class="text-right">
                      <div class="font-black text-emerald-700 text-sm">
                        +{{ formatMoney(p.settledAmount ?? p.amount, selectedBooking.currency || 'EUR') }}
                      </div>
                      <div class="text-[10px] text-slate-400">
                        {{ formatDateTime(p.receivedUtc) }}
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Footer Ledger Actions -->
              <div class="pt-4 border-t border-slate-100 flex items-center justify-between text-xs mt-4">
                <span class="text-slate-400 text-[11px]">Audit snapshot synced with Finance microservice.</span>
                <button 
                  @click="loadBookingPayments(selectedBooking.id)" 
                  class="text-[#062d4d] hover:underline font-bold text-xs flex items-center gap-1"
                >
                  <span>🔄</span> Refresh
                </button>
              </div>
            </div>

          </div>
        </div>

      </div>

    </div>

  </div>
</template>

<style scoped>
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
