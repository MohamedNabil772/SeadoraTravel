<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import type { Destination } from '../../../core/models/Destination'

const { t, locale } = useI18n()
const router = useRouter()

// Extended destination for fallback logic
interface LocalDestination extends Destination {
  guid?: string
  name?: any
  description?: any
  categoryTag?: any
  bgClass?: string
  imagePath?: string
}

const destinations = ref<LocalDestination[]>([])
const loading = ref(true)
const toursCount = ref<Record<string, number>>({})

const selectDestination = (dest: LocalDestination) => {
  const name = dest.names?.['en'] || dest.name?.['en'] || (typeof dest.name === 'string' ? dest.name : '') || ''
  const slug = name.trim().length > 0 
    ? name.toLowerCase().trim().replace(/\s+/g, '-') 
    : (dest.id || dest.guid || '')
  router.push({ path: '/tours', query: { destination: slug } })
}

const getLocalized = (field: any) => {
  if (!field) return ''
  if (typeof field === 'string') return field
  return field[locale.value] || field['en'] || ''
}

const getLocalizedArray = (field: any) => {
  if (!field) return []
  if (Array.isArray(field)) return field
  
  const localizedString = field[locale.value] || field['en'] || ''
  if (typeof localizedString === 'string') {
    return localizedString.split(',').map((s: string) => s.trim()).filter((s: string) => s.length > 0)
  }
  if (Array.isArray(localizedString)) return localizedString
  
  return []
}

onMounted(async () => {
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000';
    
    // Fetch Destinations
    const destRes = await fetch(`${API_URL}/api/content/api/destinations`).catch(() => null);
    if (destRes && destRes.ok) {
      destinations.value = await destRes.json()
    } else {
      // Fallback if API is missing or fails
      destinations.value = [
        {
          id: 'hurghada',
          guid: '00000000-0000-0000-0000-000000000011',
          names: { en: 'Hurghada', ru: 'Хургада', de: 'Hurghada', it: 'Hurghada', cs: 'Hurghada' },
          categoryTag: { en: '🤿 Red Sea Capital' },
          highlights: { en: 'VIP Cruises, Diving, Islands' },
          bgClass: 'bg-hurghada',
          flagEmoji: '🇪🇬'
        },
        {
          id: 'cairo',
          guid: '00000000-0000-0000-0000-000000000013',
          names: { en: 'Cairo', ru: 'Каир' },
          categoryTag: { en: '🏺 Ancient Wonders' },
          highlights: { en: 'Pyramids, Museum' },
          bgClass: 'bg-cairo',
          flagEmoji: '🇪🇬'
        },
        {
          id: 'luxor',
          guid: '00000000-0000-0000-0000-000000000012',
          names: { en: 'Luxor', ru: 'Луксор' },
          categoryTag: { en: '🏛️ Pharaonic Valley' },
          highlights: { en: 'Valley of Kings, Karnak' },
          bgClass: 'bg-luxor',
          flagEmoji: '🇪🇬'
        },
        {
          id: 'sharm',
          guid: '00000000-0000-0000-0000-000000000014',
          names: { en: 'Sharm El-Sheikh', ru: 'Шарм-эль-Шейх' },
          categoryTag: { en: '🏖️ Coral Coast' },
          highlights: { en: 'Ras Mohammed, Resorts' },
          bgClass: 'bg-sharm',
          flagEmoji: '🇪🇬'
        }
      ]
    }

    // Fetch Tours to count
    const tourRes = await fetch(`${API_URL}/api/content/api/tours`).catch(() => null);
    if (tourRes && tourRes.ok) {
      const tours = await tourRes.json()
      const counts: Record<string, number> = {}
      tours.forEach((t: any) => {
        if (t.destinationId) {
          counts[t.destinationId] = (counts[t.destinationId] || 0) + 1
        }
      })
      toursCount.value = counts
    }
  } catch (err) {
    console.error('Failed to fetch destinations/tours:', err)
  } finally {
    loading.value = false
  }
})

