<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import CategoryDrawerForm from '../components/CategoryDrawerForm.vue'

interface Category {
  id: string
  names: Record<string, string>
  icon: string
}

const categories = ref<Category[]>([])
const loading = ref(true)
const actionLoading = ref(false)
const showDrawer = ref(false)
const isEdit = ref(false)
const selectedCategory = ref<Category | null>(null)

// Drag and drop state
const draggedIndex = ref<number | null>(null)
const dragOverIndex = ref<number | null>(null)

async function fetchCategories() {
  loading.value = true
  try {
    const res = await api.get('/api/content/api/categories')
    categories.value = res.data
  } catch (e) {
    console.error('Failed to fetch categories', e)
  } finally {
    loading.value = false
  }
}

function openCreateDrawer() {
  isEdit.value = false
  selectedCategory.value = null
  showDrawer.value = true
}

function openEditDrawer(cat: Category) {
  isEdit.value = true
  selectedCategory.value = { ...cat }
  showDrawer.value = true
}

async function saveCategory(formData: any) {
  actionLoading.value = true
  try {
    const payload = {
      names: formData.names,
      icon: formData.icon
    }

    if (isEdit.value && formData.id) {
      await api.put(`/api/content/api/categories/${formData.id}`, { id: formData.id, ...payload })
    } else {
      await api.post('/api/content/api/categories', payload)
    }

    showDrawer.value = false
    await fetchCategories()
  } catch (e) {
    console.error('Failed to save category', e)
    alert('Failed to save category.')
  } finally {
    actionLoading.value = false
  }
}

async function deleteCategory(id: string) {
  if (!confirm('Are you sure you want to delete this category? (Note: It must have no associated tours to be deleted)')) return
  actionLoading.value = true
  try {
    await api.delete(`/api/content/api/categories/${id}`)
    await fetchCategories()
  } catch (e: any) {
    console.error('Failed to delete category', e)
    const err = e.response?.data?.error || 'Failed to delete category.'
    alert(err)
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
    // A small visual trick for dragging row
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

async function onDrop(event: DragEvent, index: number) {
  if (draggedIndex.value === null || draggedIndex.value === index) {
    onDragEnd()
    return
  }
  
  const draggedCat = categories.value[draggedIndex.value]
  categories.value.splice(draggedIndex.value, 1)
  categories.value.splice(index, 0, draggedCat)
  
  // Here we would typically make an API call to save the new order
  // e.g. await api.post('/api/content/api/categories/reorder', { ids: categories.value.map(c => c.id) })
  
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
      <button @click="openCreateDrawer" class="btn-create">+ Add Category</button>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading categories...</p>
    </div>

    <!-- Table -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th width="50"></th>
            <th>Icon</th>
            <th>Name (EN)</th>
            <th>Name (RU)</th>
            <th>Name (DE)</th>
            <th align="right">Actions</th>
          </tr>
        </thead>
        <tbody @dragover.prevent>
          <tr 
            v-for="(cat, index) in categories" 
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
              <div class="icon-wrapper">{{ cat.icon }}</div>
            </td>
            <td class="name-cell">{{ cat.names?.en || 'Untitled' }}</td>
            <td class="text-muted">{{ cat.names?.ru || '—' }}</td>
            <td class="text-muted">{{ cat.names?.de || '—' }}</td>
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

      <div v-if="categories.length === 0" class="empty-state">
        <div class="empty-icon">📂</div>
        <p>No categories found.</p>
        <button @click="openCreateDrawer" class="btn-ghost">Create your first category</button>
      </div>
    </div>

    <!-- Drawer Form -->
    <CategoryDrawerForm
      v-model="showDrawer"
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

.btn-create { 
  padding: 10px 20px; 
  background: linear-gradient(135deg, #e8820a, #f5a435); 
  border: none; 
  border-radius: 8px; 
  color: #fff; 
  font-weight: 600; 
  cursor: pointer; 
  font-size: 14px; 
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1); 
  box-shadow: 0 4px 12px rgba(232, 130, 10, 0.2);
}
.btn-create:hover { transform: translateY(-2px); box-shadow: 0 8px 24px rgba(232, 130, 10, 0.3); }

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
  transition: all 0.2s; 
}
.btn-action:hover { background: rgba(26,139,196,0.2); border-color: rgba(26,139,196,0.4); color: #fff; }
.btn-delete:hover { background: rgba(220,53,69,0.2); border-color: rgba(220,53,69,0.4); color: #ff6b6b; }

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
