<template>
  <!-- Trigger Button -->
  <button 
    v-if="!isOpen" 
    class="concierge-trigger"
    @click="isOpen = true"
  >
    <div class="trigger-icon-wrapper">
      <span class="trigger-icon">👑</span>
      <div class="status-pulse"></div>
    </div>
  </button>

  <!-- Main Wrapper: Handles Desktop & Mobile layout dynamically -->
  <div 
    v-if="isOpen"
    class="concierge-wrapper"
    :class="[isMobile ? 'mobile-view' : 'desktop-view', { 'is-maximized': isMaximized }]"
  >
    <!-- Overlay for Mobile -->
    <div v-if="isMobile" class="concierge-overlay" @click="closeWidget"></div>
    
    <!-- Chat Container -->
    <div 
      class="concierge-container"
      :class="{ 'glassmorphic': !isMobile }"
      @touchstart="handleTouchStart"
      @touchmove="handleTouchMove"
      @touchend="handleTouchEnd"
      :style="mobileSwipeStyle"
    >
      <!-- Header -->
      <header class="concierge-header">
        <div class="header-left">
          <!-- Mobile Drag Handle -->
          <div v-if="isMobile" class="drag-handle"></div>
          <div class="concierge-avatar">
            <span class="avatar-icon">✨</span>
          </div>
          <div class="concierge-info">
            <h3>{{ $t('concierge.title') }}</h3>
            <p class="status">{{ $t('concierge.status.online') }}</p>
          </div>
        </div>
        <div class="header-actions">
          <button class="icon-btn" @click="toggleSound" :title="$t('actions.sound')">
            <span v-if="soundEnabled">🔊</span>
            <span v-else>🔇</span>
          </button>
          <button v-if="!isMobile" class="icon-btn" @click="toggleMaximize" :title="$t('actions.maximize')">
            <span v-if="isMaximized">↙️</span>
            <span v-else>↗️</span>
          </button>
          <button class="icon-btn" @click="clearChat" :title="$t('actions.clear')">🗑️</button>
          <button class="icon-btn close-btn" @click="closeWidget" :title="$t('actions.close')">✖</button>
        </div>
      </header>

      <!-- Messages Area -->
      <div class="concierge-messages" ref="messagesArea">
        <div class="message system-message">
          <p>{{ $t('concierge.welcome') }}</p>
        </div>
        
        <!-- Mock Tour Card for Luxury Aesthetics -->
        <div class="message tour-card">
          <div class="tour-card-header">
            <h4>Red Sea Yacht Experience</h4>
            <span class="rating">★ 4.9</span>
          </div>
          <div class="tour-card-body">
            <p>Experience the ultimate luxury on the crystal waters.</p>
            <div class="price-pill">$299 / person</div>
          </div>
          <div class="tour-card-footer">
            <button class="book-now-btn ripple">Book Now</button>
          </div>
        </div>
        
        <!-- Dynamic Messages -->
        <template v-for="msg in dynamicMessages" :key="msg.id">
          <!-- User Message -->
          <div v-if="msg.type === 'user'" class="message user-message">
            <p>{{ msg.text }}</p>
          </div>

          <!-- VIP Contact Card -->
          <div v-else-if="msg.type === 'contact'" class="message contact-card">
            <div class="contact-card-header">
              <h4>VIP Support</h4>
              <span class="icon">👑</span>
            </div>
            <div class="contact-card-body">
              <a href="https://wa.me/201223456789?text=Hello%20Seadora%20Concierge" target="_blank" class="contact-link whatsapp-link">
                <span class="icon">💬</span> Message on WhatsApp
              </a>
              
              <div class="contact-info-row">
                <span class="icon">📞</span>
                <div class="info-text">
                  <span class="info-label">24/7 VIP Hotline</span>
                  <span class="info-value">+20 122 345 6789</span>
                </div>
              </div>
              
              <div class="contact-info-row">
                <span class="icon">✉️</span>
                <div class="info-text">
                  <span class="info-label">Concierge Email</span>
                  <a href="mailto:concierge@seadoratravel.com" class="info-value">concierge@seadoratravel.com</a>
                </div>
              </div>
              
              <div class="contact-info-row">
                <span class="icon">📍</span>
                <div class="info-text">
                  <span class="info-label">Location</span>
                  <span class="info-value">Hurghada Marina Boulevard, Red Sea, Egypt</span>
                </div>
              </div>
              
              <div class="contact-info-row">
                <span class="icon">🕒</span>
                <div class="info-text">
                  <span class="info-label">Hours</span>
                  <span class="info-value">24/7 Dedicated Concierge Support</span>
                </div>
              </div>
            </div>
          </div>
        </template>
      </div>

      <!-- Quick Actions (Context-Aware Pills) -->
      <div class="quick-actions-scroll">
        <div class="quick-actions">
          <button 
            v-for="pill in quickActions" 
            :key="pill.id"
            class="action-pill"
            @click="selectQuickAction(pill)"
          >
            {{ pill.icon }} {{ $t(`quickActions.${pill.id}`) }}
          </button>
        </div>
      </div>

      <!-- Input Area -->
      <footer class="concierge-footer">
        <div class="input-wrapper">
          <input 
            type="text" 
            v-model="userInput" 
            :placeholder="$t('concierge.placeholder')"
            @keyup.enter="sendMessage"
          />
          <button class="send-btn" @click="sendMessage" :disabled="!userInput.trim()">
            ➤
          </button>
        </div>
      </footer>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n' // Multilingual Architecture Support

