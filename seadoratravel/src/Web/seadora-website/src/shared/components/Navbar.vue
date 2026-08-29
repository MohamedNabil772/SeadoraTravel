<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useCurrencyStore } from '@/store/currency'
import { useAuthStore } from '@/features/auth/store/auth'
import LuxuryIcons from '@/shared/components/LuxuryIcons.vue'
import CustomerProfileDropdown from '@/shared/components/CustomerProfileDropdown.vue'
const { locale, t } = useI18n()
const currencyStore = useCurrencyStore()
const authStore = useAuthStore()
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()

const showProfileDropdown = ref(false)
const showLangDropdown = ref(false)
const showCurrencyDropdown = ref(false)

const handleLogout = () => {
  authStore.logout()
  showProfileDropdown.value = false
  if (route.path.startsWith('/portal') || route.path.startsWith('/dashboard')) {
    router.push('/')
  }
}



import { loadLanguageAsync } from '@/i18n'

const setLang = async (lang: string) => {
  await loadLanguageAsync(lang)
  isMenuOpen.value = false // close menu on switch
  showLangDropdown.value = false
}

const selectCurrency = (curr: string) => {
  currencyStore.setCurrency(curr)
  isMenuOpen.value = false // close menu on switch
  showCurrencyDropdown.value = false
}

const languages = [
  { code: 'en', label: '🇬🇧 EN' },
  { code: 'de', label: '🇩🇪 DE' },
  { code: 'it', label: '🇮🇹 IT' },
  { code: 'fr', label: '🇫🇷 FR' },
  { code: 'ru', label: '🇷🇺 RU' }
]

const currencies = [
  { code: 'EUR', label: '€ EUR' },
  { code: 'USD', label: '$ USD' },
  { code: 'EGP', label: '🇪🇬 EGP' }
]

const isScrolled = ref(false)
const isMenuOpen = ref(false)

const handleScroll = () => {
  isScrolled.value = window.scrollY > 40
}

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value
}