const getTourCount = (dest: LocalDestination) => {
  if (dest.tourCount !== undefined) return dest.tourCount
  return toursCount.value[dest.guid || dest.id] || 0
}

const getBgStyle = (dest: LocalDestination) => {
  const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000';
  let url = dest.imageUrl || dest.imagePath
  
  if (!url) {
    const fallbackMap: Record<string, string> = {
      hurghada: '/images/hurghada.jpg',
      cairo: '/images/cairo.jpg',
      luxor: '/images/luxor.jpg',
      sharm: '/images/sharm.jpg',
    }
    const nameStr = dest.names?.en?.toLowerCase() || dest.name?.en?.toLowerCase() || ''
    url = fallbackMap[dest.id] || fallbackMap[nameStr] || ''
  } else if (!url.startsWith('http') && !url.startsWith('/')) {
    url = `${API_URL}/${url}`
  } else if (url.startsWith('/api/files')) {
    url = `${API_URL}${url}`
  }
  
  if (url) {
    return { backgroundImage: `url(${url})` }
  }
  return {}
}
</script>

<template>
  <section class="section" id="destinations">
    <div class="section-header" v-reveal="'reveal-fade-up'">
      <div class="section-eyebrow">
        {{ t('destinations.eyebrow') }}
      </div>
      <h2 class="section-title">
        <span v-html="t('destinations.title')"></span>
      </h2>
      <p class="section-sub">
        {{ t('destinations.description') }}
      </p>
    </div>

    <!-- Loading Skeleton -->
    <div v-if="loading" class="dest-bento-grid">
      <div 
        v-for="i in 4" 
        :key="`skeleton-${i}`"
        class="bento-card skeleton-card"
        :class="`bento-card-${i}`"
      >
        <div class="skeleton-bg"></div>
        <div class="dest-content">
          <div class="dest-header">
            <div class="skeleton-tag"></div>
            <div class="skeleton-badge"></div>
          </div>
          <div class="dest-footer">
            <div class="skeleton-title"></div>
            <div class="skeleton-desc"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loaded Data -->
    <div v-else class="dest-bento-grid stagger-container" v-reveal="'stagger-container'">
      <div 
        v-for="(dest, index) in destinations" 
        :key="dest.id || index"
        class="bento-card"
        :class="`bento-card-${(index % 4) + 1}`"
        tabindex="0"
        @click="selectDestination(dest)"
        @keydown.enter="selectDestination(dest)"
        @keydown.space.prevent="selectDestination(dest)"
      >
        <div class="dest-bg" :class="dest.bgClass" :style="getBgStyle(dest)"></div>
        <div class="dest-overlay"></div>
        
        <div class="dest-content">
          <div class="dest-header">
            <span class="category-tag">
              <span v-if="dest.flagEmoji || dest.flag" class="flag-emoji">{{ dest.flagEmoji || dest.flag }}</span>
              {{ getLocalized(dest.categoryTag) || 'Destination' }}
            </span>
            <div class="tour-count-badge">
              <span class="pulse-dot"></span>
              {{ getTourCount(dest) }} {{ t('destinations.tours', 'Tours') }}
            </div>
          </div>
          
          <div class="dest-footer">
            <h3 class="dest-name">{{ getLocalized(dest.names || dest.name) }}</h3>
            <p v-if="dest.descriptions || dest.description" class="dest-desc">{{ getLocalized(dest.descriptions || dest.description) }}</p>
            
            <div class="dest-hover-content">
              <div class="highlights">
                <span v-for="hl in getLocalizedArray(dest.highlights)" :key="hl" class="highlight-pill">{{ hl }}</span>
              </div>
              <button class="explore-btn">
                {{ t('destinations.explore', 'Explore Experiences') }}
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14M12 5l7 7-7 7"/></svg>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* ─── DESTINATIONS SECTION ─── */
.section { padding: 100px 48px; background: var(--cream); }
.section-header { text-align: center; margin-bottom: 60px; }
.section-eyebrow {
  font-family: var(--font-sans);
  font-size: 13px; letter-spacing: 0.15em; text-transform: uppercase;
  color: var(--grass); font-weight: 600; margin-bottom: 14px;
  display: flex; align-items: center; justify-content: center; gap: 12px;
}
.section-eyebrow::before, .section-eyebrow::after {
  content: ''; width: 40px; height: 1px; background: var(--grass); opacity: 0.5;
}
.section-sub {
  font-family: var(--font-serif-accent);
  font-size: 19px; color: var(--muted); max-width: 600px; margin: 0 auto; line-height: 1.7;
  font-style: italic;
}

