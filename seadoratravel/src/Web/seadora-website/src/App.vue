<script setup lang="ts">
import { RouterView } from 'vue-router'
import { MotionConfig } from 'motion-v'
import SeadoraConcierge from '@/components/chat/SeadoraConcierge.vue'
import AppLoader from '@/shared/components/AppLoader.vue'
import AuthModal from '@/features/auth/components/AuthModal.vue'
import { useAuthStore } from '@/features/auth/store/auth'

const authStore = useAuthStore()
</script>

<template>
  <MotionConfig :reducedMotion="'user'">
    <AppLoader />
    <router-view v-slot="{ Component }">
      <transition name="page-fade" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
    <SeadoraConcierge v-if="false" />
    <AuthModal :isOpen="authStore.isAuthModalOpen" @close="authStore.closeAuthModal()" />
  </MotionConfig>
</template>

<style>
/* Global Page Transition Animations */
.page-fade-enter-active,
.page-fade-leave-active {
  transition: opacity 0.6s cubic-bezier(0.16, 1, 0.3, 1), transform 0.6s cubic-bezier(0.16, 1, 0.3, 1);
}

.page-fade-enter-from {
  opacity: 0;
  transform: scale(0.99) translateY(12px);
}
.page-fade-leave-to {
  opacity: 0;
  transform: scale(0.99) translateY(-12px);
}
</style>
