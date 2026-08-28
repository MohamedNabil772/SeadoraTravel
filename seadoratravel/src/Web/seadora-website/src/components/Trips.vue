<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store/auth'
import { getFullImageUrl } from '@/shared/utils/helpers'

const { t, locale, te } = useI18n()
const router = useRouter()
const authStore = useAuthStore()

interface Category {
  id: string;
  names: Record<string, string>;
  icon: string;
}

interface Tour {
  id: string;
  slug?: string;
  title?: string;
  description?: string;
  names?: Record<string, string>;
  descriptions?: Record<string, string>;
  categoryId: string;
  price: number;
  duration: string;
  includes?: string[];
  imageUrl?: string;
  mainImage?: string;
  images?: string[];
  emoji?: string;
  bgGradient?: string;
  badge?: string;
}

const currentCategoryId = ref('all')
const tours = ref<Tour[]>([])
const categories = ref<Category[]>([])
const loading = ref(true)

const getSlug = (name: string) => {
  return name
    .toLowerCase()
    .replace(/[^\w\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
    .trim()
}

const openDetailsPage = (tour: Tour) => {
  const slug = tour.slug || getSlug(tour.title || tour.names?.['en'] || 'tour')
  router.push(`/tour/${slug}`)
}

// Booking Modal State
const showModal = ref(false)
const selectedTour = ref<Tour | null>(null)
const customerName = ref('')
const customerEmail = ref('')
const bookingSuccess = ref(false)
const bookingLoading = ref(false)

onMounted(async () => {
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000';
    
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
  } catch (error) {
    console.error("Failed to load content:", error)
  } finally {
    loading.value = false
  }
})

const filteredTrips = computed(() => {
  if (currentCategoryId.value === 'all') return tours.value
  return tours.value.filter(trip => trip.categoryId === currentCategoryId.value)
})

const filterTrips = (catId: string) => {
  currentCategoryId.value = catId
}

const getLocalized = (dict: any, fallback: string = '') => {
  if (!dict) return fallback;
  if (typeof dict === 'string') return dict;
  if (typeof dict === 'object') {
    return dict[locale.value] || dict['en'] || Object.values(dict)[0] || fallback;
  }
  return fallback;
}

const generateGradient = (id: string) => {
  if (!id) return 'linear-gradient(135deg,#7c4a14,#e8820a)'
  const hash = id.split('').reduce((acc, char) => char.charCodeAt(0) + ((acc << 5) - acc), 0)
  const color1 = `hsl(${Math.abs(hash) % 360}, 70%, 30%)`
  const color2 = `hsl(${(Math.abs(hash) + 40) % 360}, 80%, 40%)`
  return `linear-gradient(135deg, ${color1}, ${color2})`
}

const getCategory = (catId: string) => categories.value.find(c => c.id === catId)

const getTourBackground = (trip: Tour) => {
  if (trip.imageUrl || trip.mainImage) {
    return `url(${getFullImageUrl(trip.imageUrl || trip.mainImage)}) center/cover`;
  }
  const cat = getCategory(trip.categoryId);
  if (cat && (cat as any).coverImageUrl) {
    return `url(${getFullImageUrl((cat as any).coverImageUrl)}) center/cover`;
  }
  return trip.bgGradient || generateGradient(trip.categoryId);
}

const getCategoryIcon = (catId: string) => {
  const cat = getCategory(catId);
  return cat ? cat.icon : '🌍';
}

const getCategoryName = (catId: string) => {
  const cat = getCategory(catId);
  return cat ? getLocalized(cat.names, 'Adventure') : 'Adventure';
}

const openBookingModal = (tour: Tour) => {
  selectedTour.value = tour;
  if (authStore.user) {
    customerName.value = authStore.user.name || authStore.user.fullName || '';
    customerEmail.value = authStore.user.email || '';
  }
  showModal.value = true;
  bookingSuccess.value = false;
}

const submitBooking = async () => {
  if (!selectedTour.value || !customerName.value || !customerEmail.value) return;
  
  bookingLoading.value = true;
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000';
    const response = await fetch(`${API_URL}/api/booking/api/bookings`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        tourId: selectedTour.value.id,
        customerName: customerName.value,
        customerEmail: customerEmail.value
      })
    });
    
    if (response.ok) {
      bookingSuccess.value = true;
      customerName.value = '';
      customerEmail.value = '';
    } else {
      alert("Booking failed. Please try again.");
    }
  } catch (error) {
    console.error("Booking error:", error);
  } finally {
    bookingLoading.value = false;
  }
}
</script>

