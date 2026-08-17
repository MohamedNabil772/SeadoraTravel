<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue'
import LocaleTabs from '@/shared/components/locale/LocaleTabs.vue'
import LocalizedInput from '@/shared/components/locale/LocalizedInput.vue'
import LocalizedTextarea from '@/shared/components/locale/LocalizedTextarea.vue'

const props = defineProps<{
  modelValue: boolean
  isEdit: boolean
  destination: any
  actionLoading: boolean
}>()

const emit = defineEmits(['update:modelValue', 'save'])

const activeLang = ref('en')
const locales = [
  { code: 'en', label: 'EN', flag: '🇬🇧', status: 'complete' as const },
  { code: 'de', label: 'DE', flag: '🇩🇪', status: 'complete' as const },
  { code: 'it', label: 'IT', flag: '🇮🇹', status: 'complete' as const },
  { code: 'fr', label: 'FR', flag: '🇫🇷', status: 'complete' as const },
  { code: 'ru', label: 'RU', flag: '🇷🇺', status: 'complete' as const }
]

const form = ref({
  id: '',
  names: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  descriptions: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  imageUrl: '',
  flag: '📍',
  isFeatured: false
})

const isDragging = ref(false)

watch(() => props.modelValue, (val) => {
  if (val) {
    if (props.isEdit && props.destination) {
      form.value = {
        id: props.destination.id,
        names: { ...props.destination.names },
        descriptions: props.destination.descriptions ? { ...props.destination.descriptions } : { en: '', de: '', it: '', fr: '', ru: '' },
        imageUrl: props.destination.imageUrl,
        flag: props.destination.flag || '📍',
        isFeatured: props.destination.isFeatured || false
      }
    } else {
      form.value = {
        id: '',
        names: { en: '', de: '', it: '', fr: '', ru: '' },
        descriptions: { en: '', de: '', it: '', fr: '', ru: '' },
        imageUrl: '',
        flag: '📍',
        isFeatured: false
      }
    }
  }
})

function close() {
  if (props.actionLoading) return
  emit('update:modelValue', false)
}