/* ─── EDITORIAL BENTO GRID ─── */
.dest-bento-grid {
  display: grid;
  grid-template-columns: repeat(12, 1fr);
  gap: 24px;
  max-width: 1280px;
  margin: 0 auto;
}

.bento-card {
  position: relative;
  border-radius: 12px;
  overflow: hidden;
  cursor: pointer;
  height: 460px;
  border: 1px solid rgba(201, 168, 76, 0.1);
  box-shadow: 0 10px 30px rgba(6, 45, 77, 0.05);
  transition: all 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.bento-card:hover {
  transform: translateY(-8px);
  border-color: rgba(201, 168, 76, 0.4);
  box-shadow: 0 20px 40px rgba(201, 168, 76, 0.15), 0 0 30px rgba(201, 168, 76, 0.1) inset;
}

.bento-card-1 { grid-column: span 7; }
.bento-card-2 { grid-column: span 5; }
.bento-card-3 { grid-column: span 5; }
.bento-card-4 { grid-column: span 7; }

/* ─── CARD BACKGROUND & OVERLAY ─── */
.dest-bg {
  position: absolute;
  inset: -2px;
  background-size: cover;
  background-position: center;
  transition: transform 0.7s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.bento-card:hover .dest-bg {
  transform: scale(1.06);
}

.dest-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(6, 45, 77, 0.95) 0%, rgba(6, 45, 77, 0.2) 60%, transparent 100%);
  transition: background 0.5s ease;
  z-index: 1;
}

/* ─── CARD CONTENT ─── */
.dest-content {
  position: absolute;
  inset: 0;
  z-index: 2;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 32px;
}

.dest-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  flex-wrap: wrap;
  gap: 12px;
}

.category-tag {
  background: rgba(0, 0, 0, 0.35);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  border: 1px solid rgba(255, 255, 255, 0.15);
  padding: 6px 14px;
  border-radius: 20px;
  font-family: var(--font-sans);
  font-size: 13px;
  font-weight: 500;
  color: white;
  letter-spacing: 0.02em;
  display: flex;
  align-items: center;
  gap: 6px;
}
.flag-emoji {
  font-size: 1.1em;
}

.tour-count-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  padding: 6px 12px;
  border-radius: 20px;
  font-family: var(--font-sans);
  font-size: 12px;
  font-weight: 600;
  color: white;
  letter-spacing: 0.02em;
}

.pulse-dot {
  width: 6px;
  height: 6px;
  background-color: #c9a84c;
  border-radius: 50%;
  box-shadow: 0 0 0 rgba(201, 168, 76, 0.4);
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% { box-shadow: 0 0 0 0 rgba(201, 168, 76, 0.7); }
  70% { box-shadow: 0 0 0 6px rgba(201, 168, 76, 0); }
  100% { box-shadow: 0 0 0 0 rgba(201, 168, 76, 0); }
}