const { t } = useI18n()

// Props & State
const isOpen = ref(false)
const isMaximized = ref(false)
const soundEnabled = ref(true)
const userInput = ref('')

// Responsive State
const windowWidth = ref(window.innerWidth)
const isMobile = computed(() => windowWidth.value <= 768)

const updateWidth = () => {
  windowWidth.value = window.innerWidth
}

onMounted(() => {
  window.addEventListener('resize', updateWidth)
})

onUnmounted(() => {
  window.removeEventListener('resize', updateWidth)
})

// Quick Action Pills
const quickActions = ref([
  { id: 'topRated', icon: '🏝️' },
  { id: 'waterSports', icon: '🌊' },
  { id: 'desertSafari', icon: '🏜️' },
  { id: 'weekend', icon: '📅' },
  { id: 'permits', icon: '🛡️' },
  { id: 'contactUs', icon: '📞' },
])

// Actions
const closeWidget = () => {
  isOpen.value = false
}

const toggleMaximize = () => {
  isMaximized.value = !isMaximized.value
}

const toggleSound = () => {
  soundEnabled.value = !soundEnabled.value
}

const clearChat = () => {
  dynamicMessages.value = []
}

const dynamicMessages = ref<any[]>([])
const messagesArea = ref<HTMLElement | null>(null)

const scrollToBottom = () => {
  setTimeout(() => {
    if (messagesArea.value) {
      messagesArea.value.scrollTop = messagesArea.value.scrollHeight
    }
  }, 100)
}

const selectQuickAction = (pill: any) => {
  const text = t(`quickActions.${pill.id}`)
  dynamicMessages.value.push({ id: Date.now(), type: 'user', text: text || pill.id })
  
  if (pill.id === 'contactUs') {
    setTimeout(() => {
      dynamicMessages.value.push({ id: Date.now(), type: 'contact' })
      scrollToBottom()
    }, 400)
  } else {
    setTimeout(() => {
      dynamicMessages.value.push({ id: Date.now(), type: 'contact' }) // For unhandled query simulation
      scrollToBottom()
    }, 400)
  }
  scrollToBottom()
}

const sendMessage = () => {
  if (!userInput.value.trim()) return
  const text = userInput.value.trim()
  userInput.value = ''
  
  dynamicMessages.value.push({
    id: Date.now(),
    type: 'user',
    text
  })
  
  setTimeout(() => {
    // Show VIP contact card for any query right now to fulfill the requirement
    dynamicMessages.value.push({
      id: Date.now(),
      type: 'contact'
    })
    scrollToBottom()
  }, 500)
  scrollToBottom()
}

// Mobile Swipe-to-Dismiss Logic
const touchStartY = ref(0)
const touchCurrentY = ref(0)
const isSwiping = ref(false)

const handleTouchStart = (e: TouchEvent) => {
  if (!isMobile.value) return
  touchStartY.value = e.touches[0].clientY
  isSwiping.value = true
}

const handleTouchMove = (e: TouchEvent) => {
  if (!isSwiping.value) return
  const currentY = e.touches[0].clientY
  if (currentY > touchStartY.value) { // Only allow swiping down
    touchCurrentY.value = currentY - touchStartY.value
  }
}

const handleTouchEnd = () => {
  if (!isSwiping.value) return
  if (touchCurrentY.value > 150) {
    closeWidget()
  }
  touchCurrentY.value = 0
  isSwiping.value = false
}

const mobileSwipeStyle = computed(() => {
  if (!isMobile.value || !isSwiping.value) return {}
  return {
    transform: `translateY(${touchCurrentY.value}px)`,
    transition: isSwiping.value ? 'none' : 'transform 0.3s cubic-bezier(0.25, 0.8, 0.25, 1)'
  }
})
</script>

<style scoped>
/* Base Variables & Typography */
:root {
  --concierge-bg: rgba(255, 255, 255, 0.95);
  --concierge-border: rgba(255, 255, 255, 0.4);
  --concierge-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  --primary-color: #062d4d;
  --accent-color: #c9a84c;
  --success-color: #10b981;
  --text-primary: #1F2937;
  --text-secondary: #6B7280;
}

