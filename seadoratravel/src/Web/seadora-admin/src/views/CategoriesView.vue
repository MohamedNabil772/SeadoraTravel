<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '../services/api'

interface Category {
  id: string
  names: Record<string, string>
  icon: string
}

const categories = ref<Category[]>([])
const loading = ref(true)
const actionLoading = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const activeLang = ref('en')

const form = ref({
  id: '',
  names: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  icon: ''
})

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

function openCreateModal() {
  isEdit.value = false
  form.value = {
    id: '',
    names: { en: '', de: '', it: '', fr: '', ru: '' },
    icon: '⛵'
  }
  showModal.value = true
}

function openEditModal(cat: Category) {
  isEdit.value = true
  form.value = {
    id: cat.id,
    names: { ...cat.names },
    icon: cat.icon
  }
  showModal.value = true
}

async function saveCategory() {
  actionLoading.value = true
  try {
    const payload = {
      names: form.value.names,
      icon: form.value.icon
    }

    if (isEdit.value) {
      await api.put(`/api/content/api/categories/${form.value.id}`, { id: form.value.id, ...payload })
    } else {
      await api.post('/api/content/api/categories', payload)
    }

    showModal.value = false
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

onMounted(fetchCategories)
</script>

<template>
  <div class="categories-page">
    <div class="page-header">
      <div>
        <h2>Categories Management</h2>
        <p>Manage tour categories, icons, and localized names.</p>
      </div>
      <button @click="openCreateModal" class="btn-create">+ Add New Category</button>
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
            <th>Icon</th>
            <th>Name (EN)</th>
            <th>Name (RU)</th>
            <th>Name (DE)</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="cat in categories" :key="cat.id">
            <td class="icon-cell">{{ cat.icon }}</td>
            <td class="name-cell">{{ cat.names?.en || 'Untitled' }}</td>
            <td>{{ cat.names?.ru || '—' }}</td>
            <td>{{ cat.names?.de || '—' }}</td>
            <td>
              <div class="actions">
                <button @click="openEditModal(cat)" class="btn-edit-action" :disabled="actionLoading">✏️</button>
                <button @click="deleteCategory(cat.id)" class="btn-delete-action" :disabled="actionLoading">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="categories.length === 0" class="empty-state">
        <p>No categories found</p>
      </div>
    </div>

    <!-- Modal Form -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-card">
        <div class="modal-header">
          <h3>{{ isEdit ? 'Edit Category' : 'Create Category' }}</h3>
          <button @click="showModal = false" class="btn-close">✕</button>
        </div>

        <form @submit.prevent="saveCategory" class="modal-form">
          <!-- Multi-language tabs -->
          <div class="lang-tabs-wrapper">
            <label>Localized Names</label>
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
                  :placeholder="`Category Name (${activeLang.toUpperCase()})`"
                  required
                />
              </div>
            </div>
          </div>

          <div class="form-group">
            <label>Icon Emoji</label>
            <input v-model="form.icon" type="text" placeholder="e.g. 🏝️" required />
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
.categories-page { color: #e0e0e0; }
.page-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 28px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #fff; margin-bottom: 4px; }
.page-header p { color: #8eafc2; font-size: 14px; }
.btn-create { padding: 12px 24px; background: linear-gradient(135deg, #e8820a, #f5a435); border: none; border-radius: 8px; color: #fff; font-weight: 600; cursor: pointer; font-size: 14px; transition: all 0.3s; }
.btn-create:hover { transform: translateY(-1px); box-shadow: 0 8px 24px rgba(232, 130, 10, 0.3); }

.table-container { background: rgba(10,25,41,0.6); border: 1px solid rgba(255,255,255,0.06); border-radius: 12px; overflow: hidden; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 14px 20px; text-align: left; font-size: 12px; letter-spacing: 0.1em; text-transform: uppercase; color: #8eafc2; background: rgba(0,0,0,0.2); border-bottom: 1px solid rgba(255,255,255,0.06); }
.data-table td { padding: 16px 20px; border-bottom: 1px solid rgba(255,255,255,0.04); font-size: 14px; }
.data-table tr:hover { background: rgba(255,255,255,0.02); }

.icon-cell { font-size: 24px; }
.name-cell { font-weight: 600; color: #fff; }

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
.modal-card { background: #0a1929; border: 1px solid rgba(201,168,76,0.2); border-radius: 12px; width: 100%; max-width: 500px; display: flex; flex-direction: column; overflow: hidden; }
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
