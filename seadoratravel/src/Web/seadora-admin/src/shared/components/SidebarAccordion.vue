<script setup lang="ts">
import { ref } from 'vue'
import { ChevronDown } from 'lucide-vue-next'

const props = defineProps<{
  title: string
  defaultOpen?: boolean
}>()

const isOpen = ref(props.defaultOpen ?? true)

const toggle = () => {
  isOpen.value = !isOpen.value
}
</script>

<template>
  <div class="mb-4">
    <button 
      @click="toggle"
      class="w-full flex items-center justify-between px-4 mb-2 group focus:outline-none"
    >
      <span class="text-[10px] font-bold text-white/40 uppercase tracking-widest group-hover:text-white/60 transition-colors">{{ title }}</span>
      <ChevronDown 
        class="w-3.5 h-3.5 text-white/40 transition-transform duration-300 cubic-bezier(0.34, 1.56, 0.64, 1)"
        :class="{ 'rotate-180': isOpen }"
      />
    </button>
    
    <div 
      class="overflow-hidden transition-all duration-400 ease-[cubic-bezier(0.34,1.56,0.64,1)] origin-top"
      :class="isOpen ? 'max-h-96 opacity-100 scale-y-100' : 'max-h-0 opacity-0 scale-y-95'"
    >
      <div class="space-y-1 mt-1">
        <slot></slot>
      </div>
    </div>
  </div>
</template>
