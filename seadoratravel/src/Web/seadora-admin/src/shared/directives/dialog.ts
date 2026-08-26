import type { Directive } from 'vue'

const FOCUSABLE = [
  'a[href]',
  'button:not([disabled])',
  'input:not([disabled]):not([type="hidden"])',
  'select:not([disabled])',
  'textarea:not([disabled])',
  '[tabindex]:not([tabindex="-1"])',
].join(',')

type CloseFn = () => void
type DialogBinding = CloseFn | { open: boolean; close: CloseFn }

interface DialogState {
  onKeydown: (e: KeyboardEvent) => void
  previous: HTMLElement | null
  active: boolean
}

const states = new WeakMap<HTMLElement, DialogState>()

const resolve = (value: DialogBinding | undefined): { open: boolean; close?: CloseFn } =>
  typeof value === 'function' ? { open: true, close: value } : value ?? { open: true }

const isVisible = (n: HTMLElement) =>
  n.offsetWidth > 0 || n.offsetHeight > 0 || n.getClientRects().length > 0

const focusables = (el: HTMLElement) =>
  Array.from(el.querySelectorAll<HTMLElement>(FOCUSABLE)).filter(isVisible)

function activate(el: HTMLElement, state: DialogState) {
  state.active = true
  state.previous = document.activeElement as HTMLElement | null
  if (!el.hasAttribute('tabindex')) el.setAttribute('tabindex', '-1')
  requestAnimationFrame(() => {
    if (!state.active || !el.isConnected || el.contains(document.activeElement)) return
    const target = el.querySelector<HTMLElement>('[data-autofocus]') ?? focusables(el)[0] ?? el
    target.focus()
  })
}

function deactivate(state: DialogState) {
  state.active = false
  if (state.previous?.isConnected) state.previous.focus()
  state.previous = null
}

/**
 * ponytail: one directive instead of a shared base-dialog refactor. Put
 * `v-dialog="close"` on a modal root rendered with `v-if` (mount == open), or
 * `v-dialog="{ open, close }"` on a drawer that stays mounted. Gives Escape-to-close,
 * a Tab focus trap, and focus restore to the trigger on close.
 * Upgrade path: swap for a <BaseDialog> wrapper if modals ever need shared markup.
 */
export const vDialog: Directive<HTMLElement, DialogBinding | undefined> = {
  mounted(el, binding) {
    const state: DialogState = { onKeydown: () => {}, previous: null, active: false }

    state.onKeydown = (e: KeyboardEvent) => {
      if (!state.active) return
      if (e.key === 'Escape') {
        e.stopPropagation()
        resolve(binding.value).close?.()
        return
      }
      if (e.key !== 'Tab') return
      const items = focusables(el)
      if (!items.length) {
        e.preventDefault()
        el.focus()
        return
      }
      const first = items[0]
      const last = items[items.length - 1]
      const active = document.activeElement
      if (e.shiftKey && (active === first || !el.contains(active))) {
        e.preventDefault()
        last.focus()
      } else if (!e.shiftKey && (active === last || !el.contains(active))) {
        e.preventDefault()
        first.focus()
      }
    }

    el.addEventListener('keydown', state.onKeydown)
    states.set(el, state)

    if (resolve(binding.value).open) activate(el, state)
  },
  updated(el, binding) {
    const state = states.get(el)
    if (!state) return
    const { open } = resolve(binding.value)
    if (open && !state.active) activate(el, state)
    else if (!open && state.active) deactivate(state)
  },
  unmounted(el) {
    const state = states.get(el)
    if (!state) return
    el.removeEventListener('keydown', state.onKeydown)
    states.delete(el)
    if (state.active) deactivate(state)
  },
}
