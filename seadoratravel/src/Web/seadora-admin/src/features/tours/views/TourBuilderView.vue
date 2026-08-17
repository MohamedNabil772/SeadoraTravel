<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import TourEditor from '../components/TourEditor.vue'
import ItineraryBuilder from '../components/ItineraryBuilder.vue'
import FaqBuilder from '../components/FaqBuilder.vue'
import AddonsBuilder from '../components/AddonsBuilder.vue'
import InclusionsBuilder from '../components/InclusionsBuilder.vue'
import MediaGalleryBuilder from '../components/MediaGalleryBuilder.vue'

const router = useRouter()
const route = useRoute()

const isEditing = computed(() => route.name === 'tour-edit')
const currentTab = ref('basic')

const tabs = [
  { id: 'basic', name: 'Basic Information' },
  { id: 'itinerary', name: 'Itinerary' },
  { id: 'inclusions', name: 'Inclusions & Exclusions' },
  { id: 'addons', name: 'Addons' },
  { id: 'media', name: 'Media Gallery' },
  { id: 'faqs', name: 'FAQs' }
]

const saveTour = async () => {
  // Mock save logic
  console.log('Saving tour...')
  router.push({ name: 'tours' })
}

const cancel = () => {
  router.push({ name: 'tours' })
}
</script>

<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 animate-fade-in-up">
    <!-- Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-gray-900 tracking-tight">
          {{ isEditing ? 'Edit Tour' : 'Create New Tour' }}
        </h1>
        <p class="mt-2 text-sm text-gray-500">
          Build and customize your travel experience.
        </p>
      </div>
      <div class="flex items-center gap-4">
        <button
          @click="cancel"
          class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-all shadow-sm"
        >
          Cancel
        </button>
        <button
          @click="saveTour"
          class="px-4 py-2 text-sm font-medium text-white bg-indigo-600 rounded-lg hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-all shadow-sm hover:shadow-md"
        >
          Save Tour
        </button>
      </div>
    </div>

    <!-- Tabs Container -->
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
      <div class="border-b border-gray-200">
        <nav class="flex -mb-px px-6 space-x-8 overflow-x-auto" aria-label="Tabs">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            @click="currentTab = tab.id"
            class="whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors duration-200"
            :class="[
              currentTab === tab.id
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            {{ tab.name }}
          </button>
        </nav>
      </div>

      <!-- Content Area with Transition -->
      <div class="p-6">
        <transition name="fade-slide" mode="out-in">
          <div :key="currentTab" class="w-full">
            <TourEditor v-if="currentTab === 'basic'" />
            <ItineraryBuilder v-else-if="currentTab === 'itinerary'" />
            <InclusionsBuilder v-else-if="currentTab === 'inclusions'" />
            <AddonsBuilder v-else-if="currentTab === 'addons'" />
            <MediaGalleryBuilder v-else-if="currentTab === 'media'" />
            <FaqBuilder v-else-if="currentTab === 'faqs'" />
          </div>
        </transition>
      </div>
    </div>
  </div>
</template>

<style scoped>
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(10px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

.animate-fade-in-up {
  animation: fadeInUp 0.5s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