<template>
  <section class="section trips-bg" id="trips">
    <div class="section-header text-center mb-[70px] relative z-10">
      <div class="section-eyebrow text-[11px] tracking-[0.25em] uppercase text-sun-light font-semibold mb-3.5 flex items-center justify-center gap-3">
        <span class="w-10 h-[1px] bg-sun-light"></span>
        {{ t('trips.eyebrow') }}
        <span class="w-10 h-[1px] bg-sun-light"></span>
      </div>
      <h2 class="font-serif text-[clamp(32px,4vw,52px)] font-bold text-white leading-[1.15] mb-4.5 text-center">
        <span v-html="t('trips.title')"></span>
      </h2>
      <p class="section-sub font-cormorant text-[19px] text-[rgba(255,255,255,0.55)] max-w-[600px] mx-auto leading-[1.7] text-center">
        {{ t('trips.description') }}
      </p>
    </div>

    <div class="trips-tabs flex gap-2 justify-center mb-[50px] flex-wrap relative z-10">
      <button 
        @click="filterTrips('all')"
        class="trip-tab bg-[rgba(255,255,255,0.06)] border border-[rgba(255,255,255,0.1)] text-[rgba(255,255,255,0.6)] py-2.5 px-6 rounded-[30px] cursor-pointer font-sans text-[13px] tracking-[0.08em] transition-all duration-250 hover:bg-sea hover:border-sea-light hover:text-white"
        :class="{ 'active bg-sea border-sea-light text-white': currentCategoryId === 'all' }"
      >
        {{ t('trips.categories.all') }}
      </button>
      <button 
        v-for="cat in categories" 
        :key="cat.id"
        @click="filterTrips(cat.id)"
        class="trip-tab bg-[rgba(255,255,255,0.06)] border border-[rgba(255,255,255,0.1)] text-[rgba(255,255,255,0.6)] py-2.5 px-6 rounded-[30px] cursor-pointer font-sans text-[13px] tracking-[0.08em] transition-all duration-250 hover:bg-sea hover:border-sea-light hover:text-white"
        :class="{ 'active bg-sea border-sea-light text-white': currentCategoryId === cat.id }"
      >
        <span class="mr-2">{{ cat.icon }}</span>
        {{ getLocalized(cat.names, 'Category') }}
      </button>
    </div>

    <div v-if="loading" class="text-center text-white py-10 relative z-10">Loading tours...</div>

    <TransitionGroup v-else name="list" tag="div" class="trips-grid">
      <div 
        v-for="trip in filteredTrips" 
        :key="trip.id"
        class="trip-card cursor-pointer list-item"
        @click="openDetailsPage(trip)"
      >
        <div class="trip-img" :style="{ background: getTourBackground(trip) }">
          <div v-if="!(trip.imageUrl || trip.mainImage)" class="trip-img-emoji">{{ trip.emoji || getCategoryIcon(trip.categoryId) || '🌍' }}</div>
          <span class="trip-duration">
            {{ te('trips.durations.' + trip.duration) ? t('trips.durations.' + trip.duration) : trip.duration }}
          </span>
          <span v-if="trip.badge" class="trip-badge">
            {{ trip.badge }}
          </span>
        </div>
        <div class="trip-body">
          <div class="trip-cat">{{ getCategoryName(trip.categoryId) }}</div>
          <div class="trip-name">
            {{ trip.title || getLocalized(trip.names, 'Unnamed Tour') }}
          </div>
          <div class="trip-desc">
            {{ trip.description || getLocalized(trip.descriptions, '') }}
          </div>
          <div class="trip-includes">
            <span v-for="inc in trip.includes" :key="inc" class="trip-tag">
              {{ inc }}
            </span>
          </div>
          <div class="trip-footer">
            <div class="trip-price">
              <div class="from">FROM</div>
              <div class="amount">€{{ trip.price }}</div>
              <div class="per">/person</div>
            </div>
            <button @click.stop="openBookingModal(trip)" class="btn-book">
              {{ t('trips.bookBtn') }}
            </button>
          </div>
        </div>
      </div>
    </TransitionGroup>

    <!-- Booking Modal -->
    <Transition name="fade">
      <div v-if="showModal" class="fixed inset-0 z-[2000] flex items-center justify-center bg-dark/80 backdrop-blur-md p-4" @click="showModal = false">
        <div class="bg-white/95 backdrop-blur-md border border-gold/45 rounded-2xl overflow-hidden shadow-2xl relative w-full max-w-md animate-slide-up" @click.stop>
          <!-- Top Decorator Gold Line -->
          <div class="h-1.5 w-full bg-gradient-to-r from-sea via-gold to-sun"></div>

          <!-- Close Icon button -->
          <button @click="showModal = false" class="absolute top-4 right-4 z-50 w-7 h-7 rounded-full bg-dark/10 border border-gold/20 text-dark hover:bg-sun hover:border-sun-light hover:text-white flex items-center justify-center text-sm font-bold cursor-pointer transition-all duration-300 hover:rotate-90" aria-label="Close modal">
            <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
          
          <div class="p-8">
            <div v-if="bookingSuccess" class="text-center py-6 font-sans">
              <div class="relative mb-6 inline-flex justify-center items-center w-20 h-20">
                <div class="w-20 h-20 rounded-full bg-grass/10 border-2 border-grass flex items-center justify-center animate-scale-up">
                  <svg class="w-10 h-10 text-grass" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="3">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <div class="absolute -inset-1 rounded-full bg-grass/5 animate-pulse"></div>
              </div>
              <h3 class="text-2xl font-bold text-dark mb-2 font-serif">Reservation Confirmed</h3>
              <p class="text-muted text-xs leading-relaxed max-w-sm mx-auto">
                Shukran! We have received your luxury reservation request. Our private concierge will contact you shortly to coordinate payment and details.
              </p>
              <div class="mt-8 flex flex-col sm:flex-row justify-center gap-3">
                <button 
                  v-if="authStore.isAuthenticated"
                  @click="showModal = false; router.push('/portal/bookings')"
                  class="bg-gradient-to-r from-sea-deep to-sea text-white px-6 py-3 rounded-lg font-bold uppercase text-[10px] tracking-widest shadow-md hover:translate-y-[-1px] transition-all cursor-pointer flex items-center justify-center gap-2"
                >
                  <span>⛵</span>
                  <span>View in Your Journeys</span>
                </button>
                <button @click="showModal = false" class="bg-slate-100 hover:bg-slate-200 text-slate-700 px-6 py-3 rounded-lg font-bold uppercase text-[10px] tracking-widest transition-all cursor-pointer">
                  Close Window
                </button>
              </div>
            </div>
            
            <div v-else>
              <span class="text-[9px] tracking-widest text-gold uppercase font-bold text-center block mb-1.5 font-sans">Luxury Booking Portal</span>
              <h3 class="text-2xl font-bold text-dark mb-2 text-center font-serif">Reserve Experience</h3>
              <p class="text-sea font-serif text-base text-center mb-6 max-w-xs mx-auto border-b border-gold/15 pb-4">{{ selectedTour ? (selectedTour.title || getLocalized(selectedTour.names, '')) : '' }}</p>
              
              <form @submit.prevent="submitBooking" class="space-y-4 font-sans">
                <div>
                  <label class="block text-[9px] font-bold text-muted mb-1.5 uppercase tracking-widest font-sans">Full Name</label>
                  <input v-model="customerName" type="text" required class="w-full p-3 border border-gold/30 rounded-lg focus:ring-1 focus:ring-sea focus:border-sea bg-cream/35 focus:bg-white outline-none text-xs font-jost transition-all" :placeholder="$t('placeholders.fullName')">
                </div>
                <div class="space-y-1.5">
                  <label class="block text-[11px] font-medium text-dark/70 uppercase tracking-wider font-cormorant">Email Address</label>
                  <input v-model="customerEmail" type="email" required class="w-full p-3 border border-gold/30 rounded-lg focus:ring-1 focus:ring-sea focus:border-sea bg-cream/35 focus:bg-white outline-none text-xs font-jost transition-all" :placeholder="$t('placeholders.email')">
                </div>
                
                <div class="pt-4 mt-6 border-t border-gold/15 flex justify-between items-center bg-cream/30 p-4 rounded-xl border border-gold/10">
                  <div class="text-left font-jost">
                    <div class="text-[8px] text-muted tracking-widest">TOTAL VALUE</div>
                    <div class="text-2xl text-gold font-bold font-serif">€{{ selectedTour?.price }}</div>
                    <div class="text-[8px] text-muted">/ person</div>
                  </div>
                  <button type="submit" :disabled="bookingLoading" class="bg-gradient-to-r from-sea-deep to-sea text-white px-6 py-3.5 rounded-lg font-bold uppercase text-[10px] tracking-widest shadow-md hover:translate-y-[-1px] disabled:opacity-50 transition-all cursor-pointer">
                    <span v-if="bookingLoading" class="flex items-center gap-1.5">
                      <svg class="animate-spin h-3.5 w-3.5 text-white animate-spin" fill="none" viewBox="0 0 24 24">
                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                      Booking...
                    </span>
                    <span v-else>Confirm Lead</span>
                  </button>
                </div>
                
                <p class="text-[9px] text-muted text-center mt-4 italic tracking-wide leading-relaxed">
                  No payment is taken at this step. Your personal travel butler will confirm availability and secure the booking.
                </p>
              </form>
            </div>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Tour Details Modal is now opened via Router details page route -->
  </section>
