<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import {
  ArrowLeft,
  Calendar,
  Clock,
  User,
  Mail,
  Phone,
  Sparkles,
  Check,
  Plus,
  Trash2,
  Edit2,
  AlertTriangle,
  ChevronUp,
  ClipboardList
} from 'lucide-vue-next'
import { useToast } from '@/composables/useToast'
import api from '@/services/api'

interface Tour {
  id: string
  names: Record<string, string>
  price: number
  duration: string
  imageUrl: string
  currency?: string
  destination?: { names: Record<string, string> }
}

interface PackageTier {
  id: string
  name: string
  price: number
  tier: string
}

interface GuestItem {
  id: string
  fullName: string
  email?: string
  phone?: string
  passportNumber?: string
  passportFileName?: string
  ageCategory: 'Adult' | 'Child' | 'Infant'
  nationality?: string
  specialRequests?: string
}

const router = useRouter()
const toast = useToast()

// Loading & Catalogs
const loadingTours = ref(true)
const tours = ref<Tour[]>([])
const availablePackages = ref<PackageTier[]>([])
const isSubmitting = ref(false)

// Booking Main Form
const selectedTourId = ref('')
const selectedPackageId = ref<string | null>(null)
const tourDate = ref('')
const pickupTime = ref('08:30 AM')
const tripType = ref('GROUP')
const hotelPickup = ref(true)
const hotelName = ref('')
const roomNumber = ref('')
const language = ref('en')

// Booking Profile & Corporate/Group Classification
const bookingProfile = ref<'INDIVIDUAL' | 'CORPORATE' | 'GROUP'>('INDIVIDUAL')
const companyName = ref('')
const billingTaxId = ref('')
const groupName = ref('')

// Lead Customer
const customerName = ref('')
const customerEmail = ref('')
const whatsApp = ref('')

// Headcount & Guests State
const totalGuests = ref(1)
const activeGuestTab = ref(0)
const guests = ref<GuestItem[]>([
  {
    id: crypto.randomUUID(),
    fullName: '',
    email: '',
    phone: '',
    passportNumber: '',
    passportFileName: '',
    ageCategory: 'Adult',
    nationality: '',
    specialRequests: ''
  }
])

// For > 5 guests: inline sub-section accordion & edit state
const isAddSectionOpen = ref(false)
const editingGuestIndex = ref<number | null>(null)
const inlineGuestForm = ref<GuestItem>({
  id: '',
  fullName: '',
  email: '',
  phone: '',
  passportNumber: '',
  passportFileName: '',
  ageCategory: 'Adult',
  nationality: '',
  specialRequests: ''
})

// Bulk Paste Modal State
const isBulkModalOpen = ref(false)
const bulkText = ref('')

// Pre-defined time slots
const timeSlots = [
  '06:00 AM', '07:30 AM', '08:30 AM', '09:00 AM', '10:00 AM',
  '12:00 PM', '02:00 PM', '03:30 PM', '05:00 PM', '06:30 PM'
]

// Common Nationalities
const popularNationalities = [
  'Germany', 'United Kingdom', 'Italy', 'France', 'Russia', 
  'Poland', 'Czech Republic', 'United States', 'Switzerland', 'Austria', 'Egypt'
]

// Fetch Tours Catalog
async function fetchTours() {
  loadingTours.value = true
  try {
    const res = await api.get('/api/content/api/tours')
    tours.value = res.data?.items || res.data || []
  } catch (err) {
    console.error('Failed to load tours', err)
    toast.error('Catalog Error', 'Failed to load tour packages catalog.')
  } finally {
    loadingTours.value = false
  }
}

// Watch selected tour to load packages & reset pricing
watch(selectedTourId, async (newTourId) => {
  if (!newTourId) {
    availablePackages.value = []
    selectedPackageId.value = null
    return
  }

  try {
    const res = await api.get(`/api/content/api/tours/${newTourId}`)
    const tourData = res.data
    if (tourData?.packages && tourData.packages.length > 0) {
      availablePackages.value = tourData.packages
      selectedPackageId.value = tourData.packages[0].id
    } else {
      availablePackages.value = []
      selectedPackageId.value = null
    }
  } catch (e) {
    console.error('Failed to load tour package details', e)
    availablePackages.value = []
  }
})

// Synchronize guests array with totalGuests headcount (when <= 5 tabs mode)
watch(totalGuests, (newCount) => {
  const count = Math.max(1, newCount || 1)
  if (count <= 5) {
    while (guests.value.length < count) {
      guests.value.push({
        id: crypto.randomUUID(),
        fullName: '',
        email: '',
        phone: '',
        passportNumber: '',
        passportFileName: '',
        ageCategory: 'Adult',
        nationality: '',
        specialRequests: ''
      })
    }
    if (guests.value.length > count) {
      guests.value = guests.value.slice(0, count)
    }
    if (activeGuestTab.value >= count) {
      activeGuestTab.value = count - 1
    }
  }
})

// Keep Lead guest name synced with Customer Name
watch(customerName, (val) => {
  if (guests.value.length > 0 && !guests.value[0].fullName) {
    guests.value[0].fullName = val
  }
})
watch(customerEmail, (val) => {
  if (guests.value.length > 0 && !guests.value[0].email) {
    guests.value[0].email = val
  }
})
watch(whatsApp, (val) => {
  if (guests.value.length > 0 && !guests.value[0].phone) {
    guests.value[0].phone = val
  }
})

