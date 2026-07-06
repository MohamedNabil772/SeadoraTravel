<script setup lang="ts">
import { ref, computed, onMounted, watch, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const tour = ref<any>(null)
const loading = ref(true)
const categoryName = ref('Adventure')
const tourDestinationName = ref('')
const reviews = ref<any[]>([])
const loadingReviews = ref(false)

const routeSlug = computed(() => route.params.slug as string)

const getSlug = (name: string) => {
  return name
    .toLowerCase()
    .replace(/[^\w\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
    .trim()
}

const fetchTourData = async () => {
  const currentSlug = routeSlug.value
  if (!currentSlug) return
  
  loading.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const res = await fetch(`${API_URL}/api/content/api/tours`)
    if (res.ok) {
      const tours = await res.json()
      // Find the tour matching the slug of its English name
      tour.value = tours.find((t: any) => getSlug(t.names?.['en'] || '') === currentSlug)
      
      // Fetch category and destination
      if (tour.value) {
        const catRes = await fetch(`${API_URL}/api/content/api/categories`)
        if (catRes.ok) {
          const categories = await catRes.json()
          const cat = categories.find((c: any) => c.id === tour.value.categoryId)
          if (cat) {
            categoryName.value = cat.names?.['en'] || 'Adventure'
          }
        }
        
        try {
          const destRes = await fetch(`${API_URL}/api/content/api/destinations`)
          if (destRes.ok) {
            const destinations = await destRes.json()
            const dest = destinations.find((d: any) => d.id === tour.value.destinationId)
            if (dest) {
              tourDestinationName.value = dest.names?.['en'] || ''
            }
          }
        } catch (destErr) {
          console.error('Failed to fetch destination details:', destErr)
        }

        await fetchReviews()
      }
    }
  } catch (e) {
    console.error('Failed to fetch tour details:', e)
  } finally {
    loading.value = false
  }
}

// Watch for routeSlug changes to support browser navigation (back/forward)
watch(routeSlug, (newSlug) => {
  if (newSlug) {
    fetchTourData()
  }
})

// Generate dynamic gallery content reflecting the tour theme with premium free imagery
const galleryImages = computed(() => {
  if (!tour.value) return []
  
  const category = categoryName.value.toLowerCase()
  if (category.includes('sea') || category.includes('div')) {
    return [
      { 
        title: 'Coral Reef Sanctuary', 
        tag: 'Diving Spot', 
        url: 'https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=1200&q=80',
        desc: 'Explore the vibrant marine ecosystems and coral reefs teeming with exotic sea life in the crystal clear Red Sea waters.'
      },
      { 
        title: 'Red Sea Marine Life', 
        tag: 'Dolphin Reef', 
        url: 'https://images.unsplash.com/photo-1570481662006-a3a1374699e8?auto=format&fit=crop&w=1200&q=80',
        desc: 'Swim alongside pods of wild spinner dolphins in their natural coastal sanctuaries.'
      },
      { 
        title: 'Sunken Shipwreck Expedition', 
        tag: 'Deep Dive', 
        url: 'https://images.unsplash.com/photo-1682687220063-4742bd7fd538?auto=format&fit=crop&w=1200&q=80',
        desc: 'Uncover the mysteries of historic shipwrecks lying silently on the seafloor.'
      },
      { 
        title: 'Snorkeling Coastline', 
        tag: 'Shallow Waters', 
        url: 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=1200&q=80',
        desc: 'Enjoy relaxing snorkeling sessions in warm, shallow coastal waters over fine golden sands.'
      }
    ]
  } else if (category.includes('cultur') || category.includes('histor') || category.includes('temple') || category.includes('pyramid')) {
    return [
      { 
        title: 'Luxor Temple Columns', 
        tag: 'Ancient Egypt', 
        url: 'https://images.unsplash.com/photo-1600577916048-804c9191e36c?auto=format&fit=crop&w=1200&q=80',
        desc: 'Walk through the towering colonnades and giant statues built by Pharaoh Amenhotep III.'
      },
      { 
        title: 'Giza Pyramids Sunset', 
        tag: 'Wonder of World', 
        url: 'https://images.unsplash.com/photo-1539650116574-8efeb43e2750?auto=format&fit=crop&w=1200&q=80',
        desc: 'Witness the iconic ancient tombs of Khufu, Khafre, and Menkaure silhouetted against a dramatic desert sunset.'
      },
      { 
        title: 'Valley of the Kings Tomb', 
        tag: 'Pharaoh Heritage', 
        url: 'https://images.unsplash.com/photo-1503177119275-0aa32b31d468?auto=format&fit=crop&w=1200&q=80',
        desc: 'Descend into beautifully painted tombs decorated with sacred hieroglyphs and astronomical guides for the afterlife.'
      },
      { 
        title: 'Nile Felucca Sailing', 
        tag: 'Sunset Cruise', 
        url: 'https://images.unsplash.com/photo-1547127796-06bb04e4b315?auto=format&fit=crop&w=1200&q=80',
        desc: 'Sail on a traditional wooden felucca boat, catching the cool evening breeze on the legendary Nile River.'
      }
    ]
  } else {
    // Adventure / Desert Safari
    return [
      { 
        title: 'Golden Sahara Dunes', 
        tag: 'Dune Trekking', 
        url: 'https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9?auto=format&fit=crop&w=1200&q=80',
        desc: 'Embark on a camel caravan trek across the endless waves of shifting sand dunes in the Eastern Sahara.'
      },
      { 
        title: 'Bedouin Oasis Camp', 
        tag: 'Night Stargazing', 
        url: 'https://images.unsplash.com/photo-1534447677768-be436bb09401?auto=format&fit=crop&w=1200&q=80',
        desc: 'Gather around a traditional campfire under a dome of countless stars in the clear desert night sky.'
      },
      { 
        title: 'Sunset ATV Quad Riding', 
        tag: 'Thrill Safari', 
        url: 'https://images.unsplash.com/photo-1542362567-b07eac790947?auto=format&fit=crop&w=1200&q=80',
        desc: 'Feel the adrenaline rush riding high-performance quad bikes through desert tracks and canyon floors.'
      },
      { 
        title: 'Valley of El-Hitan Rock', 
        tag: 'Natural Heritage', 
        url: 'https://images.unsplash.com/photo-1473580044384-7ba9967e16a0?auto=format&fit=crop&w=1200&q=80',
        desc: 'Discover prehistoric whale fossils and wind-carved sandstone formations in this unique UNESCO World Heritage valley.'
      }
    ]
  }
})

const amenitiesIcons: Record<string, string> = {
  yacht: '<path d="M2 19h20l-1-3H3l-1 3zm1.6-4h16.8L18 8H6L3.6 15zM12 2v6.4l5.3-2.6L12 2z" />',
  instructor: '<path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />',
  gear: '<path d="M12 2a10 10 0 0 0-10 10c0 5.52 4.48 10 10 10s10-4.48 10-10S17.52 2 12 2zm3.5 13.5c-.83 0-1.5-.67-1.5-1.5S14.67 12.5 15.5 12.5s1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm-7 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5z" />',
  transfer: '<path d="M18.92 6.01C18.72 5.42 18.16 5 17.5 5h-11c-.66 0-1.21.42-1.42.99L3 12v8c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1h12v1c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-8l-2.08-5.99zM6.5 16c-.83 0-1.5-.67-1.5-1.5S5.67 13 6.5 13s1.5.67 1.5 1.5S7.33 16 6.5 16zm11 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5z" />',
  lunch: '<path d="M11 9H9V2H7v7H5V2H3v7c0 2.12 1.66 3.84 3.75 3.97V22h2.5v-9.03C11.34 12.84 13 11.12 13 9V2h-2v7zm5-3v8h2.5v8H21V2c-2.76 0-5 2.24-5 4z" />',
  camera: '<path d="M9 2L7.17 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2h-3.17L15 2H9zm3 15c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5z" />',
  guide: '<path d="M20.5 3l-.16.03L15 5.1 9 3 3.36 4.9c-.21.07-.36.25-.36.48v15.12c0 .28.24.5.52.5l.16-.03L9 18.9l6 2.1 5.64-1.9c.21-.07.36-.25.36-.48V3.52c0-.28-.24-.5-.52-.5zM15 19l-6-2.11V5l6 2.11V19z" />',
  chauffeur: '<path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm0-10c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2z" />',
  tickets: '<path d="M22 10V6c0-1.1-.9-2-2-2H4c-1.1 0-2 .9-2 2v4c1.1 0 2 .9 2 2s-1 2-2 2v4c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2v-4c0-1.1-1-2-2-2s2-.9 2-2zm-9 7.5h-2v-2h2v2zm0-4.5h-2v-2h2v2zm0-4.5h-2v-2h2v2z" />',
  felucca: '<path d="M12 2v6.4l5.3-2.6L12 2zm-1 7.2L3.5 12h14.9L11 9.2zm-6 3.8c0 3.3 2.7 6 6 6s6-2.7 6-6H5z" />',
  dining: '<path d="M11 9H9V2H7v7H5V2H3v7c0 2.12 1.66 3.84 3.75 3.97V22h2.5v-9.03C11.34 12.84 13 11.12 13 9V2h-2v7zm5-3v8h2.5v8H21V2c-2.76 0-5 2.24-5 4z" />',
  photography: '<path d="M9 2L7.17 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2h-3.17L15 2H9zm3 15c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5z" />',
  atv: '<path d="M19 13h-2.07c-.42-2.62-2.7-4.6-5.43-4.6s-5.01 1.98-5.43 4.6H4c-.55 0-1 .45-1 1v2c0 .55.45 1 1 1h1.07c.42 2.62 2.7 4.6 5.43 4.6s5.01-1.98 5.43-4.6H19c.55 0 1-.45 1-1v-2c0-.55-.45-1-1-1zm-7.5 7c-1.93 0-3.5-1.57-3.5-3.5S9.57 13 11.5 13s3.5 1.57 3.5 3.5-1.57 3.5-3.5 3.5z" />',
  camp: '<path d="M12 2L1 21h22L12 2zm0 4.85L19.3 19H4.7L12 6.85z" />',
  stargazing: '<path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" />',
  camel: '<path d="M12 2c1.1 0 2 .9 2 2v2h4c1.1 0 2 .9 2 2v2c0 1.1-.9 2-2 2h-4v4h3c1.1 0 2 .9 2 2v2c0 1.1-.9 2-2 2H9c-1.1 0-2-.9-2-2v-2c0-1.1.9-2 2-2h3v-4H8c-1.1 0-2-.9-2-2V8c0-1.1.9-2 2-2h4V4c0-1.1.9-2 2-2z" />',
  bbq: '<path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z" />',
  safety: '<path d="M12 1L3 5v6c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V5l-9-4zm0 10.99h7c-.53 4.12-3.28 7.79-7 8.94V12H5V6.3l7-3.11v8.8z" />'
}

const popularAmenities = computed(() => {
  if (!tour.value) return []
  const category = categoryName.value.toLowerCase()
  if (category.includes('sea') || category.includes('div')) {
    return [
      { name: 'Private Yacht', icon: 'yacht' },
      { name: 'Certified Guide', icon: 'instructor' },
      { name: 'Snorkeling Gear', icon: 'gear' },
      { name: 'Hotel Transfer', icon: 'transfer' },
      { name: 'Seafood Lunch', icon: 'lunch' },
      { name: 'Underwater Camera', icon: 'camera' }
    ]
  } else if (category.includes('cultur') || category.includes('histor') || category.includes('temple') || category.includes('pyramid')) {
    return [
      { name: 'Historian Guide', icon: 'guide' },
      { name: 'A/C Chauffeur', icon: 'chauffeur' },
      { name: 'Entrance Tickets', icon: 'tickets' },
      { name: 'Felucca Ride', icon: 'felucca' },
      { name: 'Gourmet Dining', icon: 'dining' },
      { name: 'Bespoke Photos', icon: 'photography' }
    ]
  } else {
    return [
      { name: 'Quad ATV Ride', icon: 'atv' },
      { name: 'Bedouin Camp', icon: 'camp' },
      { name: 'Stargazing Guide', icon: 'stargazing' },
      { name: 'Camel Caravan', icon: 'camel' },
      { name: 'BBQ Banquet', icon: 'bbq' },
      { name: 'Safety Equipment', icon: 'safety' }
    ]
  }
})

const getInclusionIconKey = (name: string): string => {
  const n = name.toLowerCase()
  if (n.includes('boat') || n.includes('yacht') || n.includes('felucca') || n.includes('sailing')) return 'yacht'
  if (n.includes('guide') || n.includes('instructor') || n.includes('concierge') || n.includes('historian')) return 'instructor'
  if (n.includes('gear') || n.includes('safety') || n.includes('equipment')) return 'safety'
  if (n.includes('transfer') || n.includes('chauffeur') || n.includes('car') || n.includes('ride') || n.includes('atv')) {
    if (n.includes('atv') || n.includes('quad')) return 'atv'
    return 'chauffeur'
  }
  if (n.includes('lunch') || n.includes('dining') || n.includes('bbq') || n.includes('banquet') || n.includes('breakfast') || n.includes('bar')) return 'dining'
  if (n.includes('camera') || n.includes('photograph') || n.includes('photo')) return 'photography'
  if (n.includes('ticket') || n.includes('entrance')) return 'tickets'
  if (n.includes('camp') || n.includes('oasis')) return 'camp'
  if (n.includes('star') || n.includes('sky')) return 'stargazing'
  if (n.includes('camel') || n.includes('caravan')) return 'camel'
  return 'safety' // fallback icon
}

const allInclusions = computed(() => {
  if (!tour.value) return []
  const base = Array.isArray(tour.value.includes) ? [...tour.value.includes] : []
  const items = [...base, 'Five-Star Private Concierge', 'Luxury Chauffeur Service']
  return items.map(name => ({
    name,
    icon: getInclusionIconKey(name)
  }))
})

const topInclusions = computed(() => {
  if (!tour.value) return []
  const base = Array.isArray(tour.value.includes) ? tour.value.includes.slice(0, 3) : []
  return base.map((name: string) => ({
    name,
    icon: getInclusionIconKey(name)
  }))
})

// Gallery Lightbox Carousel State & Navigation
const showLightbox = ref(false)
const lightboxIndex = ref(0)

const openLightbox = (index: number) => {
  lightboxIndex.value = index
  showLightbox.value = true
}

const closeLightbox = () => {
  showLightbox.value = false
}

const handleKeydown = (e: KeyboardEvent) => {
  if (!showLightbox.value) return
  if (e.key === 'Escape') {
    closeLightbox()
  } else if (e.key === 'ArrowRight') {
    nextLightboxImage()
  } else if (e.key === 'ArrowLeft') {
    prevLightboxImage()
  }
}

watch(showLightbox, (value) => {
  if (value) {
    window.addEventListener('keydown', handleKeydown)
  } else {
    window.removeEventListener('keydown', handleKeydown)
  }
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown)
})

const nextLightboxImage = () => {
  lightboxIndex.value = (lightboxIndex.value + 1) % galleryImages.value.length
}

const prevLightboxImage = () => {
  lightboxIndex.value = (lightboxIndex.value - 1 + galleryImages.value.length) % galleryImages.value.length
}

// Guest Reviews Carousel
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

const averageRating = computed(() => {
  if (reviews.value.length === 0) return 5
  const sum = reviews.value.reduce((acc, r) => acc + r.rating, 0)
  return Math.round((sum / reviews.value.length) * 10) / 10
})

const fetchReviews = async () => {
  if (!tour.value) return
  loadingReviews.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const res = await fetch(`${API_URL}/api/booking/api/feedbacks?tourId=${tour.value.id}`)
    if (res.ok) {
      const data = await res.json()
      if (data && data.length > 0) {
        reviews.value = data
      } else {
        reviews.value = getMockReviews(tour.value.id)
      }
    } else {
      reviews.value = getMockReviews(tour.value.id)
    }
  } catch (e) {
    reviews.value = getMockReviews(tour.value.id)
  } finally {
    loadingReviews.value = false
  }
}

