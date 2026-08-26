<template>
  <div class="flex flex-col h-full bg-white relative">
    <!-- Header/Toolbar -->
    <div class="flex items-center justify-between border-b border-gray-200 px-5 py-4 bg-gray-50/50 flex-wrap gap-4">
      <div class="flex items-center gap-4 flex-wrap">
        <div class="relative">
          <svg class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
          <input v-model="searchQuery" type="text" aria-label="Search translation keys" class="h-9 w-72 rounded-lg border-gray-200 border bg-white pl-9 pr-4 text-sm text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all shadow-sm" placeholder="Search keys or translations..." />
        </div>
        <select v-model="selectedNamespace" aria-label="Filter by namespace" class="h-9 rounded-lg border-gray-200 border bg-white px-3 text-sm text-gray-900 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 shadow-sm transition-all cursor-pointer">
          <option value="all">All Namespaces</option>
          <option v-for="ns in namespaces" :key="ns" :value="ns">{{ ns }}</option>
        </select>
        <button @click="showAddKeyModal = true" class="h-9 inline-flex items-center justify-center rounded-lg bg-white border border-gray-200 px-3 py-1.5 text-sm font-medium text-gray-700 hover:bg-gray-50 hover:text-gray-900 transition-colors shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20">
          <svg class="mr-1.5 h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg>
          Add Key
        </button>
      </div>
      <div class="flex items-center gap-3">
        <span class="text-sm text-gray-500 flex items-center gap-2 font-medium">
          <span v-if="store.isSaving" class="flex items-center gap-1.5"><span class="h-2 w-2 rounded-full bg-yellow-400 animate-pulse"></span> Saving...</span>
          <span v-else class="flex items-center gap-1.5"><span class="h-2 w-2 rounded-full bg-green-400"></span> Saved</span>
        </span>
      </div>
    </div>

    <!-- Dual Pane Editor -->
    <div class="flex-1 overflow-auto bg-gray-50/30 p-5">
      <div v-if="store.isLoading" class="flex justify-center items-center py-20 text-gray-500">
        Loading...
      </div>
      <div v-else class="mx-auto max-w-6xl space-y-4">
        <div v-for="item in filteredTranslations" :key="`${item.namespace}:${item.key}`" class="rounded-xl border border-gray-200 bg-white p-5 shadow-sm transition-all hover:shadow-md hover:border-gray-300">
          <div class="mb-4 flex items-center gap-2">
            <span class="inline-flex items-center rounded-md bg-gray-100 px-2 py-1 text-xs font-medium text-gray-600 ring-1 ring-inset ring-gray-500/10">{{ item.namespace }}</span>
            <span class="text-sm font-mono font-medium text-gray-900">{{ item.key }}</span>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
            <div v-for="lang in store.activeLanguages" :key="lang.code" class="relative group">
              <label :for="`trans-${item.namespace}-${item.key}-${lang.code}`.replace(/[^a-z0-9-]/gi, '-')" class="mb-1.5 block text-xs font-medium text-gray-500 uppercase tracking-wider flex items-center gap-1.5">
                <span class="text-sm">{{ lang.flag }}</span> {{ lang.name }}
              </label>
              <textarea 
                :id="`trans-${item.namespace}-${item.key}-${lang.code}`.replace(/[^a-z0-9-]/gi, '-')"
                :value="item.values[lang.code]"
                @input="handleInput(item.key, item.namespace, lang.code, $event)"
                rows="2"
                :dir="lang.isRtl ? 'rtl' : 'ltr'"
                class="block w-full rounded-lg border border-gray-200 px-3 py-2.5 text-sm text-gray-900 placeholder-gray-300 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all bg-gray-50/50 group-hover:bg-white group-hover:border-gray-300 focus:bg-white resize-y min-h-[44px] shadow-sm"
                :class="lang.isRtl ? 'font-arabic text-right' : ''"
                :placeholder="`Enter ${lang.name} translation...`"
              ></textarea>
            </div>
          </div>
        </div>
        
        <div v-if="filteredTranslations.length === 0" class="text-center py-16">
          <div class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-gray-100 mb-4">
            <svg class="h-6 w-6 text-gray-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" /></svg>
          </div>
          <h3 class="mt-2 text-sm font-medium text-gray-900">No translations found</h3>
          <p class="mt-1 text-sm text-gray-500">We couldn't find anything matching your search criteria.</p>
        </div>
      </div>
    </div>

    <!-- Add Key Modal -->
    <div v-if="showAddKeyModal" class="fixed inset-0 z-[60] flex items-center justify-center p-4 sm:p-0">
      <div class="fixed inset-0 bg-gray-900/40 backdrop-blur-sm transition-opacity" @click="showAddKeyModal = false"></div>
      <div class="relative transform overflow-hidden rounded-2xl bg-white p-6 text-left shadow-2xl transition-all sm:w-full sm:max-w-md border border-gray-200 animate-in fade-in zoom-in-95 duration-200 ease-out" role="dialog" aria-modal="true" aria-labelledby="add-key-title" v-dialog="() => showAddKeyModal = false">
        <div class="flex items-center justify-between mb-5">
          <h3 id="add-key-title" class="text-lg font-semibold text-gray-900">Add Translation Key</h3>
          <button type="button" @click="showAddKeyModal = false" aria-label="Close" class="text-gray-400 hover:text-gray-500 transition-colors">
            <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
          </button>
        </div>
        <div class="space-y-4">
          <div>
            <label for="new-key-namespace" class="block text-sm font-medium text-gray-700 mb-1.5">Namespace</label>
            <input id="new-key-namespace" v-model="newKeyNamespace" type="text" list="namespaces-list" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. common, nav..." />
            <datalist id="namespaces-list">
              <option v-for="ns in namespaces" :key="ns" :value="ns">{{ ns }}</option>
            </datalist>
          </div>
          <div>
            <label for="new-key-name" class="block text-sm font-medium text-gray-700 mb-1.5">Key</label>
            <input id="new-key-name" v-model="newKey" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. welcome_message" />
          </div>
        </div>
        <div class="mt-6 flex justify-end gap-3 pt-4 border-t border-gray-100">
          <button type="button" @click="showAddKeyModal = false" class="rounded-lg px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-gray-200">Cancel</button>
          <button type="button" @click="handleAddKey" :disabled="!newKey || !newKeyNamespace" class="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-blue-600 focus:ring-offset-2">Add Key</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useLanguageStore } from '../store/languageStore'

