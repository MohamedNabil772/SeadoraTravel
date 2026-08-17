<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue';

const props = defineProps<{
  modelValue: boolean;
  pdfUrl: string | null;
  title?: string;
  filename?: string;
}>();

const emit = defineEmits(['update:modelValue']);

const isVisible = ref(props.modelValue);
const isClosing = ref(false);

watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    isVisible.value = true;
    document.body.style.overflow = 'hidden';
  } else {
    closeModal();
  }
});

const closeModal = () => {
  isClosing.value = true;
  document.body.style.overflow = '';
  setTimeout(() => {
    isVisible.value = false;
    isClosing.value = false;
    emit('update:modelValue', false);
  }, 300); // matching the transition duration
};

const handleBackdropClick = (e: MouseEvent) => {
  if ((e.target as HTMLElement).classList.contains('modal-backdrop')) {
    closeModal();
  }
};

const handleEscape = (e: KeyboardEvent) => {
  if (e.key === 'Escape' && isVisible.value) {
    closeModal();
  }
};

const printPdf = () => {
  if (!props.pdfUrl) return;
  const iframe = document.createElement('iframe');
  iframe.style.display = 'none';
  iframe.src = props.pdfUrl;
  document.body.appendChild(iframe);
  iframe.onload = () => {
    iframe.contentWindow?.print();
  };
};

const downloadPdf = () => {
  if (!props.pdfUrl) return;
  const link = document.createElement('a');
  link.href = props.pdfUrl;
  link.download = props.filename || 'document.pdf';
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

onMounted(() => {
  document.addEventListener('keydown', handleEscape);
});

onUnmounted(() => {
  document.removeEventListener('keydown', handleEscape);
  document.body.style.overflow = '';
});
</script>

<template>
  <Teleport to="body">
    <div 
      v-if="isVisible" 
      class="modal-backdrop"
      :class="{ 'is-closing': isClosing }"
      @click="handleBackdropClick"
      role="dialog"
      aria-modal="true"
    >
      <div 
        class="modal-container"
        :class="{ 'is-closing': isClosing }"
      >
        <div class="modal-header">
          <h3 class="modal-title">{{ title || 'PDF Preview' }}</h3>
          <div class="modal-actions">
            <button @click="downloadPdf" class="action-btn" title="Download" :disabled="!pdfUrl">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>
            </button>
            <button @click="printPdf" class="action-btn" title="Print" :disabled="!pdfUrl">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="6 9 6 2 18 2 18 9"></polyline><path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"></path><rect x="6" y="14" width="12" height="8"></rect></svg>
            </button>
            <button @click="closeModal" class="action-btn close-btn" title="Close">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
            </button>
          </div>
        </div>
        
        <div class="modal-body">
          <div v-if="!pdfUrl" class="empty-state">
            <div class="spinner"></div>
            <p>Loading document...</p>
          </div>
          <iframe 
            v-else 
            :src="pdfUrl" 
            class="pdf-viewer" 
            title="PDF Document Viewer"
          ></iframe>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background-color: rgba(17, 24, 39, 0.4);
  backdrop-filter: blur(4px);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
  animation: fadeIn 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.modal-backdrop.is-closing {
  animation: fadeOut 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.modal-container {
  background-color: #ffffff;
  border-radius: 12px;
  width: 100%;
  max-width: 900px;
  height: 85vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  transform-origin: center;
  animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards;
  overflow: hidden;
}

.modal-container.is-closing {
  animation: slideDown 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 24px;
  border-bottom: 1px solid #e5e7eb;
  background-color: #f9fafb;
}

.modal-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #111827;
}

.modal-actions {
  display: flex;
  gap: 8px;
}

.action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 8px;
  border: none;
  background-color: transparent;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.action-btn:hover:not(:disabled) {
  background-color: #e5e7eb;
  color: #111827;
}

.action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.close-btn:hover {
  background-color: #fee2e2;
  color: #ef4444;
}

.modal-body {
  flex: 1;
  background-color: #f3f4f6;
  position: relative;
}

.pdf-viewer {
  width: 100%;
  height: 100%;
  border: none;
}

.empty-state {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #6b7280;
  gap: 16px;
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes fadeOut {
  from { opacity: 1; }
  to { opacity: 0; }
}

@keyframes slideUp {
  from { 
    opacity: 0;
    transform: translateY(20px) scale(0.98); 
  }
  to { 
    opacity: 1;
    transform: translateY(0) scale(1); 
  }
}

@keyframes slideDown {
  from { 
    opacity: 1;
    transform: translateY(0) scale(1); 
  }
  to { 
    opacity: 0;
    transform: translateY(20px) scale(0.98); 
  }
}
</style>
