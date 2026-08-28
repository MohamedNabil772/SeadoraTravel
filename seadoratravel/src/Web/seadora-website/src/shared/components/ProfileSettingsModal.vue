<template>
  <div v-if="isOpen" class="fixed inset-0 z-[100] flex items-center justify-center p-4 sm:p-0" role="dialog" aria-modal="true" @keydown.esc="closeModal">
    <div 
      class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm transition-opacity duration-300"
      @click="closeModal"
    ></div>

    <div class="relative w-full max-w-md bg-white rounded-3xl shadow-2xl overflow-hidden transform transition-all duration-300">
      <!-- Header -->
      <div class="px-6 py-5 border-b border-slate-100 flex justify-between items-center bg-[#F8FAFC]">
        <h2 class="text-lg font-bold text-[#062d4d]">My Profile Settings</h2>
        <button 
          @click="closeModal"
          class="text-slate-400 hover:text-slate-600 bg-slate-100 hover:bg-slate-200 rounded-full p-1.5 transition-colors"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
        </button>
      </div>

      <!-- Content -->
      <div class="p-6">
        <form @submit.prevent="saveProfile" class="space-y-5">
          
          <!-- Avatar Upload -->
          <div class="flex items-center gap-4">
            <div class="w-16 h-16 rounded-full overflow-hidden border-2 border-slate-200 bg-slate-100 shrink-0 relative group cursor-pointer" @click="triggerFileInput">
              <img v-if="formData.avatarUrl" :src="formData.avatarUrl" class="w-full h-full object-cover" />
              <div v-else class="w-full h-full flex items-center justify-center text-[#062d4d] font-bold text-xl">
                {{ userInitials }}
              </div>
              <div class="absolute inset-0 bg-black/40 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" /><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 13a3 3 0 11-6 0 3 3 0 016 0z" /></svg>
              </div>
            </div>
            <div class="flex-1">
              <label class="block text-xs font-semibold text-slate-700 mb-1">Profile Photo</label>
              <input type="file" ref="fileInput" accept="image/*" class="hidden" @change="onFileChange" />
              <p class="text-[11px] text-slate-500 mb-2">Click the image to upload a new avatar. Square, max 2MB.</p>
              <input v-model="formData.avatarUrl" type="text" placeholder="Or paste image URL" class="w-full bg-slate-50 border border-slate-200 rounded-lg px-3 py-2 text-sm text-slate-800 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
            </div>
          </div>

          <!-- Name -->
          <div>
            <label class="block text-xs font-semibold text-slate-700 mb-1">Full Name</label>
            <input v-model="formData.name" type="text" required class="w-full bg-slate-50 border border-slate-200 rounded-lg px-3 py-2 text-sm text-slate-800 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
          </div>

          <!-- Phone -->
          <div>
            <label class="block text-xs font-semibold text-slate-700 mb-1">Phone Number</label>
            <input v-model="formData.phoneNumber" type="tel" class="w-full bg-slate-50 border border-slate-200 rounded-lg px-3 py-2 text-sm text-slate-800 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all" />
          </div>

          <!-- Preferred Language -->
          <div>
            <label class="block text-xs font-semibold text-slate-700 mb-1">Preferred Language</label>
            <select v-model="formData.preferredLanguage" class="w-full bg-slate-50 border border-slate-200 rounded-lg px-3 py-2 text-sm text-slate-800 focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c] transition-all">
              <option value="en">English (EN)</option>
              <option value="de">Deutsch (DE)</option>
              <option value="it">Italiano (IT)</option>
              <option value="fr">Français (FR)</option>
              <option value="ru">Русский (RU)</option>
            </select>
          </div>

          <div class="pt-4 border-t border-slate-100 flex justify-end gap-3">
            <button type="button" @click="closeModal" class="px-4 py-2 text-sm font-medium text-slate-600 hover:text-slate-800 bg-slate-100 hover:bg-slate-200 rounded-lg transition-colors">
              Cancel
            </button>
            <button type="submit" :disabled="isLoading" class="px-5 py-2 text-sm font-bold text-white bg-[#062d4d] hover:bg-[#083a63] rounded-lg shadow-md transition-colors disabled:opacity-50 flex items-center gap-2">
              <svg v-if="isLoading" class="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
              Save Changes
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed } from 'vue'
import { useAuthStore } from '@/features/auth/store/auth'
import { authApi } from '@/features/auth/api/authApi'
import { loadLanguageAsync } from '@/i18n'

const props = defineProps<{ isOpen: boolean }>()
const emit = defineEmits<{ (e: 'update:isOpen', val: boolean): void }>()

const authStore = useAuthStore()
const isLoading = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)

const formData = reactive({
  name: '',
  phoneNumber: '',
  preferredLanguage: 'en',
  avatarUrl: ''
})

watch(() => props.isOpen, (newVal) => {
  if (newVal && authStore.user) {
    formData.name = authStore.user.name || ''
    formData.phoneNumber = authStore.user.phoneNumber || ''
    formData.preferredLanguage = authStore.user.preferredLanguage || 'en'
    formData.avatarUrl = authStore.user.avatarUrl || ''
  }
})

const userInitials = computed(() => {
  if (!formData.name) return 'VIP'
  const names = formData.name.split(' ')
  return names.map((n) => n[0]).join('').toUpperCase().slice(0, 2)
})

const closeModal = () => emit('update:isOpen', false)

const triggerFileInput = () => fileInput.value?.click()

const onFileChange = (e: Event) => {
  const target = e.target as HTMLInputElement
  if (target.files && target.files[0]) {
    const file = target.files[0]
    // In a real app we'd upload to S3/Cloudinary here.
    // For now, we generate a local blob URL.
    formData.avatarUrl = URL.createObjectURL(file)
  }
}

const saveProfile = async () => {
  try {
    isLoading.value = true
    // Wait for the backend update
    await authApi.updateProfile(formData)
    
    // Update local store
    authStore.updateUser(formData)
    
    // If language changed, apply immediately
    if (formData.preferredLanguage) {
      await loadLanguageAsync(formData.preferredLanguage)
    }

    closeModal()
  } catch (error) {
    console.error('Failed to update profile', error)
  } finally {
    isLoading.value = false
  }
}
</script>
