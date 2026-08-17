<template>
  <div class="space-y-6">
    <LocaleSwitcher v-model="currentLocale" />
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <div class="border border-gray-200 rounded-xl p-6 relative">
        <button class="absolute top-4 right-4 text-gray-400 hover:text-red-500">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
        </button>
        <div class="space-y-4">
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700">Addon Name ({{ currentLocale.toUpperCase() }})</label>
            <input type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="e.g. Airport Transfer" />
          </div>
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700">Price (USD)</label>
            <input type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="0.00" />
          </div>
        </div>
      </div>
      <div class="border-2 border-dashed border-gray-200 rounded-xl p-6 flex flex-col items-center justify-center text-center hover:bg-gray-50 transition-colors cursor-pointer text-gray-500 hover:text-indigo-600">
        <svg class="w-8 h-8 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" /></svg>
        <span class="font-medium">Add New Addon</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'

const currentLocale = ref('en')
</script>
