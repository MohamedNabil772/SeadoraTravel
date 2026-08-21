<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { onClickOutside } from '@vueuse/core'
import { Motion } from 'motion-v'
import axios from 'axios'

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

export interface CountryOption {
  code: string
  name: string
  nationality: string
  flag: string
}

// Helper to reliably compute flag emoji from ISO 2-letter country code
function getFlagEmoji(code: string, fallback?: string): string {
  if (fallback && fallback !== '????' && fallback.trim() && fallback !== '🏳️') {
    return fallback
  }
  if (!code || code.length !== 2) return '🏳️'
  try {
    const upper = code.toUpperCase()
    const codePoints = upper.split('').map(char => 127397 + char.charCodeAt(0))
    return String.fromCodePoint(...codePoints)
  } catch {
    return '🏳️'
  }
}

const defaultCountries: CountryOption[] = [
  { code: "EG", name: "Egypt", nationality: "Egyptian", flag: "🇪🇬" },
  { code: "US", name: "United States", nationality: "American", flag: "🇺🇸" },
  { code: "GB", name: "United Kingdom", nationality: "British", flag: "🇬🇧" },
  { code: "DE", name: "Germany", nationality: "German", flag: "🇩🇪" },
  { code: "FR", name: "France", nationality: "French", flag: "🇫🇷" },
  { code: "IT", name: "Italy", nationality: "Italian", flag: "🇮🇹" },
  { code: "RU", name: "Russia", nationality: "Russian", flag: "🇷🇺" },
  { code: "SA", name: "Saudi Arabia", nationality: "Saudi", flag: "🇸🇦" },
  { code: "AE", name: "United Arab Emirates", nationality: "Emirati", flag: "🇦🇪" },
  { code: "CH", name: "Switzerland", nationality: "Swiss", flag: "🇨🇭" },
  { code: "AT", name: "Austria", nationality: "Austrian", flag: "🇦🇹" },
  { code: "PL", name: "Poland", nationality: "Polish", flag: "🇵🇱" },
  { code: "NL", name: "Netherlands", nationality: "Dutch", flag: "🇳🇱" },
  { code: "BE", name: "Belgium", nationality: "Belgian", flag: "🇧🇪" },
  { code: "SE", name: "Sweden", nationality: "Swedish", flag: "🇸🇪" },
  { code: "NO", name: "Norway", nationality: "Norwegian", flag: "🇳🇴" },
  { code: "ES", name: "Spain", nationality: "Spanish", flag: "🇪🇸" },
  { code: "CA", name: "Canada", nationality: "Canadian", flag: "🇨🇦" },
  { code: "AU", name: "Australia", nationality: "Australian", flag: "🇦🇺" },
  { code: "CN", name: "China", nationality: "Chinese", flag: "🇨🇳" },
  { code: "JP", name: "Japan", nationality: "Japanese", flag: "🇯🇵" },
  { code: "KR", name: "South Korea", nationality: "South Korean", flag: "🇰🇷" },
  { code: "IN", name: "India", nationality: "Indian", flag: "🇮🇳" },
  { code: "BR", name: "Brazil", nationality: "Brazilian", flag: "🇧🇷" },
  { code: "MX", name: "Mexico", nationality: "Mexican", flag: "🇲🇽" },
  { code: "KW", name: "Kuwait", nationality: "Kuwaiti", flag: "🇰🇼" },
  { code: "QA", name: "Qatar", nationality: "Qatari", flag: "🇶🇦" },
  { code: "BH", name: "Bahrain", nationality: "Bahraini", flag: "🇧🇭" },
  { code: "OM", name: "Oman", nationality: "Omani", flag: "🇴🇲" },
  { code: "JO", name: "Jordan", nationality: "Jordanian", flag: "🇯🇴" },
  { code: "LB", name: "Lebanon", nationality: "Lebanese", flag: "🇱🇧" },
  { code: "TR", name: "Turkey", nationality: "Turkish", flag: "🇹🇷" }
]

const countries = ref<CountryOption[]>(defaultCountries)

const fetchNationalities = async () => {
  try {
    const res = await axios.get('/api/content/api/v1/nationalities')
    if (Array.isArray(res.data) && res.data.length > 0) {
      countries.value = res.data.map((n: any) => ({
        code: (n.code || n.flagCode || '').toUpperCase().trim(),
        name: n.countryName || n.name || '',
        nationality: n.nationalityName || n.countryName || n.name || '',
        flag: getFlagEmoji(n.code || n.flagCode, n.flagEmoji)
      }))
    }
  } catch (e) {
    console.warn('Failed to load dynamic nationalities, using defaults', e)
  }
}

onMounted(() => {
  fetchNationalities()
})

const filteredCountries = computed(() => {
  if (!searchQuery.value) return countries.value
  const q = searchQuery.value.toLowerCase().trim()
  return countries.value.filter(c => 
    c.name.toLowerCase().includes(q) || 
    c.nationality.toLowerCase().includes(q) ||
    c.code.toLowerCase().includes(q)
  )
})

const selectedCountry = computed(() => {
  if (!props.modelValue) return null
  const val = props.modelValue.toLowerCase().trim()
  return countries.value.find(c => 
    c.name.toLowerCase() === val || 
    c.nationality.toLowerCase() === val ||
    c.code.toLowerCase() === val
  )
})