.dest-footer {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.dest-name {
  font-family: var(--font-serif-display);
  font-size: 38px;
  font-weight: 700;
  color: white;
  margin: 0;
  text-shadow: 0 2px 10px rgba(6, 45, 77, 0.4);
  transition: color 0.4s ease;
}

.dest-desc {
  font-family: var(--font-sans);
  font-size: 14px;
  color: rgba(255, 255, 255, 0.9);
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.bento-card:hover .dest-name {
  color: #c9a84c;
}

.dest-hover-content {
  max-height: 0;
  opacity: 0;
  overflow: hidden;
  transition: all 0.5s cubic-bezier(0.16, 1, 0.3, 1);
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.bento-card:hover .dest-hover-content {
  max-height: 200px;
  opacity: 1;
  margin-top: 4px;
}

.highlights {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.highlight-pill {
  font-family: var(--font-sans);
  font-size: 12px;
  font-weight: 500;
  padding: 4px 10px;
  border-radius: 4px;
  background: rgba(201, 168, 76, 0.15);
  border: 1px solid rgba(201, 168, 76, 0.3);
  color: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(4px);
  -webkit-backdrop-filter: blur(4px);
}

.explore-btn {
  align-self: flex-start;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: transparent;
  color: white;
  border: none;
  font-family: var(--font-sans);
  font-size: 14px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  padding: 0 0 4px 0;
  position: relative;
}

.explore-btn::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0%;
  height: 1px;
  background: #c9a84c;
  transition: width 0.3s ease;
}

.bento-card:hover .explore-btn::after {
  width: 100%;
}

.explore-btn svg {
  transition: transform 0.3s ease, color 0.3s ease;
}

.explore-btn:hover svg {
  transform: translateX(4px);
  color: #c9a84c;
}

/* ─── LOADING SKELETON ─── */
.skeleton-card {
  background: rgba(6, 45, 77, 0.05);
  border: 1px solid rgba(6, 45, 77, 0.1);
  pointer-events: none;
}
.skeleton-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(90deg, rgba(201,168,76,0.05) 25%, rgba(201,168,76,0.1) 50%, rgba(201,168,76,0.05) 75%);
  background-size: 400% 100%;
  animation: shimmer 1.5s infinite;
}
@keyframes shimmer {
  0% { background-position: 100% 0; }
  100% { background-position: -100% 0; }
}
.skeleton-tag {
  width: 120px;
  height: 28px;
  background: rgba(0,0,0,0.1);
  border-radius: 20px;
}
.skeleton-badge {
  width: 80px;
  height: 28px;
  background: rgba(0,0,0,0.1);
  border-radius: 20px;
}
.skeleton-title {
  width: 60%;
  height: 40px;
  background: rgba(0,0,0,0.1);
  border-radius: 4px;
  margin-bottom: 8px;
}
.skeleton-desc {
  width: 80%;
  height: 16px;
  background: rgba(0,0,0,0.1);
  border-radius: 4px;
}

/* ─── BACKGROUND IMAGES ─── */
.bg-hurghada { background-image: url('/images/hurghada.jpg'); }
.bg-cairo { background-image: url('/images/cairo.jpg'); }
.bg-luxor { background-image: url('/images/luxor.jpg'); }
.bg-sharm { background-image: url('/images/sharm.jpg'); }

/* ─── RESPONSIVE ─── */
@media (max-width: 1024px) {
  .section { padding: 80px 32px; }
  .bento-card-1 { grid-column: span 12; }
  .bento-card-2 { grid-column: span 6; }
  .bento-card-3 { grid-column: span 6; }
  .bento-card-4 { grid-column: span 12; }
}

@media (max-width: 768px) {
  .section { padding: 60px 20px; }
  .dest-bento-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  .bento-card {
    height: 400px;
    grid-column: span 1 !important;
  }
  .dest-content {
    padding: 24px;
  }
  .dest-hover-content {
    max-height: 200px;
    opacity: 1;
    margin-top: 8px;
  }
  .explore-btn::after {
    width: 100%;
    background: rgba(255, 255, 255, 0.3);
  }
}
</style>
