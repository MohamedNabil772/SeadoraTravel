<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getFullImageUrl } from '@/shared/utils/helpers'

interface Tour {
  id: string;
  slug?: string;
  title?: string;
  description?: string;
  names?: Record<string, string>;
  descriptions?: Record<string, string>;
  categoryId?: string;
  price: number;
  duration: string;
  includes?: string[];
  imageUrl?: string;
  mainImage?: string;
  emoji?: string;
  bgGradient?: string;
  badge?: string;
  mediaUrls?: string[];
  images?: string[];
  originalPrice?: number;
  discountPercentage?: number;
}

interface Review {
  id: string;
  customerName: string;
  rating: number;
  comment: string;
  createdAt: string;
}

const props = defineProps<{
  show: boolean;
  tour: Tour | null;
  categoryName: string;
}>()

const emit = defineEmits<{
  (e: 'close'): void;
  (e: 'book', tour: Tour): void;
}>()

const router = useRouter()
const reviews = ref<Review[]>([])
const loadingReviews = ref(false)

// Generate some premium default mock reviews based on the tour
const getMockReviews = (tourId: string): Review[] => {
  const reviewsPool: Record<string, Review[]> = {
    '1': [
      { id: 'r1', customerName: 'Charlotte Sterling', rating: 5, comment: 'Sailing on the Nile under the stars was pure magic. The private guide was incredibly knowledgeable and the dining was spectacular.', createdAt: '2026-05-18' },
      { id: 'r2', customerName: 'Dr. Arthur Pendelton', rating: 5, comment: 'An exceptional journey through Egypt\'s heritage. Every detail was curated with 5-star service. Highly recommend the sunset deck lounge.', createdAt: '2026-06-02' },
      { id: 'r3', customerName: 'Sophia Vianni', rating: 4, comment: 'Fabulous views and comfortable cabins. Luxor temples are absolutely breathtaking at night. Minor delay at embarkation but resolved smoothly.', createdAt: '2026-06-10' }
    ],
    '2': [
      { id: 'r4', customerName: 'Maximilian Schwarz', rating: 5, comment: 'Crystal clear visibility and spectacular marine life. We swam alongside sea turtles and explored untouched corals. Unforgettable!', createdAt: '2026-04-20' },
      { id: 'r5', customerName: 'Jessica Vance', rating: 5, comment: 'The dive masters are true professionals. Safety and luxury service combined seamlessly. The yacht used for the dive was elite.', createdAt: '2026-05-14' }
    ],
    '3': [
      { id: 'r6', customerName: 'Amina Al-Mansoor', rating: 5, comment: 'A captivating trek across the dunes. The Bedouin tea by the campfire under the Milky Way was a highlight of my year.', createdAt: '2026-03-30' },
      { id: 'r7', customerName: 'Liam O\'Connor', rating: 4, comment: 'Stunning landscapes and premium quad bikes. Very thrilling but also felt very safe and comfortable. The sunset photos are unreal.', createdAt: '2026-05-08' }
    ]
  }

  // Fallback reviews if the tour ID doesn't have custom ones
  const defaultReviews = [
    { id: 'd1', customerName: 'Valerie Laurent', rating: 5, comment: 'Absolutely breathtaking! Seadora Travel provided a flawless, ultra-premium experience from start to finish.', createdAt: '2026-06-12' },
    { id: 'd2', customerName: 'James Sinclair', rating: 4, comment: 'Stunning scenery, professional staff, and superb coordination. True luxury in the heart of Egypt.', createdAt: '2026-06-15' }
  ]

  // Map tour id digits or fallback
  const numericId = tourId.replace(/\D/g, '') || '1'
  const key = Object.keys(reviewsPool).includes(numericId) ? numericId : '1'
  return reviewsPool[key] || defaultReviews
}

onMounted(() => {
  fetchReviews()
})

const fetchReviews = async () => {
  if (!props.tour) return
  loadingReviews.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const res = await fetch(`${API_URL}/api/booking/api/feedbacks?tourId=${props.tour.id}`)
    if (res.ok) {
      const data = await res.json()
      if (data && data.length > 0) {
        reviews.value = data
      } else {
        reviews.value = getMockReviews(props.tour.id)
      }
    } else {
      reviews.value = getMockReviews(props.tour.id)
    }
  } catch (e) {
    reviews.value = getMockReviews(props.tour.id)
  } finally {
    loadingReviews.value = false
  }
}

