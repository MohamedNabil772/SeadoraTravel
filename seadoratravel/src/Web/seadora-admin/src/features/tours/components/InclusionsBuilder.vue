<template>
  <div class="space-y-8">
    <LocaleSwitcher v-model="currentLocale" />
    
    <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
      <div class="space-y-4 bg-white p-6 rounded-xl border border-gray-200">
        <h3 class="text-lg font-bold text-gray-900 border-b pb-2">What's Included ({{ currentLocale.toUpperCase() }})</h3>
        <div v-for="(_, index) in form.inclusions[currentLocale]" :key="'inc-'+index" class="flex items-center gap-2">
          <input v-model="form.inclusions[currentLocale][index]" type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. Hotel pickup" />
          <button @click="form.inclusions[currentLocale].splice(index, 1)" class="text-red-500 hover:text-red-700 font-bold px-2">✕</button>
        </div>
        <button @click="addInclusion" class="text-indigo-600 text-sm font-medium hover:text-indigo-800">+ Add Inclusion</button>
      </div>
      
      <div class="space-y-4 bg-white p-6 rounded-xl border border-gray-200">
        <h3 class="text-lg font-bold text-gray-900 border-b pb-2">What's Not Included ({{ currentLocale.toUpperCase() }})</h3>
        <div v-for="(_, index) in form.exclusions[currentLocale]" :key="'exc-'+index" class="flex items-center gap-2">
          <input v-model="form.exclusions[currentLocale][index]" type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. Gratuities" />
          <button @click="form.exclusions[currentLocale].splice(index, 1)" class="text-red-500 hover:text-red-700 font-bold px-2">✕</button>
        </div>
        <button @click="addExclusion" class="text-indigo-600 text-sm font-medium hover:text-indigo-800">+ Add Exclusion</button>
      </div>
    </div>

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Important Information ({{ currentLocale.toUpperCase() }})</h3>
      
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="space-y-4">
          <label class="block text-sm font-medium text-gray-700">What to Bring</label>
          <div v-for="(_, index) in form.importantInfo.whatToBring[currentLocale]" :key="'wtb-'+index" class="flex items-center gap-2">
            <input v-model="form.importantInfo.whatToBring[currentLocale][index]" type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. Passport" />
            <button @click="form.importantInfo.whatToBring[currentLocale].splice(index, 1)" class="text-red-500 hover:text-red-700 font-bold px-2">✕</button>
          </div>
          <button @click="addWhatToBring" class="text-indigo-600 text-sm font-medium hover:text-indigo-800">+ Add Item</button>
        </div>

        <div class="space-y-4">
          <label class="block text-sm font-medium text-gray-700">Not Suitable For</label>
          <div v-for="(_, index) in form.importantInfo.notSuitableFor[currentLocale]" :key="'nsf-'+index" class="flex items-center gap-2">
            <input v-model="form.importantInfo.notSuitableFor[currentLocale][index]" type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. Wheelchair users" />
            <button @click="form.importantInfo.notSuitableFor[currentLocale].splice(index, 1)" class="text-red-500 hover:text-red-700 font-bold px-2">✕</button>
          </div>
          <button @click="addNotSuitableFor" class="text-indigo-600 text-sm font-medium hover:text-indigo-800">+ Add Item</button>
        </div>
        
        <div class="space-y-4 md:col-span-2">
          <label class="block text-sm font-medium text-gray-700">Notes / Know Before You Go</label>
          <div v-for="(_, index) in form.importantInfo.notes[currentLocale]" :key="'note-'+index" class="flex items-center gap-2">
            <input v-model="form.importantInfo.notes[currentLocale][index]" type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. Please wait in the hotel lobby" />
            <button @click="form.importantInfo.notes[currentLocale].splice(index, 1)" class="text-red-500 hover:text-red-700 font-bold px-2">✕</button>
          </div>
          <button @click="addNote" class="text-indigo-600 text-sm font-medium hover:text-indigo-800">+ Add Note</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, inject, onMounted } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'

const currentLocale = ref('en')
const form = inject<any>('tourForm')

const initLangStructure = (obj: any) => {
  const langs = ['en', 'de', 'it', 'fr', 'ru']
  langs.forEach(l => {
    if (!obj[l]) obj[l] = []
  })
}

onMounted(() => {
  if (!form.value.inclusions) form.value.inclusions = {}
  initLangStructure(form.value.inclusions)

  if (!form.value.exclusions) form.value.exclusions = {}
  initLangStructure(form.value.exclusions)
  
  if (!form.value.importantInfo) form.value.importantInfo = { whatToBring: {}, notSuitableFor: {}, notes: {} }
  if (!form.value.importantInfo.whatToBring) form.value.importantInfo.whatToBring = {}
  if (!form.value.importantInfo.notSuitableFor) form.value.importantInfo.notSuitableFor = {}
  if (!form.value.importantInfo.notes) form.value.importantInfo.notes = {}
  initLangStructure(form.value.importantInfo.whatToBring)
  initLangStructure(form.value.importantInfo.notSuitableFor)
  initLangStructure(form.value.importantInfo.notes)
})

const addInclusion = () => form.value.inclusions[currentLocale.value].push('')
const addExclusion = () => form.value.exclusions[currentLocale.value].push('')
const addWhatToBring = () => form.value.importantInfo.whatToBring[currentLocale.value].push('')
const addNotSuitableFor = () => form.value.importantInfo.notSuitableFor[currentLocale.value].push('')
const addNote = () => form.value.importantInfo.notes[currentLocale.value].push('')
</script>
