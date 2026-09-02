<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, computed } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import { useToast } from '@/composables/useToast'
import { useNotificationStore } from '@/features/notifications/store/notificationStore'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'
import CreateBookingModal from '../components/CreateBookingModal.vue'
import { Plus, RotateCw } from 'lucide-vue-next'

const router = useRouter()
const notificationStore = useNotificationStore()
const isCreateModalOpen = ref(false)
let autoRefreshTimer: any = null

interface Booking {
  id: string
  tourId: string
  customerName: string
  customerEmail: string
  bookingDate: string
  tourDate?: string
  status: string
  whatsApp?: string
  hotelName?: string
  roomNumber?: string
  passportFileName?: string
  tripType?: string
  missingIdentification?: boolean
  guestsList?: { fullName: string, passportFileName?: string }[]
  selectedAddons?: { addonId: string, title: string, unitPrice: number, quantity: number }[]
}

interface Tour {
  id: string
  names: Record<string, string>
}

const bookings = ref<Booking[]>([])
const tours = ref<Tour[]>([])
const loading = ref(true)
const actionLoading = ref(false)

const currentPage = ref(1)
const pageSize = ref(10)

const paginatedBookings = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return bookings.value.slice(start, start + pageSize.value)
})



async function loadData() {
  loading.value = true
  try {
    const [bookingsRes, toursRes] = await Promise.all([
      api.get('/api/booking/api/bookings'),
      api.get('/api/content/api/tours')
    ])
    const rawBookings = bookingsRes.data
    bookings.value = Array.isArray(rawBookings) ? rawBookings : (rawBookings?.items || [])
    tours.value = toursRes.data
  } catch (e) {
    console.error('Failed to load bookings data', e)
  } finally {
    loading.value = false
  }
}

function getBookedCount(tourId: string, dateStr?: string) {
  if (!bookings.value || bookings.value.length === 0 || !dateStr) return 0
  const targetDate = new Date(dateStr).toDateString()
  return bookings.value.filter(b => b.tourId === tourId && new Date(b.tourDate || b.bookingDate).toDateString() === targetDate).length
}

function getMaxAllocations(tourId: string) {
  if (!tours.value || tours.value.length === 0) return 20
  const tour = tours.value.find(t => t.id === tourId)
  return tour ? (tour as any).maxAllocations || 20 : 20
}

function getTourName(tourId: string) {
  if (!tours.value || tours.value.length === 0) return 'Unknown Tour'
  const tour = tours.value.find(t => t.id === tourId)
  return tour ? (tour.names?.en || 'Untitled Tour') : 'Unknown Tour'
}

