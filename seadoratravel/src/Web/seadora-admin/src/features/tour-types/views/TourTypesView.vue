<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Plus, Edit2, Trash2, Search, Compass, Sparkles, X, Tag } from 'lucide-vue-next'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'
import { useToast } from '@/composables/useToast'
import api from '@/services/api'

export interface TourTypeDto {
  id: string
  code: string
  icon: string
  order: number
  isActive: boolean
  names: Record<string, string>
  descriptions: Record<string, string>
  tourCount?: number
}

const toast = useToast()

// State
const tourTypes = ref<TourTypeDto[]>([])
const loading = ref(false)
const searchQuery = ref('')
const selectedType = ref<TourTypeDto | null>(null)
const isModalOpen = ref(false)
const isEditing = ref(false)
const isSaving = ref(false)

// Form state
const form = ref<{
  id: string
  code: string
  icon: string
  order: number
  isActive: boolean
  names: { en: string; de: string; it: string; fr: string; ru: string }
  descriptions: { en: string; de: string; it: string; fr: string; ru: string }
}>({
  id: '',
  code: '',
  icon: '⛵',
  order: 1,
  isActive: true,
  names: { en: '', de: '', it: '', fr: '', ru: '' },
  descriptions: { en: '', de: '', it: '', fr: '', ru: '' }
})

const activeLocale = ref<'en' | 'de' | 'it' | 'fr' | 'ru'>('en')

// Pagination
const currentPage = ref(1)
const pageSize = ref(10)

const filteredTourTypes = computed(() => {
  if (!searchQuery.value.trim()) return tourTypes.value
  const q = searchQuery.value.toLowerCase()
  return tourTypes.value.filter(t => 
    t.code?.toLowerCase().includes(q) || 
    t.names?.en?.toLowerCase().includes(q) ||
    t.names?.de?.toLowerCase().includes(q)
  )
})

const paginatedTourTypes = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredTourTypes.value.slice(start, start + pageSize.value)
})

