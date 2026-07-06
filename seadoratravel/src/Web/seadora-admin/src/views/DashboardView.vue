<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '../services/api'

const toursCount = ref(0)
const destinationsCount = ref(0)
const categoriesCount = ref(0)
const bookingsCount = ref(0)
const recentBookings = ref<any[]>([])
const loading = ref(true)

async function fetchStats() {
  loading.value = true
  try {
    const [toursRes, destsRes, catsRes, bookingsRes] = await Promise.all([
      api.get('/api/content/api/tours'),
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories'),
      api.get('/api/booking/api/bookings')
    ])
    
    toursCount.value = toursRes.data.length
    destinationsCount.value = destsRes.data.length
    categoriesCount.value = catsRes.data.length
    bookingsCount.value = bookingsRes.data.length
    
    // Map tour titles to bookings and slice recent 5
    const toursMap = new Map(toursRes.data.map((t: any) => [t.id, t.names?.en || 'Untitled Tour']))
    recentBookings.value = bookingsRes.data.slice(0, 5).map((b: any) => ({
      ...b,
      tourName: toursMap.get(b.tourId) || 'Unknown Tour'
    }))
  } catch (e) {
    console.error('Failed to fetch dashboard stats', e)
  } finally {
    loading.value = false
  }
}

function formatDate(dateStr: string) {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

onMounted(fetchStats)
</script>

<template>
  <div class="dashboard-view">
    <div class="welcome-section">
      <h2>Welcome back to Seadora Travel Admin</h2>
      <p>Here is an overview of your platform contents and bookings activity.</p>
    </div>

    <!-- Stats Grid -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon">⛵</div>
        <div class="stat-info">
          <div class="stat-num">{{ loading ? '...' : toursCount }}</div>
          <div class="stat-label">Active Tours</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon">🗺️</div>
        <div class="stat-info">
          <div class="stat-num">{{ loading ? '...' : destinationsCount }}</div>
          <div class="stat-label">Destinations</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon">🏷️</div>
        <div class="stat-info">
          <div class="stat-num">{{ loading ? '...' : categoriesCount }}</div>
          <div class="stat-label">Categories</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon">📅</div>
        <div class="stat-info">
          <div class="stat-num">{{ loading ? '...' : bookingsCount }}</div>
          <div class="stat-label">Total Bookings</div>
        </div>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="activity-section">
      <h3>Recent Bookings</h3>
      <div v-if="loading" class="loading-inline">
        Loading recent bookings...
      </div>
      <div v-else class="bookings-list">
        <div v-for="b in recentBookings" :key="b.id" class="booking-item">
          <div class="booking-left">
            <span class="status-indicator" :class="b.status.toLowerCase()"></span>
            <div>
              <div class="customer-name">{{ b.customerName }}</div>
              <div class="tour-title">{{ b.tourName }}</div>
            </div>
          </div>
          <div class="booking-right">
            <span class="booking-date">{{ formatDate(b.bookingDate) }}</span>
            <span class="status-badge-inline" :class="b.status.toLowerCase()">{{ b.status }}</span>
          </div>
        </div>
        <div v-if="recentBookings.length === 0" class="empty-state">
          No bookings logged yet.
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard-view { color: #e0e0e0; display: flex; flex-direction: column; gap: 32px; }
.welcome-section h2 { font-size: 24px; font-weight: 700; color: #fff; margin-bottom: 4px; }
.welcome-section p { color: #8eafc2; font-size: 14px; }

.stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 20px; }
.stat-card {
  background: rgba(10, 25, 41, 0.6); border: 1px solid rgba(255, 255, 255, 0.06); border-radius: 12px;
  padding: 24px; display: flex; align-items: center; gap: 20px;
}
.stat-icon { font-size: 32px; background: rgba(26, 139, 196, 0.1); width: 60px; height: 60px; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
.stat-num { font-size: 28px; font-weight: 700; color: #fff; line-height: 1.2; }
.stat-label { font-size: 13px; color: #8eafc2; }

.activity-section { background: rgba(10, 25, 41, 0.6); border: 1px solid rgba(255, 255, 255, 0.06); border-radius: 12px; padding: 24px; }
.activity-section h3 { font-size: 18px; font-weight: 600; color: #fff; margin-bottom: 20px; }

.loading-inline { color: #8eafc2; font-size: 14px; text-align: center; padding: 20px; }
.bookings-list { display: flex; flex-direction: column; gap: 12px; }
.booking-item {
  background: rgba(255, 255, 255, 0.02); border: 1px solid rgba(255, 255, 255, 0.04); border-radius: 8px;
  padding: 16px; display: flex; justify-content: space-between; align-items: center;
}
.booking-left { display: flex; align-items: center; gap: 16px; }
.status-indicator { width: 8px; height: 8px; border-radius: 50%; }
.status-indicator.pending { background: #f5a435; }
.status-indicator.confirmed { background: #1a8bc4; }
.status-indicator.completed { background: #4caf78; }
.status-indicator.cancelled { background: #ff6b6b; }

.customer-name { font-weight: 600; color: #fff; }
.tour-title { font-size: 13px; color: #8eafc2; margin-top: 2px; }

.booking-right { display: flex; align-items: center; gap: 16px; }
.booking-date { font-size: 12px; color: #8eafc2; }
.status-badge-inline {
  padding: 4px 8px; border-radius: 12px; font-size: 10px; font-weight: 600; text-transform: uppercase;
}
.status-badge-inline.pending { background: rgba(232, 130, 10, 0.1); color: #f5a435; }
.status-badge-inline.confirmed { background: rgba(26, 139, 196, 0.1); color: #1a8bc4; }
.status-badge-inline.completed { background: rgba(46, 125, 79, 0.1); color: #4caf78; }
.status-badge-inline.cancelled { background: rgba(220, 53, 69, 0.1); color: #ff6b6b; }

.empty-state { text-align: center; padding: 20px; color: #8eafc2; font-size: 14px; }
</style>
