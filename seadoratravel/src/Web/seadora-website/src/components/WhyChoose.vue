<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const reasons = [
  { id: 'languages', emoji: '🌍', borderColor: 'border-sea' },
  { id: 'safety', emoji: '🛡️', borderColor: 'border-sun' },
  { id: 'luxury', emoji: '💎', borderColor: 'border-grass' },
  { id: 'price', emoji: '🤝', borderColor: 'border-gold' }
]
</script>

<template>
  <section class="section" id="why">
    <div class="section-header">
      <div class="section-eyebrow">
        {{ t('why.eyebrow') }}
      </div>
      <h2>
        <span v-html="t('why.title')"></span>
      </h2>
      <p class="section-sub">
        {{ t('why.description') }}
      </p>
    </div>
    <div class="why-grid">
      <div 
        v-for="reason in reasons" 
        :key="reason.id"
        class="why-card"
        :class="reason.borderColor"
      >
        <div class="why-icon">{{ reason.emoji }}</div>
        <div class="why-title">
          {{ t(`why.items.${reason.id}.title`) }}
        </div>
        <div class="why-text">
          {{ t(`why.items.${reason.id}.text`) }}
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* ─── WHY CHOOSE ─── */
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

.why-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 32px; margin-top: 10px; }
.why-card {
  text-align: center; padding: 40px 28px;
  border-radius: 12px; background: var(--white);
  box-shadow: 0 4px 30px rgba(0,0,0,0.06);
  transition: transform 0.3s, box-shadow 0.3s;
  border-bottom: 3px solid transparent;
}
.why-card:hover {
  transform: translateY(-4px); box-shadow: 0 16px 50px rgba(0,0,0,0.12);
}
.why-card.border-sea { border-color: var(--sea); }
.why-card.border-sun { border-color: var(--sun); }
.why-card.border-grass { border-color: var(--grass); }
.why-card.border-gold { border-color: var(--gold); }

.why-icon { font-size: 40px; margin-bottom: 18px; }
.why-title {
  font-family: 'Playfair Display', serif;
  font-size: 19px; font-weight: 700; color: var(--dark); margin-bottom: 12px;
}
.why-text { font-size: 14px; color: var(--muted); line-height: 1.65; }

@media (max-width: 1100px) {
  .why-grid { grid-template-columns: 1fr 1fr; }
}
@media (max-width: 768px) {
  .section { padding: 70px 20px; }
  .why-grid { grid-template-columns: 1fr 1fr; }
}
</style>
