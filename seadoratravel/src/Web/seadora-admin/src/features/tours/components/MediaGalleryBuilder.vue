<template>
  <div class="space-y-6">
    <div class="border-2 border-dashed border-gray-300 rounded-xl p-8 text-center transition-colors cursor-pointer"
         :class="isUploading ? 'bg-indigo-50 border-indigo-300' : 'bg-gray-50 hover:bg-gray-100'"
         @click="!isUploading && triggerFileInput()">
      <div v-if="isUploading" class="flex flex-col items-center justify-center">
        <svg class="animate-spin h-10 w-10 text-indigo-600 mb-3" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <p class="text-sm font-medium text-indigo-700">Uploading images... {{ uploadProgress }}</p>
      </div>
      <div v-else class="flex flex-col items-center justify-center">
        <svg class="w-12 h-12 text-gray-400 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
        <p class="text-sm font-medium text-gray-900">Click to upload images</p>
        <p class="mt-1 text-xs text-gray-500">PNG, JPG, GIF up to 10MB</p>
      </div>
      <input ref="fileInput" type="file" multiple accept="image/*" class="hidden" @change="handleFileUpload" />
    </div>

    <!-- Gallery Grid -->
    <div v-if="form.mediaGallery && form.mediaGallery.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="(item, index) in form.mediaGallery" :key="index" class="relative group bg-white rounded-lg overflow-hidden border border-gray-200 shadow-sm flex flex-col">
        <div class="relative aspect-video">
          <img :src="item.url" :alt="item.caption || 'Tour image'" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex flex-col items-center justify-center gap-2">
            <button @click.stop="removeMedia(Number(index))" type="button" class="px-3 py-2 bg-red-600/90 hover:bg-red-600 text-white font-medium rounded-lg backdrop-blur-sm transition-all transform hover:scale-105 shadow-lg flex items-center gap-2">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
              Remove
            </button>
          </div>
        </div>
        <div class="p-4 flex-1">
          <input v-model="item.caption" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 text-sm" placeholder="Enter caption for alt tag" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, inject } from 'vue'
import api from '@/services/api'

const form = inject<any>('tourForm')
const fileInput = ref<HTMLInputElement | null>(null)
const isUploading = ref(false)
const uploadProgress = ref('')

if (!form.value.mediaGallery) {
  form.value.mediaGallery = []
}
if (form.value.mediaUrls && form.value.mediaUrls.length > 0 && form.value.mediaGallery.length === 0) {
  form.value.mediaGallery = form.value.mediaUrls.map((url: string) => ({ url, caption: '' }))
}

const triggerFileInput = () => {
  fileInput.value?.click()
}

const handleFileUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement
  if (!target.files?.length) return

  const files = Array.from(target.files).filter(f => f.type.startsWith('image/'))
  if (files.length === 0) return

  const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
  isUploading.value = true
  
  let uploadedCount = 0
  
  for (const file of files) {
    uploadedCount++
    uploadProgress.value = `(${uploadedCount}/${files.length})`
    const formData = new FormData()
    formData.append('file', file)
    try {
      const res = await api.post('/api/files', formData)
      const fileId = res.data.fileId || res.data.FileId
      const url = `${API_URL}/api/files/${fileId}`
      form.value.mediaGallery.push({ url, caption: '' })
      // keep mediaUrls in sync
      if (!form.value.mediaUrls) form.value.mediaUrls = []
      form.value.mediaUrls.push(url)
    } catch (e) {
      console.error('Failed to upload file', e)
    }
  }

  isUploading.value = false
  uploadProgress.value = ''
  target.value = ''
}

const removeMedia = (index: number) => {
  form.value.mediaGallery.splice(index, 1)
  if (form.value.mediaUrls) {
    form.value.mediaUrls.splice(index, 1)
  }
}
</script>
