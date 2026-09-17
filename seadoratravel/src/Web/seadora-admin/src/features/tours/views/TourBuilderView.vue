<script setup lang="ts">
import { ref, computed, onMounted, provide } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import api from '@/services/api'
import { useToast } from '@/composables/useToast'
import TourEditor from '../components/TourEditor.vue'
import ItineraryBuilder from '../components/ItineraryBuilder.vue'
import FaqBuilder from '../components/FaqBuilder.vue'
import AddonsBuilder from '../components/AddonsBuilder.vue'
import InclusionsBuilder from '../components/InclusionsBuilder.vue'
import MediaGalleryBuilder from '../components/MediaGalleryBuilder.vue'
import PackagesBuilder from '../components/PackagesBuilder.vue'

const router = useRouter()
const route = useRoute()
const toast = useToast()

const isEditing = computed(() => route.name === 'tour-edit' || !!route.params.id)
const currentTab = ref('basic')
const isSaving = ref(false)

const tabs = [
  { id: 'basic', name: 'Basic Information' },
  { id: 'packages', name: 'Packages' },
  { id: 'itinerary', name: 'Itinerary' },
  { id: 'inclusions', name: 'Inclusions & Important Info' },
  { id: 'addons', name: 'Addons' },
  { id: 'media', name: 'Media Gallery' },
  { id: 'faqs', name: 'FAQs' }
]

const form = ref({
  id: '',
  names: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  descriptions: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  highlights: { en: [], de: [], it: [], fr: [], ru: [] } as Record<string, string[]>,
  price: 0,
  currency: 'USD',
  originalPrice: 0,
  discountPercentage: 0,
  duration: '',
  startTime: '',
  rating: 0,
  reviewCount: 0,
  imageUrl: '',
  mediaUrls: [] as string[],
  mediaGallery: [] as { url: string, caption: string }[],
  emoji: '⛵',
  bgGradient: 'linear-gradient(135deg,#063a5c,#1a9b8a)',
  badge: '',
  destinationId: '',
  categoryId: '',
  tourTypeId: '',
  groupMinCapacity: 1,
  groupMaxCapacity: 20,
  supplierId: '',
  supplierPercentage: 0,
  maxAllocations: 20,
  isTopRated: false,
  isBestseller: false,
  isInHighDemand: false,
  reserveAndPayLater: false,
  hotelPickup: false,
  freeCancellation: false,
  isPrivateOption: false,
  pickupTimeType: 'FixedSlots',
  availablePickupTimes: [] as string[],
  packages: [] as any[],
  itinerary: [] as any[],
  itineraryType: 'Day-based',
  inclusions: { en: [], de: [], it: [], fr: [], ru: [] } as Record<string, string[]>,
  exclusions: { en: [], de: [], it: [], fr: [], ru: [] } as Record<string, string[]>,
  importantInfo: {
    whatToBring: { en: [], de: [], it: [], fr: [], ru: [] },
    notSuitableFor: { en: [], de: [], it: [], fr: [], ru: [] },
    notes: { en: [], de: [], it: [], fr: [], ru: [] }
  } as any,
  faqs: [] as any[],
  addons: [] as any[],
})

provide('tourForm', form)

const langs = ['en', 'de', 'it', 'fr', 'ru']

