<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import ExcelImportExportModal from '@/shared/components/ExcelImportExportModal.vue'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'
import { Plus } from 'lucide-vue-next'

interface Tour {
  id: string
  names: Record<string, string>
  descriptions?: Record<string, string>
  price: number
  currency?: string
  originalPrice?: number
  discountPercentage?: number
  duration: string
  startTime?: string
  maxAllocations?: number
  groupMaxCapacity?: number
  includes?: string[]
  imageUrl?: string
  emoji?: string
  bgGradient?: string
  badge?: string
  destinationId?: string
  categoryId?: string
  tourTypeId?: string
  supplierId?: string
  supplierPercentage?: number
  isTopRated?: boolean
  isBestseller?: boolean
  isInHighDemand?: boolean
  hotelPickup?: boolean
  destinationName?: string
  categoryName?: string
  destination?: {
    id: string
    names?: Record<string, string>
    flagEmoji?: string
    flag?: string
  }
  category?: {
    id: string
    names?: Record<string, string>
    icon?: string
    iconName?: string
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
  flagEmoji?: string
  flag?: string
}

interface Category {
  id: string
  names: Record<string, string>
  icon?: string
  iconName?: string
}

interface TourType {
  id: string
  names: Record<string, string>
  icon?: string
  code?: string
}

const router = useRouter()

const tours = ref<Tour[]>([])
const destinations = ref<Destination[]>([])
const categories = ref<Category[]>([])
const suppliers = ref<Supplier[]>([])
const tourTypes = ref<TourType[]>([])
const loading = ref(true)
const actionLoading = ref(false)

const searchQuery = ref('')
const selectedCategoryFilter = ref('all')
const selectedDestinationFilter = ref('all')
const selectedSupplierFilter = ref('all')
const selectedTourTypeFilter = ref('all')
const isExcelModalOpen = ref(false)

const isAnyFilterActive = computed(() => {
  return searchQuery.value !== '' || 
         selectedCategoryFilter.value !== 'all' || 
         selectedDestinationFilter.value !== 'all' ||
         selectedSupplierFilter.value !== 'all' ||
         selectedTourTypeFilter.value !== 'all'
})

function resetFilters() {
  searchQuery.value = ''
  selectedCategoryFilter.value = 'all'
  selectedDestinationFilter.value = 'all'
  selectedSupplierFilter.value = 'all'
  selectedTourTypeFilter.value = 'all'
}

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
    const [toursRes, destsRes, catsRes, supsRes, typesRes] = await Promise.allSettled([
      api.get('/api/content/api/tours'),
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories'),
      api.get('/api/content/api/suppliers'),
      api.get('/api/content/api/tour-types')
    ])
    
    tours.value = toursRes.status === 'fulfilled' ? (Array.isArray(toursRes.value.data) ? toursRes.value.data : (toursRes.value.data?.items || [])) : []
    destinations.value = destsRes.status === 'fulfilled' ? (Array.isArray(destsRes.value.data) ? destsRes.value.data : (destsRes.value.data?.items || [])) : []
    categories.value = catsRes.status === 'fulfilled' ? (Array.isArray(catsRes.value.data) ? catsRes.value.data : (catsRes.value.data?.items || [])) : []
    suppliers.value = supsRes.status === 'fulfilled' ? (Array.isArray(supsRes.value.data) ? supsRes.value.data : (supsRes.value.data?.items || [])) : []
    tourTypes.value = typesRes.status === 'fulfilled' ? (Array.isArray(typesRes.value.data) ? typesRes.value.data : (typesRes.value.data?.items || [])) : []

  } catch (e) {
    console.error('Failed to load tours dashboard data', e)
  } finally {
    loading.value = false
  }
}

// Lookup Resolvers for Related Data
function getTourDestination(tour: Tour): { name: string, flag: string } {
  if (tour.destination?.names?.en) {
    return {
      name: tour.destination.names.en,
      flag: tour.destination.flagEmoji || tour.destination.flag || '📍'
    }
  }
  if (tour.destinationId) {
    const d = destinations.value.find(x => x.id === tour.destinationId)
    if (d) {
      return {
        name: d.names?.en || 'Destination',
        flag: d.flagEmoji || d.flag || '📍'
      }
    }
  }
  return {
    name: tour.destinationName || '—',
    flag: '📍'
  }
}

