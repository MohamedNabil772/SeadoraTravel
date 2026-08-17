<template>
  <div class="flex flex-col h-full bg-white">
    <!-- Header/Toolbar -->
    <div class="flex items-center justify-between border-b border-gray-200 px-5 py-4 bg-gray-50/50">
      <div class="flex items-center gap-4">
        <div class="relative">
          <svg class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
          <input v-model="searchQuery" type="text" class="h-9 w-72 rounded-lg border-gray-200 border bg-white pl-9 pr-4 text-sm text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all shadow-sm" placeholder="Search keys or translations..." />
        </div>
        <select v-model="selectedNamespace" class="h-9 rounded-lg border-gray-200 border bg-white px-3 text-sm text-gray-900 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 shadow-sm transition-all cursor-pointer">
          <option value="all">All Namespaces</option>
          <option v-for="ns in namespaces" :key="ns" :value="ns">{{ ns }}</option>
        </select>
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
      <div class="mx-auto max-w-6xl space-y-4">
        <div v-for="item in filteredTranslations" :key="`${item.namespace}:${item.key}`" class="rounded-xl border border-gray-200 bg-white p-5 shadow-sm transition-all hover:shadow-md hover:border-gray-300">
          <div class="mb-4 flex items-center gap-2">
            <span class="inline-flex items-center rounded-md bg-gray-100 px-2 py-1 text-xs font-medium text-gray-600 ring-1 ring-inset ring-gray-500/10">{{ item.namespace }}</span>
            <span class="text-sm font-mono font-medium text-gray-900">{{ item.key }}</span>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
            <div v-for="lang in store.languages" :key="lang.code" class="relative group">
              <label class="mb-1.5 block text-xs font-medium text-gray-500 uppercase tracking-wider flex items-center gap-1.5">
                <span class="text-sm">{{ lang.flag }}</span> {{ lang.name }}
              </label>
              <textarea 
                :value="item.values[lang.code]"
                @input="e => updateValue(item.key, item.namespace, lang.code, (e.target as HTMLTextAreaElement).value)"
                rows="2"
                :dir="lang.isRtl ? 'rtl' : 'ltr'"
                class="block w-full rounded-lg border border-gray-200 px-3 py-2.5 text-sm text-gray-900 placeholder-gray-300 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all bg-gray-50/50 group-hover:bg-white group-hover:border-gray-300 focus:bg-white resize-y min-h-[44px]"
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useLanguageStore } from '../store/languageStore'

const store = useLanguageStore()
const searchQuery = ref('')
const selectedNamespace = ref('all')

const namespaces = computed(() => {
  const ns = new Set<string>()
  store.translations.forEach(t => ns.add(t.namespace))
  return Array.from(ns)
})

const filteredTranslations = computed(() => {
  return store.translations.filter(t => {
    const matchesSearch = 
      t.key.toLowerCase().includes(searchQuery.value.toLowerCase()) || 
      Object.values(t.values).some(v => v.toLowerCase().includes(searchQuery.value.toLowerCase()))
    const matchesNs = selectedNamespace.value === 'all' || t.namespace === selectedNamespace.value
    return matchesSearch && matchesNs
  })
})

const updateValue = (key: string, namespace: string, langCode: string, value: string) => {
  store.updateTranslation(key, namespace, langCode, value)
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
