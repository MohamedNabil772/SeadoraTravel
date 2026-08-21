<template>
  <div class="space-y-6">
    <LocaleSwitcher v-model="currentLocale" />
    <div v-for="(faq, index) in form.faqs" :key="index" class="border border-gray-200 rounded-xl p-6 relative group">
      <button @click="removeFaq(index)" class="absolute top-4 right-4 text-gray-400 hover:text-red-500 opacity-0 group-hover:opacity-100 transition-opacity">
        ✕
      </button>
      <div class="space-y-4">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Question ({{ currentLocale.toUpperCase() }})</label>
          <input v-model="faq.questions[currentLocale]" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="e.g. What should I bring?" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Answer ({{ currentLocale.toUpperCase() }})</label>
          <textarea v-model="faq.answers[currentLocale]" rows="3" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" placeholder="Provide a helpful answer"></textarea>
        </div>
      </div>
    </div>

    <button @click="addFaq" class="mt-4 px-4 py-2 text-sm font-medium text-indigo-600 bg-indigo-50 rounded-lg hover:bg-indigo-100 transition-colors border border-indigo-100 border-dashed w-full flex justify-center items-center gap-2">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z" clip-rule="evenodd" />
      </svg>
      Add FAQ
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

const addFaq = () => {
  const newFaq = { questions: {} as Record<string, string>, answers: {} as Record<string, string> }
  languages.value.forEach(lang => {
    newFaq.questions[lang.code] = ''
    newFaq.answers[lang.code] = ''
  })
  if (!form.value.faqs) form.value.faqs = []
  form.value.faqs.push(newFaq)
}

const removeFaq = (index: number | string) => {
  form.value.faqs.splice(Number(index), 1)
}
</script>
