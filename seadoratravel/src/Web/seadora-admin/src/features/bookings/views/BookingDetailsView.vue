<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'

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
  passportFileName?: string
  tripType?: string
  isPaid: boolean
  attendance: string
  guestsList?: { fullName: string, passportFileName?: string }[]
  selectedAddons?: { addonId: string, title: string, unitPrice: number, quantity: number }[]
}

interface Tour {
  id: string
  names: Record<string, string>
  price: number
  currency: string
  maxAllocations: number
}

const route = useRoute()
const booking = ref<Booking | null>(null)
const tour = ref<Tour | null>(null)
const tripGuests = ref<Booking[]>([])
const loading = ref(true)
const searchQuery = ref('')
const actionLoading = ref(false)

async function loadData() {
  loading.value = true
  try {
    const bookingId = route.params.id as string
    const bookingRes = await api.get(`/api/booking/api/bookings/${bookingId}`)
    const bData = bookingRes.data as Booking
    booking.value = bData

    const tourRes = await api.get(`/api/content/api/tours/${bData.tourId}`)
    tour.value = tourRes.data

    // Fetch all bookings for this tour to build the customer trip grid
    const allBookingsRes = await api.get(`/api/booking/api/bookings?tourId=${bData.tourId}`)
    const targetDate = new Date(bData.bookingDate).toDateString()

    tripGuests.value = allBookingsRes.data.filter((b: any) => {
      return new Date(b.bookingDate).toDateString() === targetDate
    })
  } catch (e) {
    console.error('Failed to load booking details', e)
  } finally {
    loading.value = false
  }
}

const filteredGuests = computed(() => {
  return tripGuests.value.filter(g => {
    const q = searchQuery.value.toLowerCase()
    return g.customerName.toLowerCase().includes(q) ||
           g.customerEmail.toLowerCase().includes(q) ||
           (g.whatsApp && g.whatsApp.toLowerCase().includes(q)) ||
           (g.hotelName && g.hotelName.toLowerCase().includes(q))
  })
})

