<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'
import DestinationDrawerForm from '../components/DestinationDrawerForm.vue'

interface Destination {
  id: string
  names: Record<string, string>
  descriptions: Record<string, string>
  imageUrl: string
  flag: string
  isFeatured?: boolean
}

const destinations = ref<Destination[]>([])
const loading = ref(true)
const actionLoading = ref(false)
const showDrawer = ref(false)
const isEdit = ref(false)
const selectedDestination = ref<Destination | null>(null)
const viewMode = ref<'table' | 'grid'>('grid')

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

function openCreateDrawer() {
  isEdit.value = false
  selectedDestination.value = null
  showDrawer.value = true
}

function openEditDrawer(dest: Destination) {
  isEdit.value = true
  selectedDestination.value = { ...dest }
  showDrawer.value = true
}

async function saveDestination(formData: any) {
  actionLoading.value = true
  try {
    const payload = {
      names: formData.names,
      descriptions: formData.descriptions,
      imageUrl: formData.imageUrl,
      flag: formData.flag,
      isFeatured: formData.isFeatured
    }

    if (isEdit.value && formData.id) {
      await api.put(`/api/content/api/destinations/${formData.id}`, { id: formData.id, ...payload })
    } else {
      await api.post('/api/content/api/destinations', payload)
    }

    showDrawer.value = false
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
        <h2>Destinations</h2>
        <p>Manage travel areas, regions, default images, and translations.</p>
      </div>
      <div class="header-actions">
        <div class="view-toggle">
          <button 
            :class="{ active: viewMode === 'table' }" 
            @click="viewMode = 'table'"
            title="List View"
          >
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="8" y1="6" x2="21" y2="6"></line><line x1="8" y1="12" x2="21" y2="12"></line><line x1="8" y1="18" x2="21" y2="18"></line><line x1="3" y1="6" x2="3.01" y2="6"></line><line x1="3" y1="12" x2="3.01" y2="12"></line><line x1="3" y1="18" x2="3.01" y2="18"></line></svg>
          </button>
          <button 
            :class="{ active: viewMode === 'grid' }" 
            @click="viewMode = 'grid'"
            title="Grid View"
          >
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="7" height="7"></rect><rect x="14" y="3" width="7" height="7"></rect><rect x="14" y="14" width="7" height="7"></rect><rect x="3" y="14" width="7" height="7"></rect></svg>
          </button>
        </div>
        <button @click="openCreateDrawer" class="btn-create">+ Add Destination</button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading destinations...</p>
    </div>

    <!-- Empty State -->
    <div v-else-if="destinations.length === 0" class="empty-state table-container">
      <div class="empty-icon">🌍</div>
      <p>No destinations found.</p>
      <button @click="openCreateDrawer" class="btn-ghost">Add your first destination</button>
    </div>

    <!-- Grid View -->
    <div v-else-if="viewMode === 'grid'" class="destinations-grid">
      <div v-for="dest in destinations" :key="dest.id" class="destination-card">
        <div class="card-image-wrap">
          <img :src="dest.imageUrl || 'https://images.unsplash.com/photo-1506929562872-bb421503ef21?auto=format&fit=crop&w=800&q=80'" :alt="dest.names?.en" class="card-image" />
          <div v-if="dest.isFeatured" class="featured-badge">Featured</div>
          <div class="card-actions">
            <button @click="openEditDrawer(dest)" class="btn-action" title="Edit" :disabled="actionLoading">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20h9"></path><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"></path></svg>
            </button>
            <button @click="deleteDestination(dest.id)" class="btn-action btn-delete" title="Delete" :disabled="actionLoading">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
            </button>
          </div>
        </div>
        <div class="card-content">
          <div class="card-title-row">
            <span class="flag-emoji">{{ dest.flag }}</span>
            <h3 class="card-title">{{ dest.names?.en || 'Untitled' }}</h3>
          </div>
          <p class="card-desc">{{ dest.descriptions?.en || 'No description provided.' }}</p>
        </div>
      </div>
    </div>

    <!-- Table View -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th width="80">Image</th>
            <th>Name (EN)</th>
            <th>Description (EN)</th>
            <th>Featured</th>
            <th align="right">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="dest in destinations" :key="dest.id">
            <td>
              <div class="thumb-wrapper">
                <img :src="dest.imageUrl || 'https://images.unsplash.com/photo-1506929562872-bb421503ef21?auto=format&fit=crop&w=150&q=80'" class="thumb-image" />
                <span class="thumb-flag">{{ dest.flag }}</span>
              </div>
            </td>
            <td class="name-cell">{{ dest.names?.en || 'Untitled' }}</td>
            <td class="desc-cell">{{ dest.descriptions?.en || '—' }}</td>
            <td>
              <span v-if="dest.isFeatured" class="badge-success">Yes</span>
              <span v-else class="text-muted">—</span>
            </td>
            <td>
              <div class="actions">
                <button @click="openEditDrawer(dest)" class="btn-action" title="Edit" :disabled="actionLoading">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20h9"></path><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"></path></svg>
                </button>
                <button @click="deleteDestination(dest.id)" class="btn-action btn-delete" title="Delete" :disabled="actionLoading">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Drawer Form -->
    <DestinationDrawerForm
      v-model="showDrawer"
      :isEdit="isEdit"
      :destination="selectedDestination"
      :actionLoading="actionLoading"
      @save="saveDestination"
    />
  </div>
</template>

<style scoped>
.destinations-page { color: #e0e0e0; animation: fadeIn 0.4s ease; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }

.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 28px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #fff; margin-bottom: 4px; letter-spacing: -0.02em; }
.page-header p { color: #8eafc2; font-size: 14px; margin: 0; }
.header-actions { display: flex; align-items: center; gap: 16px; }

.view-toggle { display: flex; background: rgba(0,0,0,0.2); border-radius: 8px; border: 1px solid rgba(255,255,255,0.1); overflow: hidden; }
.view-toggle button {
  background: transparent; border: none; padding: 8px 12px; color: #5c7585; cursor: pointer; transition: all 0.2s;
  display: flex; align-items: center; justify-content: center;
}
.view-toggle button:hover { color: #8eafc2; background: rgba(255,255,255,0.05); }
.view-toggle button.active { color: #fff; background: rgba(255,255,255,0.1); }

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

/* Grid View */
.destinations-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 24px;
}
.destination-card {
  background: rgba(10,25,41,0.6);
  border: 1px solid rgba(255,255,255,0.06);
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 4px 20px rgba(0,0,0,0.1);
}
.destination-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 32px rgba(0,0,0,0.2);
  border-color: rgba(255,255,255,0.1);
}
.card-image-wrap {
  position: relative;
  width: 100%;
  height: 180px;
  overflow: hidden;
}
.card-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}
.destination-card:hover .card-image { transform: scale(1.05); }
.featured-badge {
  position: absolute;
  top: 12px;
  left: 12px;
  background: rgba(26,139,196,0.9);
  color: #fff;
  font-size: 11px;
  font-weight: 600;
  padding: 4px 8px;
  border-radius: 4px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  backdrop-filter: blur(4px);
}
.card-actions {
  position: absolute;
  top: 12px;
  right: 12px;
  display: flex;
  gap: 8px;
  opacity: 0;
  transform: translateY(-10px);
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}
.destination-card:hover .card-actions { opacity: 1; transform: translateY(0); }
.card-content { padding: 20px; }
.card-title-row { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
.flag-emoji { font-size: 20px; }
.card-title { font-size: 18px; font-weight: 600; color: #fff; margin: 0; }
.card-desc { color: #8eafc2; font-size: 14px; margin: 0; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; line-height: 1.5; }

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
  padding: 16px 20px; text-align: left; font-size: 12px; letter-spacing: 0.1em; 
  text-transform: uppercase; color: #8eafc2; background: rgba(0,0,0,0.2); 
  border-bottom: 1px solid rgba(255,255,255,0.06); font-weight: 600;
}
.data-table th[align="right"] { text-align: right; }
.data-table td { padding: 16px 20px; border-bottom: 1px solid rgba(255,255,255,0.03); font-size: 14px; vertical-align: middle; }
.data-table tr:hover { background: rgba(255,255,255,0.03); }

.thumb-wrapper {
  position: relative; width: 64px; height: 48px; border-radius: 6px; overflow: hidden; background: rgba(0,0,0,0.2);
}
.thumb-image { width: 100%; height: 100%; object-fit: cover; }
.thumb-flag { position: absolute; bottom: -4px; right: 2px; font-size: 16px; filter: drop-shadow(0 2px 4px rgba(0,0,0,0.5)); }
.name-cell { font-weight: 600; color: #fff; font-size: 15px; }
.desc-cell { max-width: 250px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; color: #8eafc2; }

.badge-success { background: rgba(16,185,129,0.1); color: #10b981; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: 600; }
.text-muted { color: #8eafc2; }

.actions { display: flex; gap: 8px; justify-content: flex-end; }
.btn-action { 
  background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); color: #e0e0e0; cursor: pointer; 
  display: flex; align-items: center; justify-content: center; width: 32px; height: 32px; border-radius: 6px; transition: all 0.2s; 
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
  background: transparent; border: 1px solid rgba(255,255,255,0.2); color: #fff; padding: 8px 16px; border-radius: 6px; cursor: pointer; transition: all 0.2s;
}
.btn-ghost:hover { background: rgba(255,255,255,0.05); }
</style>
