<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store/auth'

const email = ref('')
const password = ref('')
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
    error.value = e.response?.data?.error || e.message || 'Login failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-header">
        <img src="/logo-emblem.png" alt="Seadora Travel" style="width: 5.5rem; height: 5.5rem; margin: 0 auto 1rem; object-fit: contain; filter: drop-shadow(0 4px 10px rgba(0,0,0,0.15));" />
        <h1>Seadora Admin</h1>
        <p>Sign in to manage your travel platform</p>
      </div>

      <div v-if="error" class="error-alert">
        {{ error }}
      </div>

      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label>Email</label>
          <input v-model="email" type="email" placeholder="admin@seadoratravel.com" required :disabled="loading" />
        </div>
        <div class="form-group">
          <label>Password</label>
          <input v-model="password" type="password" placeholder="••••••••" required :disabled="loading" />
        </div>
        <button type="submit" class="login-btn" :disabled="loading">
          {{ loading ? 'Signing In...' : 'Sign In →' }}
        </button>
      </form>
    </div>
  </div>
</template>
