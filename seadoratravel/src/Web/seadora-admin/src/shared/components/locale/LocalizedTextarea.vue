<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  modelValue: string;
  locale: string;
  label?: string;
  placeholder?: string;
  error?: string;
  rows?: number;
  dictionary?: Record<string, string>;
}>();

const emit = defineEmits(['update:modelValue']);

const isRTL = computed(() => props.locale === 'ar');

const dynamicPlaceholder = computed(() => {
  if (props.placeholder) return props.placeholder;
  if (props.dictionary && props.dictionary[props.locale]) {
    return props.dictionary[props.locale];
  }
  return isRTL.value ? 'أدخل النص هنا...' : 'Enter text here...';
});
</script>

<template>
  <div class="localized-textarea-wrapper" :dir="isRTL ? 'rtl' : 'ltr'">
    <label v-if="label" class="textarea-label">
      {{ label }}
      <span class="locale-badge" :class="isRTL ? 'ar' : 'en'">{{ locale.toUpperCase() }}</span>
    </label>
    
    <div class="textarea-container">
      <textarea
        :value="modelValue"
        @input="emit('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
        :placeholder="dynamicPlaceholder"
        :rows="rows || 4"
        class="localized-textarea"
        :class="{ 'has-error': error, 'is-rtl': isRTL }"
        :aria-label="label || undefined"
      ></textarea>
    </div>
    
    <span v-if="error" class="error-text">{{ error }}</span>
  </div>
</template>

<style scoped>
.localized-textarea-wrapper {
  display: flex;
  flex-direction: column;
  gap: 6px;
  width: 100%;
}

.textarea-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 500;
  color: #374151;
}

.locale-badge {
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 4px;
  font-weight: 600;
  letter-spacing: 0.5px;
}

.locale-badge.en {
  background-color: #e0e7ff;
  color: #4338ca;
}

.locale-badge.ar {
  background-color: #dcfce7;
  color: #15803d;
}

.textarea-container {
  position: relative;
  width: 100%;
}

.localized-textarea {
  width: 100%;
  padding: 10px 14px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 14px;
  color: #1f2937;
  background-color: #ffffff;
  transition: all 0.2s ease;
  box-shadow: 0 1px 2px rgba(0,0,0,0.05);
  font-family: inherit;
  box-sizing: border-box;
  resize: vertical;
}

.localized-textarea.is-rtl {
  font-family: 'Tajawal', 'Cairo', sans-serif;
}

.localized-textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15);
}

.localized-textarea.has-error {
  border-color: #ef4444;
}

.localized-textarea.has-error:focus {
  box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.15);
}

.error-text {
  font-size: 12px;
  color: #ef4444;
  margin-top: 2px;
}
</style>