function getTourCategory(tour: Tour): string {
  if (tour.category?.names?.en) return tour.category.names.en
  if (tour.categoryId) {
    const c = categories.value.find(x => x.id === tour.categoryId)
    if (c?.names?.en) return c.names.en
  }
  return tour.categoryName || 'General Tour'
}

function getTourTypeName(tour: Tour): string {
  if (tour.tourTypeId) {
    const tt = tourTypes.value.find(x => x.id === tour.tourTypeId)
    if (tt?.names?.en) return tt.names.en
  }
  return 'VIP Experience'
}

function getTourSupplier(tour: Tour): { name: string, percent?: number } {
  if (tour.supplier) {
    return {
      name: tour.supplier.nameEn || tour.supplier.nameAr || 'Partner',
      percent: tour.supplierPercentage
    }
  }
  if (tour.supplierId) {
    const s = suppliers.value.find(x => x.id === tour.supplierId)
    if (s) {
      return {
        name: s.nameEn || s.nameAr || 'Partner',
        percent: tour.supplierPercentage
      }
    }
  }
  return {
    name: 'Direct Operation',
    percent: undefined
  }
}

const filteredTours = computed(() => {
  return tours.value.filter(t => {
    const nameMatch = t.names?.en?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                      t.names?.ru?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                      t.names?.de?.toLowerCase().includes(searchQuery.value.toLowerCase())
    const catMatch = selectedCategoryFilter.value === 'all' || t.categoryId === selectedCategoryFilter.value
    const destMatch = selectedDestinationFilter.value === 'all' || t.destinationId === selectedDestinationFilter.value
    const supMatch = selectedSupplierFilter.value === 'all' || t.supplierId === selectedSupplierFilter.value
    const typeMatch = selectedTourTypeFilter.value === 'all' || t.tourTypeId === selectedTourTypeFilter.value
    return nameMatch && catMatch && destMatch && supMatch && typeMatch
  })
})

const currentPage = ref(1)
const pageSize = ref(10)

const paginatedTours = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredTours.value.slice(start, start + pageSize.value)
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

function generatePDF() {
  const url = `${api.defaults.baseURL || ''}/api/content/api/admin/pdf/catalog`
  window.open(url, '_blank')
  toast.success('Generating PDF', 'Generating luxury QuestPDF catalog.')
}

onMounted(loadData)
</script>

