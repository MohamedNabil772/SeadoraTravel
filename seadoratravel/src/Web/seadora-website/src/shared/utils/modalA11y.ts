import { nextTick, ref, watch, type Ref } from 'vue'

const FOCUSABLE =
  'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])'

/**
 * Minimal dialog accessibility: focus moves into the dialog on open, Tab cycles
 * inside it, and focus returns to the trigger on close.
 * ponytail: hand-rolled trap; swap for focus-trap if nested dialogs ever appear.
 */
export function useModalA11y(isOpen: Ref<boolean>) {
  const dialogEl = ref<HTMLElement | null>(null)
  let lastFocused: HTMLElement | null = null

  const setDialogEl = (el: unknown) => {
    dialogEl.value = (el as HTMLElement | null) ?? null
  }

  const focusables = () =>
    Array.from(dialogEl.value?.querySelectorAll<HTMLElement>(FOCUSABLE) ?? []).filter(
      (el) => el.offsetParent !== null
    )

  const trapTab = (e: KeyboardEvent) => {
    const items = focusables()
    if (!items.length) return
    const first = items[0]
    const last = items[items.length - 1]
    const active = document.activeElement as HTMLElement | null

    if (e.shiftKey && (active === first || active === dialogEl.value)) {
      e.preventDefault()
      last.focus()
    } else if (!e.shiftKey && active === last) {
      e.preventDefault()
      first.focus()
    }
  }

  watch(isOpen, async (open) => {
    if (open) {
      lastFocused = document.activeElement as HTMLElement | null
      await nextTick()
      ;(focusables()[0] ?? dialogEl.value)?.focus()
    } else {
      lastFocused?.focus()
      lastFocused = null
    }
  })

  return { setDialogEl, trapTab }
}
