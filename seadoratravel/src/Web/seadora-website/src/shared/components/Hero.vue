<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'

const { t } = useI18n()
const router = useRouter()
const searchQuery = ref('')

const quickFilters = [
  { key: 'diving', icon: 'ðŸ¤¿', query: 'diving' },
  { key: 'luxor', icon: 'ðŸ›ï¸', query: 'luxor' },
  { key: 'cairo', icon: 'ðŸº', query: 'cairo' },
  { key: 'cruise', icon: 'â›µ', query: 'cruise' },
  { key: 'safari', icon: 'ðŸœï¸', query: 'safari' }
]

const handleSearch = () => {
  if (searchQuery.value.trim()) {
    router.push(`/tours?search=${encodeURIComponent(searchQuery.value.trim())}`)
  } else {
    router.push('/tours')
  }
}

const applyQuickFilter = (query: string) => {
  router.push(`/tours?search=${encodeURIComponent(query)}`)
}
</script>

<template>
  <section class="hero-section">
    <!-- Sun-drenched Vibrant Cinematic Backdrop with Pyramids, Golden Sun & Integrated Moving Red Sea Video -->
    <div class="hero-backdrop">
      <!-- Background Egypt Majestic Image (Pyramids & Golden Horizon) -->
      <img 
        src="/hero-egypt-majestic.jpg" 
        alt="Majestic Egypt - Pyramids, Sun & Turquoise Red Sea" 
        class="hero-bg-media"
        fetchpriority="high"
        loading="eager"
        decoding="async"
      />
      
      <!-- Bright, Luminous Sunlit Atmosphere Scrim -->
      <div class="hero-sunlight-scrim"></div>
      <div class="hero-sunbeam-glow"></div>
    </div>

    <!-- Sacred Golden Dot Pattern -->
    <div class="hero-sacred-pattern"></div>

    <!-- Main Content Container -->
    <div class="hero-container w-full max-w-[1360px] mx-auto px-6 sm:px-10 lg:px-12 relative z-10">
      <div class="hero-layout-wrapper pt-6 pb-14">
        
        <!-- Hero Headline & Search -->
        <div class="hero-left-content max-w-[840px]">
          
          <!-- Sunlit Royal Badge -->
          <div class="hero-royal-badge animate-fade-in-up">
            <span class="badge-sparkle">âœ¦</span>
            <span class="badge-text">{{ t('hero.badge') }}</span>
            <span class="badge-sparkle">âœ¦</span>
          </div>

          <!-- Bright & Radiant Headline -->
          <h1 class="hero-headline font-serif animate-fade-in-up delay-1">
            <span v-html="t('hero.title')"></span>
          </h1>

          <!-- High-Contrast Tagline -->
          <p class="hero-tagline font-sans animate-fade-in-up delay-2">
            {{ t('hero.description') }}
          </p>

          <!-- Bright Glassmorphic Search & Booking Bar -->
          <div class="hero-interactive-search animate-fade-in-up delay-3">
            <form @submit.prevent="handleSearch" class="search-box-form">
              <div class="search-input-group">
                <span class="search-icon-emblem">âœ¨</span>
                <input 
                  v-model="searchQuery" 
                  type="text" 
                  :placeholder="t('hero.searchPlaceholder')" 
                  class="hero-search-field"
                />
              </div>
              <button type="submit" class="hero-search-action-btn">
                <span>{{ t('hero.searchBtn') }}</span>
                <svg class="w-4 h-4 ml-1.5 transform transition-transform group-hover:translate-x-1" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M14 5l7 7m0 0l-7 7m7-7H3" />
                </svg>
              </button>
            </form>
          </div>

          <!-- Quick Discovery Tags -->
          <div class="hero-quick-discovery animate-fade-in-up delay-4">
            <span class="quick-title">{{ t('hero.trending') }}</span>
            <div class="quick-pills-list">
              <button 
                v-for="filter in quickFilters" 
                :key="filter.key"
                @click="applyQuickFilter(filter.query)"
                class="quick-pill-item"
              >
                <span class="pill-icon">{{ filter.icon }}</span>
                <span>{{ t('hero.quickTags.' + filter.key) }}</span>
              </button>
            </div>
          </div>

          <!-- Direct CTA Buttons -->
          <div class="hero-cta-group animate-fade-in-up delay-5">
            <router-link to="/tours" class="hero-btn-primary group">
              <span class="btn-text">{{ t('hero.browseAll') }}</span>
              <span class="btn-shine"></span>
              <svg class="w-4 h-4 transition-transform duration-300 group-hover:translate-x-1" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                <path stroke-linecap="round" stroke-linejoin="round" d="M17 8l4 4m0 0l-4 4m4-4H3" />
              </svg>
            </router-link>

            <a href="https://wa.me/201068940967?text=Hello%20SeeDora%20Travel,%20I%20would%20like%20to%20inquire%20about%20a%20luxury%20custom%20tour." target="_blank" rel="noopener noreferrer" class="hero-btn-vip-concierge">
              <span class="vip-pulse-dot"></span>
              <span>{{ t('hero.vipConcierge') }}</span>
            </a>
          </div>
        </div>

      </div>
    </div>

    <!-- Scroll Down Hint -->
    <div class="hero-scroll-indicator">
      <span class="scroll-label">{{ t('hero.scroll') }}</span>
      <div class="scroll-track">
        <div class="scroll-ball"></div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.hero-section {
  position: relative;
  min-height: 100vh;
  display: flex;
  align-items: center;
  overflow: hidden;
  padding: 120px 0 70px;
  background: #f4fbff;
}