<template>
  <div class="tours-page">
    <div class="page-header">
      <div class="header-content">
        <h2>Tours Management</h2>
        <p>Manage all tour packages, localizations, categories, and prices.</p>
      </div>
      <div class="header-actions">
        <button @click="isExcelModalOpen = true" class="btn-action-secondary">
          <span>📊</span>
          <span>Import / Export</span>
        </button>
        <button @click="generatePDF" class="btn-action-secondary" title="Download Luxury Printable Catalog">
          <span>📄</span>
          <span>Export Catalog (PDF)</span>
        </button>
        <button @click="openCreateModal" class="btn-create">
          <Plus class="w-4 h-4" />
          <span>Add Tour</span>
        </button>
      </div>
    </div>

    <!-- Filter Toolbar -->
    <div class="filter-toolbar flex flex-wrap gap-3">
      <div class="search-bar">
        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="search-icon"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
        <input type="text" v-model="searchQuery" aria-label="Search tours" placeholder="Search tours by name, destination, or keyword..." />
      </div>
      
      <select v-model="selectedDestinationFilter" aria-label="Filter by destination" class="filter-select">
        <option value="all">All Destinations</option>
        <option v-for="dest in destinations" :key="dest.id" :value="dest.id">
          {{ dest.names?.en || 'Unknown' }}
        </option>
      </select>

      <select v-model="selectedCategoryFilter" aria-label="Filter by category" class="filter-select">
        <option value="all">All Categories</option>
        <option v-for="cat in categories" :key="cat.id" :value="cat.id">
          {{ cat.names?.en || 'Unknown' }}
        </option>
      </select>
      
      <select v-model="selectedTourTypeFilter" aria-label="Filter by tour type" class="filter-select">
        <option value="all">All Tour Types</option>
        <option v-for="tt in tourTypes" :key="tt.id" :value="tt.id">
          {{ tt.names?.en || 'Unknown' }}
        </option>
      </select>
      
      <select v-model="selectedSupplierFilter" aria-label="Filter by supplier" class="filter-select">
        <option value="all">All Suppliers</option>
        <option v-for="sup in suppliers" :key="sup.id" :value="sup.id">
          {{ sup.nameEn || sup.nameAr || 'Unknown' }}
        </option>
      </select>

      <button v-if="isAnyFilterActive" @click="resetFilters" class="btn-action-secondary" title="Reset Filters">
        Reset
      </button>
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
            <th>Experience</th>
            <th>Destination</th>
            <th>Category</th>
            <th>Tour Type</th>
            <th>Duration & Timing</th>
            <th>Capacity</th>
            <th>Supplier</th>
            <th>Price</th>
            <th align="right">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tour in paginatedTours" :key="tour.id">
            <!-- Experience / Tour Column -->
            <td>
              <div class="tour-cell">
                <img
                  v-if="tour.imageUrl"
                  :src="tour.imageUrl"
                  class="tour-thumb"
                  alt=""
                  @error="tour.imageUrl = ''"
                />
                <span v-else class="tour-emoji">{{ tour.emoji || '✨' }}</span>
                <div>
                  <div class="tour-name font-bold text-slate-900">{{ tour.names?.en || 'Untitled Tour' }}</div>
                  <div class="flex items-center gap-1.5 mt-1">
                    <span v-if="tour.badge" class="tour-badge">{{ tour.badge }}</span>
                    <span v-if="tour.isBestseller" class="text-[9px] font-bold px-1.5 py-0.5 rounded bg-amber-100 text-amber-800 border border-amber-200">★ Bestseller</span>
                    <span v-if="tour.isTopRated" class="text-[9px] font-bold px-1.5 py-0.5 rounded bg-emerald-100 text-emerald-800 border border-emerald-200">★ Top Rated</span>
                  </div>
                </div>
              </div>
            </td>

            <!-- Destination Column -->
            <td>
              <div class="flex items-center gap-1.5 text-xs font-semibold text-slate-800">
                <span>{{ getTourDestination(tour).flag }}</span>
                <span>{{ getTourDestination(tour).name }}</span>
              </div>
            </td>

            <!-- Category Column -->
            <td>
              <span class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 border border-slate-200">
                {{ getTourCategory(tour) }}
              </span>
            </td>

            <!-- Tour Type Column -->
            <td>
              <span class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-[11px] font-bold bg-navy-950/5 text-primary border border-primary/20">
                {{ getTourTypeName(tour) }}
              </span>
            </td>

            <!-- Duration & Timing Column -->
            <td>
              <div class="text-xs font-medium text-slate-800">{{ getDurationLabel(tour.duration) }}</div>
              <div v-if="tour.startTime" class="text-[10px] text-slate-500 font-mono mt-0.5">Starts {{ tour.startTime }}</div>
            </td>

            <!-- Capacity Column -->
            <td>
              <span class="text-xs font-medium text-slate-700">
                {{ (tour.maxAllocations && tour.maxAllocations > 0) ? `${tour.maxAllocations} Guests` : ((tour.groupMaxCapacity && tour.groupMaxCapacity > 0) ? `${tour.groupMaxCapacity} Guests` : '20 Guests') }}
              </span>
            </td>

            <!-- Supplier Column -->
            <td>
              <div class="text-xs font-medium text-slate-800">
                {{ getTourSupplier(tour).name }}
              </div>
              <div v-if="getTourSupplier(tour).percent" class="text-[10px] text-slate-500 font-mono">
                Share: {{ getTourSupplier(tour).percent }}%
              </div>
            </td>

            <!-- Price Column -->
            <td class="price-cell">
              <div class="font-bold text-sm text-slate-900">
                {{ getCurrencySymbol(tour.currency) }}{{ tour.price }}
              </div>
              <div v-if="tour.originalPrice && tour.originalPrice > tour.price" class="text-[10px] text-slate-400 line-through">
                {{ getCurrencySymbol(tour.currency) }}{{ tour.originalPrice }}
              </div>
            </td>

            <!-- Actions Column -->
            <td>
              <div class="actions justify-end">
                <button @click="openEditModal(tour)" class="btn-action" title="Edit Tour" :disabled="actionLoading">
                  ✏️
                </button>
                <button @click="deleteTour(tour.id)" class="btn-action btn-delete" title="Delete Tour" :disabled="actionLoading">
                  🗑️
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="filteredTours.length === 0" class="empty-state">
        <p>No tours found matching your search.</p>
      </div>

      <LuxuryPagination
        v-if="filteredTours.length > 0"
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="filteredTours.length"
      />
    </div>

    <!-- Excel Import / Export Modal -->
    <ExcelImportExportModal
      v-if="isExcelModalOpen"
      :isOpen="isExcelModalOpen"
      entity="tours"
      entityTitle="Tours Catalog"
      @close="isExcelModalOpen = false"
      @import-complete="loadData"
    />
  </div>