function formatDate(dateStr?: string) {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

async function togglePayment(guest: Booking) {
  actionLoading.value = true
  try {
    const newPaid = !guest.isPaid
    await api.put(`/api/booking/api/bookings/${guest.id}/payment`, {
      id: guest.id,
      isPaid: newPaid
    })
    guest.isPaid = newPaid
  } catch (e) {
    console.error(e)
    alert('Failed to update payment status.')
  } finally {
    actionLoading.value = false
  }
}

async function setAttendance(guest: Booking, status: string) {
  actionLoading.value = true
  try {
    await api.put(`/api/booking/api/bookings/${guest.id}/attendance`, {
      id: guest.id,
      attendance: status
    })
    guest.attendance = status
  } catch (e) {
    console.error(e)
    alert('Failed to update attendance.')
  } finally {
    actionLoading.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <div class="booking-details-container p-6 md:p-10">
    <div v-if="loading" class="flex justify-center items-center py-24">
      <div class="spinner"></div>
    </div>

    <div v-else-if="!booking" class="text-center py-20 text-body">
      <h3>Booking details not found.</h3>
    </div>

    <div v-else class="space-y-8">
      <!-- Title Bar -->
      <div class="flex justify-between items-center pb-4 border-b border-stroke">
        <div>
          <h2 class="text-2xl font-bold text-dark">Trip Manifest Details</h2>
          <p class="text-sm text-body">Excursion schedule, capacity summary, and customer passenger checklist.</p>
        </div>
        <button onclick="window.close()" class="btn-close-window">
          ✕ Close Details
        </button>
      </div>

      <!-- Main Columns -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        <!-- Left Panel: Trip Info & Capacity Summary -->
        <div class="space-y-6">
          <div class="card p-6 space-y-4">
            <h3 class="font-bold text-lg text-dark border-b border-stroke pb-2">🗺️ Trip Summary</h3>
            
            <div class="space-y-3">
              <div>
                <span class="text-xs text-body block uppercase font-semibold">Excursion Tour</span>
                <span class="font-bold text-dark text-base">{{ tour?.names?.en || 'Untitled Tour' }}</span>
              </div>
              <div>
                <span class="text-xs text-body block uppercase font-semibold">Date & Time</span>
                <span class="font-medium text-dark">{{ formatDate(booking.bookingDate) }}</span>
              </div>
              <div class="grid grid-cols-2 gap-2">
                <div>
                  <span class="text-xs text-body block uppercase font-semibold">Trip Type</span>
                  <span class="text-xs font-bold tracking-wider uppercase text-primary bg-primary/10 px-2 py-0.5 rounded inline-block">
                    {{ booking.tripType || 'Group' }}
                  </span>
                </div>
                <div>
                  <span class="text-xs text-body block uppercase font-semibold">Status</span>
                  <span :class="['status-badge !text-[10px]', booking.status.toLowerCase()]">
                    {{ booking.status }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div class="card p-6 space-y-4">
            <h3 class="font-bold text-lg text-dark border-b border-stroke pb-2">📊 Capacity Allocations</h3>
            
            <div class="space-y-4">
              <div class="flex justify-between items-center text-sm font-semibold">
                <span class="text-body">Total Booked Places</span>
                <span class="text-dark">{{ tripGuests.length }} Seats</span>
              </div>
              <div class="flex justify-between items-center text-sm font-semibold">
                <span class="text-body">Max Excursion Capacity</span>
                <span class="text-dark">{{ tour?.maxAllocations || 20 }} Seats</span>
              </div>
              
              <!-- Progress Bar -->
              <div class="w-full bg-[#E2E8F0] rounded-full h-3 overflow-hidden">
                <div 
                  class="bg-primary h-full transition-all duration-500"
                  :style="{ width: Math.min(100, (tripGuests.length / (tour?.maxAllocations || 20)) * 100) + '%' }"
                ></div>
              </div>
              <div class="text-right text-xs text-body font-mono">
                {{ Math.round((tripGuests.length / (tour?.maxAllocations || 20)) * 100) }}% Reserved
              </div>
            </div>
          </div>
        </div>

        <!-- Right Panel: Guest list check grid -->
        <div class="lg:col-span-2 space-y-6">
          <div class="card p-6">
            <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6">
              <div>
                <h3 class="font-bold text-lg text-dark">👥 Booked Customers & Guest Manifest</h3>
                <p class="text-xs text-body">Verify passenger identity, toggle payment, and check attendance logs.</p>
              </div>
              
              <!-- Search guest -->
              <input 
                v-model="searchQuery" 
                type="text" 
                placeholder="🔍 Search guests by name/hotel..." 
                class="px-4 py-2 border border-stroke rounded-lg text-sm bg-white text-dark outline-none focus:border-primary w-full md:w-72 shadow-sm"
              />
            </div>

            <!-- Guest Grid Table -->
            <div class="overflow-x-auto border border-stroke rounded-lg">
              <table class="w-full text-left border-collapse">
                <thead>
                  <tr class="bg-[#F8FAFC] border-b border-stroke text-xs text-[#64748B] font-semibold uppercase tracking-wider">
                    <th class="p-4">Customer Info</th>
                    <th class="p-4">WhatsApp</th>
                    <th class="p-4">Hotel / Room</th>
                    <th class="p-4">Paid Status</th>
                    <th class="p-4">Attendance</th>
                    <th class="p-4 text-center">Manage Attendance</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-stroke text-sm text-black">
                  <tr v-for="guest in filteredGuests" :key="guest.id" class="hover:bg-[#F9FAFB]">
                    <td class="p-4">
                      <div class="font-bold text-dark">{{ guest.customerName }}</div>
                      <div class="text-xs text-body font-mono">{{ guest.customerEmail }}</div>
                      <div v-if="guest.guestsList && guest.guestsList.length > 0" class="mt-2 text-[10px] bg-slate-100 p-1.5 rounded text-slate-600">
                        <div class="font-bold uppercase mb-1">Guests List:</div>
                        <div v-for="(g, i) in guest.guestsList" :key="i">
                          • {{ g.fullName }} <span v-if="g.passportFileName">(Passport Attached)</span>
                        </div>
                      </div>
                      <div v-if="guest.selectedAddons && guest.selectedAddons.length > 0" class="mt-2 flex flex-wrap gap-1">
                        <span v-for="(addon, i) in guest.selectedAddons" :key="i" class="bg-amber-100 text-amber-800 text-[9px] px-1.5 py-0.5 rounded font-bold uppercase border border-amber-200">
                          + {{ addon.title }} ({{ addon.quantity }})
                        </span>
                      </div>
                    </td>
                    <td class="p-4 font-mono text-xs">{{ guest.whatsApp || '—' }}</td>
                    <td class="p-4">
                      <div class="font-medium" v-if="guest.hotelName">{{ guest.hotelName }}</div>
                      <div class="text-xs text-body" v-if="guest.roomNumber">Room: {{ guest.roomNumber }}</div>
                      <span v-if="!guest.hotelName && !guest.roomNumber" class="text-body text-xs">—</span>
                    </td>
                    <!-- Paid status -->
                    <td class="p-4">
                      <button 
                        @click="togglePayment(guest)"
                        :class="['px-3 py-1 rounded-full text-xs font-bold transition-all select-none cursor-pointer', guest.isPaid ? 'bg-emerald-100 text-emerald-700' : 'bg-rose-100 text-rose-700']"
                        :disabled="actionLoading"
                        title="Click to toggle Payment Status"
                      >
                        {{ guest.isPaid ? '✓ Paid' : '✕ Unpaid' }}
                      </button>
                    </td>
                    <!-- Attendance Status -->
                    <td class="p-4">
                      <span :class="['px-2.5 py-0.5 rounded text-xs font-semibold uppercase tracking-wider', 
                        guest.attendance === 'Present' ? 'bg-emerald-100 text-emerald-700' : 
                        guest.attendance === 'Absent' ? 'bg-rose-100 text-rose-700' : 
                        'bg-amber-100 text-amber-700']"
                      >
                        {{ guest.attendance || 'Pending' }}
                      </span>
                    </td>
                    <!-- Actions -->
                    <td class="p-4 text-center">
                      <div class="flex gap-2 justify-center">
                        <button 
                          @click="setAttendance(guest, 'Present')"
                          class="px-2 py-1 bg-emerald-600 hover:bg-emerald-700 text-white rounded text-xs font-semibold cursor-pointer shadow-sm transition-all"
                          :disabled="guest.attendance === 'Present' || actionLoading"
                        >
                          Present
                        </button>
                        <button 
                          @click="setAttendance(guest, 'Absent')"
                          class="px-2 py-1 bg-rose-600 hover:bg-rose-700 text-white rounded text-xs font-semibold cursor-pointer shadow-sm transition-all"
                          :disabled="guest.attendance === 'Absent' || actionLoading"
                        >
                          Absent
                        </button>
                      </div>
                    </td>
                  </tr>
                  <tr v-if="filteredGuests.length === 0">
                    <td colspan="6" class="p-8 text-center text-body">No guests match search filters.</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<style scoped>
.booking-details-container {
  max-width: 1300px;
  margin: 0 auto;
  min-height: 100vh;
}
.card {
  background: #ffffff;
  border: 1px solid var(--stroke, #E2E8F0);
  border-radius: 8px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);
}
.btn-close-window {
  background: #ffffff;
  color: var(--body, #64748b);
  border: 1px solid var(--stroke, #e2e8f0);
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}
.btn-close-window:hover {
  background: #f8fafc;
  color: var(--dark, #1c2434);
}
.status-badge { 
  padding: 4px 10px; 
  border-radius: 20px; 
  font-size: 11px; 
  font-weight: 600; 
  letter-spacing: 0.05em; 
  text-transform: uppercase; 
  display: inline-block; 
}
.status-badge.pending { background: rgba(232, 130, 10, 0.1); color: #e8820a; border: 1px solid rgba(232, 130, 10, 0.2); }
.status-badge.confirmed { background: rgba(60, 80, 224, 0.1); color: #3C50E0; border: 1px solid rgba(60, 80, 224, 0.2); }
.status-badge.completed { background: rgba(16, 185, 129, 0.1); color: #10B981; border: 1px solid rgba(16, 185, 129, 0.2); }
.status-badge.cancelled { background: rgba(211, 64, 83, 0.1); color: #D34053; border: 1px solid rgba(211, 64, 83, 0.2); }

.spinner {
  border: 3px solid rgba(60, 80, 224, 0.1);
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border-left-color: #3c50e0;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
