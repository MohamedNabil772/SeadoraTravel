<script setup lang="ts">
import { ref, onMounted, watch, nextTick, computed } from 'vue';
import { useLanguageStore } from '../../../features/languages/store/languageStore';

const props = withDefaults(defineProps<{
  modelValue: string;
  locales?: { code: string; label: string; flag?: string; status?: 'complete' | 'missing' | 'draft' }[] | null;
}>(), {
  locales: null
});

const emit = defineEmits(['update:modelValue']);
const store = useLanguageStore();

const computedLocales = computed(() => {
  if (props.locales && props.locales.length > 0) return props.locales;
  return store.activeLanguages.map((l: any) => ({
    code: l.code,
    label: l.name,
    flag: l.flag,
    status: 'complete'
  }));
});

const tabsRef = ref<HTMLElement[]>([]);
const activePillStyle = ref({
  width: '0px',
  transform: 'translateX(0px)'
});

const selectTab = (index: number, code: string) => {
  emit('update:modelValue', code);
  updatePillPosition(index);
};

const updatePillPosition = async (index: number) => {
  await nextTick();
  if (!tabsRef.value[index]) return;
  const tab = tabsRef.value[index];
  
  activePillStyle.value = {
    width: `${tab.offsetWidth}px`,
    transform: `translateX(${tab.offsetLeft}px)`
  };
};

watch(() => props.modelValue, async (newVal) => {
  const index = computedLocales.value.findIndex(l => l.code === newVal);
  if (index !== -1) {
    updatePillPosition(index);
  }
});

watch(computedLocales, () => {
  nextTick(() => {
    const index = computedLocales.value.findIndex(l => l.code === props.modelValue);
    if (index !== -1) updatePillPosition(index);
  });
});

onMounted(() => {
  store.init();
  const index = computedLocales.value.findIndex(l => l.code === props.modelValue);
  if (index !== -1) {
    updatePillPosition(index);
  }
});
</script>

<template>
  <div class="locale-tabs-container">
    <div class="locale-tabs-wrapper">
      <div 
        class="active-pill" 
        :style="activePillStyle"
      ></div>
      
      <button
        v-for="(locale, index) in computedLocales"
        :key="locale.code"
        ref="tabsRef"
        @click="selectTab(index, locale.code)"
        class="locale-tab"
        :class="{ 'is-active': modelValue === locale.code }"
        type="button"
      >
        <span v-if="locale.flag" class="flag" aria-hidden="true">{{ locale.flag }}</span>
        <span class="label">{{ locale.label }}</span>
        
        <span 
          v-if="locale.status"
          class="status-badge" 
          :class="`status-${locale.status}`"
          :title="locale.status"
        ></span>
      </button>
    </div>
  </div>
</template>

<style scoped>
.locale-tabs-container {
  display: inline-flex;
  padding: 4px;
  background-color: #f3f4f6;
  border-radius: 9999px;
  position: relative;
  box-shadow: inset 0 1px 2px rgba(0,0,0,0.05);
}

.locale-tabs-wrapper {
  position: relative;
  display: flex;
  gap: 4px;
}

.active-pill {
  position: absolute;
  top: 0;
  left: 0;
  height: 100%;
  background-color: white;
  border-radius: 9999px;
  transition: transform 0.5s cubic-bezier(0.16, 1, 0.3, 1), width 0.5s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 1px 3px rgba(0,0,0,0.1), 0 1px 2px rgba(0,0,0,0.06);
  z-index: 1;
}

.locale-tab {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  border: none;
  background: transparent;
  border-radius: 9999px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  color: #4b5563;
  transition: color 0.3s ease;
  outline: none;
}

.locale-tab:focus-visible {
  box-shadow: 0 0 0 2px #3b82f6;
}

.locale-tab.is-active {
  color: #111827;
}

.flag {
  font-size: 16px;
}

.status-badge {
  width: 6px;
  height: 6px;
  border-radius: 50%;
}

.status-complete {
  background-color: #10b981;
}

.status-missing {
  background-color: #ef4444;
}

.status-draft {
  background-color: #f59e0b;
}
</style>
