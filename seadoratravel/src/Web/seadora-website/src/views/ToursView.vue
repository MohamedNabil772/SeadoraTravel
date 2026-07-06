<script setup lang="ts">
import { ref, onMounted, computed, watch, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import Navbar from '../components/Navbar.vue'
import Footer from '../components/Footer.vue'

const { locale } = useI18n()
const router = useRouter()

interface Tour {
  id: string
  categoryId: string
  destinationId: string
  price: number
  names: Record<string, string>
  descriptions: Record<string, string>
  duration: string
  emoji?: string
  bgGradient?: string
}

interface Category {
  id: string
  names: Record<string, string>
  icon?: string
}

interface Destination {
  id: string
  names: Record<string, string>
}

const tours = ref<Tour[]>([])
const categories = ref<Category[]>([])
const destinations = ref<Destination[]>([])
const loading = ref(true)

// Filter states
const searchQuery = ref('')
const selectedDestinationId = ref('')
const selectedCategoryId = ref('')
const maxPrice = ref(500)

// Pagination states
const currentPage = ref(1)
const itemsPerPage = 6

// Booking Modal states
const showBookingModal = ref(false)
const bookingLoading = ref(false)
const bookingSuccess = ref(false)
const bookingReference = ref('')
const selectedTourForBooking = ref<Tour | null>(null)

const bookingForm = ref({
  name: '',
  email: '',
  phone: '',
  destination: '',
  date: '',
  guests: '2',
  notes: '',
  packageOption: 'premium',
  guideLanguage: 'en',
  pickupRequired: 'no'
})

const bookingErrors = ref({
  name: '',
  email: '',
  phone: '',
  destination: '',
  date: '',
  guests: ''
})

onMounted(async () => {
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    
    // Fetch Categories
    const catRes = await fetch(`${API_URL}/api/content/api/categories`)
    if (catRes.ok) {
      categories.value = await catRes.json()
    }

    // Fetch Tours
    const tourRes = await fetch(`${API_URL}/api/content/api/tours`)
    if (tourRes.ok) {
      tours.value = await tourRes.json()
    }

    // Fetch Destinations
    const destRes = await fetch(`${API_URL}/api/content/api/destinations`)
    if (destRes.ok) {
      destinations.value = await destRes.json()
    }
  } catch (error) {
    console.error("Failed to load content in ToursView:", error)
  } finally {
    loading.value = false
  }
})

