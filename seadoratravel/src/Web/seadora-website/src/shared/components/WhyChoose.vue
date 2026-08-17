<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import LuxuryIcons from '@/shared/components/LuxuryIcons.vue'

const { t } = useI18n()

const reasons = [
  { id: 'languages', borderColor: 'border-sea' },
  { id: 'safety', borderColor: 'border-sun' },
  { id: 'luxury', borderColor: 'border-grass' },
  { id: 'price', borderColor: 'border-gold' }
]
</script>

<template>
  <section class="luxury-about-section" id="why">
    <!-- Ambient glowing backgrounds -->
    <div class="luxury-ambient-glow glow-1"></div>
    <div class="luxury-ambient-glow glow-2"></div>
    <div class="luxury-ambient-glow glow-3"></div>

    <div class="luxury-container">
      <div class="luxury-header" v-reveal="'reveal-fade-up'">
        <div class="luxury-eyebrow">
          <span class="eyebrow-sparkle">✦</span>
          {{ t('why.eyebrow') }}
          <span class="eyebrow-sparkle">✦</span>
        </div>
        <h2 class="luxury-title">
          <span v-html="t('why.title')"></span>
        </h2>
        <p class="luxury-sub">
          {{ t('why.description') }}
        </p>
      </div>

      <div class="luxury-features-grid stagger-container" v-reveal="'stagger-container'">
        <div 
          v-for="reason in reasons" 
          :key="reason.id"
          class="luxury-feature-card"
        >
          <div class="luxury-card-glow-border"></div>
          
          <div class="luxury-icon-wrapper">
            <div class="icon-inner-glow"></div>
            <LuxuryIcons v-if="reason.id === 'languages'" name="search" size="36" stroke-width="1.5" class="luxury-icon" />
            <LuxuryIcons v-else-if="reason.id === 'safety'" name="heart" size="36" stroke-width="1.5" class="luxury-icon" />
            <LuxuryIcons v-else-if="reason.id === 'luxury'" name="star" size="36" stroke-width="1.5" class="luxury-icon" />
            <LuxuryIcons v-else-if="reason.id === 'price'" name="calendar" size="36" stroke-width="1.5" class="luxury-icon" />
          </div>

          <h3 class="luxury-feature-title">
            {{ t(`why.items.${reason.id}.title`) }}
          </h3>
          <p class="luxury-feature-text">
            {{ t(`why.items.${reason.id}.text`) }}
          </p>

          <!-- Decorative subtle line -->
          <div class="luxury-feature-divider"></div>
        </div>
      </div>
      
      <!-- Trust Badges Row -->
      <div class="luxury-trust-badges" v-reveal="'reveal-fade-up'">
        <div class="trust-badge">
          <span class="trust-check">✓</span>
          <span class="trust-text">Verified Luxury Standard</span>
        </div>
        <div class="trust-badge">
          <span class="trust-check">✓</span>
          <span class="trust-text">Premium Concierge</span>
        </div>
        <div class="trust-badge">
          <span class="trust-check">✓</span>
          <span class="trust-text">Bespoke Experiences</span>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* ─── ULTRA-LUXURY EDITORIAL ABOUT US ─── */
.luxury-about-section {
  position: relative;
  padding: 120px 0;
  background: #02111d; /* Deep Ocean Blue Base */
  overflow: hidden;
  color: #fff;
}

/* Ambient Glows */
.luxury-ambient-glow {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  pointer-events: none;
  z-index: 0;
}

.glow-1 {
  width: 600px;
  height: 600px;
  top: -10%;
  left: -10%;
  background: radial-gradient(circle, rgba(201, 168, 76, 0.12) 0%, transparent 70%);
}

.glow-2 {
  width: 800px;
  height: 800px;
  bottom: -20%;
  right: -10%;
  background: radial-gradient(circle, rgba(14, 165, 233, 0.08) 0%, transparent 70%);
}

.glow-3 {
  width: 500px;
  height: 500px;
  top: 40%;
  left: 40%;
  background: radial-gradient(circle, rgba(230, 202, 101, 0.05) 0%, transparent 60%);
}

.luxury-container {
  position: relative;
  z-index: 10;
  max-width: 1360px;
  margin: 0 auto;
  padding: 0 24px;
}

/* Header */
.luxury-header {
  text-align: center;
  margin-bottom: 70px;
}

