<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '../services/api'

interface Tour {
  id: string
  names: Record<string, string>
  descriptions: Record<string, string>
  price: number
  duration: string
  includes: string[]
  imageUrl: string
  emoji: string
  bgGradient: string
  badge: string
  destinationId: string
  categoryId: string
  destination?: {
    id: string
    names: Record<string, string>
  }
  category?: {
    id: string
    names: Record<string, string>
  }
}

interface Destination {
  id: string
  names: Record<string, string>
  flag: string
}

interface Category {
  id: string
  names: Record<string, string>
  icon: string
}

const tours = ref<Tour[]>([])
const destinations = ref<Destination[]>([])
const categories = ref<Category[]>([])
const loading = ref(true)
const actionLoading = ref(false)

const searchQuery = ref('')
const selectedCategoryFilter = ref('all')
const showModal = ref(false)
const isEdit = ref(false)
const activeLang = ref('en')

const form = ref({
  id: '',
  names: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  descriptions: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  price: 0,
  duration: 'fullDay',
  includesInput: '',
  imageUrl: '',
  emoji: '',
  bgGradient: '',
  badge: '',
  destinationId: '',
  categoryId: ''
})

const durations = [
  { value: 'fullDay', label: 'Full Day' },
  { value: 'halfDay', label: 'Half Day' },
  { value: 'twoDays', label: '2 Days' },
  { value: 'fiveDays', label: '5 Days' },
  { value: 'oneDay', label: '1 Day' },
  { value: 'threeHours', label: '3 Hours' },
  { value: 'evening', label: 'Evening' }
]

async function loadData() {
  loading.value = true
  try {
    const [toursRes, destsRes, catsRes] = await Promise.all([
      api.get('/api/content/api/tours'),
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories')
    ])
    tours.value = toursRes.data
    destinations.value = destsRes.data
    categories.value = catsRes.data
  } catch (e) {
    console.error('Failed to load tours dashboard data', e)
  } finally {
    loading.value = false
  }
}

const filteredTours = computed(() => {
  return tours.value.filter(t => {
    const nameMatch = t.names?.en?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                      t.names?.ru?.toLowerCase().includes(searchQuery.value.toLowerCase())
    const catMatch = selectedCategoryFilter.value === 'all' || t.categoryId === selectedCategoryFilter.value
    return nameMatch && catMatch
  })
})

function getDurationLabel(d: string) {
  const match = durations.find(item => item.value === d)
  return match ? match.label : d
}

function openCreateModal() {
  isEdit.value = false
  form.value = {
    id: '',
    names: { en: '', de: '', it: '', fr: '', ru: '' },
    descriptions: { en: '', de: '', it: '', fr: '', ru: '' },
    price: 25,
    duration: 'fullDay',
    includesInput: '',
    imageUrl: 'https://images.unsplash.com/photo-1544551763-46a013bb70d5',
    emoji: '⛵',
    bgGradient: 'linear-gradient(135deg,#063a5c,#1a9b8a)',
    badge: '',
    destinationId: destinations.value[0]?.id || '',
    categoryId: categories.value[0]?.id || ''
  }
  showModal.value = true
}

function openEditModal(tour: Tour) {
  isEdit.value = true
  form.value = {
    id: tour.id,
    names: { ...tour.names },
    descriptions: { ...tour.descriptions },
    price: tour.price,
    duration: tour.duration,
    includesInput: tour.includes ? tour.includes.join(', ') : '',
    imageUrl: tour.imageUrl,
    emoji: tour.emoji,
    bgGradient: tour.bgGradient,
    badge: tour.badge,
    destinationId: tour.destinationId,
    categoryId: tour.categoryId
  }
  showModal.value = true
}

async function saveTour() {
  actionLoading.value = true
  try {
    const payload = {
      names: form.value.names,
      descriptions: form.value.descriptions,
      price: form.value.price,
      duration: form.value.duration,
      includes: form.value.includesInput.split(',').map(s => s.trim()).filter(Boolean),
      imageUrl: form.value.imageUrl,
      emoji: form.value.emoji,
      bgGradient: form.value.bgGradient,
      badge: form.value.badge,
      destinationId: form.value.destinationId,
      categoryId: form.value.categoryId
    }

    if (isEdit.value) {
      await api.put(`/api/content/api/tours/${form.value.id}`, { id: form.value.id, ...payload })
    } else {
      await api.post('/api/content/api/tours', payload)
    }

    showModal.value = false
    await loadData()
  } catch (e) {
    console.error('Failed to save tour', e)
    alert('Failed to save tour. See console for details.')
  } finally {
    actionLoading.value = false
  }
}

