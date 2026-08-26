<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import logoEmblem from '@/assets/logo-emblem.png'
import logoHorizontal from '@/assets/logo-horizontal.png'
import logoFull from '@/assets/logo-full.png'

const mouseX = ref(0)
const mouseY = ref(0)
const targetX = ref(0)
const targetY = ref(0)
const isMounted = ref(false)
const timeString = ref('')
let animationFrameId = 0
let timeInterval: ReturnType<typeof setInterval>

// Smooth momentum spring spotlight for sunshine flare
const animateSpotlight = () => {
  const ease = 0.08
  mouseX.value += (targetX.value - mouseX.value) * ease
  mouseY.value += (targetY.value - mouseY.value) * ease
  animationFrameId = requestAnimationFrame(animateSpotlight)
}

const handleMouseMove = (e: MouseEvent) => {
  targetX.value = e.clientX
  targetY.value = e.clientY
}

// Update Egypt Local Time (Africa/Cairo)
const updateEgyptTime = () => {
  const now = new Date()
  timeString.value = now.toLocaleTimeString('en-US', {
    timeZone: 'Africa/Cairo',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: true
  })
}

// Interactive floating tourism experiences
const travelExperiences = [
  { icon: '⛵', title: 'Ultra-Luxury Yacht Charters', subtitle: 'Private Red Sea Cruising & VIP Crew', tag: 'VIP Fleet' },
  { icon: '🤿', title: 'Pristine Coral Reef Expeditions', subtitle: 'Guided Diving & Marine Safaris', tag: 'Exclusive' },
  { icon: '🏝️', title: 'VIP Island & Coastal Escapes', subtitle: 'Giftun, Utopia & Orange Bay', tag: 'Paradise' },
  { icon: '✨', title: 'Bespoke 24/7 Red Sea Concierge', subtitle: 'Tailored Luxury Itineraries', tag: 'Ultra-Luxe' }
]

const activeCardIndex = ref<number | null>(null)

onMounted(() => {
  window.addEventListener('mousemove', handleMouseMove)
  targetX.value = window.innerWidth / 2
  targetY.value = window.innerHeight / 2
  mouseX.value = targetX.value
  mouseY.value = targetY.value

  animationFrameId = requestAnimationFrame(animateSpotlight)

  setTimeout(() => {
    isMounted.value = true
  }, 80)

  updateEgyptTime()
  timeInterval = setInterval(updateEgyptTime, 1000)
})

onUnmounted(() => {
  window.removeEventListener('mousemove', handleMouseMove)
  cancelAnimationFrame(animationFrameId)
  clearInterval(timeInterval)
})
</script>