const currentIndex = ref(0)
const transitionName = ref('slide-next')

const nextReview = () => {
  if (reviews.value.length === 0) return
  transitionName.value = 'slide-next'
  currentIndex.value = (currentIndex.value + 1) % reviews.value.length
}

const prevReview = () => {
  if (reviews.value.length === 0) return
  transitionName.value = 'slide-prev'
  currentIndex.value = (currentIndex.value - 1 + reviews.value.length) % reviews.value.length
}

const setReview = (index: number) => {
  transitionName.value = index > currentIndex.value ? 'slide-next' : 'slide-prev'
  currentIndex.value = index
}

const activeMediaIndex = ref(0)

const activeVisualList = computed(() => {
  if (props.tour?.mediaUrls && props.tour.mediaUrls.length > 0) return props.tour.mediaUrls
  if (props.tour?.images && props.tour.images.length > 0) return props.tour.images
  if (props.tour?.imageUrl) return [props.tour.imageUrl]
  if (props.tour?.mainImage) return [props.tour.mainImage]
  return []
})

const nextMedia = () => {
  const urls = activeVisualList.value
  if (urls.length === 0) return
  activeMediaIndex.value = (activeMediaIndex.value + 1) % urls.length
}

const prevMedia = () => {
  const urls = activeVisualList.value
  if (urls.length === 0) return
  activeMediaIndex.value = (activeMediaIndex.value - 1 + urls.length) % urls.length
}

function isVideo(url?: string): boolean {
  if (!url) return false
  const videoExtensions = ['mp4', 'mov', 'avi', 'mkv', 'webm', 'ogg']
  return videoExtensions.some(ext => url.endsWith(ext) || url.toLowerCase().includes('video') || url.toLowerCase().includes('.mp4'))
}

// Watch for tour change to fetch reviews
import { watch } from 'vue'
watch(() => props.tour, () => {
  if (props.tour) {
    currentIndex.value = 0
    activeMediaIndex.value = 0
    fetchReviews()
  }
})

const averageRating = computed(() => {
  if (reviews.value.length === 0) return 5
  const sum = reviews.value.reduce((acc, r) => acc + r.rating, 0)
  return Math.round((sum / reviews.value.length) * 10) / 10
})

const navigateToFeedback = () => {
  if (!props.tour) return
  emit('close')
  router.push({ path: '/feedback', query: { tourId: props.tour.id } })
}

const formatDate = (dateString: string) => {
  const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'long', day: 'numeric' }
  return new Date(dateString).toLocaleDateString('en-US', options)
}

const getStarDisplayWidth = (star: number, rating: number) => {
  if (rating >= star) return '100%'
  if (rating > star - 1) {
    const fraction = rating - (star - 1)
    return `${fraction * 100}%`
  }
  return '0%'
}
</script>

