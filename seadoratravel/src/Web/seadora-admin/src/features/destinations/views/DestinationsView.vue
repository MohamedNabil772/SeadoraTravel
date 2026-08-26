<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'
import DestinationModalForm from '../components/DestinationModalForm.vue'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'
import ExcelImportExportModal from '@/shared/components/ExcelImportExportModal.vue'
import { Plus } from 'lucide-vue-next'

const isExcelModalOpen = ref(false)

interface Destination {
  id: string
  names: Record<string, string>
  descriptions: Record<string, string>
  highlights?: Record<string, string>
  imageUrl: string
  flag?: string
  flagEmoji?: string
  isFeatured?: boolean
  toursCount?: number
}

const destinations = ref<Destination[]>([])
const loading = ref(true)
const actionLoading = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const selectedDestination = ref<Destination | null>(null)
const searchQuery = ref('')

const currentPage = ref(1)
const pageSize = ref(12)

const { confirm } = useConfirm()
const toast = useToast()

const defaultFallbackImg = 'https://images.unsplash.com/photo-1506929562872-bb421503ef21?auto=format&fit=crop&w=800&q=80'

function resolveImageUrl(url?: string): string {
  if (!url) return defaultFallbackImg
  if (url.startsWith('http://') || url.startsWith('https://')) return url
  const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
  if (url.startsWith('/api/files/') || url.startsWith('api/files/')) {
    const cleanPath = url.startsWith('/') ? url : `/${url}`
    return `${API_URL}${cleanPath}`
  }
  return url
}

function handleImageError(event: Event) {
  const target = event.target as HTMLImageElement
  if (target && target.src !== defaultFallbackImg) {
    target.src = defaultFallbackImg
  }
}

const filteredDestinations = computed(() => {
  if (!searchQuery.value) return destinations.value
  const query = searchQuery.value.toLowerCase()
  return destinations.value.filter(dest => 
    dest.names?.en?.toLowerCase().includes(query) || 
    dest.descriptions?.en?.toLowerCase().includes(query)
  )
})

const paginatedDestinations = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredDestinations.value.slice(start, start + pageSize.value)
})