</template>

<style scoped>
.tours-page {
  animation: fadeIn 0.3s ease;
  color: #1e293b;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  gap: 16px;
}

.page-header h2 {
  font-size: 24px;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 4px;
}

.page-header p {
  color: #64748b;
  font-size: 14px;
  margin: 0;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.filter-toolbar {
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 24px;
  padding: 14px 16px;
  background: #f4f6e8;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
}

.search-bar {
  display: flex;
  align-items: center;
  background: #fdfff5;
  border: 1.5px solid #cbd5e1;
  border-radius: 10px;
  padding: 0 16px;
  height: 44px;
  transition: all 0.2s;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
  flex-grow: 1;
  max-width: 520px;
}

.search-bar:focus-within {
  border-color: #0f172a;
  box-shadow: 0 0 0 3px rgba(15, 23, 42, 0.12);
}

.search-icon {
  color: #64748b;
  margin-right: 10px;
  flex-shrink: 0;
}

.search-bar input {
  background: transparent;
  border: none;
  color: #0f172a;
  padding: 0;
  outline: none;
  width: 100%;
  font-size: 14px;
  font-weight: 500;
}

.search-bar input::placeholder {
  color: #94a3b8;
  font-weight: 400;
}

.filter-select {
  height: 44px;
  padding: 0 16px;
  background: #fdfff5;
  border: 1.5px solid #cbd5e1;
  border-radius: 10px;
  color: #334155;
  font-size: 14px;
  font-weight: 500;
  outline: none;
  cursor: pointer;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
  transition: all 0.2s;
  min-width: 180px;
}

.filter-select:focus {
  border-color: #0f172a;
  box-shadow: 0 0 0 3px rgba(15, 23, 42, 0.12);
}

.btn-action-secondary {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  background: #fdfff5;
  border: 1px solid #cbd5e1;
  border-radius: 10px;
  color: #334155;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.btn-action-secondary:hover {
  background: #f4f6e8;
  color: #0f172a;
  border-color: #94a3b8;
  transform: translateY(-1px);
}

.btn-create {
  display: flex;
  align-items: center;
  gap: 8px;
  height: 40px;
  padding: 0 20px;
  background: #0f172a;
  border: none;
  border-radius: 8px;
  color: #ffffff;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 4px 12px rgba(15, 23, 42, 0.2);
  position: relative;
  overflow: hidden;
}

.btn-create::after {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0; height: 1px;
  background: linear-gradient(90deg, transparent, rgba(212, 175, 55, 0.6), transparent);
}

.btn-create:hover {
  background: #1e293b;
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(15, 23, 42, 0.3);
}

.tour-cell {
  display: flex;
  align-items: center;
  gap: 12px;
}

.tour-thumb {
  width: 44px;
  height: 44px;
  border-radius: 10px;
  object-fit: cover;
  border: 1px solid #e2e8f0;
}

.tour-emoji {
  width: 44px;
  height: 44px;
  border-radius: 10px;
  background: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
}

.tour-badge {
  display: inline-block;
  padding: 2px 6px;
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  border-radius: 4px;
  background: #0f172a;
  color: #ffffff;
}

.table-container {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);
  overflow: hidden;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th {
  padding: 12px 16px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #64748b;
  background: #f8fafc;
  border-bottom: 1px solid #e2e8f0;
}

.data-table td {
  padding: 16px;
  font-size: 14px;
  color: #334155;
  border-bottom: 1px solid #f1f5f9;
  vertical-align: middle;
}

.data-table tr:hover {
  background: #f8fafc;
}

.btn-action {
  width: 32px;
  height: 32px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-action:hover {
  background: #f1f5f9;
  color: #0f172a;
  border-color: #cbd5e1;
}

.btn-delete:hover {
  background: #fef2f2;
  color: #ef4444;
  border-color: #fca5a5;
}

.loading, .empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #64748b;
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #e2e8f0;
  border-top-color: #0f172a;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
