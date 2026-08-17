<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-gray-900/50 backdrop-blur-sm animate-fade-in" v-if="isOpen">
    <div class="bg-white rounded-2xl shadow-xl w-full max-w-2xl overflow-hidden animate-scale-in">
      <div class="p-6 border-b border-gray-100 flex items-center justify-between bg-gray-50/50">
        <h3 class="text-lg font-semibold text-gray-900">Import Tours (Excel)</h3>
        <button @click="close" class="text-gray-400 hover:text-gray-600 transition-colors">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
      
      <div class="p-8">
        <div 
          class="border-2 border-dashed rounded-xl p-10 text-center transition-all duration-200"
          :class="isDragging ? 'border-indigo-500 bg-indigo-50' : 'border-gray-200 hover:border-gray-300'"
          @dragover.prevent="isDragging = true"
          @dragleave.prevent="isDragging = false"
          @drop.prevent="handleDrop"
        >
          <div class="w-16 h-16 mx-auto mb-4 bg-indigo-100 rounded-full flex items-center justify-center">
            <svg class="w-8 h-8 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
            </svg>
          </div>
          <h4 class="text-lg font-medium text-gray-900 mb-1">Click to upload or drag and drop</h4>
          <p class="text-sm text-gray-500 mb-4">XLSX, XLS (Max 10MB)</p>
          <button @click="triggerFileInput" class="px-4 py-2 bg-white border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50 transition-colors shadow-sm">
            Select File
          </button>
          <input type="file" ref="fileInput" class="hidden" accept=".xlsx, .xls" @change="handleFileChange" />
        </div>

        <div v-if="selectedFile" class="mt-6">
          <div class="flex items-center justify-between p-4 bg-gray-50 rounded-lg border border-gray-100">
            <div class="flex items-center space-x-3">
              <svg class="w-8 h-8 text-green-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              <div>
                <p class="text-sm font-medium text-gray-900">{{ selectedFile.name }}</p>
                <p class="text-xs text-gray-500">{{ (selectedFile.size / 1024).toFixed(2) }} KB • Sheet: "Tours Data"</p>
              </div>
            </div>
            <button @click="selectedFile = null" class="text-gray-400 hover:text-red-500 transition-colors">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>

          <div v-if="hasErrors" class="mt-4 p-4 bg-red-50 rounded-lg border border-red-100">
            <h4 class="text-sm font-semibold text-red-800 flex items-center">
              <svg class="w-4 h-4 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
              Bulk Import Errors (2)
            </h4>
            <ul class="mt-2 space-y-1 text-xs text-red-700 list-disc list-inside">
              <li>Row 4: Missing required field "Duration"</li>
              <li>Row 7: Invalid price format "100USD"</li>
            </ul>
          </div>
          <div v-else class="mt-4 p-4 bg-blue-50 rounded-lg border border-blue-100">
            <h4 class="text-sm font-semibold text-blue-800">Sheet Preview</h4>
            <p class="text-xs text-blue-600 mt-1">12 valid tours found ready for import.</p>
          </div>
        </div>

        <div class="mt-6 flex items-center justify-between">
          <button @click="downloadTemplate" class="text-sm text-indigo-600 hover:text-indigo-700 font-medium flex items-center transition-colors">
            <svg class="w-4 h-4 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
            </svg>
            Download Template
          </button>
          <button 
            :disabled="!selectedFile"
            @click="importFile"
            class="px-5 py-2.5 bg-indigo-600 text-white rounded-lg text-sm font-medium hover:bg-indigo-700 focus:ring-4 focus:ring-indigo-100 transition-all shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Import Tours
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const props = defineProps({
  isOpen: Boolean
})

const emit = defineEmits(['close', 'import'])

const isDragging = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)
const selectedFile = ref<File | null>(null)
const hasErrors = ref(false) // Mocked state for UI preview

const close = () => {
  emit('close')
  selectedFile.value = null
}

const triggerFileInput = () => {
  fileInput.value?.click()
}

const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files?.length) {
    selectedFile.value = target.files[0]
  }
}

const handleDrop = (event: DragEvent) => {
  isDragging.value = false
  if (event.dataTransfer?.files.length) {
    const file = event.dataTransfer.files[0]
    if (file.name.endsWith('.xlsx') || file.name.endsWith('.xls')) {
      selectedFile.value = file
    }
  }
}

const downloadTemplate = () => {
  // Mock template download
  console.log('Downloading template...')
}

const importFile = () => {
  if (selectedFile.value) {
    emit('import', selectedFile.value)
    close()
  }
}
</script>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.2s ease-out;
}
.animate-scale-in {
  animation: scaleIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
@keyframes scaleIn {
  from { opacity: 0; transform: scale(0.95) translateY(10px); }
  to { opacity: 1; transform: scale(1) translateY(0); }
}
</style>