const getMockReviews = (id: string): any[] => {
  const reviewsPool: Record<string, any[]> = {
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

  const defaultReviews = [
    { id: 'd1', customerName: 'Valerie Laurent', rating: 5, comment: 'Absolutely breathtaking! Seadora Travel provided a flawless, ultra-premium experience from start to finish.', createdAt: '2026-06-12' },
    { id: 'd2', customerName: 'James Sinclair', rating: 4, comment: 'Stunning scenery, professional staff, and superb coordination. True luxury in the heart of Egypt.', createdAt: '2026-06-15' }
  ]

  const numericId = id.replace(/\D/g, '') || '1'
  const key = Object.keys(reviewsPool).includes(numericId) ? numericId : '1'
  return reviewsPool[key] || defaultReviews
}

// Review Modal (Write Review) Form Details
const showReviewModal = ref(false)
const hoverRating = ref(0)
const submittingReview = ref(false)
const showDiscardPrompt = ref(false)
const errors = ref<Record<string, string>>({})

const reviewForm = ref({
  name: '',
  email: '',
  rating: 0,
  comment: ''
})

const openReviewModal = () => {
  reviewForm.value = {
    name: '',
    email: '',
    rating: 0,
    comment: ''
  }
  errors.value = {}
  showDiscardPrompt.value = false
  showReviewModal.value = true
}

// Form Dirty Validation (checks if any user edits are present)
const isFormDirty = computed(() => {
  const f = reviewForm.value
  return f.name.trim() !== '' || f.email.trim() !== '' || f.comment.trim() !== '' || f.rating > 0
})

const handleReviewModalCloseAttempt = () => {
  if (isFormDirty.value) {
    // Show validation dialog/warning box inside modal
    showDiscardPrompt.value = true
  } else {
    // Clean close instantly
    showReviewModal.value = false
  }
}

const closeReviewModalAndDiscard = () => {
  showDiscardPrompt.value = false
  showReviewModal.value = false
  reviewForm.value = {
    name: '',
    email: '',
    rating: 0,
    comment: ''
  }
}

const submitReview = async () => {
  const newErrors: Record<string, string> = {}
  if (!reviewForm.value.name.trim()) newErrors.name = 'Please enter your name.'
  if (!reviewForm.value.email.trim()) {
    newErrors.email = 'Please enter your email.'
  } else if (!/\S+@\S+\.\S+/.test(reviewForm.value.email)) {
    newErrors.email = 'Please enter a valid email address.'
  }
  if (reviewForm.value.rating === 0) newErrors.rating = 'Please select a star rating.'

  if (Object.keys(newErrors).length > 0) {
    errors.value = newErrors
    return
  }

  submittingReview.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const payload = {
      tourId: tour.value.id,
      rating: reviewForm.value.rating,
      comment: reviewForm.value.comment,
      customerName: reviewForm.value.name,
      customerEmail: reviewForm.value.email
    }

    const res = await fetch(`${API_URL}/api/booking/api/feedbacks`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })

    if (!res.ok) {
      const errorText = await res.text()
      throw new Error(errorText || 'Failed to submit review')
    }

    showReviewModal.value = false
    await fetchReviews()
  } catch (e: any) {
    console.error(e)
    errors.value = { form: e.message || 'Could not submit your review. Please try again.' }
  } finally {
    submittingReview.value = false
  }
}