// Selected Tour Object
const currentTour = computed(() => {
  return tours.value.find(t => t.id === selectedTourId.value)
})

// Selected Package Tier Object
const currentPackage = computed(() => {
  if (!selectedPackageId.value) return null
  return availablePackages.value.find(p => p.id === selectedPackageId.value)
})

// Calculated Unit & Total Price
const unitPrice = computed(() => {
  if (currentPackage.value) return currentPackage.value.price
  if (currentTour.value) return currentTour.value.price
  return 0
})

const totalPrice = computed(() => {
  const count = Math.max(1, totalGuests.value || guests.value.length || 1)
  return unitPrice.value * count
})

// Missing Identification Flag Check
const hasMissingIdentification = computed(() => {
  if (guests.value.length === 0) return true
  return guests.value.some(g => !g.passportFileName && !g.passportNumber?.trim())
})

// Open Inline Sub-Section Form
function openAddGuestSection() {
  editingGuestIndex.value = null
  inlineGuestForm.value = {
    id: crypto.randomUUID(),
    fullName: '',
    email: '',
    phone: '',
    passportNumber: '',
    passportFileName: '',
    ageCategory: 'Adult',
    nationality: '',
    specialRequests: ''
  }
  isAddSectionOpen.value = true
}

function openEditGuestSection(index: number) {
  editingGuestIndex.value = index
  const target = guests.value[index]
  inlineGuestForm.value = { ...target }
  isAddSectionOpen.value = true
}

// Save from Sub-Section and automatically collapse/close
function saveInlineGuest() {
  if (!inlineGuestForm.value.fullName.trim()) {
    toast.error('Validation Error', 'Guest full name is required.')
    return
  }

  if (editingGuestIndex.value !== null && editingGuestIndex.value >= 0) {
    guests.value[editingGuestIndex.value] = { ...inlineGuestForm.value }
    toast.success('Guest Updated', `Updated details for ${inlineGuestForm.value.fullName}.`)
  } else {
    guests.value.push({ ...inlineGuestForm.value, id: crypto.randomUUID() })
    toast.success('Guest Added', `Added ${inlineGuestForm.value.fullName} to passenger manifest.`)
    if (guests.value.length > totalGuests.value) {
      totalGuests.value = guests.value.length
    }
  }

  // Automatically close sub section until user presses add again
  isAddSectionOpen.value = false
  editingGuestIndex.value = null
}

function removeGuest(index: number) {
  const removed = guests.value[index]
  guests.value.splice(index, 1)
  if (totalGuests.value > guests.value.length && totalGuests.value > 5) {
    totalGuests.value = Math.max(1, guests.value.length)
  }
  toast.info('Guest Removed', `Removed ${removed.fullName || 'guest'} from manifest.`)
}

// Bulk Paste Processing
function processBulkPaste() {
  if (!bulkText.value.trim()) {
    toast.error('Bulk Import', 'Please paste passenger list lines.')
    return
  }

  const lines = bulkText.value.split('\n').map(l => l.trim()).filter(l => l.length > 0)
  let countAdded = 0

  for (const line of lines) {
    // Format: Name, Nationality, PassportNumber
    const parts = line.split(/[,;\t|]+/).map(p => p.trim())
    const name = parts[0]
    if (name) {
      const nationality = parts.length > 1 ? parts[1] : ''
      const passport = parts.length > 2 ? parts[2] : ''

      guests.value.push({
        id: crypto.randomUUID(),
        fullName: name,
        nationality: nationality,
        passportNumber: passport,
        passportFileName: '',
        ageCategory: 'Adult',
        specialRequests: ''
      })
      countAdded++
    }
  }

  totalGuests.value = Math.max(totalGuests.value, guests.value.length)
  isBulkModalOpen.value = false
  bulkText.value = ''
  toast.success('Bulk Import Complete', `Added ${countAdded} guests from text manifest.`)
}

// Submit VIP Booking
async function submitBooking() {
  if (!selectedTourId.value) {
    toast.error('Validation Error', 'Please select a tour package.')
    return
  }
  if (!customerName.value.trim()) {
    toast.error('Validation Error', 'Lead customer full name is required.')
    return
  }
  if (!customerEmail.value.trim()) {
    toast.error('Validation Error', 'Lead customer email is required.')
    return
  }
  if (!tourDate.value) {
    toast.error('Validation Error', 'Please select a tour departure date.')
    return
  }

  // Ensure at least 1 guest in list
  if (guests.value.length === 0) {
    guests.value.push({
      id: crypto.randomUUID(),
      fullName: customerName.value.trim(),
      email: customerEmail.value.trim(),
      phone: whatsApp.value.trim(),
      passportNumber: '',
      passportFileName: '',
      ageCategory: 'Adult',
      nationality: '',
      specialRequests: ''
    })
  }

  isSubmitting.value = true

  const payload = {
    tourId: selectedTourId.value,
    packageId: selectedPackageId.value,
    customerName: customerName.value.trim(),
    customerEmail: customerEmail.value.trim(),
    whatsApp: whatsApp.value.trim() || null,
    hotelName: hotelName.value.trim() || null,
    roomNumber: roomNumber.value.trim() || null,
    pickupTime: pickupTime.value,
    tripType: tripType.value,
    tourDate: new Date(tourDate.value).toISOString(),
    guests: totalGuests.value,
    hotelPickup: hotelPickup.value,
    totalPrice: totalPrice.value,
    language: language.value,
    missingIdentification: hasMissingIdentification.value,
    guestsList: guests.value.map(g => ({
      fullName: g.fullName.trim() || customerName.value.trim(),
      email: g.email?.trim() || null,
      phone: g.phone?.trim() || null,
      passportNumber: g.passportNumber?.trim() || null,
      passportFileName: g.passportFileName?.trim() || null,
      ageCategory: g.ageCategory || 'Adult',
      nationality: g.nationality?.trim() || null,
      specialRequests: g.specialRequests?.trim() || null
    }))
  }

  try {
    const res = await api.post('/api/booking/api/bookings', payload)
    const bookingId = res.data

    toast.success('VIP Booking Created', `Booking #${bookingId.toString().substring(0, 8).toUpperCase()} has been confirmed!`)
    
    // Redirect to booking details or bookings list
    router.push({ name: 'booking-details', params: { id: bookingId } })
  } catch (err: any) {
    console.error('Failed to create VIP booking', err)
    toast.error('Booking Creation Failed', err.response?.data?.error || err.response?.data?.message || 'Server error while generating booking.')
  } finally {
    isSubmitting.value = false
  }
}

