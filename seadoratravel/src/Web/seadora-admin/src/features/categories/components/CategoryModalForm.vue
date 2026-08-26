<script setup lang="ts">
import { ref, watch } from 'vue'
import LocaleTabs from '@/shared/components/locale/LocaleTabs.vue'
import LocalizedInput from '@/shared/components/locale/LocalizedInput.vue'
import LocalizedTextarea from '@/shared/components/locale/LocalizedTextarea.vue'
import LuxuryIconPicker from './LuxuryIconPicker.vue'
import api from '@/services/api'

const props = defineProps<{
  modelValue: boolean
  isEdit: boolean
  category: any
  actionLoading: boolean
}>()

const emit = defineEmits(['update:modelValue', 'save'])

const locales = [
  { code: 'en', label: 'EN', flag: '🇬🇧', status: 'complete' as const },
  { code: 'de', label: 'DE', flag: '🇩🇪', status: 'complete' as const },
  { code: 'it', label: 'IT', flag: '🇮🇹', status: 'complete' as const },
  { code: 'fr', label: 'FR', flag: '🇫🇷', status: 'complete' as const },
  { code: 'ru', label: 'RU', flag: '🇷🇺', status: 'complete' as const }
]

const activeLang = ref('en')

const form = ref({
  id: '',
  names: {} as Record<string, string>,
  descriptions: {} as Record<string, string>,
  icon: null as string | null,
  customIconUrl: null as string | null,
  coverImageUrl: null as string | null,
  order: 0
})

const errors = ref({
  names: false
})

const isUploadingCover = ref(false)
const isUploadingCustomIcon = ref(false)
const coverInput = ref<HTMLInputElement | null>(null)
const customIconInput = ref<HTMLInputElement | null>(null)

watch(() => props.modelValue, (val) => {
  if (val) {
    const initialNames: Record<string, string> = {}
    const initialDesc: Record<string, string> = {}
    locales.forEach(lang => {
      initialNames[lang.code] = ''
      initialDesc[lang.code] = ''
    })
    
    errors.value.names = false
    activeLang.value = 'en'

    if (props.isEdit && props.category) {
      form.value = {
        id: props.category.id,
        names: { ...initialNames, ...props.category.names },
        descriptions: { ...initialDesc, ...props.category.descriptions },
        icon: props.category.iconName || props.category.icon || null,
        customIconUrl: props.category.customIconUrl || null,
        coverImageUrl: props.category.coverImageUrl || null,
        order: props.category.order || 0
      }
    } else {
      form.value = {
        id: '',
        names: initialNames,
        descriptions: initialDesc,
        icon: null,
        customIconUrl: null,
        coverImageUrl: null,
        order: 0
      }
    }
  }
})

function close() {
  if (props.actionLoading) return
  emit('update:modelValue', false)
}

function save() {
  if (!form.value.names['en']) {
    errors.value.names = true
    activeLang.value = 'en'
    return
  }
  
  // Mapping 'icon' back to 'iconName' if needed by backend, though standardizing on 'icon'
  emit('save', { 
    ...form.value,
    iconName: form.value.icon,
    icon: form.value.icon 
  })
}

async function uploadFile(event: Event, targetField: 'coverImageUrl' | 'customIconUrl') {
  const target = event.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  const file = target.files[0]
  if (!file.type.startsWith('image/')) return

  const formData = new FormData()
  formData.append('file', file)

  if (targetField === 'coverImageUrl') isUploadingCover.value = true
  else isUploadingCustomIcon.value = true

  try {
    const res = await api.post('/api/files', formData)
    const fileId = res.data.fileId || res.data.FileId
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    form.value[targetField] = `${API_URL}/api/files/${fileId}`
  } catch (err) {
    console.error('Failed to upload', err)
  } finally {
    if (targetField === 'coverImageUrl') isUploadingCover.value = false
    else isUploadingCustomIcon.value = false
  }
  
  if (target) target.value = ''
}
</script>

