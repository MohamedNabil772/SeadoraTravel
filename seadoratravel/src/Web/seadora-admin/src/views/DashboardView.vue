<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import api from '@/services/api'
import { useNotificationStore } from '@/features/notifications/store/notificationStore'

const notificationStore = useNotificationStore()
let autoRefreshTimer: any = null

const toursCount = ref(0)
const destinationsCount = ref(0)
const categoriesCount = ref(0)
const totalBookings = ref(0)
const totalRevenue = ref(0)
const totalPlatformEarnings = ref(0)
const totalFavoritesCount = ref(0)
const recentBookings = ref<any[]>([])
const favoriteTours = ref<any[]>([])
const loading = ref(true)

async function fetchStats(isSilent = false) {
  if (!isSilent) loading.value = true
  try {
    const [toursRes, destsRes, catsRes, reportsRes, favRes] = await Promise.all([
      api.get('/api/content/api/tours'),
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories'),
      api.get('/api/booking/api/reports/dashboard'),
      api.get('/api/content/api/tours/favorites/leaderboard?limit=10').catch(() => ({ data: [] }))
    ])
    
    toursCount.value = toursRes.data.length
    destinationsCount.value = destsRes.data.length
    categoriesCount.value = catsRes.data.length
    
    totalBookings.value = reportsRes.data.totalBookings
    totalRevenue.value = reportsRes.data.totalRevenue
    totalPlatformEarnings.value = reportsRes.data.totalPlatformEarnings
    recentBookings.value = reportsRes.data.recentBookings.slice(0, 5)

    if (favRes.data && Array.isArray(favRes.data) && favRes.data.length > 0) {
      favoriteTours.value = favRes.data
    } else if (Array.isArray(toursRes.data)) {
      favoriteTours.value = [...toursRes.data]
        .sort((a, b) => (b.favoriteCount || 0) - (a.favoriteCount || 0))
        .slice(0, 10)
    }

    totalFavoritesCount.value = Array.isArray(toursRes.data)
      ? toursRes.data.reduce((sum: number, t: any) => sum + (t.favoriteCount || 0), 0)
      : 0
  } catch (e) {
    console.error('Failed to fetch dashboard stats', e)
  } finally {
    if (!isSilent) loading.value = false
  }
}

watch(() => notificationStore.lastUpdated, () => {
  fetchStats(true)
})

