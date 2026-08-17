<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const testimonials = [
  { 
    id: 'klaus', 
    avatar: '🇩🇪', 
    name: 'Klaus & Monika W.', 
    country: 'Munich, Germany', 
    flag: '🇩🇪',
    tour: 'Nile Cruise & Luxor',
    verified: true
  },
  { 
    id: 'marco', 
    avatar: '🇮🇹', 
    name: 'Marco & Giulia F.', 
    country: 'Milan, Italy', 
    flag: '🇮🇹',
    tour: 'Red Sea Diving VIP',
    verified: true
  },
  { 
    id: 'natalia', 
    avatar: '🇷🇺', 
    name: 'Natalia & Dmitri K.', 
    country: 'Moscow, Russia', 
    flag: '🇷🇺',
    tour: 'Pyramids Safari',
    verified: true
  }
]
</script>

<template>
  <section class="luxury-testimonials-section" id="testimonials">
    <!-- Warm Sunset & Coastal Cream Gradients Background -->
    <div class="sunset-ambient-glow glow-top"></div>
    <div class="sunset-ambient-glow glow-bottom"></div>
    <div class="sunset-wave-pattern"></div>

    <div class="testimonials-container">
      <div class="section-header" v-reveal="'reveal-fade-up'">
        <div class="section-eyebrow">
          <span class="eyebrow-sparkle">✧</span>
          {{ t('testimonials.eyebrow') }}
          <span class="eyebrow-sparkle">✧</span>
        </div>
        <h2 class="section-title">
          <span v-html="t('testimonials.title')"></span>
        </h2>
        <p class="section-sub">
          Discover the unforgettable memories crafted for our esteemed guests.
        </p>
      </div>

      <div class="testimonials-grid stagger-container" v-reveal="'stagger-container'">
        <div v-for="test in testimonials" :key="test.id" class="luxury-glass-card">
          <!-- Ambient Hover Glow -->
          <div class="card-ambient-hover-glow"></div>
          
          <div class="card-header">
            <div class="stars-container">
              <span class="star-icon" v-for="i in 5" :key="i">★</span>
            </div>
            <div v-if="test.verified" class="verified-badge">
              <span class="v-check">✓</span> Verified Guest
            </div>
          </div>

          <div class="testimonial-content">
            <span class="quote-mark">“</span>
            <p class="testimonial-text">
              {{ t(`testimonials.items.${test.id}.text`) }}
            </p>
          </div>

          <!-- Tour Reference Pill -->
          <div class="tour-ref-pill">
            <span class="tour-icon">🧭</span>
            <span>{{ test.tour }}</span>
          </div>

          <div class="card-footer">
            <div class="guest-avatar-wrapper">
              <div class="guest-avatar">{{ test.avatar }}</div>
              <div class="avatar-ring"></div>
            </div>
            <div class="guest-info">
              <div class="guest-name">{{ test.name }}</div>
              <div class="guest-location">
                <span class="guest-flag">{{ test.flag }}</span>
                <span>{{ test.country }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* ─── VIBRANT PRESTIGIOUS TESTIMONIALS ─── */
.luxury-testimonials-section {
  position: relative;
  padding: 120px 0;
  background: linear-gradient(135deg, #fdfbf7 0%, #fff7ed 50%, #fef3c7 100%); /* Coastal cream & sunset gold base */
  overflow: hidden;
  color: #1e293b;
}

/* Ambient Sunset Glows */
.sunset-ambient-glow {
  position: absolute;
  border-radius: 50%;
  filter: blur(100px);
  pointer-events: none;
  z-index: 0;
}

.glow-top {
  width: 700px;
  height: 700px;
  top: -20%;
  left: -10%;
  background: radial-gradient(circle, rgba(251, 146, 60, 0.15) 0%, transparent 70%); /* Warm sunset orange */
}

.glow-bottom {
  width: 800px;
  height: 800px;
  bottom: -20%;
  right: -10%;
  background: radial-gradient(circle, rgba(217, 119, 6, 0.12) 0%, transparent 70%); /* Warm sunset gold */
}

.sunset-wave-pattern {
  position: absolute;
  inset: 0;
  background-image: repeating-linear-gradient(
    45deg,
    rgba(201, 168, 76, 0.03) 0px,
    rgba(201, 168, 76, 0.03) 2px,
    transparent 2px,
    transparent 12px
  );
  opacity: 0.6;
  z-index: 1;
}

.testimonials-container {
  position: relative;
  z-index: 10;
  max-width: 1360px;
  margin: 0 auto;
  padding: 0 24px;
}

/* Header */
.section-header {
  text-align: center;
  margin-bottom: 70px;
}

.section-eyebrow {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  font-family: var(--font-sans, system-ui);
  font-size: 13px;
  letter-spacing: 0.25em;
  text-transform: uppercase;
  color: #d97706; /* Rich gold/orange */
  font-weight: 700;
  margin-bottom: 24px;
}

.eyebrow-sparkle {
  color: #f59e0b;
  font-size: 14px;
}

.section-title {
  font-family: 'Playfair Display', 'Cormorant', var(--font-serif-display, serif);
  font-size: clamp(2.5rem, 4vw, 3.5rem);
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 20px;
  line-height: 1.2;
}

:deep(.section-title em) {
  font-style: italic;
  color: #d97706;
}

.section-sub {
  font-family: var(--font-serif-accent, serif);
  font-size: 20px;
  color: #475569;
  max-width: 680px;
  margin: 0 auto;
  line-height: 1.8;
  font-style: italic;
}

/* Frosted Glass Cards */
.testimonials-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 32px;
}

.luxury-glass-card {
  position: relative;
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.9);
  border-radius: 24px;
  padding: 40px 32px;
  box-shadow: 0 20px 40px rgba(217, 119, 6, 0.05), inset 0 0 0 1px rgba(255, 255, 255, 0.5);
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.card-ambient-hover-glow {
  position: absolute;
  inset: 0;
  background: radial-gradient(circle at 50% 0%, rgba(251, 146, 60, 0.1) 0%, transparent 70%);
  opacity: 0;
  transition: opacity 0.4s ease;
  pointer-events: none;
}

.luxury-glass-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 30px 60px rgba(217, 119, 6, 0.1), inset 0 0 0 1px rgba(255, 255, 255, 1);
  border-color: rgba(217, 119, 6, 0.2);
}

