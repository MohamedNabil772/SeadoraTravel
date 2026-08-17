<script setup lang="ts">
import { ref, nextTick, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useCurrencyStore } from '@/store/currency'
import { useWindowSize, useSwipe } from '@vueuse/core'

const router = useRouter()
const { locale, t } = useI18n()
const currencyStore = useCurrencyStore()
const { width } = useWindowSize()
const isMobile = computed(() => width.value < 640)

interface TourSummaryDto {
  slug: string;
  mainImage?: string;
  names?: Record<string, string>;
  title?: string;
  priceEur?: number;
  price?: number;
}

interface ChatMessage {
  role: 'system' | 'user' | 'assistant';
  text: string;
  type?: 'text' | 'tours' | 'calendar';
  data?: TourSummaryDto[];
  copied?: boolean;
}

interface QuickAction {
  label: string;
  key: string;
}

const ChatIntent = {
  General: 0,
  TourRecommendation: 1
} as const;

const isOpen = ref(false)
const isMinimized = ref(false)
const showNotification = ref(true)
const soundEnabled = ref(true)

const initialMessage = {
  role: 'assistant' as const,
  text: t('concierge.welcome')
}

const messages = ref<ChatMessage[]>([
  { ...initialMessage }
])

const inputQuery = ref('')
const isTyping = ref(false)
const chatContainer = ref<HTMLElement | null>(null)
const chatSheet = ref<HTMLElement | null>(null)

// Mobile Swipe to Dismiss
const { lengthY } = useSwipe(chatSheet, {
  onSwipeEnd(_e, dir) {
    if (dir === 'down' && lengthY.value < -100) {
      closeChat()
    }
  }
})

const toggleChat = () => {
  if (isOpen.value) {
    closeChat()
  } else {
    openChat()
  }
}

const openChat = () => {
  isOpen.value = true
  isMinimized.value = false
  showNotification.value = false
  scrollToBottom()
  // Prevent body scroll on mobile
  if (isMobile.value) document.body.style.overflow = 'hidden'
}

const closeChat = () => {
  isOpen.value = false
  isMinimized.value = false
  if (isMobile.value) document.body.style.overflow = ''
}

const toggleMinimize = () => {
  isMinimized.value = !isMinimized.value
}

const clearChat = () => {
  messages.value = [{ ...initialMessage }]
  currentMenuState.value = 'main'
}

const scrollToBottom = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}

const currentMenuState = ref('main')

const currentOptions = computed<QuickAction[]>(() => {
  if (currentMenuState.value === 'main') {
    return [
      { label: '🏝️ Explore Tours', key: 'explore' },
      { label: '📅 Availability & Dates', key: 'availability' },
      { label: '💳 Payment & Booking', key: 'payment' },
      { label: '🛂 Passports & Permits', key: 'passports' },
      { label: '🚐 Hotel Transfers', key: 'transfers' },
      { label: '💬 VIP Concierge', key: 'human' }
    ]
  } else if (currentMenuState.value === 'explore') {
    return [
      { label: '🌊 Sea & Islands', key: 'tour_sea' },
      { label: '🏜️ Desert Safari', key: 'tour_safari' },
      { label: '🏛️ Historical Excursions', key: 'tour_history' },
      { label: '🤿 Diving & Snorkeling', key: 'tour_diving' },
      { label: '⬅️ Back to Menu', key: 'main' },
      { label: '💬 VIP Concierge', key: 'human' }
    ]
  } else {
    return [
      { label: '⬅️ Back to Menu', key: 'main' },
      { label: '💬 VIP Concierge', key: 'human' }
    ]
  }
})

