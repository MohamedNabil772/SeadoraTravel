<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import LocaleTabs from '@/shared/components/locale/LocaleTabs.vue'
import LocalizedInput from '@/shared/components/locale/LocalizedInput.vue'
import LocalizedTextarea from '@/shared/components/locale/LocalizedTextarea.vue'
import api from '@/services/api'

const props = defineProps<{
  modelValue: boolean
  isEdit: boolean
  destination: any
  actionLoading: boolean
}>()

const emit = defineEmits(['update:modelValue', 'save'])

const appLocales = [
  { code: 'en', label: 'EN', flag: '🇬🇧' },
  { code: 'de', label: 'DE', flag: '🇩🇪' },
  { code: 'it', label: 'IT', flag: '🇮🇹' },
  { code: 'fr', label: 'FR', flag: '🇫🇷' },
  { code: 'ru', label: 'RU', flag: '🇷🇺' }
]

const locales = computed(() => {
  return appLocales.map(lang => ({
    code: lang.code,
    label: lang.label,
    flag: lang.flag,
    status: 'complete' as const
  }))
})

const activeLang = ref('en')

const form = ref({
  id: '',
  names: {} as Record<string, string>,
  descriptions: {} as Record<string, string>,
  highlights: {} as Record<string, string>,
  imageUrl: '',
  flag: '📍',
  isFeatured: false
})

const isDragging = ref(false)
const isUploading = ref(false)
const uploadError = ref('')
const fileInput = ref<HTMLInputElement | null>(null)

function initForm(dest: any = null) {
  const names: Record<string, string> = {}
  const descriptions: Record<string, string> = {}
  const highlights: Record<string, string> = {}
  
  appLocales.forEach(lang => {
    names[lang.code] = dest?.names?.[lang.code] || ''
    descriptions[lang.code] = dest?.descriptions?.[lang.code] || ''
    highlights[lang.code] = dest?.highlights?.[lang.code] || ''
  })
  
  form.value = {
    id: dest?.id || '',
    names,
    descriptions,
    highlights,
    imageUrl: dest?.imageUrl || '',
    flag: dest?.flagEmoji || dest?.flag || '📍',
    isFeatured: dest?.isFeatured || false
  }
}

watch(() => props.modelValue, (val) => {
  if (val) {
    if (props.isEdit && props.destination) {
      initForm(props.destination)
    } else {
      initForm(null)
    }
    // Also reset language tab to default
    activeLang.value = 'en'
    uploadError.value = ''
  }
})

function close() {
  if (props.actionLoading) return
  emit('update:modelValue', false)
}

function save() {
  emit('save', { ...form.value })
}

function triggerFileInput() {
  fileInput.value?.click()
}

// Drag & Drop Image Handlers
function onDragOver(e: DragEvent) {
  e.preventDefault()
  isDragging.value = true
}
function onDragLeave(e: DragEvent) {
  e.preventDefault()
  isDragging.value = false
}
async function onDrop(e: DragEvent) {
  e.preventDefault()
  isDragging.value = false
  if (e.dataTransfer?.files && e.dataTransfer.files.length > 0) {
    const file = e.dataTransfer.files[0]
    await uploadFile(file)
  }
}
async function onFileSelected(e: Event) {
  const target = e.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    await uploadFile(target.files[0])
  }
  // reset input so same file can be selected again
  if (fileInput.value) fileInput.value.value = ''
}
async function uploadFile(file: File) {
  if (!file.type.startsWith('image/')) {
    uploadError.value = 'Please select an image file.'
    return
  }
  isUploading.value = true
  uploadError.value = ''
  const formData = new FormData()
  formData.append('file', file)
  try {
    const res = await api.post('/api/files', formData)
    const fileId = res.data.fileId || res.data.FileId
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    form.value.imageUrl = `${API_URL}/api/files/${fileId}`
  } catch (err: any) {
    uploadError.value = 'Failed to upload image. Please try again.'
    console.error('Upload failed:', err)
  } finally {
    isUploading.value = false
  }
}
function removeImage() {
  form.value.imageUrl = ''
}

