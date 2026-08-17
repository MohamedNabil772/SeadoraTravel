<script setup lang="ts">
import { ref, computed } from 'vue'
import { useNationalityStore } from '../store/nationalityStore'
import { Search, Plus, X } from 'lucide-vue-next'

const store = useNationalityStore()
const searchQuery = ref('')
const isDrawerOpen = ref(false)

const editingNationality = ref<any>(null)

const formData = ref({
  countryName: '',
  nationalityName: '',
  flagCode: ''
})

const filteredNationalities = computed(() => {
  if (!searchQuery.value) return store.nationalities
  const q = searchQuery.value.toLowerCase()
  return store.nationalities.filter(n => 
    n.countryName.toLowerCase().includes(q) || 
    n.nationalityName.toLowerCase().includes(q)
  )
})

const openDrawer = (nat: any = null) => {
  editingNationality.value = nat
  if (nat) {
    formData.value = { ...nat }
  } else {
    formData.value = { countryName: '', nationalityName: '', flagCode: '' }
  }
  isDrawerOpen.value = true
}

const closeDrawer = () => {
  isDrawerOpen.value = false
  setTimeout(() => {
    editingNationality.value = null
    formData.value = { countryName: '', nationalityName: '', flagCode: '' }
  }, 500)
}

const save = () => {
  if (editingNationality.value) {
    store.updateNationality(editingNationality.value.id, formData.value)
  } else {
    store.addNationality({ ...formData.value, isActive: true })
  }
  closeDrawer()
}

const getFlagUrl = (code: string) => `https://flagcdn.com/w40/${code.toLowerCase()}.png`
</script>

<template>
  <div class="space-y-6 max-w-6xl mx-auto relative h-full">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-2xl font-bold tracking-tight text-gray-900">Nationalities</h1>
        <p class="text-sm text-gray-500 mt-1">Manage traveler nationalities and countries.</p>
      </div>
      <div class="flex items-center space-x-3 w-full sm:w-auto">
        <div class="relative w-full sm:w-64">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search nationalities..."
            class="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all duration-200 shadow-sm"
          />
        </div>
        <button
          @click="openDrawer()"
          class="inline-flex items-center justify-center gap-2 px-4 py-2 text-sm font-medium text-white bg-gray-900 rounded-xl hover:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-gray-900/20 transition-all active:scale-95 shadow-sm"
        >
          <Plus class="w-4 h-4" />
          Add New
        </button>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
      <div
        v-for="nat in filteredNationalities"
        :key="nat.id"
        class="bg-white p-5 rounded-2xl border border-gray-100 shadow-[0_1px_3px_0_rgb(0,0,0,0.02)] hover:shadow-[0_4px_12px_0_rgb(0,0,0,0.05)] hover:-translate-y-0.5 transition-all duration-300 cursor-pointer group flex flex-col"
        @click="openDrawer(nat)"
      >
        <div class="flex justify-between items-start mb-4">
          <div class="w-10 h-10 rounded-full overflow-hidden border border-gray-100 bg-gray-50 shadow-sm flex items-center justify-center">
            <img :src="getFlagUrl(nat.flagCode)" :alt="nat.countryName" class="w-full h-full object-cover" />
          </div>
          <button
            @click.stop="store.toggleStatus(nat.id)"
            class="relative inline-flex h-5 w-9 shrink-0 cursor-pointer items-center justify-center rounded-full focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500/50"
            role="switch"
            :aria-checked="nat.isActive"
          >
            <div class="absolute inset-0 rounded-full transition-colors duration-300 ease-in-out" :class="nat.isActive ? 'bg-blue-500' : 'bg-gray-200'" />
            <div
              class="absolute left-0 inline-block h-4 w-4 transform rounded-full bg-white shadow-sm ring-0 transition-transform duration-500 ease-[cubic-bezier(0.34,1.56,0.64,1)]"
              :class="nat.isActive ? 'translate-x-[18px]' : 'translate-x-[2px]'"
            />
          </button>
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 group-hover:text-blue-600 transition-colors">{{ nat.nationalityName }}</h3>
          <p class="text-sm text-gray-500">{{ nat.countryName }}</p>
        </div>
      </div>
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
    >
      <div class="flex items-center justify-between px-6 py-5 border-b border-gray-100">
        <h2 class="text-lg font-semibold text-gray-900">{{ editingNationality ? 'Edit Nationality' : 'Add Nationality' }}</h2>
        <button
          @click="closeDrawer"
          class="p-2 -mr-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-full transition-colors focus:outline-none focus:ring-2 focus:ring-gray-200"
        >
          <X class="w-5 h-5" />
        </button>
      </div>
      
      <div class="flex-1 overflow-y-auto p-6 space-y-6">
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700">Country Name</label>
          <input
            v-model="formData.countryName"
            type="text"
            class="w-full px-4 py-2.5 bg-gray-50/50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition-all"
            placeholder="e.g. United States"
          />
        </div>
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700">Nationality Name</label>
          <input
            v-model="formData.nationalityName"
            type="text"
            class="w-full px-4 py-2.5 bg-gray-50/50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition-all"
            placeholder="e.g. American"
          />
        </div>
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 flex items-center justify-between">
            <span>Country Code</span>
            <span class="text-xs text-gray-400 font-normal">2 letters (e.g. US)</span>
          </label>
          <input
            v-model="formData.flagCode"
            type="text"
            maxlength="2"
            class="w-full px-4 py-2.5 bg-gray-50/50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition-all uppercase"
            placeholder="US"
          />
          <div v-if="formData.flagCode && formData.flagCode.length === 2" class="mt-3 flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-100">
            <span class="text-xs text-gray-500 font-medium">Preview:</span>
            <img :src="getFlagUrl(formData.flagCode)" class="w-6 h-6 rounded-full object-cover shadow-sm border border-gray-100" />
          </div>
        </div>
      </div>

      <div class="p-6 border-t border-gray-100 bg-gray-50/50 flex justify-end gap-3">
        <button
          @click="closeDrawer"
          class="px-5 py-2.5 text-sm font-medium text-gray-700 bg-white border border-gray-200 rounded-xl hover:bg-gray-50 hover:text-gray-900 focus:outline-none focus:ring-2 focus:ring-gray-200 transition-all"
        >
          Cancel
        </button>
        <button
          @click="save"
          class="px-5 py-2.5 text-sm font-medium text-white bg-gray-900 rounded-xl hover:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-gray-900/20 transition-all active:scale-95 shadow-sm"
        >
          Save Changes
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Prevent scroll on body when drawer is open */
:global(body:has(.fixed.inset-0)) {
  overflow: hidden;
}
</style>
