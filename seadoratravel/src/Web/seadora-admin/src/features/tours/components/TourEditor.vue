<template>
  <div class="space-y-8">
    <LocaleSwitcher v-model="currentLocale" />

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Localized Information</h3>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Tour Title ({{ currentLocale.toUpperCase() }})</label>
          <input v-model="form.names[currentLocale]" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="Enter tour title" />
        </div>
        <div class="space-y-2 md:col-span-2">
          <label class="block text-sm font-medium text-gray-700">Description ({{ currentLocale.toUpperCase() }})</label>
          <textarea v-model="form.descriptions[currentLocale]" rows="4" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="Enter full tour description"></textarea>
        </div>
        <div class="space-y-2 md:col-span-2">
          <label class="block text-sm font-medium text-gray-700">Highlights ({{ currentLocale.toUpperCase() }}) - Comma separated</label>
          <input v-model="highlightsInput" @change="updateHighlights" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. Stunning views, Expert Guide, Free drinks" />
        </div>
      </div>
    </div>

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Pricing & Logistics</h3>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Base Price</label>
          <input v-model.number="form.price" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Currency</label>
          <select v-model="form.currency" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500">
            <option value="EUR">EUR</option>
            <option value="USD">USD</option>
            <option value="EGP">EGP</option>
          </select>
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Original Price</label>
          <input v-model.number="form.originalPrice" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Discount %</label>
          <input v-model.number="form.discountPercentage" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Duration</label>
          <input v-model="form.duration" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. 7 Days" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Start Time</label>
          <input v-model="form.startTime" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. 09:00 AM" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Rating (0-5)</label>
          <input v-model.number="form.rating" type="number" step="0.1" max="5" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Review Count</label>
          <input v-model.number="form.reviewCount" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
      </div>
    </div>

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Visuals</h3>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="space-y-2 col-span-1 md:col-span-2">
          <label class="block text-sm font-medium text-gray-700 mb-2">Cover Image</label>
          
          <div v-if="!form.imageUrl && !isUploadingCover" 
               class="border-2 border-dashed border-gray-300 rounded-xl p-8 text-center bg-gray-50 hover:bg-gray-100 transition-colors cursor-pointer" 
               @click="coverInput?.click()">
            <div class="flex flex-col items-center justify-center">
              <svg class="w-10 h-10 text-gray-400 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
              <p class="text-sm font-medium text-gray-900">Click to upload cover image</p>
              <p class="mt-1 text-xs text-gray-500">PNG, JPG up to 5MB</p>
            </div>
          </div>
          
          <div v-if="isUploadingCover" class="border-2 border-dashed border-indigo-300 rounded-xl p-8 flex flex-col items-center justify-center bg-indigo-50">
            <svg class="animate-spin h-8 w-8 text-indigo-600 mb-2" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            <span class="text-sm font-medium text-indigo-700">Uploading cover image...</span>
          </div>

          <div v-if="form.imageUrl && !isUploadingCover" class="relative group bg-white rounded-xl overflow-hidden border border-gray-200 shadow-sm inline-block">
            <div class="relative w-full max-w-md aspect-video">
              <img :src="form.imageUrl" alt="Tour Cover Image" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" />
              <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex flex-col items-center justify-center">
                <button @click.stop="form.imageUrl = null" type="button" class="px-4 py-2 bg-red-600/90 hover:bg-red-600 text-white font-medium rounded-lg backdrop-blur-sm transition-all transform hover:scale-105 shadow-lg flex items-center gap-2">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                  Remove Cover
                </button>
              </div>
            </div>
          </div>
          
          <input type="file" ref="coverInput" accept="image/*" @change="uploadCover" class="hidden" />
        </div>
        
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Background Gradient</label>
          <input v-model="form.bgGradient" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="linear-gradient(...)" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Emoji</label>
          <input v-model="form.emoji" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Promo Badge</label>
          <input v-model="form.badge" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
      </div>
    </div>

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Relationships</h3>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Destination</label>
          <select v-model="form.destinationId" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500">
            <option v-for="d in destinations" :key="d.id" :value="d.id">{{ d.names?.en }}</option>
          </select>
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Category</label>
          <select v-model="form.categoryId" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500">
            <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.names?.en }}</option>
          </select>
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Supplier</label>
          <select v-model="form.supplierId" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500">
            <option value="">No Supplier</option>
            <option v-for="s in suppliers" :key="s.id" :value="s.id">{{ s.nameEn || s.nameAr }}</option>
          </select>
        </div>
      </div>
    </div>

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Flags</h3>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
        <label class="flex items-center gap-2"><input type="checkbox" v-model="form.isTopRated" class="w-4 h-4 text-indigo-600 rounded" /> Top Rated</label>
        <label class="flex items-center gap-2"><input type="checkbox" v-model="form.isBestseller" class="w-4 h-4 text-indigo-600 rounded" /> Bestseller</label>
        <label class="flex items-center gap-2"><input type="checkbox" v-model="form.isInHighDemand" class="w-4 h-4 text-indigo-600 rounded" /> High Demand</label>
        <label class="flex items-center gap-2"><input type="checkbox" v-model="form.reserveAndPayLater" class="w-4 h-4 text-indigo-600 rounded" /> Reserve & Pay Later</label>
        <label class="flex items-center gap-2"><input type="checkbox" v-model="form.hotelPickup" class="w-4 h-4 text-indigo-600 rounded" /> Hotel Pickup</label>
        <label class="flex items-center gap-2"><input type="checkbox" v-model="form.freeCancellation" class="w-4 h-4 text-indigo-600 rounded" /> Free Cancellation</label>
        <label class="flex items-center gap-2"><input type="checkbox" v-model="form.isPrivateOption" class="w-4 h-4 text-indigo-600 rounded" /> Private Option Available</label>
      </div>
    </div>

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Pickup Configuration</h3>
      <div class="space-y-4">
        <div class="flex gap-4">
          <label class="flex items-center gap-2"><input type="radio" value="FixedSlots" v-model="form.pickupTimeType" class="w-4 h-4 text-indigo-600" /> Fixed Slots</label>
          <label class="flex items-center gap-2"><input type="radio" value="Flexible" v-model="form.pickupTimeType" class="w-4 h-4 text-indigo-600" /> Flexible</label>
          <label class="flex items-center gap-2"><input type="radio" value="DriverAssigned" v-model="form.pickupTimeType" class="w-4 h-4 text-indigo-600" /> Driver Assigned</label>
        </div>
        <div v-if="form.pickupTimeType === 'FixedSlots'" class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Available Pickup Times</label>
          <div class="flex gap-2 mb-2">
            <input v-model="newSlot" type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. 10:00 - 10:30 (Morning)" @keyup.enter="addSlot" />
            <button type="button" @click="addSlot" class="px-4 py-2 bg-indigo-600 text-white rounded-lg">Add</button>
          </div>
          <div class="flex flex-wrap gap-2">
            <span v-for="(slot, idx) in form.availablePickupTimes" :key="idx" class="px-3 py-1 bg-gray-100 rounded-full text-sm flex items-center gap-2">
              {{ slot }}
              <button type="button" @click="removeSlot(Number(idx))" class="text-red-500 hover:text-red-700">✕</button>
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, inject, onMounted, watch } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'
import api from '@/services/api'