function formatTripDate(dateStr?: string) {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  if (isNaN(date.getTime())) return dateStr
  return date.toLocaleDateString('en-US', {
    weekday: 'short',
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function formatCreationDate(dateStr?: string) {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  if (isNaN(date.getTime())) return dateStr
  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const toast = useToast()

async function updateStatus(bookingId: string, status: string) {
  actionLoading.value = true
  try {
    await api.put(`/api/booking/api/bookings/${bookingId}/status`, {
      id: bookingId,
      status: status
    })
    toast.success('Booking status updated successfully')
    // Reload bookings
    const bookingsRes = await api.get('/api/booking/api/bookings')
    const rawBookings = bookingsRes.data
    bookings.value = Array.isArray(rawBookings) ? rawBookings : (rawBookings?.items || [])
  } catch (e) {
    console.error('Failed to update booking status', e)
    toast.error('Failed to update booking status.')
  } finally {
    actionLoading.value = false
  }
}



async function refreshBookingsSilently() {
  try {
    const bookingsRes = await api.get('/api/booking/api/bookings')
    const rawBookings = bookingsRes.data
    bookings.value = Array.isArray(rawBookings) ? rawBookings : (rawBookings?.items || [])
  } catch (e) {
    console.debug('Silent bookings sync', e)
  }
}

// Watch for notification changes from the store (triggered when bookings are made)
watch(() => notificationStore.lastUpdated, () => {
  refreshBookingsSilently()
})

watch(() => notificationStore.unreadBookingsCount, () => {
  refreshBookingsSilently()
})

function goToCreateBooking() {
  router.push('/bookings/create')
}

onMounted(() => {
  loadData()
  // Active page auto-poll every 10 seconds to catch all real-time creations
  autoRefreshTimer = setInterval(() => {
    refreshBookingsSilently()
  }, 10000)
})

onUnmounted(() => {
  if (autoRefreshTimer) {
    clearInterval(autoRefreshTimer)
    autoRefreshTimer = null
  }
})
</script>

<template>
  <div class="bookings-page">
    <div class="page-header">
      <div>
        <h2>Bookings Management</h2>
        <p>Review customer reservations, capacity allocations, and create manual VIP bookings.</p>
      </div>
      <div class="header-actions flex items-center gap-3">
        <button
          @click="loadData"
          class="inline-flex items-center gap-1.5 px-3 py-2 text-xs font-semibold text-text-muted hover:text-text-main bg-white border border-border/80 rounded-lg shadow-xs hover:bg-surface-sunken transition-all cursor-pointer"
          title="Refresh bookings grid"
        >
          <RotateCw class="w-3.5 h-3.5" :class="{ 'animate-spin': loading }" />
          <span>Refresh</span>
        </button>
        <button
          @click="goToCreateBooking"
          class="btn-create"
        >
          <Plus class="w-4 h-4" />
          <span>Create VIP Booking</span>
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading bookings...</p>
    </div>

    <!-- Table -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Trip Date</th>
            <th>Trip Type</th>
            <th>Customer</th>
            <th>Tour & Capacity</th>
            <th>Status</th>
            <th class="text-right pr-6">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="b in paginatedBookings" :key="b.id">
            <td class="font-mono text-xs text-body">
              <div class="font-bold text-[#062d4d] text-sm">{{ formatTripDate(b.tourDate || b.bookingDate) }}</div>
              <div class="text-[10px] text-slate-400 mt-0.5">Booked: {{ formatCreationDate(b.bookingDate) }}</div>
            </td>
            <td class="font-bold text-xs tracking-wider uppercase text-primary">
              {{ b.tripType || 'Group' }}
            </td>
            <td>
              <div class="customer-name text-black">{{ b.customerName }}</div>
              <div class="customer-email">{{ b.customerEmail }}</div>
              <div v-if="b.guestsList && b.guestsList.length > 0" class="mt-2 text-[10px] bg-slate-100 p-1 rounded text-slate-600 max-w-[200px]">
                <div class="font-bold uppercase mb-0.5">Guests: {{ b.guestsList.length }}</div>
              </div>
              <div v-if="b.selectedAddons && b.selectedAddons.length > 0" class="mt-1 flex flex-wrap gap-1 max-w-[200px]">
                <span v-for="(addon, i) in b.selectedAddons" :key="i" class="bg-amber-100 text-amber-800 text-[9px] px-1.5 py-0.5 rounded font-bold uppercase border border-amber-200" title="Add-on">
                  +{{ addon.title }}
                </span>
              </div>
            </td>
            <td class="tour-name font-semibold text-black">
              <div>{{ getTourName(b.tourId) }}</div>
              <div class="text-xs text-body font-mono mt-1">
                Allocations: {{ getBookedCount(b.tourId, b.tourDate || b.bookingDate) }} / {{ getMaxAllocations(b.tourId) }}
              </div>
            </td>
            <td>
              <div class="flex flex-wrap gap-1.5 items-center">
                <span :class="['status-badge', b.status.toLowerCase()]">
                  {{ b.status }}
                </span>
                <span 
                  v-if="b.missingIdentification" 
                  class="status-badge !bg-amber-100 !text-amber-900 border border-amber-300"
                  title="Missing passenger passport identification or ID number"
                >
                  ⚠️ Missing Passports
                </span>
                <span 
                  v-else-if="b.guestsList && b.guestsList.length > 0" 
                  class="status-badge !bg-emerald-50 !text-emerald-800 border border-emerald-300"
                  title="Passenger identification on file"
                >
                  🛡️ ID Verified
                </span>
                <span 
                  v-if="getBookedCount(b.tourId, b.tourDate || b.bookingDate) >= getMaxAllocations(b.tourId)" 
                  class="status-badge completed !bg-emerald-600 !text-white"
                  title="All capacity places booked for this trip"
                >
                  All Places Booked
                </span>
              </div>
            </td>
            <td class="text-right pr-6">
              <div class="actions justify-end">
                <router-link
                  :to="'/bookings/' + b.id + '/details'"
                  target="_blank"
                  class="btn-action view-guests text-center inline-block"
                  style="background: #64748B;"
                >
                  Details 📄
                </router-link>
                <button
                  v-if="b.status.toLowerCase() === 'pending'"
                  @click="updateStatus(b.id, 'Confirmed')"
                  class="btn-action confirm"
                  :disabled="actionLoading"
                >
                  Confirm ✓
                </button>
                <button
                  v-if="b.status.toLowerCase() === 'confirmed'"
                  @click="updateStatus(b.id, 'Completed')"
                  class="btn-action complete"
                  :disabled="actionLoading"
                >
                  All Places Booked 🏁
                </button>
                <button
                  v-if="b.status.toLowerCase() !== 'cancelled' && b.status.toLowerCase() !== 'completed'"
                  @click="updateStatus(b.id, 'Cancelled')"
                  class="btn-action cancel"
                  :disabled="actionLoading"
                >
                  Cancel ✕
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="bookings.length === 0">
            <td colspan="6" class="text-center py-8 text-body">No bookings found.</td>
          </tr>
        </tbody>
      </table>

      <LuxuryPagination
        v-if="bookings.length > 0"
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="bookings.length"
      />
    </div>

    <!-- Create VIP Booking Modal -->
    <CreateBookingModal 
      :is-open="isCreateModalOpen" 
      @close="isCreateModalOpen = false" 
      @booking-created="loadData" 
    />
  </div>
</template>

<style scoped>
.bookings-page { color: #24303F; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #1C2434; margin-bottom: 4px; }
.page-header p { color: #64748B; font-size: 14px; }

.table-container { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; overflow: hidden; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 16px 24px; text-align: left; font-size: 12px; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748B; background: #F7F9FC; border-bottom: 1px solid #E2E8F0; }
.data-table td { padding: 16px 24px; border-bottom: 1px solid #E2E8F0; font-size: 14px; color: #24303F; }
.data-table tr:hover { background: #F9FAFB; }

.customer-name { font-weight: 600; }
.customer-email { font-size: 12px; color: #64748B; margin-top: 2px; }
.tour-name { font-weight: 500; }

.status-badge { 
  padding: 4px 10px; 
  border-radius: 20px; 
  font-size: 11px; 
  font-weight: 600; 
  letter-spacing: 0.05em; 
  text-transform: uppercase; 
  display: inline-block; 
  box-shadow: 0 1px 2px rgba(0,0,0,0.02);
  transition: all 0.2s ease;
}
.status-badge:hover { filter: brightness(0.95); transform: translateY(-0.5px); }
.status-badge.pending { background: rgba(232, 130, 10, 0.1); color: #e8820a; border: 1px solid rgba(232, 130, 10, 0.2); }
.status-badge.confirmed { background: rgba(60, 80, 224, 0.1); color: #3C50E0; border: 1px solid rgba(60, 80, 224, 0.2); }
.status-badge.completed { background: rgba(16, 185, 129, 0.1); color: #10B981; border: 1px solid rgba(16, 185, 129, 0.2); }
.status-badge.cancelled { background: rgba(211, 64, 83, 0.1); color: #D34053; border: 1px solid rgba(211, 64, 83, 0.2); }

.actions { display: flex; gap: 8px; align-items: center; }
.btn-action { 
  padding: 8px 14px; 
  border: none; 
  border-radius: 6px; 
  color: #fff; 
  font-size: 12px; 
  font-weight: 600; 
  cursor: pointer; 
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 1px 2px rgba(0,0,0,0.05);
}
.btn-action:hover { 
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  opacity: 0.95; 
}
.btn-action:active {
  transform: translateY(0);
  box-shadow: 0 1px 2px rgba(0,0,0,0.05);
}
.btn-action:disabled { opacity: 0.5; cursor: not-allowed; transform: none; box-shadow: none; }

.btn-action.confirm { background: #3C50E0; }
.btn-action.complete { background: #10B981; }
.btn-action.cancel { background: rgba(211, 64, 83, 0.1); border: 1px solid rgba(211, 64, 83, 0.2); color: #D34053; }
.no-actions { color: #8A99AD; font-size: 13px; }

.loading { text-align: center; padding: 60px; color: #64748B; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(60, 80, 224, 0.1); border-top-color: #3C50E0; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state { text-align: center; padding: 48px; color: #64748B; }
</style>
