<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import LuxuryIcons from '@/shared/components/LuxuryIcons.vue'

const { t } = useI18n()
</script>

<template>
  <footer class="luxury-footer">
    <div class="footer-inner">
      <!-- Left: Brand -->
      <router-link to="/" class="footer-brand flex items-center gap-3 text-decoration-none group">
        <img src="/logo-emblem.png" alt="Seadora Travel" class="w-10 h-10 object-contain drop-shadow-[0_0_8px_rgba(201,168,76,0.35)] group-hover:scale-105 transition-transform" />
        <div class="flex flex-col justify-center">
          <span class="brand-name font-serif text-sm sm:text-base font-bold text-white tracking-[0.06em]">SEADORA TRAVEL</span>
          <span class="tagline text-[9px] text-[#c9a84c] tracking-[0.16em] uppercase mt-0.5">Where the Red Sea Becomes Your Story</span>
        </div>
      </router-link>

      <!-- Center: Links -->
      <nav class="footer-links hidden md:flex">
        <router-link to="/tours" class="hover-underline">{{ t('nav.tours') || t('footer.sections.tours') || 'Tours' }}</router-link>
        <a href="/#destinations" class="hover-underline">{{ t('nav.destinations') || 'Destinations' }}</a>
        <a href="/#contact" class="hover-underline">{{ t('footer.sections.contact') || 'Contact' }}</a>
      </nav>

      <!-- Right: Contact / Social / Copyright -->
      <div class="footer-actions">
        <a href="https://wa.me/201068940967" target="_blank" rel="noopener noreferrer" class="whatsapp-link" aria-label="WhatsApp">
          <LuxuryIcons name="phone" size="14" color="currentColor" />
          <span class="hidden lg:inline">WhatsApp</span>
        </a>
        <div class="social-icons">
          <a href="https://www.facebook.com/share/1ERTGyUJvs/?mibextid=wwXIfr" target="_blank" rel="noopener noreferrer" aria-label="Facebook">
            <LuxuryIcons name="facebook" size="14" color="currentColor" />
          </a>
          <a href="https://www.instagram.com/seadora.travel.egypt?igsh=MWMzaGpsbDV6NGQzNw==" target="_blank" rel="noopener noreferrer" aria-label="Instagram">
            <LuxuryIcons name="instagram" size="14" color="currentColor" />
          </a>
          <a href="https://www.tiktok.com/@seadora.travel.eg?_r=1&_t=ZS-99IAMFd3PRl" target="_blank" rel="noopener noreferrer" aria-label="TikTok">
            <LuxuryIcons name="tiktok" size="14" color="currentColor" />
          </a>
        </div>
        <span class="copyright">© 2026 SeeDora Travel</span>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.luxury-footer {
  position: relative;
  z-index: 40;
  background: rgba(6, 45, 77, 0.95);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-top: 1px solid rgba(201, 168, 76, 0.2);
  color: var(--white);
  height: auto;
  min-height: 56px;
  display: flex;
  align-items: center;
  padding: 0 24px;
}
@media (min-width: 640px) {
  .luxury-footer {
    padding: 0 48px;
  }
}

.footer-inner {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
  padding: 12px 0;
}

.footer-brand {
  display: flex;
  align-items: center;
  gap: 8px;
}
.brand-name {
  font-family: var(--font-serif-display);
  font-weight: 700;
  font-size: 16px;
  color: var(--white);
}
.separator {
  color: var(--gold);
}
.tagline {
  font-family: var(--font-sans);
  font-size: 10px;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--gold);
}
.hidden {
  display: none;
}
@media (min-width: 640px) {
  .sm\:inline {
    display: inline;
  }
}

.footer-links {
  display: flex;
  gap: 24px;
  align-items: center;
}
@media (max-width: 768px) {
  .md\:flex {
    display: none;
  }
}
.footer-links a {
  font-family: var(--font-sans);
  font-size: 12px;
  color: rgba(255, 255, 255, 0.75);
  text-decoration: none;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  transition: color 0.3s ease;
}
.footer-links a:hover {
  color: var(--gold);
}
.hover-underline {
  position: relative;
}
.hover-underline::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 0;
  width: 0;
  height: 1px;
  background: var(--gold);
  transition: width 0.3s ease;
}
.hover-underline:hover::after {
  width: 100%;
}

.footer-actions {
  display: flex;
  align-items: center;
  gap: 16px;
}
@media (min-width: 1024px) {
  .lg\:inline {
    display: inline;
  }
}
@media (max-width: 1023px) {
  .lg\:inline {
    display: none;
  }
}
.whatsapp-link {
  display: flex;
  align-items: center;
  gap: 6px;
  color: rgba(255, 255, 255, 0.85);
  font-family: var(--font-sans);
  font-size: 12px;
  text-decoration: none;
  transition: color 0.3s ease;
}
.whatsapp-link:hover {
  color: #25D366;
}
.social-icons {
  display: flex;
  align-items: center;
  gap: 12px;
  border-left: 1px solid rgba(255, 255, 255, 0.15);
  border-right: 1px solid rgba(255, 255, 255, 0.15);
  padding: 0 16px;
}
.social-icons a {
  color: rgba(255, 255, 255, 0.65);
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}
.social-icons a:hover {
  color: var(--gold);
  transform: translateY(-2px);
}
.copyright {
  font-family: var(--font-sans);
  font-size: 11px;
  color: rgba(255, 255, 255, 0.5);
}

@media (max-width: 768px) {
  .footer-inner {
    justify-content: space-between;
  }
  .social-icons {
    border-right: none;
    padding-right: 0;
  }
}
@media (max-width: 640px) {
  .luxury-footer {
    padding: 0 16px;
  }
  .social-icons {
    padding: 0 12px;
  }
}
</style>