<template>
  <div 
    class="coming-soon-root"
    :style="{ '--mouse-x': mouseX + 'px', '--mouse-y': mouseY + 'px' }"
  >
    <!-- Dynamic Sunshine / Seafoam Flare Following Cursor -->
    <div class="sun-flare"></div>

    <!-- Radiant Tropical Ocean Background with Caustics Wave -->
    <div class="ocean-caustics"></div>
    <div class="sunburst-beams"></div>

    <!-- Giant Glowing Watermark Logo in Background -->
    <div class="watermark-logo-container" aria-hidden="true">
      <img :src="logoEmblem" alt="" class="watermark-emblem" />
      <div class="watermark-halo"></div>
    </div>

    <!-- Floating Shimmering Marine Sparkles -->
    <div class="sparkles-container" aria-hidden="true">
      <span class="sparkle s1"></span>
      <span class="sparkle s2"></span>
      <span class="sparkle s3"></span>
      <span class="sparkle s4"></span>
      <span class="sparkle s5"></span>
      <span class="sparkle s6"></span>
    </div>

    <!-- Main Content Presentation -->
    <main class="main-wrapper" :class="{ 'is-visible': isMounted }">
      
      <!-- Top Navigation & Location Indicator -->
      <header class="top-bar animate-node stagger-1">
        <div class="brand-badge">
          <img :src="logoHorizontal || logoFull" alt="Seadora Travel" class="brand-logo-img" />
        </div>

        <div class="location-pill">
          <span class="live-dot"></span>
          <span class="location-text">Hurghada & Red Sea (27.2579° N, 33.8116° E)</span>
          <span class="time-separator">•</span>
          <span class="time-text">{{ timeString || 'Egypt Local Time' }}</span>
        </div>
      </header>

      <!-- Center Hero Presentation -->
      <section class="hero-section">
        
        <!-- Glowing Floating Emblem -->
        <div class="emblem-centerpiece animate-node stagger-2">
          <div class="emblem-glow-ring"></div>
          <img :src="logoEmblem" alt="Seadora Emblem" class="hero-emblem-img" />
          <div class="emblem-shimmer"></div>
        </div>

        <!-- Super Luxury Typography -->
        <div class="headline-container animate-node stagger-3">
          <div class="pre-badge">
            <span class="golden-star">★</span>
            <span>EXQUISITE MARINE TRAVEL & EXPEDITIONS</span>
            <span class="golden-star">★</span>
          </div>

          <h1 class="main-title">
            A NEW WAVE OF <br />
            <span class="gradient-text-gold">LUXURY IS DAWNING</span>
          </h1>

          <p class="subtitle">
            Crafting private superyacht charters, secluded coral reef adventures, and 
            unmatched VIP concierge experiences across Egypt’s most breathtaking crystal waters.
          </p>
        </div>

        <!-- Interactive Floating Tourism Cards -->
        <div class="tourism-grid animate-node stagger-4">
          <div 
            v-for="(item, idx) in travelExperiences" 
            :key="idx"
            class="luxury-card"
            :class="[`float-delay-${idx + 1}`, { 'is-active': activeCardIndex === idx }]"
            @mouseenter="activeCardIndex = idx"
            @mouseleave="activeCardIndex = null"
          >
            <div class="card-inner">
              <div class="card-top">
                <span class="card-icon">{{ item.icon }}</span>
                <span class="card-tag">{{ item.tag }}</span>
              </div>
              <h3 class="card-title">{{ item.title }}</h3>
              <p class="card-desc">{{ item.subtitle }}</p>
              <div class="card-glint"></div>
            </div>
          </div>
        </div>

        <!-- Luxury Status & Exploration Invitation -->
        <div class="status-banner animate-node stagger-5">
          <div class="status-content">
            <div class="status-pulse">
              <span class="pulse-ring"></span>
              <span class="pulse-core"></span>
            </div>
            <div class="status-text">
              <span class="status-label">Grand Portal Launch</span>
              <span class="status-desc">Summer 2026 • Curating the Finest Coastal Sanctuaries</span>
            </div>
          </div>
        </div>

      </section>

      <!-- Footer Elements -->
      <footer class="footer-bar animate-node stagger-6">
        <p class="copyright-text">
          © 2026 Seadora Travel Luxury Marine Group. All rights reserved.
        </p>
        <div class="social-tags">
          <span class="tag-link">#SeadoraLuxury</span>
          <span class="tag-link">#RedSeaYachts</span>
          <span class="tag-link">#VIPExpeditions</span>
        </div>
      </footer>

    </main>
  </div>
</template>

<style scoped>
/* ==========================================================================
   CSS Variables & Foundations (Bright, Vibrant Luxury Tourism Palette)
   ========================================================================== */
