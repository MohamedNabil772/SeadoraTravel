<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import LuxuryIcons from '@/shared/components/LuxuryIcons.vue'

const { t } = useI18n()
</script>

<template>
  <footer class="luxury-footer">
    <!-- Top Ambient Subtle Glow Line -->
    <div class="footer-ambient-border"></div>

    <div class="footer-container">
      <!-- MAIN FOOTER CONTENT -->
      <div class="footer-main-grid">
        
        <!-- Brand Identity Column (Mobile: Centered & Spacious, Desktop: Left Aligned) -->
        <div class="brand-section">
          <router-link to="/" class="brand-link group" aria-label="Seadora Travel Home">
            <div class="brand-emblem-wrapper">
              <img 
                src="/logo-emblem.png" 
                alt="Seadora Travel Emblem" 
                class="brand-emblem" 
              />
              <div class="emblem-glow"></div>
            </div>
            <div class="brand-text-block">
              <span class="brand-title">SEADORA TRAVEL</span>
              <span class="brand-tagline">Where the Red Sea Becomes Your Story</span>
            </div>
          </router-link>
          
          <p class="brand-desc hidden md:block">
            Curating bespoke nautical journeys, private charters, and timeless Red Sea expeditions.
          </p>
        </div>

        <!-- Quick Navigation Column -->
        <nav class="nav-section" aria-label="Footer Navigation">
          <router-link to="/tours" class="nav-item">
            <span>{{ t('nav.tours') || t('footer.sections.tours') || 'Tours' }}</span>
          </router-link>
          <a href="/#destinations" class="nav-item">
            <span>{{ t('nav.destinations') || 'Destinations' }}</span>
          </a>
          <router-link to="/portal" class="nav-item">
            <span>VIP Lounge</span>
          </router-link>
          <a href="/#contact" class="nav-item">
            <span>{{ t('footer.sections.contact') || 'Contact' }}</span>
          </a>
        </nav>

        <!-- Social Channels & Direct Contact -->
        <div class="actions-section">
          <div class="social-button-grid">
            <!-- WhatsApp -->
            <a 
              href="https://wa.me/201068940967" 
              target="_blank" 
              rel="noopener noreferrer" 
              class="social-btn whatsapp-btn group"
              aria-label="Contact Concierge on WhatsApp"
            >
              <LuxuryIcons name="phone" size="18" color="currentColor" stroke-width="1.8" />
              <span class="btn-tooltip">WhatsApp</span>
            </a>

            <!-- Facebook -->
            <a 
              href="https://www.facebook.com/share/1ERTGyUJvs/?mibextid=wwXIfr" 
              target="_blank" 
              rel="noopener noreferrer" 
              class="social-btn facebook-btn group"
              aria-label="Seadora on Facebook"
            >
              <LuxuryIcons name="facebook" size="18" color="currentColor" stroke-width="1.8" />
              <span class="btn-tooltip">Facebook</span>
            </a>

            <!-- Instagram -->
            <a 
              href="https://www.instagram.com/seadora.travel.egypt?igsh=MWMzaGpsbDV6NGQzNw==" 
              target="_blank" 
              rel="noopener noreferrer" 
              class="social-btn instagram-btn group"
              aria-label="Seadora on Instagram"
            >
              <LuxuryIcons name="instagram" size="18" color="currentColor" stroke-width="1.8" />
              <span class="btn-tooltip">Instagram</span>
            </a>

            <!-- TikTok -->
            <a 
              href="https://www.tiktok.com/@seadora.travel.eg?_r=1&_t=ZS-99IAMFd3PRl" 
              target="_blank" 
              rel="noopener noreferrer" 
              class="social-btn tiktok-btn group"
              aria-label="Seadora on TikTok"
            >
              <LuxuryIcons name="tiktok" size="18" color="currentColor" stroke-width="1.8" />
              <span class="btn-tooltip">TikTok</span>
            </a>
          </div>

          <!-- Direct Line Pill Badge -->
          <a 
            href="https://wa.me/201068940967" 
            target="_blank" 
            rel="noopener noreferrer" 
            class="direct-line-badge"
          >
            <span class="pulse-dot"></span>
            <span>Concierge: +20 106 894 0967</span>
          </a>
        </div>

      </div>

      <!-- BOTTOM BAR: DIVIDER & COPYRIGHT -->
      <div class="footer-bottom-bar">
        <div class="bottom-divider"></div>
        <div class="bottom-content">
          <p class="copyright-text">
            © 2026 SeeDora Travel. All rights reserved.
          </p>
          <div class="bottom-badges hidden sm:flex">
            <span class="badge-item">Luxury Red Sea Experiences</span>
            <span class="badge-dot">•</span>
            <span class="badge-item">Bespoke Expeditions</span>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.luxury-footer {
  position: relative;
  z-index: 30;
  background: linear-gradient(180deg, #05243d 0%, #03182b 100%);
  color: #ffffff;
  overflow: hidden;
  /* Extra bottom padding on mobile so floating concierge widget never collides */
  padding: 3rem 1.5rem calc(6.5rem + env(safe-area-inset-bottom, 0px)) 1.5rem;
}

@media (min-width: 768px) {
  .luxury-footer {
    padding: 3.5rem 2.5rem 2.5rem 2.5rem;
  }
}

@media (min-width: 1024px) {
  .luxury-footer {
    padding: 4rem 3.5rem 2.5rem 3.5rem;
  }
}

.footer-ambient-border {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent 0%, rgba(201, 168, 76, 0.4) 35%, rgba(201, 168, 76, 0.6) 50%, rgba(201, 168, 76, 0.4) 65%, transparent 100%);
}

