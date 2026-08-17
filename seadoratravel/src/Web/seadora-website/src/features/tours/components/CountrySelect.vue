<script setup lang="ts">
import { ref, computed } from 'vue'
import { onClickOutside } from '@vueuse/core'
import { Motion } from 'motion-v'

const props = defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const isOpen = ref(false)
const searchQuery = ref('')
const target = ref(null)

onClickOutside(target, () => {
  isOpen.value = false
})

const countries = [
  { code: 'EG', name: 'Egypt', flag: '🇪🇬' },
  { code: 'US', name: 'United States', flag: '🇺🇸' },
  { code: 'GB', name: 'United Kingdom', flag: '🇬🇧' },
  { code: 'FR', name: 'France', flag: '🇫🇷' },
  { code: 'DE', name: 'Germany', flag: '🇩🇪' },
  { code: 'IT', name: 'Italy', flag: '🇮🇹' },
  { code: 'RU', name: 'Russia', flag: '🇷🇺' },
  { code: 'CH', name: 'Switzerland', flag: '🇨🇭' },
  { code: 'AT', name: 'Austria', flag: '🇦🇹' },
  { code: 'PL', name: 'Poland', flag: '🇵🇱' },
  { code: 'NL', name: 'Netherlands', flag: '🇳🇱' },
  { code: 'BE', name: 'Belgium', flag: '🇧🇪' },
  { code: 'SE', name: 'Sweden', flag: '🇸🇪' },
  { code: 'NO', name: 'Norway', flag: '🇳🇴' },
  { code: 'SA', name: 'Saudi Arabia', flag: '🇸🇦' },
  { code: 'AE', name: 'UAE', flag: '🇦🇪' }
]

const filteredCountries = computed(() => {
  return countries.filter(c => c.name.toLowerCase().includes(searchQuery.value.toLowerCase()))
})

const selectedCountry = computed(() => {
  return countries.find(c => c.name === props.modelValue)
})

const selectCountry = (name: string) => {
  emit('update:modelValue', name)
  isOpen.value = false
  searchQuery.value = ''
}
</script>

<template>
  <div class="relative" ref="target">
    <div 
      class="flex items-center px-4 py-2.5 bg-neutral-50 border border-neutral-200 rounded-xl cursor-pointer hover:bg-neutral-100 transition-colors focus-within:ring-2 focus-within:ring-neutral-900/10 focus-within:border-neutral-400"
      @click="isOpen = !isOpen"
    >
      <span v-if="selectedCountry" class="mr-2 text-lg leading-none">{{ selectedCountry.flag }}</span>
      <span :class="selectedCountry ? 'text-neutral-900' : 'text-neutral-400'">
        {{ selectedCountry ? selectedCountry.name : 'e.g. United States' }}
      </span>
      <svg 
        class="w-4 h-4 ml-auto text-neutral-400 transition-transform duration-200"
        :class="{ 'rotate-180': isOpen }"
        fill="none" viewBox="0 0 24 24" stroke="currentColor"
      >
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
      </svg>
    </div>

    <Motion
      v-if="isOpen"
      initial="{ opacity: 0, y: -10, scale: 0.95 }"
      animate="{ opacity: 1, y: 0, scale: 1 }"
      exit="{ opacity: 0, y: -10, scale: 0.95 }"
      :transition="{ type: 'spring', stiffness: 300, damping: 30 }"
      class="absolute z-50 w-full mt-2 bg-white border border-neutral-200 rounded-xl shadow-xl overflow-hidden"
    >
      <div class="p-2 border-b border-neutral-100">
        <input 
          v-model="searchQuery"
          type="text"
          placeholder="Search country..."
          class="w-full px-3 py-2 text-sm bg-neutral-50 border border-neutral-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-neutral-900/10 focus:border-neutral-400 transition-all placeholder:text-neutral-400"
          @click.stop
        />
      </div>
      <div class="max-h-60 overflow-y-auto p-1 scrollbar-thin scrollbar-thumb-neutral-200 hover:scrollbar-thumb-neutral-300">
        <div 
          v-for="country in filteredCountries" 
          :key="country.code"
          class="flex items-center px-3 py-2 text-sm rounded-lg cursor-pointer hover:bg-neutral-50 transition-colors"
          :class="modelValue === country.name ? 'bg-neutral-100 font-medium text-neutral-900' : 'text-neutral-700'"
          @click="selectCountry(country.name)"
        >
          <span class="mr-2 text-lg leading-none">{{ country.flag }}</span>
          {{ country.name }}
          <svg v-if="modelValue === country.name" class="w-4 h-4 ml-auto text-neutral-900" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <div v-if="filteredCountries.length === 0" class="px-3 py-4 text-sm text-center text-neutral-500">
          No countries found
        </div>
      </div>
    </Motion>
  </div>
</template>
