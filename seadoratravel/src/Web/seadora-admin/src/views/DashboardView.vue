<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/services/api'

const toursCount = ref(0)
const destinationsCount = ref(0)
const categoriesCount = ref(0)
const totalBookings = ref(0)
const totalRevenue = ref(0)
const totalPlatformEarnings = ref(0)
const recentBookings = ref<any[]>([])
const loading = ref(true)

async function fetchStats() {
  loading.value = true
  try {
    const [toursRes, destsRes, catsRes, reportsRes] = await Promise.all([
      api.get('/api/content/api/tours'),
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories'),
      api.get('/api/booking/api/reports/dashboard')
    ])
    
    toursCount.value = toursRes.data.length
    destinationsCount.value = destsRes.data.length
    categoriesCount.value = catsRes.data.length
    
    totalBookings.value = reportsRes.data.totalBookings
    totalRevenue.value = reportsRes.data.totalRevenue
    totalPlatformEarnings.value = reportsRes.data.totalPlatformEarnings
    recentBookings.value = reportsRes.data.recentBookings.slice(0, 5)
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

function formatPrice(val: number) {
  return new Intl.NumberFormat('en-IE', { style: 'currency', currency: 'EUR' }).format(val)
}

onMounted(fetchStats)
</script>

<template>
  <div class="dashboard-view">
    <div class="welcome-section">
      <h2>Welcome back to Seadora Travel Admin</h2>
      <p>Here is an overview of your platform contents, bookings, and financial analytics.</p>
    </div>

    <!-- Stats Grid -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon bg-[rgba(60,80,224,0.08)] text-primary">💰</div>
        <div class="stat-info">
          <div class="stat-num text-black">{{ loading ? '...' : formatPrice(totalRevenue) }}</div>
          <div class="stat-label">Total Revenue</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-[rgba(16,185,129,0.08)] text-[#10B981]">📈</div>
        <div class="stat-info">
          <div class="stat-num text-[#10B981]">{{ loading ? '...' : formatPrice(totalPlatformEarnings) }}</div>
          <div class="stat-label">Net Platform Earnings</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-[rgba(60,80,224,0.08)] text-primary">📅</div>
        <div class="stat-info">
          <div class="stat-num text-black">{{ loading ? '...' : totalBookings }}</div>
          <div class="stat-label">Total Bookings</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-[rgba(240,149,14,0.08)] text-[#F0950E]">⛵</div>
        <div class="stat-info">
          <div class="stat-num text-black">{{ loading ? '...' : toursCount }}</div>
          <div class="stat-label">Active Tours</div>
        </div>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="activity-section">
      <h3>Recent Booking Transactions</h3>
      <div v-if="loading" class="loading-inline">
        Loading recent bookings...
      </div>
      <div v-else class="bookings-list">
        <div v-for="b in recentBookings" :key="b.id" class="booking-item">
          <div class="booking-left">
            <span class="status-indicator" :class="b.status.toLowerCase()"></span>
            <div>
              <div class="customer-name text-black">
                {{ b.customerName }} 
                <span class="customer-email">({{ b.customerEmail }})</span>
              </div>
              <div class="tour-title">{{ b.tourName }} — <span class="text-primary font-bold">{{ formatPrice(b.price) }}</span></div>
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
.dashboard-view { color: #24303F; display: flex; flex-direction: column; gap: 32px; }
.welcome-section h2 { font-size: 24px; font-weight: 700; color: #1C2434; margin-bottom: 4px; }
.welcome-section p { color: #64748B; font-size: 14px; }

.stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 20px; }
.stat-card {
  background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07);
  padding: 24px; display: flex; align-items: center; gap: 20px;
}
.stat-icon { font-size: 28px; background: rgba(60, 80, 224, 0.08); width: 55px; height: 55px; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
.stat-num { font-size: 24px; font-weight: 700; line-height: 1.2; }
.stat-label { font-size: 13px; color: #64748B; margin-top: 2px; }

.activity-section { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; padding: 24px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); }
.activity-section h3 { font-size: 18px; font-weight: 700; color: #1C2434; margin-bottom: 20px; }

.loading-inline { color: #64748B; font-size: 14px; text-align: center; padding: 20px; }
.bookings-list { display: flex; flex-direction: column; gap: 12px; }
.booking-item {
  background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px;
  padding: 16px; display: flex; justify-content: space-between; align-items: center; transition: background-color 0.2s;
}
.booking-item:hover { background-color: #F9FAFB; }
.booking-left { display: flex; align-items: center; gap: 16px; }
.status-indicator { width: 8px; height: 8px; border-radius: 50%; }
.status-indicator.pending { background: #e8820a; }
.status-indicator.confirmed { background: #3C50E0; }
.status-indicator.completed { background: #10B981; }
.status-indicator.cancelled { background: #D34053; }

.customer-name { font-weight: 600; }
.customer-email { font-size: 11px; color: #64748B; font-weight: normal; margin-left: 8px; }
.tour-title { font-size: 13px; color: #64748B; margin-top: 2px; }

.booking-right { display: flex; align-items: center; gap: 16px; }
.booking-date { font-size: 12px; color: #64748B; }
.status-badge-inline {
  padding: 4px 8px; border-radius: 12px; font-size: 10px; font-weight: 600; text-transform: uppercase;
}
.status-badge-inline.pending { background: rgba(232, 130, 10, 0.1); color: #e8820a; }
.status-badge-inline.confirmed { background: rgba(60, 80, 224, 0.1); color: #3C50E0; }
.status-badge-inline.completed { background: rgba(16, 185, 129, 0.1); color: #10B981; }
.status-badge-inline.cancelled { background: rgba(211, 64, 83, 0.1); color: #D34053; }

.empty-state { text-align: center; padding: 20px; color: #64748B; font-size: 14px; }
</style>