onMounted(() => {
  // Set default tour date to tomorrow
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  tourDate.value = tomorrow.toISOString().split('T')[0]
  fetchTours()
})
</script>

<template>
  <div class="min-h-screen bg-[#F8FAFC] pb-24 text-[#1E293B]">
    <!-- Top Luxury Header Bar -->
    <header class="sticky top-0 z-40 bg-navy-950 text-white border-b border-navy-900 shadow-lg backdrop-blur-md bg-opacity-95">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-20 flex items-center justify-between">
        <div class="flex items-center gap-4">
          <button
            @click="router.push({ name: 'bookings' })"
            class="p-2.5 rounded-xl bg-white/10 hover:bg-white/20 text-white transition-all cursor-pointer flex items-center gap-2 text-sm font-medium"
          >
            <ArrowLeft class="w-4 h-4" />
            <span class="hidden sm:inline">Back to Bookings</span>
          </button>

          <div class="h-6 w-px bg-white/20"></div>

          <div>
            <div class="flex items-center gap-2">
              <span class="text-secondary text-sm">✦</span>
              <h1 class="text-xl sm:text-2xl font-serif font-bold text-white tracking-wide">
                Issue VIP Tour Booking
              </h1>
            </div>
            <p class="text-xs text-white/70 hidden sm:block">Concierge booking reservation, customized passenger manifest & tier pricing</p>
          </div>
        </div>

        <div class="flex items-center gap-3">
          <button
            @click="submitBooking"
            :disabled="isSubmitting"
            class="inline-flex items-center gap-2 px-6 py-2.5 bg-gradient-to-r from-secondary to-secondary-dark text-navy-950 font-bold text-sm rounded-xl shadow-md hover:brightness-105 active:scale-[0.98] transition-all cursor-pointer disabled:opacity-50"
          >
            <div v-if="isSubmitting" class="w-4 h-4 border-2 border-navy-950 border-t-transparent rounded-full animate-spin"></div>
            <Sparkles v-else class="w-4 h-4" />
            <span>{{ isSubmitting ? 'Issuing Booking...' : 'Confirm VIP Booking' }}</span>
          </button>
        </div>
      </div>
    </header>

    <!-- Main Content Container -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-8">
      <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
        
        <!-- Left Column: Form Details (8 cols) -->
        <div class="lg:col-span-8 space-y-8">
          
          <!-- Card 1: Experience & Tier Selection -->
          <section class="bg-white rounded-3xl p-6 sm:p-8 border border-slate-200/80 shadow-sm space-y-6">
            <div class="flex items-center gap-3 pb-4 border-b border-slate-100">
              <div class="w-10 h-10 rounded-xl bg-navy-950 text-secondary flex items-center justify-center text-lg font-bold">
                1
              </div>
              <div>
                <h2 class="text-lg font-bold text-slate-900">Experience & Tour Package</h2>
                <p class="text-xs text-slate-500">Select catalog experience, package tier, and scheduled departure.</p>
              </div>
            </div>

            <!-- Tour Picker -->
            <div class="space-y-2">
              <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider">
                Select Tour Package <span class="text-rose-500">*</span>
              </label>
              <select
                v-model="selectedTourId"
                class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
              >
                <option value="" disabled>-- Select an experience --</option>
                <option v-for="t in tours" :key="t.id" :value="t.id">
                  {{ t.names?.en || 'Untitled Tour' }} ({{ t.duration }}) — €{{ t.price }}
                </option>
              </select>
            </div>

            <!-- Package Tier Selector (if available) -->
            <div v-if="availablePackages.length > 0" class="space-y-3">
              <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider">
                Select Package Tier
              </label>
              <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                <div
                  v-for="pkg in availablePackages"
                  :key="pkg.id"
                  @click="selectedPackageId = pkg.id"
                  :class="[
                    'p-4 rounded-2xl border-2 cursor-pointer transition-all flex flex-col justify-between',
                    selectedPackageId === pkg.id 
                      ? 'border-secondary bg-secondary/5 shadow-sm' 
                      : 'border-slate-200 bg-white hover:border-slate-300'
                  ]"
                >
                  <div class="flex justify-between items-start">
                    <span class="text-xs font-bold uppercase tracking-wider text-slate-900">{{ pkg.name }}</span>
                    <span v-if="selectedPackageId === pkg.id" class="w-5 h-5 rounded-full bg-secondary text-navy-950 flex items-center justify-center text-xs">✓</span>
                  </div>
                  <div class="mt-3 text-lg font-bold text-slate-900">
                    €{{ pkg.price }} <span class="text-xs font-normal text-slate-500">/ person</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Date, Time & Headcount Grid -->
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 pt-2">
              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2">
                  Tour Departure Date <span class="text-rose-500">*</span>
                </label>
                <div class="relative">
                  <Calendar class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
                  <input
                    v-model="tourDate"
                    type="date"
                    class="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                  />
                </div>
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2">
                  Pickup / Departure Time
                </label>
                <div class="relative">
                  <Clock class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
                  <select
                    v-model="pickupTime"
                    class="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                  >
                    <option v-for="time in timeSlots" :key="time" :value="time">{{ time }}</option>
                  </select>
                </div>
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2">
                  Total Guests (Headcount) <span class="text-rose-500">*</span>
                </label>
                <input
                  v-model.number="totalGuests"
                  type="number"
                  min="1"
                  max="200"
                  class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-bold text-slate-900 focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                />
              </div>
            </div>

            <!-- Transfer & Hotel Pickup -->
            <div class="border-t border-slate-100 pt-4 space-y-4">
              <div class="flex items-center justify-between">
                <div>
                  <div class="text-sm font-bold text-slate-800">VIP Hotel Transfer Included</div>
                  <div class="text-xs text-slate-500">Pick up guests directly from their hotel or resort.</div>
                </div>
                <label class="relative inline-flex items-center cursor-pointer">
                  <input type="checkbox" v-model="hotelPickup" class="sr-only peer" />
                  <div class="w-11 h-6 bg-slate-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-slate-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-secondary"></div>
                </label>
              </div>

              <div v-if="hotelPickup" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <input
                  v-model="hotelName"
                  type="text"
                  placeholder="Hotel / Resort Name (e.g. Oberoi Sahl Hasheesh)"
                  class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                />
                <input
                  v-model="roomNumber"
                  type="text"
                  placeholder="Room / Suite Number (Optional)"
                  class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                />
              </div>
            </div>
          </section>

          <!-- Card 2: Lead Customer Contact -->
          <section class="bg-white rounded-3xl p-6 sm:p-8 border border-slate-200/80 shadow-sm space-y-6">
            <div class="flex items-center gap-3 pb-4 border-b border-slate-100">
              <div class="w-10 h-10 rounded-xl bg-navy-950 text-secondary flex items-center justify-center text-lg font-bold">
                2
              </div>
              <div>
                <h2 class="text-lg font-bold text-slate-900">Lead Customer & Communication</h2>
                <p class="text-xs text-slate-500">Contact person receiving confirmation receipts and WhatsApp updates.</p>
              </div>
            </div>

            <!-- Booking Profile Selector (Individual VIP, Corporate Company, Group Event) -->
            <div class="space-y-2">
              <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider">
                Booking Classification
              </label>
              <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                <button
                  type="button"
                  @click="bookingProfile = 'INDIVIDUAL'"
                  :class="[
                    'p-3.5 rounded-2xl border-2 text-left transition-all flex items-center gap-3 cursor-pointer',
                    bookingProfile === 'INDIVIDUAL' 
                      ? 'border-secondary bg-secondary/5 text-navy-950 font-bold shadow-sm' 
                      : 'border-slate-200 bg-white text-slate-600 hover:border-slate-300'
                  ]"
                >
                  <span class="text-xl">👑</span>
                  <div>
                    <div class="text-xs font-bold">Private VIP / Family</div>
                    <div class="text-[10px] text-slate-500 font-normal">Personal luxury journey</div>
                  </div>
                </button>

                <button
                  type="button"
                  @click="bookingProfile = 'CORPORATE'"
                  :class="[
                    'p-3.5 rounded-2xl border-2 text-left transition-all flex items-center gap-3 cursor-pointer',
                    bookingProfile === 'CORPORATE' 
                      ? 'border-secondary bg-secondary/5 text-navy-950 font-bold shadow-sm' 
                      : 'border-slate-200 bg-white text-slate-600 hover:border-slate-300'
                  ]"
                >
                  <span class="text-xl">🏢</span>
                  <div>
                    <div class="text-xs font-bold">Corporate Company</div>
                    <div class="text-[10px] text-slate-500 font-normal">Company retreat / employees</div>
                  </div>
                </button>

                <button
                  type="button"
                  @click="bookingProfile = 'GROUP'"
                  :class="[
                    'p-3.5 rounded-2xl border-2 text-left transition-all flex items-center gap-3 cursor-pointer',
                    bookingProfile === 'GROUP' 
                      ? 'border-secondary bg-secondary/5 text-navy-950 font-bold shadow-sm' 
                      : 'border-slate-200 bg-white text-slate-600 hover:border-slate-300'
                  ]"
                >
                  <span class="text-xl">👥</span>
                  <div>
                    <div class="text-xs font-bold">Group / Delegation</div>
                    <div class="text-[10px] text-slate-500 font-normal">Friends, club or delegation</div>
                  </div>
                </button>
              </div>
            </div>

            <!-- Conditional Corporate / Group Inputs -->
            <div v-if="bookingProfile === 'CORPORATE'" class="grid grid-cols-1 sm:grid-cols-2 gap-4 bg-slate-50 p-4 rounded-2xl border border-slate-200/80">
              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1">
                  Company / Organization Name <span class="text-rose-500">*</span>
                </label>
                <input
                  v-model="companyName"
                  type="text"
                  placeholder="e.g. McKinsey & Company / Siemens AG"
                  class="w-full px-4 py-2 bg-white border border-slate-200 rounded-xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-secondary/40"
                />
              </div>
              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1">
                  Corporate Billing / Tax ID (VAT)
                </label>
                <input
                  v-model="billingTaxId"
                  type="text"
                  placeholder="e.g. DE123456789 (Optional)"
                  class="w-full px-4 py-2 bg-white border border-slate-200 rounded-xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-secondary/40"
                />
              </div>
            </div>

            <div v-if="bookingProfile === 'GROUP'" class="bg-slate-50 p-4 rounded-2xl border border-slate-200/80">
              <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1">
                Group / Event / Delegation Name <span class="text-rose-500">*</span>
              </label>
              <input
                v-model="groupName"
                type="text"
                placeholder="e.g. Zurich Yacht Club Red Sea Expedition 2026"
                class="w-full px-4 py-2 bg-white border border-slate-200 rounded-xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-secondary/40"
              />
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2">
                  Lead Customer Full Name <span class="text-rose-500">*</span>
                </label>
                <div class="relative">
                  <User class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
                  <input
                    v-model="customerName"
                    type="text"
                    placeholder="e.g. Lord Arthur Wellesley"
                    class="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                  />
                </div>
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2">
                  Email Address <span class="text-rose-500">*</span>
                </label>
                <div class="relative">
                  <Mail class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
                  <input
                    v-model="customerEmail"
                    type="email"
                    placeholder="vip.guest@example.com"
                    class="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                  />
                </div>
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2">
                  WhatsApp Number (with country code)
                </label>
                <div class="relative">
                  <Phone class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
                  <input
                    v-model="whatsApp"
                    type="text"
                    placeholder="+44 7700 900123"
                    class="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                  />
                </div>
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2">
                  Preferred Language
                </label>
                <select
                  v-model="language"
                  class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                >
                  <option value="en">English (EN)</option>
                  <option value="de">German (DE)</option>
                  <option value="it">Italian (IT)</option>
                  <option value="fr">French (FR)</option>
                  <option value="ru">Russian (RU)</option>
                </select>
              </div>
            </div>
          </section>

          <!-- Card 3: Guest Information & Identification (SMART TABS vs MANIFEST GRID) -->
          <section class="bg-white rounded-3xl p-6 sm:p-8 border border-slate-200/80 shadow-sm space-y-6">
            <div class="flex flex-col sm:flex-row justify-between sm:items-center gap-3 pb-4 border-b border-slate-100">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-navy-950 text-secondary flex items-center justify-center text-lg font-bold">
                  3
                </div>
                <div>
                  <h2 class="text-lg font-bold text-slate-900">Guest Manifest & Identification</h2>
                  <p class="text-xs text-slate-500">
                    {{ totalGuests <= 5 ? 'Tabbed individual guest profiles' : 'Group passenger manifest grid & bulk tools' }}
                  </p>
                </div>
              </div>

              <!-- Bulk Action Tool Button (When > 5 guests or anytime) -->
              <button
                type="button"
                @click="isBulkModalOpen = true"
                class="inline-flex items-center gap-1.5 px-3.5 py-1.5 bg-slate-100 hover:bg-slate-200 text-slate-800 rounded-xl text-xs font-bold transition-all cursor-pointer"
              >
                <ClipboardList class="w-4 h-4 text-secondary-dark" />
                <span>Bulk Paste Guests</span>
              </button>
            </div>

            <!-- MODE A: TOTAL GUESTS <= 5 (INTERACTIVE LUXURY TABS) -->
            <div v-if="totalGuests <= 5" class="space-y-6">
              <!-- Tabs Header (High-Contrast Luxury Tabs) -->
              <div class="flex items-center gap-2.5 border-b border-slate-200 pb-3 overflow-x-auto">
                <button
                  v-for="(g, idx) in guests"
                  :key="g.id"
                  type="button"
                  @click="activeGuestTab = idx"
                  :style="activeGuestTab === idx 
                    ? 'background: #0B1B3D !important; color: #FFFFFF !important; border-color: #D4AF37 !important; box-shadow: 0 4px 12px rgba(11, 27, 61, 0.25);' 
                    : 'background: #FFFFFF !important; color: #0F172A !important; border-color: #CBD5E1 !important;'"
                  class="px-4 py-2.5 rounded-xl font-bold text-xs tracking-wide transition-all flex items-center gap-2.5 cursor-pointer shrink-0 border-2"
                >
                  <span
                    :style="activeGuestTab === idx 
                      ? 'background: #D4AF37 !important; color: #0B1B3D !important;' 
                      : 'background: #E2E8F0 !important; color: #334155 !important;'"
                    class="w-5 h-5 rounded-full flex items-center justify-center text-[10px] font-bold shrink-0"
                  >
                    {{ idx + 1 }}
                  </span>
                  <span 
                    :style="activeGuestTab === idx ? 'color: #FFFFFF !important;' : 'color: #0F172A !important;'"
                    class="font-bold text-xs whitespace-nowrap"
                  >
                    {{ idx === 0 ? (g.fullName ? g.fullName : 'Guest 1 (Lead)') : (g.fullName ? g.fullName : `Guest ${idx + 1}`) }}
                  </span>
                  <span
                    v-if="!g.passportFileName && !g.passportNumber"
                    style="background: #FEF3C7 !important; color: #92400E !important; border: 1px solid #FCD34D !important;"
                    class="px-1.5 py-0.5 rounded text-[9px] font-bold whitespace-nowrap"
                    title="Missing identification"
                  >
                    ⚠️ No ID
                  </span>
                  <span
                    v-else
                    style="background: #ECFDF5 !important; color: #065F46 !important; border: 1px solid #6EE7B7 !important;"
                    class="px-1.5 py-0.5 rounded text-[9px] font-bold whitespace-nowrap"
                    title="ID on file"
                  >
                    🛡️ ID
                  </span>
                </button>
              </div>

              <!-- Active Tab Body -->
              <div v-if="guests[activeGuestTab]" class="space-y-4 bg-slate-50/70 p-5 rounded-2xl border border-slate-200/60">
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
                      Guest Full Name <span class="text-rose-500">*</span>
                    </label>
                    <input
                      v-model="guests[activeGuestTab].fullName"
                      type="text"
                      placeholder="Passenger full legal name"
                      class="w-full px-4 py-2.5 bg-white border border-slate-200 rounded-xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                    />
                  </div>

                  <div>
                    <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
                      Nationality / Country
                    </label>
                    <input
                      v-model="guests[activeGuestTab].nationality"
                      list="nationalities-list"
                      placeholder="e.g. Germany, UK, Italy"
                      class="w-full px-4 py-2.5 bg-white border border-slate-200 rounded-xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                    />
                  </div>

                  <div>
                    <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
                      Age Category
                    </label>
                    <select
                      v-model="guests[activeGuestTab].ageCategory"
                      class="w-full px-4 py-2.5 bg-white border border-slate-200 rounded-xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                    >
                      <option value="Adult">Adult (12+ years)</option>
                      <option value="Child">Child (2-11 years)</option>
                      <option value="Infant">Infant (Under 2 years)</option>
                    </select>
                  </div>

                  <div>
                    <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
                      Passport or National ID Number
                    </label>
                    <input
                      v-model="guests[activeGuestTab].passportNumber"
                      type="text"
                      placeholder="e.g. C12345678"
                      class="w-full px-4 py-2.5 bg-white border border-slate-200 rounded-xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                    />
                  </div>
                </div>

                <div>
                  <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
                    Special Dietary / Concierge Requests
                  </label>
                  <input
                    v-model="guests[activeGuestTab].specialRequests"
                    type="text"
                    placeholder="Vegetarian meal, anniversary celebration, wheelchair assistance..."
                    class="w-full px-4 py-2.5 bg-white border border-slate-200 rounded-xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
                  />
                </div>
              </div>
            </div>

            <!-- MODE B: TOTAL GUESTS > 5 (MANIFEST GRID WITH COLLAPSIBLE ADD SUB-SECTION) -->
            <div v-else class="space-y-4">
              <!-- Top Action Button Bar -->
              <div class="flex items-center justify-between bg-slate-50 p-4 rounded-2xl border border-slate-200">
                <div>
                  <span class="text-xs font-bold uppercase tracking-wider text-slate-700">Passenger Count:</span>
                  <span class="ml-2 font-bold text-slate-900">{{ guests.length }} of {{ totalGuests }} Entered</span>
                </div>
                <button
                  type="button"
                  @click="isAddSectionOpen ? (isAddSectionOpen = false) : openAddGuestSection()"
                  class="inline-flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-primary to-primary-light text-white font-semibold text-xs rounded-xl shadow-sm hover:shadow transition-all cursor-pointer"
                >
                  <Plus v-if="!isAddSectionOpen" class="w-4 h-4" />
                  <ChevronUp v-else class="w-4 h-4" />
                  <span>{{ isAddSectionOpen ? 'Collapse Sub-Section' : '+ Add Guest Details' }}</span>
                </button>
              </div>

              <!-- Animated Collapsible Sub-Section Input Form -->
              <Transition name="expand">
                <div v-if="isAddSectionOpen" class="bg-navy-950 text-white p-6 rounded-2xl shadow-md border border-navy-900 space-y-4">
                  <div class="flex items-center justify-between border-b border-white/10 pb-3">
                    <h3 class="text-sm font-bold text-white flex items-center gap-2">
                      <User class="w-4 h-4 text-secondary" />
                      <span>{{ editingGuestIndex !== null ? 'Edit Passenger Profile' : 'Input New Passenger' }}</span>
                    </h3>
                    <span class="text-xs text-white/60">Saves and appends to manifest below</span>
                  </div>

                  <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div>
                      <label class="block text-[11px] font-bold text-white/80 uppercase tracking-wider mb-1">
                        Full Legal Name <span class="text-rose-400">*</span>
                      </label>
                      <input
                        v-model="inlineGuestForm.fullName"
                        type="text"
                        placeholder="Passenger name"
                        class="w-full px-3.5 py-2 bg-white/10 border border-white/20 rounded-xl text-sm text-white placeholder-white/40 focus:outline-none focus:ring-2 focus:ring-secondary/50"
                      />
                    </div>

                    <div>
                      <label class="block text-[11px] font-bold text-white/80 uppercase tracking-wider mb-1">
                        Nationality
                      </label>
                      <input
                        v-model="inlineGuestForm.nationality"
                        list="nationalities-list"
                        placeholder="e.g. Germany"
                        class="w-full px-3.5 py-2 bg-white/10 border border-white/20 rounded-xl text-sm text-white placeholder-white/40 focus:outline-none focus:ring-2 focus:ring-secondary/50"
                      />
                    </div>

                    <div>
                      <label class="block text-[11px] font-bold text-white/80 uppercase tracking-wider mb-1">
                        Age Category
                      </label>
                      <select
                        v-model="inlineGuestForm.ageCategory"
                        class="w-full px-3.5 py-2 bg-slate-900 border border-white/20 rounded-xl text-sm text-white focus:outline-none focus:ring-2 focus:ring-secondary/50"
                      >
                        <option value="Adult">Adult (12+ years)</option>
                        <option value="Child">Child (2-11 years)</option>
                        <option value="Infant">Infant (Under 2 years)</option>
                      </select>
                    </div>

                    <div>
                      <label class="block text-[11px] font-bold text-white/80 uppercase tracking-wider mb-1">
                        Passport or National ID #
                      </label>
                      <input
                        v-model="inlineGuestForm.passportNumber"
                        type="text"
                        placeholder="Passport / ID number"
                        class="w-full px-3.5 py-2 bg-white/10 border border-white/20 rounded-xl text-sm text-white placeholder-white/40 focus:outline-none focus:ring-2 focus:ring-secondary/50"
                      />
                    </div>
                  </div>

                  <div class="flex items-center justify-end gap-3 pt-2">
                    <button
                      type="button"
                      @click="isAddSectionOpen = false"
                      class="px-4 py-2 text-xs font-medium text-white/70 hover:text-white"
                    >
                      Cancel
                    </button>
                    <button
                      type="button"
                      @click="saveInlineGuest"
                      class="px-6 py-2 bg-secondary text-navy-950 font-bold text-xs rounded-xl shadow hover:brightness-105 transition-all cursor-pointer"
                    >
                      {{ editingGuestIndex !== null ? 'Save Changes' : 'Save & Add to Manifest' }}
                    </button>
                  </div>
                </div>
              </Transition>

              <!-- Passenger Manifest Grid / Table -->
              <div class="overflow-x-auto rounded-2xl border border-slate-200">
                <table class="w-full text-left text-xs whitespace-nowrap">
                  <thead class="bg-slate-100 text-slate-700 font-bold uppercase tracking-wider">
                    <tr>
                      <th class="py-3 px-4 w-12">#</th>
                      <th class="py-3 px-4">Passenger Name</th>
                      <th class="py-3 px-4">Nationality</th>
                      <th class="py-3 px-4">Category</th>
                      <th class="py-3 px-4">Passport / ID #</th>
                      <th class="py-3 px-4">Identification Status</th>
                      <th class="py-3 px-4 text-right">Actions</th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-100 bg-white">
                    <tr v-for="(g, idx) in guests" :key="g.id" class="hover:bg-slate-50/80 transition-colors">
                      <td class="py-3 px-4 font-bold text-slate-500">{{ idx + 1 }}</td>
                      <td class="py-3 px-4 font-bold text-slate-900">
                        {{ g.fullName || 'Unnamed Passenger' }}
                        <span v-if="idx === 0" class="ml-1 text-[10px] font-semibold text-secondary-dark bg-secondary/15 px-2 py-0.5 rounded-full">Lead</span>
                      </td>
                      <td class="py-3 px-4 text-slate-600">{{ g.nationality || '—' }}</td>
                      <td class="py-3 px-4 text-slate-600">{{ g.ageCategory }}</td>
                      <td class="py-3 px-4 font-mono text-slate-700">{{ g.passportNumber || '—' }}</td>
                      <td class="py-3 px-4">
                        <span
                          v-if="g.passportNumber || g.passportFileName"
                          class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-[11px] font-bold bg-emerald-50 text-emerald-700 border border-emerald-200"
                        >
                          🛡️ ID On File
                        </span>
                        <span
                          v-else
                          class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-[11px] font-bold bg-amber-50 text-amber-700 border border-amber-200"
                        >
                          ⚠️ Missing ID
                        </span>
                      </td>
                      <td class="py-3 px-4 text-right">
                        <div class="flex items-center justify-end gap-2">
                          <button
                            type="button"
                            @click="openEditGuestSection(idx)"
                            class="p-1.5 rounded-lg text-slate-400 hover:text-primary hover:bg-slate-100 transition-colors"
                            title="Edit Guest"
                          >
                            <Edit2 class="w-3.5 h-3.5" />
                          </button>
                          <button
                            type="button"
                            @click="removeGuest(idx)"
                            class="p-1.5 rounded-lg text-slate-400 hover:text-rose-600 hover:bg-rose-50 transition-colors"
                            title="Remove Guest"
                          >
                            <Trash2 class="w-3.5 h-3.5" />
                          </button>
                        </div>
                      </td>
                    </tr>
                    <tr v-if="guests.length === 0">
                      <td colspan="7" class="py-6 text-center text-slate-400">
                        No passenger entries yet. Click "+ Add Guest Details" above.
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <!-- Missing ID Warning Alert Banner -->
            <div
              v-if="hasMissingIdentification"
              class="p-4 rounded-2xl bg-amber-50 border border-amber-200/80 flex items-start gap-3 text-amber-900 text-xs"
            >
              <AlertTriangle class="w-5 h-5 text-amber-600 shrink-0 mt-0.5" />
              <div>
                <strong class="font-bold">Missing Customer Identification:</strong>
                <p class="mt-0.5 text-amber-800 leading-relaxed">
                  One or more guests in this booking are missing passport or national ID records. This reservation will be flagged with a <span class="font-bold underline">⚠️ Missing Identification / Passports</span> badge for VIP concierge document collection.
                </p>
              </div>
            </div>
          </section>
        </div>

        <!-- Right Column: Live VIP Price Breakdown & Confirmation (4 cols) -->
        <div class="lg:col-span-4 space-y-6">
          
          <!-- Sticky Summary Card -->
          <div class="sticky top-28 bg-white rounded-3xl p-6 sm:p-8 border border-slate-200/80 shadow-md space-y-6">
            <div class="pb-4 border-b border-slate-100">
              <span class="text-[10px] font-bold tracking-widest uppercase text-secondary-dark">Seadora Concierge</span>
              <h3 class="text-lg font-serif font-bold text-slate-900">VIP Booking Summary</h3>
            </div>

            <!-- Selected Experience Snippet -->
            <div v-if="currentTour" class="flex gap-3 items-center bg-slate-50 p-3.5 rounded-2xl border border-slate-200/60">
              <img
                :src="currentTour.imageUrl || 'https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=400&q=80'"
                class="w-14 h-14 rounded-xl object-cover"
              />
              <div class="overflow-hidden">
                <div class="text-xs font-bold text-slate-900 truncate">{{ currentTour.names?.en || 'Tour' }}</div>
                <div class="text-[11px] text-slate-500 mt-0.5">{{ currentTour.duration }} • {{ tripType }}</div>
              </div>
            </div>

            <!-- Pricing Breakdown -->
            <div class="space-y-3 text-xs">
              <div class="flex justify-between text-slate-600">
                <span>Base Rate / Person</span>
                <span class="font-bold text-slate-900">€{{ unitPrice.toFixed(2) }}</span>
              </div>
              <div v-if="currentPackage" class="flex justify-between text-slate-600">
                <span>Selected Tier</span>
                <span class="font-bold text-secondary-dark">{{ currentPackage.name }}</span>
              </div>
              <div class="flex justify-between text-slate-600">
                <span>Total Passengers</span>
                <span class="font-bold text-slate-900">× {{ totalGuests }}</span>
              </div>
              <div class="flex justify-between text-slate-600">
                <span>Hotel Pickup</span>
                <span class="font-bold text-emerald-600">{{ hotelPickup ? 'Included (VIP)' : 'None' }}</span>
              </div>

              <div class="border-t border-slate-100 pt-3 flex justify-between items-baseline">
                <span class="text-sm font-bold text-slate-900">Total Amount</span>
                <span class="text-2xl font-serif font-bold text-navy-950">€{{ totalPrice.toFixed(2) }}</span>
              </div>
            </div>

            <!-- Primary Action Button -->
            <button
              type="button"
              @click="submitBooking"
              :disabled="isSubmitting"
              class="w-full py-3.5 px-6 bg-gradient-to-r from-primary to-primary-light hover:from-primary-light hover:to-primary text-white font-bold text-sm rounded-xl shadow-md hover:shadow-lg transition-all active:scale-[0.98] cursor-pointer flex items-center justify-center gap-2 disabled:opacity-50"
            >
              <div v-if="isSubmitting" class="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
              <Check v-else class="w-5 h-5" />
              <span>{{ isSubmitting ? 'Confirming...' : 'Issue VIP Booking' }}</span>
            </button>
          </div>
        </div>
      </div>
    </main>

    <!-- Bulk Paste Modal -->
    <Teleport to="body">
      <div v-if="isBulkModalOpen" class="fixed inset-0 z-[9999] flex items-center justify-center p-4">
        <div class="fixed inset-0 bg-navy-950/60 backdrop-blur-sm" @click="isBulkModalOpen = false"></div>
        <div class="relative w-full max-w-lg bg-white rounded-3xl shadow-2xl p-6 sm:p-8 space-y-4 border border-slate-100">
          <div class="flex justify-between items-center border-b pb-3">
            <h3 class="text-lg font-bold text-slate-900">📋 Bulk Paste Passenger Manifest</h3>
            <button @click="isBulkModalOpen = false" class="text-slate-400 hover:text-slate-600">✕</button>
          </div>
          <p class="text-xs text-slate-500">
            Paste one guest per line. Formats supported:<br/>
            <code>Full Name, Nationality, Passport Number</code> or simply <code>Full Name</code>
          </p>
          <textarea
            v-model="bulkText"
            rows="7"
            placeholder="John Doe, Germany, C12345678&#10;Jane Doe, Germany, C87654321&#10;Arthur Smith, UK"
            class="w-full p-3.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono focus:bg-white focus:outline-none focus:ring-2 focus:ring-secondary/40"
          ></textarea>
          <div class="flex justify-end gap-3 pt-2">
            <button @click="isBulkModalOpen = false" class="px-4 py-2 text-xs font-medium text-slate-600">Cancel</button>
            <button @click="processBulkPaste" class="px-6 py-2.5 bg-primary text-white text-xs font-bold rounded-xl shadow">
              Import Passengers
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Nationalities Datalist -->
    <datalist id="nationalities-list">
      <option v-for="nat in popularNationalities" :key="nat" :value="nat" />
    </datalist>
  </div>
</template>

<style scoped>
.expand-enter-active,
.expand-leave-active {
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
  max-height: 500px;
  overflow: hidden;
}
.expand-enter-from,
.expand-leave-to {
  max-height: 0;
  opacity: 0;
  transform: translateY(-8px);
}
</style>
