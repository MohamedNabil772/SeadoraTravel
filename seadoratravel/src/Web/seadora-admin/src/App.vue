<script setup lang="ts">
import ConfirmModal from '@/shared/components/ConfirmModal.vue';
import ToastContainer from '@/shared/components/ToastContainer.vue';
</script>

<template>
  <router-view />
  <ConfirmModal />
  <ToastContainer />
</template>
