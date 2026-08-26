<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'

const isMounted = ref(false)

// Smooth mouse tracking for flare and parallax
const mouseX = ref(0)
const mouseY = ref(0)
const targetX = ref(0)
const targetY = ref(0)
let animationFrameId = 0

const animate = () => {
  // Smooth spring interpolation
  const ease = 0.06
  mouseX.value += (targetX.value - mouseX.value) * ease
  mouseY.value += (targetY.value - mouseY.value) * ease
  animationFrameId = requestAnimationFrame(animate)
}

const handleMouseMove = (e: MouseEvent) => {
  targetX.value = e.clientX
  targetY.value = e.clientY
}

// Countdown logic
const targetDate = new Date()
targetDate.setDate(targetDate.getDate() + 42)

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

// Particles
const getParticleStyle = () => {
  const size = Math.random() * 4 + 2
  const left = Math.random() * 100
  const duration = Math.random() * 5 + 4 // 4s to 9s
  const delay = Math.random() * 5
  return {
    width: `${size}px`,
    height: `${size}px`,
    left: `${left}%`,
    animationDuration: `${duration}s`,
    animationDelay: `${delay}s`
  }
}

// Floating pills
const pills = [
  { text: 'Maldives', duration: '3s', parallaxFactor: 0.02 },
  { text: 'Santorini', duration: '4.2s', parallaxFactor: -0.03 },
  { text: 'Bora Bora', duration: '5.5s', parallaxFactor: 0.04 },
  { text: 'Amalfi Coast', duration: '6s', parallaxFactor: -0.02 }
]

// Note: mouseX/Y is already interpolated for smooth parallax
const normalizedMouseX = computed(() => {
  if (typeof window === 'undefined') return 0;
  return (mouseX.value / window.innerWidth) * 2 - 1
})
const normalizedMouseY = computed(() => {
  if (typeof window === 'undefined') return 0;
  return (mouseY.value / window.innerHeight) * 2 - 1
})

const getParallaxTransform = (factor: number) => {
  return `translate(${normalizedMouseX.value * factor * 100}px, ${normalizedMouseY.value * factor * 100}px)`
}

// CTA Magnetic Hover Effect
const ctaRef = ref<HTMLElement | null>(null)
const ctaTransform = ref('')
const handleCtaMouseMove = (e: MouseEvent) => {
  if (!ctaRef.value) return
  const rect = ctaRef.value.getBoundingClientRect()
  const x = e.clientX - rect.left - rect.width / 2
  const y = e.clientY - rect.top - rect.height / 2
  ctaTransform.value = `translate(${x * 0.2}px, ${y * 0.2}px) scale(1.02)`
}
const handleCtaMouseLeave = () => {
  ctaTransform.value = 'translate(0px, 0px) scale(1)'
}

onMounted(() => {
  targetX.value = window.innerWidth / 2
  targetY.value = window.innerHeight / 2
  mouseX.value = targetX.value
  mouseY.value = targetY.value

  window.addEventListener('mousemove', handleMouseMove)
  animationFrameId = requestAnimationFrame(animate)

  setTimeout(() => {
    isMounted.value = true
  }, 100)

  updateCountdown()
  timerInterval = setInterval(updateCountdown, 1000)
})

onUnmounted(() => {
  window.removeEventListener('mousemove', handleMouseMove)
  cancelAnimationFrame(animationFrameId)
  clearInterval(timerInterval)
})
</script>

<template>
  <div class="coming-soon-wrapper">
    <!-- Background Elements -->
    <div class="background-gradient"></div>
    <div class="water-caustics"></div>
    <div class="light-rays"></div>

    <!-- Giant Background Watermark -->
    <div class="giant-watermark">SEADORA</div>

    <!-- Sparkling Sunlight Particles -->
    <div class="particles">
      <div v-for="i in 25" :key="i" class="particle" :style="getParticleStyle()"></div>
    </div>

    <!-- Floating Travel Pills (Background parallax layer) -->
    <div class="floating-pills-container">
      <div 
        v-for="(pill, index) in pills" 
        :key="index"
        class="pill-parallax-wrapper"
        :class="`pill-pos-${index + 1}`"
        :style="{ transform: getParallaxTransform(pill.parallaxFactor) }"
      >
        <div class="floating-pill" :style="{ animationDuration: pill.duration }">
          {{ pill.text }}
        </div>
      </div>
    </div>

    <!-- Interactive Light Trail / Sunshine Flare -->
    <div class="sunshine-flare" :style="{ transform: `translate(${mouseX}px, ${mouseY}px)` }"></div>

    <!-- Main Content -->
    <div class="content-container" :class="{ 'is-mounted': isMounted }">
      <div class="logo animate-in stagger-1">
        Seadora Travel
      </div>
      
      <h1 class="tagline animate-in stagger-2">
        Redefining Luxury<br />Marine Journeys
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
      
      <div class="action-area animate-in stagger-4">
        <button 
          ref="ctaRef"
          class="luxury-cta" 
          :style="{ transform: ctaTransform }"
          @mousemove="handleCtaMouseMove"
          @mouseleave="handleCtaMouseLeave"
        >
          Discover Our World
        </button>
      </div>

      <div class="teaser-cards animate-in stagger-5">
        <div class="card">Exclusive Yachts</div>
        <div class="card">Tailored Itineraries</div>
      </div>
    </div>
  </div>
