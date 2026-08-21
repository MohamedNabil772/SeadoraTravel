<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/services/api'
import { useToast } from '@/composables/useToast'

interface Destination {
  id: string
  names: Record<string, string>
  flag: string
}

interface Category {
  id: string
  names: Record<string, string>
  icon: string
}

interface Supplier {
  id: string
  nameEn?: string
  nameAr: string
}

const route = useRoute()
const router = useRouter()
const toast = useToast()

const isEdit = ref(false)
const loading = ref(true)
const saveLoading = ref(false)
const uploadLoading = ref(false)
const coverLoading = ref(false)
const activeLang = ref('en')

const gradientStart = ref('#063a5c')
const gradientEnd = ref('#1a9b8a')

const form = ref({
  id: '',
  names: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  descriptions: { en: '', de: '', it: '', fr: '', ru: '' } as Record<string, string>,
  price: 25 as number | string,
  currency: 'EUR',
  duration: 'fullDay',
  includesInput: '',
  mediaUrls: [] as string[],
  imageUrl: 'https://images.unsplash.com/photo-1544551763-46a013bb70d5',
  emoji: '⛵',
  bgGradient: 'linear-gradient(135deg,#063a5c,#1a9b8a)',
  badge: '',
  destinationId: '',
  categoryId: '',
  supplierId: '',
  supplierPercentage: 15,
  maxAllocations: 20,
  pickupTimeType: 'FixedSlots',
  availablePickupTimes: ['15:00 - 15:30 (Sunset)', '10:00 - 10:30 (Morning)'] as string[],
  newSlotInput: ''
})

const destinations = ref<Destination[]>([])
const categories = ref<Category[]>([])
const suppliers = ref<Supplier[]>([])

const durations = [
  { value: 'fullDay', label: 'Full Day' },
  { value: 'halfDay', label: 'Half Day' },
  { value: 'twoDays', label: '2 Days' },
  { value: 'fiveDays', label: '5 Days' },
  { value: 'oneDay', label: '1 Day' },
  { value: 'threeHours', label: '3 Hours' },
  { value: 'evening', label: 'Evening' }
]

const languages = [
  { code: 'en', label: 'English' },
  { code: 'de', label: 'German' },
  { code: 'it', label: 'Italian' },
  { code: 'fr', label: 'French' },
  { code: 'ru', label: 'Russian' }
]

async function loadData() {
  loading.value = true
  try {
    const results = await Promise.allSettled([
      api.get('/api/content/api/destinations'),
      api.get('/api/content/api/categories'),
      api.get('/api/content/api/suppliers')
    ])
    
    if (results[0].status === 'fulfilled') {
      const data = results[0].value.data
      destinations.value = Array.isArray(data) ? data : (data?.items || [])
    }
    if (results[1].status === 'fulfilled') {
      const data = results[1].value.data
      categories.value = Array.isArray(data) ? data : (data?.items || [])
    }
    if (results[2].status === 'fulfilled') {
      const data = results[2].value.data
      suppliers.value = Array.isArray(data) ? data : (data?.items || [])
    }

    const tourId = route.params.id as string
    if (tourId && tourId !== 'create') {
      isEdit.value = true
      const tourRes = await api.get(`/api/content/api/tours/${tourId}`)
      const tour = tourRes.data
      form.value = {
        id: tour.id,
        names: { ...tour.names },
        descriptions: { ...tour.descriptions },
        price: tour.price,
        currency: tour.currency || 'EUR',
        duration: tour.duration,
        includesInput: tour.includes ? tour.includes.join(', ') : '',
        mediaUrls: tour.mediaUrls || [],
        imageUrl: tour.imageUrl,
        emoji: tour.emoji,
        bgGradient: tour.bgGradient,
        badge: tour.badge,
        destinationId: tour.destinationId,
        categoryId: tour.categoryId,
        supplierId: tour.supplierId || '',
        supplierPercentage: tour.supplierPercentage || 0,
        maxAllocations: tour.maxAllocations || 20,
        pickupTimeType: tour.pickupTimeType || 'FixedSlots',
        availablePickupTimes: tour.availablePickupTimes && tour.availablePickupTimes.length > 0 
          ? [...tour.availablePickupTimes] 
          : ['15:00 - 15:30 (Sunset)', '10:00 - 10:30 (Morning)'],
        newSlotInput: ''
      }
      parseGradient(tour.bgGradient || '')
    } else {
      isEdit.value = false
      form.value.destinationId = destinations.value[0]?.id || ''
      form.value.categoryId = categories.value[0]?.id || ''
      form.value.supplierId = suppliers.value[0]?.id || ''
    }
  } catch (e) {
    console.error('Failed to load data for tour edit view', e)
  } finally {
    loading.value = false
  }
}