// Booking Modal Logic
const showBookingModal = ref(false)
const bookingLoading = ref(false)
const bookingSuccess = ref(false)
const bookingReference = ref('')

const bookingForm = ref({
  name: '',
  email: '',
  phone: '',
  destination: '',
  date: '',
  guests: '2',
  notes: ''
})

const bookingErrors = ref({
  name: '',
  email: '',
  phone: '',
  destination: '',
  date: '',
  guests: ''
})

const mapDestinationToValue = (destName: string) => {
  const name = destName.toLowerCase()
  if (name.includes('hurghada')) return 'hurghada'
  if (name.includes('cairo')) return 'cairo'
  if (name.includes('luxor')) return 'luxor'
  if (name.includes('sharm')) return 'sharm'
  return ''
}

const openBookingModal = () => {
  const destVal = tour.value ? mapDestinationToValue(tourDestinationName.value || tour.value.names?.['en'] || '') : ''
  bookingForm.value = {
    name: '',
    email: '',
    phone: '',
    destination: destVal,
    date: '',
    guests: '2',
    notes: ''
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
  if (!tour.value) return
  
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
        tourId: tour.value.id,
        customerName: bookingForm.value.name.trim(),
        customerEmail: bookingForm.value.email.trim()
      })
    })
    
    if (response.ok) {
      bookingReference.value = generateReferenceCode()
      bookingSuccess.value = true
    } else {
      const errText = await response.text()
      console.error("Booking failed:", errText)
      alert("Booking failed. " + (errText || "Please try again."))
    }
  } catch (error) {
    console.error("Booking error:", error)
    alert("Booking error. Please check your connection and try again.")
  } finally {
    bookingLoading.value = false
  }
}

// Global Helper Methods
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

const getStarWidth = (star: number, currentRating: number) => {
  if (currentRating >= star) return '100%'
  if (currentRating === star - 0.5) return '50%'
  return '0%'
}

onMounted(() => {
  fetchTourData()
})
</script>