const store = useLanguageStore()
const searchQuery = ref('')
const selectedNamespace = ref('all')

const showAddKeyModal = ref(false)
const newKey = ref('')
const newKeyNamespace = ref('common')

const namespaces = computed(() => {
  const ns = new Set<string>()
  store.translations.forEach(t => ns.add(t.namespace))
  // Add some default ones if empty
  const defaults = ['common', 'nav', 'hero', 'destinations', 'trips', 'footer', 'tour_details']
  defaults.forEach(d => ns.add(d))
  return Array.from(ns).sort()
})

const filteredTranslations = computed(() => {
  return store.translations.filter(t => {
    const matchesSearch = 
      t.key.toLowerCase().includes(searchQuery.value.toLowerCase()) || 
      Object.values(t.values).some(v => v?.toLowerCase().includes(searchQuery.value.toLowerCase()))
    const matchesNs = selectedNamespace.value === 'all' || t.namespace === selectedNamespace.value
    return matchesSearch && matchesNs
  })
})

const updateValue = (key: string, namespace: string, langCode: string, value: string) => {
  store.updateTranslation(key, namespace, langCode, value)
}

const handleInput = (key: string, namespace: string, langCode: string, event: Event) => {
  const target = event.target as HTMLTextAreaElement
  if (target) {
    updateValue(key, namespace, langCode, target.value)
  }
}

const handleAddKey = () => {
  if (newKey.value && newKeyNamespace.value) {
    store.addTranslationKey(newKey.value.trim(), newKeyNamespace.value.trim())
    newKey.value = ''
    showAddKeyModal.value = false
  }
}
</script>
<style scoped>
/* Custom scrollbar for textarea */
textarea::-webkit-scrollbar {
  width: 6px;
}
textarea::-webkit-scrollbar-track {
  background: transparent;
}
textarea::-webkit-scrollbar-thumb {
  background-color: #d1d5db;
  border-radius: 20px;
}
</style>
