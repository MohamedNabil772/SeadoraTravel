<template>
  <div class="space-y-6">
    <LocaleSwitcher v-model="currentLocale" />
    <div v-for="(addon, index) in form.addons" :key="index" class="border border-gray-200 rounded-xl p-6 relative group bg-white shadow-sm">
      <button @click="removeAddon(index)" class="absolute top-4 right-4 text-gray-400 hover:text-red-500 opacity-0 group-hover:opacity-100 transition-opacity">
        ✕
      </button>
      <div class="space-y-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700">Addon Name ({{ currentLocale.toUpperCase() }})</label>
            <input v-model="addon.names[currentLocale]" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="e.g. Extra Luggage" />
          </div>
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700">Price (EUR)</label>
            <input v-model.number="addon.priceEur" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="0.00" />
          </div>
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700">Icon Emoji</label>
            <input v-model="addon.icon" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="e.g. 🧳" />
          </div>
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700">Category</label>
            <select v-model="addon.category" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500">
              <option value="equipment">Equipment</option>
              <option value="food">Food & Drink</option>
              <option value="transfer">Transfer</option>
              <option value="other">Other</option>
            </select>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <input type="checkbox" v-model="addon.isPerPerson" class="w-4 h-4 text-indigo-600 rounded" />
          <label class="text-sm font-medium text-gray-700">Price is Per Person</label>
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Description ({{ currentLocale.toUpperCase() }})</label>
          <textarea v-model="addon.descriptions[currentLocale]" rows="2" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Describe the addon..."></textarea>
        </div>
      </div>
    </div>

    <button @click="addAddon" class="mt-4 px-4 py-2 text-sm font-medium text-indigo-600 bg-indigo-50 rounded-lg hover:bg-indigo-100 transition-colors border border-indigo-100 border-dashed w-full flex justify-center items-center gap-2">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z" clip-rule="evenodd" />
      </svg>
      Add Addon
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

const addAddon = () => {
  const newAddon = { 
    priceEur: 0, 
    isPerPerson: true, 
    icon: '', 
    category: 'other', 
    names: {} as Record<string, string>, 
    descriptions: {} as Record<string, string> 
  }
  languages.value.forEach(lang => {
    newAddon.names[lang.code] = ''
    newAddon.descriptions[lang.code] = ''
  })
  if (!form.value.addons) form.value.addons = []
  form.value.addons.push(newAddon)
}

const removeAddon = (index: number | string) => {
  form.value.addons.splice(Number(index), 1)
}
</script>
