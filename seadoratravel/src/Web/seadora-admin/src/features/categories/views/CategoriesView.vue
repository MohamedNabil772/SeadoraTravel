<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'
import CategoryModalForm from '../components/CategoryModalForm.vue'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'
import ExcelImportExportModal from '@/shared/components/ExcelImportExportModal.vue'
import { Plus } from 'lucide-vue-next'

const isExcelModalOpen = ref(false)

interface Category {
  id: string
  names: Record<string, string>
  descriptions?: Record<string, string>
  icon: string | null
  customIconUrl?: string | null
  coverImageUrl?: string | null
  order?: number
  tourCount?: number
}

const categories = ref<Category[]>([])
const loading = ref(true)
const actionLoading = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const selectedCategory = ref<Category | null>(null)
const searchQuery = ref('')

const currentPage = ref(1)
const pageSize = ref(12)

const { confirm } = useConfirm()
const toast = useToast()

// Drag and drop state
const draggedIndex = ref<number | null>(null)
const dragOverIndex = ref<number | null>(null)

const filteredCategories = computed(() => {
  if (!searchQuery.value) return categories.value
  const query = searchQuery.value.toLowerCase()
  return categories.value.filter(cat => 
    cat.names?.en?.toLowerCase().includes(query) || 
    (cat.descriptions?.en && cat.descriptions.en.toLowerCase().includes(query))
  )
})

const paginatedCategories = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredCategories.value.slice(start, start + pageSize.value)
})

async function fetchCategories() {
  loading.value = true
  try {
    const res = await api.get('/api/content/api/categories')
    let items = Array.isArray(res.data) ? res.data : (res.data?.items || [])
    categories.value = items.sort((a: any, b: any) => (a.order || 0) - (b.order || 0))
  } catch (e) {
    console.error('Failed to fetch categories', e)
  } finally {
    loading.value = false
  }
}

function openCreateDrawer() {
  isEdit.value = false
  selectedCategory.value = null
  showModal.value = true
}

function openEditDrawer(cat: Category) {
  isEdit.value = true
  selectedCategory.value = { ...cat }
  showModal.value = true
}

async function saveCategory(formData: any) {
  actionLoading.value = true
  try {
    const payload = {
      names: formData.names,
      descriptions: formData.descriptions,
      icon: formData.icon,
      customIconUrl: formData.customIconUrl,
      coverImageUrl: formData.coverImageUrl,
      order: formData.order !== undefined ? formData.order : categories.value.length
    }

    if (isEdit.value && formData.id) {
      await api.put(`/api/content/api/categories/${formData.id}`, { id: formData.id, ...payload })
      toast.success('Category updated successfully')
    } else {
      await api.post('/api/content/api/categories', payload)
      toast.success('Category created successfully')
    }

    showModal.value = false
    await fetchCategories()
  } catch (e) {
    console.error('Failed to save category', e)
    toast.error('Failed to save category.')
  } finally {
    actionLoading.value = false
  }
}

async function deleteCategory(id: string) {
  const ok = await confirm({
    title: 'Delete Category',
    message: 'Are you sure you want to delete this category? (Note: It must have no associated tours to be deleted)',
    confirmText: 'Delete',
    type: 'danger'
  })
  if (!ok) return

  actionLoading.value = true
  try {
    await api.delete(`/api/content/api/categories/${id}`)
    toast.success('Category deleted successfully')
    await fetchCategories()
  } catch (e: any) {
    console.error('Failed to delete category', e)
    const err = e.response?.data?.error || 'Failed to delete category.'
    toast.error(err)
  } finally {
    actionLoading.value = false
  }
}

// Drag and drop logic
function onDragStart(event: DragEvent, index: number) {
  draggedIndex.value = index
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'move'
    event.dataTransfer.dropEffect = 'move'
    const target = event.target as HTMLElement
    event.dataTransfer.setDragImage(target, 20, 20)
  }
}

function onDragEnter(index: number) {
  if (draggedIndex.value !== null && draggedIndex.value !== index) {
    dragOverIndex.value = index
  }
}

function onDragEnd() {
  draggedIndex.value = null
  dragOverIndex.value = null
}

async function onDrop(_event: DragEvent, index: number) {
  if (draggedIndex.value === null || draggedIndex.value === index) {
    onDragEnd()
    return
  }
  
  const draggedCat = categories.value[draggedIndex.value]
  categories.value.splice(draggedIndex.value, 1)
  categories.value.splice(index, 0, draggedCat)
  
  categories.value.forEach((cat, i) => {
    cat.order = i
  })

  try {
    await api.post('/api/content/api/categories/reorder', { ids: categories.value.map(c => c.id) })
  } catch(e) {
    console.error('Failed to reorder', e)
  }
  
  onDragEnd()
}

onMounted(fetchCategories)
</script>

