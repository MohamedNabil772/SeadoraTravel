<template>
  <div class="space-y-8">
    <LocaleSwitcher v-model="currentLocale" />

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Localized Information</h3>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="space-y-2">
          <label for="tour-editor-name" class="block text-sm font-medium text-gray-700">Tour Title ({{ currentLocale.toUpperCase() }})</label>
          <input id="tour-editor-name" v-model="form.names[currentLocale]" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="Enter tour title" />
        </div>
        <div class="space-y-2 md:col-span-2">
          <label for="tour-editor-description" class="block text-sm font-medium text-gray-700">Description ({{ currentLocale.toUpperCase() }})</label>
          <textarea id="tour-editor-description" v-model="form.descriptions[currentLocale]" rows="4" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="Enter full tour description"></textarea>
        </div>
        <div class="space-y-2 md:col-span-2">
          <label for="tour-editor-highlights" class="block text-sm font-medium text-gray-700">Highlights ({{ currentLocale.toUpperCase() }}) - Comma separated</label>
          <input id="tour-editor-highlights" v-model="highlightsInput" @change="updateHighlights" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. Stunning views, Expert Guide, Free drinks" />
        </div>
      </div>
    </div>

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Pricing & Logistics</h3>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="space-y-2">
          <label for="tour-editor-price" class="block text-sm font-medium text-gray-700">Base Price</label>
          <input id="tour-editor-price" v-model.number="form.price" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-currency" class="block text-sm font-medium text-gray-700">Currency</label>
          <select id="tour-editor-currency" v-model="form.currency" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500">
            <option value="EUR">EUR</option>
            <option value="USD">USD</option>
            <option value="EGP">EGP</option>
          </select>
        </div>
        <div class="space-y-2">
          <label for="tour-editor-original-price" class="block text-sm font-medium text-gray-700">Original Price</label>
          <input id="tour-editor-original-price" v-model.number="form.originalPrice" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-discount" class="block text-sm font-medium text-gray-700">Discount %</label>
          <input id="tour-editor-discount" v-model.number="form.discountPercentage" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-duration" class="block text-sm font-medium text-gray-700">Duration</label>
          <input id="tour-editor-duration" v-model="form.duration" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. 7 Days" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-start-time" class="block text-sm font-medium text-gray-700">Start Time</label>
          <input id="tour-editor-start-time" v-model="form.startTime" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. 09:00 AM" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-max-allocations" class="block text-sm font-medium text-gray-700">Capacity / Max Allocations</label>
          <input id="tour-editor-max-allocations" v-model.number="form.maxAllocations" type="number" min="1" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 font-bold text-gray-900" placeholder="e.g. 20 Guests" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-rating" class="block text-sm font-medium text-gray-700">Rating (0-5)</label>
          <input id="tour-editor-rating" v-model.number="form.rating" type="number" step="0.1" max="5" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-review-count" class="block text-sm font-medium text-gray-700">Review Count</label>
          <input id="tour-editor-review-count" v-model.number="form.reviewCount" type="number" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
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
               role="button"
               tabindex="0"
               aria-label="Upload cover image"
               @click="coverInput?.click()"
               @keydown.enter.prevent="coverInput?.click()"
               @keydown.space.prevent="coverInput?.click()">
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
          <label for="tour-editor-bg-gradient" class="block text-sm font-medium text-gray-700">Background Gradient</label>
          <input id="tour-editor-bg-gradient" v-model="form.bgGradient" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="linear-gradient(...)" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-emoji" class="block text-sm font-medium text-gray-700">Emoji</label>
          <input id="tour-editor-emoji" v-model="form.emoji" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
        <div class="space-y-2">
          <label for="tour-editor-badge" class="block text-sm font-medium text-gray-700">Promo Badge</label>
          <input id="tour-editor-badge" v-model="form.badge" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
        </div>
      </div>
    </div>

    <!-- Tour Classification & Trip Type -->
    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <div class="flex items-center justify-between border-b pb-3">
        <div>
          <h3 class="text-lg font-bold text-gray-900">Trip Format & Classification</h3>
          <p class="text-xs text-gray-500 mt-0.5">Specify whether this is a group excursion, private tour, yacht charter, or VIP concierge journey.</p>
        </div>
      </div>

      <!-- Tour Type Visual Selection Cards -->
      <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3">
        <button
          v-for="tt in tourTypes"
          :key="tt.id"
          type="button"
          @click="form.tourTypeId = tt.id"
          :aria-pressed="form.tourTypeId === tt.id"
          class="cursor-pointer p-3.5 rounded-xl border transition-all duration-200 flex flex-col items-center text-center gap-2 group relative"
          :class="form.tourTypeId === tt.id 
            ? 'bg-secondary/10 border-secondary ring-2 ring-secondary/30 shadow-sm' 
            : 'bg-gray-50/60 border-gray-200/80 hover:bg-white hover:border-gray-300 hover:shadow-sm'"
        >
          <div class="w-10 h-10 rounded-xl bg-white border border-gray-200/70 shadow-sm flex items-center justify-center text-xl transition-transform group-hover:scale-110">
            {{ tt.icon || '⛵' }}
          </div>
          <div>
            <div class="text-xs font-bold text-gray-900 leading-snug">{{ tt.names?.en || tt.code }}</div>
            <div class="text-[10px] font-mono text-gray-400 mt-0.5">{{ tt.code }}</div>
          </div>
          <div v-if="form.tourTypeId === tt.id" class="absolute top-2 right-2 w-2 h-2 rounded-full bg-secondary"></div>
        </button>
      </div>

      <!-- Capacity & Booking Allocation Controls -->
      <div class="p-5 rounded-2xl bg-gradient-to-r from-slate-50 to-secondary/5 border border-slate-200/80 space-y-4">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-2 text-xs font-bold text-navy-950 uppercase tracking-wider">
            <span>👥</span>
            <span>Capacity & Allocation Limits</span>
          </div>
          <span class="text-[11px] text-slate-500 font-medium hidden sm:inline">Controls capacity constraints and overbooking thresholds</span>
        </div>
        <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div class="space-y-1.5">
            <label for="tour-editor-group-min" class="block text-xs font-bold text-gray-700">Minimum Group / Party Size</label>
            <input 
              id="tour-editor-group-min"
              v-model.number="form.groupMinCapacity" 
              type="number" 
              min="1" 
              class="w-full px-3.5 py-2 text-sm bg-white border border-gray-300 rounded-lg focus:ring-2 focus:ring-secondary/40 focus:border-secondary font-medium" 
              placeholder="e.g. 1 Guest" 
            />
            <p class="text-[11px] text-gray-400">Min guests required to operate departure</p>
          </div>
          <div class="space-y-1.5">
            <label for="tour-editor-group-max" class="block text-xs font-bold text-gray-700">Maximum Group / Party Size</label>
            <input 
              id="tour-editor-group-max"
              v-model.number="form.groupMaxCapacity" 
              type="number" 
              min="1" 
              class="w-full px-3.5 py-2 text-sm bg-white border border-gray-300 rounded-lg focus:ring-2 focus:ring-secondary/40 focus:border-secondary font-medium" 
              placeholder="e.g. 20 Guests" 
            />
            <p class="text-[11px] text-gray-400">Total physical capacity per departure</p>
          </div>
          <div class="space-y-1.5">
            <label for="tour-editor-max-alloc" class="block text-xs font-bold text-gray-700">Max Booking Allocations</label>
            <input 
              id="tour-editor-max-alloc"
              v-model.number="form.maxAllocations" 
              type="number" 
              min="1" 
              class="w-full px-3.5 py-2 text-sm bg-white border border-gray-300 rounded-lg focus:ring-2 focus:ring-secondary/40 focus:border-secondary font-bold text-gray-900" 
              placeholder="e.g. 20 Slots" 
            />
            <p class="text-[11px] text-gray-400">Threshold before "All Places Booked"</p>
          </div>
        </div>
      </div>
    </div>

    <div class="bg-white p-6 rounded-xl border border-gray-200 space-y-6">
      <h3 class="text-lg font-bold text-gray-900 border-b pb-2">Relationships</h3>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="space-y-2">
          <label for="tour-editor-destination" class="block text-sm font-medium text-gray-700">Destination</label>
          <select id="tour-editor-destination" v-model="form.destinationId" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500">
            <option v-for="d in destinations" :key="d.id" :value="d.id">{{ d.names?.en }}</option>
          </select>
        </div>
        <div class="space-y-2">
          <label for="tour-editor-category" class="block text-sm font-medium text-gray-700">Category</label>
          <select id="tour-editor-category" v-model="form.categoryId" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500">
            <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.names?.en }}</option>
          </select>
        </div>
        <div class="space-y-2">
          <label for="tour-editor-supplier" class="block text-sm font-medium text-gray-700">Supplier</label>
          <select id="tour-editor-supplier" v-model="form.supplierId" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500">
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
          <label for="tour-editor-new-slot" class="block text-sm font-medium text-gray-700">Available Pickup Times</label>
          <div class="flex gap-2 mb-2">
            <input id="tour-editor-new-slot" v-model="newSlot" type="text" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" placeholder="e.g. 10:00 - 10:30 (Morning)" @keyup.enter="addSlot" />
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

