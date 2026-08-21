<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<template>
  <footer>
    <div class="footer-grid">
      <div class="footer-brand">
        <div class="logo-text">
          <span class="brand">SeeDora Travel</span>
          <span class="tagline">✦ EGYPT · LUXURY EXPERIENCES ✦</span>
        </div>
        <p class="footer-desc">
          {{ t('footer.description') }}
        </p>
        <div class="social-links">
          <a class="social-link" href="#">📘</a>
          <a class="social-link" href="#">📸</a>
          <a class="social-link" href="#">🐦</a>
          <a class="social-link" href="#">💼</a>
        </div>
      </div>
      <div class="footer-col">
        <h4>
          {{ t('footer.sections.destinations') }}
        </h4>
        <ul>
          <li><a href="#destinations">🌊 Hurghada</a></li>
          <li><a href="#destinations">🏛️ Cairo & Pyramids</a></li>
          <li><a href="#destinations">🏺 Luxor & Aswan</a></li>
          <li><a href="#destinations">🌴 Sharm El-Sheikh</a></li>
        </ul>
      </div>
      <div class="footer-col">
        <h4>
          {{ t('footer.sections.tours') }}
        </h4>
        <ul>
          <li><a href="#trips">🤿 Diving & Snorkeling</a></li>
          <li><a href="#trips">🏜️ Desert Safari</a></li>
          <li><a href="#trips">🛳️ Nile Cruise</a></li>
          <li><a href="#trips">🏛️ Cultural Tours</a></li>
        </ul>
      </div>
      <div class="footer-col">
        <h4>
          {{ t('footer.sections.contact') }}
        </h4>
        <ul>
          <li><a href="tel:+201001296641">📞 +20 100 129 6641</a></li>
          <li><a href="mailto:info@seadoratravel.com">✉️ info@seadoratravel.com</a></li>
          <li><a href="#">📍 Hurghada, Egypt</a></li>
          <li><a href="#">💬 WhatsApp Us</a></li>
        </ul>
      </div>
    </div>
    <div class="footer-bottom">
      <span>© 2025 SeeDora Travel. {{ t('footer.rights') }}</span>
      <span>
        <a href="#">{{ t('footer.privacy') }}</a> · <a href="#">{{ t('footer.terms') }}</a> · 
        <span>{{ t('footer.madeWith') }}</span>
      </span>
    </div>
  </footer>
</template>

<style scoped>
/* ─── FOOTER ─── */
footer {
  background: var(--dark); color: rgba(255,255,255,0.6);
  padding: 64px 48px 32px;
}
.footer-grid { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr; gap: 48px; margin-bottom: 48px; }
.footer-brand .logo-text .brand { font-size: 26px; display: block; color: var(--white); margin-bottom: 4px; font-family: 'Playfair Display', serif; font-weight: 700; }
.footer-brand .logo-text .tagline { font-size: 11px; color: var(--gold); letter-spacing: 0.2em; text-transform: uppercase; }
.footer-desc { font-size: 14px; line-height: 1.7; margin-top: 18px; margin-bottom: 24px; }
.social-links { display: flex; gap: 12px; }
.social-link {
  width: 38px; height: 38px; border-radius: 8px;
  background: rgba(255,255,255,0.06); border: 1px solid rgba(255,255,255,0.1);
  display: flex; align-items: center; justify-content: center; font-size: 16px;
  text-decoration: none; transition: all 0.2s;
}
.social-link:hover { background: var(--sea); border-color: var(--sea); }
.footer-col h4 {
  font-family: 'Playfair Display', serif;
  font-size: 16px; color: var(--white); margin-bottom: 20px;
}
.footer-col ul { list-style: none; }
.footer-col ul li { margin-bottom: 10px; }
.footer-col ul li a {
  color: rgba(255,255,255,0.5); text-decoration: none;
  font-size: 14px; transition: color 0.2s;
}
.footer-col ul li a:hover { color: var(--sun-light); }
.footer-bottom {
  border-top: 1px solid rgba(255,255,255,0.08);
  padding-top: 28px; display: flex; justify-content: space-between; align-items: center;
  font-size: 12px;
}
.footer-bottom a { color: rgba(255,255,255,0.4); text-decoration: none; }

@media (max-width: 1100px) {
  .footer-grid { grid-template-columns: 1fr 1fr; }
}
@media (max-width: 768px) {
  .footer-grid { grid-template-columns: 1fr; }
}
</style>
