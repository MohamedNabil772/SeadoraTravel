<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'
import CategoryModalForm from '../components/CategoryModalForm.vue'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'

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
const viewMode = ref<'grid' | 'table'>('grid')
const searchQuery = ref('')

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
      <div>
        <h2>Categories</h2>
        <p>Manage tour categories, icons, and localized names.</p>
      </div>
      <div class="header-actions">
        <div class="search-box">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
          <input type="text" v-model="searchQuery" placeholder="Search categories..." />
        </div>
        <div class="view-toggles">
          <button class="btn-icon" :class="{ active: viewMode === 'grid' }" @click="viewMode = 'grid'" title="Grid View">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="7" height="7"></rect><rect x="14" y="3" width="7" height="7"></rect><rect x="14" y="14" width="7" height="7"></rect><rect x="3" y="14" width="7" height="7"></rect></svg>
          </button>
          <button class="btn-icon" :class="{ active: viewMode === 'table' }" @click="viewMode = 'table'" title="Table View">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="8" y1="6" x2="21" y2="6"></line><line x1="8" y1="12" x2="21" y2="12"></line><line x1="8" y1="18" x2="21" y2="18"></line><line x1="3" y1="6" x2="3.01" y2="6"></line><line x1="3" y1="12" x2="3.01" y2="12"></line><line x1="3" y1="18" x2="3.01" y2="18"></line></svg>
          </button>
        </div>
        <button @click="openCreateDrawer" class="btn-create">+ Add Category</button>
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
      <button @click="openCreateDrawer" class="btn-ghost">Create your first category</button>
    </div>

    <div v-else>
      <!-- Grid View -->
      <div v-if="viewMode === 'grid'" class="grid-container">
        <div 
          v-for="cat in filteredCategories" 
          :key="cat.id" 
          class="category-card"
        >
          <div class="card-cover">
            <img v-if="cat.coverImageUrl" :src="cat.coverImageUrl" alt="Cover" />
            <div v-else class="card-cover-placeholder"></div>
            <div class="card-actions">
              <button @click="openEditDrawer(cat)" class="btn-action" title="Edit">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20h9"></path><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"></path></svg>
              </button>
              <button @click="deleteCategory(cat.id)" class="btn-action btn-delete" title="Delete">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
              </button>
            </div>
            <div class="card-icon">
              <template v-if="cat.customIconUrl">
                <img :src="cat.customIconUrl" alt="Icon" />
              </template>
              <template v-else-if="cat.icon">
                <img v-if="cat.icon.startsWith('data:') || cat.icon.startsWith('http')" :src="cat.icon" alt="icon" />
                <span v-else>{{ cat.icon }}</span>
              </template>
              <span v-else>—</span>
            </div>
          </div>
          <div class="card-content">
            <h3 class="card-title">{{ cat.names?.en || 'Untitled' }}</h3>
            <div class="card-langs">
              <span v-if="cat.names?.de" class="lang-tag">DE</span>
              <span v-if="cat.names?.it" class="lang-tag">IT</span>
              <span v-if="cat.names?.fr" class="lang-tag">FR</span>
              <span v-if="cat.names?.ru" class="lang-tag">RU</span>
            </div>
            <div class="card-meta">
              <span>{{ cat.tourCount || 0 }} Tours</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Table View -->
      <div v-else class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th width="50"></th>
              <th>Icon</th>
              <th>Name (EN)</th>
              <th>Name (RU)</th>
              <th>Name (DE)</th>
              <th>Tours</th>
              <th align="right">Actions</th>
            </tr>
          </thead>
          <tbody @dragover.prevent>
            <tr 
              v-for="(cat, index) in filteredCategories" 
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
                  <span v-else class="text-muted" style="font-size: 14px;">—</span>
                </div>
              </td>
              <td class="name-cell">{{ cat.names?.en || 'Untitled' }}</td>
              <td class="text-muted">{{ cat.names?.ru || '—' }}</td>
              <td class="text-muted">{{ cat.names?.de || '—' }}</td>
              <td class="text-muted">{{ cat.tourCount || 0 }}</td>
              <td>
                <div class="actions">
                  <button @click="openEditDrawer(cat)" class="btn-action" title="Edit" :disabled="actionLoading">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20h9"></path><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"></path></svg>
                  </button>
                  <button @click="deleteCategory(cat.id)" class="btn-action btn-delete" title="Delete" :disabled="actionLoading">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Form -->
    <CategoryModalForm
      v-model="showModal"
      :isEdit="isEdit"
      :category="selectedCategory"
      :actionLoading="actionLoading"
      @save="saveCategory"
    />
  </div>