const selectCountry = (item: CountryOption) => {
  emit('update:modelValue', item.nationality || item.name)
  isOpen.value = false
  searchQuery.value = ''
}
</script>

<template>
  <div class="relative" ref="target">
    <!-- Trigger Button -->
    <div 
      class="flex items-center justify-between px-3.5 py-2.5 bg-neutral-50 border border-neutral-200 rounded-xl cursor-pointer hover:bg-neutral-100/80 hover:border-neutral-300 transition-all duration-200 focus-within:ring-2 focus-within:ring-neutral-900/10 focus-within:border-neutral-400 select-none"
      @click="isOpen = !isOpen"
    >
      <div class="flex items-center gap-2.5 min-w-0">
        <span v-if="selectedCountry" class="text-xl leading-none shrink-0 drop-shadow-sm">{{ selectedCountry.flag }}</span>
        <div v-if="selectedCountry" class="flex items-center gap-1.5 min-w-0">
          <span class="text-neutral-900 font-semibold text-xs sm:text-sm truncate">{{ selectedCountry.nationality }}</span>
          <span class="text-neutral-400 text-xs font-normal shrink-0">({{ selectedCountry.name }})</span>
        </div>
        <span v-else class="text-neutral-400 text-xs sm:text-sm font-normal truncate">
          {{ modelValue || 'Select nationality (e.g. Egypt, German, British)' }}
        </span>
      </div>

      <div class="flex items-center gap-2 ml-2 shrink-0">
        <span v-if="selectedCountry" class="font-mono text-[10px] font-bold px-1.5 py-0.5 rounded bg-neutral-200/70 text-neutral-600 uppercase tracking-wider">
          {{ selectedCountry.code }}
        </span>
        <svg 
          class="w-4 h-4 text-neutral-400 transition-transform duration-200"
          :class="{ 'rotate-180 text-neutral-700': isOpen }"
          fill="none" viewBox="0 0 24 24" stroke="currentColor"
        >
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
        </svg>
      </div>
    </div>

    <!-- Dropdown Popover -->
    <Motion
      v-if="isOpen"
      :initial="{ opacity: 0, y: -6, scale: 0.98 }"
      :animate="{ opacity: 1, y: 0, scale: 1 }"
      :exit="{ opacity: 0, y: -6, scale: 0.98 }"
      :transition="{ type: 'spring', stiffness: 350, damping: 28 }"
      class="absolute z-50 w-full mt-1.5 bg-white border border-neutral-200/90 rounded-2xl shadow-2xl overflow-hidden backdrop-blur-lg"
    >
      <!-- Search Box -->
      <div class="p-2.5 border-b border-neutral-100 bg-neutral-50/70">
        <div class="relative">
          <svg class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-neutral-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input 
            v-model="searchQuery"
            type="text"
            placeholder="Search country or nationality (e.g. Egypt, German, British)..."
            class="w-full pl-9 pr-3 py-2 text-xs sm:text-sm bg-white border border-neutral-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-amber-500/20 focus:border-amber-500 transition-all placeholder:text-neutral-400 shadow-inner"
            @click.stop
            autofocus
          />
        </div>
      </div>

      <!-- Countries List with clean spacing -->
      <div class="max-h-64 overflow-y-auto p-1.5 scrollbar-thin scrollbar-thumb-neutral-200 hover:scrollbar-thumb-neutral-300 divide-y divide-neutral-50">
        <div 
          v-for="country in filteredCountries" 
          :key="country.code + country.name"
          class="flex items-center px-3 py-2.5 rounded-xl cursor-pointer transition-all duration-150 group select-none my-0.5"
          :class="(selectedCountry && selectedCountry.code === country.code) 
            ? 'bg-amber-500/10 text-neutral-900 font-medium' 
            : 'hover:bg-neutral-100/80 text-neutral-700'"
          @click="selectCountry(country)"
        >
          <!-- Flag Emoji with Spacing -->
          <span class="mr-3 text-xl leading-none shrink-0 drop-shadow-sm select-none">{{ country.flag }}</span>

          <!-- Country & Nationality with Clear Hierarchy and Spacing -->
          <div class="flex flex-col flex-1 min-w-0 pr-2">
            <div class="flex items-baseline gap-2 flex-wrap">
              <span class="font-semibold text-neutral-900 text-xs sm:text-sm group-hover:text-amber-700 transition-colors">
                {{ country.name }}
              </span>
              <span class="text-[11px] text-neutral-500 font-normal">
                {{ country.nationality }}
              </span>
            </div>
          </div>

          <!-- ISO Country Code Badge with clear space -->
          <span 
            class="ml-auto font-mono text-[10px] font-bold px-2 py-0.5 rounded-md uppercase tracking-wider shrink-0 transition-colors"
            :class="(selectedCountry && selectedCountry.code === country.code)
              ? 'bg-amber-500 text-white shadow-sm'
              : 'bg-neutral-100 text-neutral-500 border border-neutral-200/80 group-hover:bg-neutral-200 group-hover:text-neutral-700'"
          >
            {{ country.code }}
          </span>
        </div>

        <!-- Empty State -->
        <div v-if="filteredCountries.length === 0" class="px-4 py-8 text-center">
          <p class="text-xs sm:text-sm text-neutral-500 font-medium">No nationalities found matching</p>
          <p class="text-xs text-neutral-400 font-mono mt-1">"{{ searchQuery }}"</p>
        </div>
      </div>
    </Motion>
  </div>
</template>
