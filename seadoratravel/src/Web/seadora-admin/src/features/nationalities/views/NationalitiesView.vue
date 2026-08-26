<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useNationalityStore } from '../store/nationalityStore'
import { Search, Plus, X, Globe } from 'lucide-vue-next'

const store = useNationalityStore()
const searchQuery = ref('')
const isDrawerOpen = ref(false)
const selectedFilter = ref<'all' | 'active' | 'inactive'>('all')

onMounted(() => {
  store.fetchNationalities()
})

const editingNationality = ref<any>(null)

const formData = ref({
  countryName: '',
  nationalityName: '',
  flagCode: '',
  flagEmoji: '',
  isActive: true
})

const filteredNationalities = computed(() => {
  let list = store.nationalities
  if (selectedFilter.value === 'active') {
    list = list.filter(n => n.isActive)
  } else if (selectedFilter.value === 'inactive') {
    list = list.filter(n => !n.isActive)
  }

  if (!searchQuery.value) return list
  const q = searchQuery.value.toLowerCase()
  return list.filter(n => 
    (n.countryName && n.countryName.toLowerCase().includes(q)) || 
    (n.nationalityName && n.nationalityName.toLowerCase().includes(q)) ||
    (n.code && n.code.toLowerCase().includes(q)) ||
    (n.flagCode && n.flagCode.toLowerCase().includes(q))
  )
})

const activeCount = computed(() => store.nationalities.filter(n => n.isActive).length)
const totalCount = computed(() => store.nationalities.length)

const openDrawer = (nat: any = null) => {
  editingNationality.value = nat
  if (nat) {
    formData.value = {
      countryName: nat.countryName || '',
      nationalityName: nat.nationalityName || '',
      flagCode: nat.flagCode || nat.code || '',
      flagEmoji: nat.flagEmoji || '',
      isActive: nat.isActive ?? true
    }
  } else {
    formData.value = { countryName: '', nationalityName: '', flagCode: '', flagEmoji: '', isActive: true }
  }
  isDrawerOpen.value = true
}

const closeDrawer = () => {
  isDrawerOpen.value = false
  setTimeout(() => {
    editingNationality.value = null
    formData.value = { countryName: '', nationalityName: '', flagCode: '', flagEmoji: '', isActive: true }
  }, 300)
}

const save = async () => {
  if (!formData.value.countryName.trim() || !formData.value.nationalityName.trim()) return
  const payload = {
    code: (formData.value.flagCode || '').toUpperCase().trim(),
    countryName: formData.value.countryName.trim(),
    nationalityName: formData.value.nationalityName.trim(),
    flagEmoji: formData.value.flagEmoji || '',
    flagCode: (formData.value.flagCode || '').toUpperCase().trim(),
    isActive: formData.value.isActive
  }
  if (editingNationality.value) {
    await store.updateNationality(editingNationality.value.id, payload)
  } else {
    await store.addNationality(payload)
  }
  closeDrawer()
}

const getFlagEmoji = (code?: string, fallback?: string) => {
  if (fallback && fallback !== '????' && fallback.trim() && fallback !== '🏳️') {
    return fallback
  }
  if (!code || code.length !== 2) return ''
  try {
    const upper = code.toUpperCase()
    const codePoints = upper.split('').map(char => 127397 + char.charCodeAt(0))
    return String.fromCodePoint(...codePoints)
  } catch {
    return ''
  }
}

const getFlagUrl = (code?: string) => {
  if (!code) return ''
  return `https://flagcdn.com/w40/${code.toLowerCase()}.png`
}
</script>

