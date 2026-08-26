<template>
  <div class="space-y-8 max-w-[1400px] mx-auto py-8 px-6">
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div>
        <h1 class="text-3xl font-semibold tracking-tight text-gray-900">Localization</h1>
        <p class="text-gray-500 mt-1.5 text-sm">Manage supported languages and update translations.</p>
      </div>
      <button @click="openAddModal" class="btn-create w-full sm:w-auto">
        <svg class="h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg>
        <span>Add Language</span>
      </button>
    </div>

    <!-- Languages Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5">
      <div v-for="lang in store.languages" :key="lang.code" class="relative overflow-hidden rounded-2xl border border-gray-200 bg-white p-5 shadow-sm transition-all hover:shadow-md hover:border-gray-300 group flex flex-col h-full">
        <div class="flex justify-between items-start mb-5">
          <div class="flex items-center gap-3.5">
            <span class="text-3xl drop-shadow-sm">{{ lang.flag }}</span>
            <div>
              <h3 class="font-semibold text-gray-900">{{ lang.name }}</h3>
              <div class="flex items-center gap-1.5 mt-1 flex-wrap">
                <span class="inline-flex items-center rounded-md bg-gray-100 px-1.5 py-0.5 text-[10px] font-medium text-gray-600 tracking-wider uppercase ring-1 ring-inset ring-gray-500/10">{{ lang.code }}</span>
                <span class="inline-flex items-center rounded-md bg-gray-100 px-1.5 py-0.5 text-[10px] font-medium text-gray-600 tracking-wider uppercase ring-1 ring-inset ring-gray-500/10">{{ lang.isRtl ? 'RTL' : 'LTR' }}</span>
              </div>
            </div>
          </div>
          <div class="flex flex-col items-end gap-2">
            <span v-if="lang.isDefault" class="inline-flex items-center rounded-full bg-blue-50 px-2.5 py-1 text-xs font-semibold text-blue-700 ring-1 ring-inset ring-blue-700/10">Default</span>
            
            <label class="relative inline-flex items-center cursor-pointer" title="Toggle Active Status">
              <input type="checkbox" :checked="lang.isActive" @change="store.toggleLanguageStatus(lang.code)" class="sr-only peer" :disabled="lang.isDefault">
              <div class="w-9 h-5 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-blue-600" :class="{ 'opacity-50 cursor-not-allowed': lang.isDefault }"></div>
            </label>
          </div>
        </div>
        
        <div class="space-y-2.5 flex-1">
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
        
        <div class="mt-5 pt-4 border-t border-gray-100 flex flex-wrap gap-2 justify-between items-center">
          <div class="flex gap-2">
            <button @click="editLang(lang)" class="text-xs font-medium text-gray-500 hover:text-blue-600 transition-colors p-1 rounded hover:bg-blue-50" title="Edit">
              <svg class="h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg>
            </button>
            <button v-if="!lang.isDefault" @click="store.setAsDefault(lang.code)" class="text-xs font-medium text-gray-500 hover:text-blue-600 transition-colors p-1 rounded hover:bg-blue-50" title="Set as Default">
              <svg class="h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon></svg>
            </button>
            <button v-if="!lang.isDefault" @click="store.deleteLanguage(lang.code)" class="text-xs font-medium text-gray-500 hover:text-red-600 transition-colors p-1 rounded hover:bg-red-50" title="Delete">
              <svg class="h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
            </button>
          </div>
          <div class="flex gap-2">
            <button @click="exportLang(lang.code)" class="text-xs font-medium text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1 p-1 rounded hover:bg-gray-100" title="Export JSON">
              <svg class="h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" x2="12" y1="15" y2="3"/></svg>
            </button>
            <button @click="triggerImport(lang.code)" class="text-xs font-medium text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1 p-1 rounded hover:bg-gray-100" title="Import JSON">
              <svg class="h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v-4a2 2 0 0 0-2-2H5a2 2 0 0 0-2 2v4"/><polyline points="17 8 12 3 7 8"/><line x1="12" x2="12" y1="3" y2="15"/></svg>
            </button>
          </div>
        </div>
      </div>
    </div>
    
    <input type="file" ref="fileInput" @change="handleImport" accept="application/json" class="hidden" />

    <!-- Translation Editor -->
    <div class="rounded-2xl border border-gray-200 bg-white shadow-sm overflow-hidden flex flex-col h-[650px] ring-1 ring-black/5">
      <TranslationsEditor />
    </div>

    <!-- Add Modal -->
    <div v-if="showAddModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-0">
      <div class="fixed inset-0 bg-gray-900/40 backdrop-blur-sm transition-opacity" @click="showAddModal = false"></div>
      <div class="relative transform overflow-hidden rounded-2xl bg-white p-6 text-left shadow-2xl transition-all sm:w-full sm:max-w-md border border-gray-200 animate-in fade-in zoom-in-95 duration-200 ease-out" role="dialog" aria-modal="true" aria-labelledby="language-modal-title" v-dialog="closeModal">
        <div class="flex items-center justify-between mb-5">
          <h3 id="language-modal-title" class="text-lg font-semibold text-gray-900">{{ isEditing ? 'Edit Language' : 'Add New Language' }}</h3>
          <button type="button" @click="closeModal" aria-label="Close" class="text-gray-400 hover:text-gray-500 transition-colors">
            <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
          </button>
        </div>
        <div class="space-y-4">
          <div>
            <label for="language-name" class="block text-sm font-medium text-gray-700 mb-1.5">Language Name</label>
            <input id="language-name" v-model="newLang.name" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. French" />
          </div>
          <div>
            <label for="language-code" class="block text-sm font-medium text-gray-700 mb-1.5">Code (ISO)</label>
            <input id="language-code" v-model="newLang.code" type="text" :disabled="isEditing" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all disabled:opacity-50 disabled:bg-gray-50" placeholder="e.g. fr" />
          </div>
          <div>
            <label for="language-flag" class="block text-sm font-medium text-gray-700 mb-1.5">Flag Emoji</label>
            <input id="language-flag" v-model="newLang.flag" type="text" class="block w-full rounded-lg border-gray-300 border px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 sm:text-sm transition-all" placeholder="e.g. 🇫🇷" />
          </div>
          <div class="grid grid-cols-2 gap-3 mt-4">
            <label class="flex items-center gap-2 bg-gray-50 p-3 rounded-lg border border-gray-100 cursor-pointer hover:bg-gray-100 transition-colors">
              <input v-model="newLang.isRtl" type="checkbox" class="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500" />
              <span class="text-sm font-medium text-gray-700 select-none">RTL Layout</span>
            </label>
            <label class="flex items-center gap-2 bg-gray-50 p-3 rounded-lg border border-gray-100 cursor-pointer hover:bg-gray-100 transition-colors">
              <input v-model="newLang.isActive" type="checkbox" class="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500" />
              <span class="text-sm font-medium text-gray-700 select-none">Active</span>
            </label>
          </div>
        </div>
        <div class="mt-6 flex justify-end gap-3 pt-4 border-t border-gray-100">
          <button @click="closeModal" class="rounded-lg px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-gray-200">Cancel</button>
          <button @click="handleSave" :disabled="!newLang.code || !newLang.name" class="rounded-lg bg-gray-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-gray-900 focus:ring-offset-2">{{ isEditing ? 'Save Changes' : 'Add Language' }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useLanguageStore } from '../store/languageStore'
