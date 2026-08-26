<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const mouseX = ref(0)
const mouseY = ref(0)
const isMounted = ref(false)

const targetX = ref(0)
const targetY = ref(0)
let animationFrameId = 0

// Momentum/spring simulation for spotlight
const animateSpotlight = () => {
  // Easing factor (lower is slower)
  const ease = 0.08
  mouseX.value += (targetX.value - mouseX.value) * ease
  mouseY.value += (targetY.value - mouseY.value) * ease
  
  animationFrameId = requestAnimationFrame(animateSpotlight)
}

const handleMouseMove = (e: MouseEvent) => {
  targetX.value = e.clientX
  targetY.value = e.clientY
}

// Countdown logic
const targetDate = new Date()
targetDate.setDate(targetDate.getDate() + 42) // 42 days from now

const days = ref('00')
const hours = ref('00')
const minutes = ref('00')
const seconds = ref('00')

let timerInterval: ReturnType<typeof setInterval>

const updateCountdown = () => {
  const now = new Date().getTime()
  const distance = targetDate.getTime() - now

  if (distance < 0) {
    clearInterval(timerInterval)
    return
  }

  const d = Math.floor(distance / (1000 * 60 * 60 * 24))
  const h = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
  const m = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60))
  const s = Math.floor((distance % (1000 * 60)) / 1000)

  days.value = d.toString().padStart(2, '0')
  hours.value = h.toString().padStart(2, '0')
  minutes.value = m.toString().padStart(2, '0')
  seconds.value = s.toString().padStart(2, '0')
}

onMounted(() => {
  window.addEventListener('mousemove', handleMouseMove)
  // Center initially
  targetX.value = window.innerWidth / 2
  targetY.value = window.innerHeight / 2
  mouseX.value = targetX.value
  mouseY.value = targetY.value
  
  animationFrameId = requestAnimationFrame(animateSpotlight)
  
  setTimeout(() => {
    isMounted.value = true
  }, 100) // Trigger stagger entrance

  updateCountdown()
  timerInterval = setInterval(updateCountdown, 1000)
})

onUnmounted(() => {
  window.removeEventListener('mousemove', handleMouseMove)
  cancelAnimationFrame(animationFrameId)
  clearInterval(timerInterval)
})

const email = ref('')
const handleSubscribe = () => {
  console.log('Subscribing:', email.value)
}
</script>

<template>
  <div class="coming-soon-wrapper" :style="{ '--mouse-x': mouseX + 'px', '--mouse-y': mouseY + 'px' }">
    <div class="spotlight"></div>
    <div class="wave-shimmer"></div>

    <div class="content-container" :class="{ 'is-mounted': isMounted }">
      <div class="logo animate-in stagger-1">
        Seadora Travel
      </div>
      
      <h1 class="tagline animate-in stagger-2">
        Redefining Luxury Marine Journeys
      </h1>
      
      <div class="countdown-blocks animate-in stagger-3">
        <div class="time-block">
          <div class="value-wrapper">
            <Transition name="flip" mode="out-in">
              <span class="value" :key="days">{{ days }}</span>
            </Transition>
          </div>
          <span class="label">Days</span>
        </div>
        <div class="time-block">
          <div class="value-wrapper">
            <Transition name="flip" mode="out-in">
              <span class="value" :key="hours">{{ hours }}</span>
            </Transition>
          </div>
          <span class="label">Hours</span>
        </div>
        <div class="time-block">
          <div class="value-wrapper">
            <Transition name="flip" mode="out-in">
              <span class="value" :key="minutes">{{ minutes }}</span>
            </Transition>
          </div>
          <span class="label">Minutes</span>
        </div>
        <div class="time-block">
          <div class="value-wrapper">
            <Transition name="flip" mode="out-in">
              <span class="value" :key="seconds">{{ seconds }}</span>
            </Transition>
          </div>
          <span class="label">Seconds</span>
        </div>
      </div>
      
      <div class="vip-form animate-in stagger-4">
        <input type="email" v-model="email" placeholder="Enter your email for VIP access" aria-label="Email address" />
        <button class="snappy-btn" @click="handleSubscribe">Notify Me</button>
      </div>

      <div class="teaser-cards animate-in stagger-5">
        <div class="card">Exclusive Yachts</div>
        <div class="card">Tailored Itineraries</div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Motion Design Principles */
:global(:root) {
  --easing-snappy: cubic-bezier(0.23, 1, 0.32, 1);
  --easing-smooth: cubic-bezier(0.16, 1, 0.3, 1);
}

.coming-soon-wrapper {
  position: relative;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #040d1a;
  color: #fff;
  overflow: hidden;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
}

/* Mouse-reactive spotlight with momentum */
.spotlight {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  background: radial-gradient(
    800px circle at var(--mouse-x, 50%) var(--mouse-y, 50%),
    rgba(255, 255, 255, 0.05),
    transparent 40%
  );
  z-index: 1;
}

/* Subtle luxury floating animations / gentle wave shimmer */
.wave-shimmer {
  position: absolute;
  inset: -50%;
  background: linear-gradient(
    to bottom,
    transparent,
    rgba(255, 255, 255, 0.015) 50%,
    transparent
  );
  transform: rotate(-15deg);
  animation: float-waves 25s ease-in-out infinite alternate;
  pointer-events: none;
  z-index: 0;
}

