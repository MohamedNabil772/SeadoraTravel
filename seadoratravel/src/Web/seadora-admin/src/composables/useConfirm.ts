import { ref, readonly } from 'vue';

type ConfirmType = 'danger' | 'warning' | 'info';

interface ConfirmOptions {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: ConfirmType;
}

interface ConfirmState extends ConfirmOptions {
  isOpen: boolean;
  resolve: ((value: boolean) => void) | null;
}

const defaultState: ConfirmState = {
  isOpen: false,
  title: '',
  message: '',
  confirmText: 'Confirm',
  cancelText: 'Cancel',
  type: 'info',
  resolve: null,
};

const state = ref<ConfirmState>({ ...defaultState });

export function useConfirm() {
  const confirm = (options: ConfirmOptions): Promise<boolean> => {
    return new Promise((resolve) => {
      state.value = {
        ...defaultState,
        ...options,
        isOpen: true,
        resolve,
      };
    });
  };

  const close = (result: boolean) => {
    if (state.value.resolve) {
      state.value.resolve(result);
    }
    state.value.isOpen = false;
    setTimeout(() => {
      state.value = { ...defaultState };
    }, 200);
  };

  return {
    state: readonly(state),
    confirm,
    close,
  };
}
