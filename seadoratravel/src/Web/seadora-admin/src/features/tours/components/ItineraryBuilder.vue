<template>
  <div class="space-y-6">
    <LocaleSwitcher v-model="currentLocale" />
    <div class="border border-gray-200 rounded-xl p-6">
      <h3 class="text-lg font-medium text-gray-900 mb-4">Day 1</h3>
      <div class="space-y-4">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Day Title ({{ currentLocale.toUpperCase() }})</label>
          <input type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Arrival & Welcome" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Activities Description ({{ currentLocale.toUpperCase() }})</label>
          <textarea rows="3" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Describe the day's events"></textarea>
        </div>
      </div>
      <button class="mt-4 px-4 py-2 text-sm font-medium text-indigo-600 bg-indigo-50 rounded-lg hover:bg-indigo-100 transition-colors">
        + Add Another Day
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'

const currentLocale = ref('en')
</script>