</template>

<style scoped>
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
  overflow: hidden;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
}

/* 1. Bright turquoise gradient */
.background-gradient {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, #00d2ff 0%, #0277bd 100%);
  z-index: -4;
}

/* 1. Water caustics / shimmering */
.water-caustics {
  position: absolute;
  inset: 0;
  background-image: radial-gradient(circle at 50% 50%, rgba(255,255,255,0.4) 0%, transparent 60%);
  mix-blend-mode: overlay;
  z-index: -3;
  animation: caustics-shimmer 8s infinite alternate ease-in-out;
}

@keyframes caustics-shimmer {
  0% { transform: scale(1); opacity: 0.5; }
  100% { transform: scale(1.15); opacity: 0.8; }
}

/* 1. Radiant light beam rays */
.light-rays {
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: repeating-linear-gradient(
    45deg,
    transparent,
    transparent 15px,
    rgba(255, 255, 255, 0.05) 15px,
    rgba(255, 255, 255, 0.05) 30px
  );
  animation: rays-rotate 90s linear infinite;
  z-index: -2;
  pointer-events: none;
  mix-blend-mode: soft-light;
}

@keyframes rays-rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

/* 2. Giant background watermark */
.giant-watermark {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: clamp(8rem, 20vw, 25rem);
  font-weight: 900;
  color: rgba(255, 255, 255, 0.06);
  white-space: nowrap;
  pointer-events: none;
  z-index: -1;
  animation: breathing-watermark 12s ease-in-out infinite;
  letter-spacing: -0.05em;
}

@keyframes breathing-watermark {
  0%, 100% { transform: translate(-50%, -50%) scale(1) rotate(0deg); }
  50% { transform: translate(-50%, -50%) scale(1.06) rotate(1deg); }
}

/* 3. Floating travel pills */
.floating-pills-container {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 1;
}

.pill-parallax-wrapper {
  position: absolute;
  will-change: transform;
}

.pill-pos-1 { top: 20%; left: 15%; }
.pill-pos-2 { top: 30%; right: 15%; }
.pill-pos-3 { bottom: 25%; left: 20%; }
.pill-pos-4 { bottom: 20%; right: 20%; }

.floating-pill {
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.4);
  padding: 0.6rem 1.5rem;
  border-radius: 999px;
  font-weight: 600;
  font-size: 0.9rem;
  color: #fff;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  animation: float-pill ease-in-out infinite alternate;
  will-change: transform;
}

@keyframes float-pill {
  0% { transform: translateY(0px); }
  100% { transform: translateY(-25px); }
}

/* 4. Interactive light trail / sunshine flare */
.sunshine-flare {
  position: absolute;
  top: -200px;
  left: -200px;
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.5) 0%, rgba(255, 255, 255, 0) 70%);
  pointer-events: none;
  z-index: 5;
  mix-blend-mode: overlay;
  will-change: transform;
}

/* 5. Sparkling sunlight particles */
.particles {
  position: absolute;
  inset: 0;
  pointer-events: none;
  overflow: hidden;
  z-index: 1;
}

.particle {
  position: absolute;
  bottom: -20px;
  background: #fff;
  border-radius: 50%;
  box-shadow: 0 0 12px rgba(255, 255, 255, 0.9);
  animation: particle-rise linear infinite;
  opacity: 0;
}

@keyframes particle-rise {
  0% { transform: translateY(0) scale(0.5); opacity: 0; }
  20% { opacity: 0.8; }
  80% { opacity: 0.8; }
  100% { transform: translateY(-100vh) scale(1.5); opacity: 0; }
}

/* Content Container */
.content-container {
  position: relative;
  z-index: 10;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: 2.5rem;
  max-width: 900px;
  padding: 2rem;
  width: 100%;
}

.animate-in {
  opacity: 0;
  transform: translateY(15px) scale(0.96);
  transition: opacity 0.8s var(--easing-smooth), transform 0.8s var(--easing-smooth);
}

