<script setup lang="ts">
import { ref } from 'vue'
import { Motion } from 'motion-v'

const props = defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const isDragging = ref(false)
const isUploading = ref(false)
const progress = ref(0)
const uploadedFile = ref<{ name: string; size: string; type: string } | null>(null)
const fileInput = ref<HTMLInputElement | null>(null)

const handleDragOver = (e: DragEvent) => {
  e.preventDefault()
  isDragging.value = true
}

const handleDragLeave = () => {
  isDragging.value = false
}

const simulateUpload = (file: File) => {
  if (!file) return
  isUploading.value = true
  progress.value = 0
  
  const sizeStr = (file.size / 1024 / 1024).toFixed(2) + ' MB'
  
  const interval = setInterval(() => {
    progress.value += Math.floor(Math.random() * 15) + 5
    if (progress.value >= 100) {
      progress.value = 100
      clearInterval(interval)
      setTimeout(() => {
        isUploading.value = false
        uploadedFile.value = {
          name: file.name,
          size: sizeStr,
          type: file.type
        }
        emit('update:modelValue', URL.createObjectURL(file))
      }, 300)
    }
  }, 100)
}

const handleDrop = (e: DragEvent) => {
  e.preventDefault()
  isDragging.value = false
  const files = e.dataTransfer?.files
  if (files && files.length > 0) {
    simulateUpload(files[0])
  }
}

const handleFileSelect = (e: Event) => {
  const target = e.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    simulateUpload(target.files[0])
  }
}

const removeFile = () => {
  uploadedFile.value = null
  progress.value = 0
  if (fileInput.value) {
    fileInput.value.value = ''
  }
  emit('update:modelValue', '')
}
</script>

<template>
  <div class="w-full">
    <!-- Upload Zone -->
    <div 
      v-if="!uploadedFile && !isUploading"
      class="relative flex flex-col items-center justify-center w-full px-6 py-8 border-2 border-dashed rounded-xl transition-all duration-200 cursor-pointer group"
      :class="isDragging ? 'border-neutral-900 bg-neutral-50/50' : 'border-neutral-200 hover:border-neutral-300 hover:bg-neutral-50'"
      @dragover="handleDragOver"
      @dragleave="handleDragLeave"
      @drop="handleDrop"
      @click="fileInput?.click()"
    >
      <input 
        ref="fileInput"
        type="file" 
        class="hidden" 
        accept="image/*,.pdf"
        @change="handleFileSelect"
      />
      <div class="p-3 mb-3 bg-neutral-100 rounded-full group-hover:bg-neutral-200/50 transition-colors">
        <svg class="w-6 h-6 text-neutral-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
        </svg>
      </div>
      <p class="text-sm font-medium text-neutral-700 mb-1">Click to upload or drag and drop</p>
      <p class="text-xs text-neutral-500">SVG, PNG, JPG or PDF (max. 10MB)</p>
    </div>

    <!-- Upload Progress -->
    <Motion
      v-else-if="isUploading"
      initial="{ opacity: 0, scale: 0.95 }"
      animate="{ opacity: 1, scale: 1 }"
      class="w-full p-4 border border-neutral-200 rounded-xl bg-white shadow-sm"
    >
      <div class="flex items-center gap-4 mb-3">
        <div class="p-2 bg-neutral-50 rounded-lg">
          <svg class="w-5 h-5 text-neutral-500 animate-pulse" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
          </svg>
        </div>
        <div class="flex-1">
          <div class="flex justify-between mb-1">
            <span class="text-sm font-medium text-neutral-700">Uploading document...</span>
            <span class="text-sm text-neutral-500 font-medium">{{ progress }}%</span>
          </div>
          <div class="w-full h-1.5 bg-neutral-100 rounded-full overflow-hidden">
            <div 
              class="h-full bg-neutral-900 transition-all duration-200 ease-out rounded-full"
              :style="{ width: `${progress}%` }"
            ></div>
          </div>
        </div>
      </div>
    </Motion>

    <!-- Uploaded File -->
    <Motion
      v-else-if="uploadedFile"
      initial="{ opacity: 0, y: 10 }"
      animate="{ opacity: 1, y: 0 }"
      class="flex items-center justify-between p-3 border border-neutral-200 rounded-xl bg-white shadow-sm hover:border-neutral-300 transition-colors"
    >
      <div class="flex items-center gap-3 overflow-hidden">
        <div class="flex-shrink-0 p-2.5 bg-neutral-50 rounded-lg">
          <svg class="w-5 h-5 text-neutral-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
        </div>
        <div class="min-w-0 flex-1">
          <p class="text-sm font-medium text-neutral-900 truncate">{{ uploadedFile.name }}</p>
          <p class="text-xs text-neutral-500">{{ uploadedFile.size }}</p>
        </div>
      </div>
      <button 
        @click="removeFile"
        class="p-2 text-neutral-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors outline-none focus:ring-2 focus:ring-red-500/20"
        title="Remove file"
      >
        <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
        </svg>
      </button>
    </Motion>
  </div>
</template>