/* Sun-drenched Cinematic Backdrop */
.hero-backdrop {
  position: absolute;
  inset: 0;
  overflow: hidden;
  z-index: 0;
}

.hero-bg-media {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center 35%;
  transform: scale(1.03);
  will-change: transform;
  animation: slowZoom 25s ease-in-out infinite alternate;
}

@keyframes slowZoom {
  0% { transform: scale(1.01) translate3d(0, 0, 0); }
  100% { transform: scale(1.06) translate3d(0, -8px, 0); }
}



/* Bright, Luminous Sunlit Scrim */
.hero-sunlight-scrim {
  position: absolute;
  inset: 0;
  z-index: 2;
  background: linear-gradient(
    90deg,
    rgba(255, 255, 255, 0.92) 0%,
    rgba(255, 255, 255, 0.8) 32%,
    rgba(255, 255, 255, 0.35) 55%,
    rgba(255, 255, 255, 0.05) 75%,
    transparent 100%
  );
  pointer-events: none;
}

.hero-sunbeam-glow {
  position: absolute;
  top: -10%;
  left: 20%;
  width: 60%;
  height: 70%;
  z-index: 2;
  background: radial-gradient(
    ellipse at top center,
    rgba(255, 237, 160, 0.35) 0%,
    rgba(255, 255, 255, 0.15) 50%,
    transparent 80%
  );
  pointer-events: none;
}

/* Dot Pattern */
.hero-sacred-pattern {
  position: absolute;
  inset: 0;
  background-image: radial-gradient(rgba(201, 168, 76, 0.15) 1px, transparent 1px);
  background-size: 32px 32px;
  pointer-events: none;
  opacity: 0.5;
  z-index: 2;
}

/* Layout Wrapper */
.hero-layout-wrapper {
  display: flex;
  align-items: center;
  width: 100%;
}

/* Left Content */
.hero-left-content {
  position: relative;
  z-index: 10;
}

.hero-royal-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: rgba(255, 255, 255, 0.95);
  border: 1px solid #c9a84c;
  padding: 6px 16px;
  border-radius: 999px;
  margin-bottom: 20px;
  box-shadow: 0 4px 16px rgba(201, 168, 76, 0.25);
  backdrop-filter: blur(8px);
}