async function fetchDestinations() {
  loading.value = true
  try {
    const res = await api.get('/api/content/api/destinations')
    destinations.value = Array.isArray(res.data) ? res.data : (res.data?.items || [])
  } catch (e) {
    console.error('Failed to fetch destinations', e)
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  isEdit.value = false
  selectedDestination.value = null
  showModal.value = true
}

function openEditModal(dest: Destination) {
  isEdit.value = true
  selectedDestination.value = { ...dest }
  showModal.value = true
}

async function saveDestination(formData: any) {
  actionLoading.value = true
  try {
    const payload = {
      names: formData.names,
      descriptions: formData.descriptions,
      highlights: formData.highlights,
      imageUrl: formData.imageUrl,
      flagEmoji: formData.flag || formData.flagEmoji,
      isFeatured: formData.isFeatured
    }

    if (isEdit.value && formData.id) {
      await api.put(`/api/content/api/destinations/${formData.id}`, { id: formData.id, ...payload })
      toast.success('Destination updated successfully')
    } else {
      await api.post('/api/content/api/destinations', payload)
      toast.success('Destination created successfully')
    }

    showModal.value = false
    await fetchDestinations()
  } catch (e) {
    console.error('Failed to save destination', e)
    toast.error('Failed to save destination.')
  } finally {
    actionLoading.value = false
  }
}

async function deleteDestination(id: string) {
  const ok = await confirm({
    title: 'Delete Destination',
    message: 'Are you sure you want to delete this destination? (Note: It must have no associated tours to be deleted)',
    confirmText: 'Delete',
    type: 'danger'
  })
  if (!ok) return

  actionLoading.value = true
  try {
    await api.delete(`/api/content/api/destinations/${id}`)
    toast.success('Destination deleted successfully')
    await fetchDestinations()
  } catch (e: any) {
    console.error('Failed to delete destination', e)
    const err = e.response?.data?.error || 'Failed to delete destination.'
    toast.error(err)
  } finally {
    actionLoading.value = false
  }
}

onMounted(fetchDestinations)
</script>

<template>
  <div class="destinations-page">
    <div class="page-header">
      <div class="header-content">
        <h2>Destinations</h2>
        <p>Manage travel areas, regions, default images, and translations.</p>
      </div>
      <div class="header-actions">
        <button @click="isExcelModalOpen = true" class="btn-action-secondary">
          <span>📊</span>
          <span>Import / Export</span>
        </button>
        <button @click="openCreateModal" class="btn-create">
          <Plus class="w-4 h-4" />
          <span>Add Destination</span>
        </button>
      </div>
    </div>

    <!-- Filter Toolbar -->
    <div class="filter-toolbar">
      <div class="search-bar">
        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="search-icon"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
        <input type="text" v-model="searchQuery" placeholder="Search destinations by region, country, or title..." />
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading destinations...</p>
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredDestinations.length === 0" class="empty-state">
      <div class="empty-icon">🌍</div>
      <p v-if="searchQuery">No destinations match your search.</p>
      <p v-else>No destinations found.</p>
      <button v-if="!searchQuery" @click="openCreateModal" class="btn-action-secondary mx-auto mt-4">Add your first destination</button>
    </div>

    <!-- Table View -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th width="80">Image</th>
            <th>Name (EN)</th>
            <th>Description (EN)</th>
            <th>Highlights & Tags</th>
            <th>Linked Tours</th>
            <th>Featured</th>
            <th align="right">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="dest in paginatedDestinations" :key="dest.id">
            <td>
              <div class="thumb-wrapper">
                <img :src="resolveImageUrl(dest.imageUrl)" class="thumb-image" @error="handleImageError" />
                <span class="thumb-flag">{{ dest.flagEmoji || dest.flag || '📍' }}</span>
              </div>
            </td>
            <td class="name-cell">{{ dest.names?.en || 'Untitled' }}</td>
            <td class="desc-cell">{{ dest.descriptions?.en || '—' }}</td>
            <td>
              <div v-if="dest.highlights?.en" class="flex flex-wrap gap-1 max-w-[220px]">
                <span 
                  v-for="(tag, idx) in dest.highlights.en.split(',').slice(0, 3)" 
                  :key="idx"
                  class="px-2 py-0.5 rounded-md text-[10px] font-medium bg-slate-100 text-slate-700 border border-slate-200"
                >
                  {{ tag.trim() }}
                </span>
                <span 
                  v-if="dest.highlights.en.split(',').length > 3"
                  class="px-1.5 py-0.5 rounded-md text-[10px] font-bold bg-slate-200 text-slate-600"
                >
                  +{{ dest.highlights.en.split(',').length - 3 }}
                </span>
              </div>
              <span v-else class="text-slate-400 text-xs">—</span>
            </td>
            <td>
              <span class="inline-flex items-center gap-1.5 px-2.5 py-1 text-xs font-semibold rounded-full bg-slate-100 text-slate-800 border border-slate-200">
                <span>✦</span> {{ dest.toursCount || 0 }} {{ (dest.toursCount === 1) ? 'Tour' : 'Tours' }}
              </span>
            </td>
            <td>
              <span v-if="dest.isFeatured" class="badge-success">Yes</span>
              <span v-else class="text-slate-400">—</span>
            </td>
            <td>
              <div class="actions justify-end">
                <button @click="openEditModal(dest)" class="btn-action" title="Edit" :disabled="actionLoading">
                  ✏️
                </button>
                <button @click="deleteDestination(dest.id)" class="btn-action btn-delete" title="Delete" :disabled="actionLoading">
                  🗑️
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <LuxuryPagination
        v-if="filteredDestinations.length > 0"
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="filteredDestinations.length"
      />
    </div>

    <!-- Modal Form -->
    <DestinationModalForm
      v-model="showModal"
      :isEdit="isEdit"
      :destination="selectedDestination"
      :actionLoading="actionLoading"
      @save="saveDestination"
    />

    <!-- Excel & PDF Tools Modal -->
    <ExcelImportExportModal
      :is-open="isExcelModalOpen"
      entity="destinations"
      entity-title="Destinations"
      @close="isExcelModalOpen = false"
      @import-complete="fetchDestinations"
    />
  </div>
</template>

<style scoped>
.destinations-page {
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

.btn-action-secondary {
  display: flex;
  align-items: center;
  gap: 8px;
  height: 40px;
  padding: 0 16px;
  background: #fdfff5;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  color: #334155;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.btn-action-secondary:hover {
  background: #f4f6e8;
  border-color: #94a3b8;
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

.thumb-wrapper {
  position: relative;
  width: 64px;
  height: 48px;
  border-radius: 8px;
  overflow: hidden;
  background: #f1f5f9;
  border: 1px solid #e2e8f0;
}

.thumb-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumb-flag {
  position: absolute;
  bottom: 2px;
  right: 2px;
  font-size: 14px;
  background: #ffffff;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.name-cell {
  font-weight: 600;
  color: #0f172a;
}

.desc-cell {
  max-width: 250px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  color: #64748b;
}

.badge-success {
  background: #dcfce7;
  color: #166534;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 600;
}

.actions {
  display: flex;
  gap: 8px;
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

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}
</style>
