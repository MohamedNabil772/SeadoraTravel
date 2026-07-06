<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const tourId = computed(() => route.query.tourId as string || '')
const tourName = ref('')
const loadingTour = ref(false)

// Form fields
const rating = ref(0)
const hoverRating = ref(0)
const comment = ref('')
const customerName = ref('')
const customerEmail = ref('')

const isSubmitting = ref(false)
const submitSuccess = ref(false)
const errors = ref<Record<string, string>>({})

onMounted(async () => {
  if (tourId.value) {
    loadingTour.value = true
    try {
      const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
      const res = await fetch(`${API_URL}/api/content/api/tours`)
      if (res.ok) {
        const tours = await res.json()
        const selected = tours.find((t: any) => t.id === tourId.value)
        if (selected) {
          tourName.value = selected.names?.['en'] || selected.names?.['de'] || 'Selected Tour'
        }
      }
    } catch (e) {
      console.error('Failed to load tour details:', e)
    } finally {
      loadingTour.value = false
    }
  }
})

const setRating = (val: number) => {
  rating.value = val
  if (errors.value.rating) {
    delete errors.value.rating
  }
}

const validate = () => {
  const newErrors: Record<string, string> = {}
  if (!customerName.value.trim()) newErrors.name = 'Please enter your name.'
  if (!customerEmail.value.trim()) {
    newErrors.email = 'Please enter your email.'
  } else if (!/\S+@\S+\.\S+/.test(customerEmail.value)) {
    newErrors.email = 'Please enter a valid email address.'
  }
  if (rating.value === 0) newErrors.rating = 'Please select a rating between 1 and 5 stars.'
  
  errors.value = newErrors
  return Object.keys(newErrors).length === 0
}

const handleSubmit = async () => {
  if (!validate()) return

  isSubmitting.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    // Ensure the tourId is a valid Guid. If empty or not set, use empty Guid.
    const isGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(tourId.value)
    const validTourId = isGuid ? tourId.value : '00000000-0000-0000-0000-000000000000'

    const payload = {
      tourId: validTourId,
      rating: rating.value,
      comment: comment.value,
      customerName: customerName.value,
      customerEmail: customerEmail.value
    }
    
    // Attempt backend call
    const res = await fetch(`${API_URL}/api/booking/api/feedbacks`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })

    if (!res.ok) {
      const errorText = await res.text()
      throw new Error(errorText || 'Failed to submit feedback to server')
    }

    submitSuccess.value = true
  } catch (e: any) {
    console.error('Feedback submit error:', e)
    errors.value = { ...errors.value, form: e.message || 'We could not submit your feedback right now. Please try again.' }
  } finally {
    isSubmitting.value = false
  }
}

const resetForm = () => {
  rating.value = 0
  hoverRating.value = 0
  comment.value = ''
  customerName.value = ''
  customerEmail.value = ''
  submitSuccess.value = false
  errors.value = {}
}

const getStarWidth = (star: number, currentRating: number) => {
  if (currentRating >= star) return '100%'
  if (currentRating === star - 0.5) return '50%'
  return '0%'
}
</script>