/* Wrapper & Layout */
.concierge-wrapper {
  position: fixed;
  z-index: 9999;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
}

.desktop-view {
  bottom: 24px;
  right: 24px;
}

.desktop-view .concierge-container {
  width: 420px;
  height: 620px;
  border-radius: 24px;
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  transform-origin: bottom right;
}

.desktop-view.is-maximized .concierge-container {
  width: 80vw;
  height: 85vh;
  max-width: 1200px;
  max-height: 900px;
}

/* Mobile View (Bottom Sheet) */
.mobile-view {
  inset: 0;
  display: flex;
  align-items: flex-end;
  pointer-events: none; /* Let clicks pass to overlay */
}

.concierge-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(4px);
  pointer-events: auto;
  animation: fadeIn 0.3s ease-out;
}

.mobile-view .concierge-container {
  width: 100%;
  height: 90vh; /* Fullscreenish */
  border-radius: 32px 32px 0 0;
  pointer-events: auto;
  animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  margin: 0;
}

/* Glassmorphism */
.glassmorphic {
  background: var(--concierge-bg);
  backdrop-filter: blur(24px);
  -webkit-backdrop-filter: blur(24px);
  border: 1px solid var(--concierge-border);
  box-shadow: var(--concierge-shadow);
}

/* Container Shared */
.concierge-container {
  background: #ffffff; /* fallback */
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* Header */
.concierge-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
  position: relative;
}

.drag-handle {
  position: absolute;
  top: -12px;
  left: 50%;
  transform: translateX(-50%);
  width: 40px;
  height: 4px;
  border-radius: 2px;
  background: rgba(0, 0, 0, 0.2);
}

.concierge-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--primary-color), var(--accent-color));
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  box-shadow: 0 4px 12px rgba(201, 168, 76, 0.3);
}

.concierge-info h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: var(--text-primary);
}

.concierge-info .status {
  margin: 0;
  font-size: 12px;
  color: var(--text-secondary);
}

.header-actions {
  display: flex;
  gap: 8px;
}

