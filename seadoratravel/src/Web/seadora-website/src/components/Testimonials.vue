<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const testimonials = [
  { id: 'klaus', avatar: '🇩🇪', name: 'Klaus & Monika W.', country: 'Munich, Germany' },
  { id: 'marco', avatar: '🇮🇹', name: 'Marco & Giulia F.', country: 'Milan, Italy' },
  { id: 'natalia', avatar: '🇷🇺', name: 'Natalia & Dmitri K.', country: 'Moscow, Russia' }
]
</script>

<template>
  <section class="section testimonials-bg">
    <div class="section-header">
      <div class="section-eyebrow">
        {{ t('testimonials.eyebrow') }}
      </div>
      <h2>
        <span v-html="t('testimonials.title')"></span>
      </h2>
    </div>
    <div class="testimonials-grid">
      <div v-for="test in testimonials" :key="test.id" class="test-card">
        <div class="stars">★★★★★</div>
        <div class="test-text">
          {{ t(`testimonials.items.${test.id}.text`) }}
        </div>
        <div class="test-author">
          <div class="test-avatar">{{ test.avatar }}</div>
          <div>
            <div class="test-name">{{ test.name }}</div>
            <div class="test-country">{{ test.country }}</div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* ─── TESTIMONIALS ─── */
.section { padding: 100px 48px; }
.section-header { text-align: center; margin-bottom: 70px; }
.section-eyebrow {
  font-size: 11px; letter-spacing: 0.25em; text-transform: uppercase;
  color: var(--gold); font-weight: 600; margin-bottom: 14px;
  display: flex; align-items: center; justify-content: center; gap: 12px;
}
.section-eyebrow::before, .section-eyebrow::after {
  content: ''; width: 40px; height: 1px; background: var(--gold);
}
.section h2 {
  font-family: 'Playfair Display', serif;
  font-size: clamp(32px, 4vw, 52px); font-weight: 700;
  color: var(--white); line-height: 1.15; margin-bottom: 18px;
}
.section h2 :deep(span) { color: var(--sun-light); }

.testimonials-bg { background: linear-gradient(135deg, var(--sea-deep) 0%, var(--sea) 100%); }
.testimonials-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 28px; }
.test-card {
  background: rgba(255,255,255,0.07);
  border: 1px solid rgba(255,255,255,0.12);
  border-radius: 12px; padding: 32px;
}
.stars { color: var(--sun-light); font-size: 16px; margin-bottom: 16px; letter-spacing: 2px; }
.test-text {
  font-family: 'Cormorant Garamond', serif;
  font-size: 17px; font-style: italic;
  color: rgba(255,255,255,0.85); line-height: 1.7; margin-bottom: 24px;
}
.test-author { display: flex; align-items: center; gap: 14px; }
.test-avatar {
  width: 46px; height: 46px; border-radius: 50%;
  background: linear-gradient(135deg, var(--sun), var(--grass));
  display: flex; align-items: center; justify-content: center;
  font-size: 20px;
}
.test-name { font-size: 14px; color: var(--white); font-weight: 500; }
.test-country { font-size: 12px; color: rgba(255,255,255,0.5); }

@media (max-width: 1100px) {
  .testimonials-grid { grid-template-columns: 1fr 1fr; }
}
@media (max-width: 768px) {
  .section { padding: 70px 20px; }
  .testimonials-grid { grid-template-columns: 1fr; }
}
</style>