<template>
  <Transition name="fade">
    <div v-if="show && tour" class="fixed inset-0 z-[1500] flex items-center justify-center bg-dark/80 backdrop-blur-md p-4 overflow-y-auto" @click="emit('close')">
      <div 
        class="modal-wrapper animate-slide-up"
        @click.stop
      >
        <!-- Close Button -->
        <button 
          @click="emit('close')" 
          class="modal-close-btn"
          aria-label="Close modal"
        >
          &times;
        </button>

        <!-- Left Column: Tour Visuals & Core Specs -->
        <div class="modal-visuals-col">
          <!-- background media slider -->
          <div class="absolute inset-0 z-0">
            <template v-if="activeVisualList.length > 0">
              <video 
                v-if="isVideo(activeVisualList[activeMediaIndex])"
                :key="'vid-' + activeMediaIndex"
                :src="getFullImageUrl(activeVisualList[activeMediaIndex])"
                autoplay 
                loop 
                muted 
                playsinline
                class="w-full h-full object-cover opacity-35"
              ></video>
              <img 
                v-else
                :key="'img-' + activeMediaIndex"
                :src="getFullImageUrl(activeVisualList[activeMediaIndex])"
                class="w-full h-full object-cover opacity-35"
                alt="Tour Detail Visual"
              />
              
              <!-- Left/Right navigation for media -->
              <div v-if="activeVisualList.length > 1" class="absolute bottom-24 right-4 z-20 flex gap-2">
                <button type="button" @click.stop="prevMedia" class="w-7 h-7 rounded-full bg-dark/60 border border-white/20 text-white flex items-center justify-center hover:bg-gold hover:border-gold transition-all cursor-pointer" title="Previous Visual">&larr;</button>
                <button type="button" @click.stop="nextMedia" class="w-7 h-7 rounded-full bg-dark/60 border border-white/20 text-white flex items-center justify-center hover:bg-gold hover:border-gold transition-all cursor-pointer" title="Next Visual">&rarr;</button>
              </div>
            </template>
            <template v-else>
              <div 
                class="w-full h-full opacity-35"
                :style="{ background: tour.bgGradient || 'linear-gradient(135deg,#063a5c,#1a9b8a)' }"
              ></div>
            </template>
          </div>
          
          <div class="relative z-10 p-8 flex flex-col h-full justify-between gap-12">
            <div>
              <span class="inline-block text-[9px] tracking-[0.25em] text-sun-light font-bold uppercase mb-2 font-jost">
                {{ categoryName }} · Egypt
              </span>
              <h2 class="font-playfair text-3xl md:text-4xl font-bold text-white leading-tight mb-4">
                {{ tour.title || tour.names?.['en'] || 'Unnamed Luxury Experience' }}
              </h2>
              <p class="text-white/70 text-sm leading-relaxed mb-6 font-jost">
                {{ tour.description || tour.descriptions?.['en'] }}
              </p>
            </div>

            <div>
              <!-- Tour Quick Stats -->
              <div class="grid grid-cols-2 gap-4 mb-6">
                <div class="stat-box">
                  <span class="stat-label">Duration</span>
                  <span class="stat-value">{{ tour.duration }}</span>
                </div>
                <div class="stat-box">
                  <span class="stat-label">Includes</span>
                  <span class="stat-value text-xs truncate block">{{ tour.includes?.join(', ') || 'Guide, Services' }}</span>
                </div>
              </div>

              <!-- Price & Booking Row -->
              <div class="booking-row">
                <div class="text-left font-jost">
                  <div class="rate-from">Private Tour Rate</div>
                  <div class="flex items-center gap-2">
                    <div class="rate-amount">€{{ tour.price }}</div>
                    <div v-if="tour.originalPrice && tour.originalPrice > tour.price" class="text-white/40 line-through text-sm">€{{ tour.originalPrice }}</div>
                    <div v-if="tour.discountPercentage" class="bg-red-500/20 text-red-300 text-[10px] px-1.5 py-0.5 rounded font-bold">{{ tour.discountPercentage }}% OFF</div>
                  </div>
                  <div class="rate-per">/person</div>
                </div>
                <button 
                  @click="emit('book', tour)" 
                  class="reserve-btn"
                >
                  Reserve Now
                </button>
              </div>
            </div>
          </div>

          <!-- Elegant Decorative Motif -->
          <div class="absolute bottom-2 right-4 text-white/5 font-serif text-[120px] select-none pointer-events-none leading-none">
            {{ tour.emoji || '🐪' }}
          </div>
        </div>

        <!-- Right Column: Reviews & Comments -->
        <div class="modal-reviews-col">
          <div>
            <!-- Review Section Header -->
            <div class="reviews-header-row font-jost">
              <div>
                <h3 class="font-playfair text-xl font-bold text-dark">Guest Reviews</h3>
                <div class="flex items-center gap-2 mt-1.5">
                  <!-- Star displays -->
                  <div class="flex items-center text-sun text-base filter drop-shadow-[0_0_2px_rgba(232,130,10,0.3)] gap-0.5">
                    <span 
                      v-for="s in 5" 
                      :key="s"
                      class="relative inline-block w-4 h-4 select-none leading-none"
                    >
                      <span class="text-gold/20 absolute top-0 left-0">★</span>
                      <span 
                        class="absolute top-0 left-0 overflow-hidden text-sun"
                        :style="{ width: getStarDisplayWidth(s, averageRating) }"
                      >★</span>
                    </span>
                  </div>
                  <span class="text-xs font-bold text-dark/75">{{ averageRating }} / 5</span>
                  <span class="text-[10px] text-muted">({{ reviews.length }} reviews)</span>
                </div>
              </div>
              
              <!-- Review Action Link -->
              <button 
                @click="navigateToFeedback" 
                class="text-[10px] text-sea hover:text-sea-light font-bold uppercase tracking-wider underline focus:outline-none transition-colors"
              >
                Write Review
              </button>
            </div>

            <!-- Review Cards List -->
            <div v-if="loadingReviews" class="py-12 text-center text-xs text-muted animate-pulse font-jost">
              Retrieving guests feedback...
            </div>
            
            <div v-else-if="reviews.length === 0" class="py-12 text-center font-jost">
              <p class="text-sm text-muted italic">No reviews yet for this adventure.</p>
              <button @click="navigateToFeedback" class="mt-4 text-xs font-bold text-gold uppercase tracking-wider border border-gold/40 px-4 py-2 rounded-full hover:bg-gold/10 transition-all">
                Be the first to review
              </button>
            </div>

            <!-- Review Carousel -->
            <div v-else class="relative min-h-[220px] flex flex-col justify-between">
              <!-- Direction-aware Slide Transition -->
              <div class="relative overflow-hidden flex-1 min-h-[160px] flex items-stretch">
                <Transition :name="transitionName" mode="out-in">
                  <div 
                    :key="reviews[currentIndex].id"
                    class="review-carousel-card"
                  >
                    <div>
                      <div class="flex justify-between items-start mb-3">
                        <div class="font-jost">
                          <h4 class="text-xs font-bold text-dark tracking-wide uppercase">{{ reviews[currentIndex].customerName }}</h4>
                          <!-- Date -->
                          <span class="text-[9px] text-muted">{{ formatDate(reviews[currentIndex].createdAt) }}</span>
                        </div>
                        
                        <!-- Stars -->
                        <div class="flex items-center text-sun text-sm filter drop-shadow-[0_0_2px_rgba(232,130,10,0.3)] gap-0.5">
                          <span 
                            v-for="s in 5" 
                            :key="s"
                            class="relative inline-block w-3.5 h-3.5 select-none leading-none"
                          >
                            <span class="text-gold/20 absolute top-0 left-0">★</span>
                            <span 
                              class="absolute top-0 left-0 overflow-hidden text-sun"
                              :style="{ width: getStarDisplayWidth(s, reviews[currentIndex].rating) }"
                            >★</span>
                          </span>
                        </div>
                      </div>
                      
                      <p class="font-cormorant text-sm text-text leading-relaxed italic">
                        "{{ reviews[currentIndex].comment }}"
                      </p>
                    </div>
                  </div>
                </Transition>
              </div>

              <!-- Carousel Controls -->
              <div v-if="reviews.length > 1" class="flex justify-between items-center mt-4 px-1">
                <!-- Dot indicators -->
                <div class="flex gap-1.5">
                  <button
                    v-for="(rev, idx) in reviews"
                    :key="'dot-' + rev.id"
                    @click="setReview(idx)"
                    class="w-1.5 h-1.5 rounded-full transition-all duration-300"
                    :class="idx === currentIndex ? 'bg-gold w-3' : 'bg-gold/30 hover:bg-gold/60'"
                    :aria-label="'Go to review ' + (idx + 1)"
                  ></button>
                </div>

                <!-- Arrow Nav -->
                <div class="flex gap-2">
                  <button 
                    @click="prevReview"
                    class="w-7 h-7 rounded-full border border-gold/30 hover:border-gold hover:text-white bg-white hover:bg-gold flex items-center justify-center text-gold transition-all duration-300 text-xs font-bold"
                    aria-label="Previous review"
                  >
                    ←
                  </button>
                  <button 
                    @click="nextReview"
                    class="w-7 h-7 rounded-full border border-gold/30 hover:border-gold hover:text-white bg-white hover:bg-gold flex items-center justify-center text-gold transition-all duration-300 text-xs font-bold"
                    aria-label="Next review"
                  >
                    →
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Bottom Notice -->
          <div class="pt-6 mt-6 border-t border-gold/10 flex justify-between items-center text-[10px] text-muted tracking-wider uppercase font-semibold font-jost">
            <span>Verified Luxury Bookings</span>
            <span>SeeDora Experiences</span>
          </div>
        </div>

      </div>
    </div>
  </Transition>
