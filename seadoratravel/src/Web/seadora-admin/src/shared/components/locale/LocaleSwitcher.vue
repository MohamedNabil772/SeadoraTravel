<script setup lang="ts">
import { ref, watch, nextTick, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';

const { locale } = useI18n({ useScope: 'global' });

const localesInfo = ref([
  { code: 'en', label: 'En', flag: '🇬🇧' },
  { code: 'de', label: 'De', flag: '🇩🇪' },
  { code: 'it', label: 'It', flag: '🇮🇹' },
  { code: 'fr', label: 'Fr', flag: '🇫🇷' },
  { code: 'ru', label: 'Ru', flag: '🇷🇺' }
]);

const activeIndex = ref(0);
const optionsRef = ref<HTMLElement[]>([]);
const activePillStyle = ref({
  width: '0px',
  transform: 'translateX(0px)'
});

const selectLocale = async (code: string, index: number) => {
  locale.value = code;
  activeIndex.value = index;
  await updatePill(index);
};

const updatePill = async (index: number) => {
  await nextTick();
  if (optionsRef.value[index]) {
    const el = optionsRef.value[index];
    
    // Calculate the position based on the element's position relative to its parent
    // using offsetLeft which generally handles RTL automatically in modern browsers
    // if the container itself has direction: rtl
    activePillStyle.value = {
      width: `${el.offsetWidth}px`,
      transform: `translateX(${el.offsetLeft}px)`
    };
  }
};

const updateDirection = (code: string) => {
  const isRtl = code === 'ar';
  document.documentElement.dir = isRtl ? 'rtl' : 'ltr';
  // Also add a class to body for easier CSS styling
  if (isRtl) {
    document.body.classList.add('is-rtl');
  } else {
    document.body.classList.remove('is-rtl');
  }
  
  // Re-calculate pill after layout changes
  setTimeout(() => {
    updatePill(activeIndex.value);
  }, 50);
};

watch(locale, (newLoc) => {
  const index = localesInfo.value.findIndex(l => l.code === newLoc);
  if (index !== -1) {
    activeIndex.value = index;
    updateDirection(newLoc);
    updatePill(index);
  }
});

onMounted(() => {
  const index = localesInfo.value.findIndex(l => l.code === locale.value);
  const initialIndex = index !== -1 ? index : 0;
  activeIndex.value = initialIndex;
  updateDirection(locale.value);
  updatePill(initialIndex);
  
  // Handle window resize
  window.addEventListener('resize', () => updatePill(activeIndex.value));
});
</script>

<template>
  <div class="locale-switcher">
    <div class="switcher-track">
      <div class="active-pill" :style="activePillStyle"></div>
      <button
        v-for="(loc, index) in localesInfo"
        :key="loc.code"
        ref="optionsRef"
        class="switcher-option"
        :class="{ 'is-active': activeIndex === index }"
        @click="selectLocale(loc.code, index)"
        type="button"
      >
        <span class="flag">{{ loc.flag }}</span>
        <span class="label">{{ loc.label }}</span>
      </button>
    </div>
  </div>
</template>

<style scoped>
.locale-switcher {
  display: inline-flex;
  padding: 4px;
  background-color: #f3f4f6;
  border-radius: 9999px;
  box-shadow: inset 0 1px 2px rgba(0,0,0,0.05);
}

.switcher-track {
  position: relative;
  display: flex;
  gap: 2px;
}

[dir="rtl"] .switcher-track {
  /* In RTL, we might need specific handling if transform is acting weird, 
     but usually transform: translateX(offsetLeft) works if we don't reverse the flow.
     If issues arise, we can use logical properties. */
}

.active-pill {
  position: absolute;
  top: 0;
  /* Always left: 0 so translateX is relative to the left edge of the track */
  left: 0;
  height: 100%;
  background-color: white;
  border-radius: 9999px;
  transition: transform 0.5s cubic-bezier(0.16, 1, 0.3, 1), width 0.5s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 1px 3px rgba(0,0,0,0.1), 0 1px 2px rgba(0,0,0,0.06);
  z-index: 1;
}

/* Fix RTL for active pill animation */
[dir="rtl"] .active-pill {
  right: 0;
  left: auto;
}

.switcher-option {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  border: none;
  background: transparent;
  border-radius: 9999px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 500;
  color: #6b7280;
  transition: color 0.3s ease;
  outline: none;
}

.switcher-option:focus-visible {
  box-shadow: 0 0 0 2px #3b82f6;
}

.switcher-option.is-active {
  color: #111827;
}

.flag {
  font-size: 14px;
}
</style>