.badge-sparkle {
  color: #c9a84c;
  font-size: 11px;
}

.badge-text {
  font-family: var(--font-sans, system-ui);
  font-size: 11.5px;
  font-weight: 800;
  letter-spacing: 0.15em;
  text-transform: uppercase;
  color: #8a6818;
}

.hero-headline {
  font-size: clamp(2.6rem, 5.2vw, 4.4rem);
  font-weight: 700;
  line-height: 1.12;
  color: #062d4d;
  letter-spacing: -0.01em;
  margin-bottom: 20px;
  text-shadow: 0 2px 20px rgba(255, 255, 255, 0.8);
}

:deep(.hero-headline .hero-gold-text) {
  background: linear-gradient(135deg, #b88a28 0%, #d4af37 50%, #996515 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  display: inline-block;
  font-style: italic;
  font-weight: 800;
}

:deep(.hero-headline .hero-highlight-red-sea) {
  color: #0284c7;
  display: inline-block;
}

.hero-tagline {
  font-size: clamp(1.05rem, 1.8vw, 1.25rem);
  color: #1e3a5f;
  font-weight: 500;
  line-height: 1.65;
  margin-bottom: 32px;
  max-width: 680px;
  text-shadow: 0 1px 10px rgba(255, 255, 255, 0.9);
}

/* Interactive Search Bar */
.hero-interactive-search {
  margin-bottom: 28px;
  max-width: 640px;
}

.search-box-form {
  display: flex;
  align-items: center;
  background: #ffffff;
  padding: 6px 6px 6px 18px;
  border-radius: 12px;
  border: 1.5px solid #c9a84c;
  box-shadow: 0 12px 36px rgba(6, 45, 77, 0.12), 0 0 20px rgba(201, 168, 76, 0.15);
  transition: all 0.3s ease;
}

.search-box-form:focus-within {
  border-color: #8a6818;
  box-shadow: 0 16px 44px rgba(6, 45, 77, 0.18), 0 0 28px rgba(201, 168, 76, 0.3);
}

.search-input-group {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
}

.search-icon-emblem {
  font-size: 18px;
}

.hero-search-field {
  width: 100%;
  background: transparent;
  border: none;
  outline: none;
  font-family: var(--font-sans, system-ui);
  font-size: 15px;
  font-weight: 600;
  color: #062d4d;
}

.hero-search-field::placeholder {
  color: #94a3b8;
  font-weight: 500;
}

.hero-search-action-btn {
  display: inline-flex;
  align-items: center;
  background: linear-gradient(135deg, #c9a84c 0%, #b88a28 100%);
  color: #ffffff;
  border: none;
  font-family: var(--font-sans, system-ui);
  font-size: 14px;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  padding: 13px 24px;
  border-radius: 8px;
  cursor: pointer;
  box-shadow: 0 4px 14px rgba(201, 168, 76, 0.35);
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  white-space: nowrap;
}

.hero-search-action-btn:hover {
  filter: brightness(1.08);
  transform: scale(1.02);
  box-shadow: 0 6px 20px rgba(201, 168, 76, 0.5);
}

/* Quick Discovery Tags */
.hero-quick-discovery {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 30px;
}

.quick-title {
  font-size: 11.5px;
  font-weight: 800;
  letter-spacing: 0.12em;
  color: #8a6818;
  text-transform: uppercase;
}

.quick-pills-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.quick-pill-item {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background: rgba(255, 255, 255, 0.85);
  border: 1px solid rgba(6, 45, 77, 0.12);
  backdrop-filter: blur(8px);
  color: #062d4d;
  font-size: 12.5px;
  font-weight: 700;
  padding: 7px 15px;
  border-radius: 999px;
  cursor: pointer;
  box-shadow: 0 2px 8px rgba(6, 45, 77, 0.06);
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
}

.quick-pill-item:hover {
  background: #ffffff;
  border-color: #c9a84c;
  color: #8a6818;
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(201, 168, 76, 0.25);
}

.pill-icon {
  font-size: 13px;
}

/* CTA Buttons */
.hero-cta-group {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.hero-btn-primary {
  position: relative;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  background: linear-gradient(135deg, #c9a84c 0%, #e2c87b 50%, #af8930 100%);
  color: #06192a;
  padding: 14px 32px;
  font-family: var(--font-sans, system-ui);
  font-size: 14px;
  font-weight: 800;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  border-radius: 8px;
  text-decoration: none;
  overflow: hidden;
  box-shadow: 0 8px 24px rgba(201, 168, 76, 0.4);
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.hero-btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 32px rgba(201, 168, 76, 0.6);
}

.btn-shine {
  position: absolute;
  top: 0; left: -100%;
  width: 60%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.5), transparent);
  transform: skewX(-25deg);
  transition: left 0.75s ease;
}

.hero-btn-primary:hover .btn-shine {
  left: 140%;
}

.hero-btn-vip-concierge {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  background: rgba(255, 255, 255, 0.9);
  border: 1.5px solid #25D366;
  color: #062d4d;
  padding: 14px 24px;
  font-family: var(--font-sans, system-ui);
  font-size: 13.5px;
  font-weight: 800;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  border-radius: 8px;
  text-decoration: none;
  box-shadow: 0 4px 16px rgba(37, 211, 102, 0.2);
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.hero-btn-vip-concierge:hover {
  background: #25D366;
  color: #ffffff;
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(37, 211, 102, 0.35);
}

.vip-pulse-dot {
  width: 8px;
  height: 8px;
  background: #25D366;
  border-radius: 50%;
  box-shadow: 0 0 10px #25D366;
  animation: pulseGreen 2s infinite;
}

.hero-btn-vip-concierge:hover .vip-pulse-dot {
  background: #ffffff;
  box-shadow: 0 0 10px #ffffff;
}

@keyframes pulseGreen {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.5; transform: scale(1.2); }
}

/* Scroll Indicator */
.hero-scroll-indicator {
  position: absolute;
  bottom: 20px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
  z-index: 10;
  color: #64748b;
  text-transform: uppercase;
  font-size: 9.5px;
  font-weight: 800;
  letter-spacing: 0.2em;
}

.scroll-track {
  width: 18px;
  height: 32px;
  border: 1.5px solid #c9a84c;
  border-radius: 999px;
  position: relative;
}

.scroll-ball {
  width: 4px;
  height: 6px;
  background: #c9a84c;
  border-radius: 999px;
  position: absolute;
  top: 4px;
  left: 50%;
  transform: translateX(-50%);
  animation: scrollDown 2s infinite cubic-bezier(0.65, 0, 0.35, 1);
}

@keyframes scrollDown {
  0% { top: 4px; opacity: 1; height: 6px; }
  50% { top: 15px; opacity: 1; height: 9px; }
  100% { top: 20px; opacity: 0; height: 4px; }
}

/* Staggered CSS Animation Helpers */
.animate-fade-in-up {
  opacity: 0;
  transform: translateY(20px);
  animation: fadeInUp 0.8s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.delay-1 { animation-delay: 0.12s; }
.delay-2 { animation-delay: 0.24s; }
.delay-3 { animation-delay: 0.36s; }
.delay-4 { animation-delay: 0.48s; }
.delay-5 { animation-delay: 0.6s; }

@keyframes fadeInUp {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive */
@media (max-width: 640px) {
  .hero-section {
    padding-top: 85px;
    padding-bottom: 35px;
  }
  .search-box-form {
    flex-direction: column;
    padding: 8px;
    gap: 8px;
  }
  .hero-search-action-btn {
    width: 100%;
    justify-content: center;
  }
  .hero-cta-group {
    flex-direction: column;
    width: 100%;
  }
  .hero-btn-primary, .hero-btn-vip-concierge {
    width: 100%;
    justify-content: center;
  }
  .hero-scroll-indicator {
    display: none;
  }
}
</style>
