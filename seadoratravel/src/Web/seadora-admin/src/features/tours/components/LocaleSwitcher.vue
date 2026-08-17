<template>
  <div class="flex items-center space-x-4 mb-6">
    <div class="relative flex p-1 bg-navy-900 rounded-lg shadow-inner">
      <div
        class="absolute inset-y-1 bg-navy-700 rounded-md shadow-sm transition-all duration-300 ease-[cubic-bezier(0.23,1,0.32,1)]"
        :style="indicatorStyle"
      ></div>
      <button
        v-for="(locale, index) in locales"
        :key="locale.code"
        ref="tabRefs"
        @click="setLocale(locale.code, index)"
        class="relative z-10 px-4 py-1.5 text-sm font-medium transition-colors duration-300 rounded-md"
        :class="modelValue === locale.code ? 'text-white' : 'text-gray-400 hover:text-gray-200'"
      >
        {{ locale.label }}
      </button>
    </div>
    <div class="text-xs flex items-center space-x-2" :class="saveStateClass">
      <div v-if="saveState === 'saving'" class="w-2 h-2 rounded-full bg-luxury-gold animate-pulse"></div>
      <div v-else-if="saveState === 'saved'" class="w-2 h-2 rounded-full bg-green-500"></div>
      <span>{{ saveMessage }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick, onMounted } from 'vue';

const props = defineProps({
  modelValue: { type: String, required: true },
  locales: { 
    type: Array, 
    default: () => [
      { code: 'en', label: 'English' },
      { code: 'ar', label: 'Arabic' },
      { code: 'fr', label: 'French' }
    ]
  },
  saveState: { type: String, default: 'idle' } // 'idle', 'saving', 'saved', 'error'
});

const emit = defineEmits(['update:modelValue']);
const tabRefs = ref<HTMLElement[]>([]);
const activeIndex = ref(0);
const indicatorStyle = ref({ left: '4px', width: '0px' });

const updateIndicator = () => {
  const activeTab = tabRefs.value[activeIndex.value];
  if (activeTab) {
    indicatorStyle.value = {
      left: `${activeTab.offsetLeft}px`,
      width: `${activeTab.offsetWidth}px`
    };
  }
};

const setLocale = (code: string, index: number) => {
  activeIndex.value = index;
  emit('update:modelValue', code);
  updateIndicator();
};

onMounted(() => {
  const index = props.locales.findIndex(l => l.code === props.modelValue);
  if (index !== -1) activeIndex.value = index;
  nextTick(updateIndicator);
});

watch(() => props.modelValue, (newVal) => {
  const index = props.locales.findIndex(l => l.code === newVal);
  if (index !== -1 && index !== activeIndex.value) {
    activeIndex.value = index;
    nextTick(updateIndicator);
  }
});

const saveMessage = computed(() => {
  if (props.saveState === 'saving') return 'Saving draft...';
  if (props.saveState === 'saved') return 'Draft saved';
  if (props.saveState === 'error') return 'Error saving';
  return '';
});

const saveStateClass = computed(() => {
  if (props.saveState === 'saving') return 'text-luxury-gold';
  if (props.saveState === 'saved') return 'text-green-500';
  if (props.saveState === 'error') return 'text-red-500';
  return 'text-gray-500 opacity-0';
});
</script>
