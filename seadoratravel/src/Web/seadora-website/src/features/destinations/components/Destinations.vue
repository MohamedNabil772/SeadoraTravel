<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const router = useRouter()

const destinations = [
  {
    id: 'hurghada',
    guid: '00000000-0000-0000-0000-000000000011',
    name: 'Hurghada',
    bgClass: 'bg-hurghada',
    categoryTag: '🤿 Red Sea Capital',
    highlights: ['VIP Cruises', 'Diving', 'Islands']
  },
  {
    id: 'cairo',
    guid: '00000000-0000-0000-0000-000000000013',
    name: 'Cairo',
    bgClass: 'bg-cairo',
    categoryTag: '🏺 Ancient Wonders & GEM',
    highlights: ['Pyramids', 'Grand Museum', 'Nile']
  },
  {
    id: 'luxor',
    guid: '00000000-0000-0000-0000-000000000012',
    name: 'Luxor',
    bgClass: 'bg-luxor',
    categoryTag: '🏛️ Pharaonic Valley',
    highlights: ['Valley of Kings', 'Karnak', 'Balloon']
  },
  {
    id: 'sharm',
    guid: '00000000-0000-0000-0000-000000000014',
    name: 'Sharm El-Sheikh',
    bgClass: 'bg-sharm',
    categoryTag: '🏖️ Coral Coast',
    highlights: ['Ras Mohammed', 'Luxury Resorts']
  }
]

const toursCount = ref<Record<string, number>>({})

const selectDestination = (destId: string) => {
  router.push({ path: '/tours', query: { location: destId } })
}

onMounted(async () => {
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000';
    const res = await fetch(`${API_URL}/api/content/api/tours`)
    if (res.ok) {
      const tours = await res.json()
      const counts: Record<string, number> = {}
      tours.forEach((t: any) => {
        if (t.destinationId) {
          counts[t.destinationId] = (counts[t.destinationId] || 0) + 1
        }
      })
      toursCount.value = counts
    }
  } catch (err) {
    console.error('Failed to fetch tours counts in Destinations:', err)
  }
})
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

    <div class="dest-bento-grid stagger-container" v-reveal="'stagger-container'">
      <div 
        v-for="(dest, index) in destinations" 
        :key="dest.id"
        class="bento-card"
        :class="`bento-card-${index + 1}`"
        @click="selectDestination(dest.id)"
      >
        <div class="dest-bg" :class="dest.bgClass"></div>
        <div class="dest-overlay"></div>
        
        <div class="dest-content">
          <div class="dest-header">
            <span class="category-tag">{{ dest.categoryTag }}</span>
            <div class="tour-count-badge">
              <span class="pulse-dot"></span>
              {{ toursCount[dest.guid] || 0 }} Tours
            </div>
          </div>
          
          <div class="dest-footer">
            <h3 class="dest-name">{{ dest.name }}</h3>
            
            <div class="dest-hover-content">
              <div class="highlights">
                <span v-for="hl in dest.highlights" :key="hl" class="highlight-pill">{{ hl }}</span>
              </div>
              <button class="explore-btn">
                Explore Experiences
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

/* ─── BACKGROUND IMAGES ─── */
.bg-hurghada { background-image: url('/images/hurghada.jpg') !important; }
.bg-cairo { background-image: url('/images/cairo.jpg') !important; }
.bg-luxor { background-image: url('/images/luxor.jpg') !important; }
.bg-sharm { background-image: url('/images/sharm.jpg') !important; }

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

