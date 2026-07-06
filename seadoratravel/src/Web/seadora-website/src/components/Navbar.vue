<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { locale, t } = useI18n()

const setLang = (lang: string) => {
  locale.value = lang
}

const languages = [
  { code: 'en', label: '🇬🇧 EN' },
  { code: 'de', label: '🇩🇪 DE' },
  { code: 'it', label: '🇮🇹 IT' },
  { code: 'fr', label: '🇫🇷 FR' },
  { code: 'ru', label: '🇷🇺 RU' }
]
</script>

<template>
  <!-- LANGUAGE BAR -->
  <div class="lang-bar">
    <div class="contact-info">
      <span>📞 <a href="tel:+201001296641">+20 100 129 6641</a></span>
      <span>✉️ <a href="mailto:info@sedoratravel.com">info@sedoratravel.com</a></span>
    </div>
    <div class="lang-selector">
      <button 
        v-for="lang in languages" 
        :key="lang.code"
        @click="setLang(lang.code)"
        class="lang-btn"
        :class="{ 'active': locale === lang.code }"
      >
        {{ lang.label }}
      </button>
    </div>
  </div>

  <!-- NAVBAR -->
  <nav>
    <a class="logo" href="#">
      <div class="logo-icon">🌊</div>
      <div class="logo-text">
        <span class="brand">SeeDora Travel</span>
        <span class="tagline">Egypt · Luxury Experiences</span>
      </div>
    </a>
    <div class="nav-links">
      <a href="/#destinations">
        <span>{{ t('nav.destinations') }}</span>
      </a>
      <a href="/#trips">
        <span>{{ t('nav.tours') }}</span>
      </a>
      <a href="/#why">
        <span>{{ t('nav.aboutUs') }}</span>
      </a>
      <a href="/tours" class="nav-cta">
        <span>{{ t('nav.bookNow') }}</span>
      </a>
    </div>
  </nav>
</template>

<style scoped>
/* ─── LANGUAGE BAR ─── */
.lang-bar {
  background: var(--dark);
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 48px;
  font-size: 12px;
  letter-spacing: 0.08em;
}
.lang-bar .contact-info { color: #8eafc2; display: flex; gap: 24px; }
.lang-bar .contact-info span { display: flex; align-items: center; gap: 6px; }
.lang-bar .contact-info a { color: var(--sun-light); text-decoration: none; }
.lang-selector { display: flex; gap: 4px; }
.lang-btn {
  background: transparent; border: 1px solid #2a4a5e; color: #8eafc2;
  padding: 4px 10px; cursor: pointer; border-radius: 3px; font-size: 11px;
  font-family: 'Jost', sans-serif; letter-spacing: 0.06em; transition: all 0.2s;
}
.lang-btn:hover, .lang-btn.active {
  background: var(--sun); border-color: var(--sun); color: var(--white);
}

/* ─── NAVBAR ─── */
nav {
  position: sticky; top: 0; z-index: 1000;
  background: rgba(6, 58, 92, 0.97);
  backdrop-filter: blur(12px);
  display: flex; justify-content: space-between; align-items: center;
  padding: 0 48px;
  border-bottom: 1px solid rgba(201,168,76,0.3);
}
.logo {
  display: flex; align-items: center; gap: 14px; padding: 16px 0;
  text-decoration: none;
}
.logo-icon {
  width: 46px; height: 46px;
  background: linear-gradient(135deg, var(--sun), var(--sun-light));
  border-radius: 50%; display: flex; align-items: center; justify-content: center;
  font-size: 22px;
}
.logo-text { line-height: 1; }
.logo-text .brand { font-family: 'Playfair Display', serif; font-size: 22px; font-weight: 700; color: var(--white); letter-spacing: 0.05em; }
.logo-text .tagline { font-size: 10px; color: var(--gold); letter-spacing: 0.2em; text-transform: uppercase; }
.nav-links { display: flex; gap: 8px; align-items: center; }
.nav-links a {
  color: #a8c8dc; text-decoration: none; font-size: 13px;
  letter-spacing: 0.1em; text-transform: uppercase; padding: 10px 14px;
  border-radius: 4px; transition: all 0.2s;
}
.nav-links a:hover { color: var(--white); background: rgba(255,255,255,0.08); }
.nav-cta {
  background: linear-gradient(135deg, var(--sun), var(--sun-light)) !important;
  color: var(--white) !important; padding: 10px 22px !important;
  border-radius: 4px; font-weight: 500;
}
.nav-cta:hover { transform: translateY(-1px); box-shadow: 0 6px 20px rgba(232,130,10,0.4) !important; }

@media (max-width: 768px) {
  nav { padding: 0 20px; }
  .nav-links { display: none; }
  .lang-bar { padding: 8px 20px; flex-direction: column; gap: 8px; }
}
</style>