function save() {
  emit('save', { ...form.value })
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
function onDrop(e: DragEvent) {
  e.preventDefault()
  isDragging.value = false
  if (e.dataTransfer?.files && e.dataTransfer.files.length > 0) {
    const file = e.dataTransfer.files[0]
    // Normally we'd upload this file to the server and get a URL back.
    // For now, if it's an image, let's create a local object URL or just alert.
    if (file.type.startsWith('image/')) {
      // In a real app: uploadImage(file).then(url => form.value.imageUrl = url)
      // Simulating a success upload with a placeholder for demonstration:
      form.value.imageUrl = URL.createObjectURL(file)
    }
  }
}
function removeImage() {
  form.value.imageUrl = ''
}
</script>

<template>
  <div v-if="modelValue" class="drawer-overlay" @click.self="close">
    <div class="drawer">
      <div class="drawer-header">
        <h3>{{ isEdit ? 'Edit Destination' : 'New Destination' }}</h3>
        <button @click="close" class="btn-close" :disabled="actionLoading">✕</button>
      </div>
      
      <div class="drawer-content">
        <!-- Cover Image Dropzone -->
        <div class="form-group">
          <label>Cover Image</label>
          <div 
            class="image-dropzone" 
            :class="{ 'is-dragging': isDragging, 'has-image': !!form.imageUrl }"
            @dragover="onDragOver"
            @dragleave="onDragLeave"
            @drop="onDrop"
          >
            <div v-if="!form.imageUrl" class="dropzone-placeholder">
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mb-2 text-muted"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><circle cx="8.5" cy="8.5" r="1.5"></circle><polyline points="21 15 16 10 5 21"></polyline></svg>
              <p>Drag & drop image here or click to browse</p>
              <span class="text-xs text-muted">Supports JPG, PNG (Max 5MB)</span>
            </div>
            <div v-else class="dropzone-preview">
              <img :src="form.imageUrl" alt="Cover preview" />
              <div class="preview-actions">
                <button type="button" class="btn-remove" @click.stop="removeImage">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18"></path><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"></path><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"></path></svg>
                  Remove
                </button>
              </div>
            </div>
          </div>
          <div class="url-input mt-2">
            <input v-model="form.imageUrl" type="text" placeholder="Or paste image URL..." class="form-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group flex-1">
            <label>Flag Emoji</label>
            <input v-model="form.flag" type="text" placeholder="e.g. 🇪🇬" class="form-input text-2xl text-center w-20" />
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
            </div>
          </div>
        </div>
      </div>
      
      <div class="drawer-footer">
        <button @click="close" class="btn-cancel" :disabled="actionLoading">Cancel</button>
        <button @click="save" class="btn-save" :disabled="actionLoading">
          {{ actionLoading ? 'Saving...' : 'Save Destination' }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.drawer-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.6);
  backdrop-filter: blur(4px);
  z-index: 2000;
  display: flex;
  justify-content: flex-end;
}
.drawer {
  width: 100%;
  max-width: 500px;
  background: #0a1929;
  border-left: 1px solid rgba(255,255,255,0.1);
  display: flex;
  flex-direction: column;
  animation: slideIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
@keyframes slideIn {
  from { transform: translateX(100%); }
  to { transform: translateX(0); }
}
.drawer-header {
  padding: 24px;
  border-bottom: 1px solid rgba(255,255,255,0.06);
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.drawer-header h3 { margin: 0; color: #fff; font-size: 1.125rem; font-weight: 600; }
.btn-close { background: none; border: none; color: #8eafc2; cursor: pointer; font-size: 1.25rem; transition: color 0.2s; }
.btn-close:hover { color: #fff; }

.drawer-content {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.drawer-footer {
  padding: 20px 24px;
  border-top: 1px solid rgba(255,255,255,0.06);
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  background: rgba(0,0,0,0.2);
}

.form-group { display: flex; flex-direction: column; gap: 8px; }
.form-row { display: flex; gap: 16px; align-items: flex-end; }
.flex-1 { flex: 1; }
.flex-2 { flex: 2; }
.form-group label { color: #8eafc2; font-size: 0.875rem; font-weight: 500; text-transform: uppercase; letter-spacing: 0.05em; }
.sub-label { color: #8eafc2; font-size: 0.75rem; font-weight: 500; display: block; margin-bottom: 6px; }

.form-input {
  width: 100%;
  padding: 10px 12px;
  background: rgba(0,0,0,0.2);
  border: 1px solid rgba(255,255,255,0.1);
  border-radius: 8px;
  color: #fff;
  outline: none;
  font-size: 14px;
  transition: border-color 0.2s;
}
.form-input:focus { border-color: #1a8bc4; }
.mt-2 { margin-top: 8px; }
.mb-2 { margin-bottom: 8px; }
.mb-4 { margin-bottom: 16px; display: block; }
.text-2xl { font-size: 1.5rem; }
.text-center { text-align: center; }
.w-20 { width: 80px; }
.text-xs { font-size: 0.75rem; }
.text-muted { color: #8eafc2; }
.space-y-4 > * + * { margin-top: 16px; }

/* Dropzone */
.image-dropzone {
  width: 100%;
  height: 160px;
  border: 2px dashed rgba(255,255,255,0.15);
  border-radius: 12px;
  background: rgba(0,0,0,0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  position: relative;
  overflow: hidden;
  cursor: pointer;
}
.image-dropzone:hover, .image-dropzone.is-dragging {
  border-color: #1a8bc4;
  background: rgba(26,139,196,0.1);
}
.image-dropzone.has-image {
  border-style: solid;
  border-color: rgba(255,255,255,0.1);
  padding: 4px;
}
.dropzone-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  pointer-events: none;
}
.dropzone-placeholder p { color: #fff; font-size: 14px; margin: 0 0 4px; font-weight: 500; }
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
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.2s;
}
.dropzone-preview:hover .preview-actions { opacity: 1; }
.btn-remove {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  background: #dc3545;
  color: #fff;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transform: translateY(10px);
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}
.dropzone-preview:hover .btn-remove { transform: translateY(0); }
.btn-remove:hover { background: #c82333; }

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
  background-color: rgba(255,255,255,0.1); transition: .4s; border-radius: 24px;
}
.slider:before {
  position: absolute; content: ""; height: 18px; width: 18px; left: 3px; bottom: 3px;
  background-color: #8eafc2; transition: .4s; border-radius: 50%;
}
input:checked + .slider { background-color: #1a8bc4; }
input:checked + .slider:before { transform: translateX(20px); background-color: #fff; }

.locale-wrapper {
  background: rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(255,255,255,0.05);
  border-radius: 12px;
  padding: 16px;
}

.btn-cancel { padding: 10px 20px; background: transparent; border: 1px solid rgba(255,255,255,0.1); border-radius: 8px; color: #8eafc2; cursor: pointer; font-weight: 500; transition: all 0.2s; }
.btn-cancel:hover:not(:disabled) { background: rgba(255,255,255,0.05); color: #fff; }
.btn-save { padding: 10px 24px; background: linear-gradient(135deg, #1a8bc4, #146c99); border: none; border-radius: 8px; color: #fff; font-weight: 600; cursor: pointer; transition: all 0.2s; box-shadow: 0 4px 12px rgba(26,139,196,0.2); }
.btn-save:hover:not(:disabled) { transform: translateY(-1px); box-shadow: 0 6px 16px rgba(26,139,196,0.3); }
.btn-save:disabled { opacity: 0.7; cursor: not-allowed; }
</style>