const handleMenuClick = async (option: QuickAction) => {
  messages.value.push({ role: 'user', text: option.label })
  scrollToBottom()
  
  if (option.key === 'main') {
    currentMenuState.value = 'main'
    messages.value.push({ role: 'assistant', text: t('concierge.welcome') })
    scrollToBottom()
    return
  }

  if (option.key === 'explore') {
    currentMenuState.value = 'explore'
    messages.value.push({ role: 'assistant', text: "We have a variety of amazing experiences. What type of adventure are you looking for?" })
    scrollToBottom()
    return
  }

  if (option.key === 'human') {
    currentMenuState.value = 'chat'
    messages.value.push({ role: 'assistant', text: "💬 **VIP Concierge**\n\nYou are now connected with our VIP Concierge team. Please type your specific question or request below, and a human agent will assist you shortly." })
    scrollToBottom()
    return
  }

  currentMenuState.value = 'info'
  isTyping.value = true
  scrollToBottom()

  setTimeout(async () => {
    isTyping.value = false
    let replyText = ''
    let queryForTours = ''

    switch (option.key) {
      case 'availability':
        replyText = "🗓️ **Availability & Dates**\n\nAll our tours run daily! You can check specific available dates and times directly on any tour's booking page. We recommend booking at least 24 hours in advance to secure your spot."
        break;
      case 'payment':
        replyText = "💳 **Payment & Booking Policies**\n\n• **Payment Methods**: We accept secure online credit card payments, or you can choose to pay cash upon pickup.\n• **Cancellation**: Enjoy peace of mind with our 72-hour cancellation guarantee for a full refund.\n• **Vouchers**: You will receive a receipt and booking voucher via email immediately after confirming."
        break;
      case 'passports':
        replyText = "🛂 **Passports & Security Permits**\n\nFor certain sea trips and excursions outside Hurghada (like Luxor or Cairo), local authorities require passport copies for security permits. Please have a photo of your passport ready when booking these specific tours."
        break;
      case 'transfers':
        replyText = "🚐 **Hotel Transfers**\n\nWe provide comfortable, air-conditioned transfers from and to your hotel in Hurghada. For hotels outside Hurghada (e.g., El Gouna, Makadi Bay, Safaga), a small additional transfer fee may apply."
        break;
      case 'tour_sea':
        replyText = "🌊 **Sea & Islands**\n\nDiscover the crystal-clear waters of the Red Sea! Popular choices include Orange Bay, Giftun Island, and Paradise Island. Enjoy snorkeling, white sandy beaches, and lunch on board."
        queryForTours = 'sea'
        break;
      case 'tour_safari':
        replyText = "🏜️ **Desert Safari**\n\nExperience the thrill of the Sahara! Ride ATV quads, drive spider buggies, and enjoy a traditional Bedouin dinner under the stars with spectacular oriental shows."
        queryForTours = 'safari'
        break;
      case 'tour_history':
        replyText = "🏛️ **Historical Excursions**\n\nStep back in time! Visit the breathtaking temples of Luxor or the iconic Pyramids of Cairo. We offer both day trips and overnight stays with expert Egyptologist guides."
        queryForTours = 'luxor'
        break;
      case 'tour_diving':
        replyText = "🤿 **Diving & Snorkeling**\n\nExplore vibrant coral reefs and marine life. We offer introductory dives for beginners, PADI courses, and daily diving packages for certified divers."
        queryForTours = 'diving'
        break;
    }

    if (queryForTours) {
      const tours = await fetchTours(queryForTours)
      messages.value.push({ role: 'assistant', text: replyText, type: 'tours', data: tours.slice(0, 3) })
    } else {
      messages.value.push({ role: 'assistant', text: replyText })
    }
    scrollToBottom()
  }, 600)
}

const formatPrice = (eur: number | undefined) => {
  const val = eur ?? 0;
  if (currencyStore.selectedCurrency === 'USD') return `$${(val * 1.08).toFixed(2)}`
  if (currencyStore.selectedCurrency === 'EGP') return `EGP ${(val * 50).toFixed(0)}`
  return `€${val.toFixed(2)}`
}

const fetchTours = async (query: string) => {
  try {
    const res = await fetch(`/api/content/api/tours?search=${encodeURIComponent(query)}&lang=${locale.value}`)
    if (res.ok) {
      const data = await res.json()
      return data.items || data || []
    }
    return []
  } catch (err) {
    console.error('Failed to fetch tours', err)
    return []
  }
}