<template>
  <div class="tour-details-page min-h-screen bg-cream text-dark flex flex-col font-sans relative overflow-hidden">
    <!-- Pharaonic Background Grid Overlay -->
    <div class="absolute inset-0 bg-[radial-gradient(ellipse_at_top,_var(--sea-deep)_0%,_transparent_70%)] opacity-35 pointer-events-none"></div>
    <div class="absolute -top-40 -left-40 w-96 h-96 rounded-full bg-sun opacity-10 blur-3xl pointer-events-none"></div>
    <div class="absolute -bottom-40 -right-40 w-96 h-96 rounded-full bg-sea-light opacity-10 blur-3xl pointer-events-none"></div>

    <!-- Header/Navigation -->
    <header class="w-full py-5 px-8 md:px-16 border-b border-gold/25 flex justify-between items-center bg-sea-deep/95 backdrop-blur-md relative z-10">
      <router-link to="/" class="flex items-center gap-3 no-underline">
        <div class="logo-icon w-9 h-9 bg-gradient-to-br from-sun to-sun-light rounded-full flex items-center justify-center text-lg text-white">🌊</div>
        <div class="leading-none">
          <span class="font-playfair text-base font-bold text-white tracking-wide block">SeeDora Travel</span>
          <span class="text-[8px] text-gold tracking-widest uppercase font-semibold">Egypt · Luxury Experiences</span>
        </div>
      </router-link>
      <router-link to="/" class="text-[10px] text-sun-light uppercase tracking-widest hover:text-white font-semibold transition-colors duration-300 flex items-center">
        <svg class="w-3 h-3 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
        </svg>
        Back to Adventures
      </router-link>
    </header>

    <!-- Main Content Loader -->
    <main v-if="loading" class="flex-1 flex items-center justify-center py-24 text-gold animate-pulse">
      <div class="text-center">
        <span class="text-3xl block mb-2">🐪</span>
        <span class="text-xs uppercase tracking-widest font-bold">Unveiling experience details...</span>
      </div>
    </main>

    <div v-else-if="!tour" class="flex-1 flex items-center justify-center py-24 text-center">
      <div>
        <span class="text-4xl block mb-4">🏜️</span>
        <h3 class="font-playfair text-xl font-bold mb-2">Adventure Not Found</h3>
        <p class="text-xs text-muted mb-6">The requested luxury experience could not be located.</p>
        <router-link to="/" class="px-6 py-2.5 bg-sea text-white text-xs font-bold uppercase tracking-wider rounded-full hover:bg-sea-light transition-all">Go Home</router-link>
      </div>
    </div>

    <!-- Page Details Layout -->
    <div v-else class="flex-1 relative z-10 flex flex-col">
      <!-- 1. Top Section: Full Details -->
      <section class="details-top-hero relative bg-dark overflow-hidden flex flex-col md:flex-row min-h-[550px]">
        <!-- Immersive background photo on the left half -->
        <div 
          v-if="galleryImages && galleryImages.length > 0"
          class="absolute inset-y-0 left-0 md:w-1/2 bg-cover bg-center hidden md:block"
          :style="{ backgroundImage: `url(${galleryImages[0].url})` }"
        ></div>
        <div class="absolute inset-y-0 left-0 md:w-1/2 bg-gradient-to-r from-dark/95 via-dark/85 to-dark/40 hidden md:block"></div>

        <div class="md:w-1/2 p-8 md:p-16 flex flex-col justify-center relative z-10 min-h-[350px] md:min-h-0 bg-dark/60 md:bg-transparent">
          <div 
            v-if="galleryImages && galleryImages.length > 0"
            class="absolute inset-0 bg-cover bg-center md:hidden"
            :style="{ backgroundImage: `url(${galleryImages[0].url})` }"
          ></div>
          <div class="absolute inset-0 bg-gradient-to-r from-dark/95 to-dark/70 md:hidden"></div>

          <div class="relative z-10">
            <span class="inline-block text-[10px] tracking-[0.25em] text-gold font-bold uppercase mb-3 font-jost">
              {{ categoryName }} · Egypt
            </span>
            <h1 class="font-playfair text-4xl md:text-5xl lg:text-6xl font-bold text-white leading-tight mb-6">
              {{ tour.names?.['en'] }}
            </h1>
            <p class="text-white/80 text-sm md:text-base leading-relaxed mb-8 max-w-lg font-jost">
              {{ tour.descriptions?.['en'] }}
            </p>
          </div>
        </div>

        <div class="w-full md:w-1/2 p-8 md:p-14 flex flex-col justify-between relative z-10 bg-black/40 backdrop-blur-lg border-l border-white/10 min-h-[600px] md:min-h-0">
          <div class="w-full flex-1 flex flex-col justify-between space-y-8">
            
            <!-- Luxury Experience Header Badge -->
            <div class="flex items-center justify-between border-b border-white/10 pb-5">
              <div class="flex items-center gap-2">
                <span class="bg-gold/15 text-gold px-2.5 py-1 rounded-full text-[9px] uppercase font-bold tracking-widest border border-gold/30">Verified Partner</span>
                <span class="text-[10px] text-white/60 tracking-wider">Luxury Experience · 5.0 Rating</span>
              </div>
              <div class="flex items-center text-gold text-xs filter drop-shadow-[0_0_4px_rgba(201,168,76,0.35)] gap-0.5">
                <svg v-for="i in 5" :key="i" class="w-3.5 h-3.5 text-gold" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                </svg>
              </div>
            </div>

            <!-- Details Section -->
            <div>
              <h3 class="text-xs uppercase tracking-[0.2em] text-gold font-bold font-jost mb-5">Exclusive Experience Details</h3>
              
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
                <!-- 1. Duration -->
                <div class="flex items-center gap-4 bg-white/5 border border-white/10 rounded-xl p-4.5 hover:bg-white/10 transition-all duration-300 hover:border-gold/30">
                  <div class="p-2.5 bg-white/5 border border-white/10 rounded-lg text-gold flex-shrink-0">
                    <svg class="w-5.5 h-5.5 text-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </div>
                  <div>
                    <span class="text-[9px] uppercase tracking-widest text-gold/60 block">Duration</span>
                    <span class="text-base font-bold text-white tracking-wide mt-1 block">{{ tour.duration }}</span>
                  </div>
                </div>

                <!-- 2. Services Included -->
                <div class="flex items-start gap-4 bg-white/5 border border-white/10 rounded-xl p-4.5 hover:bg-white/10 transition-all duration-300 hover:border-gold/30">
                  <div class="p-2.5 bg-white/5 border border-white/10 rounded-lg text-gold flex-shrink-0 mt-0.5">
                    <svg class="w-5.5 h-5.5 text-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 002-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
                    </svg>
                  </div>
                  <div class="flex-1 min-w-0">
                    <span class="text-[9px] uppercase tracking-widest text-gold/60 block mb-2">Inclusions</span>
                    <ul class="space-y-1.5">
                      <li v-for="inc in topInclusions" :key="inc.name" class="flex items-center gap-2.5 text-xs text-white/90 font-jost">
                        <svg class="w-4 h-4 text-gold flex-shrink-0" fill="currentColor" viewBox="0 0 24 24" v-html="amenitiesIcons[inc.icon]"></svg>
                        <span class="truncate block font-semibold">{{ inc.name }}</span>
                      </li>
                      <li v-if="tour.includes?.length > 3" class="text-[9px] text-gold/80 italic font-semibold pl-6 mt-1">
                        +{{ tour.includes.length - 3 }} more details below
                      </li>
                    </ul>
                  </div>
                </div>

                <!-- 3. Tour Type -->
                <div class="flex items-center gap-4 bg-white/5 border border-white/10 rounded-xl p-4.5 hover:bg-white/10 transition-all duration-300 hover:border-gold/30">
                  <div class="p-2.5 bg-white/5 border border-white/10 rounded-lg text-gold flex-shrink-0">
                    <svg class="w-5.5 h-5.5 text-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                  </div>
                  <div>
                    <span class="text-[9px] uppercase tracking-widest text-gold/60 block">Experience</span>
                    <span class="text-base font-bold text-white tracking-wide mt-1 block">Private Tour</span>
                  </div>
                </div>

                <!-- 4. Flexibility -->
                <div class="flex items-center gap-4 bg-white/5 border border-white/10 rounded-xl p-4.5 hover:bg-white/10 transition-all duration-300 hover:border-gold/30">
                  <div class="p-2.5 bg-white/5 border border-white/10 rounded-lg text-gold flex-shrink-0">
                    <svg class="w-5.5 h-5.5 text-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                    </svg>
                  </div>
                  <div>
                    <span class="text-[9px] uppercase tracking-widest text-gold/60 block">Flexibility</span>
                    <span class="text-base font-bold text-white tracking-wide mt-1 block">Free Cancellation</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Booking.com / Airbnb style Popular Amenities & Services -->
            <div class="border-t border-white/10 pt-6">
              <h3 class="text-xs uppercase tracking-[0.2em] text-gold font-bold font-jost mb-4 flex items-center gap-2">
                <svg class="w-4 h-4 text-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
                </svg>
                Most Popular Amenities & Services
              </h3>
              <div class="grid grid-cols-2 gap-3">
                <div 
                  v-for="amenity in popularAmenities" 
                  :key="amenity.name"
                  class="flex items-center gap-3 bg-white/5 border border-white/10 rounded-xl py-3 px-4 hover:bg-white/10 transition-all duration-300 hover:border-gold/25"
                >
                  <svg class="w-4.5 h-4.5 text-gold flex-shrink-0" fill="currentColor" viewBox="0 0 24 24" v-html="amenitiesIcons[amenity.icon]"></svg>
                  <span class="text-xs text-white/90 font-jost font-semibold tracking-wide">{{ amenity.name }}</span>
                </div>
              </div>
            </div>

            <!-- Inclusions list -->
            <div class="border-t border-white/10 pt-6">
              <h3 class="text-xs uppercase tracking-[0.2em] text-gold font-bold font-jost mb-4 flex items-center gap-2">
                <svg class="w-4.5 h-4.5 text-gold flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 002-2h2a2 2 0 012 2" />
                </svg>
                Premium Package Inclusions
              </h3>
              <div class="grid grid-cols-2 gap-3">
                <div 
                  v-for="inc in allInclusions" 
                  :key="inc.name"
                  class="flex items-center gap-3 bg-white/5 border border-white/10 rounded-xl py-3 px-4 hover:bg-white/10 transition-all duration-300 hover:border-gold/25"
                >
                  <svg class="w-4.5 h-4.5 text-gold flex-shrink-0" fill="currentColor" viewBox="0 0 24 24" v-html="amenitiesIcons[inc.icon]"></svg>
                  <span class="text-xs text-white/90 font-jost font-semibold tracking-wide">{{ inc.name }}</span>
                </div>
              </div>
            </div>

            <!-- Booking Rate Row -->
            <div class="booking-row bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md flex justify-between items-center hover:border-gold/30 transition-all duration-300">
              <div class="text-left font-jost">
                <div class="rate-from text-gold/60 text-[9px] uppercase tracking-widest">Private Tour Rate</div>
                <div class="rate-amount text-3xl text-gold font-bold leading-tight">€{{ tour.price }}</div>
                <div class="rate-per text-[9px] text-white/50">/ person</div>
              </div>
              <button 
                @click="openBookingModal" 
                class="reserve-btn bg-gradient-to-r from-gold to-[#e5c158] hover:from-[#e5c158] hover:to-gold text-dark border-none py-4 px-8 rounded-lg font-jost font-bold uppercase tracking-widest text-[11px] cursor-pointer shadow-lg hover:shadow-gold/20 transition-all duration-300 hover:translate-y-[-2px] active:translate-y-0"
              >
                Reserve Now
              </button>
            </div>
          </div>
        </div>
      </section>      <!-- Visual Divider with Signature ornament (Flex based, clean and robust) -->
      <div class="py-16 md:py-24 bg-cream flex items-center justify-center px-8 md:px-16 border-t border-gold/10">
        <div class="w-full max-w-5xl flex items-center justify-center gap-6">
          <div class="flex-1 border-t border-gold/20"></div>
          <div class="flex items-center gap-3 text-gold">
            <span class="text-gold/40">✥</span>
            <span class="text-[10px] uppercase tracking-[0.45em] font-jost text-gold/80 font-bold whitespace-nowrap">SeeDora Journeys</span>
            <span class="text-gold/40">✥</span>
          </div>
          <div class="flex-1 border-t border-gold/20"></div>
        </div>
      </div>

      <!-- 2. Interactive Photo Gallery -->
      <section class="py-24 md:py-36 px-8 md:px-16 bg-[#faf8f5]/60 relative">
        <div class="section-header text-center mb-16 md:mb-24">
          <span class="text-[10px] tracking-[0.25em] uppercase text-gold font-bold mb-3 block font-jost">Visual Journeys</span>
          <h2 class="font-playfair text-3xl md:text-4xl font-bold text-dark">Experience Highlights Gallery</h2>
        </div>

        <div class="gallery-grid">
          <div 
            v-for="(img, idx) in galleryImages" 
            :key="idx" 
            class="gallery-card group"
            @click="openLightbox(idx)"
          >
            <div 
              class="gallery-img-bg bg-cover bg-center absolute inset-0 transition-transform duration-700 ease-out group-hover:scale-110"
              :style="{ backgroundImage: `url(${img.url})` }"
            ></div>
            <div class="gallery-overlay"></div>
            <div class="gallery-info font-jost">
              <span class="gallery-tag">{{ img.tag }}</span>
              <h4 class="gallery-title font-playfair">{{ img.title }}</h4>
            </div>
            <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-300 bg-dark/40 backdrop-blur-[2px]">
              <span class="px-4 py-2 border border-gold/60 text-gold text-[10px] uppercase tracking-widest font-bold bg-dark/70 rounded-full flex items-center gap-1.5 transform translate-y-2 group-hover:translate-y-0 transition-transform duration-300">
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
                View Photo
              </span>
            </div>
          </div>
        </div>
      </section>      <!-- Visual Divider with Signature ornament (Flex based, clean and robust) -->
      <div class="py-16 md:py-24 bg-cream flex items-center justify-center px-8 md:px-16 border-t border-gold/10">
        <div class="w-full max-w-5xl flex items-center justify-center gap-6">
          <div class="flex-1 border-t border-gold/20"></div>
          <div class="flex items-center gap-3 text-gold">
            <span class="text-gold/40">✥</span>
            <span class="text-[10px] uppercase tracking-[0.45em] font-jost text-gold/80 font-bold whitespace-nowrap">Guest Reflections</span>
            <span class="text-gold/40">✥</span>
          </div>
          <div class="flex-1 border-t border-gold/20"></div>
        </div>
      </div>

      <!-- 3. Reviews Carousel Section -->
      <section class="py-24 md:py-36 px-8 md:px-16 bg-gradient-to-b from-cream to-white">
        <!-- Centered Header -->
        <div class="section-header text-center mb-20 max-w-2xl mx-auto">
          <span class="text-[10px] tracking-[0.25em] uppercase text-gold font-bold mb-3 block font-jost">Guest Testimonials</span>
          <h2 class="font-playfair text-3xl md:text-4xl lg:text-5xl font-bold text-dark mb-4">Verified Guest Reflections</h2>
          <p class="font-cormorant text-base md:text-lg text-muted italic">What our esteemed travelers say about their luxury Egyptian odyssey.</p>
          <div class="w-16 h-[1px] bg-gold/30 mx-auto mt-6"></div>
        </div>

        <div class="max-w-6xl mx-auto flex flex-col md:flex-row gap-12 items-stretch">
          <!-- Plaque style average rating block -->
          <div class="md:w-5/12 flex flex-col justify-between p-10 bg-[#faf8f5] border border-gold/35 rounded-2xl shadow-sm text-center font-jost relative overflow-hidden">
            <div class="absolute top-0 inset-x-0 h-1 bg-gradient-to-r from-sea via-gold to-sun"></div>
            
            <div class="my-auto space-y-6">
              <span class="text-[9px] tracking-[0.25em] uppercase text-muted font-bold block">Overall Sentiment</span>
              
              <div class="space-y-2">
                <div class="flex items-baseline justify-center gap-1">
                  <span class="text-6xl font-serif font-bold text-dark tracking-tighter">{{ averageRating }}</span>
                  <span class="text-sm text-muted font-semibold">/ 5.0</span>
                </div>
                
                <div class="flex justify-center items-center text-gold text-xl filter drop-shadow-[0_0_4px_rgba(201,168,76,0.35)] gap-1">
                  <span 
                    v-for="s in 5" 
                    :key="s"
                    class="relative inline-block w-6 h-6 select-none"
                  >
                    <svg class="w-6 h-6 text-gold/20" fill="currentColor" viewBox="0 0 24 24">
                      <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                    </svg>
                    <span 
                      class="absolute top-0 left-0 overflow-hidden h-full flex items-center"
                      :style="{ width: getStarDisplayWidth(s, averageRating) }"
                    >
                      <svg class="w-6 h-6 text-gold" fill="currentColor" viewBox="0 0 24 24">
                        <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                      </svg>
                    </span>
                  </span>
                </div>
              </div>

              <p class="text-xs text-muted max-w-[200px] mx-auto leading-relaxed">Derived from {{ reviews.length }} authenticated guest reviews.</p>
            </div>

            <button 
              @click="openReviewModal" 
              class="mt-8 w-full bg-gradient-to-r from-sea-deep to-sea hover:from-sea hover:to-sea-deep text-white border-none py-3.5 px-6 rounded-lg text-xs font-bold uppercase tracking-widest transition-all duration-300 shadow-md hover:translate-y-[-1px] cursor-pointer"
            >
              Write Review
            </button>
          </div>

          <!-- Reviews Carousel Card -->
          <div class="md:w-7/12 flex flex-col justify-center">
            <div v-if="loadingReviews" class="py-12 text-center text-xs text-muted animate-pulse font-jost">
              Retrieving guests feedback...
            </div>
            
            <div v-else-if="reviews.length === 0" class="py-12 text-center font-jost bg-white/40 border border-gold/10 rounded-2xl p-8 flex flex-col items-center justify-center">
              <p class="text-sm text-muted italic">No reviews yet for this adventure.</p>
              <button @click="openReviewModal" class="mt-4 text-xs font-bold text-gold uppercase tracking-wider border border-gold/40 px-6 py-2.5 rounded-full hover:bg-gold/10 transition-all">
                Be the first to review
              </button>
            </div>

            <div v-else class="relative min-h-[280px] flex flex-col justify-between">
              <div class="relative overflow-hidden flex-1 min-h-[220px] flex items-stretch">
                <Transition :name="transitionName" mode="out-in">
                  <div 
                    :key="reviews[currentIndex].id"
                    class="review-carousel-card relative bg-white border border-gold/15 p-10 rounded-2xl shadow-[0_15px_40px_rgba(201,168,76,0.04)] overflow-hidden flex flex-col justify-between w-full"
                  >
                    <!-- Large decorative quote icon in background -->
                    <div class="absolute right-8 top-8 text-gold/10 pointer-events-none">
                      <svg class="w-16 h-16" fill="currentColor" viewBox="0 0 24 24">
                        <path d="M14.017 21v-7.391c0-5.704 3.731-9.57 8.983-10.609l.995 2.151c-2.432.917-4.795 2.851-4.795 6.3l.008.34h5.792V21H14.017zm-11.02 0v-7.391c0-5.704 3.748-9.57 9-10.609l.996 2.151c-2.433.917-4.796 2.851-4.796 6.3l.008.34h5.783V21H2.997z" />
                      </svg>
                    </div>

                    <div class="flex-1 flex flex-col justify-between z-10">
                      <div class="flex justify-between items-start mb-6">
                        <div class="font-jost">
                          <h4 class="text-xs font-bold text-dark tracking-wider uppercase">{{ reviews[currentIndex].customerName }}</h4>
                          <span class="text-[9px] text-muted">{{ formatDate(reviews[currentIndex].createdAt) }}</span>
                        </div>
                        
                        <div class="flex items-center text-gold text-sm filter drop-shadow-[0_0_2px_rgba(201,168,76,0.3)] gap-0.5">
                          <span 
                            v-for="s in 5" 
                            :key="s"
                            class="relative inline-block w-3.5 h-3.5 select-none"
                          >
                            <svg class="w-3.5 h-3.5 text-gold/20" fill="currentColor" viewBox="0 0 24 24">
                              <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                            </svg>
                            <span 
                              class="absolute top-0 left-0 overflow-hidden h-full flex items-center"
                              :style="{ width: getStarDisplayWidth(s, reviews[currentIndex].rating) }"
                            >
                              <svg class="w-3.5 h-3.5 text-gold" fill="currentColor" viewBox="0 0 24 24">
                                <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                              </svg>
                            </span>
                          </span>
                        </div>
                      </div>
                      
                      <p class="font-cormorant text-lg md:text-xl text-text leading-relaxed italic pr-8">
                        "{{ reviews[currentIndex].comment }}"
                      </p>
                    </div>
                  </div>
                </Transition>
              </div>

              <!-- Carousel Navigation Controls -->
              <div v-if="reviews.length > 1" class="flex justify-between items-center mt-6 px-1 z-10">
                <div class="flex gap-2">
                  <button
                    v-for="(rev, idx) in reviews"
                    :key="'dot-' + rev.id"
                    @click="setReview(idx)"
                    class="w-2 h-2 rounded-full transition-all duration-300"
                    :class="idx === currentIndex ? 'bg-gold w-4' : 'bg-gold/30 hover:bg-gold/60'"
                  ></button>
                </div>

                <div class="flex gap-2">
                  <button @click="prevReview" class="w-8 h-8 rounded-full border border-gold/30 hover:border-gold hover:text-white bg-white hover:bg-gold flex items-center justify-center text-gold transition-all duration-300" aria-label="Previous review">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M15 19l-7-7 7-7" />
                    </svg>
                  </button>
                  <button @click="nextReview" class="w-8 h-8 rounded-full border border-gold/30 hover:border-gold hover:text-white bg-white hover:bg-gold flex items-center justify-center text-gold transition-all duration-300" aria-label="Next review">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M9 5l7 7-7 7" />
                    </svg>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>

    <!-- 4. Review Modal (Write Review) -->
    <Transition name="fade">
      <div v-if="showReviewModal" class="fixed inset-0 z-[1500] flex items-center justify-center bg-dark/80 backdrop-blur-md p-4 overflow-y-auto" @click="handleReviewModalCloseAttempt">
        <div 
          class="feedback-card relative animate-slide-up"
          @click.stop
        >
          <!-- Top Decorator Gold Line -->
          <div class="h-1.5 w-full bg-gradient-to-r from-sea via-gold to-sun"></div>

          <!-- Close Icon button -->
          <button @click="handleReviewModalCloseAttempt" class="modal-close-btn" aria-label="Close modal">
            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
          
          <div class="p-8">
            <h3 class="font-playfair text-2xl font-bold text-dark text-center mb-6">Write Review</h3>
            
            <form @submit.prevent="submitReview" class="space-y-4">
              <!-- General form error alerts -->
              <div v-if="errors.form" class="bg-red-500/10 border border-red-500/30 text-red-600 rounded-lg p-3.5 text-xs font-semibold font-sans">
                {{ errors.form }}
              </div>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label for="reviewName" class="block text-[9px] font-bold text-muted mb-1.5 uppercase tracking-widest font-sans">Full Name</label>
                  <input id="reviewName" type="text" v-model="reviewForm.name" required class="w-full p-3 border border-gold/30 rounded-lg focus:ring-1 focus:ring-sea focus:border-sea bg-cream/35 focus:bg-white outline-none text-xs font-jost transition-all" placeholder="Enter your name">
                  <p v-if="errors.name" class="text-[10px] text-red-500 mt-1 font-jost">{{ errors.name }}</p>
                </div>
                <div>
                  <label for="reviewEmail" class="block text-[9px] font-bold text-muted mb-1.5 uppercase tracking-widest font-sans">Email Address</label>
                  <input id="reviewEmail" type="email" v-model="reviewForm.email" required class="w-full p-3 border border-gold/30 rounded-lg focus:ring-1 focus:ring-sea focus:border-sea bg-cream/35 focus:bg-white outline-none text-xs font-jost transition-all" placeholder="maria@example.com">
                  <p v-if="errors.email" class="text-[10px] text-red-500 mt-1 font-jost">{{ errors.email }}</p>
                </div>
              </div>

              <!-- Star selection zone -->
              <div class="bg-cream/45 border border-gold/15 rounded-xl p-5 text-center transition-all hover:border-gold/30">
                <label class="block text-[9px] font-bold text-muted mb-3 uppercase tracking-widest font-sans">Your Rating</label>
                <div class="flex justify-center items-center gap-3" @mouseleave="hoverRating = 0">
                  <button 
                    v-for="star in 5" 
                    :key="star"
                    type="button"
                    class="star-btn relative w-10 h-10 flex items-center justify-center focus:outline-none cursor-pointer group"
                  >
                    <!-- Left Half Hitbox -->
                    <div class="absolute top-0 left-0 w-1/2 h-full z-10" @click="reviewForm.rating = star - 0.5" @mouseover="hoverRating = star - 0.5"></div>
                    <!-- Right Half Hitbox -->
                    <div class="absolute top-0 right-0 w-1/2 h-full z-10" @click="reviewForm.rating = star" @mouseover="hoverRating = star"></div>
                    
                    <span class="relative w-10 h-10 transition-all duration-300 transform group-hover:scale-110 flex items-center justify-center pointer-events-none">
                      <!-- Empty Star Background -->
                      <svg class="w-10 h-10 text-gold/20" fill="currentColor" viewBox="0 0 24 24">
                        <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                      </svg>
                      <!-- Filled Star Overlay -->
                      <span 
                        class="absolute top-0 left-0 overflow-hidden h-full flex items-center transition-all duration-150"
                        :style="{ width: getStarWidth(star, hoverRating || reviewForm.rating) }"
                      >
                        <svg class="w-10 h-10 text-gold filter drop-shadow-[0_0_4px_rgba(201,168,76,0.6)]" fill="currentColor" viewBox="0 0 24 24">
                          <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                        </svg>
                      </span>
                    </span>
                  </button>
                </div>
                <span class="text-[10px] font-bold text-gold mt-3 block h-4 tracking-wide font-sans">
                  {{ reviewForm.rating === 5 ? 'Excellent & Luxury' : reviewForm.rating === 4.5 ? 'Spectacular Luxury' : reviewForm.rating === 4 ? 'Very Good' : reviewForm.rating === 3.5 ? 'Extremely Good' : reviewForm.rating === 3 ? 'Good' : reviewForm.rating === 2.5 ? 'Average' : reviewForm.rating === 2 ? 'Fair' : reviewForm.rating === 1.5 ? 'Mediocre' : reviewForm.rating === 1 ? 'Needs Improvement' : reviewForm.rating === 0.5 ? 'Dissatisfied' : 'Select your rating' }}
                </span>
                <p v-if="errors.rating" class="text-[10px] text-red-500 mt-1 font-jost">{{ errors.rating }}</p>
              </div>

              <div>
                <label for="reviewComments" class="block text-[9px] font-bold text-muted mb-1.5 uppercase tracking-widest font-sans">Your Experience (Optional)</label>
                <textarea id="reviewComments" v-model="reviewForm.comment" rows="3" class="w-full p-3 border border-gold/30 rounded-lg focus:ring-1 focus:ring-sea focus:border-sea bg-cream/35 focus:bg-white outline-none text-xs font-jost resize-none transition-all" placeholder="Tell us about the highlights of your luxury journey..."></textarea>
              </div>

              <button type="submit" :disabled="submittingReview" class="w-full bg-gradient-to-r from-sea-deep to-sea text-white py-3.5 px-6 rounded-lg font-bold text-xs uppercase tracking-widest transition-all shadow-md hover:translate-y-[-1px] disabled:opacity-50 mt-2 cursor-pointer">
                {{ submittingReview ? 'Submitting...' : 'Submit Feedback' }}
              </button>
            </form>

            <!-- Discard Prompt Dialog Overlay (Inside Review Form Modal) -->
            <Transition name="fade">
              <div v-if="showDiscardPrompt" class="absolute inset-0 bg-dark/95 backdrop-blur-md rounded-2xl p-8 flex flex-col justify-center items-center text-center z-[2000] font-sans">
                <svg class="w-12 h-12 text-sun mb-4 animate-bounce" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
                <h4 class="font-playfair text-xl font-bold text-white mb-2">Discard Unsaved Review?</h4>
                <p class="text-white/60 text-xs max-w-xs mb-8 leading-relaxed">You have started writing a review. Discarding will permanently delete your draft.</p>
                <div class="flex gap-4">
                  <button @click="closeReviewModalAndDiscard" class="px-6 py-2.5 bg-red-600 hover:bg-red-700 text-white font-bold text-xs uppercase tracking-widest rounded transition-all cursor-pointer">Discard Changes</button>
                  <button @click="showDiscardPrompt = false" class="px-6 py-2.5 border border-gold/50 text-gold hover:bg-gold/10 font-bold text-xs uppercase tracking-widest rounded transition-all cursor-pointer">Keep Writing</button>
                </div>
              </div>
            </Transition>

          </div>
        </div>
      </div>
    </Transition>

    <!-- 5. Booking Modal (Reserve Tour) -->
    <Transition name="fade">
      <div v-if="showBookingModal" class="fixed inset-0 z-[2000] flex items-center justify-center p-4 bg-[#0d1f2d]/85 backdrop-blur-md" @click="showBookingModal = false">
        
        <!-- SUCCESS STATE -->
        <div v-if="bookingSuccess" class="relative w-full max-w-lg overflow-hidden rounded-2xl bg-[#faf7f2] border border-[#c9a84c]/30 shadow-2xl p-8 text-center transition-all transform duration-300" @click.stop>
          
          <!-- Top-Right Close Button -->
          <button type="button" @click="showBookingModal = false" aria-label="Close success modal" class="absolute top-4 right-4 z-10 p-2 text-[#6b8a9a] hover:text-[#063a5c] hover:bg-black/5 rounded-full transition-all focus:outline-none focus:ring-2 focus:ring-[#c9a84c]">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- Luxury Circular Success Icon (Cream Backing & Gold Checkmark) -->
          <div class="mx-auto flex items-center justify-center w-20 h-20 rounded-full bg-[#c9a84c]/10 border border-[#c9a84c]/30 mb-6">
            <svg class="w-10 h-10 text-[#c9a84c]" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
            </svg>
          </div>

          <!-- Success Header -->
          <span class="inline-block px-3 py-1 text-[9px] font-bold tracking-[0.25em] uppercase text-[#c9a84c] border border-[#c9a84c]/30 rounded-full mb-3 bg-[#c9a84c]/5">
            Reservation Initiated
          </span>
          
          <h2 class="text-2xl font-bold font-serif text-[#063a5c] tracking-tight leading-snug mb-3">
            Your Egypt Journey Awaits
          </h2>

          <!-- Subtext Details -->
          <p class="text-sm text-[#2a3f4f] leading-relaxed mb-6 max-w-sm mx-auto">
            Thank you for choosing Seadora Travel. A dedicated Luxury Travel Planner has been assigned to your request and will contact you via email within the next 4 hours to review your customized itinerary.
          </p>

          <!-- Detailed Summary Box -->
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

          <!-- Return/Call-to-Action Button -->
          <button type="button" @click="showBookingModal = false" class="w-full bg-[#063a5c] hover:bg-[#0a5c8a] text-white font-medium text-xs tracking-widest uppercase py-4 px-8 rounded-lg shadow-lg transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-[#063a5c] focus:ring-offset-2">
            Close Window
          </button>
        </div>

        <!-- FORM STATE -->
        <div v-else class="relative w-full max-w-4xl overflow-hidden rounded-2xl bg-[#faf7f2] border border-[#c9a84c]/30 shadow-2xl flex flex-col md:flex-row transition-all transform duration-300" @click.stop>
          
          <!-- Top-Right Close Button -->
          <button type="button" @click="showBookingModal = false" aria-label="Close booking modal" class="absolute top-4 right-4 z-10 p-2 text-[#6b8a9a] hover:text-[#063a5c] hover:bg-black/5 rounded-full transition-all focus:outline-none focus:ring-2 focus:ring-[#c9a84c]">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- COLUMN 1: Brand Reassurance & Details (Deep Sea Backing) -->
          <div class="w-full md:w-5/12 bg-[#063a5c] text-white p-8 md:p-10 flex flex-col justify-between relative overflow-hidden">
            <!-- Subtle Brand Background Pattern -->
            <div class="absolute inset-0 opacity-5 mix-blend-overlay bg-repeat bg-center" style="background-image: radial-gradient(#c9a84c 1px, transparent 1px); background-size: 20px 20px;"></div>
            
            <div class="relative z-10">
              <!-- Brand Badge -->
              <span class="inline-block px-3 py-1 text-[10px] font-medium tracking-[0.2em] uppercase text-[#c9a84c] border border-[#c9a84c]/40 rounded-full mb-6 bg-[#c9a84c]/10">
                Exclusive Experiences
              </span>
              
              <!-- Headline -->
              <h2 class="text-3xl font-extrabold font-serif tracking-tight leading-tight text-white mb-4">
                Begin Your <br>Luxury Egypt <br>Journey
              </h2>
              
              <!-- Reassurance Paragraph -->
              <p class="text-sm text-[#8eafc2] leading-relaxed mb-8">
                Crafted by certified Egyptologists and luxury hospitality specialists, our tours offer unmatched access and elite accommodations.
              </p>

              <!-- Reassurance Benefits List -->
              <div class="space-y-4">
                <!-- Benefit 1 -->
                <div class="flex items-start space-x-3">
                  <div class="flex-shrink-0 w-5 h-5 rounded-full bg-[#c9a84c]/20 flex items-center justify-center text-[#c9a84c]">
                    <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-xs font-semibold uppercase tracking-wider text-white">Elite Local Guides</h4>
                    <p class="text-xs text-[#8eafc2] mt-0.5">Accompanied by dedicated, private Egyptologists</p>
                  </div>
                </div>
                
                <!-- Benefit 2 -->
                <div class="flex items-start space-x-3">
                  <div class="flex-shrink-0 w-5 h-5 rounded-full bg-[#c9a84c]/20 flex items-center justify-center text-[#c9a84c]">
                    <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-xs font-semibold uppercase tracking-wider text-white">Flexible Booking Policy</h4>
                    <p class="text-xs text-[#8eafc2] mt-0.5">Free cancellation up to 72 hours prior to departure</p>
                  </div>
                </div>

                <!-- Benefit 3 -->
                <div class="flex items-start space-x-3">
                  <div class="flex-shrink-0 w-5 h-5 rounded-full bg-[#c9a84c]/20 flex items-center justify-center text-[#c9a84c]">
                    <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-xs font-semibold uppercase tracking-wider text-white">Bespoke Concierge</h4>
                    <p class="text-xs text-[#8eafc2] mt-0.5">24/7 dedicated support throughout your stay</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Trust Badging -->
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

          <!-- COLUMN 2: Booking Form (Cream Backing) -->
          <form @submit.prevent="submitBooking" class="w-full md:w-7/12 p-8 md:p-10 flex flex-col justify-between">
            <div>
              <h3 class="text-xl font-bold font-serif text-[#063a5c] tracking-tight mb-6">
                Request Private Reservation
              </h3>

              <div class="space-y-4">
                <!-- Field: Full Name -->
                <div class="flex flex-col">
                  <label for="booking-name" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">
                    Full Name
                  </label>
                  <input type="text" id="booking-name" v-model="bookingForm.name" @input="validateField('name')" placeholder="Alexander Vance"
                    class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]">
                  <span v-if="bookingErrors.name" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.name }}</span>
                </div>

                <!-- Fields Row: Email and Phone -->
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div class="flex flex-col">
                    <label for="booking-email" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">
                      Email Address
                    </label>
                    <input type="email" id="booking-email" v-model="bookingForm.email" @input="validateField('email')" placeholder="alex@vance.com"
                      class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]">
                    <span v-if="bookingErrors.email" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.email }}</span>
                  </div>
                  
                  <div class="flex flex-col">
                    <label for="booking-phone" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">
                      Phone Number
                    </label>
                    <input type="tel" id="booking-phone" v-model="bookingForm.phone" @input="validateField('phone')" placeholder="+1 555-0199"
                      class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]">
                    <span v-if="bookingErrors.phone" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.phone }}</span>
                  </div>
                </div>

                <!-- Fields Row: Destination Select and Travel Date -->
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div class="flex flex-col">
                    <label for="booking-destination" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">
                      Select Destination
                    </label>
                    <div class="relative">
                      <select id="booking-destination" v-model="bookingForm.destination" @change="validateField('destination')"
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
                    <label for="booking-date" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">
                      Target Date
                    </label>
                    <input type="date" id="booking-date" v-model="bookingForm.date" @change="validateField('date')"
                      class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)] cursor-pointer">
                    <span v-if="bookingErrors.date" class="text-xs text-rose-600 mt-1 font-sans">{{ bookingErrors.date }}</span>
                  </div>
                </div>

                <!-- Field: Travel Party Size & Special Requirements -->
                <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
                  <div class="flex flex-col sm:col-span-1">
                    <label for="booking-guests" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">
                      Guests
                    </label>
                    <div class="relative">
                      <select id="booking-guests" v-model="bookingForm.guests" @change="validateField('guests')"
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
                    <label for="booking-notes" class="text-[10px] font-semibold tracking-wider text-[#6b8a9a] uppercase mb-1.5">
                      Special Requests or Preferences
                    </label>
                    <input type="text" id="booking-notes" v-model="bookingForm.notes" placeholder="Private yacht charter, dietary restrictions..."
                      class="bg-[#f7fbfd] border border-[#dce6ec] text-[#2a3f4f] placeholder-[#6b8a9a] rounded-lg px-4 py-3.5 text-sm transition-all duration-200 outline-none w-full font-sans hover:border-[#0a5c8a]/60 hover:bg-white focus:border-[#c9a84c] focus:bg-white focus:ring-1 focus:ring-[#c9a84c] focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]">
                  </div>
                </div>
              </div>
            </div>

            <!-- Action Footer -->
            <div class="mt-8 pt-6 border-t border-[#dce6ec] flex flex-col sm:flex-row items-center justify-between gap-4">
              <p class="text-xs text-[#6b8a9a] text-center sm:text-left leading-relaxed">
                No charge at booking. Review payment terms in business policies.
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

    <!-- 6. Gallery Lightbox Carousel Modal -->
    <Transition name="fade">
      <div v-if="showLightbox" class="fixed inset-0 z-[2500] flex flex-col justify-between bg-dark/95 backdrop-blur-xl" @click="closeLightbox">
        <!-- Lightbox Header -->
        <div class="p-6 flex justify-between items-center text-white relative z-10" @click.stop>
          <div class="font-jost">
            <span class="text-[9px] tracking-widest text-gold uppercase font-bold">{{ categoryName }} Experience</span>
            <div class="text-xs text-white/50">{{ lightboxIndex + 1 }} / {{ galleryImages.length }}</div>
          </div>
          <button @click="closeLightbox" class="w-10 h-10 rounded-full bg-white/5 hover:bg-white/10 border border-white/10 flex items-center justify-center text-white transition-all cursor-pointer">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Lightbox Main Content -->
        <div class="flex-1 flex items-center justify-center px-4 md:px-12 relative">
          <!-- Prev Button -->
          <button 
            @click.stop="prevLightboxImage" 
            class="absolute left-4 md:left-8 z-10 w-12 h-12 rounded-full bg-white/5 hover:bg-white/10 border border-white/10 flex items-center justify-center text-white transition-all cursor-pointer hover:scale-105 active:scale-95"
            aria-label="Previous image"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 19l-7-7 7-7" />
            </svg>
          </button>

          <!-- Current Image Container -->
          <div class="max-w-5xl max-h-[70vh] flex flex-col items-center" @click.stop>
            <img 
              :src="galleryImages[lightboxIndex].url" 
              :alt="galleryImages[lightboxIndex].title" 
              class="max-w-full max-h-[60vh] object-contain rounded-lg shadow-2xl border border-white/5 transition-all duration-300"
            />
          </div>

          <!-- Next Button -->
          <button 
            @click.stop="nextLightboxImage" 
            class="absolute right-4 md:right-8 z-10 w-12 h-12 rounded-full bg-white/5 hover:bg-white/10 border border-white/10 flex items-center justify-center text-white transition-all cursor-pointer hover:scale-105 active:scale-95"
            aria-label="Next image"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7" />
            </svg>
          </button>
        </div>

        <!-- Lightbox Caption Footer -->
        <div class="p-8 text-center text-white max-w-2xl mx-auto relative z-10" @click.stop>
          <span class="text-[10px] tracking-widest text-gold uppercase font-semibold font-jost">{{ galleryImages[lightboxIndex].tag }}</span>
          <h3 class="font-playfair text-2xl font-bold mb-2">{{ galleryImages[lightboxIndex].title }}</h3>
          <p class="text-white/70 text-xs font-jost leading-relaxed">{{ galleryImages[lightboxIndex].desc }}</p>
        </div>
      </div>
    </Transition>

    <!-- Footer -->
    <footer class="py-6 text-center border-t border-gold/10 bg-dark relative z-10 mt-16 md:mt-24">
      <p class="text-[9px] uppercase tracking-widest text-white/40">© {{ new Date().getFullYear() }} SeeDora Travel. All rights reserved.</p>
    </footer>
  </div>
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

