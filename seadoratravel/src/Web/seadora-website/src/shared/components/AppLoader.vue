<script setup lang="ts">
import { ref, onMounted } from 'vue'

const isVisible = ref(true)
const isFadingOut = ref(false)
const progress = ref(0)

onMounted(() => {
  // Animate progress smoothly
  const interval = setInterval(() => {
    if (progress.value < 100) {
      progress.value += Math.floor(Math.random() * 15) + 10
      if (progress.value > 100) progress.value = 100
    } else {
      clearInterval(interval)
      setTimeout(() => {
        isFadingOut.value = true
        setTimeout(() => {
          isVisible.value = false
        }, 700) // matches fade-out duration
      }, 400)
    }
  }, 100)
})
</script>

<template>
  <Transition name="loader-fade">
    <div v-if="isVisible" class="seadora-global-loader" :class="{ 'fade-out': isFadingOut }">
      <!-- Ambient Background Glows -->
      <div class="glow-orb glow-top"></div>
      <div class="glow-orb glow-bottom"></div>
      
      <!-- Subtle Pharaonic Geometric Backdrop -->
      <div class="loader-backdrop-pattern"></div>

      <div class="loader-center-content">
        <!-- Logo Emblem with Golden Spinner Orbit -->
        <div class="emblem-container">
          <!-- Animated SVG Golden Ring -->
          <svg class="ring-svg" viewBox="0 0 160 160">
            <circle class="ring-track" cx="80" cy="80" r="72" />
            <circle class="ring-fill" cx="80" cy="80" r="72" />
          </svg>

          <!-- Golden Pulsing Aura Glow -->
          <div class="emblem-aura"></div>

          <!-- Official Extracted Logo Emblem -->
          <div class="emblem-img-wrapper">
            <img src="/logo-emblem.png" alt="Seadora" class="emblem-img" />
          </div>
        </div>

        <!-- Brand Typography -->
        <div class="brand-text-block">
          <h1 class="brand-title">SEADORA</h1>
          <div class="brand-divider">
            <span class="star-accent">✦</span>
            <span class="brand-sub">TRAVEL · EGYPT</span>
            <span class="star-accent">✦</span>
          </div>
          <p class="brand-slogan">Where the Red Sea Becomes Your Story</p>
        </div>

        <!-- Luxury Progress Bar -->
        <div class="progress-bar-container">
          <div class="progress-bar-track">
            <div class="progress-bar-fill" :style="{ width: `${progress}%` }">
              <div class="progress-shimmer"></div>
            </div>
          </div>
          <div class="progress-info">
            <span class="progress-label">Preparing Luxury Journey</span>
            <span class="progress-pct">{{ progress }}%</span>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.seadora-global-loader {
  position: fixed;
  inset: 0;
  z-index: 99999;
  background: radial-gradient(circle at center, #0a2540 0%, #06192a 50%, #030d17 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  transition: opacity 0.7s cubic-bezier(0.16, 1, 0.3, 1), transform 0.7s cubic-bezier(0.16, 1, 0.3, 1);
}

.seadora-global-loader.fade-out {
  opacity: 0;
  transform: scale(1.04);
  pointer-events: none;
}

/* Ambient Glow Orbs */
.glow-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  pointer-events: none;
}
.glow-top {
  width: 400px;
  height: 400px;
  top: -100px;
  left: 50%;
  transform: translateX(-50%);
  background: radial-gradient(circle, rgba(201, 168, 76, 0.25) 0%, rgba(6, 45, 77, 0) 70%);
}
.glow-bottom {
  width: 500px;
  height: 500px;
  bottom: -150px;
  left: 50%;
  transform: translateX(-50%);
  background: radial-gradient(circle, rgba(0, 168, 204, 0.15) 0%, rgba(6, 45, 77, 0) 70%);
}

.loader-backdrop-pattern {
  position: absolute;
  inset: 0;
  background-image: 
    radial-gradient(rgba(201, 168, 76, 0.08) 1px, transparent 1px);
  background-size: 32px 32px;
  pointer-events: none;
  opacity: 0.6;
}

.loader-center-content {
  position: relative;
  z-index: 10;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 420px;
  padding: 0 24px;
}