.luxury-glass-card:hover .card-ambient-hover-glow {
  opacity: 1;
}

/* Header: Stars & Verified Badge */
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.stars-container {
  display: flex;
  gap: 2px;
}

.star-icon {
  color: #f59e0b; /* Amber */
  font-size: 16px;
}

.verified-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background: rgba(16, 185, 129, 0.1);
  color: #059669;
  padding: 4px 10px;
  border-radius: 999px;
  font-family: var(--font-sans, system-ui);
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border: 1px solid rgba(16, 185, 129, 0.2);
}

.v-check {
  font-weight: 900;
}

/* Quote Text */
.testimonial-content {
  position: relative;
  flex-grow: 1;
  margin-bottom: 28px;
}

.quote-mark {
  position: absolute;
  top: -20px;
  left: -15px;
  font-family: 'Georgia', serif;
  font-size: 80px;
  color: rgba(217, 119, 6, 0.1);
  line-height: 1;
  pointer-events: none;
}

.testimonial-text {
  position: relative;
  z-index: 1;
  font-family: var(--font-serif-accent, serif);
  font-size: 18px;
  color: #334155;
  line-height: 1.7;
  font-style: italic;
}

/* Tour Ref Pill */
.tour-ref-pill {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  align-self: flex-start;
  background: rgba(245, 158, 11, 0.1);
  border: 1px solid rgba(245, 158, 11, 0.2);
  color: #b45309;
  padding: 6px 14px;
  border-radius: 8px;
  font-family: var(--font-sans, system-ui);
  font-size: 12px;
  font-weight: 600;
  margin-bottom: 28px;
  transition: all 0.3s ease;
}

.luxury-glass-card:hover .tour-ref-pill {
  background: rgba(245, 158, 11, 0.15);
  border-color: rgba(245, 158, 11, 0.3);
}

.tour-icon {
  font-size: 14px;
}

/* Footer: Avatar & Info */
.card-footer {
  display: flex;
  align-items: center;
  gap: 16px;
  padding-top: 24px;
  border-top: 1px solid rgba(15, 23, 42, 0.06);
}

.guest-avatar-wrapper {
  position: relative;
}

.guest-avatar {
  width: 54px;
  height: 54px;
  border-radius: 50%;
  background: #ffffff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  position: relative;
  z-index: 2;
  transition: transform 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.avatar-ring {
  position: absolute;
  inset: -4px;
  border-radius: 50%;
  border: 1.5px solid #f59e0b;
  opacity: 0.3;
  z-index: 1;
  transition: all 0.4s ease;
}

.luxury-glass-card:hover .guest-avatar {
  transform: scale(1.08);
}

.luxury-glass-card:hover .avatar-ring {
  opacity: 0.8;
  transform: scale(1.05);
}

.guest-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.guest-name {
  font-family: var(--font-sans, system-ui);
  font-size: 15px;
  font-weight: 700;
  color: #0f172a;
}

.guest-location {
  display: flex;
  align-items: center;
  gap: 6px;
  font-family: var(--font-sans, system-ui);
  font-size: 13px;
  color: #64748b;
  font-weight: 500;
}

.guest-flag {
  font-size: 14px;
}

/* Responsive */
@media (max-width: 1100px) {
  .testimonials-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .luxury-testimonials-section {
    padding: 80px 0;
  }
  .testimonials-grid {
    grid-template-columns: 1fr;
    gap: 24px;
  }
}
</style>