</template>

<style scoped>
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

/* Carousel Transitions */
.slide-next-enter-active,
.slide-next-leave-active,
.slide-prev-enter-active,
.slide-prev-leave-active {
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.slide-next-enter-from {
  opacity: 0;
  transform: translateX(30px) scale(0.98);
}
.slide-next-leave-to {
  opacity: 0;
  transform: translateX(-30px) scale(0.98);
}

.slide-prev-enter-from {
  opacity: 0;
  transform: translateX(-30px) scale(0.98);
}
.slide-prev-leave-to {
  opacity: 0;
  transform: translateX(30px) scale(0.98);
}

/* Custom Scrollbar for review section */
::-webkit-scrollbar {
  width: 5px;
}
::-webkit-scrollbar-track {
  background: rgba(201, 168, 76, 0.05);
}
::-webkit-scrollbar-thumb {
  background: rgba(201, 168, 76, 0.25);
  border-radius: 4px;
}
::-webkit-scrollbar-thumb:hover {
  background: rgba(201, 168, 76, 0.45);
}

/* Luxurious Styling details matching Egypt theme */
.modal-wrapper {
  background-color: var(--cream);
  border: 1px solid rgba(201, 168, 76, 0.4);
  box-shadow: 0 25px 60px rgba(6, 28, 40, 0.45), 0 0 45px rgba(201, 168, 76, 0.18);
  border-radius: 16px;
  overflow: hidden;
  position: relative;
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 896px; /* max-w-4xl */
  max-height: 90vh;
}
@media (min-width: 768px) {
  .modal-wrapper {
    flex-direction: row;
  }
}

.modal-close-btn {
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 50;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: rgba(13, 31, 45, 0.65);
  border: 1px solid rgba(201, 168, 76, 0.25);
  color: var(--white);
  font-size: 18px;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
}
.modal-close-btn:hover {
  background: var(--sun);
  border-color: var(--sun-light);
  transform: rotate(90deg);
}

.modal-visuals-col {
  position: relative;
  background-color: var(--dark);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  overflow: hidden;
  min-height: 300px;
}
.modal-visuals-col::before {
  content: '';
  position: absolute;
  inset: 0;
  background: radial-gradient(ellipse 70% 70% at 50% 50%, rgba(10, 92, 138, 0.25) 0%, transparent 85%);
  pointer-events: none;
  z-index: 1;
}
@media (min-width: 768px) {
  .modal-visuals-col {
    width: 50%;
    min-height: 100%;
  }
}

.stat-box {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.09);
  border-radius: 12px;
  padding: 14px;
  backdrop-filter: blur(4px);
}
.stat-label {
  font-family: 'Jost', sans-serif;
  font-size: 9px;
  color: rgba(255, 255, 255, 0.4);
  display: block;
  text-transform: uppercase;
  letter-spacing: 0.15em;
  margin-bottom: 4px;
}
.stat-value {
  font-family: 'Jost', sans-serif;
  color: var(--white);
  font-weight: 600;
  font-size: 13px;
}

