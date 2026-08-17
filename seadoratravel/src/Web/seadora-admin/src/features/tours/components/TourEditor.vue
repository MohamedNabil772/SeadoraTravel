<template>
  <div class="space-y-6">
    <LocaleSwitcher v-model="currentLocale" />
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-700">Tour Title ({{ currentLocale.toUpperCase() }})</label>
        <input type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Enter tour title" />
      </div>
      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-700">Duration (Days)</label>
        <input type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="e.g. 7" />
      </div>
      <div class="space-y-2 md:col-span-2">
        <label class="block text-sm font-medium text-gray-700">Description ({{ currentLocale.toUpperCase() }})</label>
        <textarea rows="4" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Enter full tour description"></textarea>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'

const currentLocale = ref('en')
</script>
