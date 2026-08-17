<template>
  <div class="space-y-6">
    <LocaleSwitcher v-model="currentLocale" />
    <div class="border border-gray-200 rounded-xl p-6">
      <div class="space-y-4">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Question ({{ currentLocale.toUpperCase() }})</label>
          <input type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="e.g. What should I pack?" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Answer ({{ currentLocale.toUpperCase() }})</label>
          <textarea rows="3" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Provide a helpful answer"></textarea>
        </div>
      </div>
      <button class="mt-4 px-4 py-2 text-sm font-medium text-indigo-600 bg-indigo-50 rounded-lg hover:bg-indigo-100 transition-colors">
        + Add FAQ
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'

const currentLocale = ref('en')
</script>