.footer-container {
  max-width: 1280px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 2.5rem;
}

/* MAIN GRID */
.footer-main-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 2rem;
  align-items: center;
  text-align: center;
}

@media (min-width: 768px) {
  .footer-main-grid {
    grid-template-columns: 1.2fr 1fr 1.2fr;
    text-align: left;
    align-items: start;
    gap: 2.5rem;
  }
}

/* BRAND SECTION */
.brand-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
}

@media (min-width: 768px) {
  .brand-section {
    align-items: flex-start;
  }
}

.brand-link {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
}

@media (min-width: 640px) {
  .brand-link {
    flex-direction: row;
    gap: 1rem;
  }
}

.brand-emblem-wrapper {
  position: relative;
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.brand-emblem {
  width: 44px;
  height: 44px;
  object-fit: contain;
  filter: drop-shadow(0 0 10px rgba(201, 168, 76, 0.35));
  transition: transform 300ms cubic-bezier(0.16, 1, 0.3, 1);
}

.brand-link:hover .brand-emblem {
  transform: scale(1.06);
}

.emblem-glow {
  position: absolute;
  inset: -4px;
  border-radius: 9999px;
  background: radial-gradient(circle, rgba(201, 168, 76, 0.25) 0%, transparent 70%);
  pointer-events: none;
}

.brand-text-block {
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.brand-title {
  font-family: var(--font-serif-display, Georgia, serif);
  font-size: 1.125rem;
  font-weight: 700;
  letter-spacing: 0.12em;
  color: #ffffff;
  line-height: 1.2;
}

.brand-tagline {
  font-family: var(--font-sans, system-ui, sans-serif);
  font-size: 0.625rem;
  font-weight: 600;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: #c9a84c;
  margin-top: 0.25rem;
}

.brand-desc {
  font-size: 0.8125rem;
  color: rgba(255, 255, 255, 0.6);
  line-height: 1.6;
  max-width: 320px;
  margin-top: 0.25rem;
}

/* NAV LINKS */
.nav-section {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1.25rem 1.75rem;
}

@media (min-width: 768px) {
  .nav-section {
    flex-direction: column;
    justify-content: flex-start;
    align-items: flex-start;
    gap: 0.875rem;
  }
}

.nav-item {
  font-family: var(--font-sans, system-ui, sans-serif);
  font-size: 0.8125rem;
  font-weight: 600;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.75);
  text-decoration: none;
  position: relative;
  padding: 0.25rem 0;
  transition: color 200ms ease;
}

.nav-item:hover {
  color: #c9a84c;
}

.nav-item::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 1.5px;
  background: #c9a84c;
  transition: width 240ms cubic-bezier(0.23, 1, 0.32, 1);
}

