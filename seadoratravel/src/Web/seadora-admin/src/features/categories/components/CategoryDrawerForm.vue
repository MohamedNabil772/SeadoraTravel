<script setup lang="ts">
import { ref, watch } from 'vue'
import LocaleTabs from '@/shared/components/locale/LocaleTabs.vue'
import LocalizedInput from '@/shared/components/locale/LocalizedInput.vue'
import LuxuryIconPicker from './LuxuryIconPicker.vue'

const props = defineProps<{
  modelValue: boolean
  isEdit: boolean
  category: any
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
  icon: '⛵'
})

watch(() => props.modelValue, (val) => {
  if (val) {
    if (props.isEdit && props.category) {
      form.value = {
        id: props.category.id,
        names: { ...props.category.names },
        icon: props.category.icon || '⛵'
      }
    } else {
      form.value = {
        id: '',
        names: { en: '', de: '', it: '', fr: '', ru: '' },
        icon: '⛵'
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
</script>

<template>
  <div v-if="modelValue" class="drawer-overlay" @click.self="close">
    <div class="drawer">
      <div class="drawer-header">
        <h3>{{ isEdit ? 'Edit Category' : 'New Category' }}</h3>
        <button @click="close" class="btn-close" :disabled="actionLoading">✕</button>
      </div>
      <div class="drawer-content">
        <div class="form-group">
          <label>Category Icon</label>
          <LuxuryIconPicker v-model="form.icon" />
        </div>
        
        <div class="form-group">
          <label>Localized Names</label>
          <div class="locale-wrapper">
            <LocaleTabs v-model="activeLang" :locales="locales" class="mb-4" />
            <LocalizedInput
              v-model="form.names[activeLang]"
              :locale="activeLang"
              :placeholder="`Enter category name in ${activeLang.toUpperCase()}`"
            />
          </div>
        </div>
      </div>
      <div class="drawer-footer">
        <button @click="close" class="btn-cancel" :disabled="actionLoading">Cancel</button>
        <button @click="save" class="btn-save" :disabled="actionLoading">
          {{ actionLoading ? 'Saving...' : 'Save Category' }}
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
  max-width: 450px;
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
.drawer-header h3 {
  margin: 0;
  color: #fff;
  font-size: 1.125rem;
  font-weight: 600;
}
.btn-close {
  background: none;
  border: none;
  color: #8eafc2;
  cursor: pointer;
  font-size: 1.25rem;
  transition: color 0.2s;
}
.btn-close:hover { color: #fff; }
.drawer-content {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 28px;
}
.drawer-footer {
  padding: 20px 24px;
  border-top: 1px solid rgba(255,255,255,0.06);
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  background: rgba(0,0,0,0.2);
}
.form-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.form-group label {
  color: #8eafc2;
  font-size: 0.875rem;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.locale-wrapper {
  background: rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(255,255,255,0.05);
  border-radius: 12px;
  padding: 16px;
}
.mb-4 { margin-bottom: 16px; display: block; }
.btn-cancel {
  padding: 10px 20px;
  background: transparent;
  border: 1px solid rgba(255,255,255,0.1);
  border-radius: 8px;
  color: #8eafc2;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
}
.btn-cancel:hover:not(:disabled) { background: rgba(255,255,255,0.05); color: #fff; }
.btn-save {
  padding: 10px 24px;
  background: linear-gradient(135deg, #1a8bc4, #146c99);
  border: none;
  border-radius: 8px;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 4px 12px rgba(26,139,196,0.2);
}
.btn-save:hover:not(:disabled) { transform: translateY(-1px); box-shadow: 0 6px 16px rgba(26,139,196,0.3); }
.btn-save:disabled { opacity: 0.7; cursor: not-allowed; }
</style>