.coming-soon-root {
  --primary-azure: #0066FF;
  --tropical-cyan: #00D2FF;
  --crystal-turquoise: #00F2FE;
  --royal-navy: #031B33;
  --deep-ocean: #011224;
  --gold-glow: #F3C64F;
  --gold-rich: #D4AF37;
  --sunshine-light: rgba(255, 248, 220, 0.85);
  --glass-bg: rgba(255, 255, 255, 0.18);
  --glass-border: rgba(255, 255, 255, 0.45);
  --glass-shadow: 0 16px 48px -8px rgba(0, 70, 140, 0.25);
  --easing-natural: cubic-bezier(0.23, 1, 0.32, 1);
  --easing-spring: cubic-bezier(0.34, 1.56, 0.64, 1);

  position: relative;
  min-height: 100vh;
  width: 100%;
  overflow-x: hidden;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #FFFFFF;
  font-family: -apple-system, BlinkMacSystemFont, 'Inter', 'Segoe UI', Roboto, sans-serif;
  background: radial-gradient(120% 100% at 50% 10%, #00C6FF 0%, #0072FF 50%, #031D38 100%);
  user-select: none;
}

/* ==========================================================================
   Animated Background Layers (Ocean Caustics, Sunbeams & Watermark)
   ========================================================================== */
.ocean-caustics {
  position: absolute;
  inset: 0;
  background: 
    radial-gradient(ellipse 80% 50% at 50% 0%, rgba(255, 255, 255, 0.45) 0%, transparent 70%),
    linear-gradient(135deg, rgba(0, 242, 254, 0.25) 0%, rgba(0, 102, 255, 0.3) 100%);
  mix-blend-mode: overlay;
  pointer-events: none;
  animation: causticsWave 16s ease-in-out infinite alternate;
}

.sunburst-beams {
  position: absolute;
  top: -20%;
  left: 10%;
  right: 10%;
  height: 80%;
  background: radial-gradient(circle at 50% 0%, rgba(255, 250, 220, 0.35) 0%, rgba(0, 210, 255, 0.1) 45%, transparent 70%);
  filter: blur(40px);
  pointer-events: none;
  animation: sunburstPulse 10s ease-in-out infinite alternate;
}

/* Interactive Cursor Sunshine Flare */
.sun-flare {
  position: absolute;
  width: 600px;
  height: 600px;
  left: calc(var(--mouse-x) - 300px);
  top: calc(var(--mouse-y) - 300px);
  border-radius: 50%;
  background: radial-gradient(circle, rgba(255, 245, 180, 0.35) 0%, rgba(0, 242, 254, 0.15) 40%, transparent 70%);
  pointer-events: none;
  filter: blur(35px);
  z-index: 1;
  transition: opacity 0.4s ease;
}

/* Giant Background Watermark Logo */
.watermark-logo-container {
  position: absolute;
  top: 48%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: min(85vw, 720px);
  height: min(85vw, 720px);
  pointer-events: none;
  z-index: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0.12;
  animation: watermarkBreathe 12s ease-in-out infinite alternate;
}

.watermark-emblem {
  width: 100%;
  height: 100%;
  object-fit: contain;
  filter: brightness(2) drop-shadow(0 0 80px rgba(0, 242, 254, 0.8));
}

.watermark-halo {
  position: absolute;
  inset: -15%;
  border-radius: 50%;
  background: conic-gradient(from 0deg, rgba(255, 215, 0, 0.3), rgba(0, 242, 254, 0.5), rgba(255, 255, 255, 0.4), rgba(255, 215, 0, 0.3));
  filter: blur(70px);
  animation: rotateHalo 35s linear infinite;
}

/* Floating Sparkle Particles */
.sparkles-container {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 1;
}

.sparkle {
  position: absolute;
  width: 6px;
  height: 6px;
  background: #FFFFFF;
  border-radius: 50%;
  box-shadow: 0 0 12px 3px rgba(255, 255, 255, 0.9), 0 0 24px 6px rgba(0, 242, 254, 0.8);
  opacity: 0;
  animation: floatSparkle 7s ease-in-out infinite;
}

.sparkle.s1 { top: 20%; left: 15%; animation-delay: 0s; }
.sparkle.s2 { top: 35%; right: 18%; animation-delay: 1.8s; }
.sparkle.s3 { top: 65%; left: 22%; animation-delay: 3.2s; }
.sparkle.s4 { top: 75%; right: 28%; animation-delay: 4.5s; }
.sparkle.s5 { top: 15%; right: 35%; animation-delay: 2.2s; }
.sparkle.s6 { top: 80%; left: 45%; animation-delay: 5.5s; }

/* ==========================================================================
   Main Content Layout & Staggered Animations
   ========================================================================== */
.main-wrapper {
  position: relative;
  z-index: 2;
  width: 100%;
  max-width: 1240px;
  padding: 2.5rem 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  min-height: 100vh;
}

/* Emil Kowalski Stagger Entrance System */
.animate-node {
  opacity: 0;
  transform: translateY(14px) scale(0.97);
  transition: opacity 0.75s var(--easing-natural), transform 0.75s var(--easing-natural);
}

.main-wrapper.is-visible .animate-node {
  opacity: 1;
  transform: translateY(0) scale(1);
}

.stagger-1 { transition-delay: 0.05s; }
.stagger-2 { transition-delay: 0.12s; }
.stagger-3 { transition-delay: 0.20s; }
.stagger-4 { transition-delay: 0.28s; }
.stagger-5 { transition-delay: 0.36s; }
.stagger-6 { transition-delay: 0.44s; }

/* ==========================================================================
   Header & Location Pill
   ========================================================================== */
.top-bar {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.brand-badge {
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(16px);
  border: 1px solid var(--glass-border);
  padding: 0.5rem 1.25rem;
  border-radius: 999px;
  box-shadow: 0 8px 24px rgba(0, 30, 70, 0.15);
}

.brand-logo-img {
  height: 32px;
  width: auto;
  filter: drop-shadow(0 2px 8px rgba(0, 0, 0, 0.2));
}

.location-pill {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  background: rgba(2, 28, 56, 0.45);
  backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.25);
  padding: 0.45rem 1.15rem;
  border-radius: 999px;
  font-size: 0.825rem;
  font-weight: 500;
  color: #E2F1FF;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
}

.live-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #00FFA3;
  box-shadow: 0 0 10px #00FFA3;
  animation: liveDotPulse 2s infinite ease-in-out;
}

