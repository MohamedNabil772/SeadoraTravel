<template>
  <div class="space-y-6">
    <LocaleSwitcher v-model="currentLocale" />
    <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
      <div>
        <h3 class="text-lg font-medium text-gray-900 mb-4">Inclusions</h3>
        <div class="space-y-3">
          <div class="flex items-center space-x-2">
            <input type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" :placeholder="`Included item (${currentLocale.toUpperCase()})`" />
            <button class="p-2 text-gray-400 hover:text-red-500"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg></button>
          </div>
          <button class="text-sm font-medium text-indigo-600 hover:text-indigo-700">+ Add Inclusion</button>
        </div>
      </div>
      <div>
        <h3 class="text-lg font-medium text-gray-900 mb-4">Exclusions</h3>
        <div class="space-y-3">
          <div class="flex items-center space-x-2">
            <input type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" :placeholder="`Excluded item (${currentLocale.toUpperCase()})`" />
            <button class="p-2 text-gray-400 hover:text-red-500"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg></button>
          </div>
          <button class="text-sm font-medium text-indigo-600 hover:text-indigo-700">+ Add Exclusion</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'

const currentLocale = ref('en')
</script>
