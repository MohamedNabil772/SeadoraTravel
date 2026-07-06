<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '../services/api'

interface Destination {
  id: string
  names: Record<string, string>
  descriptions: Record<string, string>
  imageUrl: string
  flag: string
}

const destinations = ref<Destination[]>([])
const loading = ref(true)
const actionLoading = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const activeLang = ref('en')

const form = ref({
  id: '',
  names: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  descriptions: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  imageUrl: '',
  flag: ''
})

async function fetchDestinations() {
  loading.value = true
  try {
    const res = await api.get('/api/content/api/destinations')
    destinations.value = res.data
  } catch (e) {
    console.error('Failed to fetch destinations', e)
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  isEdit.value = false
  form.value = {
    id: '',
    names: { en: '', de: '', it: '', fr: '', ru: '' },
    descriptions: { en: '', de: '', it: '', fr: '', ru: '' },
    imageUrl: 'https://images.unsplash.com/photo-',
    flag: '📍'
  }
  showModal.value = true
}

function openEditModal(dest: Destination) {
  isEdit.value = true
  form.value = {
    id: dest.id,
    names: { ...dest.names },
    descriptions: dest.descriptions ? { ...dest.descriptions } : { en: '', de: '', it: '', fr: '', ru: '' },
    imageUrl: dest.imageUrl,
    flag: dest.flag
  }
  showModal.value = true
}

async function saveDestination() {
  actionLoading.value = true
  try {
    const payload = {
      names: form.value.names,
      descriptions: form.value.descriptions,
      imageUrl: form.value.imageUrl,
      flag: form.value.flag
    }

    if (isEdit.value) {
      await api.put(`/api/content/api/destinations/${form.value.id}`, { id: form.value.id, ...payload })
    } else {
      await api.post('/api/content/api/destinations', payload)
    }

    showModal.value = false
    await fetchDestinations()
  } catch (e) {
    console.error('Failed to save destination', e)
    alert('Failed to save destination.')
  } finally {
    actionLoading.value = false
  }
}

async function deleteDestination(id: string) {
  if (!confirm('Are you sure you want to delete this destination? (Note: It must have no associated tours to be deleted)')) return
  actionLoading.value = true
  try {
    await api.delete(`/api/content/api/destinations/${id}`)
    await fetchDestinations()
  } catch (e: any) {
    console.error('Failed to delete destination', e)
    const err = e.response?.data?.error || 'Failed to delete destination.'
    alert(err)
  } finally {
    actionLoading.value = false
  }
}

onMounted(fetchDestinations)
</script>

<template>
  <div class="destinations-page">
    <div class="page-header">
      <div>
        <h2>Destinations Management</h2>
        <p>Manage travel areas, default images, flag emojis, and translations.</p>
      </div>
      <button @click="openCreateModal" class="btn-create">+ Add New Destination</button>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading destinations...</p>
    </div>

    <!-- Table -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Flag</th>
            <th>Name (EN)</th>
            <th>Description (EN)</th>
            <th>Image URL</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="dest in destinations" :key="dest.id">
            <td class="flag-cell">{{ dest.flag }}</td>
            <td class="name-cell">{{ dest.names?.en || 'Untitled' }}</td>
            <td class="desc-cell">{{ dest.descriptions?.en || '—' }}</td>
            <td class="url-cell">
              <a :href="dest.imageUrl" target="_blank" class="image-link">View Image 🖼️</a>
            </td>
            <td>
              <div class="actions">
                <button @click="openEditModal(dest)" class="btn-edit-action" :disabled="actionLoading">✏️</button>
                <button @click="deleteDestination(dest.id)" class="btn-delete-action" :disabled="actionLoading">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="destinations.length === 0" class="empty-state">
        <p>No destinations found</p>
      </div>
    </div>

    <!-- Modal Form -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-card">
        <div class="modal-header">
          <h3>{{ isEdit ? 'Edit Destination' : 'Create Destination' }}</h3>
          <button @click="showModal = false" class="btn-close">✕</button>
        </div>

        <form @submit.prevent="saveDestination" class="modal-form">
          <!-- Multi-language tabs -->
          <div class="lang-tabs-wrapper">
            <label>Localized Names & Descriptions</label>
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
                  :placeholder="`Destination Name (${activeLang.toUpperCase()})`"
                  required
                />
              </div>
              <div class="form-group">
                <textarea
                  v-model="form.descriptions[activeLang]"
                  rows="3"
                  :placeholder="`Destination Description (${activeLang.toUpperCase()})`"
                  required
                ></textarea>
              </div>
            </div>
          </div>

          <div class="form-group">
            <label>Image URL</label>
            <input v-model="form.imageUrl" type="text" placeholder="https://..." required />
          </div>

          <div class="form-group">
            <label>Flag Emoji</label>
            <input v-model="form.flag" type="text" placeholder="e.g. 🌴" required />
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
.destinations-page { color: #e0e0e0; }
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

.flag-cell { font-size: 24px; }
.name-cell { font-weight: 600; color: #fff; }
.desc-cell { max-width: 300px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.image-link { color: #1a8bc4; text-decoration: none; font-size: 13px; }
.image-link:hover { text-decoration: underline; }

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
.modal-card { background: #0a1929; border: 1px solid rgba(201,168,76,0.2); border-radius: 12px; width: 100%; max-width: 600px; display: flex; flex-direction: column; overflow: hidden; }
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