.booking-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 12px;
  padding: 16px;
  backdrop-filter: blur(8px);
}
.rate-from {
  font-size: 9px;
  color: rgba(255, 255, 255, 0.45);
  letter-spacing: 0.1em;
  text-transform: uppercase;
}
.rate-amount {
  font-size: 26px;
  color: var(--sun-light);
  font-weight: bold;
}
.rate-per {
  font-size: 9px;
  color: rgba(255, 255, 255, 0.45);
}
.reserve-btn {
  background: linear-gradient(135deg, var(--sun), var(--sun-light));
  color: var(--white);
  border: none;
  padding: 12px 22px;
  border-radius: 8px;
  font-family: 'Jost', sans-serif;
  font-size: 11px;
  font-weight: bold;
  text-transform: uppercase;
  letter-spacing: 0.12em;
  cursor: pointer;
  box-shadow: 0 10px 25px rgba(232, 130, 10, 0.3);
  transition: all 0.3s ease;
}
.reserve-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 15px 30px rgba(232, 130, 10, 0.45);
  background: linear-gradient(135deg, var(--sun-light), var(--sun));
}

.modal-reviews-col {
  padding: 32px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  background-color: var(--cream);
  overflow-y: auto;
}
@media (min-width: 768px) {
  .modal-reviews-col {
    width: 50%;
    max-height: 100%;
  }
}
.reviews-header-row {
  border-bottom: 1px solid rgba(201, 168, 76, 0.2);
  padding-bottom: 16px;
  margin-bottom: 24px;
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
}
.review-carousel-card {
  width: 100%;
  background: linear-gradient(to bottom, #ffffff, var(--cream));
  border: 1px solid rgba(201, 168, 76, 0.16);
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 10px 25px rgba(13, 31, 45, 0.03);
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}
.review-carousel-card:hover {
  box-shadow: 0 15px 35px rgba(201, 168, 76, 0.08);
  border-color: rgba(201, 168, 76, 0.3);
}

/* Fonts configurations */
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