const handleSend = async (text: string) => {
  if (!text.trim()) return
  
  if (isMinimized.value) isMinimized.value = false
  
  if (currentMenuState.value !== 'chat') {
    currentMenuState.value = 'chat'
  }
  
  messages.value.push({ role: 'user', text })
  inputQuery.value = ''
  scrollToBottom()
  
  isTyping.value = true
  scrollToBottom()
  
  setTimeout(async () => {
    isTyping.value = false
    try {
      const res = await fetch('/api/concierge/chat', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message: text })
      })
      if (res.ok) {
        const data = await res.json()
        
        const q = text.toLowerCase()
        if (data.intent === ChatIntent.TourRecommendation || q.includes('recommend') || q.includes('tour') || q.includes('best')) {
          const tours = await fetchTours('best')
          messages.value.push({
            role: 'assistant',
            text: data.replyText || t('concierge.recommendationFallback'),
            type: 'tours',
            data: tours.slice(0, 3)
          })
        } else {
          messages.value.push({
            role: 'assistant',
            text: data.replyText
          })
        }
      } else {
        messages.value.push({
          role: 'assistant',
          text: t('concierge.connectionError')
        })
      }
    } catch (err) {
      messages.value.push({
        role: 'assistant',
        text: t('concierge.processingError')
      })
    }
    scrollToBottom()
  }, 1000)
}

const viewTour = (slug: string) => {
  closeChat()
  router.push(`/tour/${slug}`)
}

const copyToClipboard = async (msg: any) => {
  try {
    await navigator.clipboard.writeText(msg.text)
    msg.copied = true
    setTimeout(() => { msg.copied = false }, 2000)
  } catch (err) {
    console.error('Failed to copy', err)
  }
}

const readAloud = (text: string) => {
  if (!soundEnabled.value) return
  if ('speechSynthesis' in window) {
    window.speechSynthesis.cancel();
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = locale.value === 'en' ? 'en-US' : locale.value;
    window.speechSynthesis.speak(utterance);
  }
}

watch(isMobile, (newVal) => {
  if (isOpen.value && !newVal) {
    document.body.style.overflow = ''
  } else if (isOpen.value && newVal) {
    document.body.style.overflow = 'hidden'
  }
})
</script>