function resolveImageUrl(url?: string): string {
  if (!url) return ''
  if (url.startsWith('http://') || url.startsWith('https://')) return url
  const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
  if (url.startsWith('/api/files/') || url.startsWith('api/files/')) {
    const cleanPath = url.startsWith('/') ? url : `/${url}`
    return `${API_URL}${cleanPath}`
  }
  return url
}

// Flag Selector logic
const showFlagSelector = ref(false)
const flagSearch = ref('')

const worldFlags = [
  { flag: '🇪🇬', name: 'Egypt' },
  { flag: '🇦🇪', name: 'United Arab Emirates' },
  { flag: '🇸🇦', name: 'Saudi Arabia' },
  { flag: '🇯🇴', name: 'Jordan' },
  { flag: '🇱🇧', name: 'Lebanon' },
  { flag: '🇴🇲', name: 'Oman' },
  { flag: '🇶🇦', name: 'Qatar' },
  { flag: '🇧🇭', name: 'Bahrain' },
  { flag: '🇰🇼', name: 'Kuwait' },
  { flag: '🇲🇦', name: 'Morocco' },
  { flag: '🇹🇷', name: 'Turkey' },
  { flag: '🇬🇷', name: 'Greece' },
  { flag: '🇮🇹', name: 'Italy' },
  { flag: '🇫🇷', name: 'France' },
  { flag: '🇪🇸', name: 'Spain' },
  { flag: '🇩🇪', name: 'Germany' },
  { flag: '🇬🇧', name: 'United Kingdom' },
  { flag: '🇺🇸', name: 'United States' },
  { flag: '🇯🇵', name: 'Japan' },
  { flag: '🇹🇭', name: 'Thailand' },
  { flag: '🇲🇻', name: 'Maldives' },
  { flag: '🇮🇩', name: 'Indonesia' },
  { flag: '🇲🇾', name: 'Malaysia' },
  { flag: '🇻🇳', name: 'Vietnam' },
  { flag: '🇮🇳', name: 'India' },
  { flag: '🇿🇦', name: 'South Africa' },
  { flag: '🇧🇷', name: 'Brazil' },
  { flag: '🇲🇽', name: 'Mexico' },
  { flag: '🇨🇦', name: 'Canada' },
  { flag: '🇦🇺', name: 'Australia' },
  { flag: '🇨🇭', name: 'Switzerland' },
  { flag: '🇦🇹', name: 'Austria' },
  { flag: '🇵🇹', name: 'Portugal' },
  { flag: '🇳🇱', name: 'Netherlands' },
  { flag: '📍', name: 'Pin (Default)' },
  { flag: '🏖️', name: 'Beach' },
  { flag: '⛰️', name: 'Mountain' },
  { flag: '🏛️', name: 'Museum' }
]

const filteredFlags = computed(() => {
  if (!flagSearch.value) return worldFlags
  const query = flagSearch.value.toLowerCase()
  return worldFlags.filter(f => f.name.toLowerCase().includes(query))
})

function selectFlag(flag: string) {
  form.value.flag = flag
  showFlagSelector.value = false
  flagSearch.value = ''
}
</script>

