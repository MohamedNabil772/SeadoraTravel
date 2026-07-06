<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<template>
  <section class="hero">
    <div class="hero-pattern"></div>
    <div class="hero-content">
      <div class="hero-badge">✦ Premium Egypt Travel Since 2010</div>
      <h1>
        <span v-html="t('hero.title')"></span>
      </h1>
      <p>
        {{ t('hero.description') }}
      </p>
      <div class="hero-btns">
        <a href="#trips" class="btn-primary">
          {{ t('hero.exploreBtn') }}
        </a>
        <a href="#contact" class="btn-outline">
          {{ t('hero.consultationBtn') }}
        </a>
      </div>
    </div>
    <div class="hero-stats">
      <div class="stat-card"><div class="num">5000+</div><div class="label">{{ t('hero.stats.guests') }}</div></div>
      <div class="stat-card"><div class="num">40+</div><div class="label">{{ t('hero.stats.packages') }}</div></div>
      <div class="stat-card"><div class="num">15</div><div class="label">{{ t('hero.stats.years') }}</div></div>
    </div>
    <div class="scroll-hint">
      <span>{{ t('hero.scroll') }}</span>
      <div class="scroll-arrow"></div>
    </div>
  </section>
</template>

<style scoped>
/* ─── HERO ─── */
.hero {
  min-height: 92vh;
  background: linear-gradient(160deg, var(--sea-deep) 0%, var(--sea) 40%, #1a6e8a 70%, #0d4d3a 100%);
  position: relative; overflow: hidden;
  display: flex; align-items: center;
}
.hero::before {
  content: '';
  position: absolute; inset: 0;
  background:
    radial-gradient(ellipse 80% 60% at 70% 40%, rgba(232,130,10,0.12) 0%, transparent 60%),
    radial-gradient(ellipse 50% 70% at 20% 80%, rgba(46,125,79,0.15) 0%, transparent 50%);
}
.hero-pattern {
  position: absolute; inset: 0; opacity: 0.04;
  background-image: repeating-linear-gradient(45deg, var(--gold) 0px, var(--gold) 1px, transparent 1px, transparent 40px),
                    repeating-linear-gradient(-45deg, var(--gold) 0px, var(--gold) 1px, transparent 1px, transparent 40px);
}
.hero-content {
  position: relative; z-index: 2;
  padding: 0 48px; max-width: 760px;
  animation: fadeUp 1s ease forwards;
}
.hero-badge {
  display: inline-flex; align-items: center; gap: 8px;
  background: rgba(201,168,76,0.15); border: 1px solid rgba(201,168,76,0.4);
  padding: 6px 16px; border-radius: 20px;
  font-size: 11px; letter-spacing: 0.2em; text-transform: uppercase;
  color: var(--gold); margin-bottom: 28px;
}
.hero h1 {
  font-family: 'Playfair Display', serif;
  font-size: clamp(44px, 6vw, 80px);
  font-weight: 900; color: var(--white); line-height: 1.05;
  margin-bottom: 24px;
}
.hero h1 :deep(em) { font-style: italic; color: var(--sun-light); }
.hero p {
  font-family: 'Cormorant Garamond', serif;
  font-size: 20px; color: rgba(255,255,255,0.75);
  line-height: 1.7; margin-bottom: 40px; max-width: 580px;
}
.hero-btns { display: flex; gap: 16px; flex-wrap: wrap; }
.btn-primary {
  background: linear-gradient(135deg, var(--sun), #f59c20);
  color: var(--white); text-decoration: none; padding: 16px 36px;
  border-radius: 4px; font-size: 14px; font-weight: 600;
  letter-spacing: 0.1em; text-transform: uppercase;
  transition: all 0.3s; box-shadow: 0 8px 30px rgba(232,130,10,0.35);
}
.btn-primary:hover { transform: translateY(-2px); box-shadow: 0 14px 40px rgba(232,130,10,0.45); }
.btn-outline {
  border: 1px solid rgba(255,255,255,0.4); color: var(--white);
  text-decoration: none; padding: 16px 36px;
  border-radius: 4px; font-size: 14px; font-weight: 400;
  letter-spacing: 0.1em; text-transform: uppercase; transition: all 0.3s;
}
.btn-outline:hover { background: rgba(255,255,255,0.1); border-color: rgba(255,255,255,0.7); }
.hero-stats {
  position: absolute; right: 48px; top: 50%; transform: translateY(-50%);
  display: flex; flex-direction: column; gap: 20px; z-index: 2;
}
.stat-card {
  background: rgba(255,255,255,0.08); backdrop-filter: blur(10px);
  border: 1px solid rgba(255,255,255,0.12);
  padding: 20px 28px; border-radius: 8px; text-align: center;
  border-left: 3px solid var(--sun);
}
.stat-card .num {
  font-family: 'Playfair Display', serif;
  font-size: 36px; font-weight: 700; color: var(--sun-light);
}
.stat-card .label { font-size: 11px; letter-spacing: 0.15em; color: rgba(255,255,255,0.6); text-transform: uppercase; }
.scroll-hint {
  position: absolute; bottom: 36px; left: 50%; transform: translateX(-50%);
  color: rgba(255,255,255,0.4); font-size: 11px; letter-spacing: 0.15em;
  text-transform: uppercase; display: flex; flex-direction: column; align-items: center; gap: 8px;
  animation: bounce 2s infinite;
}
.scroll-arrow { width: 20px; height: 20px; border-right: 2px solid; border-bottom: 2px solid; transform: rotate(45deg); border-color: rgba(255,255,255,0.3); }

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(40px); }
  to   { opacity: 1; transform: translateY(0); }
}
@keyframes bounce {
  0%, 100% { transform: translateX(-50%) translateY(0); }
  50%       { transform: translateX(-50%) translateY(8px); }
}

@media (max-width: 1100px) {
  .hero-stats { display: none; }
}
@media (max-width: 768px) {
  .hero-content { padding: 0 20px; }
  .hero h1 { font-size: 38px; }
}
</style>