async function deleteTour(id: string) {
  if (!confirm('Are you sure you want to delete this tour?')) return
  actionLoading.value = true
  try {
    await api.delete(`/api/content/api/tours/${id}`)
    await loadData()
  } catch (e) {
    console.error('Failed to delete tour', e)
    alert('Failed to delete tour.')
  } finally {
    actionLoading.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <div class="posts-page">
    <div class="page-header">
      <div>
        <h2>Tours Management</h2>
        <p>Manage all tour packages, localizations, categories, and prices.</p>
      </div>
      <button @click="openCreateModal" class="btn-create">+ Add New Tour</button>
    </div>

    <!-- Filters -->
    <div class="filters">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="🔍 Search tours..."
        class="search-input"
      />
      <select v-model="selectedCategoryFilter" class="filter-select">
        <option value="all">All Categories</option>
        <option v-for="cat in categories" :key="cat.id" :value="cat.id">
          {{ cat.icon }} {{ cat.names?.en || 'Unknown' }}
        </option>
      </select>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading tours...</p>
    </div>

    <!-- Tours Table -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Tour</th>
            <th>Destination</th>
            <th>Category</th>
            <th>Duration</th>
            <th>Price</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tour in filteredTours" :key="tour.id">
            <td>
              <div class="tour-cell">
                <span class="tour-emoji">{{ tour.emoji }}</span>
                <div>
                  <div class="tour-name">{{ tour.names?.en || 'Untitled' }}</div>
                  <div class="tour-badge" v-if="tour.badge">{{ tour.badge }}</div>
                </div>
              </div>
            </td>
            <td>{{ tour.destination?.names?.en || '—' }}</td>
            <td>
              <span class="category-badge">{{ tour.category?.names?.en || '—' }}</span>
            </td>
            <td>{{ getDurationLabel(tour.duration) }}</td>
            <td class="price-cell">${{ tour.price }}</td>
            <td>
              <div class="actions">
                <button @click="openEditModal(tour)" class="btn-edit-action" :disabled="actionLoading">✏️</button>
                <button @click="deleteTour(tour.id)" class="btn-delete-action" :disabled="actionLoading">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="filteredTours.length === 0" class="empty-state">
        <p>No tours found</p>
      </div>
    </div>

    <!-- Modal Form -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-card">
        <div class="modal-header">
          <h3>{{ isEdit ? 'Edit Tour' : 'Create Tour' }}</h3>
          <button @click="showModal = false" class="btn-close">✕</button>
        </div>

        <form @submit.prevent="saveTour" class="modal-form">
          <!-- Multi-language tabs -->
          <div class="lang-tabs-wrapper">
            <label>Localized Fields</label>
            <div class="lang-tabs">
              <button
                v-for="lang in ['en', 'de', 'it', 'fr', 'ru']"
                :key="lang"
                type="button"
                :class="{ active: activeLang === lang }"
                @click="activeLang = lang"
              >
                {{ lang.toUpperCase() }}
              </button>
            </div>
            <div class="lang-fields">
              <div class="form-group">
                <input
                  v-model="form.names[activeLang]"
                  type="text"
                  :placeholder="`Tour Name (${activeLang.toUpperCase()})`"
                  required
                />
              </div>
              <div class="form-group">
                <textarea
                  v-model="form.descriptions[activeLang]"
                  rows="3"
                  :placeholder="`Tour Description (${activeLang.toUpperCase()})`"
                  required
                ></textarea>
              </div>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Price ($)</label>
              <input v-model.number="form.price" type="number" min="0" required />
            </div>
            <div class="form-group">
              <label>Duration</label>
              <select v-model="form.duration">
                <option v-for="d in durations" :key="d.value" :value="d.value">
                  {{ d.label }}
                </option>
              </select>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Destination</label>
              <select v-model="form.destinationId" required>
                <option v-for="dest in destinations" :key="dest.id" :value="dest.id">
                  {{ dest.flag }} {{ dest.names?.en || 'Unknown' }}
                </option>
              </select>
            </div>
            <div class="form-group">
              <label>Category</label>
              <select v-model="form.categoryId" required>
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                  {{ cat.icon }} {{ cat.names?.en || 'Unknown' }}
                </option>
              </select>
            </div>
          </div>

          <div class="form-group">
            <label>Includes (comma-separated list)</label>
            <input v-model="form.includesInput" type="text" placeholder="e.g. 🚌 Transfer, 🥗 Lunch, 🤿 Equipment" />
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Image URL</label>
              <input v-model="form.imageUrl" type="text" placeholder="https://..." required />
            </div>
            <div class="form-group">
              <label>Emoji Icon</label>
              <input v-model="form.emoji" type="text" placeholder="e.g. ⛵" required />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Background Gradient</label>
              <input v-model="form.bgGradient" type="text" placeholder="e.g. linear-gradient(...)" required />
            </div>
            <div class="form-group">
              <label>Badge text (optional)</label>
              <input v-model="form.badge" type="text" placeholder="e.g. ⭐ BESTSELLER" />
            </div>
          </div>

          <div class="modal-actions">
            <button type="button" @click="showModal = false" class="btn-cancel" :disabled="actionLoading">Cancel</button>
            <button type="submit" class="btn-save" :disabled="actionLoading">
              {{ actionLoading ? 'Saving...' : 'Save' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.posts-page { color: #e0e0e0; }
.page-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 28px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #fff; margin-bottom: 4px; }
.page-header p { color: #8eafc2; font-size: 14px; }
.btn-create { padding: 12px 24px; background: linear-gradient(135deg, #e8820a, #f5a435); border: none; border-radius: 8px; color: #fff; font-weight: 600; cursor: pointer; font-size: 14px; transition: all 0.3s; }
.btn-create:hover { transform: translateY(-1px); box-shadow: 0 8px 24px rgba(232, 130, 10, 0.3); }

.filters { display: flex; gap: 12px; margin-bottom: 24px; }
.search-input { flex: 1; padding: 12px 16px; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 8px; color: #fff; font-size: 14px; outline: none; }
.search-input:focus { border-color: #1a8bc4; }
.filter-select { padding: 12px 16px; background: #0a1929; border: 1px solid rgba(255,255,255,0.1); border-radius: 8px; color: #fff; font-size: 14px; outline: none; cursor: pointer; }

.table-container { background: rgba(10,25,41,0.6); border: 1px solid rgba(255,255,255,0.06); border-radius: 12px; overflow: hidden; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 14px 20px; text-align: left; font-size: 12px; letter-spacing: 0.1em; text-transform: uppercase; color: #8eafc2; background: rgba(0,0,0,0.2); border-bottom: 1px solid rgba(255,255,255,0.06); }
.data-table td { padding: 16px 20px; border-bottom: 1px solid rgba(255,255,255,0.04); font-size: 14px; }
.data-table tr:hover { background: rgba(255,255,255,0.02); }

.tour-cell { display: flex; align-items: center; gap: 12px; }
.tour-emoji { font-size: 28px; }
.tour-name { font-weight: 600; color: #fff; }
.tour-badge { font-size: 11px; color: #c9a84c; margin-top: 2px; }
.category-badge { padding: 4px 12px; background: rgba(10,92,138,0.2); color: #1a8bc4; border-radius: 20px; font-size: 12px; }
.price-cell { color: #4caf78; font-weight: 600; }

.actions { display: flex; gap: 8px; }
.btn-edit-action, .btn-delete-action { background: none; border: none; cursor: pointer; font-size: 18px; padding: 4px; border-radius: 4px; transition: background 0.2s; }
.btn-edit-action:hover { background: rgba(26,139,196,0.2); }
.btn-delete-action:hover { background: rgba(220,53,69,0.2); }

.loading { text-align: center; padding: 60px; color: #8eafc2; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(26,139,196,0.2); border-top-color: #1a8bc4; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state { text-align: center; padding: 48px; color: #8eafc2; }

/* Modal overlay styles */
.modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 2000; padding: 20px; }
.modal-card { background: #0a1929; border: 1px solid rgba(201,168,76,0.2); border-radius: 12px; width: 100%; max-width: 680px; display: flex; flex-direction: column; overflow: hidden; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 20px; border-bottom: 1px solid rgba(255,255,255,0.06); }
.modal-header h3 { font-size: 18px; font-weight: 600; color: #fff; }
.btn-close { background: none; border: none; color: #8eafc2; cursor: pointer; font-size: 18px; }

.modal-form { padding: 20px; display: flex; flex-direction: column; gap: 16px; max-height: 80vh; overflow-y: auto; }
.form-group { display: flex; flex-direction: column; gap: 6px; }
.form-group label { color: #8eafc2; font-size: 12px; font-weight: 500; }
.form-group input, .form-group select, .form-group textarea {
  padding: 10px 12px; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 6px; color: #fff; outline: none; font-size: 14px;
}
.form-group input:focus, .form-group select:focus, .form-group textarea:focus { border-color: #1a8bc4; }
.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }

.lang-tabs-wrapper { border: 1px solid rgba(255,255,255,0.06); padding: 12px; border-radius: 8px; background: rgba(0,0,0,0.1); }
.lang-tabs { display: flex; gap: 4px; margin-bottom: 12px; }
.lang-tabs button {
  flex: 1; padding: 6px; background: rgba(255,255,255,0.03); border: 1px solid rgba(255,255,255,0.06); border-radius: 4px; color: #8eafc2; cursor: pointer; font-size: 11px;
}
.lang-tabs button.active { background: #1a8bc4; color: #fff; border-color: #1a8bc4; }
.lang-fields { display: flex; flex-direction: column; gap: 12px; }

.modal-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 10px; }
.btn-cancel { padding: 10px 20px; background: none; border: 1px solid rgba(255,255,255,0.1); border-radius: 6px; color: #8eafc2; cursor: pointer; }
.btn-save { padding: 10px 24px; background: #1a8bc4; border: none; border-radius: 6px; color: #fff; font-weight: 600; cursor: pointer; }
.btn-save:hover { background: #1880b4; }
</style>
