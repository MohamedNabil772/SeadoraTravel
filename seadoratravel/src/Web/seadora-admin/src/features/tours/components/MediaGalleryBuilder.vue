<template>
  <div class="space-y-6">
    <div class="border-2 border-dashed border-gray-300 rounded-xl p-12 text-center hover:bg-gray-50 transition-colors">
      <div class="flex flex-col items-center">
        <svg class="w-12 h-12 text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg>
        <p class="text-lg font-medium text-gray-900">Drop images here or click to upload</p>
        <p class="text-sm text-gray-500 mt-1">PNG, JPG up to 5MB</p>
        <button class="mt-4 px-4 py-2 bg-white border border-gray-300 rounded-lg text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50">
          Select Files
        </button>
      </div>
    </div>
    
    <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mt-6">
      <div class="aspect-square bg-gray-100 rounded-lg border border-gray-200 flex items-center justify-center relative group overflow-hidden">
        <span class="text-sm text-gray-400">Preview 1</span>
        <button class="absolute top-2 right-2 p-1.5 bg-red-500 text-white rounded-md opacity-0 group-hover:opacity-100 transition-opacity shadow-sm">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Media gallery doesn't typically need locale switcher for image uploads, but might for alt text. Kept simple.
</script>