function addPickupSlot() {
  const val = form.value.newSlotInput.trim()
  if (val && !form.value.availablePickupTimes.includes(val)) {
    form.value.availablePickupTimes.push(val)
    form.value.newSlotInput = ''
  }
}

function removePickupSlot(index: number) {
  form.value.availablePickupTimes.splice(index, 1)
}

async function handleMediaUpload(e: Event) {
  const target = e.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  uploadLoading.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    for (let i = 0; i < target.files.length; i++) {
      const file = target.files[i]
      const formData = new FormData()
      formData.append('file', file)

      const uploadRes = await api.post('/api/files', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      const fileId = uploadRes.data.fileId
      const fileUrl = `${API_URL}/api/files/${fileId}`
      form.value.mediaUrls.push(fileUrl)
    }
  } catch (e) {
    console.error('File upload failed', e)
    toast.error('Failed to upload some media files.')
  } finally {
    uploadLoading.value = false
    target.value = ''
  }
}

function removeMedia(index: number) {
  form.value.mediaUrls.splice(index, 1)
}

function isVideo(url: string): boolean {
  const extension = url.split('.').pop()?.toLowerCase() || ''
  const videoExtensions = ['mp4', 'mov', 'avi', 'mkv', 'webm', 'ogg']
  return videoExtensions.includes(extension) || url.toLowerCase().includes('video') || url.toLowerCase().includes('.mp4')
}