<template>
  <div class="space-y-6 max-w-7xl mx-auto relative h-full">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <div class="flex items-center gap-3">
          <h1 class="text-2xl font-bold tracking-tight text-gray-900">Nationalities</h1>
          <span class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-gray-100 text-gray-700">
            <Globe class="w-3.5 h-3.5" /> {{ totalCount }} Total ({{ activeCount }} Active)
          </span>
        </div>
        <p class="text-sm text-gray-500 mt-1">Manage global traveler nationalities, country codes, and active availability.</p>
      </div>

      <div class="flex items-center space-x-3 w-full sm:w-auto flex-wrap gap-2">
        <!-- Filter Tabs -->
        <div class="inline-flex rounded-xl bg-gray-100 p-1 text-xs font-medium text-gray-600">
          <button 
            @click="selectedFilter = 'all'"
            :class="['px-3 py-1.5 rounded-lg transition-all', selectedFilter === 'all' ? 'bg-white text-gray-900 shadow-sm font-semibold' : 'hover:text-gray-900']"
          >
            All ({{ totalCount }})
          </button>
          <button 
            @click="selectedFilter = 'active'"
            :class="['px-3 py-1.5 rounded-lg transition-all', selectedFilter === 'active' ? 'bg-white text-emerald-700 shadow-sm font-semibold' : 'hover:text-emerald-700']"
          >
            Active ({{ activeCount }})
          </button>
          <button 
            @click="selectedFilter = 'inactive'"
            :class="['px-3 py-1.5 rounded-lg transition-all', selectedFilter === 'inactive' ? 'bg-white text-gray-900 shadow-sm font-semibold' : 'hover:text-gray-900']"
          >
            Inactive ({{ totalCount - activeCount }})
          </button>
        </div>

        <div class="relative w-full sm:w-56">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            v-model="searchQuery"
            type="text"
            aria-label="Search nationalities"
            placeholder="Search nationalities..."
            class="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all duration-200 shadow-sm"
          />
        </div>

        <button
          @click="openDrawer()"
          class="btn-create"
        >
          <Plus class="w-4 h-4" />
          <span>Add Nationality</span>
        </button>
      </div>
    </div>

    <!-- Nationalities Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-4">
      <div
        v-for="nat in filteredNationalities"
        :key="nat.id"
        class="bg-white p-4 rounded-2xl border border-gray-100 shadow-[0_1px_3px_0_rgb(0,0,0,0.02)] hover:shadow-[0_4px_14px_0_rgb(0,0,0,0.06)] hover:-translate-y-0.5 transition-all duration-300 cursor-pointer group flex flex-col justify-between focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500/50"
        role="button"
        tabindex="0"
        :aria-label="`Edit ${nat.nationalityName}`"
        @click="openDrawer(nat)"
        @keydown.enter.prevent="openDrawer(nat)"
        @keydown.space.prevent="openDrawer(nat)"
      >
        <div>
          <div class="flex justify-between items-start mb-3">
            <div class="flex items-center gap-2.5">
              <span v-if="getFlagEmoji(nat.code || nat.flagCode, nat.flagEmoji)" class="text-2xl drop-shadow-sm">
                {{ getFlagEmoji(nat.code || nat.flagCode, nat.flagEmoji) }}
              </span>
              <div v-else class="w-8 h-8 rounded-full overflow-hidden border border-gray-100 bg-gray-50 shadow-sm flex items-center justify-center">
                <img :src="getFlagUrl(nat.flagCode || nat.code)" :alt="nat.countryName" class="w-full h-full object-cover" />
              </div>
              <span class="inline-flex items-center rounded-md bg-gray-50 px-1.5 py-0.5 text-[10px] font-mono font-semibold text-gray-600 ring-1 ring-inset ring-gray-500/10 uppercase">
                {{ nat.code || nat.flagCode }}
              </span>
            </div>

            <button
              @click.stop="store.toggleStatus(nat.id)"
              class="relative inline-flex h-5 w-9 shrink-0 cursor-pointer items-center justify-center rounded-full focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500/50"
              role="switch"
              :aria-checked="nat.isActive"
              title="Toggle Active Status"
            >
              <div class="absolute inset-0 rounded-full transition-colors duration-300 ease-in-out" :class="nat.isActive ? 'bg-blue-500' : 'bg-gray-200'" />
              <div
                class="absolute left-0 inline-block h-4 w-4 transform rounded-full bg-white shadow-sm ring-0 transition-transform duration-500 ease-[cubic-bezier(0.34,1.56,0.64,1)]"
                :class="nat.isActive ? 'translate-x-[18px]' : 'translate-x-[2px]'"
              />
            </button>
          </div>

          <div>
            <h3 class="font-semibold text-gray-900 group-hover:text-blue-600 transition-colors text-sm">{{ nat.nationalityName }}</h3>
            <p class="text-xs text-gray-500 mt-0.5">{{ nat.countryName }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="filteredNationalities.length === 0" class="text-center py-16 bg-white rounded-2xl border border-gray-100">
      <Globe class="w-12 h-12 text-gray-300 mx-auto mb-3" />
      <h3 class="text-base font-semibold text-gray-900">No nationalities found</h3>
      <p class="text-sm text-gray-500 mt-1">Try adjusting your search criteria or filter.</p>
    </div>

    <!-- Backdrop -->
    <div
      v-if="isDrawerOpen"
      class="fixed inset-0 bg-gray-900/30 backdrop-blur-sm z-40 transition-opacity duration-300"
      @click="closeDrawer"
    ></div>

    <!-- Slide-over Drawer -->
    <div
      class="fixed inset-y-0 right-0 z-50 w-full max-w-md bg-white shadow-2xl transform transition-transform duration-500 ease-[cubic-bezier(0.32,0.72,0,1)] flex flex-col pointer-events-auto"
      :class="isDrawerOpen ? 'translate-x-0' : 'translate-x-full'"
      role="dialog"
      aria-modal="true"
      aria-labelledby="nationality-drawer-title"
      :inert="!isDrawerOpen"
      v-dialog="{ open: isDrawerOpen, close: closeDrawer }"
    >
      <div class="flex items-center justify-between px-6 py-5 border-b border-gray-100">
        <h2 id="nationality-drawer-title" class="text-lg font-semibold text-gray-900">{{ editingNationality ? 'Edit Nationality' : 'Add Nationality' }}</h2>
        <button
          type="button"
          @click="closeDrawer"
          aria-label="Close"
          class="p-2 -mr-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-full transition-colors focus:outline-none focus:ring-2 focus:ring-gray-200"
        >
          <X class="w-5 h-5" />
        </button>
      </div>
      
      <div class="flex-1 overflow-y-auto p-6 space-y-6">
        <div class="space-y-2">
          <label for="nat-country-name" class="text-sm font-medium text-gray-700">Country Name</label>
          <input
            id="nat-country-name"
            v-model="formData.countryName"
            type="text"
            class="w-full px-4 py-2.5 bg-gray-50/50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition-all"
            placeholder="e.g. United States"
          />
        </div>
        <div class="space-y-2">
          <label for="nat-nationality-name" class="text-sm font-medium text-gray-700">Nationality Name</label>
          <input
            id="nat-nationality-name"
            v-model="formData.nationalityName"
            type="text"
            class="w-full px-4 py-2.5 bg-gray-50/50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition-all"
            placeholder="e.g. American"
          />
        </div>
        <div class="space-y-2">
          <label for="nat-flag-code" class="text-sm font-medium text-gray-700 flex items-center justify-between">
            <span>ISO Country Code</span>
            <span class="text-xs text-gray-400 font-normal">2 letters (e.g. US)</span>
          </label>
          <input
            id="nat-flag-code"
            v-model="formData.flagCode"
            type="text"
            maxlength="2"
            class="w-full px-4 py-2.5 bg-gray-50/50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition-all uppercase font-mono"
            placeholder="US"
          />
        </div>
        <div class="space-y-2">
          <label for="nat-flag-emoji" class="text-sm font-medium text-gray-700">Flag Emoji</label>
          <input
            id="nat-flag-emoji"
            v-model="formData.flagEmoji"
            type="text"
            class="w-full px-4 py-2.5 bg-gray-50/50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition-all"
            placeholder="e.g. 🇺🇸"
          />
        </div>
        <div class="flex items-center gap-2 pt-2">
          <input type="checkbox" id="natActive" v-model="formData.isActive" class="w-4 h-4 text-blue-600 rounded" />
          <label for="natActive" class="text-sm font-medium text-gray-700 cursor-pointer select-none">Active for bookings & guest profiles</label>
        </div>
      </div>

      <div class="p-6 border-t border-gray-100 bg-gray-50/50 flex justify-end gap-3">
        <button
          type="button"
          @click="closeDrawer"
          class="px-5 py-2.5 text-sm font-medium text-gray-700 bg-white border border-gray-200 rounded-xl hover:bg-gray-50 hover:text-gray-900 focus:outline-none focus:ring-2 focus:ring-gray-200 transition-all"
        >
          Cancel
        </button>
        <button
          type="button"
          @click="save"
          class="px-5 py-2.5 text-sm font-medium text-white bg-gray-900 rounded-xl hover:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-gray-900/20 transition-all active:scale-95 shadow-sm"
        >
          Save Changes
        </button>
      </div>
    </div>
  </div>
</template>
