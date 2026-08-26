<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between mb-4">
      <LocaleSwitcher v-model="currentLocale" />
      
      <!-- Toggle for Day-based / Time-based -->
      <div class="flex items-center gap-3 bg-gray-100 p-1 rounded-lg">
        <button 
          @click="form.itineraryType = 'Day-based'"
          :class="['px-4 py-1.5 rounded-md text-sm font-medium transition-colors', form.itineraryType === 'Day-based' ? 'bg-white shadow-sm text-indigo-600' : 'text-gray-500 hover:text-gray-700']"
        >
          Day-based
        </button>
        <button 
          @click="form.itineraryType = 'Time-based'"
          :class="['px-4 py-1.5 rounded-md text-sm font-medium transition-colors', form.itineraryType === 'Time-based' ? 'bg-white shadow-sm text-indigo-600' : 'text-gray-500 hover:text-gray-700']"
        >
          Time-based
        </button>
      </div>
    </div>

    <div v-for="(step, index) in form.itinerary" :key="index" class="border border-gray-200 rounded-xl p-6 relative group">
      <button @click="removeStep(index)" class="absolute top-4 right-4 text-gray-400 hover:text-red-500 opacity-0 group-hover:opacity-100 transition-opacity">
        ✕
      </button>

      <div class="flex items-center gap-4 mb-4">
        <div class="flex-1 max-w-xs">
          <label :for="`step-${index}-label`" class="block text-sm font-medium text-gray-700">{{ form.itineraryType === 'Day-based' ? 'Day Number/Label' : 'Time' }}</label>
          <input :id="`step-${index}-label`" v-model="step.label" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" :placeholder="form.itineraryType === 'Day-based' ? 'e.g. Day 1' : 'e.g. 08:00 AM'" />
        </div>
      </div>

      <div class="space-y-4">
        <div class="space-y-2">
          <label :for="`step-${index}-title`" class="block text-sm font-medium text-gray-700">Title ({{ currentLocale.toUpperCase() }})</label>
          <input :id="`step-${index}-title`" v-model="step.titles[currentLocale]" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Step Title" />
        </div>
        <div class="space-y-2">
          <label :for="`step-${index}-description`" class="block text-sm font-medium text-gray-700">Description ({{ currentLocale.toUpperCase() }})</label>
          <textarea :id="`step-${index}-description`" v-model="step.descriptions[currentLocale]" rows="3" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Step Description"></textarea>
        </div>
      </div>
    </div>

    <button @click="addStep" class="mt-4 px-4 py-2 text-sm font-medium text-indigo-600 bg-indigo-50 rounded-lg hover:bg-indigo-100 transition-colors border border-indigo-100 border-dashed w-full flex justify-center items-center gap-2">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z" clip-rule="evenodd" />
      </svg>
      Add {{ form.itineraryType === 'Day-based' ? 'Day' : 'Time Step' }}
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, inject } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'
import { useLanguageStore } from '@/features/languages/store/languageStore'
import { storeToRefs } from 'pinia'

const currentLocale = ref('en')
const form = inject<any>('tourForm')
const languageStore = useLanguageStore()
const { languages } = storeToRefs(languageStore)

const addStep = () => {
  const newStep = { label: '', titles: {} as Record<string, string>, descriptions: {} as Record<string, string> }
  // Initialize with active languages to ensure reactivity if needed
  languages.value.forEach(lang => {
    newStep.titles[lang.code] = ''
    newStep.descriptions[lang.code] = ''
  })
  if (!form.value.itinerary) form.value.itinerary = []
  form.value.itinerary.push(newStep)
}

const removeStep = (index: number | string) => {
  form.value.itinerary.splice(Number(index), 1)
}
</script>