onMounted(async () => {
  if (isEditing.value) {
    const id = route.params.id as string
    try {
      let tourData: any = null
      try {
        const res = await api.get(`/api/content/api/admin/tours/${id}`)
        tourData = res.data
      } catch {
        const res = await api.get(`/api/content/api/tours/${id}`)
        tourData = res.data
      }

      if (tourData) {
        form.value.id = tourData.id || id
        form.value.names = { en: '', de: '', it: '', fr: '', ru: '', ...(tourData.names || {}) }
        form.value.descriptions = { en: '', de: '', it: '', fr: '', ru: '', ...(tourData.descriptions || {}) }
        form.value.price = Number(tourData.price) || 0
        form.value.currency = tourData.currency || 'USD'
        form.value.originalPrice = Number(tourData.originalPrice) || 0
        form.value.discountPercentage = Number(tourData.discountPercentage) || 0
        form.value.duration = tourData.duration || ''
        form.value.startTime = tourData.startTime || ''
        form.value.rating = Number(tourData.rating) || 0
        form.value.reviewCount = Number(tourData.reviewCount) || 0
        form.value.imageUrl = tourData.imageUrl || ''
        form.value.emoji = tourData.emoji || '⛵'
        form.value.bgGradient = tourData.bgGradient || 'linear-gradient(135deg,#063a5c,#1a9b8a)'
        form.value.badge = tourData.badge || ''
        form.value.destinationId = tourData.destinationId || ''
        form.value.categoryId = tourData.categoryId || ''
        form.value.tourTypeId = tourData.tourTypeId || ''
        form.value.supplierId = tourData.supplierId || ''
        form.value.supplierPercentage = Number(tourData.supplierPercentage) || 0
        form.value.maxAllocations = Number(tourData.maxAllocations) || 20
        form.value.groupMinCapacity = Number(tourData.groupMinCapacity) || 1
        form.value.groupMaxCapacity = Number(tourData.groupMaxCapacity) || 20
        form.value.isTopRated = Boolean(tourData.isTopRated)
        form.value.isBestseller = Boolean(tourData.isBestseller)
        form.value.isInHighDemand = Boolean(tourData.isInHighDemand)
        form.value.reserveAndPayLater = Boolean(tourData.reserveAndPayLater)
        form.value.hotelPickup = Boolean(tourData.hotelPickup)
        form.value.freeCancellation = Boolean(tourData.freeCancellation)
        form.value.isPrivateOption = Boolean(tourData.isPrivateOption)
        form.value.pickupTimeType = tourData.pickupTimeType || 'FixedSlots'
        form.value.availablePickupTimes = Array.isArray(tourData.availablePickupTimes) && tourData.availablePickupTimes.length > 0
          ? [...tourData.availablePickupTimes]
          : ['15:00 - 15:30 (Sunset)', '10:00 - 10:30 (Morning)']
        form.value.packages = Array.isArray(tourData.packages) ? tourData.packages : []
        form.value.faqs = Array.isArray(tourData.faqs) ? tourData.faqs : []
        form.value.addons = Array.isArray(tourData.addons) ? tourData.addons : []

        // Highlights -> Record<string, string[]>
        const hlMap: Record<string, string[]> = { en: [], de: [], it: [], fr: [], ru: [] }
        if (tourData.highlights) {
          langs.forEach(l => {
            const val = tourData.highlights[l]
            if (Array.isArray(val)) {
              hlMap[l] = val
            } else if (typeof val === 'string' && val.trim()) {
              hlMap[l] = val.split(',').map((s: string) => s.trim()).filter(Boolean)
            }
          })
        }
        form.value.highlights = hlMap

        // Inclusions -> Record<string, string[]>
        const incMap: Record<string, string[]> = { en: [], de: [], it: [], fr: [], ru: [] }
        if (Array.isArray(tourData.inclusions)) {
          tourData.inclusions.forEach((inc: any) => {
            langs.forEach(l => {
              const text = inc?.names?.[l] || (typeof inc === 'string' ? inc : '')
              if (text) incMap[l].push(text)
            })
          })
        } else if (tourData.inclusions && typeof tourData.inclusions === 'object') {
          langs.forEach(l => {
            incMap[l] = Array.isArray(tourData.inclusions[l]) ? [...tourData.inclusions[l]] : []
          })
        }
        form.value.inclusions = incMap

        // Exclusions -> Record<string, string[]>
        const excMap: Record<string, string[]> = { en: [], de: [], it: [], fr: [], ru: [] }
        if (Array.isArray(tourData.exclusions)) {
          tourData.exclusions.forEach((exc: any) => {
            langs.forEach(l => {
              const text = exc?.names?.[l] || (typeof exc === 'string' ? exc : '')
              if (text) excMap[l].push(text)
            })
          })
        } else if (tourData.exclusions && typeof tourData.exclusions === 'object') {
          langs.forEach(l => {
            excMap[l] = Array.isArray(tourData.exclusions[l]) ? [...tourData.exclusions[l]] : []
          })
        }
        form.value.exclusions = excMap

        // ImportantInfo -> Record<string, string[]>
        const mapToArrays = (source: any) => {
          const res: Record<string, string[]> = { en: [], de: [], it: [], fr: [], ru: [] }
          if (!source) return res
          langs.forEach(l => {
            const val = source[l]
            if (Array.isArray(val)) {
              res[l] = val
            } else if (typeof val === 'string' && val.trim()) {
              res[l] = val.split('\n').map((s: string) => s.trim()).filter(Boolean)
            }
          })
          return res
        }
        const infoSource = tourData.importantInfo || tourData.importantInformation || {}
        form.value.importantInfo = {
          whatToBring: mapToArrays(infoSource.whatToBring),
          notSuitableFor: mapToArrays(infoSource.notSuitableFor),
          notes: mapToArrays(infoSource.notes)
        }

        // Itinerary
        if (Array.isArray(tourData.itinerary)) {
          form.value.itinerary = tourData.itinerary.map((step: any) => ({
            label: step.label || step.timeString || (step.dayNumber ? `Day ${step.dayNumber}` : ''),
            titles: step.titles || { en: '', de: '', it: '', fr: '', ru: '' },
            descriptions: step.descriptions || { en: '', de: '', it: '', fr: '', ru: '' }
          }))
          if (tourData.itinerary.length > 0 && tourData.itinerary[0].itineraryType) {
            form.value.itineraryType = tourData.itinerary[0].itineraryType === 'Day' ? 'Day-based' : 'Time-based'
          }
        }

        // Media Gallery
        if (Array.isArray(tourData.media) && tourData.media.length > 0) {
          form.value.mediaGallery = tourData.media.map((m: any) => ({
            url: m.url,
            caption: m.captions?.en || m.captions?.de || ''
          }))
        } else if (Array.isArray(tourData.mediaUrls)) {
          form.value.mediaGallery = tourData.mediaUrls.map((url: string) => ({
            url,
            caption: ''
          }))
        }
      }
    } catch (e: any) {
      console.error('Failed to load tour data', e)
      toast.error('Failed to load tour data')
    }
  }
})