// Load tour types
async function fetchTourTypes() {
  loading.value = true
  try {
    const res = await api.get('/api/content/api/tour-types?includeInactive=true')
    tourTypes.value = Array.isArray(res.data) ? res.data : []
  } catch (e: any) {
    console.error('Failed to load tour types', e)
    // Fallback default types
    tourTypes.value = [
      { id: '11111111-1111-1111-1111-111111111111', code: 'GROUP', icon: '⛵', order: 1, isActive: true, names: { en: 'Group Tour', de: 'Gruppentour' }, descriptions: { en: 'Shared guided excursion.' }, tourCount: 12 },
      { id: '22222222-2222-2222-2222-222222222222', code: 'PRIVATE', icon: '👑', order: 2, isActive: true, names: { en: 'Private Tour', de: 'Privattour' }, descriptions: { en: 'Exclusive private transport and guide.' }, tourCount: 8 },
      { id: '33333333-3333-3333-3333-333333333333', code: 'VIP', icon: '✨', order: 3, isActive: true, names: { en: 'VIP Luxury Excursion', de: 'VIP Luxus Exkursion' }, descriptions: { en: 'Top-tier luxury concierge experience.' }, tourCount: 5 },
      { id: '44444444-4444-4444-4444-444444444444', code: 'YACHT', icon: '🛥️', order: 4, isActive: true, names: { en: 'Yacht & Boat Charter', de: 'Yacht- & Bootscharter' }, descriptions: { en: 'Private marine cruises and island excursions.' }, tourCount: 6 },
      { id: '55555555-5555-5555-5555-555555555555', code: 'SHORE_EXCURSION', icon: '⚓', order: 5, isActive: true, names: { en: 'Shore Excursion', de: 'Landausflug' }, descriptions: { en: 'Port excursions tailored for cruise ship arrivals.' }, tourCount: 4 },
      { id: '66666666-6666-6666-6666-666666666666', code: 'MULTI_DAY', icon: '🏔️', order: 6, isActive: true, names: { en: 'Multi-Day Expedition', de: 'Mehrtägige Expedition' }, descriptions: { en: 'Comprehensive multi-day itineraries.' }, tourCount: 3 }
    ]
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  isEditing.value = false
  form.value = {
    id: '',
    code: '',
    icon: '⛵',
    order: tourTypes.value.length + 1,
    isActive: true,
    names: { en: '', de: '', it: '', fr: '', ru: '' },
    descriptions: { en: '', de: '', it: '', fr: '', ru: '' }
  }
  isModalOpen.value = true
}

function openEditModal(t: TourTypeDto) {
  isEditing.value = true
  selectedType.value = t
  form.value = {
    id: t.id,
    code: t.code,
    icon: t.icon || '⛵',
    order: t.order || 1,
    isActive: t.isActive !== false,
    names: { en: t.names?.en || '', de: t.names?.de || '', it: t.names?.it || '', fr: t.names?.fr || '', ru: t.names?.ru || '' },
    descriptions: { en: t.descriptions?.en || '', de: t.descriptions?.de || '', it: t.descriptions?.it || '', fr: t.descriptions?.fr || '', ru: t.descriptions?.ru || '' }
  }
  isModalOpen.value = true
}

async function handleSave() {
  if (!form.value.names.en.trim() || !form.value.code.trim()) {
    toast.error('Validation Error', 'English name and Code are required.')
    return
  }

  isSaving.value = true
  try {
    if (isEditing.value) {
      await api.put(`/api/content/api/tour-types/${form.value.id}`, form.value)
      toast.success('Tour type updated successfully.')
    } else {
      await api.post('/api/content/api/tour-types', form.value)
      toast.success('Tour type created successfully.')
    }
    isModalOpen.value = false
    await fetchTourTypes()
  } catch (e: any) {
    console.error('Failed to save tour type', e)
    toast.error('Failed to save tour type.')
  } finally {
    isSaving.value = false
  }
}

async function handleDelete(t: TourTypeDto) {
  if (!confirm(`Are you sure you want to delete "${t.names?.en || t.code}"?`)) return

  try {
    await api.delete(`/api/content/api/tour-types/${t.id}`)
    toast.success('Tour type deleted successfully.')
    await fetchTourTypes()
  } catch (e: any) {
    console.error('Failed to delete tour type', e)
    toast.error('Failed to delete tour type.')
  }
}

onMounted(fetchTourTypes)
</script>

<template>
  <div class="space-y-8 animate-fade-in text-gray-900 font-sans">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-gray-200/80 pb-6">
      <div>
        <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-secondary/10 border border-secondary/20 text-secondary-text text-xs font-semibold uppercase tracking-wider mb-2">
          <Sparkles class="w-3.5 h-3.5 text-secondary-text" />
          <span>Experience Configuration</span>
        </div>
        <h1 class="text-3xl font-serif font-bold text-gray-900 tracking-tight">Tour & Trip Types</h1>
        <p class="text-sm text-gray-500 mt-1">Configure format classifications (Group, Private, VIP, Yacht, Shore Excursion) and their operational capacities.</p>
      </div>

      <div class="flex items-center gap-3">
        <button
          @click="openCreateModal"
          class="inline-flex items-center gap-2 px-4 py-2.5 bg-gradient-to-r from-primary to-primary-light text-white font-medium text-sm rounded-xl shadow-sm hover:shadow-md hover:from-primary-light hover:to-primary transition-all duration-200 active:scale-[0.98]"
        >
          <Plus class="w-4 h-4" />
          <span>Add Tour Type</span>
        </button>
      </div>
    </div>

    <!-- Filter & Search Toolbar -->
    <div class="flex flex-col sm:flex-row gap-4 justify-between items-center bg-white p-4 rounded-2xl border border-gray-100 shadow-sm">
      <div class="relative w-full sm:w-80">
        <Search class="w-4 h-4 text-gray-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search tour types by name or code..."
          aria-label="Search tour types"
          class="w-full pl-10 pr-4 py-2 text-sm bg-gray-50/70 border border-gray-200 rounded-xl focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
        />
      </div>
      <div class="text-xs text-gray-500 font-medium">
        Total Configured: <span class="font-bold text-gray-900">{{ filteredTourTypes.length }}</span> Types
      </div>
    </div>

    <!-- Data Grid Table -->
    <div class="bg-white rounded-2xl border border-gray-200/80 shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="border-b border-gray-200 bg-gray-50/75 text-xs font-semibold text-gray-500 uppercase tracking-wider">
              <th class="py-4 px-6">Icon</th>
              <th class="py-4 px-6">Type Name (English)</th>
              <th class="py-4 px-6">Code Key</th>
              <th class="py-4 px-6">Order</th>
              <th class="py-4 px-6">Linked Tours</th>
              <th class="py-4 px-6">Status</th>
              <th class="py-4 px-6 text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-100 text-sm">
            <tr v-if="loading">
              <td colspan="7" class="py-12 text-center text-gray-400">
                <div class="flex flex-col items-center justify-center gap-2">
                  <div class="w-6 h-6 border-2 border-primary border-t-transparent rounded-full animate-spin"></div>
                  <span>Loading trip types...</span>
                </div>
              </td>
            </tr>

            <tr v-else-if="paginatedTourTypes.length === 0">
              <td colspan="7" class="py-12 text-center text-gray-400">
                <Compass class="w-10 h-10 mx-auto mb-2 text-gray-300" />
                <p class="font-medium text-gray-600">No tour types found</p>
                <p class="text-xs text-gray-400 mt-1">Add a new trip type to get started</p>
              </td>
            </tr>

            <tr
              v-for="t in paginatedTourTypes"
              :key="t.id"
              class="hover:bg-gray-50/60 transition-colors group"
            >
              <td class="py-4 px-6">
                <div class="w-10 h-10 rounded-xl bg-gray-100/80 border border-gray-200/60 flex items-center justify-center text-xl shadow-inner">
                  {{ t.icon || '⛵' }}
                </div>
              </td>
              <td class="py-4 px-6">
                <div class="font-semibold text-gray-900">{{ t.names?.en || 'Untitled' }}</div>
                <div class="text-xs text-gray-400 mt-0.5 line-clamp-1">{{ t.descriptions?.en || 'No description available' }}</div>
              </td>
              <td class="py-4 px-6">
                <span class="inline-flex items-center gap-1 font-mono text-xs font-bold px-2.5 py-1 rounded-md bg-navy-50 text-navy-800 border border-navy-100">
                  <Tag class="w-3 h-3 text-secondary-text" />
                  {{ t.code }}
                </span>
              </td>
              <td class="py-4 px-6 text-gray-600 font-medium">
                #{{ t.order }}
              </td>
              <td class="py-4 px-6">
                <span class="inline-flex items-center gap-1 px-3 py-1 text-xs font-semibold rounded-full bg-navy-800 text-secondary border border-secondary/20 shadow-sm">
                  <span>✦</span> {{ t.tourCount || 0 }} {{ (t.tourCount === 1) ? 'Tour' : 'Tours' }}
                </span>
              </td>
              <td class="py-4 px-6">
                <span 
                  class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold"
                  :class="t.isActive !== false ? 'bg-emerald-50 text-emerald-700 border border-emerald-200' : 'bg-gray-100 text-gray-500 border border-gray-200'"
                >
                  <span class="w-1.5 h-1.5 rounded-full" :class="t.isActive !== false ? 'bg-emerald-500' : 'bg-gray-400'"></span>
                  {{ t.isActive !== false ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="py-4 px-6 text-right">
                <div class="inline-flex items-center gap-1">
                  <button
                    @click="openEditModal(t)"
                    class="p-2 text-gray-400 hover:text-primary hover:bg-gray-100 rounded-lg transition-colors"
                    title="Edit Tour Type"
                  >
                    <Edit2 class="w-4 h-4" />
                  </button>
                  <button
                    @click="handleDelete(t)"
                    class="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                    title="Delete Tour Type"
                  >
                    <Trash2 class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Luxury Pagination Component -->
      <LuxuryPagination
        v-if="filteredTourTypes.length > 0"
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="filteredTourTypes.length"
      />
    </div>

    <!-- Create / Edit Modal -->
    <Transition name="modal-bounce">
      <div v-if="isModalOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6 overflow-y-auto">
        <div class="fixed inset-0 bg-black/40 backdrop-blur-sm transition-opacity" @click="isModalOpen = false"></div>
        
        <div class="relative w-full max-w-2xl bg-white rounded-3xl shadow-2xl overflow-hidden flex flex-col my-8 border border-gray-100" role="dialog" aria-modal="true" aria-labelledby="tour-type-modal-title" v-dialog="() => isModalOpen = false">
          <!-- Header -->
          <div class="px-6 py-5 border-b border-gray-100 flex items-center justify-between bg-gray-50/60">
            <div>
              <h2 id="tour-type-modal-title" class="text-xl font-serif font-bold text-gray-900 tracking-wide">
                {{ isEditing ? 'Edit Tour Type' : 'Create Tour Type' }}
              </h2>
              <p class="text-xs text-gray-500 mt-0.5 font-sans">Configure trip classification code, icon, and multilingual titles.</p>
            </div>
            <button type="button" @click="isModalOpen = false" aria-label="Close" class="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-full transition-colors">
              <X class="w-5 h-5" />
            </button>
          </div>

          <!-- Body -->
          <div class="p-6 sm:p-8 space-y-6 max-h-[75vh] overflow-y-auto">
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-5">
              <div>
                <label for="tourtype-code" class="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-2">Code Key *</label>
                <input
                  id="tourtype-code"
                  v-model="form.code"
                  type="text"
                  placeholder="e.g. GROUP, VIP"
                  class="w-full px-3.5 py-2.5 text-sm bg-gray-50/70 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary font-mono uppercase"
                />
              </div>
              <div>
                <label for="tourtype-icon" class="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-2">Icon (Emoji / Character)</label>
                <input
                  id="tourtype-icon"
                  v-model="form.icon"
                  type="text"
                  placeholder="e.g. ⛵, 👑, ✨"
                  class="w-full px-3.5 py-2.5 text-sm bg-gray-50/70 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-center text-lg"
                />
              </div>
              <div>
                <label for="tourtype-order" class="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-2">Display Order</label>
                <input
                  id="tourtype-order"
                  v-model.number="form.order"
                  type="number"
                  class="w-full px-3.5 py-2.5 text-sm bg-gray-50/70 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                />
              </div>
            </div>

            <!-- Active Status Toggle -->
            <div class="flex items-center justify-between p-4 rounded-xl bg-gray-50 border border-gray-100">
              <div>
                <div class="text-sm font-semibold text-gray-900">Active Status</div>
                <div class="text-xs text-gray-500">Enable this tour type to appear as a selectable option in tour creation.</div>
              </div>
              <label class="relative inline-flex items-center cursor-pointer">
                <input type="checkbox" v-model="form.isActive" class="sr-only peer">
                <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-secondary"></div>
              </label>
            </div>

            <!-- Multilingual Tabs -->
            <div class="border border-gray-200 rounded-2xl p-5 bg-white space-y-4">
              <div class="flex items-center justify-between border-b border-gray-100 pb-3">
                <label class="text-xs font-semibold text-gray-700 uppercase tracking-wider">Localized Titles & Descriptions</label>
                <div class="flex gap-1 bg-gray-100 p-1 rounded-lg">
                  <button
                    v-for="loc in (['en', 'de', 'it', 'fr', 'ru'] as const)"
                    :key="loc"
                    type="button"
                    @click="activeLocale = loc"
                    class="px-2.5 py-1 text-xs font-bold rounded-md uppercase transition-all"
                    :class="activeLocale === loc ? 'bg-white text-gray-900 shadow-sm' : 'text-gray-500 hover:text-gray-900'"
                  >
                    {{ loc }}
                  </button>
                </div>
              </div>

              <div>
                <label :for="`tourtype-name-${activeLocale}`" class="block text-xs font-medium text-gray-600 mb-1.5">Type Name ({{ activeLocale.toUpperCase() }}) *</label>
                <input
                  :id="`tourtype-name-${activeLocale}`"
                  v-model="form.names[activeLocale]"
                  type="text"
                  :placeholder="`Enter type name in ${activeLocale.toUpperCase()}`"
                  class="w-full px-3.5 py-2.5 text-sm bg-gray-50/70 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                />
              </div>

              <div>
                <label :for="`tourtype-desc-${activeLocale}`" class="block text-xs font-medium text-gray-600 mb-1.5">Description ({{ activeLocale.toUpperCase() }})</label>
                <textarea
                  :id="`tourtype-desc-${activeLocale}`"
                  v-model="form.descriptions[activeLocale]"
                  rows="3"
                  :placeholder="`Enter type description in ${activeLocale.toUpperCase()}`"
                  class="w-full px-3.5 py-2.5 text-sm bg-gray-50/70 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                ></textarea>
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="px-6 py-4 border-t border-gray-100 bg-gray-50/60 flex items-center justify-end gap-3">
            <button
              type="button"
              @click="isModalOpen = false"
              class="px-4 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 rounded-xl transition-colors"
            >
              Cancel
            </button>
            <button
              type="button"
              @click="handleSave"
              :disabled="isSaving"
              class="inline-flex items-center gap-2 px-5 py-2 text-sm font-medium text-white bg-primary hover:bg-primary-light rounded-xl shadow-sm transition-all disabled:opacity-50"
            >
              <div v-if="isSaving" class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
              <span>{{ isEditing ? 'Save Changes' : 'Create Type' }}</span>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(6px); }
  to { opacity: 1; transform: translateY(0); }
}

.modal-bounce-enter-active,
.modal-bounce-leave-active {
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
}
.modal-bounce-enter-from,
.modal-bounce-leave-to {
  opacity: 0;
  transform: scale(0.96);
}
</style>