import TranslationsEditor from '../components/TranslationsEditor.vue'

const store = useLanguageStore()
const showAddModal = ref(false)
const isEditing = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)
const importLangCode = ref('')

onMounted(() => {
  store.init()
})

const newLang = ref({
  code: '',
  name: '',
  flag: '',
  isRtl: false,
  isActive: true
})

const closeModal = () => {
  showAddModal.value = false
  newLang.value = { code: '', name: '', flag: '', isRtl: false, isActive: true }
  isEditing.value = false
}

const openAddModal = () => {
  newLang.value = { code: '', name: '', flag: '', isRtl: false, isActive: true }
  isEditing.value = false
  showAddModal.value = true
}

const editLang = (lang: any) => {
  newLang.value = { ...lang }
  isEditing.value = true
  showAddModal.value = true
}

const handleSave = () => {
  if (!newLang.value.code || !newLang.value.name) return
  if (isEditing.value) {
    store.updateLanguage(newLang.value.code, newLang.value)
  } else {
    store.addLanguage({
      ...newLang.value,
      isDefault: false
    })
  }
  closeModal()
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

const triggerImport = (code: string) => {
  importLangCode.value = code
  if (fileInput.value) {
    fileInput.value.click()
  }
}

const handleImport = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (!file || !importLangCode.value) return

  const reader = new FileReader()
  reader.onload = (e) => {
    const result = e.target?.result as string
    if (result) {
      store.importTranslations(result, importLangCode.value)
    }
    // reset
    target.value = ''
    importLangCode.value = ''
  }
  reader.readAsText(file)
}
</script>