function parseGradient(grad: string) {
  const matches = grad.match(/#(?:[0-9a-fA-F]{3,4}){1,2}\b/g)
  if (matches && matches.length >= 2) {
    gradientStart.value = matches[0]
    gradientEnd.value = matches[1]
  } else {
    gradientStart.value = '#063a5c'
    gradientEnd.value = '#1a9b8a'
  }
}

function updateGradient() {
  form.value.bgGradient = `linear-gradient(135deg, ${gradientStart.value}, ${gradientEnd.value})`
}

function handlePriceInput(e: Event) {
  const target = e.target as HTMLInputElement
  let val = target.value.replace(/[^0-9.]/g, '')
  const parts = val.split('.')
  if (parts.length > 2) {
    val = parts[0] + '.' + parts.slice(1).join('')
  }
  target.value = val
  form.value.price = val
}

async function handleCoverImageUpload(e: Event) {
  const target = e.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  coverLoading.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const file = target.files[0]
    const formData = new FormData()
    formData.append('file', file)

    const uploadRes = await api.post('/api/files', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    const fileId = uploadRes.data.fileId
    form.value.imageUrl = `${API_URL}/api/files/${fileId}`
  } catch (e) {
    console.error('Cover upload failed', e)
    toast.error('Failed to upload cover image.')
  } finally {
    coverLoading.value = false
    target.value = ''
  }
}

async function saveTour() {
  saveLoading.value = true
  try {
    const payload = {
      names: form.value.names,
      descriptions: form.value.descriptions,
      price: typeof form.value.price === 'string' ? (parseFloat(form.value.price) || 0) : form.value.price,
      currency: form.value.currency || 'EUR',
      duration: form.value.duration,
      includes: form.value.includesInput.split(',').map(s => s.trim()).filter(Boolean),
      mediaUrls: form.value.mediaUrls,
      imageUrl: form.value.imageUrl,
      emoji: form.value.emoji,
      bgGradient: form.value.bgGradient,
      badge: form.value.badge,
      destinationId: form.value.destinationId,
      categoryId: form.value.categoryId,
      supplierId: form.value.supplierId ? form.value.supplierId : null,
      supplierPercentage: form.value.supplierPercentage || 0,
      maxAllocations: form.value.maxAllocations || 20,
      pickupTimeType: form.value.pickupTimeType || 'FixedSlots',
      availablePickupTimes: form.value.pickupTimeType === 'FixedSlots' ? form.value.availablePickupTimes : []
    }

    if (isEdit.value) {
      await api.put(`/api/content/api/tours/${form.value.id}`, { id: form.value.id, ...payload })
    } else {
      await api.post('/api/content/api/tours', payload)
    }

    router.push('/tours')
  } catch (e) {
    console.error('Failed to save tour', e)
    toast.error('Failed to save tour. See console for details.')
  } finally {
    saveLoading.value = false
  }
}

function cancel() {
  router.push('/tours')
}

onMounted(loadData)
</script>

<template>
  <div class="edit-view-container">
    <!-- Breadcrumbs / Header -->
    <div class="flex justify-between items-center mb-6">
      <div>
        <h2 class="text-2xl font-bold text-dark font-serif">
          {{ isEdit ? 'Edit Tour Details' : 'Create Tour' }}
        </h2>
        <p class="text-sm text-body">
          Configure excursion characteristics, pricing, suppliers, and media gallery.
        </p>
      </div>
      <button @click="cancel" class="btn-cancel">
        &larr; Back to List
      </button>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="flex justify-center items-center py-20">
      <div class="spinner"></div>
    </div>

    <!-- Form layout -->
    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Left Forms Column -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Localized details card -->
        <div class="card p-6">
          <h3 class="card-title mb-4">Tour Localized Content</h3>
          
          <!-- Tabs for languages -->
          <div class="tabs-header mb-4 flex border-b border-stroke gap-2">
            <button 
              v-for="lang in languages" 
              :key="lang.code"
              type="button"
              @click="activeLang = lang.code"
              class="tab-btn"
              :class="{ 'active': activeLang === lang.code }"
            >
              {{ lang.label }}
            </button>
          </div>

          <!-- Content fields for active language -->
          <div class="space-y-4">
            <div class="form-group">
              <label class="block text-xs font-semibold text-dark mb-1">Tour Title ({{ activeLang.toUpperCase() }})</label>
              <input 
                v-model="form.names[activeLang]" 
                type="text" 
                :placeholder="`Enter Tour Title in ${languages.find(l => l.code === activeLang)?.label}`" 
                required 
              />
            </div>
            <div class="form-group">
              <label class="block text-xs font-semibold text-dark mb-1">Detailed Description ({{ activeLang.toUpperCase() }})</label>
              <textarea 
                v-model="form.descriptions[activeLang]" 
                rows="6" 
                :placeholder="`Enter Tour Description in ${languages.find(l => l.code === activeLang)?.label}`" 
                required
              ></textarea>
            </div>
          </div>
        </div>

        <!-- General Info card -->
        <div class="card p-6 space-y-4">
          <h3 class="card-title">Base Pricing & Excursion Info</h3>
          
          <div class="space-y-4">
            <div class="form-group">
              <label class="block text-xs font-semibold text-dark mb-1">Base Price & Currency</label>
              <div class="flex gap-2">
                <input 
                  :value="form.price" 
                  type="text" 
                  required 
                  class="!py-3 !px-4 text-base font-semibold text-dark" 
                  style="flex: 1; min-width: 0;"
                  placeholder="0.00"
                  @input="handlePriceInput" 
                />
                <select 
                  v-model="form.currency" 
                  required 
                  class="!py-3 !px-4 font-semibold" 
                  style="width: 130px; min-width: 130px; flex-shrink: 0;"
                >
                  <option value="EUR">€ EUR</option>
                  <option value="USD">$ USD</option>
                  <option value="EGP">EGP</option>
                </select>
              </div>
            </div>

            <div class="form-group">
              <label class="block text-xs font-semibold text-dark mb-1">Duration Cycle</label>
              <select v-model="form.duration" required class="!py-3 !px-4">
                <option v-for="d in durations" :key="d.value" :value="d.value">
                  {{ d.label }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label class="block text-xs font-semibold text-dark mb-1">Maximum Allocations (Excursion Capacity)</label>
              <input 
                v-model.number="form.maxAllocations" 
                type="number" 
                min="1" 
                required 
              />
            </div>
          </div>

          <div class="form-group">
            <label class="block text-xs font-semibold text-dark mb-1">Includes (Comma separated list)</label>
            <input 
              v-model="form.includesInput" 
              type="text" 
              placeholder="e.g. 🚌 Transfer, 🥗 Lunch, 🤿 Equipment, 🧭 Guide" 
            />
          </div>

          <!-- Pickup & Departure Timing Configuration -->
          <div class="pt-4 mt-4 border-t border-stroke space-y-3">
            <label class="block text-xs font-bold text-dark uppercase tracking-wider">Pickup Timing Configuration</label>
            
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
              <label class="flex items-center gap-2 p-3 rounded-lg border border-stroke cursor-pointer hover:bg-[#F8FAFC]" :class="{ 'border-primary bg-[#F0FDF4] font-bold': form.pickupTimeType === 'FixedSlots' }">
                <input type="radio" value="FixedSlots" v-model="form.pickupTimeType" class="text-primary" />
                <span class="text-xs">Fixed Time Slots</span>
              </label>

              <label class="flex items-center gap-2 p-3 rounded-lg border border-stroke cursor-pointer hover:bg-[#F8FAFC]" :class="{ 'border-primary bg-[#F0FDF4] font-bold': form.pickupTimeType === 'Flexible' }">
                <input type="radio" value="Flexible" v-model="form.pickupTimeType" class="text-primary" />
                <span class="text-xs">Flexible / Free Time</span>
              </label>

              <label class="flex items-center gap-2 p-3 rounded-lg border border-stroke cursor-pointer hover:bg-[#F8FAFC]" :class="{ 'border-primary bg-[#F0FDF4] font-bold': form.pickupTimeType === 'DriverAssigned' }">
                <input type="radio" value="DriverAssigned" v-model="form.pickupTimeType" class="text-primary" />
                <span class="text-xs">Assigned by Concierge</span>
              </label>
            </div>

            <!-- Fixed Slots Editor -->
            <div v-if="form.pickupTimeType === 'FixedSlots'" class="p-4 bg-[#F8FAFC] rounded-lg border border-stroke space-y-3">
              <label class="block text-xs font-semibold text-dark">Available Departure / Pickup Slots</label>
              
              <div class="flex flex-wrap gap-2">
                <span 
                  v-for="(slot, idx) in form.availablePickupTimes" 
                  :key="idx" 
                  class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-white border border-stroke text-xs font-medium text-dark shadow-xs"
                >
                  ⏱️ {{ slot }}
                  <button type="button" @click="removePickupSlot(idx)" class="text-danger hover:opacity-80 font-bold ml-1">✕</button>
                </span>
              </div>

              <div class="flex gap-2">
                <input 
                  type="text" 
                  v-model="form.newSlotInput" 
                  placeholder="e.g. 15:00 - 15:30 (Sunset) or 08:30 (Morning)" 
                  class="flex-1 !py-2 !text-xs"
                  @keyup.enter.prevent="addPickupSlot"
                />
                <button 
                  type="button" 
                  @click="addPickupSlot" 
                  class="px-4 py-2 bg-primary text-white text-xs font-bold rounded-lg hover:bg-opacity-90"
                >
                  + Add Slot
                </button>
              </div>
            </div>

            <div v-else-if="form.pickupTimeType === 'Flexible'" class="p-3 bg-[#F0F9FF] rounded-lg text-xs text-[#0369A1]">
              ℹ️ Customers can select any custom departure time when booking this tour.
            </div>

            <div v-else class="p-3 bg-[#FEF3C7] rounded-lg text-xs text-[#92400E]">
              ℹ️ Pickup timing is dynamically calculated and communicated by the driver based on the guest's hotel location.
            </div>
          </div>
        </div>

        <!-- Media Upload Gallery Card -->
        <div class="card p-6">
          <div class="flex justify-between items-center mb-4">
            <div>
              <h3 class="card-title">Tour Details Media Gallery</h3>
              <p class="text-xs text-body">Upload high resolution photos and promotional videos for this tour.</p>
            </div>
            <label class="btn-upload">
              <span>Upload Photos/Videos</span>
              <input 
                type="file" 
                multiple 
                accept="image/*,video/*" 
                @change="handleMediaUpload" 
                class="hidden" 
                :disabled="uploadLoading"
              />
            </label>
          </div>

          <!-- Uploading loader -->
          <div v-if="uploadLoading" class="flex items-center gap-2 text-xs text-primary mb-4">
            <div class="spinner-small"></div>
            <span>Uploading selected media to file server...</span>
          </div>

          <!-- Media grid -->
          <div v-if="form.mediaUrls.length > 0" class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
            <div 
              v-for="(url, index) in form.mediaUrls" 
              :key="url" 
              class="media-card group relative border border-stroke rounded-lg overflow-hidden bg-[#F1F5F9] aspect-square flex items-center justify-center"
            >
              <!-- Video player or Image -->
              <video 
                v-if="isVideo(url)" 
                :src="url" 
                class="w-full h-full object-cover" 
                muted
              ></video>
              <img 
                v-else 
                :src="url" 
                class="w-full h-full object-cover" 
                alt="Tour Gallery Image"
              />

              <!-- Hover overlay with delete action -->
              <div class="absolute inset-0 bg-dark/70 opacity-0 group-hover:opacity-100 flex items-center justify-center transition-all duration-200">
                <button 
                  type="button" 
                  @click="removeMedia(index)" 
                  class="bg-red-600 hover:bg-red-700 text-white rounded-full p-2 text-xs font-bold transition-all"
                  title="Remove Media"
                >
                  ✕ Remove
                </button>
              </div>

              <!-- Media type indicator -->
              <span class="absolute bottom-2 left-2 bg-dark/80 text-[10px] text-white px-2 py-0.5 rounded font-semibold font-sans">
                {{ isVideo(url) ? '📽️ VIDEO' : '🖼️ PHOTO' }}
              </span>
            </div>
          </div>

          <!-- Empty state -->
          <div v-else class="border border-dashed border-stroke rounded-xl py-12 text-center text-body text-sm bg-[#F8FAFC]">
            <span>No detailed photo or video attachments uploaded yet.</span>
          </div>
        </div>
      </div>

      <!-- Right Metadata Column -->
      <div class="space-y-6">
        <!-- Relationship card -->
        <div class="card p-6 space-y-4">
          <h3 class="card-title">Categorization & Destination</h3>
          
          <div class="form-group">
            <label class="block text-xs font-semibold text-dark mb-1">Destination Location</label>
            <select v-model="form.destinationId" required>
              <option v-for="d in destinations" :key="d.id" :value="d.id">
                {{ d.flag }} {{ d.names?.en || 'Unknown' }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label class="block text-xs font-semibold text-dark mb-1">Tour Category</label>
            <select v-model="form.categoryId" required>
              <option v-for="c in categories" :key="c.id" :value="c.id">
                {{ c.icon }} {{ c.names?.en || 'Unknown' }}
              </option>
            </select>
          </div>
        </div>

        <!-- Supplier Details card -->
        <div class="card p-6 space-y-4">
          <h3 class="card-title">Supplier Partnership</h3>

          <div class="form-group">
            <label class="block text-xs font-semibold text-dark mb-1">Supplier Operator</label>
            <select v-model="form.supplierId">
              <option value="">No Supplier (Direct)</option>
              <option v-for="s in suppliers" :key="s.id" :value="s.id">
                {{ s.nameEn || s.nameAr }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label class="block text-xs font-semibold text-dark mb-1">Supplier Earnings share (%)</label>
            <input 
              v-model.number="form.supplierPercentage" 
              type="number" 
              min="0" 
              max="100" 
              required 
            />
          </div>
        </div>

        <!-- Design configuration card -->
        <div class="card p-6 space-y-4">
          <h3 class="card-title">Design Configuration</h3>
          
          <div class="form-group">
            <label class="block text-xs font-semibold text-dark mb-1">Main Cover Image</label>
            <div class="flex flex-col gap-2">
              <label class="btn-upload text-center w-full">
                <span>{{ form.imageUrl ? 'Change Cover Photo' : 'Upload Cover Photo' }}</span>
                <input 
                  type="file" 
                  accept="image/*" 
                  @change="handleCoverImageUpload" 
                  class="hidden" 
                  :disabled="coverLoading"
                />
              </label>
              <div v-if="coverLoading" class="flex items-center gap-2 text-xs text-primary">
                <div class="spinner-small"></div>
                <span>Uploading cover image...</span>
              </div>
              <img 
                v-if="form.imageUrl" 
                :src="form.imageUrl" 
                class="w-full h-32 object-cover rounded-lg border border-stroke" 
                alt="Cover Image Preview"
              />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div class="form-group">
              <label class="block text-xs font-semibold text-dark mb-1">Card Emoji</label>
              <input v-model="form.emoji" type="text" placeholder="e.g. ⛵" required />
            </div>

            <div class="form-group">
              <label class="block text-xs font-semibold text-dark mb-1">Promo Badge</label>
              <input v-model="form.badge" type="text" placeholder="e.g. BEST SELLER" />
            </div>
          </div>

          <div class="form-group">
            <label class="block text-xs font-semibold text-dark mb-1">Main Card Color Gradient</label>
            <div class="grid grid-cols-2 gap-2 mb-2">
              <div>
                <span class="text-[10px] text-body block mb-1">Start Color</span>
                <input type="color" v-model="gradientStart" @input="updateGradient" class="w-full h-10 p-0 border border-stroke rounded cursor-pointer bg-white" />
              </div>
              <div>
                <span class="text-[10px] text-body block mb-1">End Color</span>
                <input type="color" v-model="gradientEnd" @input="updateGradient" class="w-full h-10 p-0 border border-stroke rounded cursor-pointer bg-white" />
              </div>
            </div>
            <div 
              class="w-full h-10 rounded-lg border border-stroke flex items-center justify-center text-xs font-bold text-white shadow-inner" 
              :style="{ background: form.bgGradient }"
            >
              Gradient Preview
            </div>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex gap-4">
          <button 
            type="button" 
            @click="saveTour" 
            class="flex-1 btn-primary-action py-3 font-bold"
            :disabled="saveLoading"
          >
            {{ saveLoading ? 'Saving Tour...' : 'Save Excursion' }}
          </button>
          <button 
            type="button" 
            @click="cancel" 
            class="w-24 btn-secondary-action py-3"
            :disabled="saveLoading"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.edit-view-container {
  max-width: 1200px;
  margin: 0 auto;
}
.card {
  background: #ffffff;
  border: 1px solid var(--stroke, #E2E8F0);
  border-radius: 8px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);
}
.card-title {
  font-size: 16px;
  font-weight: 700;
  color: var(--dark, #1c2434);
  margin-bottom: 12px;
}
.form-group label {
  color: var(--dark, #1c2434);
  margin-bottom: 6px;
}
.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 10px 14px;
  border: 1px solid var(--stroke, #E2E8F0);
  border-radius: 6px;
  font-size: 14px;
  color: var(--dark, #1C2434);
  background: #ffffff;
  outline: none;
  transition: all 0.2s;
}
.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  border-color: var(--primary, #3c50e0);
  box-shadow: 0 0 0 1px var(--primary, #3c50e0);
}
.tab-btn {
  padding: 10px 16px;
  font-size: 13px;
  font-weight: 600;
  color: var(--body, #64748b);
  border-bottom: 2px solid transparent;
  background: transparent;
  cursor: pointer;
  transition: all 0.2s;
}
.tab-btn:hover {
  color: var(--primary, #3c50e0);
}
.tab-btn.active {
  color: var(--primary, #3c50e0);
  border-bottom-color: var(--primary, #3c50e0);
}
.btn-upload {
  background: var(--primary, #3c50e0);
  color: #ffffff;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  display: inline-block;
  will-change: transform;
}
.btn-upload:hover {
  background: #2b3bb3;
  transform: translateY(-1px);
}
.btn-upload:active {
  transform: scale(0.97);
}
.btn-cancel {
  background: #ffffff;
  color: var(--body, #64748b);
  border: 1px solid var(--stroke, #e2e8f0);
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  will-change: transform;
}
.btn-cancel:hover {
  background: #f8fafc;
  color: var(--dark, #1c2434);
  transform: translateY(-1px);
}
.btn-cancel:active {
  transform: scale(0.97);
}
.btn-primary-action {
  background: var(--primary, #3c50e0);
  color: #ffffff;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  will-change: transform;
}
.btn-primary-action:hover {
  background: #2b3bb3;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(60, 80, 224, 0.2);
}
.btn-primary-action:active {
  transform: scale(0.97);
}
.btn-secondary-action {
  background: #f1f5f9;
  color: var(--dark, #1c2434);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  will-change: transform;
}
.btn-secondary-action:hover {
  background: #e2e8f0;
  transform: translateY(-1px);
}
.btn-secondary-action:active {
  transform: scale(0.97);
}
.spinner-small {
  border: 2px solid rgba(60, 80, 224, 0.1);
  width: 14px;
  height: 14px;
  border-radius: 50%;
  border-left-color: #3c50e0;
  animation: spin 1s linear infinite;
}
.spinner {
  border: 3px solid rgba(60, 80, 224, 0.1);
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border-left-color: #3c50e0;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