const currentLocale = ref('en')
const form = inject<any>('tourForm')

const destinations = ref<any[]>([])
const categories = ref<any[]>([])
const suppliers = ref<any[]>([])
const highlightsInput = ref('')
const isUploadingCover = ref(false)
const coverInput = ref<HTMLInputElement | null>(null)

watch(currentLocale, () => {
  highlightsInput.value = form.value.highlights?.[currentLocale.value]?.join(', ') || ''
})

onMounted(async () => {
  if (!form.value.highlights) form.value.highlights = { en: [], de: [], it: [], fr: [], ru: [] }
  highlightsInput.value = form.value.highlights[currentLocale.value]?.join(', ') || ''
  
  try {
    const [d, c, s] = await Promise.all([
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories'),
      api.get('/api/content/api/suppliers')
    ])
    destinations.value = Array.isArray(d.data) ? d.data : d.data?.items || []
    categories.value = Array.isArray(c.data) ? c.data : c.data?.items || []
    suppliers.value = Array.isArray(s.data) ? s.data : s.data?.items || []
  } catch(e) {}
})

const updateHighlights = () => {
  if (!form.value.highlights) form.value.highlights = { en: [], de: [], it: [], fr: [], ru: [] }
  form.value.highlights[currentLocale.value] = highlightsInput.value.split(',').map(s => s.trim()).filter(Boolean)
}

const uploadCover = async (e: Event) => {
  const target = e.target as HTMLInputElement
  if (!target.files?.length) return
  const file = target.files[0]
  if (!file.type.startsWith('image/')) return

  const formData = new FormData()
  formData.append('file', file)
  
  isUploadingCover.value = true
  try {
    const res = await api.post('/api/files', formData)
    const fileId = res.data.fileId || res.data.FileId
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    form.value.imageUrl = `${API_URL}/api/files/${fileId}`
  } catch (err) {
    console.error(err)
  } finally {
    isUploadingCover.value = false
    if (target) target.value = ''
  }
}

const newSlot = ref('')
const addSlot = () => {
  if (newSlot.value.trim() && !form.value.availablePickupTimes.includes(newSlot.value.trim())) {
    form.value.availablePickupTimes.push(newSlot.value.trim())
    newSlot.value = ''
  }
}
const removeSlot = (idx: number) => {
  form.value.availablePickupTimes.splice(idx, 1)
}
</script>
