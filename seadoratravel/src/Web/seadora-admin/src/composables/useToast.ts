import { ref, readonly } from 'vue';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface Toast {
  id: string;
  type: ToastType;
  title: string;
  message?: string;
  duration?: number;
}

const toasts = ref<Toast[]>([]);

export function useToast() {
  const addToast = (type: ToastType, title: string, message?: string, duration: number = 3000) => {
    const id = Math.random().toString(36).substring(2, 9);
    const toast: Toast = { id, type, title, message, duration };
    toasts.value.push(toast);

    if (duration > 0) {
      setTimeout(() => {
        removeToast(id);
      }, duration);
    }
  };

  const removeToast = (id: string) => {
    const index = toasts.value.findIndex(t => t.id === id);
    if (index > -1) {
      toasts.value.splice(index, 1);
    }
  };

  const success = (title: string, message?: string) => addToast('success', title, message);
  const error = (title: string, message?: string) => addToast('error', title, message);
  const warning = (title: string, message?: string) => addToast('warning', title, message);
  const info = (title: string, message?: string) => addToast('info', title, message);

  return {
    toasts: readonly(toasts),
    success,
    error,
    warning,
    info,
    removeToast,
  };
}