/* Custom Scrollbar for reviews */
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

/* Page Layout details */
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
  color: rgba(255, 255, 255, 0.5);
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
  color: rgba(255, 255, 255, 0.5);
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

.review-carousel-card {
  width: 100%;
  background: linear-gradient(to bottom, #ffffff, var(--cream));
  border: 1px solid rgba(201, 168, 76, 0.16);
  border-radius: 12px;
  padding: 24px;
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

/* Picture Gallery Styles */
.gallery-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 24px;
}
@media (min-width: 1024px) {
  .gallery-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}
.gallery-card {
  border-radius: 12px;
  overflow: hidden;
  position: relative;
  height: 260px;
  cursor: pointer;
  box-shadow: 0 4px 20px rgba(0,0,0,0.06);
  border: 1px solid rgba(201, 168, 76, 0.15);
  transition: transform 0.4s ease, box-shadow 0.4s ease, border-color 0.4s ease;
}
.gallery-card:hover {
  transform: translateY(-6px);
  box-shadow: 0 20px 45px rgba(201, 168, 76, 0.18);
  border-color: rgba(201, 168, 76, 0.4);
}
.gallery-img-bg {
  position: absolute;
  inset: 0;
  background-size: cover;
  background-position: center;
  transition: transform 0.6s ease;
}
.gallery-card:hover .gallery-img-bg {
  transform: scale(1.08);
}
.gallery-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(13,31,45,0.85) 0%, rgba(13,31,45,0.2) 60%, transparent 100%);
  opacity: 0.9;
}
.gallery-info {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 20px;
  color: var(--white);
}
.gallery-tag {
  font-size: 9px;
  color: var(--sun-light);
  text-transform: uppercase;
  letter-spacing: 0.15em;
  font-weight: bold;
  display: block;
  margin-bottom: 4px;
}
.gallery-title {
  font-size: 16px;
  font-weight: bold;
  line-height: 1.3;
}

/* Feedback Modal Styles */
.feedback-card {
  width: 100%;
  max-width: 540px;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(201, 168, 76, 0.4);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 30px 70px rgba(6, 28, 40, 0.4), 0 0 50px rgba(201, 168, 76, 0.18);
  transition: all 0.5s cubic-bezier(0.16, 1, 0.3, 1);
}

.modal-close-btn {
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 50;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: rgba(13, 31, 45, 0.1);
  border: 1px solid rgba(201, 168, 76, 0.2);
  color: var(--dark);
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
  color: var(--white);
  transform: rotate(90deg);
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
