<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useCurrencyStore } from '@/store/currency'

interface TourDateAvailability {
  date: string
  status: 'Available' | 'LowStock' | 'SoldOut'
  priceEur?: number
  spotsLeft?: number
}

const props = withDefaults(defineProps<{
  modelValue?: string
  basePriceEur?: number
  availableDates?: TourDateAvailability[]
}>(), {
  modelValue: '',
  basePriceEur: 45,
  availableDates: () => []
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}>()

const { t, locale } = useI18n()
const currencyStore = useCurrencyStore()

const weekdays = computed(() => [
  t('calendar.weekdays.mo'),
  t('calendar.weekdays.tu'),
  t('calendar.weekdays.we'),
  t('calendar.weekdays.th'),
  t('calendar.weekdays.fr'),
  t('calendar.weekdays.sa'),
  t('calendar.weekdays.su')
])

// Initialize calendar view to either the selected date or today
const getInitialDate = () => {
  if (props.modelValue) {
    const parts = props.modelValue.split('-')
    if (parts.length === 3) {
      return new Date(Number(parts[0]), Number(parts[1]) - 1, 1)
    }
  }
  const now = new Date()
  return new Date(now.getFullYear(), now.getMonth(), 1)
}

const calendarViewDate = ref(getInitialDate())
const hoveredDateStr = ref<string | null>(null)

// If parent changes modelValue externally, keep calendar view in sync
watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    const parts = newVal.split('-')
    if (parts.length === 3) {
      const targetMonth = new Date(Number(parts[0]), Number(parts[1]) - 1, 1)
      if (
        calendarViewDate.value.getFullYear() !== targetMonth.getFullYear() ||
        calendarViewDate.value.getMonth() !== targetMonth.getMonth()
      ) {
        calendarViewDate.value = targetMonth
      }
    }
  }
})

const currentMonthYearLabel = computed(() => {
  const loc = locale.value === 'ar' ? 'ar-EG' : locale.value === 'de' ? 'de-DE' : locale.value === 'fr' ? 'fr-FR' : locale.value === 'it' ? 'it-IT' : locale.value === 'ru' ? 'ru-RU' : 'en-US'
  return calendarViewDate.value.toLocaleDateString(loc, {
    month: 'long',
    year: 'numeric'
  })
})

const isPrevMonthDisabled = computed(() => {
  const now = new Date()
  const currentMonthStart = new Date(now.getFullYear(), now.getMonth(), 1)
  return calendarViewDate.value <= currentMonthStart
})

const prevMonth = () => {
  if (isPrevMonthDisabled.value) return
  calendarViewDate.value = new Date(calendarViewDate.value.getFullYear(), calendarViewDate.value.getMonth() - 1, 1)
}

const nextMonth = () => {
  calendarViewDate.value = new Date(calendarViewDate.value.getFullYear(), calendarViewDate.value.getMonth() + 1, 1)
}