<template>
  <Transition name="modal-fade">
    <div v-if="modelValue" class="modal-overlay" @click.self="close">
      <div class="modal-container" role="dialog" aria-modal="true" aria-labelledby="destination-modal-title" v-dialog="close">
        <div class="modal-header">
          <h3 id="destination-modal-title">{{ isEdit ? 'Edit Destination' : 'New Destination' }}</h3>
          <button type="button" @click="close" class="btn-close" aria-label="Close" :disabled="actionLoading">✕</button>
        </div>
        
        <div class="modal-content" @click="showFlagSelector = false">
          <!-- Cover Image Dropzone -->
          <div class="form-group">
            <label>Cover Image</label>
            <div 
              class="image-dropzone" 
              :class="{ 'is-dragging': isDragging, 'has-image': !!form.imageUrl, 'is-uploading': isUploading }"
              role="button"
              tabindex="0"
              aria-label="Upload cover image"
              @dragover="onDragOver"
              @dragleave="onDragLeave"
              @drop="onDrop"
              @click="!isUploading && !form.imageUrl && triggerFileInput()"
              @keydown.enter.prevent="!isUploading && !form.imageUrl && triggerFileInput()"
              @keydown.space.prevent="!isUploading && !form.imageUrl && triggerFileInput()"
            >
              <input 
                ref="fileInput" 
                type="file" 
                accept="image/*" 
                class="hidden" 
                style="display:none"
                @change="onFileSelected" 
              />
              <div v-if="isUploading" class="dropzone-loading">
                <div class="loading-spinner"></div>
                <p>Uploading image...</p>
              </div>
              <div v-else-if="!form.imageUrl" class="dropzone-placeholder">
                <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mb-2 text-muted"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><circle cx="8.5" cy="8.5" r="1.5"></circle><polyline points="21 15 16 10 5 21"></polyline></svg>
                <p>Drag & drop image here or click to browse</p>
                <span class="text-xs text-muted">Supports JPG, PNG (Max 5MB)</span>
              </div>
              <div v-else class="dropzone-preview">
                <img :src="resolveImageUrl(form.imageUrl)" alt="Cover preview" />
                <div class="preview-actions">
                  <button type="button" class="btn-remove" @click.stop="removeImage" :disabled="isUploading">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18"></path><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"></path><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"></path></svg>
                    Remove
                  </button>
                </div>
              </div>
            </div>
            <div v-if="uploadError" class="text-xs text-red-500 mt-1">{{ uploadError }}</div>
            <div class="url-input mt-2">
              <input v-model="form.imageUrl" type="text" placeholder="Or paste image URL..." aria-label="Paste image URL" class="form-input" :disabled="isUploading" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group flex-1">
              <label>Flag / Icon</label>
              <div class="flag-selector-wrapper">
                <button type="button" class="flag-selector-btn" @click.stop="showFlagSelector = !showFlagSelector">
                  <span class="text-2xl">{{ form.flag }}</span>
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-muted"><polyline points="6 9 12 15 18 9"></polyline></svg>
                </button>
                
                <Transition name="pop-in">
                  <div v-if="showFlagSelector" class="flag-dropdown glass-panel" @click.stop>
                    <div class="flag-search">
                      <input v-model="flagSearch" type="text" placeholder="Search countries..." aria-label="Search countries" class="form-input text-sm" />
                    </div>
                    <div class="flag-grid">
                      <button 
                        v-for="item in filteredFlags" 
                        :key="item.name"
                        type="button"
                        class="flag-item"
                        :title="item.name"
                        @click="selectFlag(item.flag)"
                      >
                        {{ item.flag }}
                      </button>
                    </div>
                    <div v-if="filteredFlags.length === 0" class="no-flags">No matches</div>
                  </div>
                </Transition>
              </div>
            </div>
            <div class="form-group flex-2 toggle-group">
              <label>Featured Destination</label>
              <label class="switch">
                <input type="checkbox" v-model="form.isFeatured">
                <span class="slider round"></span>
              </label>
            </div>
          </div>

          <div class="form-group">
            <label>Localized Content</label>
            <div class="locale-wrapper">
              <LocaleTabs v-model="activeLang" :locales="locales" class="mb-4" />
              
              <div class="space-y-4">
                <div>
                  <label class="sub-label">Name</label>
                  <LocalizedInput
                    v-model="form.names[activeLang]"
                    :locale="activeLang"
                    :placeholder="`Enter name in ${activeLang.toUpperCase()}`"
                  />
                </div>
                <div>
                  <label class="sub-label">Description</label>
                  <LocalizedTextarea
                    v-model="form.descriptions[activeLang]"
                    :locale="activeLang"
                    :placeholder="`Enter description in ${activeLang.toUpperCase()}`"
                    :rows="4"
                  />
                </div>
                <div>
                  <label class="sub-label">Highlights</label>
                  <LocalizedTextarea
                    v-model="form.highlights[activeLang]"
                    :locale="activeLang"
                    :placeholder="`Enter highlights in ${activeLang.toUpperCase()}`"
                    :rows="3"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div class="modal-footer">
          <button @click="close" class="btn-cancel" :disabled="actionLoading">Cancel</button>
          <button @click="save" class="btn-save" :disabled="actionLoading">
            {{ actionLoading ? 'Saving...' : 'Save Destination' }}
          </button>
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.4);
  backdrop-filter: blur(4px);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease;
}
.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-active .modal-container {
  animation: modalScaleUp 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}