</template>

<style scoped>
/* ─── TRIPS SECTION ─── */
.section { padding: 100px 48px; }
.section-header { text-align: center; margin-bottom: 70px; }
.section-eyebrow {
  font-size: 11px; letter-spacing: 0.25em; text-transform: uppercase;
  color: var(--sun-light); font-weight: 600; margin-bottom: 14px;
  display: flex; align-items: center; justify-content: center; gap: 12px;
}
.section-eyebrow::before, .section-eyebrow::after {
  content: ''; width: 40px; height: 1px; background: var(--sun-light);
}
.section h2 {
  font-family: 'Playfair Display', serif;
  font-size: clamp(32px, 4vw, 52px); font-weight: 700;
  color: var(--white); line-height: 1.15; margin-bottom: 18px;
}
.section h2 :deep(span) { color: var(--sun-light); }
.section-sub {
  font-family: 'Cormorant Garamond', serif;
  font-size: 19px; color: rgba(255,255,255,0.55); max-width: 600px; margin: 0 auto; line-height: 1.7;
}

.trips-bg { background: var(--dark); position: relative; overflow: hidden; }
.trips-bg::before {
  content: '';
  position: absolute; inset: 0;
  background: radial-gradient(ellipse 60% 80% at 100% 50%, rgba(10,92,138,0.3) 0%, transparent 60%),
              radial-gradient(ellipse 40% 60% at 0% 80%, rgba(46,125,79,0.2) 0%, transparent 50%);
}