.icon-btn {
  background: transparent;
  border: none;
  font-size: 16px;
  cursor: pointer;
  padding: 8px;
  border-radius: 50%;
  transition: background 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon-btn:hover {
  background: rgba(0, 0, 0, 0.05);
}

.close-btn:hover {
  background: rgba(255, 0, 0, 0.1);
  color: red;
}

/* Messages Area */
.concierge-messages {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.message {
  padding: 12px 16px;
  border-radius: 16px;
  max-width: 85%;
  font-size: 14px;
  line-height: 1.5;
}

.system-message {
  align-self: flex-start;
  background: rgba(0, 0, 0, 0.04);
  border-bottom-left-radius: 4px;
}

/* Quick Actions */
.quick-actions-scroll {
  overflow-x: auto;
  padding: 12px 24px;
  scrollbar-width: none; /* Firefox */
  -ms-overflow-style: none; /* IE/Edge */
}
.quick-actions-scroll::-webkit-scrollbar {
  display: none;
}

.quick-actions {
  display: flex;
  gap: 8px;
  white-space: nowrap;
}

.action-pill {
  background: rgba(0, 0, 0, 0.03);
  border: 1px solid rgba(0, 0, 0, 0.08);
  border-radius: 20px;
  padding: 8px 16px;
  font-size: 13px;
  font-weight: 500;
  color: var(--text-primary);
  cursor: pointer;
  transition: all 0.2s ease-out;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.action-pill:hover {
  background: rgba(0, 0, 0, 0.06);
  transform: translateY(-1px);
}

.action-pill:active {
  transform: translateY(1px);
}

/* Footer / Input */
.concierge-footer {
  padding: 16px 24px;
  border-top: 1px solid rgba(0, 0, 0, 0.05);
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  background: rgba(0, 0, 0, 0.03);
  border-radius: 24px;
  padding: 4px 4px 4px 16px;
  border: 1px solid rgba(0, 0, 0, 0.05);
  transition: border-color 0.2s ease;
}

.input-wrapper:focus-within {
  border-color: var(--accent-color);
  background: #fff;
}

.input-wrapper input {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  font-size: 14px;
  color: var(--text-primary);
  padding: 8px 0;
}

.send-btn {
  background: var(--primary-color);
  color: white;
  border: none;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: transform 0.2s ease-out, opacity 0.2s ease;
}

.send-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.send-btn:not(:disabled):hover {
  transform: scale(1.05);
}

/* Trigger Button */
.concierge-trigger {
  position: fixed;
  bottom: 24px;
  right: 24px;
  width: 64px;
  height: 64px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--primary-color), #0a3d6b);
  border: 2px solid var(--accent-color);
  box-shadow: 0 8px 32px rgba(6, 45, 77, 0.4), 0 0 20px rgba(201, 168, 76, 0.3);
  cursor: pointer;
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275), box-shadow 0.3s ease;
}

.concierge-trigger:hover {
  transform: scale(1.1) translateY(-4px);
  box-shadow: 0 12px 40px rgba(6, 45, 77, 0.5), 0 0 30px rgba(201, 168, 76, 0.5);
}

.trigger-icon-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.trigger-icon {
  font-size: 28px;
  filter: drop-shadow(0 2px 4px rgba(0,0,0,0.2));
}

.status-pulse {
  position: absolute;
  top: -4px;
  right: -4px;
  width: 14px;
  height: 14px;
  background-color: var(--success-color);
  border-radius: 50%;
  border: 2px solid var(--primary-color);
  animation: pulse 2s infinite;
}

/* Tour Card */
.tour-card {
  background: #fff;
  border: 1px solid rgba(201, 168, 76, 0.2);
  box-shadow: 0 4px 16px rgba(0,0,0,0.06);
  padding: 16px;
  border-radius: 16px;
  max-width: 90%;
  align-self: flex-start;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.tour-card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.tour-card-header h4 {
  margin: 0;
  color: var(--primary-color);
  font-size: 16px;
  font-weight: 700;
}

.rating {
  color: var(--accent-color);
  font-weight: 600;
  font-size: 14px;
}

.tour-card-body p {
  margin: 0 0 8px 0;
  color: var(--text-secondary);
  font-size: 13px;
}

.price-pill {
  display: inline-block;
  background: rgba(201, 168, 76, 0.1);
  color: var(--accent-color);
  padding: 4px 12px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 13px;
  border: 1px solid rgba(201, 168, 76, 0.3);
}

.tour-card-footer {
  margin-top: 4px;
}

.book-now-btn {
  width: 100%;
  padding: 10px;
  background: linear-gradient(135deg, var(--accent-color), #b5953e);
  color: #fff;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(201, 168, 76, 0.4);
  transition: transform 0.2s, box-shadow 0.2s;
  position: relative;
  overflow: hidden;
}

.book-now-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(201, 168, 76, 0.5);
}

.ripple:after {
  content: "";
  display: block;
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  pointer-events: none;
  background-image: radial-gradient(circle, #fff 10%, transparent 10.01%);
  background-repeat: no-repeat;
  background-position: 50%;
  transform: scale(10, 10);
  opacity: 0;
  transition: transform .5s, opacity 1s;
}

.ripple:active:after {
  transform: scale(0, 0);
  opacity: .3;
  transition: 0s;
}

/* User Messages */
.user-message {
  align-self: flex-end;
  background: var(--primary-color);
  color: #fff;
  border-bottom-right-radius: 4px;
}

.user-message p {
  margin: 0;
}

/* VIP Contact Card */
.contact-card {
  background: linear-gradient(145deg, #062d4d, #0a3d6b);
  border: 1px solid var(--accent-color);
  box-shadow: 0 8px 24px rgba(6, 45, 77, 0.2);
  padding: 0;
  border-radius: 16px;
  max-width: 95%;
  align-self: flex-start;
  color: #fff;
  overflow: hidden;
  animation: fadeIn 0.4s ease-out;
}

.contact-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  background: rgba(201, 168, 76, 0.1);
  border-bottom: 1px solid rgba(201, 168, 76, 0.2);
}

.contact-card-header h4 {
  margin: 0;
  color: var(--accent-color);
  font-size: 16px;
  font-weight: 700;
  letter-spacing: 0.5px;
}

.contact-card-body {
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.whatsapp-link {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  background: #25D366;
  color: #fff;
  text-decoration: none;
  padding: 12px;
  border-radius: 12px;
  font-weight: 600;
  transition: transform 0.2s, box-shadow 0.2s;
  box-shadow: 0 4px 12px rgba(37, 211, 102, 0.3);
}

.whatsapp-link:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(37, 211, 102, 0.4);
}

.contact-info-row {
  display: flex;
  align-items: flex-start;
  gap: 12px;
}

.contact-info-row .icon {
  font-size: 18px;
  margin-top: 2px;
}

.info-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.info-label {
  font-size: 11px;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.6);
  letter-spacing: 0.5px;
}

.info-value {
  font-size: 14px;
  color: #fff;
  text-decoration: none;
  font-weight: 500;
}

a.info-value:hover {
  color: var(--accent-color);
}

/* Animations */
@keyframes slideUp {
  from { transform: translateY(100%); }
  to { transform: translateY(0); }
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes pulse {
  0% { transform: scale(1); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); }
  70% { transform: scale(1); box-shadow: 0 0 0 6px rgba(16, 185, 129, 0); }
  100% { transform: scale(1); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
}
</style>
