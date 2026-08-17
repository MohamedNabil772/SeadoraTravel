<script setup lang="ts">
const props = defineProps<{
  modelValue: string
}>()

const emit = defineEmits(['update:modelValue'])

const LuxuryIcons = [
  '⛵', '🏖️', '🏨', '🏝️', '🛳️', '🌴', '🏰', '🍷', '💎', '✈️', 
  '🌅', '🍹', '🏄', '🗺️', '🛎️', '🥂', '🏛️', '🌟', '🐬', '🦞'
]
</script>

<template>
  <div class="luxury-icon-picker">
    <button
      v-for="icon in LuxuryIcons"
      :key="icon"
      type="button"
      class="icon-btn"
      :class="{ active: modelValue === icon }"
      @click="emit('update:modelValue', icon)"
    >
      {{ icon }}
    </button>
  </div>
</template>

<style scoped>
.luxury-icon-picker {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(40px, 1fr));
  gap: 8px;
  padding: 12px;
  background: rgba(0, 0, 0, 0.2);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.05);
}
.icon-btn {
  font-size: 20px;
  background: transparent;
  border: 1px solid transparent;
  border-radius: 6px;
  padding: 8px;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
  display: flex;
  align-items: center;
  justify-content: center;
}
.icon-btn:hover {
  background: rgba(255, 255, 255, 0.1);
  transform: scale(1.1);
}
.icon-btn.active {
  background: rgba(26, 139, 196, 0.2);
  border-color: #1a8bc4;
  box-shadow: 0 0 12px rgba(26, 139, 196, 0.3);
  transform: scale(1.1);
}
</style>