.trips-tabs {
  display: flex; gap: 8px; justify-content: center; margin-bottom: 50px; flex-wrap: wrap;
  position: relative; z-index: 10;
}
.trip-tab {
  background: rgba(255,255,255,0.06); border: 1px solid rgba(255,255,255,0.1);
  color: rgba(255,255,255,0.6); padding: 10px 24px; border-radius: 30px;
  cursor: pointer; font-family: 'Jost', sans-serif; font-size: 13px;
  letter-spacing: 0.08em; transition: all 0.25s;
}
.trip-tab:hover, .trip-tab.active {
  background: var(--sea); border-color: var(--sea-light); color: var(--white);
}

.trips-grid { 
  display: grid; grid-template-columns: repeat(3, 1fr); gap: 28px;
  position: relative; z-index: 10;
}
.trip-card {
  background: rgba(255,255,255,0.04);
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 12px; overflow: hidden;
  transition: transform 0.3s, border-color 0.3s, box-shadow 0.3s;
}
.trip-card:hover {
  transform: translateY(-4px);
  border-color: rgba(232,130,10,0.4);
  box-shadow: 0 20px 50px rgba(0,0,0,0.4);
}
.trip-img {
  height: 200px; position: relative; overflow: hidden;
  display: flex; align-items: center; justify-content: center;
}
.trip-img-emoji { font-size: 72px; opacity: 0.7; }
.trip-duration {
  position: absolute; top: 16px; right: 16px;
  background: rgba(0,0,0,0.6); color: var(--white);
  padding: 4px 10px; border-radius: 20px; font-size: 12px;
}
.trip-badge {
  position: absolute; top: 16px; left: 16px;
  background: var(--sun); color: var(--white);
  padding: 4px 10px; border-radius: 20px; font-size: 11px; font-weight: 600;
}
.trip-body { padding: 24px; }
.trip-cat {
  font-size: 10px; letter-spacing: 0.2em; text-transform: uppercase;
  color: var(--grass-light); margin-bottom: 8px;
}
.trip-name {
  font-family: 'Playfair Display', serif;
  font-size: 20px; color: var(--white); margin-bottom: 10px; line-height: 1.3;
}
.trip-desc { font-size: 13px; color: rgba(255,255,255,0.55); line-height: 1.6; margin-bottom: 18px; }
.trip-includes { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 20px; }
.trip-tag {
  background: rgba(255,255,255,0.06); color: rgba(255,255,255,0.5);
  font-size: 11px; padding: 3px 10px; border-radius: 3px;
}
.trip-footer {
  display: flex; justify-content: space-between; align-items: center;
  padding-top: 18px; border-top: 1px solid rgba(255,255,255,0.07);
}
.trip-price .from { font-size: 10px; color: rgba(255,255,255,0.4); letter-spacing: 0.1em; }
.trip-price .amount {
  font-family: 'Playfair Display', serif;
  font-size: 26px; color: var(--sun-light); font-weight: 700;
}
.trip-price .per { font-size: 11px; color: rgba(255,255,255,0.4); }
.btn-book {
  background: linear-gradient(135deg, var(--sea), var(--sea-light));
  color: var(--white); border: none; padding: 10px 22px;
  border-radius: 4px; cursor: pointer; font-size: 12px;
  font-family: 'Jost', sans-serif; font-weight: 600;
  letter-spacing: 0.08em; text-transform: uppercase;
  transition: all 0.25s;
}
.btn-book:hover { transform: translateY(-1px); box-shadow: 0 8px 20px rgba(26,139,196,0.4); }

@media (max-width: 1100px) {
  .trips-grid { grid-template-columns: 1fr 1fr; }
}
@media (max-width: 768px) {
  .section { padding: 70px 20px; }
  .trips-grid { grid-template-columns: 1fr; }
}
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

@keyframes slide-up {
  from { transform: translateY(20px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

.animate-slide-up {
  animation: slide-up 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

@keyframes scale-up {
  0% { transform: scale(0.8); opacity: 0; }
  100% { transform: scale(1); opacity: 1; }
}

.animate-scale-up {
  animation: scale-up 0.4s cubic-bezier(0.34, 1.56, 0.64, 1) forwards;
}

.list-enter-active,
.list-leave-active {
  transition: all 0.4s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateY(20px);
}
.list-leave-active {
  position: absolute;
}
</style>