const saveTour = async () => {
  try {
    isSaving.value = true

    // 1. Highlights: Record<string, string>
    const highlightsDto: Record<string, string> = {}
    langs.forEach(l => {
      const items = form.value.highlights?.[l] || []
      if (items.length > 0) {
        highlightsDto[l] = items.join(', ')
      }
    })

    // 2. Inclusions: List<AdminInclusionDto>
    const maxInc = Math.max(0, ...langs.map(l => form.value.inclusions?.[l]?.length || 0))
    const inclusionsDto: Array<{ names: Record<string, string> }> = []
    for (let i = 0; i < maxInc; i++) {
      const names: Record<string, string> = {}
      let hasAny = false
      for (const l of langs) {
        const val = form.value.inclusions?.[l]?.[i]?.trim()
        if (val) {
          names[l] = val
          hasAny = true
        }
      }
      if (hasAny) inclusionsDto.push({ names })
    }

    // 3. Exclusions: List<AdminInclusionDto>
    const maxExc = Math.max(0, ...langs.map(l => form.value.exclusions?.[l]?.length || 0))
    const exclusionsDto: Array<{ names: Record<string, string> }> = []
    for (let i = 0; i < maxExc; i++) {
      const names: Record<string, string> = {}
      let hasAny = false
      for (const l of langs) {
        const val = form.value.exclusions?.[l]?.[i]?.trim()
        if (val) {
          names[l] = val
          hasAny = true
        }
      }
      if (hasAny) exclusionsDto.push({ names })
    }

    // 4. ImportantInfo: AdminImportantInfoDto
    const mapListToDict = (recordOfLists: Record<string, string[]> | undefined) => {
      const res: Record<string, string> = {}
      if (!recordOfLists) return res
      for (const l of langs) {
        const items = (recordOfLists[l] || []).filter((s: string) => s && s.trim().length > 0)
        if (items.length > 0) {
          res[l] = items.join('\n')
        }
      }
      return res
    }
    const importantInfoDto = {
      whatToBring: mapListToDict(form.value.importantInfo?.whatToBring),
      notSuitableFor: mapListToDict(form.value.importantInfo?.notSuitableFor),
      notes: mapListToDict(form.value.importantInfo?.notes)
    }

    // 5. Itinerary: List<AdminItineraryDto>
    const itineraryDto = (form.value.itinerary || []).map((step: any, idx: number) => {
      const isDay = form.value.itineraryType === 'Day-based'
      let dayNum: number | null = null
      let timeStr: string | null = null
      if (isDay) {
        const parsed = parseInt(String(step.label || '').replace(/\D/g, ''), 10)
        dayNum = isNaN(parsed) ? idx + 1 : parsed
      } else {
        timeStr = step.label || ''
      }
      return {
        itineraryType: isDay ? 'Day' : 'Time',
        dayNumber: dayNum,
        timeString: timeStr,
        titles: step.titles || {},
        descriptions: step.descriptions || {}
      }
    })

    // 6. Media
    const mediaList = (form.value.mediaGallery || []).filter((m: any) => m.url).map((m: any) => ({
      url: m.url,
      captions: { en: m.caption || '' }
    }))
    const mediaUrls = mediaList.map((m: any) => m.url)

    const payload: any = {
      id: form.value.id || undefined,
      names: form.value.names,
      descriptions: form.value.descriptions,
      highlights: highlightsDto,
      price: Number(form.value.price) || 0,
      currency: form.value.currency || 'USD',
      originalPrice: form.value.originalPrice ? Number(form.value.originalPrice) : null,
      discountPercentage: form.value.discountPercentage ? Number(form.value.discountPercentage) : null,
      duration: form.value.duration || '',
      startTime: form.value.startTime || '',
      rating: Number(form.value.rating) || 0,
      reviewCount: Number(form.value.reviewCount) || 0,
      imageUrl: form.value.imageUrl || '',
      emoji: form.value.emoji || '⛵',
      bgGradient: form.value.bgGradient || 'linear-gradient(135deg,#063a5c,#1a9b8a)',
      badge: form.value.badge || '',
      destinationId: form.value.destinationId || undefined,
      categoryId: form.value.categoryId || undefined,
      tourTypeId: form.value.tourTypeId || null,
      supplierId: form.value.supplierId || null,
      supplierPercentage: Number(form.value.supplierPercentage) || 0,
      maxAllocations: Number(form.value.maxAllocations) || 20,
      groupMinCapacity: form.value.groupMinCapacity ? Number(form.value.groupMinCapacity) : 1,
      groupMaxCapacity: form.value.groupMaxCapacity ? Number(form.value.groupMaxCapacity) : 20,
      isTopRated: Boolean(form.value.isTopRated),
      isBestseller: Boolean(form.value.isBestseller),
      isInHighDemand: Boolean(form.value.isInHighDemand),
      reserveAndPayLater: Boolean(form.value.reserveAndPayLater),
      hotelPickup: Boolean(form.value.hotelPickup),
      freeCancellation: Boolean(form.value.freeCancellation),
      isPrivateOption: Boolean(form.value.isPrivateOption),
      pickupTimeType: form.value.pickupTimeType || 'FixedSlots',
      availablePickupTimes: form.value.availablePickupTimes || [],
      packages: form.value.packages || [],
      itinerary: itineraryDto,
      inclusions: inclusionsDto,
      exclusions: exclusionsDto,
      importantInfo: importantInfoDto,
      faqs: form.value.faqs || [],
      addons: form.value.addons || [],
      media: mediaList,
      mediaUrls: mediaUrls
    }

    if (isEditing.value) {
      payload.id = form.value.id
      try {
        await api.put(`/api/content/api/admin/tours/${form.value.id}`, payload)
      } catch (errAdmin: any) {
        // Fallback to general tours endpoint if needed
        if (errAdmin.response?.status === 404) {
          await api.put(`/api/content/api/tours/${form.value.id}`, payload)
        } else {
          throw errAdmin
        }
      }
      toast.success('Tour updated successfully')
    } else {
      try {
        await api.post('/api/content/api/admin/tours', payload)
      } catch (errAdmin: any) {
        if (errAdmin.response?.status === 404) {
          await api.post('/api/content/api/tours', payload)
        } else {
          throw errAdmin
        }
      }
      toast.success('Tour created successfully')
    }
    router.push({ name: 'tours' })
  } catch (e: any) {
    console.error('Failed to save tour', e)
    const msg = e.response?.data?.title || e.response?.data?.message || (e.response?.data?.errors ? JSON.stringify(e.response.data.errors) : null) || e.message || 'Failed to save tour'
    toast.error(msg)
  } finally {
    isSaving.value = false
  }
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
          class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-all duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] shadow-sm hover:-translate-y-[1px] active:scale-95 active:translate-y-0"
        >
          Cancel
        </button>
        <button
          @click="saveTour"
          :disabled="isSaving"
          class="px-4 py-2 text-sm font-medium text-white bg-indigo-600 rounded-lg hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-all duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] shadow-sm hover:shadow-md hover:-translate-y-[1px] active:scale-95 active:translate-y-0 disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
        >
          <span v-if="isSaving" class="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full"></span>
          {{ isSaving ? 'Saving...' : 'Save Tour' }}
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
            <PackagesBuilder v-else-if="currentTab === 'packages'" />
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
