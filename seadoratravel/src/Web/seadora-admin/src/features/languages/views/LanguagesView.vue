<template>
  <div class="space-y-8 max-w-[1400px] mx-auto py-8 px-6">
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div>
        <h1 class="text-3xl font-semibold tracking-tight text-gray-900">Localization</h1>
        <p class="text-gray-500 mt-1.5 text-sm">Manage supported languages and update translations.</p>
      </div>
      <button @click="showAddModal = true" class="inline-flex items-center justify-center rounded-lg bg-gray-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-gray-800 transition-colors shadow-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:ring-offset-2 w-full sm:w-auto">
        <svg class="mr-2 h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg>
        Add Language
      </button>
    </div>

    <!-- Languages Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5">
      <div v-for="lang in store.languages" :key="lang.code" class="relative overflow-hidden rounded-2xl border border-gray-200 bg-white p-5 shadow-sm transition-all hover:shadow-md hover:border-gray-300 group">
        <div class="flex justify-between items-start mb-5">
          <div class="flex items-center gap-3.5">
            <span class="text-3xl">{{ lang.flag }}</span>
            <div>
              <h3 class="font-medium text-gray-900">{{ lang.name }}</h3>
              <p class="text-xs text-gray-500 font-medium tracking-wide mt-0.5">{{ lang.code.toUpperCase() }} <span class="mx-1 text-gray-300">•</span> {{ lang.isRtl ? 'RTL' : 'LTR' }}</p>
            </div>
          </div>
          <span v-if="lang.isDefault" class="inline-flex items-center rounded-full bg-blue-50 px-2.5 py-1 text-xs font-semibold text-blue-700 ring-1 ring-inset ring-blue-700/10">Default</span>
        </div>
        
        <div class="space-y-2.5">
          <div class="flex justify-between text-xs text-gray-600 font-medium">
            <span>Translation Progress</span>
            <span :class="store.getLanguageProgress(lang.code) === 100 ? 'text-green-600' : 'text-blue-600'">{{ store.getLanguageProgress(lang.code) }}%</span>
          </div>
          <div class="h-1.5 w-full bg-gray-100 rounded-full overflow-hidden">
            <div 
              class="h-full rounded-full transition-all duration-1000 ease-out"
              :class="store.getLanguageProgress(lang.code) === 100 ? 'bg-green-500' : 'bg-blue-500'"
              :style="{ width: `${store.getLanguageProgress(lang.code)}%` }"
            ></div>
          </div>
        </div>
        
        <div class="mt-5 pt-4 border-t border-gray-100 flex gap-3">
          <button @click="exportLang(lang.code)" class="text-xs font-medium text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1.5">
            <svg class="h-3.5 w-3.5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" x2="12" y1="15" y2="3"/></svg>
            Export JSON
          </button>
        </div>
      </div>
    </div>

    <!-- Translation Editor -->
    <div class="rounded-2xl border border-gray-200 bg-white shadow-sm overflow-hidden flex flex-col h-[650px] ring-1 ring-black/5">
      <TranslationsEditor />
    </div>

    <!-- Add Modal -->
    <div v-if="showAddModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-0">
      <div class="fixed inset-0 bg-gray-900/40 backdrop-blur-sm transition-opacity" @click="showAddModal = false"></div>
      <div class="relative transform overflow-hidden rounded-2xl bg-white p-6 text-left shadow-2xl transition-all sm:w-full sm:max-w-md border border-gray-200 animate-in fade-in zoom-in-95 duration-200 ease-out">
        <div class="flex items-center justify-between mb-5">
          <h3 class="text-lg font-semibold text-gray-900">Add New Language</h3>
          <button @click="showAddModal = false" class="text-gray-400 hover:text-gray-500 transition-colors">
            <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
          </button>
        </div>
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Language Name</label>
            <input v-model="newLang.name" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. French" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Code (ISO)</label>
            <input v-model="newLang.code" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. fr" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Flag Emoji</label>
            <input v-model="newLang.flag" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. 🇫🇷" />
          </div>
          <div class="flex items-center gap-2 mt-4 bg-gray-50 p-3 rounded-lg border border-gray-100">
            <input v-model="newLang.isRtl" type="checkbox" id="isRtl" class="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500" />
            <label for="isRtl" class="text-sm font-medium text-gray-700 cursor-pointer select-none">Right-to-Left (RTL) Layout</label>
          </div>
        </div>
        <div class="mt-6 flex justify-end gap-3 pt-4 border-t border-gray-100">
          <button @click="showAddModal = false" class="rounded-lg px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-gray-200">Cancel</button>
          <button @click="handleAdd" :disabled="!newLang.code || !newLang.name" class="rounded-lg bg-gray-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-gray-900 focus:ring-offset-2">Add Language</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useLanguageStore } from '../store/languageStore'
import TranslationsEditor from '../components/TranslationsEditor.vue'

const store = useLanguageStore()
const showAddModal = ref(false)

const newLang = ref({
  code: '',
  name: '',
  flag: '',
  isRtl: false
})

const handleAdd = () => {
  if (!newLang.value.code || !newLang.value.name) return
  store.addLanguage({
    ...newLang.value,
    isDefault: false
  })
  showAddModal.value = false
  newLang.value = { code: '', name: '', flag: '', isRtl: false }
}

const exportLang = (code: string) => {
  const data = store.exportTranslations(code)
  const blob = new Blob([data], { type: 'application/json' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `translations_${code}.json`
  a.click()
  URL.revokeObjectURL(url)
}
</script>
