<template>
  <Teleport to="body">
    <Transition name="confirm-modal">
      <div v-if="state.isOpen" class="confirm-backdrop" @mousedown.self="cancel">
        <div class="confirm-dialog" role="dialog" aria-modal="true" :aria-labelledby="titleId" v-dialog="cancel">
          <div class="confirm-icon" :class="state.type">
            <svg v-if="state.type === 'danger'" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18"></path><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"></path><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
            <svg v-else-if="state.type === 'warning'" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>
            <svg v-else xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>
          </div>
          
          <div class="confirm-content">
            <h3 :id="titleId" class="confirm-title">{{ state.title }}</h3>
            <p class="confirm-message">{{ state.message }}</p>
          </div>

          <div class="confirm-actions">
            <button class="confirm-btn cancel-btn" @click="cancel" ref="cancelBtn">
              {{ state.cancelText }}
            </button>
            <button class="confirm-btn confirm-action-btn" :class="state.type" @click="confirm" ref="confirmBtn">
              {{ state.confirmText }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, watch, nextTick, onMounted, onUnmounted } from 'vue';
import { useConfirm } from '@/composables/useConfirm';

const { state, close } = useConfirm();
const titleId = 'confirm-title-' + Math.random().toString(36).substr(2, 9);

const confirmBtn = ref<HTMLButtonElement | null>(null);

const confirm = () => close(true);
const cancel = () => close(false);

const handleKeydown = (e: KeyboardEvent) => {
  if (!state.value.isOpen) return;
  if (e.key === 'Escape') {
    cancel();
  } else if (e.key === 'Enter') {
    confirm();
  }
};

watch(() => state.value.isOpen, async (isOpen) => {
  if (isOpen) {
    await nextTick();
    confirmBtn.value?.focus();
  }
});

onMounted(() => {
  window.addEventListener('keydown', handleKeydown);
});

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown);
});
</script>

<style scoped>
.confirm-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(4, 11, 20, 0.75);
  backdrop-filter: blur(12px);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
}

.confirm-dialog {
  background: white;
  border-radius: 16px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

/* Animations */
.confirm-modal-enter-active,
.confirm-modal-leave-active {
  transition: opacity 200ms ease-out;
}

.confirm-modal-enter-active .confirm-dialog,
.confirm-modal-leave-active .confirm-dialog {
  transition: transform 200ms cubic-bezier(0.16, 1, 0.3, 1), opacity 200ms ease-out;
}

.confirm-modal-enter-from,
.confirm-modal-leave-to {
  opacity: 0;
}

.confirm-modal-enter-from .confirm-dialog,
.confirm-modal-leave-to .confirm-dialog {
  opacity: 0;
  transform: scale(0.95) translateY(10px);
}

.confirm-icon {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
}
.confirm-icon svg {
  width: 24px;
  height: 24px;
}
.confirm-icon.danger {
  background-color: #fee2e2;
  color: #ef4444;
  box-shadow: 0 0 0 4px #fef2f2;
}
.confirm-icon.warning {
  background-color: #fef3c7;
  color: #f59e0b;
  box-shadow: 0 0 0 4px #fffbeb;
}
.confirm-icon.info {
  background-color: #e0e7ff;
  color: #6366f1;
  box-shadow: 0 0 0 4px #eef2ff;
}

.confirm-content {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.confirm-title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.confirm-message {
  margin: 0;
  font-size: 0.875rem;
  color: #6b7280;
  line-height: 1.5;
}

.confirm-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 16px;
}

.confirm-btn {
  padding: 8px 16px;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  border: none;
  transition: transform 150ms ease, background-color 150ms ease;
  outline: none;
}
.confirm-btn:active {
  transform: scale(0.97);
}
.confirm-btn:focus-visible {
  box-shadow: 0 0 0 2px #fff, 0 0 0 4px #6366f1;
}

.cancel-btn {
  background-color: #f3f4f6;
  color: #374151;
}
.cancel-btn:hover {
  background-color: #e5e7eb;
}

.confirm-action-btn.danger {
  background-color: #ef4444;
  color: white;
}
.confirm-action-btn.danger:hover {
  background-color: #dc2626;
}

.confirm-action-btn.warning {
  background-color: #f59e0b;
  color: white;
}
.confirm-action-btn.warning:hover {
  background-color: #d97706;
}

.confirm-action-btn.info {
  background-color: #6366f1;
  color: white;
}
.confirm-action-btn.info:hover {
  background-color: #4f46e5;
}
</style>
