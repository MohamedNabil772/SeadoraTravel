<script lang="ts">
import { ref } from 'vue'
const sharedMyIcons = ref<string[]>([])
</script>

<script setup lang="ts">
import api from '@/services/api'

const props = defineProps<{
  modelValue: string | null
}>()

const emit = defineEmits(['update:modelValue'])

const LuxuryIcons = [
  '⛵', '🏖️', '🏨', '🏝️', '🛳️', '🌴', '🏰', '🍷', '💎', '✈️', 
  '🌅', '🍹', '🏄', '🗺️', '🛎️', '🥂', '🏛️', '🌟', '🐬', '🦞'
]

const fileInput = ref<HTMLInputElement | null>(null)
const isUploading = ref(false)

function triggerUpload() {
  if (isUploading.value) return
  fileInput.value?.click()
}

function handleFile(event: Event) {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    processFile(target.files[0])
  }
  if (fileInput.value) fileInput.value.value = ''
}

function onDrop(event: DragEvent) {
  if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
    processFile(event.dataTransfer.files[0])
  }
}

async function processFile(file: File) {
  if (file.type.startsWith('image/')) {
    isUploading.value = true
    try {
      const formData = new FormData()
      formData.append('file', file)
      const res = await api.post('/api/files', formData)
      const fileId = res.data.fileId || res.data.FileId
      const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
      const url = `${API_URL}/api/files/${fileId}`
      
      if (!sharedMyIcons.value.includes(url)) {
        sharedMyIcons.value.push(url)
      }
      emit('update:modelValue', url)
    } catch (e) {
      console.error('Failed to upload custom icon', e)
    } finally {
      isUploading.value = false
    }
  }
}
</script>

<template>
  <div class="luxury-icon-picker-container">
    <div class="section-header">System Icons</div>
    <div class="luxury-icon-picker">
      <button
        type="button"
        class="icon-btn clear-btn"
        :class="{ active: !modelValue }"
        @click="emit('update:modelValue', null)"
        title="No Icon"
      >
        🚫
      </button>
      <button
        v-for="icon in LuxuryIcons"
        :key="icon"
        type="button"
        class="icon-btn"
        :class="{ active: modelValue === icon }"
        @click="emit('update:modelValue', icon)"
      >
        {{ icon }}
      </button>
    </div>

    <div class="section-header mt-4">My Icons</div>
    
    <div 
      class="dropzone" 
      :class="{ 'is-uploading': isUploading }"
      role="button"
      tabindex="0"
      aria-label="Upload a custom icon (SVG or PNG)"
      @click="triggerUpload"
      @keydown.enter.prevent="triggerUpload"
      @keydown.space.prevent="triggerUpload"
      @dragover.prevent
      @drop.prevent="onDrop"
    >
      <div v-if="isUploading" class="dropzone-content">
        <div class="loading-spinner"></div>
        <span>Uploading...</span>
      </div>
      <div v-else class="dropzone-content">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="17 8 12 3 7 8"></polyline><line x1="12" y1="3" x2="12" y2="15"></line></svg>
        <span>Click or Drag SVG/PNG here</span>
      </div>
      <input 
        type="file" 
        ref="fileInput" 
        style="display: none" 
        accept=".svg, .png, image/svg+xml, image/png"
        @change="handleFile"
      />
    </div>

    <div v-if="sharedMyIcons.length > 0" class="luxury-icon-picker mt-3">
      <button
        v-for="(icon, idx) in sharedMyIcons"
        :key="idx"
        type="button"
        class="icon-btn custom-icon-btn"
        :class="{ active: modelValue === icon }"
        @click="emit('update:modelValue', icon)"
      >
        <img :src="icon" alt="custom icon" />
      </button>
    </div>
  </div>
</template>

<style scoped>
.luxury-icon-picker-container {
  display: flex;
  flex-direction: column;
}
.section-header {
  font-size: 0.75rem;
  text-transform: uppercase;
  color: #8eafc2;
  margin-bottom: 8px;
  font-weight: 600;
  letter-spacing: 0.05em;
}
.mt-4 { margin-top: 16px; }
.mt-3 { margin-top: 12px; }

.luxury-icon-picker {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(40px, 1fr));
  gap: 8px;
  padding: 12px;
  background: rgba(0, 0, 0, 0.2);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.05);
}
.icon-btn {
  font-size: 20px;
  background: transparent;
  border: 1px solid transparent;
  border-radius: 6px;
  padding: 8px;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  display: flex;
  align-items: center;
  justify-content: center;
}
.icon-btn:hover {
  background: rgba(255, 255, 255, 0.1);
  transform: scale(1.1);
}
.icon-btn.active {
  background: rgba(26, 139, 196, 0.2);
  border-color: #1a8bc4;
  box-shadow: 0 0 12px rgba(26, 139, 196, 0.3);
  transform: scale(1.1);
}

.clear-btn {
  opacity: 0.6;
}
.clear-btn:hover {
  opacity: 1;
}

.custom-icon-btn img {
  max-width: 24px;
  max-height: 24px;
  object-fit: contain;
}

.dropzone {
  border: 2px dashed rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 24px;
  text-align: center;
  cursor: pointer;
  transition: all 0.2s ease;
  background: rgba(0, 0, 0, 0.1);
}
.dropzone:hover {
  border-color: rgba(26, 139, 196, 0.5);
  background: rgba(26, 139, 196, 0.05);
}
.dropzone-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  color: #8eafc2;
  font-size: 0.875rem;
}
.dropzone-content svg {
  color: #5c7585;
}
.dropzone:hover .dropzone-content {
  color: #fff;
}
.dropzone:hover .dropzone-content svg {
  color: #1a8bc4;
}

.loading-spinner {
  width: 24px;
  height: 24px;
  border: 2px solid rgba(26, 139, 196, 0.2);
  border-top-color: #1a8bc4;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}
.dropzone.is-uploading {
  cursor: wait;
}
</style>