// Helpers
const getSlug = (name: string) => {
  return name
    .toLowerCase()
    .replace(/[^\w\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
    .trim()
}

const getLocalized = (dict: Record<string, string>, fallback: string) => {
  if (!dict) return fallback
  return dict[locale.value] || dict['en'] || fallback
}

const openDetailsPage = (tour: Tour) => {
  const slug = getSlug(tour.names?.['en'] || 'tour')
  router.push(`/tour/${slug}`)
}

// Map Destination GUID or static ID to select values
const mapDestinationToValue = (destId: string) => {
  const dest = destinations.value.find(d => d.id === destId)
  if (!dest) return ''
  const name = (dest.names?.['en'] || '').toLowerCase()
  if (name.includes('hurghada')) return 'hurghada'
  if (name.includes('cairo')) return 'cairo'
  if (name.includes('luxor')) return 'luxor'
  if (name.includes('sharm')) return 'sharm'
  return ''
}

// Booking Modal trigger
const openBookingModal = (tour: Tour) => {
  selectedTourForBooking.value = tour
  const destVal = mapDestinationToValue(tour.destinationId)
  
  bookingForm.value = {
    name: '',
    email: '',
    phone: '',
    destination: destVal,
    date: '',
    guests: '2',
    notes: '',
    packageOption: 'premium',
    guideLanguage: locale.value || 'en',
    pickupRequired: 'no'
  }
  
  bookingErrors.value = {
    name: '',
    email: '',
    phone: '',
    destination: '',
    date: '',
    guests: ''
  }
  
  bookingSuccess.value = false
  showBookingModal.value = true
}

// Client validation
const validateField = (field: string) => {
  const form = bookingForm.value
  if (field === 'name') {
    if (!form.name) {
      bookingErrors.value.name = 'Full name is required.'
    } else if (form.name.trim().length < 3) {
      bookingErrors.value.name = 'Full name must be at least 3 characters.'
    } else if (!/^[A-Za-z\s]+$/.test(form.name.trim())) {
      bookingErrors.value.name = 'Full name must contain only letters and spaces.'
    } else {
      bookingErrors.value.name = ''
    }
  }
  
  if (field === 'email') {
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!form.email) {
      bookingErrors.value.email = 'Email address is required.'
    } else if (!emailPattern.test(form.email)) {
      bookingErrors.value.email = 'Please enter a valid email address.'
    } else {
      bookingErrors.value.email = ''
    }
  }
  
  if (field === 'phone') {
    const phonePattern = /^\+?[0-9\s\-()]{7,20}$/
    if (!form.phone) {
      bookingErrors.value.phone = 'Phone number is required.'
    } else if (!phonePattern.test(form.phone)) {
      bookingErrors.value.phone = 'Please enter a valid phone number (e.g. +1 555-0199).'
    } else {
      bookingErrors.value.phone = ''
    }
  }
  
  if (field === 'destination') {
    if (!form.destination) {
      bookingErrors.value.destination = 'Destination is required.'
    } else {
      bookingErrors.value.destination = ''
    }
  }
  
  if (field === 'date') {
    if (!form.date) {
      bookingErrors.value.date = 'Target date is required.'
    } else {
      const selectedDate = new Date(form.date)
      selectedDate.setHours(0, 0, 0, 0)
      const today = new Date()
      today.setHours(0, 0, 0, 0)
      if (selectedDate < today) {
        bookingErrors.value.date = 'Target date must be today or in the future.'
      } else {
        bookingErrors.value.date = ''
      }
    }
  }
  
  if (field === 'guests') {
    if (!form.guests) {
      bookingErrors.value.guests = 'Number of guests is required.'
    } else {
      bookingErrors.value.guests = ''
    }
  }
}

const validateForm = () => {
  validateField('name')
  validateField('email')
  validateField('phone')
  validateField('destination')
  validateField('date')
  validateField('guests')
  
  return !bookingErrors.value.name &&
         !bookingErrors.value.email &&
         !bookingErrors.value.phone &&
         !bookingErrors.value.destination &&
         !bookingErrors.value.date &&
         !bookingErrors.value.guests
}

const generateReferenceCode = () => {
  const randomNum = Math.floor(1000 + Math.random() * 9000)
  return `SEADORA-${randomNum}-EG`
}

const submitBooking = async () => {
  if (!selectedTourForBooking.value) return
  
  if (!validateForm()) {
    return
  }
  
  bookingLoading.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const response = await fetch(`${API_URL}/api/booking/api/bookings`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        tourId: selectedTourForBooking.value.id,
        customerName: bookingForm.value.name.trim(),
        customerEmail: bookingForm.value.email.trim()
      })
    })
    
    if (response.ok) {
      bookingReference.value = generateReferenceCode()
      bookingSuccess.value = true
    } else {
      const errText = await response.text()
      alert("Booking failed. " + (errText || "Please check details and try again."))
    }
  } catch (error) {
    console.error("Booking error:", error)
    alert("Connection error. Please try again.")
  } finally {
    bookingLoading.value = false
  }
}

// Watchers for reset pagination on filter change
watch([searchQuery, selectedDestinationId, selectedCategoryId, maxPrice], () => {
  currentPage.value = 1
})

// Filtered list computed
const filteredTours = computed(() => {
  return tours.value.filter(tour => {
    // Search query matching
    if (searchQuery.value) {
      const q = searchQuery.value.toLowerCase()
      const name = getLocalized(tour.names, '').toLowerCase()
      const desc = getLocalized(tour.descriptions, '').toLowerCase()
      if (!name.includes(q) && !desc.includes(q)) return false
    }

    // Destination matching
    if (selectedDestinationId.value && tour.destinationId !== selectedDestinationId.value) {
      return false
    }

    // Category matching
    if (selectedCategoryId.value && tour.categoryId !== selectedCategoryId.value) {
      return false
    }

    // Price matching
    if (tour.price > maxPrice.value) {
      return false
    }

    return true
  })
})

// Paginated tours list
const paginatedTours = computed(() => {
  const startIndex = (currentPage.value - 1) * itemsPerPage
  return filteredTours.value.slice(startIndex, startIndex + itemsPerPage)
})

// Total page count
const totalPages = computed(() => {
  return Math.ceil(filteredTours.value.length / itemsPerPage)
})

// Lightbox Keyboard actions
const handleKeydown = (e: KeyboardEvent) => {
  if (e.key === 'Escape' && showBookingModal.value) {
    showBookingModal.value = false
  }
}

onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown)
})
</script>

<template>
  <div class="min-h-screen bg-cream text-dark flex flex-col font-sans relative overflow-hidden">
    <!-- Navbar -->
    <Navbar />

    <!-- Luxury Portal Header -->
    <header class="relative bg-sea-deep text-white py-16 px-6 text-center overflow-hidden border-b border-gold/30">
      <!-- Background overlay -->
      <div class="absolute inset-0 bg-[radial-gradient(ellipse_at_top,_var(--sea)_0%,_transparent_80%)] opacity-35"></div>
      <div class="absolute inset-0 opacity-5 mix-blend-overlay bg-repeat bg-center" style="background-image: radial-gradient(#c9a84c 1px, transparent 1px); background-size: 20px 20px;"></div>
      
      <div class="relative z-10 max-w-4xl mx-auto mt-4">
        <span class="inline-block px-3.5 py-1 text-[10px] font-bold tracking-[0.25em] uppercase text-gold border border-gold/30 rounded-full mb-4 bg-gold/5">
          Elite Egypt Excursions
        </span>
        <h1 class="font-playfair text-4xl md:text-5xl font-bold tracking-tight mb-4 text-white">
          Discover The Cradle of Civilization
        </h1>
        <p class="font-cormorant italic text-lg text-[#8eafc2] max-w-2xl mx-auto leading-relaxed">
          Filter and select customized luxury experiences across the ancient wonders and beautiful Red Sea coasts of Egypt.
        </p>
      </div>
    </header>

    <!-- Main Booking Dashboard -->
    <main class="flex-1 max-w-7xl w-full mx-auto px-6 py-12 relative z-10">
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-8">
        
        <!-- SIDEBAR FILTERS (Column 1) -->
        <aside class="lg:col-span-1 bg-white/70 backdrop-blur-md border border-gold/20 p-6 rounded-2xl shadow-[0_15px_40px_rgba(201,168,76,0.05)] h-fit">
          <h3 class="font-playfair text-lg font-bold text-sea-deep mb-5 pb-3 border-b border-gold/15 flex items-center justify-between">
            <span>Filter Experiences</span>
            <span class="text-xs text-gold uppercase tracking-widest font-semibold font-sans">Options</span>
          </h3>

          <div class="space-y-6">
            <!-- Filter Search -->
            <div class="flex flex-col">
              <label for="search" class="text-[10px] font-bold tracking-wider text-muted uppercase mb-1.5 font-sans">Search Excursions</label>
              <div class="relative">
                <input 
                  id="search"
                  type="text" 
                  v-model="searchQuery" 
                  placeholder="e.g. Pyramids, Orange Bay..."
                  class="w-full pl-9 pr-4 py-2.5 bg-cream/35 border border-gold/30 rounded-lg text-xs font-sans outline-none focus:border-sea focus:ring-1 focus:ring-sea transition-all"
                />
                <span class="absolute left-3 top-3 text-muted/65 text-xs">🔍</span>
              </div>
            </div>

            <!-- Filter Destination -->
            <div class="flex flex-col">
              <label for="filter-dest" class="text-[10px] font-bold tracking-wider text-muted uppercase mb-1.5 font-sans">Region / Destination</label>
              <select 
                id="filter-dest" 
                v-model="selectedDestinationId"
                class="w-full p-2.5 bg-cream/35 border border-gold/30 rounded-lg text-xs font-sans outline-none focus:border-sea focus:ring-1 focus:ring-sea transition-all cursor-pointer"
              >
                <option value="">All Regions</option>
                <option v-for="dest in destinations" :key="dest.id" :value="dest.id">
                  {{ getLocalized(dest.names, 'Destination') }}
                </option>
              </select>
            </div>

            <!-- Filter Category -->
            <div class="flex flex-col">
              <label for="filter-cat" class="text-[10px] font-bold tracking-wider text-muted uppercase mb-1.5 font-sans">Category</label>
              <select 
                id="filter-cat" 
                v-model="selectedCategoryId"
                class="w-full p-2.5 bg-cream/35 border border-gold/30 rounded-lg text-xs font-sans outline-none focus:border-sea focus:ring-1 focus:ring-sea transition-all cursor-pointer"
              >
                <option value="">All Categories</option>
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                  {{ getLocalized(cat.names, 'Category') }}
                </option>
              </select>
            </div>

            <!-- Filter Price (Cost) -->
            <div class="flex flex-col">
              <div class="flex justify-between items-center mb-1.5">
                <label for="filter-price" class="text-[10px] font-bold tracking-wider text-muted uppercase font-sans">Maximum Price</label>
                <span class="text-xs font-bold text-gold font-mono">€{{ maxPrice }}</span>
              </div>
              <input 
                id="filter-price"
                type="range" 
                v-model.number="maxPrice" 
                min="30" 
                max="500" 
                step="10"
                class="w-full accent-gold cursor-pointer"
              />
              <div class="flex justify-between text-[9px] text-muted/65 mt-1 font-mono">
                <span>€30</span>
                <span>€500</span>
              </div>
            </div>
            
            <!-- Clear Filters Button -->
            <button 
              @click="searchQuery = ''; selectedDestinationId = ''; selectedCategoryId = ''; maxPrice = 500"
              class="w-full bg-cream hover:bg-gold/10 border border-gold/30 text-gold py-2 px-4 rounded-lg text-xs tracking-wider uppercase font-semibold transition-all cursor-pointer"
            >
              Reset Filters
            </button>
          </div>
        </aside>

        <!-- TOURS CARDS GRID (Column 2-4) -->
        <section class="lg:col-span-3">
          <div v-if="loading" class="flex flex-col items-center justify-center py-24 text-center">
            <svg class="animate-spin h-8 w-8 text-gold mb-3" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            <p class="text-sm text-muted uppercase tracking-widest font-semibold font-jost">Retrieving Luxury Excursions...</p>
          </div>

          <div v-else>
            <!-- Results count & header info -->
            <div class="flex justify-between items-center mb-6 font-jost text-sm text-muted">
              <div>Found <span class="font-bold text-sea-deep font-mono">{{ filteredTours.length }}</span> luxury packages</div>
              <div>Page <span class="font-mono text-sea-deep">{{ currentPage }}</span> of {{ totalPages || 1 }}</div>
            </div>

            <!-- Empty State -->
            <div v-if="filteredTours.length === 0" class="bg-white/50 border border-gold/15 rounded-2xl py-20 text-center px-6">
              <span class="text-4xl block mb-4">🏜️</span>
              <h3 class="font-playfair text-xl font-bold text-sea-deep mb-2">No Matching Packages</h3>
              <p class="text-muted text-xs max-w-xs mx-auto leading-relaxed">
                Adjust your filters or query to find your perfect luxury vacation package.
              </p>
            </div>

            <!-- Tour Cards grid list -->
            <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div 
                v-for="trip in paginatedTours" 
                :key="trip.id"
                class="bg-white border border-gold/15 rounded-2xl overflow-hidden shadow-md hover:shadow-xl hover:translate-y-[-4px] transition-all duration-300 flex flex-col justify-between"
              >
                <!-- Card Cover Image (Gradient-themed) -->
                <div 
                  class="h-48 relative flex items-center justify-center text-white overflow-hidden"
                  :style="{ background: trip.bgGradient || (categories.find(c => c.id === trip.categoryId)?.names['en'] === 'Sea & Diving' ? 'linear-gradient(135deg,#063a5c,#1a9b8a)' : categories.find(c => c.id === trip.categoryId)?.names['en'] === 'Culture & History' ? 'linear-gradient(135deg,#8b6914,#c9a84c)' : 'linear-gradient(135deg,#7c4a14,#e8820a)') }"
                >
                  <div class="absolute inset-0 bg-black/10"></div>
                  <!-- Category Icon -->
                  <div class="text-5xl relative z-10 transition-transform duration-500 hover:scale-110">
                    {{ trip.emoji || categories.find(c => c.id === trip.categoryId)?.icon || '🌍' }}
                  </div>
                  
                  <!-- Duration Badge -->
                  <span class="absolute bottom-4 left-4 bg-dark/70 backdrop-blur-md text-white border border-white/10 px-3 py-1 rounded-full text-[10px] tracking-wider uppercase font-semibold font-sans">
                    ⏱️ {{ getLocalized(tours.find(t => t.id === trip.id)?.duration === 'fullDay' ? { en: 'Full Day', de: 'Ganztägig' } : { en: 'Multiple Days', de: 'Mehrere Tage' }, trip.duration) }}
                  </span>

                  <!-- Price tag -->
                  <span class="absolute bottom-4 right-4 bg-gold text-white font-bold font-mono px-3 py-1 rounded-full text-xs shadow-md">
                    €{{ trip.price }}
                  </span>
                </div>

                <!-- Card details -->
                <div class="p-6 flex-1 flex flex-col justify-between">
                  <div>
                    <!-- Region tag -->
                    <span class="text-[9px] font-bold tracking-widest text-gold uppercase block mb-1">
                      📍 {{ getLocalized(destinations.find(d => d.id === trip.destinationId)?.names || {}, 'Egypt') }} · {{ getLocalized(categories.find(c => c.id === trip.categoryId)?.names || {}, 'Adventure') }}
                    </span>

                    <h4 class="font-playfair text-lg font-bold text-sea-deep mb-2 line-clamp-1">
                      {{ getLocalized(trip.names, 'Unnamed Tour') }}
                    </h4>
                    
                    <p class="text-xs text-muted leading-relaxed line-clamp-3 mb-4">
                      {{ getLocalized(trip.descriptions, '') }}
                    </p>
                  </div>

                  <!-- Stars display & action buttons -->
                  <div class="pt-4 border-t border-gold/15 flex justify-between items-center gap-4">
                    <div class="flex items-center gap-1">
                      <div class="flex">
                        <span v-for="star in 5" :key="star" class="text-gold text-xs">★</span>
                      </div>
                      <span class="text-[9px] font-bold text-gold uppercase tracking-wider font-mono">5.0 (Luxury)</span>
                    </div>

                    <div class="flex gap-2.5">
                      <button 
                        @click="openDetailsPage(trip)"
                        class="bg-cream hover:bg-gold/10 border border-gold/30 text-gold font-semibold text-[10px] tracking-widest uppercase py-2 px-3.5 rounded-lg transition-all cursor-pointer"
                      >
                        Details
                      </button>
                      <button 
                        @click="openBookingModal(trip)"
                        class="bg-gradient-to-r from-sea-deep to-sea hover:from-sea hover:to-sea-deep text-white font-semibold text-[10px] tracking-widest uppercase py-2 px-3.5 rounded-lg shadow-md transition-all cursor-pointer"
                      >
                        Book Now
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- PAGINATION CONTROLS -->
            <div v-if="totalPages > 1" class="mt-12 flex justify-center items-center gap-3">
              <!-- Prev page button -->
              <button 
                @click="currentPage = Math.max(1, currentPage - 1)"
                :disabled="currentPage === 1"
                class="w-10 h-10 rounded-full border border-gold/30 bg-white hover:bg-gold/10 flex items-center justify-center text-gold transition-all disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
                aria-label="Previous page"
              >
                ◀
              </button>

              <!-- Page number buttons -->
              <button 
                v-for="page in totalPages" 
                :key="page"
                @click="currentPage = page"
                class="w-10 h-10 rounded-full border text-xs font-bold font-mono transition-all cursor-pointer"
                :class="currentPage === page ? 'bg-sea border-sea-light text-white shadow-md' : 'border-gold/30 bg-white text-gold hover:bg-gold/10'"
              >
                {{ page }}
              </button>

              <!-- Next page button -->
              <button 
                @click="currentPage = Math.min(totalPages, currentPage + 1)"
                :disabled="currentPage === totalPages"
                class="w-10 h-10 rounded-full border border-gold/30 bg-white hover:bg-gold/10 flex items-center justify-center text-gold transition-all disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
                aria-label="Next page"
              >
                ▶
              </button>
            </div>
          </div>
        </section>

      </div>
    </main>

    <!-- BOOKING FORM POPUP MODAL (Double column spec layout) -->
    <Transition name="fade">
      <div v-if="showBookingModal" class="fixed inset-0 z-[2000] flex items-center justify-center p-4 bg-[#0d1f2d]/85 backdrop-blur-md" @click="showBookingModal = false">
        
        <!-- SUCCESS CUE WINDOW -->
        <div v-if="bookingSuccess" class="relative w-full max-w-lg overflow-hidden rounded-2xl bg-[#faf7f2] border border-[#c9a84c]/30 shadow-2xl p-8 text-center transition-all transform duration-300" @click.stop>
          
          <!-- Close button -->
          <button type="button" @click="showBookingModal = false" aria-label="Close success modal" class="absolute top-4 right-4 z-10 p-2 text-[#6b8a9a] hover:text-[#063a5c] hover:bg-black/5 rounded-full transition-all focus:outline-none focus:ring-2 focus:ring-[#c9a84c]">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- Circular Success Gold Checkmark -->
          <div class="mx-auto flex items-center justify-center w-20 h-20 rounded-full bg-[#c9a84c]/10 border border-[#c9a84c]/30 mb-6">
            <svg class="w-10 h-10 text-[#c9a84c]" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
            </svg>
          </div>

          <span class="inline-block px-3 py-1 text-[9px] font-bold tracking-[0.25em] uppercase text-[#c9a84c] border border-[#c9a84c]/30 rounded-full mb-3 bg-[#c9a84c]/5">
            Reservation Initiated
          </span>
          
          <h2 class="text-2xl font-bold font-serif text-[#063a5c] tracking-tight leading-snug mb-3">
            Your Egypt Journey Awaits
          </h2>

          <p class="text-sm text-[#2a3f4f] leading-relaxed mb-6 max-w-sm mx-auto">
            Thank you for choosing Seadora Travel. A dedicated Luxury Travel Planner has been assigned to your request and will contact you via email within the next 4 hours to review your customized itinerary.
          </p>

          <!-- VIP Summary Box -->
          <div class="bg-[#f7fbfd] border border-[#dce6ec] rounded-xl p-4 text-left mb-8 space-y-2.5">
            <div class="flex justify-between items-center text-xs">
              <span class="text-[#6b8a9a] uppercase tracking-wider font-semibold">Reference Code</span>
              <span class="text-[#063a5c] font-bold font-mono">{{ bookingReference }}</span>
            </div>
            <div class="border-t border-[#dce6ec]/60 my-2"></div>
            <div class="flex justify-between items-center text-xs">
              <span class="text-[#6b8a9a] uppercase tracking-wider font-semibold">Priority Status</span>
              <span class="text-[#c9a84c] font-semibold">Premium VIP Queue</span>
            </div>
          </div>

          <button type="button" @click="showBookingModal = false" class="w-full bg-[#063a5c] hover:bg-[#0a5c8a] text-white font-medium text-xs tracking-widest uppercase py-4 px-8 rounded-lg shadow-lg transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-[#063a5c] focus:ring-offset-2">
            Close Window
          </button>
        </div>

        <!-- TWO COLUMN RESERVATION FORM -->
        <div v-else class="relative w-full max-w-4xl overflow-hidden rounded-2xl bg-[#faf7f2] border border-[#c9a84c]/30 shadow-2xl flex flex-col md:flex-row transition-all transform duration-300" @click.stop>
          
          <!-- Close button -->
          <button type="button" @click="showBookingModal = false" aria-label="Close booking modal" class="absolute top-4 right-4 z-10 p-2 text-[#6b8a9a] hover:text-[#063a5c] hover:bg-black/5 rounded-full transition-all focus:outline-none focus:ring-2 focus:ring-[#c9a84c]">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- Column 1: Brand Reassurance & Selected Excursion Info -->
          <div class="w-full md:w-5/12 bg-[#063a5c] text-white p-8 md:p-10 flex flex-col justify-between relative overflow-hidden">
            <div class="absolute inset-0 opacity-5 mix-blend-overlay bg-repeat bg-center" style="background-image: radial-gradient(#c9a84c 1px, transparent 1px); background-size: 20px 20px;"></div>
            
            <div class="relative z-10">
              <span class="inline-block px-3 py-1 text-[10px] font-medium tracking-[0.2em] uppercase text-[#c9a84c] border border-[#c9a84c]/40 rounded-full mb-6 bg-[#c9a84c]/10">
                Exclusive Experiences
              </span>
              
              <h2 class="text-3xl font-extrabold font-serif tracking-tight leading-tight text-white mb-4">
                Begin Your <br>Luxury Egypt <br>Journey
              </h2>
              
              <p class="text-sm text-[#8eafc2] leading-relaxed mb-6">
                Crafted by certified Egyptologists and luxury hospitality specialists, our tours offer unmatched access and elite accommodations.
              </p>

              <!-- Selected tour card info summary box -->
              <div v-if="selectedTourForBooking" class="bg-black/20 border border-white/10 rounded-xl p-4 mb-6">
                <span class="text-[9px] uppercase tracking-wider text-gold font-bold block mb-1">Selected Package</span>
                <span class="text-sm font-semibold block font-serif">{{ getLocalized(selectedTourForBooking.names, 'Unnamed Tour') }}</span>
                <span class="text-xs text-[#8eafc2] mt-1 block">Value: €{{ selectedTourForBooking.price }} / person</span>
              </div>

              <!-- Reassurance list -->
              <div class="space-y-4">
                <div class="flex items-start space-x-3">
                  <div class="flex-shrink-0 w-5 h-5 rounded-full bg-[#c9a84c]/20 flex items-center justify-center text-[#c9a84c]">
                    <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-xs font-semibold uppercase tracking-wider text-white">Elite Local Guides</h4>
                    <p class="text-xs text-[#8eafc2] mt-0.5">Accompanied by dedicated private Egyptologists</p>
                  </div>
                </div>
                
                <div class="flex items-start space-x-3">
                  <div class="flex-shrink-0 w-5 h-5 rounded-full bg-[#c9a84c]/20 flex items-center justify-center text-[#c9a84c]">
                    <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-xs font-semibold uppercase tracking-wider text-white">Flexible Cancellation</h4>
                    <p class="text-xs text-[#8eafc2] mt-0.5">Free changes up to 72 hours prior to departure</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Stripe / Gold Badge -->
            <div class="relative z-10 mt-8 pt-6 border-t border-[#8eafc2]/20 flex items-center justify-between">
              <div class="text-left">
                <div class="text-xs text-[#8eafc2] uppercase tracking-widest font-semibold">Stripe Secured</div>
                <div class="text-[10px] text-[#8eafc2]/60 mt-0.5">Encrypted Connection</div>
              </div>
              <div class="text-right">
                <div class="text-xs text-[#c9a84c] font-bold tracking-widest">SeeDora Gold</div>
                <div class="text-[10px] text-[#8eafc2]/60 mt-0.5">Signature Service</div>
              </div>
            </div>
          </div>

          <!-- Column 2: Inputs Form -->
          <form @submit.prevent="submitBooking" class="w-full md:w-7/12 p-8 md:p-10 flex flex-col justify-between">
            <div>
              <h3 class="text-xl font-bold font-serif text-[#063a5c] tracking-tight mb-6">
                Request Private Reservation
              </h3>

              <div class="space-y-4">
                <!-- Field: Name -->
                <div class="flex flex-col">
                  <label for="book-name" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Full Name</label>
                  <input type="text" id="book-name" v-model="bookingForm.name" @input="validateField('name')" placeholder="Alexander Vance"
                    class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]">
                  <span v-if="bookingErrors.name" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.name }}</span>
                </div>

                <!-- Row 1: Email and Phone -->
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div class="flex flex-col">
                    <label for="book-email" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Email Address</label>
                    <input type="email" id="book-email" v-model="bookingForm.email" @input="validateField('email')" placeholder="alex@vance.com"
                      class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]">
                    <span v-if="bookingErrors.email" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.email }}</span>
                  </div>
                  
                  <div class="flex flex-col">
                    <label for="book-phone" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Phone Number</label>
                    <input type="tel" id="book-phone" v-model="bookingForm.phone" @input="validateField('phone')" placeholder="+1 555-0199"
                      class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]">
                    <span v-if="bookingErrors.phone" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.phone }}</span>
                  </div>
                </div>

                <!-- Row 2: Destination Select & Travel Date -->
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div class="flex flex-col">
                    <label for="book-dest" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Select Destination</label>
                    <div class="relative">
                      <select id="book-dest" v-model="bookingForm.destination" @change="validateField('destination')"
                        class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)] appearance-none cursor-pointer pr-10">
                        <option value="" disabled>Select Region</option>
                        <option value="hurghada">Hurghada (Red Sea)</option>
                        <option value="cairo">Cairo (Pyramids & History)</option>
                        <option value="luxor">Luxor (Nile Cruises)</option>
                        <option value="sharm">Sharm El-Sheikh (Resorts)</option>
                      </select>
                      <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-[#6b8a9a]">
                        <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                          <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/>
                        </svg>
                      </div>
                    </div>
                    <span v-if="bookingErrors.destination" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.destination }}</span>
                  </div>
                  
                  <div class="flex flex-col">
                    <label for="book-date" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Target Date</label>
                    <input type="date" id="book-date" v-model="bookingForm.date" @change="validateField('date')"
                      class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)] cursor-pointer">
                    <span v-if="bookingErrors.date" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.date }}</span>
                  </div>
                </div>

                <!-- Row 3: Choices/Options Inside Tour -->
                <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
                  <!-- Option 1: Package Choice -->
                  <div class="flex flex-col">
                    <label for="book-pack" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Tour Tier</label>
                    <div class="relative">
                      <select id="book-pack" v-model="bookingForm.packageOption"
                        class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)] appearance-none cursor-pointer pr-10">
                        <option value="standard">Standard</option>
                        <option value="premium">Premium VIP</option>
                        <option value="elite">Elite Private</option>
                      </select>
                      <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-[#6b8a9a]">
                        <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                          <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/>
                        </svg>
                      </div>
                    </div>
                  </div>

                  <!-- Option 2: Guide Language -->
                  <div class="flex flex-col">
                    <label for="book-lang" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Guide Lang</label>
                    <div class="relative">
                      <select id="book-lang" v-model="bookingForm.guideLanguage"
                        class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)] appearance-none cursor-pointer pr-10">
                        <option value="en">🇬🇧 English</option>
                        <option value="de">🇩🇪 German</option>
                        <option value="fr">🇫🇷 French</option>
                        <option value="it">🇮🇹 Italian</option>
                        <option value="ru">🇷🇺 Russian</option>
                      </select>
                      <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-[#6b8a9a]">
                        <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                          <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/>
                        </svg>
                      </div>
                    </div>
                  </div>

                  <!-- Option 3: Pickup Option -->
                  <div class="flex flex-col">
                    <label for="book-pickup" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Pickup Service</label>
                    <div class="relative">
                      <select id="book-pickup" v-model="bookingForm.pickupRequired"
                        class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)] appearance-none cursor-pointer pr-10">
                        <option value="no">No pickup</option>
                        <option value="yes">Yes (+€15/p)</option>
                      </select>
                      <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-[#6b8a9a]">
                        <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                          <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/>
                        </svg>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Row 4: Guests & Notes -->
                <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
                  <div class="flex flex-col sm:col-span-1">
                    <label for="book-guests" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Guests</label>
                    <div class="relative">
                      <select id="book-guests" v-model="bookingForm.guests" @change="validateField('guests')"
                        class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)] appearance-none cursor-pointer pr-10">
                        <option value="1">1 Guest</option>
                        <option value="2">2 Guests</option>
                        <option value="3">3 Guests</option>
                        <option value="4">4 Guests</option>
                        <option value="5+">5+ Guests</option>
                      </select>
                      <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-[#6b8a9a]">
                        <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                          <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/>
                        </svg>
                      </div>
                    </div>
                    <span v-if="bookingErrors.guests" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.guests }}</span>
                  </div>

                  <div class="flex flex-col sm:col-span-2">
                    <label for="book-notes" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">Special Requests</label>
                    <input type="text" id="book-notes" v-model="bookingForm.notes" placeholder="Private yacht charter, dietary restrictions..."
                      class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]">
                  </div>
                </div>
              </div>
            </div>

            <!-- Footer Action -->
            <div class="mt-8 pt-6 border-t border-[#dce6ec] flex flex-col sm:flex-row items-center justify-between gap-4">
              <p class="text-xs text-[#6b8a9a] text-center sm:text-left leading-relaxed">
                No charge at booking. Review terms in details.
              </p>
              <button type="submit" :disabled="bookingLoading" class="w-full sm:w-auto bg-gradient-to-r from-[#0a5c8a] to-[#1a8bc4] hover:from-[#1a8bc4] hover:to-[#0a5c8a] text-white font-medium text-xs tracking-widest uppercase py-4 px-8 rounded-lg shadow-lg hover:shadow-xl hover:translate-y-[-1px] active:translate-y-0 transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-[#0a5c8a] focus:ring-offset-2 disabled:opacity-50">
                <span v-if="bookingLoading" class="flex items-center gap-1.5 justify-center">
                  <svg class="animate-spin h-3.5 w-3.5 text-white" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Requesting...
                </span>
                <span v-else>Request Reservation</span>
              </button>
            </div>
          </form>
        </div>

      </div>
    </Transition>

    <!-- Footer -->
    <Footer />
  </div>
</template>

<style scoped>
/* Page transition custom styling */
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;  
  overflow: hidden;
}
.line-clamp-1 {
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;  
  overflow: hidden;
}
.font-playfair {
  font-family: 'Playfair Display', serif;
}
.font-jost {
  font-family: 'Jost', sans-serif;
}
.font-cormorant {
  font-family: 'Cormorant Garamond', serif;
}
</style>