.time-separator {
  color: rgba(255, 255, 255, 0.4);
}

.time-text {
  color: #FFD700;
  font-weight: 600;
  font-variant-numeric: tabular-nums;
}

/* ==========================================================================
   Center Hero Presentation
   ========================================================================== */
.hero-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  margin: 2.5rem 0;
  width: 100%;
}

.emblem-centerpiece {
  position: relative;
  width: 104px;
  height: 104px;
  margin-bottom: 1.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.emblem-glow-ring {
  position: absolute;
  inset: -8px;
  border-radius: 50%;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.8), rgba(0, 242, 254, 0.9));
  filter: blur(14px);
  opacity: 0.8;
  animation: ringPulse 4s ease-in-out infinite alternate;
}

.hero-emblem-img {
  position: relative;
  width: 100%;
  height: 100%;
  object-fit: contain;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.9);
  padding: 10px;
  box-shadow: 0 12px 32px rgba(0, 40, 90, 0.35);
  transition: transform 0.4s var(--easing-natural);
}

.hero-emblem-img:hover {
  transform: scale(1.08) rotate(3deg);
}

.pre-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.18em;
  color: #FFE680;
  background: rgba(0, 30, 70, 0.4);
  backdrop-filter: blur(12px);
  padding: 0.35rem 1rem;
  border-radius: 999px;
  border: 1px solid rgba(255, 215, 0, 0.4);
  margin-bottom: 1.25rem;
  text-transform: uppercase;
}

.golden-star {
  color: #FFD700;
  font-size: 0.85rem;
}

.main-title {
  font-size: clamp(2.2rem, 5.5vw, 4.4rem);
  font-weight: 900;
  line-height: 1.1;
  letter-spacing: -0.02em;
  margin-bottom: 1.25rem;
  text-shadow: 0 4px 24px rgba(0, 20, 60, 0.45);
}

.gradient-text-gold {
  background: linear-gradient(135deg, #FFF6D6 0%, #FFD700 45%, #FFA500 80%, #FFFFFF 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 2px 14px rgba(255, 215, 0, 0.4));
}

.subtitle {
  max-width: 680px;
  font-size: clamp(1rem, 1.75vw, 1.2rem);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.92);
  margin: 0 auto 2.5rem;
  text-shadow: 0 2px 12px rgba(0, 20, 50, 0.3);
}