<template>
  <div class="feedback-page min-h-screen bg-cream text-dark flex flex-col font-sans relative overflow-hidden">
    <!-- Pharaonic Background Grid Overlay -->
    <div class="absolute inset-0 bg-[radial-gradient(ellipse_at_top,_var(--sea-deep)_0%,_transparent_70%)] opacity-35 pointer-events-none"></div>
    <div class="absolute -top-40 -left-40 w-96 h-96 rounded-full bg-sun opacity-10 blur-3xl pointer-events-none"></div>
    <div class="absolute -bottom-40 -right-40 w-96 h-96 rounded-full bg-sea-light opacity-10 blur-3xl pointer-events-none"></div>

    <!-- Navigation Header -->
    <header class="w-full py-6 px-8 md:px-16 border-b border-gold/20 flex justify-between items-center bg-sea-deep/95 backdrop-blur-md relative z-10">
      <router-link to="/" class="flex items-center gap-3 no-underline">
        <div class="logo-icon w-10 h-10 bg-gradient-to-br from-sun to-sun-light rounded-full flex items-center justify-center text-lg text-white">🌊</div>
        <div class="leading-none">
          <span class="font-serif text-lg font-bold text-white tracking-wide block">SeeDora Travel</span>
          <span class="text-[9px] text-gold tracking-widest uppercase font-semibold">Egypt · Luxury Experiences</span>
        </div>
      </router-link>
      <router-link to="/" class="text-xs text-sun-light uppercase tracking-widest hover:text-white font-semibold transition-colors duration-300">
        ← Back to Tours
      </router-link>
    </header>

    <!-- Main Content Container -->
    <main class="flex-1 flex items-center justify-center p-6 md:p-12 relative z-10">
      <div class="feedback-card">
        
        <!-- Top Decorative Gold Line -->
        <div class="h-1.5 w-full bg-gradient-to-r from-sea via-gold to-sun"></div>

        <!-- Submission Form Area -->
        <div class="p-8 md:p-12">
          
          <!-- Success State -->
          <div v-if="submitSuccess" class="success-state flex flex-col items-center text-center py-8">
            <div class="checkmark-wrapper relative mb-6">
              <!-- Animated Circle & Check -->
              <div class="w-20 h-20 rounded-full bg-grass/10 border-2 border-grass flex items-center justify-center animate-scale-up">
                <svg class="w-10 h-10 text-grass animate-draw-check" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="3">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                </svg>
              </div>
              <div class="absolute -inset-1 rounded-full bg-grass/5 animate-pulse-slow"></div>
            </div>
            
            <h2 class="font-serif text-3xl font-bold text-dark mb-4">Shukran!</h2>
            <p class="text-muted max-w-sm mb-8 leading-relaxed">
              Thank you for sharing your journey with us. Your feedback helps us maintain the highest standard of luxury travel in Egypt.
            </p>
            
            <div class="flex gap-4">
              <button @click="resetForm" class="btn-secondary px-6 py-2.5 rounded-full border border-gold/50 text-gold hover:bg-gold/10 font-medium text-sm tracking-wide transition-all duration-300">
                Submit Another
              </button>
              <router-link to="/" class="btn-primary px-8 py-2.5 rounded-full bg-gradient-to-r from-sun to-sun-light text-white font-bold text-sm tracking-wide shadow-lg hover:shadow-sun/30 hover:scale-105 transition-all duration-300">
                Explore More Tours
              </router-link>
            </div>
          </div>

          <!-- Active Form State -->
          <div v-else>
            <!-- Header Text -->
            <div class="text-center mb-8">
              <span class="text-[10px] tracking-[0.25em] uppercase text-gold font-bold mb-2 block font-sans">Guest Reflections</span>
              <h2 class="font-playfair text-3xl md:text-4xl font-bold text-dark mb-3">Share Your Experience</h2>
              
              <div v-if="loadingTour" class="text-xs text-muted animate-pulse">Retrieving your tour details...</div>
              <div v-else-if="tourName" class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-sea/5 border border-sea/15 mt-1">
                <span class="text-xs font-semibold text-sea-deep">Tour:</span>
                <span class="text-xs font-medium text-sea">{{ tourName }}</span>
              </div>
            </div>

            <form @submit.prevent="handleSubmit" class="space-y-6">
              
              <!-- General form error -->
              <div v-if="errors.form" class="bg-red-500/10 border border-red-500/30 text-red-600 rounded-lg p-4 text-xs font-semibold font-sans animate-scale-up">
                {{ errors.form }}
              </div>
              
              <!-- Personal details grid -->
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label for="name" class="block text-[10px] font-bold text-muted mb-1.5 uppercase tracking-widest">Full Name</label>
                  <input 
                    id="name"
                    type="text" 
                    v-model="customerName" 
                    placeholder="Enter your name" 
                    class="w-full px-4 py-3 bg-cream/35 border rounded-lg outline-none text-sm transition-all duration-300"
                    :class="errors.name ? 'border-red-500 focus:ring-1 focus:ring-red-500' : 'border-gold/30 focus:border-sea focus:ring-1 focus:ring-sea'"
                  />
                  <p v-if="errors.name" class="text-[11px] text-red-500 mt-1">{{ errors.name }}</p>
                </div>
                <div>
                  <label for="email" class="block text-[10px] font-bold text-muted mb-1.5 uppercase tracking-widest">Email Address</label>
                  <input 
                    id="email"
                    type="email" 
                    v-model="customerEmail" 
                    placeholder="name@example.com" 
                    class="w-full px-4 py-3 bg-cream/35 border rounded-lg outline-none text-sm transition-all duration-300"
                    :class="errors.email ? 'border-red-500 focus:ring-1 focus:ring-red-500' : 'border-gold/30 focus:border-sea focus:ring-1 focus:ring-sea'"
                  />
                  <p v-if="errors.email" class="text-[11px] text-red-500 mt-1">{{ errors.email }}</p>
                </div>
              </div>

              <!-- Interactive Star Rating selection -->
              <div class="bg-cream/45 border border-gold/15 rounded-xl p-6 text-center transition-all duration-300 hover:border-gold/30">
                <label class="block text-[10px] font-bold text-muted mb-4 uppercase tracking-widest font-sans">Your Rating</label>
                
                <div class="flex justify-center items-center gap-4" @mouseleave="hoverRating = 0">
                  <button 
                    v-for="star in 5" 
                    :key="star"
                    type="button"
                    class="star-btn relative w-12 h-12 flex items-center justify-center focus:outline-none cursor-pointer group"
                  >
                    <!-- Left Half Hitbox -->
                    <div 
                      class="absolute top-0 left-0 w-1/2 h-full z-10"
                      @click="setRating(star - 0.5)"
                      @mouseover="hoverRating = star - 0.5"
                    ></div>
                    
                    <!-- Right Half Hitbox -->
                    <div 
                      class="absolute top-0 right-0 w-1/2 h-full z-10"
                      @click="setRating(star)"
                      @mouseover="hoverRating = star"
                    ></div>

                    <!-- Visual Star Representing Full, Half or Empty state -->
                    <span 
                      class="relative w-12 h-12 transition-all duration-300 transform group-hover:scale-110 flex items-center justify-center pointer-events-none"
                    >
                      <!-- Empty star background SVG -->
                      <svg class="w-12 h-12 text-gold/20" fill="currentColor" viewBox="0 0 24 24">
                        <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                      </svg>
                      
                      <!-- Filled star overlay SVG with width based on state -->
                      <span 
                        class="absolute top-0 left-0 overflow-hidden h-full flex items-center transition-all duration-150"
                        :style="{ width: getStarWidth(star, hoverRating || rating) }"
                      >
                        <svg class="w-12 h-12 text-gold filter drop-shadow-[0_0_4px_rgba(201,168,76,0.6)]" fill="currentColor" viewBox="0 0 24 24">
                          <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
                        </svg>
                      </span>
                    </span>
                  </button>
                </div>
                
                <span class="text-xs font-bold text-gold mt-4 block h-4 tracking-wide font-sans">
                  {{ rating === 5 ? 'Excellent & Luxury' : rating === 4.5 ? 'Spectacular Luxury' : rating === 4 ? 'Very Good' : rating === 3.5 ? 'Extremely Good' : rating === 3 ? 'Good' : rating === 2.5 ? 'Average' : rating === 2 ? 'Fair' : rating === 1.5 ? 'Mediocre' : rating === 1 ? 'Needs Improvement' : rating === 0.5 ? 'Dissatisfied' : 'Select your rating' }}
                </span>
                
                <p v-if="errors.rating" class="text-[11px] text-red-500 mt-2 font-sans">{{ errors.rating }}</p>
              </div>

              <!-- Optional comments area -->
              <div>
                <label for="comments" class="block text-[10px] font-bold text-muted mb-1.5 uppercase tracking-widest">Your Experience (Optional)</label>
                <textarea 
                  id="comments"
                  v-model="comment" 
                  rows="4" 
                  placeholder="Tell us about the highlights of your luxury journey..." 
                  class="w-full px-4 py-3 bg-cream/35 border border-gold/30 rounded-lg outline-none text-sm transition-all duration-300 focus:border-sea focus:ring-1 focus:ring-sea resize-none"
                ></textarea>
              </div>

              <!-- Submit button with animations and loading state -->
              <button 
                type="submit" 
                :disabled="isSubmitting"
                class="w-full bg-gradient-to-r from-sea-deep to-sea text-white py-3.5 px-6 rounded-lg font-bold text-xs uppercase tracking-widest shadow-xl hover:translate-y-[-2px] hover:shadow-sea/25 active:translate-y-[0px] disabled:opacity-50 transition-all duration-300 relative overflow-hidden flex items-center justify-center gap-2 group"
              >
                <!-- Shimmer highlight effect -->
                <span class="absolute top-0 -left-[100%] w-[50%] h-full bg-white/20 skew-x-[-25deg] group-hover:animate-shimmer"></span>
                
                <span v-if="isSubmitting" class="flex items-center gap-2">
                  <svg class="animate-spin h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Submitting...
                </span>
                <span v-else class="flex items-center gap-1.5">
                  Submit Feedback
                  <span class="group-hover:translate-x-1 transition-transform">→</span>
                </span>
              </button>

            </form>
          </div>

        </div>
      </div>
    </main>

    <!-- Simple Elegant Footer -->
    <footer class="py-6 text-center border-t border-gold/10 relative z-10">
      <p class="text-[10px] uppercase tracking-widest text-muted">© {{ new Date().getFullYear() }} SeeDora Travel. All rights reserved.</p>
    </footer>
  </div>
</template>

<style scoped>
@keyframes scale-up {
  0% { transform: scale(0.8); opacity: 0; }
  100% { transform: scale(1); opacity: 1; }
}

@keyframes draw-check {
  0% { stroke-dashoffset: 20; }
  100% { stroke-dashoffset: 0; }
}

@keyframes shimmer {
  100% { left: 125%; }
}

.animate-scale-up {
  animation: scale-up 0.4s cubic-bezier(0.34, 1.56, 0.64, 1) forwards;
}

.animate-draw-check {
  stroke-dasharray: 20;
  stroke-dashoffset: 20;
  animation: draw-check 0.4s ease-out 0.2s forwards;
}

.animate-pulse-slow {
  animation: pulse 2.5s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

.animate-shimmer {
  animation: shimmer 1s ease-in-out;
}

.feedback-card {
  width: 100%;
  max-width: 576px; /* max-w-xl */
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(201, 168, 76, 0.4);
  border-radius: 20px;
  box-shadow: 0 30px 70px rgba(6, 28, 40, 0.4), 0 0 50px rgba(201, 168, 76, 0.18);
  overflow: hidden;
  transition: all 0.5s cubic-bezier(0.16, 1, 0.3, 1);
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