<template>
  <div class="fixed z-50 font-sans" :class="isMobile ? 'bottom-0 left-0 right-0 pointer-events-none' : 'bottom-6 right-6'">
    
    <!-- Mobile Backdrop -->
    <div v-if="isMobile && isOpen" 
         v-motion
         :initial="{ opacity: 0 }"
         :enter="{ opacity: 1, transition: { duration: 300 } }"
         :leave="{ opacity: 0, transition: { duration: 300 } }"
         class="fixed inset-0 bg-black/40 backdrop-blur-sm pointer-events-auto"
         @click="closeChat">
    </div>
    
    <!-- Chat Window -->
    <div v-show="isOpen" 
         v-motion
         :initial="isMobile ? { y: '100%' } : { opacity: 0, y: 30, scale: 0.95 }"
         :enter="isMobile ? { y: 0, transition: { type: 'spring', stiffness: 350, damping: 30 } } : { opacity: 1, y: 0, scale: 1, transition: { type: 'spring', stiffness: 350, damping: 25 } }"
         :leave="isMobile ? { y: '100%', transition: { type: 'spring', stiffness: 350, damping: 30 } } : { opacity: 0, y: 30, scale: 0.95, transition: { type: 'spring', stiffness: 350, damping: 25 } }"
         ref="chatSheet"
         class="pointer-events-auto bg-white shadow-2xl flex flex-col overflow-hidden origin-bottom-right transition-all duration-300"
         :class="[
           isMobile ? 'fixed bottom-0 left-0 right-0 rounded-t-3xl border-t border-[#e2e8f0]' : 'absolute bottom-16 right-0 rounded-2xl border border-[#e2e8f0]',
           !isMobile && isMinimized ? 'h-16 w-[420px]' : !isMobile ? 'w-[420px] h-[620px]' : 'w-full h-[100dvh]'
         ]">
      
      <!-- Mobile Drag Handle -->
      <div v-if="isMobile" class="w-full flex justify-center pt-3 pb-1 bg-[#062d4d]" @click="closeChat">
        <div class="w-12 h-1.5 bg-white/30 rounded-full"></div>
      </div>
      
      <!-- Header -->
      <div class="bg-[#062d4d] text-white p-4 flex items-center justify-between z-10 relative shrink-0" :class="isMobile ? 'pt-2' : ''">
        <div class="flex items-center gap-3 cursor-pointer" @click="!isMobile && toggleMinimize()">
          <div class="w-10 h-10 rounded-full bg-[#c9a84c] flex items-center justify-center font-serif font-bold text-xl shadow-[0_0_15px_rgba(201,168,76,0.5)]">S</div>
          <div>
            <h3 class="font-bold text-base tracking-wide">{{ t('concierge.title') }}</h3>
            <p class="text-xs text-[#cbd5e1] flex items-center gap-1.5 font-medium">
              <span class="w-2 h-2 rounded-full bg-emerald-400 animate-pulse shadow-[0_0_5px_#34d399]"></span>
              {{ t('concierge.online') }}
            </p>
          </div>
        </div>
        
        <div class="flex items-center gap-1.5">
          <button v-if="!isMobile" @click="soundEnabled = !soundEnabled" class="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded-full transition-all" title="Toggle Sound">
            <svg v-if="soundEnabled" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.536 8.464a5 5 0 010 7.072m2.828-9.9a9 9 0 010 12.728M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z"></path></svg>
            <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z M17 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2"></path></svg>
          </button>
          <button v-if="!isMobile" @click="clearChat" class="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded-full transition-all" title="Clear Chat">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
          </button>
          <button v-if="!isMobile" @click="toggleMinimize" class="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded-full transition-all">
            <svg v-if="isMinimized" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8V4m0 0h4M4 4l5 5m11-1V4m0 0h-4m4 0l-5 5M4 16v4m0 0h4m-4 0l5-5m11 5l-5-5m5 5v-4m0 4h-4"></path></svg>
            <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4"></path></svg>
          </button>
          <button @click="closeChat" class="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded-full transition-all hover:rotate-90">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
          </button>
        </div>
      </div>
      
      <div v-show="!isMinimized" class="flex flex-col flex-1 overflow-hidden">
        <!-- Messages -->
        <div class="flex-1 overflow-y-auto p-4 bg-[#f8fafc] space-y-5" ref="chatContainer">
          <div v-for="(msg, idx) in messages" :key="idx" 
               v-motion 
               :initial="{ opacity: 0, y: 15, scale: 0.98 }" 
               :enter="{ opacity: 1, y: 0, scale: 1, transition: { type: 'spring', stiffness: 400, damping: 30 } }"
               :class="['flex', msg.role === 'user' ? 'justify-end' : 'justify-start group/msg']">
            
            <div :class="['max-w-[85%] rounded-2xl p-3.5 text-[15px] shadow-sm relative', msg.role === 'user' ? 'bg-[#062d4d] text-white rounded-br-sm' : 'bg-white border border-[#e2e8f0] text-[#1e293b] rounded-bl-sm']">
              <p class="whitespace-pre-wrap leading-relaxed">{{ msg.text }}</p>
              
              <!-- Actions for Assistant Messages -->
              <div v-if="msg.role === 'assistant'" class="absolute -right-12 top-1 opacity-0 group-hover/msg:opacity-100 transition-opacity duration-200 flex flex-col gap-1 hidden md:flex">
                <button @click="copyToClipboard(msg)" class="p-1.5 bg-white text-gray-500 rounded-full shadow-sm hover:text-[#0284c7] hover:bg-gray-50 transition-colors border border-gray-100" title="Copy">
                  <svg v-if="!msg.copied" class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"></path></svg>
                  <svg v-else class="w-3.5 h-3.5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path></svg>
                </button>
                <button v-if="soundEnabled" @click="readAloud(msg.text)" class="p-1.5 bg-white text-gray-500 rounded-full shadow-sm hover:text-[#0284c7] hover:bg-gray-50 transition-colors border border-gray-100" title="Read Aloud">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.536 8.464a5 5 0 010 7.072m2.828-9.9a9 9 0 010 12.728M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z"></path></svg>
                </button>
              </div>
              
              <!-- Interactive Tour Cards Carousel -->
              <div v-if="msg.type === 'tours' && msg.data && msg.data.length" class="mt-4 flex gap-3 overflow-x-auto snap-x snap-mandatory hide-scrollbar pb-2 overscroll-x-contain" style="-webkit-overflow-scrolling: touch; scroll-behavior: smooth;">
                <div v-for="tour in msg.data" :key="tour.slug" class="min-w-[180px] max-w-[180px] snap-center bg-white border border-[#e2e8f0] rounded-xl overflow-hidden cursor-pointer group hover:border-[#c9a84c] hover:shadow-lg transition-all duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] relative active:scale-95" @click="viewTour(tour.slug)">
                  <div class="overflow-hidden h-28">
                    <img v-if="tour.mainImage" :src="tour.mainImage" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110" alt="Tour" />
                  </div>
                  <div class="p-3 relative">
                    <h4 class="font-bold text-xs text-[#0f172a] line-clamp-2 group-hover:text-[#c9a84c] transition-colors leading-tight h-8">{{ tour.names?.[locale] || tour.title }}</h4>
                    <div class="flex flex-col mt-2.5 gap-2">
                      <span class="font-black text-[15px] text-[#062d4d]">{{ formatPrice(tour.priceEur ?? tour.price) }}</span>
                      <button class="w-full text-xs bg-gradient-to-r from-[#c9a84c] to-[#e1c675] text-white py-2 rounded-lg font-bold shadow-md hover:shadow-lg transition-all overflow-hidden relative ripple-btn">
                        {{ t('concierge.bookNow') }}
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Typing Indicator -->
          <div v-if="isTyping" class="flex justify-start">
            <div v-motion :initial="{ opacity: 0, y: 10, scale: 0.9 }" :enter="{ opacity: 1, y: 0, scale: 1, transition: { type: 'spring', stiffness: 400, damping: 25 } }" class="bg-white border border-[#e2e8f0] rounded-2xl rounded-bl-sm px-4 py-3 text-[#94a3b8] flex gap-1.5 items-center shadow-sm">
              <span class="w-2 h-2 bg-[#c9a84c] rounded-full animate-[wave_1.2s_ease-in-out_infinite] shadow-[0_0_4px_#c9a84c]"></span>
              <span class="w-2 h-2 bg-[#c9a84c] rounded-full animate-[wave_1.2s_ease-in-out_infinite] [animation-delay:0.2s] shadow-[0_0_4px_#c9a84c]"></span>
              <span class="w-2 h-2 bg-[#c9a84c] rounded-full animate-[wave_1.2s_ease-in-out_infinite] [animation-delay:0.4s] shadow-[0_0_4px_#c9a84c]"></span>
            </div>
          </div>
        </div>
        
        <!-- Quick Prompts -->
        <div class="bg-white px-4 py-3 border-t border-[#e2e8f0] flex gap-2.5 overflow-x-auto whitespace-nowrap hide-scrollbar">
          <button v-for="prompt in currentOptions" :key="prompt.key" @click="handleMenuClick(prompt)" class="text-[13px] bg-[#f8fafc] hover:bg-[#f1f5f9] active:bg-[#e2e8f0] text-[#475569] px-4 py-2 rounded-full transition-all border border-[#e2e8f0] hover:border-[#cbd5e1] active:scale-95 shadow-sm font-medium">
            {{ prompt.label }}
          </button>
        </div>
        
        <!-- Input -->
        <div class="p-4 bg-white border-t border-[#e2e8f0] flex gap-3 items-center relative z-10 pb-safe" :class="isMobile ? 'pb-6' : ''">
          <input v-model="inputQuery" @keyup.enter="handleSend(inputQuery)" type="text" :placeholder="t('concierge.placeholder')" class="flex-1 bg-[#f8fafc] border border-[#e2e8f0] rounded-full px-5 py-3 text-[15px] focus:outline-none focus:border-[#c9a84c] focus:ring-2 focus:ring-[#c9a84c]/20 transition-all placeholder:text-[#94a3b8] shadow-inner" />
          <button @click="handleSend(inputQuery)" class="w-11 h-11 bg-gradient-to-br from-[#062d4d] to-[#0f172a] hover:from-[#0f172a] hover:to-[#1e293b] active:scale-90 text-white rounded-full flex items-center justify-center transition-all shadow-md group shrink-0">
            <svg class="w-5 h-5 translate-x-[1px] -translate-y-[1px] group-hover:translate-x-[2px] group-hover:-translate-y-[2px] transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"></path></svg>
          </button>
        </div>
      </div>
    </div>
    
    <!-- Trigger Button (Hidden on Mobile when open) -->
    <button v-show="!isMobile || !isOpen" 
            @click="toggleChat" 
            v-motion 
            :initial="{ scale: 0 }" 
            :enter="{ scale: 1, transition: { type: 'spring', stiffness: 300, damping: 20 } }" 
            class="pointer-events-auto relative group w-14 h-14 md:w-16 md:h-16 bg-[#062d4d] rounded-full shadow-[0_10px_25px_-5px_rgba(6,45,77,0.5)] flex items-center justify-center transition-all duration-300 hover:scale-105 active:scale-95 border border-[#c9a84c]/50 hover:border-[#c9a84c]"
            :class="isMobile ? 'fixed bottom-6 right-6' : ''">
      <div v-if="showNotification" class="absolute -top-1 -right-1 w-4 h-4 md:w-5 md:h-5 bg-red-500 border-2 border-white rounded-full flex items-center justify-center animate-pulse z-10"></div>
      <div v-show="!isOpen" class="absolute inset-0 rounded-full bg-[#c9a84c] opacity-0 group-hover:opacity-20 transition-opacity blur-md"></div>
      
      <div class="relative w-7 h-7 md:w-8 md:h-8 text-[#c9a84c] transition-all duration-300 transform" :class="isOpen && !isMinimized ? 'rotate-90 scale-0 opacity-0' : 'rotate-0 scale-100 opacity-100 absolute'">
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"></path>
        </svg>
      </div>
      <div class="relative w-7 h-7 md:w-8 md:h-8 text-[#c9a84c] transition-all duration-300 transform" :class="isOpen && !isMinimized ? 'rotate-0 scale-100 opacity-100' : '-rotate-90 scale-0 opacity-0 absolute'">
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
        </svg>
      </div>
    </button>
    
  </div>
</template>

<style scoped>
.hide-scrollbar::-webkit-scrollbar {
  display: none;
}
.hide-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

@keyframes wave {
  0%, 60%, 100% {
    transform: translateY(0);
  }
  30% {
    transform: translateY(-5px);
  }
}

/* Ripple effect for button */
.ripple-btn {
  position: relative;
  overflow: hidden;
}
.ripple-btn::after {
  content: "";
  position: absolute;
  top: 50%;
  left: 50%;
  width: 5px;
  height: 5px;
  background: rgba(255, 255, 255, 0.5);
  opacity: 0;
  border-radius: 100%;
  transform: scale(1, 1) translate(-50%, -50%);
  transform-origin: 50% 50%;
}
.ripple-btn:active::after {
  animation: ripple 0.4s ease-out;
}

@keyframes ripple {
  0% {
    transform: scale(0, 0);
    opacity: 0.5;
  }
  100% {
    transform: scale(20, 20);
    opacity: 0;
  }
}

/* Safe area padding for mobile input */
@supports (padding-bottom: env(safe-area-inset-bottom)) {
  .pb-safe {
    padding-bottom: calc(1rem + env(safe-area-inset-bottom));
  }
}
</style>
