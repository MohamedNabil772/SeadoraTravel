<template>
  <div v-if="isOpen" class="fixed inset-0 z-[100] flex items-center justify-center p-4 sm:p-0" role="dialog" aria-modal="true" @keydown.esc="closeModal">
    <!-- Backdrop overlay -->
    <div 
      class="absolute inset-0 bg-[#062d4d]/60 backdrop-blur-md transition-opacity duration-300"
      @click="closeModal"
    ></div>

    <!-- Modal Content -->
    <div 
      :ref="setDialogEl"
      tabindex="-1"
      class="relative w-full max-w-md bg-gradient-to-b from-[#062d4d] to-[#041a2e] rounded-3xl shadow-2xl overflow-hidden transform transition-all duration-300 border border-white/10"
      @keydown="trapTab"
    >
      <!-- Close button -->
      <button 
        @click="closeModal"
        class="absolute top-4 right-4 text-white/50 hover:text-white bg-white/5 hover:bg-white/10 rounded-full p-2 transition-colors z-10"
        aria-label="Close modal"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>

      <!-- Decorative Header -->
      <div class="relative pt-8 pb-6 px-8 text-center">
        <div class="absolute inset-0 bg-[url('/pattern.svg')] opacity-5 pointer-events-none"></div>
        <div class="w-16 h-16 mx-auto bg-gradient-to-br from-[#c9a84c] to-[#a38030] rounded-2xl flex items-center justify-center mb-4 shadow-lg shadow-[#c9a84c]/20 transform rotate-3 hover:rotate-0 transition-transform duration-300">
          <svg class="w-8 h-8 text-[#062d4d]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"/>
          </svg>
        </div>
        <h2 class="text-2xl font-bold text-white mb-2">{{ activeTab === 'register' ? 'Create Account' : activeTab === 'forgot' ? 'Reset Password' : 'Welcome Back' }}</h2>
        <p class="text-white/60 text-sm">{{ activeTab === 'register' ? 'Join Seadora for exclusive luxury travel experiences.' : activeTab === 'forgot' ? 'Enter your email to receive a reset link.' : 'Sign in to access your bookings & personalized offers.' }}</p>
      </div>

      <div class="px-8 pb-8">
        <!-- Tabs -->
        <div v-if="activeTab !== 'forgot'" class="flex p-1 bg-[#041a2e] rounded-xl mb-6 shadow-inner border border-white/5">
          <button 
            v-for="tab in ['login', 'register']"
            :key="tab"
            @click="activeTab = tab as any"
            :class="[ 
              'flex-1 py-2 text-xs font-medium rounded-lg transition-all duration-200 capitalize',
              activeTab === tab ? 'bg-[#c9a84c] text-[#062d4d] shadow-md' : 'text-white/70 hover:text-white hover:bg-white/5'
            ]"
          >
            {{ tab }}
          </button>
        </div>

        <!-- Forms Container -->
        <div class="relative min-h-[220px]">
          <!-- Login Form -->
          <transition name="fade" mode="out-in">
            <form v-if="activeTab === 'login'" @submit.prevent="handleLogin" class="space-y-4">
              <div>
                <label class="block text-xs font-medium text-white/60 mb-1 ml-1">Email</label>
                <input v-model="formData.email" type="email" placeholder="Enter your email" required class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
              </div>
              <div>
                <div class="flex justify-between items-center mb-1 ml-1 mr-1">
                  <label class="block text-xs font-medium text-white/60">Password</label>
                  <button type="button" @click="activeTab = 'forgot'" class="text-xs text-[#c9a84c] hover:text-white transition-colors">Forgot?</button>
                </div>
                <input v-model="formData.password" type="password" placeholder="Enter your password" required class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
              </div>
              <button type="submit" class="w-full bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold py-3 px-4 rounded-xl shadow-lg hover:shadow-[#c9a84c]/25 transform hover:-translate-y-0.5 transition-all duration-200">
                Sign In
              </button>
            </form>

            <!-- Register Form -->
            <form v-else-if="activeTab === 'register'" @submit.prevent="handleRegister" class="space-y-4">
              <div>
                <label class="block text-xs font-medium text-white/60 mb-1 ml-1">Full Name</label>
                <input v-model="formData.name" type="text" placeholder="John Doe" required class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
              </div>
              <div>
                <label class="block text-xs font-medium text-white/60 mb-1 ml-1">Email</label>
                <input v-model="formData.email" type="email" placeholder="you@example.com" required class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
              </div>
              <div>
                <label class="block text-xs font-medium text-white/60 mb-1 ml-1">Password</label>
                <input v-model="formData.password" type="password" placeholder="Create a password" required class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
              </div>
              
              <div class="flex items-start gap-2 pt-2">
                <input v-model="gdprConsent" type="checkbox" id="gdpr-consent" required class="mt-1 w-4 h-4 rounded border-white/10 bg-[#041a2e] text-[#c9a84c] focus:ring-[#c9a84c] focus:ring-offset-[#062d4d]" />
                <label for="gdpr-consent" class="text-xs text-white/70 leading-relaxed">
                  I agree to the <a href="#" class="text-[#c9a84c] hover:underline">Terms of Service</a> & <a href="#" class="text-[#c9a84c] hover:underline">Privacy Policy</a> (GDPR Compliance)
                </label>
              </div>

              <button type="submit" class="w-full bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold py-3 px-4 rounded-xl shadow-lg hover:shadow-[#c9a84c]/25 transform hover:-translate-y-0.5 transition-all duration-200">
                Create Account
              </button>
            </form>

            <!-- Forgot Password Form -->
            <form v-else-if="activeTab === 'forgot'" @submit.prevent="activeTab = 'login'" class="space-y-4">
              <div>
                <label class="block text-xs font-medium text-white/60 mb-1 ml-1">Email</label>
                <input type="email" placeholder="Enter your registered email" required class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
              </div>
              <button type="submit" class="w-full bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold py-3 px-4 rounded-xl shadow-lg hover:shadow-[#c9a84c]/25 transform hover:-translate-y-0.5 transition-all duration-200">
                Send Reset Link
              </button>
              <div class="text-center mt-4">
                <button type="button" @click="activeTab = 'login'" class="text-sm text-white/60 hover:text-white transition-colors">Back to Login</button>
              </div>
            </form>
          </transition>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, toRef } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../store/auth';
