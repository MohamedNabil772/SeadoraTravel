<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store/auth'
import { Mail, Lock, Eye, EyeOff, ShieldCheck, AlertCircle, ArrowRight, Sparkles } from 'lucide-vue-next'

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const rememberMe = ref(false)
const error = ref('')
const loading = ref(false)

const router = useRouter()
const auth = useAuthStore()

async function handleLogin() {
  error.value = ''
  loading.value = true
  try {
    await auth.login(email.value, password.value)
    router.push('/dashboard')
  } catch (e: any) {
    error.value = e.response?.data?.error || e.message || 'Authentication failed. Please verify your credentials.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen w-full flex flex-col justify-between bg-gradient-to-br from-[#06152B] via-[#0A192F] to-[#020C1B] text-white font-sans relative overflow-hidden selection:bg-secondary/30 selection:text-secondary-light">
    <!-- Ambient Background Lighting -->
    <div class="absolute -top-32 -left-32 w-96 h-96 bg-secondary/10 rounded-full blur-3xl pointer-events-none animate-pulse"></div>
    <div class="absolute -bottom-32 -right-32 w-96 h-96 bg-sky-500/10 rounded-full blur-3xl pointer-events-none"></div>
    <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[550px] h-[550px] bg-primary-light/10 rounded-full blur-[140px] pointer-events-none"></div>

    <!-- Top Bar -->
    <header class="w-full py-6 px-6 sm:px-12 flex items-center justify-between z-10">
      <div class="flex items-center gap-3">
        <img src="/logo-emblem.png" alt="Seadora Emblem" class="w-9 h-9 object-contain drop-shadow-md" />
        <span class="text-lg font-serif font-bold tracking-widest text-secondary">SEADORA <span class="text-xs text-white/60 font-sans tracking-widest font-normal uppercase">PORTAL</span></span>
      </div>
    </header>

    <!-- Main Content / Login Card -->
    <main class="w-full flex-1 flex items-center justify-center p-4 sm:p-6 z-10">
      <div class="w-full max-w-md">
        <!-- Glassmorphic Card -->
        <div class="bg-white/[0.04] backdrop-blur-xl border border-white/10 rounded-3xl p-8 sm:p-10 shadow-[0_24px_60px_rgba(0,0,0,0.5),inset_0_1px_0_rgba(255,255,255,0.1)] relative transition-all duration-300">
          
          <!-- Emblem & Title -->
          <div class="text-center mb-8">
            <div class="inline-flex items-center justify-center p-3 rounded-2xl bg-gradient-to-b from-white/10 to-white/5 border border-white/15 mb-4 shadow-lg ring-1 ring-white/10">
              <img 
                src="/logo-emblem.png" 
                alt="Seadora Emblem" 
                class="w-12 h-12 object-contain drop-shadow-[0_4px_12px_rgba(212,175,55,0.35)]" 
              />
            </div>
            <h1 class="text-2xl sm:text-3xl font-bold font-serif tracking-tight text-white">
              Admin Portal
            </h1>
            <p class="text-xs sm:text-sm text-white/60 mt-1.5 font-light">
              Sign in to manage tours, bookings, and operations
            </p>
          </div>

          <!-- Error Alert -->
          <div 
            v-if="error" 
            class="mb-6 p-3.5 rounded-xl bg-rose-500/10 border border-rose-500/30 flex items-start gap-3 text-rose-200 text-xs sm:text-sm animate-fade-in"
          >
            <AlertCircle class="w-4 h-4 text-rose-400 mt-0.5 shrink-0" />
            <span class="leading-relaxed">{{ error }}</span>
          </div>

          <!-- Login Form -->
          <form @submit.prevent="handleLogin" class="space-y-5">
            <!-- Email Field -->
            <div class="space-y-1.5">
              <label for="login-email" class="block text-xs font-semibold text-white/80 uppercase tracking-wider">
                Email Address
              </label>
              <div class="relative">
                <Mail class="w-4 h-4 text-white/40 absolute left-3.5 top-1/2 -translate-y-1/2 pointer-events-none" />
                <input 
                  id="login-email"
                  v-model="email" 
                  type="email" 
                  placeholder="admin@seadoratravel.com" 
                  required 
                  :disabled="loading"
                  class="w-full pl-10 pr-4 py-3 bg-black/25 border border-white/15 rounded-xl text-sm text-white placeholder:text-white/30 focus:outline-none focus:border-secondary focus:ring-2 focus:ring-secondary/20 transition-all disabled:opacity-50"
                />
              </div>
            </div>

            <!-- Password Field -->
            <div class="space-y-1.5">
              <div class="flex items-center justify-between">
                <label for="login-password" class="block text-xs font-semibold text-white/80 uppercase tracking-wider">
                  Password
                </label>
                <span class="text-[11px] text-secondary/80 hover:underline cursor-pointer font-normal">
                  Forgot?
                </span>
              </div>
              <div class="relative">
                <Lock class="w-4 h-4 text-white/40 absolute left-3.5 top-1/2 -translate-y-1/2 pointer-events-none" />
                <input 
                  id="login-password"
                  v-model="password" 
                  :type="showPassword ? 'text' : 'password'" 
                  placeholder="••••••••••••" 
                  required 
                  :disabled="loading"
                  class="w-full pl-10 pr-11 py-3 bg-black/25 border border-white/15 rounded-xl text-sm text-white placeholder:text-white/30 focus:outline-none focus:border-secondary focus:ring-2 focus:ring-secondary/20 transition-all disabled:opacity-50 font-mono tracking-wider"
                />
                <button 
                  type="button" 
                  @click="showPassword = !showPassword" 
                  class="absolute right-3.5 top-1/2 -translate-y-1/2 text-white/40 hover:text-white transition-colors p-1"
                  tabindex="-1"
                >
                  <EyeOff v-if="showPassword" class="w-4 h-4" />
                  <Eye v-else class="w-4 h-4" />
                </button>
              </div>
            </div>

            <!-- Remember me checkbox -->
            <div class="flex items-center justify-between pt-1">
              <label class="flex items-center gap-2 text-xs text-white/70 cursor-pointer select-none">
                <input 
                  type="checkbox" 
                  v-model="rememberMe"
                  class="w-4 h-4 rounded border-white/20 bg-black/30 text-secondary focus:ring-0 focus:ring-offset-0 cursor-pointer accent-secondary" 
                />
                <span>Remember this device</span>
              </label>
            </div>

            <!-- Submit Button with Emil-style tactile active press -->
            <button 
              type="submit" 
              :disabled="loading"
              class="w-full mt-2 py-3.5 px-6 rounded-xl bg-gradient-to-r from-[#D4AF37] via-[#F4D03F] to-[#D4AF37] text-primary-dark font-bold text-sm tracking-wide shadow-lg shadow-secondary/20 hover:shadow-secondary/30 hover:brightness-105 active:scale-[0.98] transition-all duration-150 ease-out flex items-center justify-center gap-2 cursor-pointer disabled:opacity-60 disabled:cursor-not-allowed select-none"
            >
              <template v-if="loading">
                <div class="w-4 h-4 border-2 border-primary-dark border-t-transparent rounded-full animate-spin"></div>
                <span>Authenticating...</span>
              </template>
              <template v-else>
                <span>Sign In to Console</span>
                <ArrowRight class="w-4 h-4 transition-transform group-hover:translate-x-0.5" />
              </template>
            </button>
          </form>

          <!-- Security Badge -->
          <div class="mt-8 pt-6 border-t border-white/10 flex items-center justify-center gap-2 text-[11px] text-white/40 font-mono">
            <ShieldCheck class="w-3.5 h-3.5 text-secondary" />
            <span>256-Bit SSL Encrypted • Enterprise Auth</span>
          </div>

        </div>
      </div>
    </main>

    <!-- Professional Admin Trademark Footer -->
    <footer class="w-full py-5 px-6 sm:px-12 border-t border-white/10 bg-black/20 backdrop-blur-md flex flex-col sm:flex-row items-center justify-between gap-3 text-xs text-white/50 z-10 select-none">
      <div class="flex items-center gap-2">
        <span class="font-semibold text-white/80 font-serif tracking-wider">SEADORA LUXURY TRAVEL</span>
        <span class="text-white/20">•</span>
        <span>&copy; {{ new Date().getFullYear() }} All Rights Reserved</span>
      </div>

      <div class="flex items-center gap-2 text-xs">
        <span class="text-white/40 text-[11px]">System Architecture & Development by</span>
        <span class="inline-flex items-center gap-1 font-bold text-secondary tracking-wider text-xs uppercase">
          <Sparkles class="w-3 h-3 text-secondary" /> TIM SOLUTIONS<sup class="text-[9px] font-bold">™</sup>
        </span>
      </div>
    </footer>
  </div>
</template>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-4px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeIn 200ms ease-out forwards;
}
</style>