onMounted(() => {
  window.addEventListener('scroll', handleScroll, { passive: true })
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<template>
  <header class="navbar-fixed-container" :class="{ 'header-scrolled': isScrolled }">
    <div class="lang-bar">
      <div class="contact-info">
      <span>
        <LuxuryIcons name="phone" size="12" color="currentColor" style="opacity: 0.85; flex-shrink: 0;" />
        <a href="tel:+201068940967" class="hover-underline">+20 106 894 0967</a>
      </span>
      <span>
        <LuxuryIcons name="mail" size="12" color="currentColor" style="opacity: 0.85; flex-shrink: 0;" />
        <a href="mailto:info@seadoratravel.com" class="hover-underline">info@seadoratravel.com</a>
      </span>
    </div>
    <div class="selectors-wrapper">
      <div class="lang-selector dropdown-container" v-click-outside="() => showLangDropdown = false">
        <button 
          @click="showLangDropdown = !showLangDropdown"
          class="dropdown-trigger-btn"
        >
          {{ languages.find(l => l.code === locale)?.label }}
          <LuxuryIcons name="chevron-down" size="12" class="ml-1" />
        </button>
        <Transition name="dropdown">
          <div v-if="showLangDropdown" class="dropdown-menu">
            <button 
              v-for="lang in languages" 
              :key="lang.code"
              @click="setLang(lang.code)"
              class="dropdown-item"
              :class="{ 'active': locale === lang.code }"
            >
              <span class="dropdown-label">{{ lang.label }}</span>
              <LuxuryIcons v-if="locale === lang.code" name="check" size="14" class="text-gold" />
            </button>
          </div>
        </Transition>
      </div>

      <div class="currency-selector dropdown-container" v-click-outside="() => showCurrencyDropdown = false">
        <button 
          @click="showCurrencyDropdown = !showCurrencyDropdown"
          class="dropdown-trigger-btn"
        >
          {{ currencies.find(c => c.code === currencyStore.selectedCurrency)?.label }}
          <LuxuryIcons name="chevron-down" size="12" class="ml-1" />
        </button>
        <Transition name="dropdown">
          <div v-if="showCurrencyDropdown" class="dropdown-menu">
            <button 
              v-for="curr in currencies" 
              :key="curr.code"
              @click="selectCurrency(curr.code)"
              class="dropdown-item"
              :class="{ 'active': currencyStore.selectedCurrency === curr.code }"
            >
              <span class="dropdown-label">{{ curr.label }}</span>
              <LuxuryIcons v-if="currencyStore.selectedCurrency === curr.code" name="check" size="14" class="text-gold" />
            </button>
          </div>
        </Transition>
      </div>
    </div>
  </div>

  <nav :class="{ 'scrolled': isScrolled, 'menu-open': isMenuOpen }" class="transition-all duration-500">
    <a class="logo group" href="/">
      <div class="logo-icon-wrapper">
        <img src="/logo-emblem.png" alt="Seadora Travel" class="brand-logo" />
      </div>
      <div class="logo-text">
        <span class="brand">SEADORA</span>
        <span class="tagline">TRAVEL · EGYPT</span>
      </div>
    </a>

    <!-- Desktop Navigation Links -->
    <div class="nav-links">
      <!-- Mega Menu Hover Expander -->
      <div class="explore-container">
        <a href="#" class="explore-btn premium-press" @click.prevent>
          <span>{{ t('nav.explore') || 'Explore' }}</span>
          <LuxuryIcons name="chevron-left" size="14" class="ml-1 transition-transform duration-300 transform -rotate-90" />
        </a>
        <!-- Mega Menu Card -->
        <div class="mega-menu">
          <div class="mega-menu-grid">
            <div class="mega-column">
              <h3>Destinations</h3>
              <a href="/tours?location=hurghada" class="mega-item">
                <span class="text-gold mr-2">✥</span> Hurghada
              </a>
              <a href="/tours?location=luxor" class="mega-item">
                <span class="text-gold mr-2">✥</span> Luxor
              </a>
              <a href="/tours?location=cairo" class="mega-item">
                <span class="text-gold mr-2">✥</span> Cairo
              </a>
              <a href="/tours?location=sharm" class="mega-item">
                <span class="text-gold mr-2">✥</span> Sharm El-Sheikh
              </a>
            </div>
            <div class="mega-column">
              <h3>Categories</h3>
              <a href="/tours?category=sea-diving" class="mega-item">
                <span class="text-gold mr-2">🤿</span> Sea & Diving
              </a>
              <a href="/tours?category=culture-history" class="mega-item">
                <span class="text-gold mr-2">🏛️</span> Culture & History
              </a>
              <a href="/tours?category=safari-adventure" class="mega-item">
                <span class="text-gold mr-2">🏜️</span> Safari & Adventure
              </a>
            </div>
          </div>
        </div>
      </div>

      <a href="/#destinations">
        <span>{{ t('nav.destinations') }}</span>
      </a>
      <a href="/#why">
        <span>{{ t('nav.aboutUs') }}</span>
      </a>

      <!-- Login / Profile Section -->
      <div class="auth-section">
        <button 
          v-if="!authStore.isAuthenticated" 
          @click="authStore.openAuthModal()" 
          class="btn-primary flex items-center gap-2" 
          style="padding: 9px 22px; font-size: 11px; letter-spacing: 0.08em;"
        >
          <span>{{ t('nav.login') || 'Sign In' }}</span>
        </button>
        <div v-else class="profile-container flex items-center gap-1.5">
          <CustomerProfileDropdown />
        </div>
      </div>
    </div>

    <!-- Mobile Hamburger Menu Button -->
    <button @click="toggleMenu" class="mobile-menu-btn premium-press" :aria-label="isMenuOpen ? 'Close menu' : 'Open menu'">
      <div class="hamburger-inner" :class="{ 'open': isMenuOpen }">
        <LuxuryIcons v-if="!isMenuOpen" name="menu" size="24" color="var(--white)" />
        <LuxuryIcons v-else name="close" size="24" color="var(--white)" />
      </div>
    </button>

    <!-- Mobile Navigation Dropdown (Elegant Dropdown, NOT full-height modal) -->
    <div class="mobile-drawer" :class="{ 'open': isMenuOpen }">
      <div class="mobile-drawer-links">
        <a href="/#destinations" @click="isMenuOpen = false">
          <span>{{ t('nav.destinations') }}</span>
        </a>
        <a href="/#why" @click="isMenuOpen = false">
          <span>{{ t('nav.aboutUs') }}</span>
        </a>

        <!-- Mobile Explore Options -->
        <div class="mobile-expand-section mt-4 mb-2">
          <span class="mobile-section-label">Browse Locations</span>
          <div class="mobile-grid-links">
            <a href="/tours?location=hurghada" @click="isMenuOpen = false">Hurghada</a>
            <a href="/tours?location=luxor" @click="isMenuOpen = false">Luxor</a>
            <a href="/tours?location=cairo" @click="isMenuOpen = false">Cairo</a>
            <a href="/tours?location=sharm" @click="isMenuOpen = false">Sharm El-Sheikh</a>
          </div>
        </div>
        <div class="mobile-expand-section border-t border-white/10 pt-3 mb-2">
          <span class="mobile-section-label">Browse Categories</span>
          <div class="mobile-grid-links">
            <a href="/tours?category=sea-diving" @click="isMenuOpen = false">Sea & Diving</a>
            <a href="/tours?category=culture-history" @click="isMenuOpen = false">Culture & History</a>
            <a href="/tours?category=safari-adventure" @click="isMenuOpen = false">Safari & Adventure</a>
          </div>
        </div>
      </div>

      <div class="mobile-drawer-selectors">
        <div class="selector-section">
          <span class="section-label">Language</span>
          <div class="mobile-flex-wrap">
            <button 
              v-for="lang in languages" 
              :key="lang.code"
              @click="setLang(lang.code)"
              class="mobile-selector-btn"
              :class="{ 'active': locale === lang.code }"
            >
              {{ lang.code.toUpperCase() }}
            </button>
          </div>
        </div>

        <div class="selector-section border-t border-white/10 pt-3 mt-1">
          <span class="section-label">Currency</span>
          <div class="mobile-flex-wrap">
            <button 
              v-for="curr in currencies" 
              :key="curr.code"
              @click="selectCurrency(curr.code)"
              class="mobile-selector-btn"
              :class="{ 'active': currencyStore.selectedCurrency === curr.code }"
            >
              {{ curr.code }}
            </button>
          </div>
        </div>

        <!-- Mobile User Auth section -->
        <div class="mobile-drawer-auth mt-3 border-t border-white/10 pt-3">
          <button v-if="!authStore.isAuthenticated" @click="authStore.openAuthModal(); isMenuOpen = false" class="mobile-login-btn">
            Sign In / Register
          </button>
          <div v-else class="mobile-logged-in flex flex-col gap-2">
            <span>Welcome, <strong>{{ authStore.user?.name || 'VIP Traveler' }}</strong></span>
            <router-link to="/portal" @click="isMenuOpen = false" class="mobile-portal-link px-3 py-1.5 bg-[#c9a84c] text-[#062d4d] rounded-lg text-xs font-bold text-center">
              Go to Customer Portal
            </router-link>
            <button @click="handleLogout" class="mobile-logout-btn text-xs text-white/70 hover:text-white">Logout</button>
          </div>
        </div>
      </div>
    </div>
  </nav>
</header>
</template>

<style scoped>
/* ─── FIXED NAVBAR WRAPPER (ALWAYS STICKY AT TOP) ─── */
.navbar-fixed-container {
  position: sticky;
  top: 0;
  z-index: 1000;
  width: 100%;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

/* ─── LANGUAGE & CURRENCY TOP BAR ─── */
.lang-bar {
  background: #031424;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 6px 48px;
  font-size: 11.5px;
  letter-spacing: 0.08em;
  border-bottom: 1px solid rgba(201, 168, 76, 0.2);
  position: relative;
  z-index: 1001;
  transition: all 0.3s ease;
}
.header-scrolled .lang-bar {
  padding: 4px 48px;
  background: rgba(3, 20, 36, 0.98);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-bottom: 1px solid rgba(201, 168, 76, 0.3);
}

.lang-bar .contact-info { color: #8eafc2; display: flex; gap: 24px; }
.lang-bar .contact-info span { display: flex; align-items: center; gap: 6px; }
.lang-bar .contact-info a { color: var(--sun-light); text-decoration: none; transition: color 0.2s; }
.lang-bar .contact-info a:hover { color: var(--gold); }
.selectors-wrapper { display: flex; gap: 16px; align-items: center; }
.lang-selector { display: flex; gap: 6px; }
.currency-selector { display: flex; gap: 6px; border-left: 1px solid rgba(255,255,255,0.15); padding-left: 16px; }
.dropdown-container { position: relative; }
.dropdown-trigger-btn {
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(201, 168, 76, 0.35);
  color: #ffffff;
  padding: 4px 12px;
  cursor: pointer;
  border-radius: 20px;
  font-size: 11px;
  font-family: var(--font-sans);
  font-weight: 600;
  letter-spacing: 0.06em;
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
  display: flex;
  align-items: center;
  gap: 4px;
}
.dropdown-trigger-btn:hover {
  background: rgba(201, 168, 76, 0.25);
  color: var(--white);
  border-color: var(--gold);
}
.dropdown-menu {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: 6px;
  background: rgba(6, 45, 77, 0.98);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(201, 168, 76, 0.35);
  border-radius: 8px;
  padding: 6px 0;
  min-width: 130px;
  box-shadow: 0 12px 36px rgba(0, 0, 0, 0.45);
  z-index: 1050;
  display: flex;
  flex-direction: column;
}
.dropdown-enter-active, .dropdown-leave-active { transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1); }
.dropdown-enter-from, .dropdown-leave-to { opacity: 0; transform: translateY(-6px); }
.dropdown-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 16px;
  background: transparent;
  border: none;
  color: rgba(255, 255, 255, 0.85);
  font-family: var(--font-sans);
  font-size: 11px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  width: 100%;
  text-align: left;
}
.dropdown-item:hover {
  background: rgba(201, 168, 76, 0.15);
  color: #ffffff;
}
.dropdown-item.active {
  color: var(--gold);
  font-weight: 700;
  background: rgba(201, 168, 76, 0.1);
}

/* ─── NAVBAR ─── */
nav {
  position: relative;
  background: rgba(6, 58, 92, 0.88);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  display: flex; justify-content: space-between; align-items: center;
  padding: 14px 48px;
  border-bottom: 1px solid rgba(201,168,76,0.2);
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}
nav.scrolled {
  background: rgba(6, 58, 92, 0.98);
  padding: 10px 48px;
  border-bottom: 1px solid rgba(201,168,76,0.35);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.2);
}
nav.menu-open {
  background: rgba(6, 58, 92, 0.98) !important;
  backdrop-filter: blur(20px) !important;
}

.logo {
  display: flex; align-items: center; gap: 12px;
  text-decoration: none;
  z-index: 60;
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}
.logo-icon-wrapper {
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  flex-shrink: 0;
  transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
.brand-logo {
  width: 100%;
  height: 100%;
  object-fit: contain;
  filter: drop-shadow(0 2px 8px rgba(0, 0, 0, 0.3)) drop-shadow(0 0 6px rgba(201, 168, 76, 0.25));
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  image-rendering: -webkit-optimize-contrast;
}
.logo:hover .logo-icon-wrapper {
  transform: scale(1.06);
}
.logo:hover .brand-logo {
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.4)) drop-shadow(0 0 14px rgba(201, 168, 76, 0.5));
}
.logo-text {
  line-height: 1.15;
  display: flex;
  flex-direction: column;
  justify-content: center;
  flex-shrink: 0;
}
.logo-text .brand {
  font-family: var(--font-serif-display, 'Cinzel', serif);
  font-size: 23px;
  font-weight: 800;
  color: var(--white, #ffffff);
  letter-spacing: 0.08em;
  transition: all 0.3s ease;
  white-space: nowrap;
}
.logo-text .tagline {
  font-size: 9.5px;
  font-weight: 700;
  color: var(--gold, #c9a84c);
  letter-spacing: 0.22em;
  text-transform: uppercase;
  display: block;
  margin-top: 2px;
  transition: all 0.3s ease;
  white-space: nowrap;
}

.nav-links { display: flex; gap: 8px; align-items: center; }
.nav-links a {
  color: #a8c8dc; text-decoration: none; font-size: 13px;
  letter-spacing: 0.12em; text-transform: uppercase; padding: 10px 14px;
  border-radius: 4px; transition: all 0.3s ease;
  position: relative;
}
.nav-links a:not(.nav-cta):hover { color: var(--white); }
.nav-links a:not(.nav-cta)::after {
  content: '';
  position: absolute;
  bottom: 2px;
  left: 14px;
  right: 14px;
  height: 2px;
  background-color: var(--gold);
  transform: scaleX(0);
  transform-origin: right;
  transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
.nav-links a:not(.nav-cta):hover::after {
  transform: scaleX(1);
  transform-origin: left;
}
.nav-cta {
  background: linear-gradient(135deg, var(--sun), var(--sun-light)) !important;
  color: var(--white) !important; padding: 10px 24px !important;
  border-radius: 4px !important; font-weight: 600 !important;
  box-shadow: 0 4px 15px rgba(232, 130, 10, 0.25);
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1) !important;
}
.nav-cta:hover { transform: translateY(-2px) !important; box-shadow: 0 8px 25px rgba(232,130,10,0.45) !important; }
.nav-cta:active { transform: translateY(0) scale(0.97) !important; }

/* ─── MOBILE HAMBURGER BUTTON ─── */
.mobile-menu-btn {
  display: none;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 8px;
  z-index: 1010;
}
.hamburger-inner {
  width: 20px;
  height: 12px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
}
.hamburger-inner span {
  display: block;
  width: 100%;
  height: 2px;
  background-color: var(--white);
  border-radius: 2px;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
.hamburger-inner.open span:nth-child(1) {
  transform: translateY(5px) rotate(45deg);
}
.hamburger-inner.open span:nth-child(2) {
  opacity: 0;
  transform: scaleX(0);
}
.hamburger-inner.open span:nth-child(3) {
  transform: translateY(-5px) rotate(-45deg);
}

/* ─── MOBILE NAVIGATION DROPDOWN (Snug, elegant slide-down dropdown) ─── */
.mobile-drawer {
  position: absolute;
  top: 100%; /* Positions directly beneath the header block */
  left: 0;
  right: 0;
  background: rgba(6, 58, 92, 0.98);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  z-index: 990;
  transform: translateY(-10px);
  opacity: 0;
  pointer-events: none;
  transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1),
              opacity 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  padding: 24px 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  border-bottom: 1.5px solid rgba(201, 168, 76, 0.35);
  box-shadow: 0 16px 32px rgba(6, 58, 92, 0.3);
}
.mobile-drawer.open {
  transform: translateY(0);
  opacity: 1;
  pointer-events: auto;
}

.mobile-drawer-links {
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.mobile-drawer-links a {
  color: rgba(255, 255, 255, 0.85);
  font-family: var(--font-sans);
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.12em;
  text-decoration: none;
  padding: 10px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  transition: color 0.25s, padding-left 0.25s;
}
.mobile-drawer-links a:hover {
  color: var(--gold);
  padding-left: 4px;
}
.mobile-cta {
  background: linear-gradient(135deg, var(--sun), var(--sun-light));
  color: var(--white) !important;
  text-align: center;
  padding: 10px !important;
  border-radius: 4px;
  font-family: var(--font-sans) !important;
  font-size: 12px !important;
  font-weight: 600 !important;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  margin-top: 10px;
  border-bottom: none !important;
  box-shadow: 0 4px 12px rgba(232, 130, 10, 0.2);
}

.mobile-drawer-selectors {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding-top: 4px;
}
.selector-section {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.section-label {
  font-family: var(--font-sans);
  font-size: 9px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.12em;
  color: var(--gold);
}
.mobile-flex-wrap {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}
.mobile-selector-btn {
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.06);
  color: rgba(255, 255, 255, 0.65);
  padding: 4px 10px;
  cursor: pointer;
  border-radius: 2px;
  font-size: 10px;
  font-family: var(--font-sans);
  text-align: center;
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
}
.mobile-selector-btn:hover {
  background: rgba(255, 255, 255, 0.08);
  color: var(--white);
}
.mobile-selector-btn.active {
  background: var(--gold);
  border-color: var(--gold);
  color: var(--dark);
  font-weight: 600;
}

@media (max-width: 768px) {
  .lang-bar { display: none; }
  nav { padding: 10px 16px; }
  nav.scrolled { padding: 10px 16px; }
  .logo-image-container { width: 38px; height: 38px; }
  .logo-text .brand { font-size: 16px; }
  .logo-text .tagline { font-size: 8px; margin-top: 0; }
  .nav-links { display: none; }
  .mobile-menu-btn { display: block; }
  .mobile-drawer {
    /* Height matches header height on mobile (10px padding + 32px icon + 10px padding) */
    top: 52px;
  }
}

/* ─── EXPLORE DROPDOWN / MEGA MENU ─── */
.explore-container {
  position: relative;
  display: inline-block;
}
.explore-btn {
  display: flex;
  align-items: center;
  color: var(--white);
  text-decoration: none;
  font-family: var(--font-sans);
  font-size: 13px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  padding: 10px 14px;
}
.explore-btn svg {
  transition: transform 0.3s ease;
}
.explore-container:hover .explore-btn svg {
  transform: rotate(180deg);
  color: var(--gold);
}
.mega-menu {
  position: absolute;
  top: 100%;
  left: 50%;
  transform: translateX(-50%) translateY(10px);
  width: 460px;
  background: rgba(6, 58, 92, 0.98);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(201, 168, 76, 0.3);
  border-radius: 6px;
  padding: 24px;
  box-shadow: 0 20px 48px rgba(6, 58, 92, 0.25);
  opacity: 0;
  pointer-events: none;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  z-index: 1000;
}
.explore-container:hover .mega-menu {
  opacity: 1;
  pointer-events: auto;
  transform: translateX(-50%) translateY(0);
}
.mega-menu-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
}
.mega-column h3 {
  font-family: var(--font-sans);
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  color: var(--gold);
  letter-spacing: 0.15em;
  margin-top: 0;
  margin-bottom: 14px;
  border-bottom: 1px solid rgba(201, 168, 76, 0.15);
  padding-bottom: 6px;
}
.mega-item {
  display: flex;
  align-items: center;
  color: rgba(255, 255, 255, 0.8) !important;
  font-family: var(--font-sans);
  font-size: 13px;
  text-decoration: none !important;
  padding: 6px 0;
  transition: all 0.2s ease;
  border-bottom: none !important;
}
.mega-item::after {
  display: none !important; /* disable default line effect */
}
.mega-item:hover {
  color: var(--sun-light) !important;
  transform: translateX(4px);
}

/* ─── AUTH / PROFILE SECTION ─── */
.auth-section {
  display: flex;
  align-items: center;
  margin-left: 8px;
}
.nav-login-btn {
  background: transparent;
  border: 1px solid var(--gold);
  color: var(--gold);
  padding: 8px 20px;
  border-radius: 4px;
  font-family: var(--font-sans);
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  cursor: pointer;
  transition: all 0.25s ease;
}
.nav-login-btn:hover {
  background: var(--gold);
  color: var(--dark);
  box-shadow: 0 4px 14px rgba(201, 168, 76, 0.25);
}
.profile-container {
  position: relative;
}
.profile-btn {
  display: flex;
  align-items: center;
  gap: 10px;
  background: transparent;
  border: none;
  cursor: pointer;
  color: var(--white);
  font-family: var(--font-sans);
  font-size: 13px;
  font-weight: 500;
  padding: 4px 8px;
}
.profile-btn .avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--gold);
  color: var(--dark);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 12px;
  box-shadow: 0 0 10px rgba(201, 168, 76, 0.2);
}
.profile-btn .username {
  max-width: 100px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.profile-dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  width: 220px;
  background: rgba(6, 58, 92, 0.98);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(201, 168, 76, 0.25);
  border-radius: 4px;
  padding: 16px;
  box-shadow: 0 10px 30px rgba(6, 58, 92, 0.35);
  z-index: 1000;
  margin-top: 10px;
  animation: slideDown 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
@keyframes slideDown {
  from { transform: translateY(10px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}
.profile-header {
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  padding-bottom: 10px;
  margin-bottom: 10px;
}
.profile-header h4 {
  font-family: var(--font-serif-display);
  font-size: 15px;
  color: var(--white);
  margin: 0 0 4px 0;
}
.profile-header p {
  font-family: var(--font-sans);
  font-size: 11px;
  color: rgba(255, 255, 255, 0.5);
  margin: 0;
  word-break: break-all;
}
.profile-links {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.profile-link {
  color: rgba(255, 255, 255, 0.8) !important;
  font-family: var(--font-sans);
  font-size: 12px;
  text-decoration: none !important;
  display: block;
  padding: 6px 0;
  transition: color 0.2s;
  border-bottom: none !important;
}
.profile-link::after {
  display: none !important;
}
.profile-link:hover {
  color: var(--gold) !important;
}
.logout-btn {
  background: rgba(232, 130, 10, 0.1);
  border: 1px solid rgba(232, 130, 10, 0.3);
  color: var(--sun-light);
  width: 100%;
  padding: 8px;
  border-radius: 4px;
  cursor: pointer;
  font-family: var(--font-sans);
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  transition: all 0.25s ease;
  margin-top: 4px;
}
.logout-btn:hover {
  background: var(--sun);
  color: var(--white);
  border-color: var(--sun);
}

/* ─── LOGIN MODAL OVERLAY ─── */
.login-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(13, 31, 45, 0.8);
  backdrop-filter: blur(8px);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}
.login-modal-card {
  background: var(--cream);
  border: 1.5px solid var(--gold);
  border-radius: 8px;
  max-width: 400px;
  width: 100%;
  padding: 36px 30px;
  position: relative;
  box-shadow: 0 24px 50px rgba(6, 58, 92, 0.35);
  animation: modalScaleUp 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
}
@keyframes modalScaleUp {
  from { transform: scale(0.9); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}
.modal-close-btn {
  position: absolute;
  top: 16px;
  right: 16px;
  background: transparent;
  border: none;
  font-size: 24px;
  color: var(--sea-deep);
  cursor: pointer;
  opacity: 0.6;
  transition: opacity 0.2s;
}
.modal-close-btn:hover {
  opacity: 1;
}
.modal-title {
  font-family: var(--font-serif-display);
  font-size: 24px;
  color: var(--sea-deep);
  margin-top: 0;
  margin-bottom: 8px;
  text-align: center;
}
.modal-subtitle {
  font-family: var(--font-sans);
  font-size: 13px;
  color: var(--text);
  opacity: 0.8;
  margin-top: 0;
  margin-bottom: 24px;
  text-align: center;
  line-height: 1.5;
}
.login-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.form-group label {
  font-family: var(--font-sans);
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  color: var(--sea-deep);
  letter-spacing: 0.05em;
}
.form-group input {
  background: var(--white);
  border: 1px solid rgba(6, 58, 92, 0.15);
  padding: 10px 14px;
  border-radius: 4px;
  font-family: var(--font-sans);
  font-size: 14px;
  outline: none;
  color: var(--text);
  transition: border-color 0.2s;
}
.form-group input:focus {
  border-color: var(--gold);
}
.login-submit-btn {
  background: linear-gradient(135deg, var(--sun), var(--sun-light));
  color: var(--white);
  border: none;
  padding: 12px;
  border-radius: 4px;
  font-family: var(--font-sans);
  font-size: 13px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  box-shadow: 0 4px 14px rgba(232, 130, 10, 0.2);
  transition: all 0.2s ease;
  margin-top: 8px;
}
.login-submit-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 20px rgba(232, 130, 10, 0.35);
}
.login-submit-btn:active {
  transform: translateY(0) scale(0.98);
}

/* ─── MOBILE DRAWER EXPANSIONS ─── */
.mobile-expand-section {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.mobile-section-label {
  font-family: var(--font-sans);
  font-size: 9px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.12em;
  color: var(--gold);
}
.mobile-grid-links {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
}
.mobile-grid-links a {
  color: rgba(255, 255, 255, 0.7);
  font-family: var(--font-sans);
  font-size: 12px;
  text-decoration: none;
  padding: 4px 0;
  transition: color 0.2s;
}
.mobile-grid-links a:hover {
  color: var(--gold);
}
.mobile-drawer-auth {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.mobile-login-btn {
  background: transparent;
  border: 1px solid var(--gold);
  color: var(--gold);
  padding: 8px;
  border-radius: 4px;
  font-family: var(--font-sans);
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  width: 100%;
  cursor: pointer;
}
.mobile-logged-in {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-family: var(--font-sans);
  font-size: 12px;
  color: var(--white);
}
.mobile-logout-btn {
  background: transparent;
  border: none;
  color: var(--sun-light);
  font-weight: 600;
  text-transform: uppercase;
  cursor: pointer;
  font-size: 11px;
}

@media (max-width: 768px) {
  .navbar-fixed-container {
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.2);
  }
  .lang-bar {
    padding: 6px 16px;
  }
  .lang-bar .contact-info {
    display: none;
  }
  .selectors-wrapper {
    width: 100%;
    justify-content: flex-end;
    gap: 10px;
  }
  .currency-selector {
    padding-left: 10px;
  }
  nav {
    padding: 12px 16px;
  }
}

</style>