import { useModalA11y } from '@/shared/utils/modalA11y';

const props = defineProps<{ isOpen: boolean }>();
const emit = defineEmits<{ (e: 'close'): void }>();

const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();
const { setDialogEl, trapTab } = useModalA11y(toRef(props, 'isOpen'));

const activeTab = ref<'login' | 'register' | 'forgot'>('login');
const formData = reactive({
  name: '',
  email: '',
  phone: '',
  password: ''
});
const gdprConsent = ref(false);

const closeModal = () => emit('close');

const redirectAfterAuth = () => {
  const redirect = (route.query.redirect as string) || '/portal';
  if (redirect === '/dashboard' || redirect === '/portal/dashboard') {
    router.push('/portal');
  } else {
    router.push(redirect);
  }
};

const handleLogin = async () => {
  try {
    await authStore.login({ email: formData.email, password: formData.password });
    closeModal();
    redirectAfterAuth();
  } catch (e) {
    console.error('Login Failed', e);
  }
};

const handleRegister = async () => {
  try {
    if (!gdprConsent.value) return;
    await authStore.registerCustomer(formData);
    closeModal();
    redirectAfterAuth();
  } catch (e) {
    console.error('Register Failed', e);
  }
};
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}
.fade-enter-from {
  opacity: 0;
  transform: translateY(5px);
}
.fade-leave-to {
  opacity: 0;
  transform: translateY(-5px);
}
</style>
