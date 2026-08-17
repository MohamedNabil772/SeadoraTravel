<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4">
    <!-- Frosted backdrop blur -->
    <div class="fixed inset-0 backdrop-blur-xl bg-[#062d4d]/60 transition-opacity" @click="closeModal"></div>

    <!-- Modal Content -->
    <div class="relative w-full max-w-md bg-[#062d4d] text-white shadow-2xl rounded-2xl overflow-hidden border border-[#c9a84c]/20 transform transition-all">
      <!-- Modal Header -->
      <div class="px-8 pt-8 pb-6 text-center">
        <!-- Close Button -->
        <button @click="closeModal" class="absolute top-4 right-4 text-white/60 hover:text-white transition-colors">
          <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
        
        <!-- Gold Crest / Logo Badge -->
        <div class="mx-auto w-16 h-16 bg-gradient-to-br from-[#c9a84c] to-[#a38030] rounded-full flex items-center justify-center mb-4 shadow-lg shadow-[#c9a84c]/20">
          <span class="text-[#062d4d] font-bold text-2xl font-serif">S</span>
        </div>
        
        <h2 class="text-2xl font-serif font-bold text-white mb-2">{{ $t('auth.welcomeTitle') }}</h2>
        <p class="text-[#c9a84c] text-sm font-medium">{{ $t('auth.welcomeSubtitle') }}</p>
      </div>

      <!-- Segmented Tabs -->
      <div class="px-8 pb-4">
        <div class="flex p-1 space-x-1 bg-[#041a2e] rounded-xl mb-6 border border-white/5">
          <button
            @click="activeTab = 'phone'"
            :class="[
              'w-full py-2.5 text-sm font-medium rounded-lg transition-all duration-200',
              activeTab === 'phone' ? 'bg-[#c9a84c] text-[#062d4d] shadow-md' : 'text-white/70 hover:text-white hover:bg-white/5'
            ]"
          >
            {{ $t('auth.tabs.phone') }}
          </button>
          <button
            @click="activeTab = 'email'"
            :class="[
              'w-full py-2.5 text-sm font-medium rounded-lg transition-all duration-200',
              activeTab === 'email' ? 'bg-[#c9a84c] text-[#062d4d] shadow-md' : 'text-white/70 hover:text-white hover:bg-white/5'
            ]"
          >
            {{ $t('auth.tabs.email') }}
          </button>
        </div>

        <!-- Phone Tab Content -->
        <div v-if="activeTab === 'phone'" class="space-y-4 animate-fade-in">
          <div>
            <label class="block text-xs font-medium text-white/60 mb-1.5 ml-1">{{ $t('auth.phone.label') }}</label>
            <div class="relative">
              <input 
                type="tel" 
                :placeholder="$t('auth.phone.placeholder')" 
                class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all"
              />
            </div>
          </div>
          <button class="w-full bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold py-3 px-4 rounded-xl shadow-lg hover:shadow-[#c9a84c]/25 transform hover:-translate-y-0.5 transition-all duration-200 flex items-center justify-center gap-2">
            <!-- WhatsApp Icon -->
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24"><path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51-.173-.008-.371-.01-.57-.01-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347m-5.421 7.403h-.004a9.87 9.87 0 01-5.031-1.378l-.361-.214-3.741.982.998-3.648-.235-.374a9.86 9.86 0 01-1.51-5.26c.001-5.45 4.436-9.884 9.888-9.884 2.64 0 5.122 1.03 6.988 2.898a9.825 9.825 0 012.893 6.994c-.003 5.45-4.437 9.884-9.885 9.884m8.413-18.297A11.815 11.815 0 0012.05 0C5.495 0 .16 5.335.157 11.892c0 2.096.547 4.142 1.588 5.945L.057 24l6.305-1.654a11.882 11.882 0 005.683 1.448h.005c6.554 0 11.89-5.335 11.893-11.893a11.821 11.821 0 00-3.48-8.413Z"/></svg>
            {{ $t('auth.phone.submit') }}
          </button>
        </div>

        <!-- Email Tab Content -->
        <div v-if="activeTab === 'email'" class="space-y-4 animate-fade-in">
          <div>
            <label class="block text-xs font-medium text-white/60 mb-1.5 ml-1">{{ $t('auth.email.emailLabel') }}</label>
            <input 
              type="email" 
              :placeholder="$t('auth.email.emailPlaceholder')" 
              class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all"
            />
          </div>
          <div>
            <label class="block text-xs font-medium text-white/60 mb-1.5 ml-1">{{ $t('auth.email.passwordLabel') }}</label>
            <input 
              type="password" 
              :placeholder="$t('auth.email.passwordPlaceholder')" 
              class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all"
            />
          </div>
          <button class="w-full bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold py-3 px-4 rounded-xl shadow-lg hover:shadow-[#c9a84c]/25 transform hover:-translate-y-0.5 transition-all duration-200">
            {{ $t('auth.email.submit') }}
          </button>
        </div>

        <!-- Divider -->
        <div class="relative my-6">
          <div class="absolute inset-0 flex items-center">
            <div class="w-full border-t border-[#c9a84c]/30"></div>
          </div>
          <div class="relative flex justify-center text-xs">
            <span class="px-3 bg-[#062d4d] text-[#c9a84c] font-medium tracking-wider">{{ $t('auth.social.divider') }}</span>
          </div>
        </div>

        <!-- Social Login -->
        <div class="space-y-3 pb-2">
          <!-- Google -->
          <button class="w-full flex items-center justify-center gap-3 bg-white text-gray-800 font-medium py-2.5 px-4 rounded-xl hover:bg-gray-50 transition-colors shadow-sm hover:shadow-md hover:shadow-white/10">
            <svg class="w-5 h-5" viewBox="0 0 24 24"><path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/><path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/><path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/><path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/></svg>
            {{ $t('auth.social.google') }}
          </button>
          
          <!-- Facebook -->
          <button class="w-full flex items-center justify-center gap-3 bg-[#1877F2] text-white font-medium py-2.5 px-4 rounded-xl hover:bg-[#166fe5] transition-colors shadow-sm hover:shadow-md hover:shadow-[#1877F2]/20">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24"><path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/></svg>
            {{ $t('auth.social.facebook') }}
          </button>

          <!-- Apple -->
          <button v-if="isAppleDevice" class="w-full flex items-center justify-center gap-3 bg-black text-white font-medium py-2.5 px-4 rounded-xl hover:bg-gray-900 border border-white/20 transition-colors shadow-sm hover:shadow-md hover:shadow-white/10">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24"><path d="M12.152 6.896c-.948 0-2.415-1.078-3.96-1.04-2.04.027-3.91 1.183-4.961 3.014-2.117 3.675-.546 9.103 1.519 12.09 1.013 1.454 2.208 3.09 3.792 3.039 1.52-.065 2.09-.987 3.935-.987 1.831 0 2.35.987 3.96.948 1.637-.026 2.62-1.496 3.609-2.978 1.155-1.684 1.636-3.325 1.662-3.415-.039-.013-3.182-1.221-3.22-4.857-.026-3.04 2.48-4.494 2.597-4.559-1.429-2.09-3.623-2.324-4.39-2.376-2-.156-3.675 1.09-4.503 1.09zM15.53 3.83c.843-1.012 1.4-2.427 1.245-3.83-1.207.052-2.662.805-3.532 1.818-.69.754-1.35 2.182-1.155 3.533 1.352.104 2.648-.48 3.441-1.521z"/></svg>
            {{ $t('auth.social.apple') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

const props = defineProps<{
  isOpen: boolean;
}>();

const emit = defineEmits<{
  (e: 'close'): void;
}>();

const activeTab = ref<'phone' | 'email'>('phone');
const isAppleDevice = ref(false);

const closeModal = () => {
  emit('close');
};

onMounted(() => {
  // Simple check for Apple device (iOS or Mac)
  if (typeof window !== 'undefined') {
    const ua = window.navigator.userAgent;
    isAppleDevice.value = /Mac|iPod|iPhone|iPad/.test(ua);
  }
});
</script>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.3s ease-out forwards;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(5px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