<template>
  <div class="categories-page">
    <div class="page-header">
      <div class="header-content">
        <h2>Categories</h2>
        <p>Manage tour categories, icons, and localized names.</p>
      </div>
      <div class="header-actions">
        <button @click="isExcelModalOpen = true" class="btn-action-secondary">
          <span>📊</span>
          <span>Import / Export</span>
        </button>
        <button @click="openCreateDrawer" class="btn-create">
          <Plus class="w-4 h-4" />
          <span>Add Category</span>
        </button>
      </div>
    </div>

    <!-- Filter Toolbar -->
    <div class="filter-toolbar">
      <div class="search-bar">
        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="search-icon"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
        <input type="text" v-model="searchQuery" placeholder="Search categories by name, icon, or translation..." />
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading categories...</p>
    </div>

    <div v-else-if="filteredCategories.length === 0" class="empty-state">
      <div class="empty-icon">📂</div>
      <p>No categories found.</p>
      <button @click="openCreateDrawer" class="btn-action-secondary mx-auto mt-4">Create your first category</button>
    </div>

    <div v-else>
      <!-- Table View -->
      <div class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th width="50"></th>
              <th>Icon</th>
              <th>Category Name</th>
              <th>Linked Experiences</th>
              <th align="right">Actions</th>
            </tr>
          </thead>
          <tbody @dragover.prevent>
            <tr 
              v-for="(cat, index) in paginatedCategories" 
              :key="cat.id"
              draggable="true"
              @dragstart="onDragStart($event, index)"
              @dragenter.prevent="onDragEnter(index)"
              @dragend="onDragEnd"
              @drop="onDrop($event, index)"
              :class="{ 'drag-over': dragOverIndex === index, 'dragging': draggedIndex === index }"
            >
              <td class="drag-handle" title="Drag to reorder">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="12" r="1"></circle><circle cx="9" cy="5" r="1"></circle><circle cx="9" cy="19" r="1"></circle><circle cx="15" cy="12" r="1"></circle><circle cx="15" cy="5" r="1"></circle><circle cx="15" cy="19" r="1"></circle></svg>
              </td>
              <td class="icon-cell">
                <div class="icon-wrapper">
                  <template v-if="cat.customIconUrl">
                    <img :src="cat.customIconUrl" alt="Icon" style="max-width:24px; max-height:24px; object-fit:contain;" />
                  </template>
                  <template v-else-if="cat.icon">
                    <img v-if="cat.icon.startsWith('data:') || cat.icon.startsWith('http')" :src="cat.icon" alt="icon" style="max-width:24px; max-height:24px; object-fit:contain;" />
                    <span v-else>{{ cat.icon }}</span>
                  </template>
                  <span v-else class="text-slate-400" style="font-size: 14px;">—</span>
                </div>
              </td>
              <td class="name-cell">{{ cat.names?.en || 'Untitled Category' }}</td>
              <td>
                <span class="inline-flex items-center gap-1.5 px-2.5 py-1 text-xs font-semibold rounded-full bg-slate-100 text-slate-800 border border-slate-200">
                  <span>✦</span> {{ cat.tourCount || 0 }} {{ (cat.tourCount === 1) ? 'Tour' : 'Tours' }}
                </span>
              </td>
              <td>
                <div class="actions justify-end">
                  <button @click="openEditDrawer(cat)" class="btn-action" title="Edit" :disabled="actionLoading">
                    ✏️
                  </button>
                  <button @click="deleteCategory(cat.id)" class="btn-action btn-delete" title="Delete" :disabled="actionLoading">
                    🗑️
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Luxury Pagination Component -->
      <LuxuryPagination
        v-if="filteredCategories.length > 0"
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="filteredCategories.length"
      />
    </div>

    <!-- Modal Form -->
    <CategoryModalForm
      v-model="showModal"
      :isEdit="isEdit"
      :category="selectedCategory"
      :actionLoading="actionLoading"
      @save="saveCategory"
    />

    <!-- Excel & PDF Tools Modal -->
    <ExcelImportExportModal
      :is-open="isExcelModalOpen"
      entity="categories"
      entity-title="Categories"
      @close="isExcelModalOpen = false"
      @import-complete="fetchCategories"
    />
  </div>
</template>

<style scoped>
.categories-page {
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

.data-table tr {
  transition: background 0.2s, transform 0.2s;
}

.data-table tr:hover {
  background: #f8fafc;
}

.data-table tr.drag-over {
  border-top: 2px solid #0f172a;
  background: #f1f5f9;
}

.data-table tr.dragging {
  opacity: 0.5;
  background: #f8fafc;
}

.drag-handle {
  cursor: grab;
  color: #94a3b8;
  text-align: center;
}

.drag-handle:active {
  cursor: grabbing;
}

.drag-handle:hover svg {
  color: #64748b;
}

.icon-wrapper {
  width: 40px;
  height: 40px;
  background: #f1f5f9;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
}

.name-cell {
  font-weight: 600;
  color: #0f172a;
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