function formatDate(dateStr: string) {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

function formatPrice(val: number) {
  return new Intl.NumberFormat('en-IE', { style: 'currency', currency: 'EUR' }).format(val)
}

onMounted(() => {
  fetchStats()
  autoRefreshTimer = setInterval(() => {
    fetchStats(true)
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
  <div class="dashboard-view">
    <div class="welcome-section">
      <h2>Welcome back to Seadora Travel Admin</h2>
      <p>Here is an overview of your platform contents, wishlist favorites, bookings, and financial analytics.</p>
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
        <div class="stat-icon bg-[rgba(244,63,94,0.08)] text-rose-500">💖</div>
        <div class="stat-info">
          <div class="stat-num text-rose-600">{{ loading ? '...' : totalFavoritesCount }}</div>
          <div class="stat-label">Saved Favorites / Wishlists</div>
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

    <!-- Two-Column Grid: Most Favorited Tours vs Recent Bookings -->
    <div class="dashboard-grid">
      <!-- Most Favorited Experiences Leaderboard -->
      <div class="activity-section">
        <div class="section-header">
          <h3>💖 Most Favorited Experiences</h3>
          <span class="badge-pill">Customer Demand</span>
        </div>
        <div v-if="loading" class="loading-inline">
          Loading favorites analytics...
        </div>
        <div v-else class="tours-leaderboard">
          <div v-for="(tour, idx) in favoriteTours" :key="tour.id" class="leaderboard-item">
            <div class="rank-badge" :class="'rank-' + (idx + 1)">#{{ idx + 1 }}</div>
            <div class="tour-details">
              <div class="tour-name text-black font-semibold">
                {{ tour.title || tour.names?.['en'] || 'Tour' }}
              </div>
              <div class="tour-sub">
                <span>{{ tour.destinationName || 'Red Sea' }}</span>
                <span class="dot">•</span>
                <span>{{ formatPrice(tour.price) }}</span>
                <span class="dot">•</span>
                <span class="text-amber-500 font-bold">★ {{ tour.rating || '4.9' }}</span>
              </div>
            </div>
            <div class="favorite-count-badge">
              <span class="heart-icon">❤️</span>
              <span class="count-num">{{ tour.favoriteCount || 0 }}</span>
              <span class="count-label">saves</span>
            </div>
          </div>
          <div v-if="favoriteTours.length === 0" class="empty-state">
            No favorites recorded yet.
          </div>
        </div>
      </div>

      <!-- Recent Booking Transactions -->
      <div class="activity-section">
        <div class="section-header">
          <h3>Recent Booking Transactions</h3>
          <span class="badge-pill">Live Orders</span>
        </div>
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
  </div>
</template>

<style scoped>
.dashboard-view { color: #24303F; display: flex; flex-direction: column; gap: 32px; }
.welcome-section h2 { font-size: 24px; font-weight: 700; color: #1C2434; margin-bottom: 4px; }
.welcome-section p { color: #64748B; font-size: 14px; }

.stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; }
.stat-card {
  background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 8px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.05);
  padding: 20px; display: flex; align-items: center; gap: 16px;
}
.stat-icon { font-size: 26px; width: 50px; height: 50px; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
.stat-num { font-size: 22px; font-weight: 700; line-height: 1.2; }
.stat-label { font-size: 12px; color: #64748B; margin-top: 2px; }

.dashboard-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(420px, 1fr)); gap: 24px; }
.activity-section { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 8px; padding: 24px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.05); }
.section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.section-header h3 { font-size: 17px; font-weight: 700; color: #1C2434; margin: 0; }
.badge-pill { font-size: 11px; font-weight: 700; padding: 3px 10px; background: #F1F5F9; color: #475569; border-radius: 20px; text-transform: uppercase; letter-spacing: 0.05em; }

.loading-inline { color: #64748B; font-size: 14px; text-align: center; padding: 20px; }
.bookings-list, .tours-leaderboard { display: flex; flex-direction: column; gap: 10px; }

.leaderboard-item {
  background: #F8FAFC; border: 1px solid #E2E8F0; border-radius: 6px;
  padding: 12px 16px; display: flex; align-items: center; gap: 14px; transition: background-color 0.2s;
}
.leaderboard-item:hover { background-color: #F1F5F9; }
.rank-badge {
  font-size: 12px; font-weight: 800; padding: 4px 8px; border-radius: 4px; background: #E2E8F0; color: #475569; min-width: 32px; text-align: center;
}
.rank-badge.rank-1 { background: #FEF3C7; color: #D97706; }
.rank-badge.rank-2 { background: #E0E7FF; color: #4F46E5; }
.rank-badge.rank-3 { background: #FCE7F3; color: #DB2777; }

.tour-details { flex: 1; min-width: 0; }
.tour-name { font-size: 13px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.tour-sub { font-size: 11px; color: #64748B; display: flex; align-items: center; gap: 6px; margin-top: 2px; }
.dot { font-size: 8px; opacity: 0.5; }

.favorite-count-badge {
  display: flex; align-items: center; gap: 5px; background: #FFF1F2; border: 1px solid #FFE4E6;
  padding: 4px 10px; border-radius: 12px; font-size: 12px; font-weight: 700; color: #E11D48; shrink: 0;
}
.count-label { font-size: 10px; color: #9F1239; font-weight: 500; }

.booking-item {
  background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 6px;
  padding: 12px 16px; display: flex; justify-content: space-between; align-items: center; transition: background-color 0.2s;
}
.booking-item:hover { background-color: #F9FAFB; }
.booking-left { display: flex; align-items: center; gap: 12px; }
.status-indicator { width: 8px; height: 8px; border-radius: 50%; }
.status-indicator.pending { background: #e8820a; }
.status-indicator.confirmed { background: #3C50E0; }
.status-indicator.completed { background: #10B981; }
.status-indicator.cancelled { background: #D34053; }

.customer-name { font-size: 13px; font-weight: 600; }
.customer-email { font-size: 11px; color: #64748B; font-weight: normal; margin-left: 6px; }
.tour-title { font-size: 12px; color: #64748B; margin-top: 2px; }

.booking-right { display: flex; align-items: center; gap: 12px; }
.booking-date { font-size: 11px; color: #64748B; }
.status-badge-inline {
  padding: 3px 8px; border-radius: 12px; font-size: 9px; font-weight: 600; text-transform: uppercase;
}
.status-badge-inline.pending { background: rgba(232, 130, 10, 0.1); color: #e8820a; }
.status-badge-inline.confirmed { background: rgba(60, 80, 224, 0.1); color: #3C50E0; }
.status-badge-inline.completed { background: rgba(16, 185, 129, 0.1); color: #10B981; }
.status-badge-inline.cancelled { background: rgba(211, 64, 83, 0.1); color: #D34053; }

.empty-state { text-align: center; padding: 20px; color: #64748B; font-size: 14px; }
</style>

