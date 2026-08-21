<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import ExcelImportModal from '../components/ExcelImportModal.vue'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'

interface Tour {
  id: string
  names: Record<string, string>
  descriptions: Record<string, string>
  price: number
  currency?: string
  duration: string
  includes: string[]
  imageUrl: string
  emoji: string
  bgGradient: string
  badge: string
  destinationId: string
  categoryId: string
  supplierId?: string
  supplierPercentage?: number
  destination?: {
    id: string
    names: Record<string, string>
  }
  category?: {
    id: string
    names: Record<string, string>
  }
  supplier?: {
    id: string
    nameEn?: string
    nameAr: string
  }
}

interface Supplier {
  id: string
  nameEn?: string
  nameAr: string
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

const router = useRouter()

const tours = ref<Tour[]>([])
const destinations = ref<Destination[]>([])
const categories = ref<Category[]>([])
const suppliers = ref<Supplier[]>([])
const loading = ref(true)
const actionLoading = ref(false)

const searchQuery = ref('')
const selectedCategoryFilter = ref('all')
const isExcelModalOpen = ref(false)

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
    const [toursRes, destsRes, catsRes, supsRes] = await Promise.allSettled([
      api.get('/api/content/api/tours'),
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories'),
      api.get('/api/content/api/suppliers')
    ])
    
    tours.value = toursRes.status === 'fulfilled' ? (Array.isArray(toursRes.value.data) ? toursRes.value.data : (toursRes.value.data?.items || [])) : []
    destinations.value = destsRes.status === 'fulfilled' ? (Array.isArray(destsRes.value.data) ? destsRes.value.data : (destsRes.value.data?.items || [])) : []
    categories.value = catsRes.status === 'fulfilled' ? (Array.isArray(catsRes.value.data) ? catsRes.value.data : (catsRes.value.data?.items || [])) : []
    suppliers.value = supsRes.status === 'fulfilled' ? (Array.isArray(supsRes.value.data) ? supsRes.value.data : (supsRes.value.data?.items || [])) : []