</template>

<style scoped>
.categories-page { color: #e0e0e0; animation: fadeIn 0.4s ease; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }

.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 28px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #fff; margin-bottom: 4px; letter-spacing: -0.02em; }
.page-header p { color: #8eafc2; font-size: 14px; margin: 0; }

.header-actions { display: flex; align-items: center; gap: 16px; }

.search-box {
  display: flex;
  align-items: center;
  background: rgba(0,0,0,0.2);
  border: 1px solid rgba(255,255,255,0.1);
  border-radius: 8px;
  padding: 0 12px;
  height: 40px;
}
.search-box svg { color: #8eafc2; margin-right: 8px; }
.search-box input {
  background: transparent;
  border: none;
  color: #fff;
  font-size: 14px;
  outline: none;
  width: 200px;
}
.search-box input::placeholder { color: #5c7585; }

.view-toggles {
  display: flex;
  background: rgba(0,0,0,0.2);
  border-radius: 8px;
  padding: 4px;
  border: 1px solid rgba(255,255,255,0.1);
}
.btn-icon {
  background: transparent;
  border: none;
  color: #5c7585;
  width: 30px;
  height: 30px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
}
.btn-icon:hover { color: #fff; }
.btn-icon.active { background: rgba(255,255,255,0.1); color: #fff; }

.btn-create { 
  padding: 0 20px; 
  height: 40px;
  background: linear-gradient(135deg, #e8820a, #f5a435); 
  border: none; 
  border-radius: 8px; 
  color: #fff; 
  font-weight: 600; 
  cursor: pointer; 
  font-size: 14px; 
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1); 
  box-shadow: 0 4px 12px rgba(232, 130, 10, 0.2);
  will-change: transform;
}
.btn-create:hover { transform: translateY(-1px); box-shadow: 0 8px 24px rgba(232, 130, 10, 0.3); }
.btn-create:active { transform: scale(0.97); box-shadow: 0 2px 8px rgba(232, 130, 10, 0.2); }

/* Grid View */
.grid-container {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 24px;
}
.category-card {
  background: rgba(10,25,41,0.6);
  border: 1px solid rgba(255,255,255,0.06);
  border-radius: 16px;
  overflow: hidden;
  transition: all 0.3s ease;
  box-shadow: 0 8px 32px rgba(0,0,0,0.15);
}
.category-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 48px rgba(0,0,0,0.3);
  border-color: rgba(255,255,255,0.15);
}
.card-cover {
  position: relative;
  height: 140px;
  background: rgba(0,0,0,0.3);
}
.card-cover img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.card-cover-placeholder {
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, rgba(255,255,255,0.05), rgba(255,255,255,0.01));
}
.card-actions {
  position: absolute;
  top: 12px;
  right: 12px;
  display: flex;
  gap: 8px;
  opacity: 0;
  transform: translateY(-10px);
  transition: all 0.2s ease;
}
.category-card:hover .card-actions {
  opacity: 1;
  transform: translateY(0);
}
.card-actions .btn-action {
  background: rgba(0,0,0,0.6);
  backdrop-filter: blur(4px);
}
.card-icon {
  position: absolute;
  bottom: -20px;
  left: 20px;
  width: 48px;
  height: 48px;
  background: #0f2133;
  border: 2px solid rgba(255,255,255,0.1);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.3);
}
.card-icon img {
  width: 28px;
  height: 28px;
  object-fit: contain;
}
.card-content {
  padding: 32px 20px 20px;
}
.card-title {
  margin: 0 0 12px 0;
  font-size: 18px;
  font-weight: 600;
  color: #fff;
}
.card-langs {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-bottom: 16px;
}
.lang-tag {
  font-size: 10px;
  font-weight: 600;
  padding: 2px 6px;
  border-radius: 4px;
  background: rgba(255,255,255,0.1);
  color: #8eafc2;
}
.card-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 13px;
  color: #5c7585;
}

/* Table View */
.table-container { 
  background: rgba(10,25,41,0.6); 
  border: 1px solid rgba(255,255,255,0.06); 
  border-radius: 12px; 
  overflow: hidden; 
  box-shadow: 0 8px 32px rgba(0,0,0,0.2);
}
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { 
  padding: 16px 20px; 
  text-align: left; 
  font-size: 12px; 
  letter-spacing: 0.1em; 
  text-transform: uppercase; 
  color: #8eafc2; 
  background: rgba(0,0,0,0.2); 
  border-bottom: 1px solid rgba(255,255,255,0.06); 
  font-weight: 600;
}
.data-table th[align="right"] { text-align: right; }
.data-table td { 
  padding: 16px 20px; 
  border-bottom: 1px solid rgba(255,255,255,0.03); 
  font-size: 14px; 
  vertical-align: middle;
}
.data-table tr {
  transition: background 0.2s, transform 0.2s;
}
.data-table tr:hover { background: rgba(255,255,255,0.03); }
.data-table tr.drag-over {
  border-top: 2px solid #1a8bc4;
  background: rgba(26,139,196,0.1);
}
.data-table tr.dragging {
  opacity: 0.5;
  background: rgba(255,255,255,0.05);
}

.drag-handle { 
  cursor: grab; 
  color: #5c7585; 
  text-align: center;
}
.drag-handle:active { cursor: grabbing; }
.drag-handle svg { transition: color 0.2s; }
.drag-handle:hover svg { color: #8eafc2; }

.icon-wrapper {
  width: 40px;
  height: 40px;
  background: rgba(0,0,0,0.2);
  border: 1px solid rgba(255,255,255,0.05);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
}

.name-cell { font-weight: 600; color: #fff; font-size: 15px; }
.text-muted { color: #8eafc2; }

.actions { display: flex; gap: 8px; justify-content: flex-end; }
.btn-action { 
  background: rgba(255,255,255,0.05); 
  border: 1px solid rgba(255,255,255,0.1); 
  color: #e0e0e0;
  cursor: pointer; 
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 6px; 
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1); 
  will-change: transform;
}
.btn-action:hover { background: rgba(26,139,196,0.2); border-color: rgba(26,139,196,0.4); color: #fff; transform: translateY(-1px); }
.btn-action:active { transform: scale(0.95); }
.btn-delete:hover { background: rgba(220,53,69,0.2); border-color: rgba(220,53,69,0.4); color: #ff6b6b; transform: translateY(-1px); }

.loading { display: flex; flex-direction: column; align-items: center; justify-content: center; padding: 100px 0; color: #8eafc2; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(26,139,196,0.2); border-top-color: #1a8bc4; border-radius: 50%; animation: spin 0.8s linear infinite; margin-bottom: 16px; }
@keyframes spin { to { transform: rotate(360deg); } }

.empty-state { text-align: center; padding: 64px 20px; color: #8eafc2; }
.empty-icon { font-size: 48px; margin-bottom: 16px; opacity: 0.5; }
.empty-state p { margin-bottom: 24px; font-size: 16px; }
.btn-ghost {
  background: transparent;
  border: 1px solid rgba(255,255,255,0.2);
  color: #fff;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}
.btn-ghost:hover { background: rgba(255,255,255,0.05); }
</style>