.modal-fade-leave-active .modal-container {
  animation: modalScaleDown 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

@keyframes modalScaleUp {
  from { opacity: 0; transform: scale(0.95) translateY(10px); }
  to { opacity: 1; transform: scale(1) translateY(0); }
}
@keyframes modalScaleDown {
  from { opacity: 1; transform: scale(1) translateY(0); }
  to { opacity: 0; transform: scale(0.95) translateY(10px); }
}

.modal-container {
  width: 100%;
  max-width: 600px;
  max-height: 90vh;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.12);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.modal-header {
  padding: 20px 24px;
  border-bottom: 1px solid #e2e8f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #ffffff;
}
.modal-header h3 { margin: 0; color: #0b1b3d; font-size: 1.125rem; font-weight: 600; letter-spacing: -0.01em; }
.btn-close { background: none; border: none; color: #64748b; cursor: pointer; font-size: 1.25rem; transition: color 0.2s; padding: 4px; border-radius: 4px; }
.btn-close:hover { color: #0f172a; background: #f1f5f9; }

.modal-content {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.modal-content::-webkit-scrollbar { width: 6px; }
.modal-content::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 3px; }

.modal-footer {
  padding: 20px 24px;
  border-top: 1px solid #e2e8f0;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  background: #f8fafc;
}

.form-group { display: flex; flex-direction: column; gap: 8px; }
.form-row { display: flex; gap: 20px; align-items: flex-end; }
.flex-1 { flex: 1; }
.flex-2 { flex: 2; }
.form-group label { color: #334155; font-size: 0.75rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.05em; }
.sub-label { color: #334155; font-size: 0.75rem; font-weight: 700; display: block; margin-bottom: 6px; text-transform: uppercase; }

.form-input {
  width: 100%;
  padding: 10px 14px;
  background: #ffffff;
  border: 1.5px solid #cbd5e1;
  border-radius: 8px;
  color: #0f172a;
  outline: none;
  font-size: 14px;
  transition: all 0.2s;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}
.form-input:focus { border-color: #0b1b3d; box-shadow: 0 0 0 3px rgba(11, 27, 61, 0.1); }

.mt-2 { margin-top: 8px; }
.mb-2 { margin-bottom: 8px; }
.mb-4 { margin-bottom: 16px; display: block; }
.text-2xl { font-size: 1.5rem; line-height: 1; }
.text-sm { font-size: 0.875rem; }
.text-xs { font-size: 0.75rem; }
.text-muted { color: #64748b; }
.space-y-4 > * + * { margin-top: 16px; }

/* Dropzone */
.image-dropzone {
  width: 100%;
  height: 180px;
  border: 2px dashed #cbd5e1;
  border-radius: 12px;
  background: #f8fafc;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  position: relative;
  overflow: hidden;
  cursor: pointer;
}
.image-dropzone:hover, .image-dropzone.is-dragging {
  border-color: #0b1b3d;
  background: #f1f5f9;
}
.image-dropzone.has-image {
  border: 1px solid #e2e8f0;
  padding: 4px;
  background: #ffffff;
}
.dropzone-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  pointer-events: none;
}
.dropzone-placeholder p { color: #0f172a; font-size: 14px; margin: 0 0 4px; font-weight: 500; }
.dropzone-preview {
  width: 100%;
  height: 100%;
  position: relative;
  border-radius: 8px;
  overflow: hidden;
}
.dropzone-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.preview-actions {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.2s;
  backdrop-filter: blur(2px);
}
.dropzone-preview:hover .preview-actions { opacity: 1; }
.btn-remove {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  background: #ef4444;
  color: #fff;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transform: translateY(10px);
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
}
.dropzone-preview:hover .btn-remove { transform: translateY(0); }
.btn-remove:hover:not(:disabled) { background: #dc2626; transform: translateY(-2px); }
.btn-remove:disabled { opacity: 0.5; cursor: not-allowed; }

.dropzone-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;
  color: #0b1b3d;
}
.loading-spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #e2e8f0;
  border-top-color: #0b1b3d;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Flag Selector */
.flag-selector-wrapper {
  position: relative;
}
.flag-selector-btn {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px 14px;
  background: #ffffff;
  border: 1.5px solid #cbd5e1;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}
.flag-selector-btn:focus, .flag-selector-btn:hover { border-color: #0b1b3d; }

.pop-in-enter-active, .pop-in-leave-active {
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}
.pop-in-enter-from, .pop-in-leave-to {
  opacity: 0;
  transform: scale(0.95) translateY(-5px);
}

.flag-dropdown {
  position: absolute;
  bottom: calc(100% + 8px);
  left: 0;
  width: 280px;
  z-index: 10;
  padding: 12px;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  transform-origin: bottom left;
}
.glass-panel {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.12);
}

.flag-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 8px;
  max-height: 200px;
  overflow-y: auto;
  padding-right: 4px;
}
.flag-grid::-webkit-scrollbar { width: 4px; }
.flag-grid::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 2px; }

.flag-item {
  background: transparent;
  border: 1px solid transparent;
  font-size: 1.5rem;
  padding: 8px 4px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  justify-content: center;
  align-items: center;
}
.flag-item:hover { background: #f1f5f9; transform: scale(1.1); }
.no-flags { text-align: center; color: #64748b; font-size: 0.875rem; padding: 12px 0; }

/* Switch Toggle */
.toggle-group {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 12px;
}
.switch { position: relative; display: inline-block; width: 44px; height: 24px; }
.switch input { opacity: 0; width: 0; height: 0; }
.slider {
  position: absolute; cursor: pointer; top: 0; left: 0; right: 0; bottom: 0;
  background-color: #cbd5e1; transition: 0.3s cubic-bezier(0.16, 1, 0.3, 1); border-radius: 24px;
}
.slider:before {
  position: absolute; content: ""; height: 18px; width: 18px; left: 3px; bottom: 3px;
  background-color: #ffffff; transition: 0.3s cubic-bezier(0.16, 1, 0.3, 1); border-radius: 50%;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}
input:checked + .slider { background-color: #0b1b3d; }
input:checked + .slider:before { transform: translateX(20px); }

.locale-wrapper {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
}

.btn-cancel { 
  padding: 10px 20px; 
  background: #ffffff; 
  border: 1px solid #cbd5e1; 
  border-radius: 8px; 
  color: #475569; 
  cursor: pointer; 
  font-weight: 600; 
  transition: all 0.2s; 
}
.btn-cancel:hover:not(:disabled) { background: #f8fafc; color: #0f172a; border-color: #94a3b8; }

.btn-save { 
  padding: 10px 24px; 
  background: linear-gradient(135deg, #0b1b3d, #1e3a8a); 
  border: none; 
  border-radius: 8px; 
  color: #ffffff; 
  font-weight: 600; 
  cursor: pointer; 
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1); 
  box-shadow: 0 4px 12px rgba(11, 27, 61, 0.2); 
}
.btn-save:hover:not(:disabled) { 
  transform: translateY(-1px); 
  box-shadow: 0 6px 16px rgba(11, 27, 61, 0.3); 
}
.btn-save:active:not(:disabled) {
  transform: translateY(1px) scale(0.97);
}
.btn-save:disabled { opacity: 0.6; cursor: not-allowed; }
</style>