<template>
  <Transition name="modal">
    <div v-if="modelValue" class="modal-overlay" @click.self="close">
      <div class="modal">
        <div class="modal-header">
          <h3>{{ isEdit ? 'Edit Category' : 'New Category' }}</h3>
          <button @click="close" class="btn-close" :disabled="actionLoading">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
          </button>
        </div>
        
        <div class="modal-content">
          <div class="form-grid">
            <div class="form-group">
              <label>Icon / Emoji</label>
              <LuxuryIconPicker v-model="form.icon" />
            </div>
            
            <div class="form-group">
              <label>Custom Icon URL</label>
              <div class="flex gap-2 mb-2">
                <input type="text" v-model="form.customIconUrl" class="text-input flex-1" placeholder="https://..." :disabled="isUploadingCustomIcon" />
                <button type="button" class="btn-upload" @click="customIconInput?.click()" :disabled="isUploadingCustomIcon">
                  <span v-if="isUploadingCustomIcon" class="loading-spinner-small"></span>
                  <span v-else>Browse</span>
                </button>
              </div>
              <input type="file" ref="customIconInput" accept="image/*" class="hidden" style="display:none" @change="e => uploadFile(e, 'customIconUrl')" />
            </div>
          </div>
          
          <div class="form-group">
            <label>Cover Image URL</label>
            <div class="flex gap-2 mb-2">
              <input type="text" v-model="form.coverImageUrl" class="text-input flex-1" placeholder="https://..." :disabled="isUploadingCover" />
              <button type="button" class="btn-upload" @click="coverInput?.click()" :disabled="isUploadingCover">
                <span v-if="isUploadingCover" class="loading-spinner-small"></span>
                <span v-else>Browse</span>
              </button>
            </div>
            <input type="file" ref="coverInput" accept="image/*" class="hidden" style="display:none" @change="e => uploadFile(e, 'coverImageUrl')" />
            <div v-if="form.coverImageUrl" class="image-preview relative group">
              <img :src="form.coverImageUrl" alt="Cover preview" />
              <div class="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                <button type="button" class="btn-remove-img" @click="form.coverImageUrl = null">Remove</button>
              </div>
            </div>
          </div>
          
          <div class="form-group">
            <label>Display Order</label>
            <input type="number" v-model.number="form.order" class="text-input" placeholder="0" style="max-width: 150px" />
          </div>

          <div class="locale-section">
            <label class="section-label">Localized Content</label>
            <div class="locale-wrapper">
              <LocaleTabs v-model="activeLang" :locales="locales" class="mb-4" />
              
              <div class="form-group mt-3">
                <LocalizedInput
                  v-model="form.names[activeLang]"
                  :locale="activeLang"
                  :label="`Name (${activeLang.toUpperCase()})`"
                  :placeholder="`Enter category name in ${activeLang.toUpperCase()}`"
                  :error="errors.names && activeLang === 'en' && !form.names['en'] ? 'English name is required' : ''"
                />
              </div>
              
              <div class="form-group mt-3">
                <LocalizedTextarea
                  v-model="form.descriptions[activeLang]"
                  :locale="activeLang"
                  :label="`Description (${activeLang.toUpperCase()})`"
                  :placeholder="`Enter category description in ${activeLang.toUpperCase()}`"
                  :rows="3"
                />
              </div>
            </div>
          </div>
        </div>
        
        <div class="modal-footer">
          <button @click="close" class="btn-cancel" :disabled="actionLoading">Cancel</button>
          <button @click="save" class="btn-save" :disabled="actionLoading">
            {{ actionLoading ? 'Saving...' : 'Save Category' }}
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
  -webkit-backdrop-filter: blur(4px);
  z-index: 2000;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
}

.modal {
  width: 100%;
  max-width: 560px;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.12);
  display: flex;
  flex-direction: column;
  max-height: 90vh;
  overflow: hidden;
}

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}
.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
.modal-enter-active .modal,
.modal-leave-active .modal {
  transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1), opacity 0.3s ease;
}
.modal-enter-from .modal {
  transform: scale(0.95) translateY(20px);
  opacity: 0;
}
.modal-leave-to .modal {
  transform: scale(0.95) translateY(20px);
  opacity: 0;
}

.modal-header {
  padding: 20px 24px;
  border-bottom: 1px solid #e2e8f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #ffffff;
}
.modal-header h3 {
  margin: 0;
  color: #0b1b3d;
  font-size: 1.125rem;
  font-weight: 600;
}
.btn-close {
  background: none;
  border: none;
  color: #64748b;
  cursor: pointer;
  transition: color 0.2s, transform 0.2s, background 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
}
.btn-close:hover { 
  color: #0f172a; 
  background: #f1f5f9;
  transform: scale(1.05);
}

.modal-content {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.modal-content::-webkit-scrollbar { width: 6px; }
.modal-content::-webkit-scrollbar-track { background: transparent; }
.modal-content::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 3px; }

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.form-group label, .section-label {
  color: #334155;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.text-input {
  width: 100%;
  padding: 10px 14px;
  background: #ffffff;
  border: 1.5px solid #cbd5e1;
  border-radius: 8px;
  color: #0f172a;
  font-size: 14px;
  transition: all 0.2s;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}
.text-input:focus {
  outline: none;
  border-color: #0b1b3d;
  box-shadow: 0 0 0 3px rgba(11, 27, 61, 0.1);
}

.image-preview {
  margin-top: 8px;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #e2e8f0;
  height: 120px;
  background: #f8fafc;
}
.image-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.locale-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.locale-wrapper {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
}

.mb-4 { margin-bottom: 16px; }
.mt-3 { margin-top: 12px; }

.modal-footer {
  padding: 20px 24px;
  border-top: 1px solid #e2e8f0;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  background: #f8fafc;
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
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 4px 12px rgba(11, 27, 61, 0.2);
}
.btn-save:hover:not(:disabled) { 
  transform: translateY(-1px); 
  box-shadow: 0 6px 16px rgba(11, 27, 61, 0.3); 
}
.btn-save:disabled { opacity: 0.7; cursor: not-allowed; }

.btn-upload {
  padding: 8px 16px;
  background: #f8fafc;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  color: #334155;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 80px;
}
.btn-upload:hover:not(:disabled) {
  background: #f1f5f9;
  border-color: #94a3b8;
  color: #0f172a;
}
.btn-upload:disabled {
  opacity: 0.5;
  cursor: wait;
}

.loading-spinner-small {
  width: 16px;
  height: 16px;
  border: 2px solid #cbd5e1;
  border-top-color: #334155;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}

.btn-remove-img {
  padding: 6px 12px;
  background: #ef4444;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transform: translateY(10px);
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}
.group:hover .btn-remove-img {
  transform: translateY(0);
}
.btn-remove-img:hover {
  background: #dc2626;
}

.flex { display: flex; }
.flex-1 { flex: 1; }
.gap-2 { gap: 8px; }
.mb-2 { margin-bottom: 8px; }
.relative { position: relative; }
.absolute { position: absolute; }
.inset-0 { top: 0; right: 0; bottom: 0; left: 0; }
.items-center { align-items: center; }
.justify-center { justify-content: center; }
.transition-opacity { transition: opacity 0.2s; }
.opacity-0 { opacity: 0; }
.group:hover .group-hover\:opacity-100 { opacity: 1; }
.bg-black\/50 { background-color: rgba(0, 0, 0, 0.4); }
</style>
