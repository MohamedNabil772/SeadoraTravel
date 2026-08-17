<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { Motion } from 'motion-v'

const { t } = useI18n()

const activePreset = ref('thisWeekend')

const presets = computed(() => [
  { key: 'thisWeekend', label: t('calendar.thisWeekend') },
  { key: 'next7Days', label: t('calendar.next7Days') },
  { key: 'next14Days', label: t('calendar.next14Days') },
  { key: 'thisMonth', label: t('calendar.thisMonth') }
])

const weekdays = computed(() => [
  t('calendar.weekdays.mo'),
  t('calendar.weekdays.tu'),
  t('calendar.weekdays.we'),
  t('calendar.weekdays.th'),
  t('calendar.weekdays.fr'),
  t('calendar.weekdays.sa'),
  t('calendar.weekdays.su')
])

const springTransition = {
  type: "spring",
  stiffness: 300,
  damping: 30
}
</script>

<template>
  <div class="flex flex-col gap-4 p-4 bg-white border border-neutral-200 rounded-2xl shadow-sm max-w-sm">
    <div class="flex items-center justify-between">
      <h3 class="text-sm font-medium tracking-tight text-neutral-900">{{ t('calendar.selectDates') }}</h3>
      <button class="text-xs text-neutral-500 hover:text-neutral-900 transition-colors">{{ t('calendar.clear') }}</button>
    </div>

    <!-- Presets -->
    <div class="flex flex-wrap gap-2">
      <button
        v-for="preset in presets"
        :key="preset.key"
        @click="activePreset = preset.key"
        class="relative px-3 py-1.5 text-xs font-medium rounded-full transition-colors outline-none border border-transparent cursor-pointer"
        :class="activePreset === preset.key ? 'text-neutral-900' : 'text-neutral-600 bg-neutral-100 hover:bg-neutral-200'"
      >
        <span class="relative z-10">{{ preset.label }}</span>
        <Motion
          v-if="activePreset === preset.key"
          layoutId="activeDatePreset"
          class="absolute inset-0 bg-neutral-900 rounded-full"
          :transition="springTransition"
        />
        <span v-if="activePreset === preset.key" class="absolute inset-0 z-20 flex items-center justify-center text-white mix-blend-difference">{{ preset.label }}</span>
      </button>
    </div>

    <hr class="border-neutral-100" />

    <!-- Calendar stub -->
    <div class="grid grid-cols-7 gap-1 text-center text-xs">
      <div v-for="d in weekdays" :key="d" class="text-neutral-400 font-medium py-1">{{ d }}</div>
      
      <div v-for="i in 30" :key="i" 
           class="aspect-square flex items-center justify-center rounded-full hover:bg-neutral-100 cursor-pointer text-neutral-700 transition-colors">
        {{ i }}
      </div>
    </div>
  </div>
</template>