/* Emblem Container & Orbit Spinner */
.emblem-container {
  position: relative;
  width: 140px;
  height: 140px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 24px;
}

.ring-svg {
  position: absolute;
  inset: -10px;
  width: calc(100% + 20px);
  height: calc(100% + 20px);
  animation: rotateSpinner 6s linear infinite;
}

.ring-track {
  fill: none;
  stroke: rgba(201, 168, 76, 0.15);
  stroke-width: 2.5;
}

.ring-fill {
  fill: none;
  stroke: url(#goldGradient);
  stroke: #c9a84c;
  stroke-width: 2.5;
  stroke-linecap: round;
  stroke-dasharray: 140 320;
  animation: dashPulse 2.5s ease-in-out infinite;
}

@keyframes rotateSpinner {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes dashPulse {
  0% { stroke-dashoffset: 0; stroke-dasharray: 60 380; }
  50% { stroke-dashoffset: -180; stroke-dasharray: 200 240; }
  100% { stroke-dashoffset: -460; stroke-dasharray: 60 380; }
}

.emblem-aura {
  position: absolute;
  inset: 10px;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(201, 168, 76, 0.35) 0%, rgba(201, 168, 76, 0) 75%);
  animation: auraPulse 3s ease-in-out infinite;
}

@keyframes auraPulse {
  0%, 100% { transform: scale(0.95); opacity: 0.6; }
  50% { transform: scale(1.15); opacity: 1; }
}

.emblem-img-wrapper {
  width: 110px;
  height: 110px;
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  justify-content: center;
  filter: drop-shadow(0 0 15px rgba(201, 168, 76, 0.4));
  animation: emblemFloat 4s ease-in-out infinite;
}

.emblem-img {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

@keyframes emblemFloat {
  0%, 100% { transform: translateY(0px) scale(1); }
  50% { transform: translateY(-4px) scale(1.02); }
}

/* Brand Typography */
.brand-text-block {
  margin-bottom: 28px;
}

.brand-title {
  font-family: 'Playfair Display', 'Cinzel', serif;
  font-size: 32px;
  font-weight: 800;
  letter-spacing: 0.22em;
  color: #ffffff;
  margin: 0;
  text-shadow: 0 0 20px rgba(255, 255, 255, 0.4), 0 2px 8px rgba(0, 0, 0, 0.8);
  animation: textGlow 3s ease-in-out infinite;
}

@keyframes textGlow {
  0%, 100% { letter-spacing: 0.22em; text-shadow: 0 0 15px rgba(201, 168, 76, 0.3); }
  50% { letter-spacing: 0.26em; text-shadow: 0 0 28px rgba(201, 168, 76, 0.7); }
}

.brand-divider {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin: 6px 0 8px;
}

.star-accent {
  color: #c9a84c;
  font-size: 10px;
}

.brand-sub {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.32em;
  color: #c9a84c;
  text-transform: uppercase;
}

.brand-slogan {
  font-family: 'Cormorant Garamond', serif;
  font-style: italic;
  font-size: 14.5px;
  color: rgba(255, 255, 255, 0.8);
  margin: 0;
  letter-spacing: 0.04em;
}

/* Progress Bar */
.progress-bar-container {
  width: 100%;
  max-width: 280px;
}

.progress-bar-track {
  width: 100%;
  height: 4px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 999px;
  overflow: hidden;
  position: relative;
  border: 1px solid rgba(201, 168, 76, 0.2);
}

.progress-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, #9b7829, #c9a84c, #ffd875, #c9a84c);
  border-radius: 999px;
  position: relative;
  transition: width 0.2s ease-out;
}

.progress-shimmer {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.6), transparent);
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(100%); }
}

.progress-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 8px;
  font-size: 10.5px;
  letter-spacing: 0.08em;
  color: rgba(255, 255, 255, 0.65);
}

.progress-pct {
  color: #c9a84c;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
}

/* Vue Transition */
.loader-fade-enter-active,
.loader-fade-leave-active {
  transition: opacity 0.6s ease;
}
.loader-fade-enter-from,
.loader-fade-leave-to {
  opacity: 0;
}
</style>