const tourTypes = ref<any[]>([])
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
    const [d, c, s, tt] = await Promise.all([
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories'),
      api.get('/api/content/api/suppliers'),
      api.get('/api/content/api/tour-types')
    ])
    destinations.value = Array.isArray(d.data) ? d.data : d.data?.items || []
    categories.value = Array.isArray(c.data) ? c.data : c.data?.items || []
    suppliers.value = Array.isArray(s.data) ? s.data : s.data?.items || []
    tourTypes.value = Array.isArray(tt.data) ? tt.data : []
    
    // Set default tour type if none selected
    if (!form.value.tourTypeId && tourTypes.value.length > 0) {
      form.value.tourTypeId = tourTypes.value[0].id
    }
  } catch(e) {
    // Fallback default tour types if offline or loading
    tourTypes.value = [
      { id: '11111111-1111-1111-1111-111111111111', code: 'GROUP', icon: '⛵', names: { en: 'Group Tour' } },
      { id: '22222222-2222-2222-2222-222222222222', code: 'PRIVATE', icon: '👑', names: { en: 'Private Tour' } },
      { id: '33333333-3333-3333-3333-333333333333', code: 'VIP', icon: '✨', names: { en: 'VIP Luxury' } },
      { id: '44444444-4444-4444-4444-444444444444', code: 'YACHT', icon: '🛥️', names: { en: 'Yacht Charter' } },
      { id: '55555555-5555-5555-5555-555555555555', code: 'SHORE_EXCURSION', icon: '⚓', names: { en: 'Shore Excursion' } },
      { id: '66666666-6666-6666-6666-666666666666', code: 'MULTI_DAY', icon: '🏔️', names: { en: 'Multi-Day' } }
    ]
    if (!form.value.tourTypeId && tourTypes.value.length > 0) {
      form.value.tourTypeId = tourTypes.value[0].id
    }
  }
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