    if (toursRes.status === 'rejected' || destsRes.status === 'rejected' || catsRes.status === 'rejected' || supsRes.status === 'rejected') {
      console.warn('Some endpoints failed to load, returning partial data.')
    }
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

const { confirm } = useConfirm()
const toast = useToast()

function openCreateModal() {
  router.push('/tours/create')
}

function openEditModal(tour: Tour) {
  router.push(`/tours/${tour.id}/edit`)
}

async function deleteTour(id: string) {
  const ok = await confirm({
    title: 'Delete Tour',
    message: 'Are you sure you want to delete this tour?',
    confirmText: 'Delete',
    type: 'danger'
  })
  if (!ok) return

  actionLoading.value = true
  try {
    await api.delete(`/api/content/api/tours/${id}`)
    toast.success('Tour deleted successfully')
    await loadData()
  } catch (e) {
    console.error('Failed to delete tour', e)
    toast.error('Failed to delete tour.')
  } finally {
    actionLoading.value = false
  }
}

function getCurrencySymbol(curr?: string) {
  if (curr === 'USD') return '$'
  if (curr === 'EGP') return 'EGP '
  return '€'
}

function handleExcelImport(file: File) {
  console.log('Importing file:', file.name)
  // Add implementation here later
}

function downloadTemplate() {
  console.log('Downloading template...')
}

function exportTranslations() {
  console.log('Exporting translations...')
}

function generatePDF() {
  console.log('Generating PDF...')
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
      <div class="header-actions">
        <button @click="downloadTemplate" class="btn-secondary">📄 Template</button>
        <button @click="isExcelModalOpen = true" class="btn-secondary">⬇️ Import</button>
        <button @click="exportTranslations" class="btn-secondary">⬆️ Export</button>
        <button @click="generatePDF" class="btn-secondary">📄 PDF Brochure</button>
        <button @click="openCreateModal" class="btn-create">+ Add New Tour</button>
      </div>
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
            <th>Supplier</th>
            <th>Price</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tour in filteredTours" :key="tour.id">
            <td>
              <div class="tour-cell">
                <img v-if="tour.imageUrl" :src="tour.imageUrl" class="tour-thumb" alt="" />
                <span v-else class="tour-emoji">{{ tour.emoji }}</span>
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
            <td>
              <div v-if="tour.supplier" style="font-size: 12px; color: #8eafc2;">
                {{ tour.supplier.nameEn || tour.supplier.nameAr }} ({{ tour.supplierPercentage }}%)
              </div>
              <div v-else style="font-size: 12px; color: rgba(255,255,255,0.3);">—</div>
            </td>
            <td class="price-cell">{{ getCurrencySymbol(tour.currency) }}{{ tour.price }}</td>
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

    <!-- Excel Import Modal -->
    <ExcelImportModal 
      :is-open="isExcelModalOpen" 
      @close="isExcelModalOpen = false"
      @import="handleExcelImport"
    />
  </div>
</template>

<style scoped>
.posts-page { color: #24303F; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #1C2434; margin-bottom: 4px; }
.page-header p { color: #64748B; font-size: 14px; }

.header-actions { display: flex; gap: 12px; align-items: center; }
.btn-create { padding: 10px 22px; background: #3C50E0; border: none; border-radius: 4px; color: #fff; font-weight: 600; cursor: pointer; font-size: 14px; transition: all 0.3s; }
.btn-create:hover { background: #2B3CA6; }
.btn-create:active { transform: scale(0.97); }
.btn-secondary { padding: 10px 16px; background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; color: #1C2434; font-weight: 600; cursor: pointer; font-size: 14px; transition: all 0.2s; box-shadow: 0 1px 2px rgba(0,0,0,0.05); }
.btn-secondary:hover { background: #F7F9FC; border-color: #CBD5E1; }
.btn-secondary:active { transform: scale(0.97); }

.filters { display: flex; gap: 12px; margin-bottom: 24px; }
.search-input { flex: 1; padding: 12px 16px; background: #fff; border: 1.5px solid #E2E8F0; border-radius: 4px; color: #24303F; font-size: 14px; outline: none; transition: border-color 0.2s; }
.search-input:focus { border-color: #3C50E0; }
.filter-select { padding: 12px 16px; background: #fff; border: 1.5px solid #E2E8F0; border-radius: 4px; color: #24303F; font-size: 14px; outline: none; cursor: pointer; transition: border-color 0.2s; }
.filter-select:focus { border-color: #3C50E0; }

.table-container { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; overflow: hidden; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 16px 24px; text-align: left; font-size: 12px; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748B; background: #F7F9FC; border-bottom: 1px solid #E2E8F0; }
.data-table td { padding: 16px 24px; border-bottom: 1px solid #E2E8F0; font-size: 14px; color: #24303F; }
.data-table tr:hover { background: #F9FAFB; }

.tour-cell { display: flex; align-items: center; gap: 12px; }
.tour-thumb { width: 44px; height: 44px; border-radius: 6px; object-fit: cover; border: 1px solid #E2E8F0; }
.tour-emoji { font-size: 28px; }
.tour-name { font-weight: 600; color: #1C2434; }
.tour-badge { font-size: 11px; color: #e8820a; margin-top: 2px; }
.category-badge { padding: 4px 12px; background: rgba(60, 80, 224, 0.08); color: #3C50E0; border-radius: 4px; font-size: 12px; font-weight: 600; }
.price-cell { color: #10B981; font-weight: 600; }

.actions { display: flex; gap: 8px; }
.btn-edit-action, .btn-delete-action { background: none; border: none; cursor: pointer; font-size: 18px; padding: 4px; border-radius: 4px; transition: background 0.2s; }
.btn-edit-action:hover { background: #EFF4FB; }
.btn-delete-action:hover { background: rgba(211, 64, 83, 0.1); }

.loading { text-align: center; padding: 60px; color: #64748B; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(60, 80, 224, 0.1); border-top-color: #3C50E0; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state { text-align: center; padding: 48px; color: #64748B; }

/* Modal overlay styles */
.modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 2000; padding: 20px; }
.modal-card { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); width: 100%; max-width: 680px; display: flex; flex-direction: column; overflow: hidden; color: #24303F; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 20px; border-bottom: 1px solid #E2E8F0; }
.modal-header h3 { font-size: 18px; font-weight: 700; color: #1C2434; }
.btn-close { background: none; border: none; color: #8A99AD; cursor: pointer; font-size: 18px; }

.modal-form { padding: 20px; display: flex; flex-direction: column; gap: 16px; max-height: 80vh; overflow-y: auto; }
.form-group { display: flex; flex-direction: column; gap: 6px; }
.form-group label { color: #64748B; font-size: 12px; font-weight: 600; text-transform: uppercase; }
.form-group input, .form-group select, .form-group textarea {
  padding: 12px 14px; background: #FFFFFF; border: 1.5px solid #E2E8F0; border-radius: 4px; color: #24303F; outline: none; font-size: 14px; transition: border-color 0.2s;
}
.form-group input:focus, .form-group select:focus, .form-group textarea:focus { border-color: #3C50E0; }
.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }

.lang-tabs-wrapper { border: 1px solid #E2E8F0; padding: 12px; border-radius: 4px; background: #F7F9FC; }
.lang-tabs { display: flex; gap: 4px; margin-bottom: 12px; }
.lang-tabs button {
  flex: 1; padding: 8px; background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; color: #64748B; cursor: pointer; font-size: 11px; font-weight: 600;
}
.lang-tabs button.active { background: #3C50E0; color: #fff; border-color: #3C50E0; }
.lang-fields { display: flex; flex-direction: column; gap: 12px; }

.modal-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 10px; }
.btn-cancel { padding: 10px 22px; background: #fff; border: 1px solid #E2E8F0; border-radius: 4px; color: #64748B; cursor: pointer; font-weight: 600; }
.btn-cancel:hover { background: #F7F9FC; }
.btn-save { padding: 10px 24px; background: #3C50E0; border: none; border-radius: 4px; color: #fff; font-weight: 600; cursor: pointer; }
.btn-save:hover { background: #2B3CA6; }
</style>