.is-mounted .animate-in {
  opacity: 1;
  transform: translateY(0) scale(1);
}

.is-mounted .stagger-1 { transition-delay: 50ms; }
.is-mounted .stagger-2 { transition-delay: 150ms; }
.is-mounted .stagger-3 { transition-delay: 250ms; }
.is-mounted .stagger-4 { transition-delay: 350ms; }
.is-mounted .stagger-5 { transition-delay: 450ms; }

.logo {
  font-size: 0.9rem;
  font-weight: 700;
  letter-spacing: 0.2em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.9);
  margin-bottom: -1rem;
}

.tagline {
  font-size: clamp(2.5rem, 6vw, 4.5rem);
  font-weight: 800;
  color: #fff;
  line-height: 1.1;
  letter-spacing: -0.02em;
  text-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

/* Countdown Blocks */
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
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  padding: 1.25rem 1.5rem;
  border-radius: 16px;
  min-width: 110px;
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  color: #fff;
}

.value-wrapper {
  height: 3.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.time-block .value {
  font-size: 3rem;
  font-weight: 700;
  line-height: 1;
  font-variant-numeric: tabular-nums;
  display: inline-block;
  text-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.flip-enter-active,
.flip-leave-active {
  transition: all 0.5s cubic-bezier(0.23, 1, 0.32, 1);
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
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.15em;
  color: rgba(255, 255, 255, 0.8);
  margin-top: 0.75rem;
  font-weight: 600;
}

/* 6. Luxury Interactive CTA */
.action-area {
  margin-top: 1rem;
  display: flex;
  justify-content: center;
}

.luxury-cta {
  position: relative;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.5);
  color: #fff;
  padding: 1.2rem 3rem;
  font-size: 1.125rem;
  font-weight: 700;
  letter-spacing: 0.05em;
  border-radius: 9999px;
  cursor: pointer;
  overflow: hidden;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  /* Use transform for magnetic effect, which is set inline via vue.
     The transition here is for when we leave the button. */
  transition: transform 0.4s var(--easing-snappy), background 0.4s ease, box-shadow 0.4s ease;
  will-change: transform;
}

.luxury-cta:hover {
  background: rgba(255, 255, 255, 0.3);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.2);
}

/* Active press physics */
.luxury-cta:active {
  transform: scale(0.97) !important; 
}

/* Shining shimmer border/sweep */
.luxury-cta::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 50%;
  height: 100%;
  background: linear-gradient(
    to right,
    transparent,
    rgba(255, 255, 255, 0.6),
    transparent
  );
  transform: skewX(-20deg);
}

@keyframes sunburstPulse {
  0% { opacity: 0.6; transform: scale(0.95); }
  100% { opacity: 1; transform: scale(1.05); }
}

@keyframes watermarkBreathe {
  0% { transform: translate(-50%, -50%) scale(0.96) rotate(-2deg); opacity: 0.10; }
  100% { transform: translate(-50%, -50%) scale(1.05) rotate(2deg); opacity: 0.18; }
}

@keyframes rotateHalo {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes floatCard {
  0% { transform: translateY(0); }
  100% { transform: translateY(-8px); }
}

@keyframes floatSparkle {
  0%, 100% { opacity: 0; transform: translateY(0) scale(0.7); }
  50% { opacity: 1; transform: translateY(-30px) scale(1.2); }
}

@keyframes liveDotPulse {
  0%, 100% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.35); opacity: 0.7; }
}

@keyframes ringPulse {
  0% { transform: scale(0.95); opacity: 0.6; }
  100% { transform: scale(1.15); opacity: 1; }
}

@keyframes ringExpand {
  0% { transform: scale(0.8); opacity: 1; }
  100% { transform: scale(2.8); opacity: 0; }
}

/* ==========================================================================
   Reduced Motion Accessibility
   ========================================================================== */
@media (prefers-reduced-motion: reduce) {
  .ocean-caustics,
  .sunburst-beams,
  .watermark-logo-container,
  .watermark-halo,
  .sparkle,
  .luxury-card,
  .emblem-glow-ring,
  .pulse-ring {
    animation: none !important;
  }

  .animate-node {
    opacity: 1 !important;
    transform: none !important;
    transition: none !important;
  }
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .top-bar {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  .footer-bar {
    flex-direction: column;
    text-align: center;
    gap: 0.75rem;
  }

  .status-banner {
    border-radius: 24px;
    padding: 1rem 1.25rem;
  }

  .status-content {
    flex-direction: column;
    text-align: center;
  }

  .status-text {
    text-align: center;
  }
}
</style>
