<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '../services/api'

interface Feedback {
  id: string
  tourId: string
  rating: number
  comment: string
  customerName: string
  customerEmail: string
  createdAt: string
  isVisible: boolean
}

interface Tour {
  id: string
  names: Record<string, string>
}

const feedbacks = ref<Feedback[]>([])
const tours = ref<Tour[]>([])
const loading = ref(true)
const actionLoading = ref(false)

async function loadData() {
  loading.value = true
  try {
    const [feedbacksRes, toursRes] = await Promise.all([
      api.get('/api/booking/api/feedbacks?includeHidden=true'), // include hidden for admin
      api.get('/api/content/api/tours')
    ])
    feedbacks.value = feedbacksRes.data
    tours.value = toursRes.data
  } catch (e) {
    console.error('Failed to load feedback data', e)
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
    day: 'numeric'
  })
}

function getStars(rating: number) {
  return '★'.repeat(Math.round(rating)) + '☆'.repeat(5 - Math.round(rating))
}

async function toggleVisibility(f: Feedback) {
  actionLoading.value = true
  const newVisibility = !f.isVisible
  try {
    await api.put(`/api/booking/api/feedbacks/${f.id}/visibility`, {
      id: f.id,
      isVisible: newVisibility
    })
    f.isVisible = newVisibility
  } catch (e) {
    console.error('Failed to toggle visibility', e)
    alert('Failed to update visibility status.')
  } finally {
    actionLoading.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <div class="feedback-page">
    <div class="page-header">
      <div>
        <h2>Customer Feedback</h2>
        <p>Review customer ratings, stars, and comments left on tour packages. Use controls to hide inappropriate reviews.</p>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading feedback...</p>
    </div>

    <!-- Table -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Date</th>
            <th>Customer</th>
            <th>Tour</th>
            <th>Rating</th>
            <th>Comment</th>
            <th>Visibility</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="f in feedbacks" :key="f.id">
            <td>{{ formatDate(f.createdAt) }}</td>
            <td>
              <div class="customer-info">
                <div class="customer-name">{{ f.customerName }}</div>
                <div class="customer-email">{{ f.customerEmail }}</div>
              </div>
            </td>
            <td class="tour-name">{{ getTourName(f.tourId) }}</td>
            <td class="stars-cell">{{ getStars(f.rating) }} <span class="rating-num">({{ f.rating }})</span></td>
            <td class="comment-cell">{{ f.comment || '—' }}</td>
            <td>
              <button
                @click="toggleVisibility(f)"
                class="btn-toggle"
                :class="{ visible: f.isVisible, hidden: !f.isVisible }"
                :disabled="actionLoading"
              >
                {{ f.isVisible ? 'Visible 👁️' : 'Hidden 🚫' }}
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="feedbacks.length === 0" class="empty-state">
        <p>No feedback found</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.feedback-page { color: #e0e0e0; }
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

.stars-cell { color: #f5a435; font-size: 16px; font-weight: bold; white-space: nowrap; }
.rating-num { color: #8eafc2; font-size: 12px; margin-left: 4px; font-weight: normal; }
.comment-cell { max-width: 400px; word-break: break-word; color: #d0d0d0; font-style: italic; }

.btn-toggle {
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  color: #fff;
  width: 90px;
}
.btn-toggle.visible {
  background: rgba(46, 125, 79, 0.15);
  border: 1px solid rgba(46, 125, 79, 0.3);
  color: #4caf78;
}
.btn-toggle.hidden {
  background: rgba(220, 53, 69, 0.15);
  border: 1px solid rgba(220, 53, 69, 0.3);
  color: #ff6b6b;
}
.btn-toggle:hover:not(:disabled) {
  opacity: 0.8;
}
.btn-toggle:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.loading { text-align: center; padding: 60px; color: #8eafc2; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(26,139,196,0.2); border-top-color: #1a8bc4; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state { text-align: center; padding: 48px; color: #8eafc2; }
</style>
