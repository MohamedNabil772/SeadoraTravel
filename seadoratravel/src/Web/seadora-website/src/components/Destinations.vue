<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const destinations = [
  {
    id: 'hurghada',
    name: 'Hurghada',
    emoji: '🐠🌊🤿',
    flag: '🌊',
    bgClass: 'bg-hurghada',
    tagKey: 'destinations.hurghada.tag'
  },
  {
    id: 'cairo',
    name: 'Cairo',
    emoji: '🏛️🐪',
    flag: '🏛️',
    bgClass: 'bg-cairo',
    tagKey: 'destinations.cairo.tag'
  },
  {
    id: 'luxor',
    name: 'Luxor',
    emoji: '🏺⚱️',
    flag: '🏺',
    bgClass: 'bg-luxor',
    tagKey: 'destinations.luxor.tag'
  },
  {
    id: 'sharm',
    name: 'Sharm El-Sheikh',
    emoji: '🐢🌴',
    flag: '🌴',
    bgClass: 'bg-sharm',
    tagKey: 'destinations.sharm.tag'
  }
]
</script>

<template>
  <section class="section" id="destinations">
    <div class="section-header">
      <div class="section-eyebrow">
        {{ t('destinations.eyebrow') }}
      </div>
      <h2>
        <span v-html="t('destinations.title')"></span>
      </h2>
      <p class="section-sub">
        {{ t('destinations.description') }}
      </p>
    </div>
    <div class="dest-grid">
      <div 
        v-for="dest in destinations" 
        :key="dest.id"
        class="dest-card"
      >
        <div class="dest-bg" :class="dest.bgClass"></div>
        <div class="dest-emoji">
          {{ dest.emoji }}
        </div>
        <div class="dest-overlay"></div>
        <div class="dest-info">
          <div class="dest-flag">{{ dest.flag }}</div>
          <div class="dest-name">
            {{ dest.name }}
          </div>
          <div class="dest-desc">
            {{ t(`destinations.${dest.id}.description`) }}
          </div>
          <span class="dest-tag">
            {{ t(dest.tagKey) }}
          </span>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* ─── DESTINATIONS SECTION ─── */
.section { padding: 100px 48px; }
.section-header { text-align: center; margin-bottom: 70px; }
.section-eyebrow {
  font-size: 11px; letter-spacing: 0.25em; text-transform: uppercase;
  color: var(--grass); font-weight: 600; margin-bottom: 14px;
  display: flex; align-items: center; justify-content: center; gap: 12px;
}
.section-eyebrow::before, .section-eyebrow::after {
  content: ''; width: 40px; height: 1px; background: var(--grass);
}
.section h2 {
  font-family: 'Playfair Display', serif;
  font-size: clamp(32px, 4vw, 52px); font-weight: 700;
  color: var(--dark); line-height: 1.15; margin-bottom: 18px;
}
.section h2 :deep(span) { color: var(--sea); }
.section-sub {
  font-family: 'Cormorant Garamond', serif;
  font-size: 19px; color: var(--muted); max-width: 600px; margin: 0 auto; line-height: 1.7;
}

/* ─── DESTINATIONS GRID ─── */
.dest-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 24px; }
.dest-card {
  border-radius: 12px; overflow: hidden; position: relative;
  height: 420px; cursor: pointer;
  transition: transform 0.4s ease, box-shadow 0.4s ease;
}
.dest-card:hover { transform: translateY(-6px); box-shadow: 0 30px 60px rgba(0,0,0,0.25); }
.dest-card:first-child { grid-column: span 2; height: 480px; }
.dest-bg {
  position: absolute; inset: 0;
  background-size: cover; background-position: center;
  transition: transform 0.6s ease;
}
.dest-card:hover .dest-bg { transform: scale(1.05); }
.dest-overlay {
  position: absolute; inset: 0;
  background: linear-gradient(to top, rgba(6,28,40,0.9) 0%, rgba(6,28,40,0.3) 50%, transparent 100%);
}
.dest-info { position: absolute; bottom: 0; left: 0; right: 0; padding: 28px; }
.dest-flag { font-size: 20px; margin-bottom: 8px; }
.dest-name {
  font-family: 'Playfair Display', serif;
  font-size: 26px; font-weight: 700; color: var(--white); margin-bottom: 6px;
}
.dest-card:first-child .dest-name { font-size: 34px; }
.dest-desc { font-size: 13px; color: rgba(255,255,255,0.7); line-height: 1.5; margin-bottom: 14px; }
.dest-tag {
  display: inline-block;
  background: rgba(232,130,10,0.85); color: var(--white);
  font-size: 11px; letter-spacing: 0.1em; padding: 4px 12px; border-radius: 20px;
}

/* emoji-based destination backgrounds */
.bg-hurghada {
  background: linear-gradient(160deg, #0a5c8a 0%, #0d8c6e 40%, #1e9b5e 80%, #c9a84c 100%);
}
.bg-cairo {
  background: linear-gradient(160deg, #c9a84c 0%, #e8820a 40%, #2e4a1a 80%, #0a5c8a 100%);
}
.bg-luxor {
  background: linear-gradient(160deg, #8b6914 0%, #c9a84c 50%, #e8820a 100%);
}
.bg-sharm {
  background: linear-gradient(160deg, #063a5c 0%, #0a7c9e 40%, #1ab8a0 100%);
}

.dest-emoji {
  font-size: 80px; position: absolute;
  top: 50%; left: 50%; transform: translate(-50%, -70%);
  opacity: 0.25; filter: blur(1px);
}
.dest-card:first-child .dest-emoji { font-size: 120px; }

@media (max-width: 1100px) {
  .dest-grid { grid-template-columns: 1fr 1fr; }
  .dest-card:first-child { grid-column: span 2; }
}
@media (max-width: 768px) {
  .section { padding: 70px 20px; }
  .dest-grid { grid-template-columns: 1fr; }
  .dest-card:first-child { grid-column: span 1; }
}
</style>