.nav-item:hover::after {
  width: 100%;
}

/* ACTIONS / SOCIAL SECTION */
.actions-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.25rem;
}

@media (min-width: 768px) {
  .actions-section {
    align-items: flex-end;
  }
}

.social-button-grid {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.social-btn {
  position: relative;
  width: 44px;
  height: 44px;
  border-radius: 9999px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.12);
  color: rgba(255, 255, 255, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  text-decoration: none;
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  transition: transform 160ms cubic-bezier(0.23, 1, 0.32, 1),
              background-color 200ms ease,
              border-color 200ms ease,
              color 200ms ease,
              box-shadow 200ms ease;
}

.social-btn:hover {
  background: rgba(201, 168, 76, 0.15);
  border-color: rgba(201, 168, 76, 0.6);
  color: #c9a84c;
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(201, 168, 76, 0.2);
}

.social-btn:active {
  transform: scale(0.93);
}

.whatsapp-btn:hover {
  background: rgba(37, 211, 102, 0.15);
  border-color: rgba(37, 211, 102, 0.6);
  color: #25d366;
  box-shadow: 0 6px 20px rgba(37, 211, 102, 0.25);
}

.btn-tooltip {
  position: absolute;
  top: -30px;
  left: 50%;
  transform: translateX(-50%) translateY(4px);
  background: #03182b;
  border: 1px solid rgba(201, 168, 76, 0.3);
  color: #ffffff;
  font-size: 10px;
  font-weight: 600;
  letter-spacing: 0.05em;
  padding: 3px 8px;
  border-radius: 6px;
  opacity: 0;
  pointer-events: none;
  transition: opacity 150ms ease, transform 150ms ease;
  white-space: nowrap;
}

.social-btn:hover .btn-tooltip {
  opacity: 1;
  transform: translateX(-50%) translateY(0);
}

/* DIRECT LINE BADGE */
.direct-line-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.4rem 0.85rem;
  border-radius: 9999px;
  background: rgba(201, 168, 76, 0.08);
  border: 1px solid rgba(201, 168, 76, 0.25);
  color: rgba(255, 255, 255, 0.85);
  font-size: 0.6875rem;
  font-weight: 600;
  letter-spacing: 0.05em;
  text-decoration: none;
  transition: background-color 200ms ease, border-color 200ms ease, color 200ms ease;
}

.direct-line-badge:hover {
  background: rgba(201, 168, 76, 0.18);
  border-color: rgba(201, 168, 76, 0.5);
  color: #ffffff;
}

.pulse-dot {
  width: 6px;
  height: 6px;
  border-radius: 9999px;
  background: #25d366;
  box-shadow: 0 0 6px #25d366;
  animation: pulseGlow 2s infinite;
}

@keyframes pulseGlow {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.5; transform: scale(0.85); }
}

/* BOTTOM BAR */
.footer-bottom-bar {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.bottom-divider {
  width: 100%;
  height: 1px;
  background: linear-gradient(90deg, transparent 0%, rgba(255, 255, 255, 0.1) 20%, rgba(201, 168, 76, 0.25) 50%, rgba(255, 255, 255, 0.1) 80%, transparent 100%);
}

.bottom-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  text-align: center;
}

@media (min-width: 640px) {
  .bottom-content {
    flex-direction: row;
    text-align: left;
  }
}

.copyright-text {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.45);
  letter-spacing: 0.03em;
}

.bottom-badges {
  align-items: center;
  gap: 0.625rem;
  font-size: 0.6875rem;
  color: rgba(255, 255, 255, 0.4);
  letter-spacing: 0.05em;
  text-transform: uppercase;
}

.badge-dot {
  color: #c9a84c;
  opacity: 0.6;
}
</style>