@keyframes float-waves {
  0% {
    transform: rotate(-15deg) translateY(0%) scale(1);
  }
  100% {
    transform: rotate(-15deg) translateY(12%) scale(1.05);
  }
}

.content-container {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: 2.5rem;
  max-width: 900px;
  padding: 2rem;
  width: 100%;
}

/* Staggered entrance animations */
.animate-in {
  opacity: 0;
  transform: translateY(12px) scale(0.96);
  transition: opacity 0.8s var(--easing-smooth), transform 0.8s var(--easing-smooth);
}

.is-mounted .animate-in {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* 50ms stagger delays */
.is-mounted .stagger-1 { transition-delay: 50ms; }
.is-mounted .stagger-2 { transition-delay: 100ms; }
.is-mounted .stagger-3 { transition-delay: 150ms; }
.is-mounted .stagger-4 { transition-delay: 200ms; }
.is-mounted .stagger-5 { transition-delay: 250ms; }

/* Content styling */
.logo {
  font-size: 0.875rem;
  font-weight: 600;
  letter-spacing: 0.15em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.6);
  margin-bottom: -1rem;
}

.tagline {
  font-size: clamp(2.5rem, 5vw, 4rem);
  font-weight: 700;
  background: linear-gradient(135deg, #ffffff 0%, #a5b4fc 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  line-height: 1.1;
  letter-spacing: -0.02em;
}

.countdown-blocks {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
  justify-content: center;
}

.time-block {
  display: flex;
  flex-direction: column;
  align-items: center;
  background: rgba(255, 255, 255, 0.02);
  border: 1px solid rgba(255, 255, 255, 0.04);
  padding: 1.25rem 1.5rem;
  border-radius: 16px;
  min-width: 110px;
  backdrop-filter: blur(12px);
}

/* Live countdown ticker flip/morph without layout shift */
.value-wrapper {
  height: 3rem;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.time-block .value {
  font-size: 2.75rem;
  font-weight: 600;
  line-height: 1;
  font-variant-numeric: tabular-nums;
  display: inline-block;
}

.flip-enter-active,
.flip-leave-active {
  transition: all 0.4s cubic-bezier(0.23, 1, 0.32, 1);
}

.flip-enter-from {
  opacity: 0;
  transform: translateY(50%) scale(0.9);
}

.flip-leave-to {
  opacity: 0;
  transform: translateY(-50%) scale(1.1);
}

.time-block .label {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: rgba(255, 255, 255, 0.4);
  margin-top: 0.75rem;
}

.vip-form {
  display: flex;
  gap: 0.75rem;
  width: 100%;
  max-width: 440px;
  margin-top: 1rem;
}

.vip-form input {
  flex: 1;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 10px;
  padding: 0.875rem 1.25rem;
  color: #fff;
  font-size: 1rem;
  outline: none;
  transition: border-color 0.3s var(--easing-snappy), background 0.3s var(--easing-snappy);
}

.vip-form input::placeholder {
  color: rgba(255, 255, 255, 0.3);
}

.vip-form input:focus {
  border-color: rgba(255, 255, 255, 0.25);
  background: rgba(255, 255, 255, 0.06);
}

/* Micro-interactions & button physics */
.snappy-btn {
  background: #ffffff;
  color: #040d1a;
  border: none;
  border-radius: 10px;
  padding: 0.875rem 1.75rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s cubic-bezier(0.23, 1, 0.32, 1), background 0.2s cubic-bezier(0.23, 1, 0.32, 1);
  will-change: transform;
}

.snappy-btn:hover {
  background: #f0f0f0;
}

.snappy-btn:active {
  transform: scale(0.97);
}

.teaser-cards {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
  flex-wrap: wrap;
  justify-content: center;
}

.card {
  background: rgba(255, 255, 255, 0.02);
  border: 1px solid rgba(255, 255, 255, 0.04);
  padding: 1rem 1.5rem;
  border-radius: 12px;
  font-size: 0.875rem;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(12px);
  transition: transform 0.4s var(--easing-snappy), background 0.4s var(--easing-snappy), border-color 0.4s var(--easing-snappy);
  cursor: default;
}

.card:hover {
  transform: translateY(-4px);
  background: rgba(255, 255, 255, 0.04);
  border-color: rgba(255, 255, 255, 0.1);
}

/* Reduced Motion Support */
@media (prefers-reduced-motion: reduce) {
  .animate-in {
    opacity: 0;
    transform: none;
    transition: opacity 0.5s ease;
  }
  
  .is-mounted .animate-in {
    transform: none;
  }
  
  .wave-shimmer {
    animation: none;
  }
  
  .flip-enter-active,
  .flip-leave-active {
    transition: opacity 0.3s ease;
  }
  
  .flip-enter-from,
  .flip-leave-to {
    transform: none;
    opacity: 0;
  }
  
  .snappy-btn:active, .card:hover {
    transform: none;
  }
}

@media (max-width: 640px) {
  .vip-form {
    flex-direction: column;
  }
  
  .countdown-blocks {
    gap: 1rem;
  }
  
  .time-block {
    min-width: 80px;
    padding: 1rem;
  }
  
  .time-block .value {
    font-size: 2rem;
  }
  
  .value-wrapper {
    height: 2.25rem;
  }
}
</style>