/* ==========================================================================
   Interactive Floating Tourism Feature Cards
   ========================================================================== */
.tourism-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 1.25rem;
  width: 100%;
  max-width: 1120px;
  margin-bottom: 2.5rem;
}

.luxury-card {
  position: relative;
  border-radius: 20px;
  padding: 1px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.6), rgba(255, 255, 255, 0.1) 60%, rgba(255, 215, 0, 0.4) 100%);
  box-shadow: var(--glass-shadow);
  cursor: pointer;
  transition: transform 0.4s var(--easing-natural), box-shadow 0.4s var(--easing-natural);
}

.luxury-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: 0 24px 48px -10px rgba(0, 100, 200, 0.4), 0 0 24px rgba(255, 215, 0, 0.3);
}

.luxury-card:active {
  transform: scale(0.97);
}

/* Floating Animation Delays simulating sea waves */
.float-delay-1 { animation: floatCard 5.5s ease-in-out infinite alternate; }
.float-delay-2 { animation: floatCard 6.2s ease-in-out infinite alternate 1s; }
.float-delay-3 { animation: floatCard 4.8s ease-in-out infinite alternate 0.5s; }
.float-delay-4 { animation: floatCard 5.8s ease-in-out infinite alternate 1.5s; }

.card-inner {
  position: relative;
  background: rgba(3, 30, 60, 0.45);
  backdrop-filter: blur(20px);
  border-radius: 19px;
  padding: 1.5rem 1.35rem;
  text-align: left;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  overflow: hidden;
}

.luxury-card:hover .card-inner {
  background: rgba(3, 40, 80, 0.6);
}

.card-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.card-icon {
  font-size: 1.85rem;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.2));
}

.card-tag {
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #FFE580;
  background: rgba(255, 215, 0, 0.2);
  border: 1px solid rgba(255, 215, 0, 0.4);
  padding: 0.2rem 0.65rem;
  border-radius: 999px;
}

.card-title {
  font-size: 1.05rem;
  font-weight: 700;
  color: #FFFFFF;
  margin-bottom: 0.35rem;
  line-height: 1.3;
}

.card-desc {
  font-size: 0.825rem;
  color: rgba(255, 255, 255, 0.78);
  line-height: 1.45;
}

.card-glint {
  position: absolute;
  top: 0;
  left: -100%;
  width: 60%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.25), transparent);
  transform: skewX(-20deg);
  transition: left 0.75s ease;
}

.luxury-card:hover .card-glint {
  left: 140%;
}

/* ==========================================================================
   Grand Launch Status Pill
   ========================================================================== */
.status-banner {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(18px);
  border: 1px solid var(--glass-border);
  padding: 0.75rem 1.75rem;
  border-radius: 999px;
  box-shadow: 0 10px 30px rgba(0, 40, 100, 0.2);
  display: inline-flex;
  align-items: center;
}

.status-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.status-pulse {
  position: relative;
  width: 14px;
  height: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.pulse-ring {
  position: absolute;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  background: #FFD700;
  opacity: 0.75;
  animation: ringExpand 2s cubic-bezier(0.24, 0, 0.38, 1) infinite;
}

.pulse-core {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #FFD700;
}

.status-text {
  display: flex;
  flex-direction: column;
  text-align: left;
}

.status-label {
  font-size: 0.75rem;
  font-weight: 800;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: #FFE680;
}

.status-desc {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.95);
  font-weight: 500;
}

/* ==========================================================================
   Footer Elements
   ========================================================================== */
.footer-bar {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.15);
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.75);
}

.social-tags {
  display: flex;
  gap: 1rem;
}

.tag-link {
  color: rgba(255, 255, 255, 0.85);
  font-weight: 500;
  transition: color 0.2s ease;
}

.tag-link:hover {
  color: #FFD700;
}

/* ==========================================================================
   Keyframe Animations
   ========================================================================== */
@keyframes causticsWave {
  0% { transform: scale(1) translateY(0); }
  100% { transform: scale(1.08) translateY(-15px); }
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