const formatDateISO = (d: Date) => {
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

// Generate calendar grid with authentic date logic and availability
const calendarDays = computed(() => {
  const year = calendarViewDate.value.getFullYear()
  const month = calendarViewDate.value.getMonth()
  
  const firstDayOfMonth = new Date(year, month, 1)
  const lastDayOfMonth = new Date(year, month + 1, 0)
  
  // Starting day of week (Monday = 0)
  let startDay = firstDayOfMonth.getDay() - 1
  if (startDay === -1) startDay = 6 // Sunday
  
  const days = []
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  // Map of date-specific availability overrides
  const overrideMap = new Map<string, TourDateAvailability>()
  if (props.availableDates && props.availableDates.length > 0) {
    props.availableDates.forEach(av => {
      overrideMap.set(av.date, av)
    })
  }

  // Helper to compute realistic dynamic pricing and spot status for any date
  const computeDayData = (d: Date, isCurrentMonth: boolean) => {
    const dateStr = formatDateISO(d)
    const isPast = d < today
    const isToday = d.getTime() === today.getTime()
    const isSelected = props.modelValue === dateStr
    const dayOfWeek = d.getDay() // 0 = Sun, 5 = Fri, 6 = Sat

    let status: 'Available' | 'LowStock' | 'SoldOut' = 'Available'
    let spotsLeft = 18
    let priceEur = props.basePriceEur

    // Check backend overrides first
    if (overrideMap.has(dateStr)) {
      const ov = overrideMap.get(dateStr)!
      status = ov.status
      if (ov.priceEur) priceEur = ov.priceEur
      if (ov.spotsLeft !== undefined) spotsLeft = ov.spotsLeft
    } else {
      // Deterministic dynamic pattern based on date day number for authentic realistic variance
      const dayNum = d.getDate()
      if (dayNum % 13 === 0) {
        status = 'SoldOut'
        spotsLeft = 0
      } else if (dayNum % 6 === 0 || dayOfWeek === 5 || dayOfWeek === 6) {
        status = 'LowStock'
        spotsLeft = (dayNum % 4) + 2 // 2 to 5 spots
        priceEur = Math.round(props.basePriceEur * 1.1) // Weekend slight surge
      } else if (dayNum % 4 === 0) {
        priceEur = Math.max(20, Math.round(props.basePriceEur * 0.95)) // Mid-week special
      }
    }

    const priceFormatted = currencyStore.formatPrice(priceEur)

    return {
      date: d,
      dateStr,
      dayNumber: d.getDate(),
      isCurrentMonth,
      isPast,
      isToday,
      isSelected,
      status,
      spotsLeft,
      priceEur,
      priceFormatted
    }
  }

  // Previous month padding
  const prevMonthLastDay = new Date(year, month, 0).getDate()
  for (let i = startDay - 1; i >= 0; i--) {
    const d = new Date(year, month - 1, prevMonthLastDay - i)
    days.push(computeDayData(d, false))
  }

  // Current month days
  for (let i = 1; i <= lastDayOfMonth.getDate(); i++) {
    const d = new Date(year, month, i)
    days.push(computeDayData(d, true))
  }

  // Next month padding to complete 5 or 6 week grid
  const totalSlots = days.length <= 35 ? 35 : 42
  const remaining = totalSlots - days.length
  for (let i = 1; i <= remaining; i++) {
    const d = new Date(year, month + 1, i)
    days.push(computeDayData(d, false))
  }

  return days
})

const selectDate = (day: typeof calendarDays.value[0]) => {
  if (day.isPast || day.status === 'SoldOut') return
  emit('update:modelValue', day.dateStr)
  emit('change', day.dateStr)
}

const getStatusBadge = (day: typeof calendarDays.value[0]) => {
  if (day.isPast) return { label: t('calendar.pastDate'), dot: 'bg-slate-300', text: 'text-slate-400' }
  if (day.status === 'SoldOut') return { label: t('calendar.soldOut'), dot: 'bg-rose-500', text: 'text-rose-600' }
  if (day.status === 'LowStock') return { label: t('calendar.spotsLeft', { count: day.spotsLeft }), dot: 'bg-amber-500', text: 'text-amber-600' }
  return { label: t('calendar.available'), dot: 'bg-emerald-500', text: 'text-emerald-600' }
}
</script>

<template>
  <div class="w-full bg-white border border-slate-200/90 rounded-3xl p-4 sm:p-5 shadow-[0_4px_25px_rgba(6,45,77,0.06)] select-none">
    
    <!-- Month Navigation Header -->
    <div class="flex items-center justify-between mb-4 pb-3 border-b border-slate-100">
      <div>
        <h3 class="text-sm sm:text-base font-extrabold text-[#062d4d] tracking-wide font-serif capitalize">
          {{ currentMonthYearLabel }}
        </h3>
        <p class="text-[10px] text-slate-500 font-semibold mt-0.5">
          {{ t('calendar.subtitle') }}
        </p>
      </div>

      <div class="flex items-center gap-1.5">
        <button 
          type="button"
          @click="prevMonth"
          :disabled="isPrevMonthDisabled"
          class="w-8 h-8 rounded-full border border-slate-200 flex items-center justify-center text-slate-600 transition-all cursor-pointer"
          :class="isPrevMonthDisabled ? 'opacity-30 cursor-not-allowed bg-slate-50' : 'hover:border-[#c9a84c] hover:bg-slate-50 active:scale-95 text-[#062d4d] shadow-2xs'"
          :title="t('calendar.prevMonth')"
        >
          <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M15 19l-7-7 7-7"/></svg>
        </button>
        <button 
          type="button"
          @click="nextMonth"
          class="w-8 h-8 rounded-full border border-slate-200 hover:border-[#c9a84c] hover:bg-slate-50 active:scale-95 flex items-center justify-center text-[#062d4d] transition-all cursor-pointer shadow-2xs"
          :title="t('calendar.nextMonth')"
        >
          <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7"/></svg>
        </button>
      </div>
    </div>

    <!-- Weekday Header -->
    <div class="grid grid-cols-7 gap-1 text-center mb-2">
      <span 
        v-for="d in weekdays" 
        :key="d" 
        class="text-[11px] uppercase font-extrabold text-slate-400 py-1 tracking-wider"
      >
        {{ d }}
      </span>
    </div>

    <!-- Calendar Grid -->
    <div class="grid grid-cols-7 gap-1 text-center">
      <div
        v-for="day in calendarDays"
        :key="day.dateStr"
        class="relative flex flex-col items-center justify-center py-1.5"
        @mouseenter="!day.isPast && (hoveredDateStr = day.dateStr)"
        @mouseleave="hoveredDateStr = null"
      >
        <!-- Date Button Container -->
        <button
          type="button"
          :disabled="day.isPast || day.status === 'SoldOut'"
          @click="selectDate(day)"
          class="w-9 h-9 sm:w-10 sm:h-10 rounded-2xl flex flex-col items-center justify-center relative transition-all duration-200 cursor-pointer"
          :class="[
            day.isSelected 
              ? 'bg-[#062d4d] text-white font-black shadow-md ring-2 ring-[#c9a84c] scale-105 z-10' 
              : (day.isPast 
                ? 'text-slate-300 opacity-40 cursor-not-allowed' 
                : (day.status === 'SoldOut'
                  ? 'text-slate-400 bg-slate-100/80 cursor-not-allowed opacity-60 line-through'
                  : (day.isCurrentMonth
                    ? 'text-slate-800 hover:bg-[#f0f7fc] hover:text-[#062d4d] hover:scale-105 active:scale-95'
                    : 'text-slate-400 hover:bg-slate-50 hover:text-slate-600'))),
            day.isToday && !day.isSelected ? 'border border-[#c9a84c]/60 font-bold' : ''
          ]"
        >
          <span class="text-xs sm:text-sm leading-none">{{ day.dayNumber }}</span>
          
          <!-- Availability Status Indicator Dot -->
          <div class="h-1 flex items-center justify-center mt-1">
            <span 
              v-if="!day.isPast && day.status !== 'SoldOut'" 
              class="w-1.5 h-1.5 rounded-full"
              :class="[
                day.isSelected ? 'bg-[#c9a84c]' : (day.status === 'LowStock' ? 'bg-amber-500' : 'bg-emerald-500')
              ]"
            ></span>
          </div>
        </button>

        <!-- Emil Kowalski Tactile Floating Price & Availability Tooltip -->
        <Transition
          enter-active-class="transition duration-150 ease-out"
          enter-from-class="opacity-0 translate-y-1 scale-95"
          enter-to-class="opacity-100 translate-y-0 scale-100"
          leave-active-class="transition duration-100 ease-in"
          leave-from-class="opacity-100 translate-y-0 scale-100"
          leave-to-class="opacity-0 translate-y-1 scale-95"
        >
          <div 
            v-if="hoveredDateStr === day.dateStr && !day.isPast"
            class="absolute -top-12 left-1/2 -translate-x-1/2 bg-[#062d4d] text-white px-2.5 py-1.5 rounded-xl shadow-[0_10px_25px_rgba(6,45,77,0.35)] whitespace-nowrap z-30 pointer-events-none flex flex-col items-center"
          >
            <div class="flex items-center gap-1.5 text-[10px] font-bold">
              <span class="w-1.5 h-1.5 rounded-full" :class="getStatusBadge(day).dot"></span>
              <span v-if="day.status !== 'SoldOut'" class="text-[#c9a84c] font-mono">{{ day.priceFormatted }}</span>
              <span>{{ getStatusBadge(day).label }}</span>
            </div>
            <!-- Arrow Tip -->
            <div class="w-0 h-0 border-x-4 border-x-transparent border-t-4 border-t-[#062d4d] absolute -bottom-1 left-1/2 -translate-x-1/2"></div>
          </div>
        </Transition>
      </div>
    </div>

    <!-- Availability Legend Footer -->
    <div class="mt-4 pt-3 border-t border-slate-100 flex items-center justify-between text-[11px] font-bold text-slate-500">
      <div class="flex items-center gap-1.5">
        <span class="w-2 h-2 rounded-full bg-emerald-500"></span>
        <span>{{ t('calendar.legendAvailable') }}</span>
      </div>
      <div class="flex items-center gap-1.5">
        <span class="w-2 h-2 rounded-full bg-amber-500"></span>
        <span>{{ t('calendar.legendLimited') }}</span>
      </div>
      <div class="flex items-center gap-1.5">
        <span class="w-2 h-2 rounded-full bg-slate-300"></span>
        <span>{{ t('calendar.legendSoldOut') }}</span>
      </div>
    </div>
  </div>
</template>
