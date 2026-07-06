<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '../services/api'

interface Booking {
  id: string
  tourId: string
  customerName: string
  customerEmail: string
  bookingDate: string
  status: string
}

interface Tour {
  id: string
  names: Record<string, string>
}

const bookings = ref<Booking[]>([])
const tours = ref<Tour[]>([])
const loading = ref(true)
const actionLoading = ref(false)

async function loadData() {
  loading.value = true
  try {
    const [bookingsRes, toursRes] = await Promise.all([
      api.get('/api/booking/api/bookings'),
      api.get('/api/content/api/tours')
    ])
    bookings.value = bookingsRes.data
    tours.value = toursRes.data
  } catch (e) {
    console.error('Failed to load bookings data', e)
  } finally {
    loading.value = false
  }
}

function getTourName(tourId: string) {
  const tour = tours.value.find(t => t.id === tourId)
  return tour ? (tour.names?.en || 'Untitled Tour') : 'Unknown Tour'
}

function formatDate(dateStr: string) {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

async function updateStatus(bookingId: string, status: string) {
  actionLoading.value = true
  try {
    await api.put(`/api/booking/api/bookings/${bookingId}/status`, {
      id: bookingId,
      status: status
    })
    // Reload bookings
    const bookingsRes = await api.get('/api/booking/api/bookings')
    bookings.value = bookingsRes.data
  } catch (e) {
    console.error('Failed to update booking status', e)
    alert('Failed to update booking status.')
  } finally {
    actionLoading.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <div class="bookings-page">
    <div class="page-header">
      <div>
        <h2>Bookings Management</h2>
        <p>Review customer reservations and update booking status.</p>
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
            <th>Date</th>
            <th>Customer</th>
            <th>Tour</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="b in bookings" :key="b.id">
            <td>{{ formatDate(b.bookingDate) }}</td>
            <td>
              <div class="customer-info">
                <div class="customer-name">{{ b.customerName }}</div>
                <div class="customer-email">{{ b.customerEmail }}</div>
              </div>
            </td>
            <td class="tour-name">{{ getTourName(b.tourId) }}</td>
            <td>
              <span class="status-badge" :class="b.status.toLowerCase()">
                {{ b.status }}
              </span>
            </td>
            <td>
              <div class="actions">
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
                  Complete 🏁
                </button>
                <button
                  v-if="b.status.toLowerCase() !== 'cancelled' && b.status.toLowerCase() !== 'completed'"
                  @click="updateStatus(b.id, 'Cancelled')"
                  class="btn-action cancel"
                  :disabled="actionLoading"
                >
                  Cancel ✕
                </button>
                <span v-else class="no-actions">—</span>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="bookings.length === 0" class="empty-state">
        <p>No bookings found</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.bookings-page { color: #e0e0e0; }
.page-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 28px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #fff; margin-bottom: 4px; }
.page-header p { color: #8eafc2; font-size: 14px; }

.table-container { background: rgba(10,25,41,0.6); border: 1px solid rgba(255,255,255,0.06); border-radius: 12px; overflow: hidden; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 14px 20px; text-align: left; font-size: 12px; letter-spacing: 0.1em; text-transform: uppercase; color: #8eafc2; background: rgba(0,0,0,0.2); border-bottom: 1px solid rgba(255,255,255,0.06); }
.data-table td { padding: 16px 20px; border-bottom: 1px solid rgba(255,255,255,0.04); font-size: 14px; }
.data-table tr:hover { background: rgba(255,255,255,0.02); }

.customer-name { font-weight: 600; color: #fff; }
.customer-email { font-size: 12px; color: #8eafc2; margin-top: 2px; }
.tour-name { color: #fafafa; font-weight: 500; }

.status-badge { padding: 4px 10px; border-radius: 20px; font-size: 11px; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; display: inline-block; }
.status-badge.pending { background: rgba(232,130,10,0.15); color: #f5a435; border: 1px solid rgba(232,130,10,0.3); }
.status-badge.confirmed { background: rgba(26,139,196,0.15); color: #1a8bc4; border: 1px solid rgba(26,139,196,0.3); }
.status-badge.completed { background: rgba(46,125,79,0.15); color: #4caf78; border: 1px solid rgba(46,125,79,0.3); }
.status-badge.cancelled { background: rgba(220,53,69,0.15); color: #ff6b6b; border: 1px solid rgba(220,53,69,0.3); }

.actions { display: flex; gap: 8px; align-items: center; }
.btn-action { padding: 6px 12px; border: none; border-radius: 4px; color: #fff; font-size: 12px; font-weight: 600; cursor: pointer; transition: opacity 0.2s; }
.btn-action:hover { opacity: 0.9; }
.btn-action:disabled { opacity: 0.5; cursor: not-allowed; }

.btn-action.confirm { background: #1a8bc4; }
.btn-action.complete { background: #2e7d4f; }
.btn-action.cancel { background: rgba(220,53,69,0.15); border: 1px solid rgba(220,53,69,0.3); color: #ff6b6b; }
.no-actions { color: #6b8a9a; font-size: 13px; }

.loading { text-align: center; padding: 60px; color: #8eafc2; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(26,139,196,0.2); border-top-color: #1a8bc4; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state { text-align: center; padding: 48px; color: #8eafc2; }
</style>