.luxury-eyebrow {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  font-family: var(--font-sans, system-ui);
  font-size: 13px;
  letter-spacing: 0.25em;
  text-transform: uppercase;
  color: #c9a84c;
  font-weight: 600;
  margin-bottom: 24px;
}

.eyebrow-sparkle {
  color: #e6ca65;
  font-size: 10px;
}

.luxury-title {
  font-family: 'Playfair Display', 'Cormorant', var(--font-serif-display, serif);
  font-size: clamp(2.5rem, 4vw, 3.5rem);
  font-weight: 400;
  color: #ffffff;
  margin-bottom: 24px;
  line-height: 1.2;
}

:deep(.luxury-title em) {
  font-style: italic;
  color: #c9a84c;
}

.luxury-sub {
  font-family: var(--font-serif-accent, serif);
  font-size: 20px;
  color: #a0aec0;
  max-width: 680px;
  margin: 0 auto;
  line-height: 1.8;
  font-style: italic;
}

/* Glassmorphism Feature Cards */
.luxury-features-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 32px;
}

.luxury-feature-card {
  position: relative;
  padding: 45px 32px;
  background: rgba(255, 255, 255, 0.02);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 20px;
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  text-align: center;
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  overflow: hidden;
}

.luxury-card-glow-border {
  position: absolute;
  inset: 0;
  border-radius: 20px;
  padding: 1px;
  background: linear-gradient(135deg, rgba(201, 168, 76, 0.4) 0%, transparent 50%, rgba(201, 168, 76, 0.1) 100%);
  -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
  mask-composite: exclude;
  opacity: 0;
  transition: opacity 0.4s ease;
}

.luxury-feature-card:hover {
  transform: translateY(-8px);
  background: rgba(255, 255, 255, 0.04);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.3), 0 0 30px rgba(201, 168, 76, 0.1);
}

.luxury-feature-card:hover .luxury-card-glow-border {
  opacity: 1;
}

.luxury-icon-wrapper {
  position: relative;
  width: 72px;
  height: 72px;
  margin: 0 auto 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  background: rgba(201, 168, 76, 0.05);
  border: 1px solid rgba(201, 168, 76, 0.2);
}

.icon-inner-glow {
  position: absolute;
  inset: 0;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(201, 168, 76, 0.2) 0%, transparent 70%);
  opacity: 0;
  transition: opacity 0.4s ease;
}

.luxury-feature-card:hover .icon-inner-glow {
  opacity: 1;
}

.luxury-icon {
  color: #c9a84c;
  z-index: 1;
  transition: transform 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.luxury-feature-card:hover .luxury-icon {
  transform: scale(1.1);
  color: #e6ca65;
}

.luxury-feature-title {
  font-family: 'Playfair Display', 'Cormorant', var(--font-serif-display, serif);
  font-size: 22px;
  font-weight: 500;
  color: #ffffff;
  margin-bottom: 16px;
  letter-spacing: 0.02em;
}

.luxury-feature-text {
  font-family: var(--font-sans, system-ui);
  font-size: 14px;
  color: #a0aec0;
  line-height: 1.7;
  font-weight: 300;
}

.luxury-feature-divider {
  width: 40px;
  height: 1px;
  background: rgba(201, 168, 76, 0.3);
  margin: 24px auto 0;
  transition: width 0.4s ease, background 0.4s ease;
}

.luxury-feature-card:hover .luxury-feature-divider {
  width: 60px;
  background: rgba(201, 168, 76, 0.8);
}

/* Trust Badges */
.luxury-trust-badges {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
  gap: 32px;
  margin-top: 70px;
  padding-top: 40px;
  border-top: 1px solid rgba(255, 255, 255, 0.06);
}

.trust-badge {
  display: flex;
  align-items: center;
  gap: 10px;
}

.trust-check {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  background: rgba(201, 168, 76, 0.15);
  border: 1px solid rgba(201, 168, 76, 0.4);
  border-radius: 50%;
  color: #c9a84c;
  font-size: 12px;
  font-weight: bold;
}

.trust-text {
  font-family: var(--font-sans, system-ui);
  font-size: 13px;
  color: #cbd5e1;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 1100px) {
  .luxury-features-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .luxury-about-section {
    padding: 80px 0;
  }
  .luxury-features-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  .luxury-trust-badges {
    flex-direction: column;
    gap: 16px;
    align-items: flex-start;
    padding-left: 20px;
  }
}
</style>
