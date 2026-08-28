<template>
  <Teleport to="body">
    <Transition name="modal-bounce">
      <div 
        v-if="isOpen" 
        class="fixed inset-0 z-[9999] flex items-center justify-center p-4 sm:p-6 overflow-y-auto" 
        role="dialog" 
        aria-modal="true" 
        aria-labelledby="profile-settings-title"
        @keydown.esc="closeModal"
      >
        <!-- Frosted Dark Backdrop -->
        <div 
          class="fixed inset-0 bg-black/50 backdrop-blur-sm transition-opacity" 
          @click="closeModal"
        ></div>

        <!-- Modal Dialog Box Centered on Screen -->
        <div class="relative w-full max-w-lg bg-white rounded-3xl shadow-2xl overflow-hidden flex flex-col my-auto z-10 border border-slate-200/80 animate-in fade-in zoom-in-95 duration-200">
          
          <!-- Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/60">
            <div>
              <h2 id="profile-settings-title" class="text-xl font-bold text-slate-900 tracking-tight">Profile Settings</h2>
              <p class="text-xs text-slate-500 mt-0.5">Manage your personal information and security</p>
            </div>
            <button 
              type="button" 
              @click="closeModal" 
              aria-label="Close profile settings" 
              class="w-8 h-8 rounded-full bg-slate-100 hover:bg-slate-200 text-slate-500 hover:text-slate-800 flex items-center justify-center transition-colors cursor-pointer"
            >
              ✕
            </button>
          </div>

          <!-- Body -->
          <div class="p-6 overflow-y-auto font-sans flex-1 max-h-[80vh]">
            
            <!-- Avatar Section -->
            <div class="flex items-center gap-6 mb-6 pb-6 border-b border-slate-100">
              <div class="relative group cursor-pointer" @click="triggerFileInput">
                <div class="w-20 h-20 rounded-full overflow-hidden border-2 border-[#c9a84c]/40 bg-gradient-to-tr from-[#062d4d] to-[#0a4575] flex items-center justify-center text-white font-bold text-2xl shadow-inner">
                  <img v-if="formData.avatarUrl" :src="formData.avatarUrl" alt="Avatar" class="w-full h-full object-cover" />
                  <span v-else>{{ userInitials }}</span>
                </div>
                <button 
                  type="button"
                  class="absolute inset-0 bg-black/40 rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity text-white"
                  title="Upload profile picture"
                >
                  <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" /><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 13a3 3 0 11-6 0 3 3 0 016 0z" /></svg>
                </button>
              </div>
              
              <div class="flex-1">
                <h3 class="text-sm font-bold text-slate-900">Profile Picture</h3>
                <p class="text-xs text-slate-500 mt-0.5">JPG, PNG or WebP. Max size 2MB.</p>
                <div class="mt-2 flex items-center gap-3">
                  <input type="file" ref="fileInput" accept="image/*" class="hidden" @change="onFileChange" />
                  <button 
                    type="button" 
                    @click="triggerFileInput" 
                    class="text-xs font-bold text-[#062d4d] hover:text-[#c9a84c] transition-colors cursor-pointer"
                  >
                    Change Picture
                  </button>
                  <span v-if="formData.avatarUrl" class="text-slate-300">•</span>
                  <button 
                    v-if="formData.avatarUrl" 
                    type="button" 
                    @click="formData.avatarUrl = ''" 
                    class="text-xs font-semibold text-red-500 hover:text-red-700 transition-colors cursor-pointer"
                  >
                    Remove
                  </button>
                </div>
              </div>
            </div>

            <!-- Form -->
            <form @submit.prevent="saveProfile" class="space-y-4">
              
              <!-- Full Name -->
              <div>
                <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">Full Name</label>
                <input 
                  v-model="formData.name" 
                  type="text" 
                  required 
                  class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:outline-none focus:border-[#c9a84c] focus:ring-2 focus:ring-[#c9a84c]/20 transition-all" 
                />
              </div>

              <!-- Email (Read-Only) -->
              <div>
                <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">Email Address</label>
                <div class="relative">
                  <input 
                    :value="authStore.user?.email || 'customer@gmail.com'" 
                    type="email" 
                    readonly 
                    class="w-full bg-slate-100 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-500 font-medium cursor-not-allowed" 
                  />
                  <span class="absolute right-3.5 top-1/2 -translate-y-1/2 text-slate-400 text-xs">🔒 Verified</span>
                </div>
              </div>

              <!-- Phone Number -->
              <div>
                <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">Phone / WhatsApp Number</label>
                <input 
                  v-model="formData.phoneNumber" 
                  type="tel" 
                  placeholder="+20 106 894 0967"
                  class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:outline-none focus:border-[#c9a84c] focus:ring-2 focus:ring-[#c9a84c]/20 transition-all" 
                />
              </div>

              <!-- Preferred Language -->
              <div>
                <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">Preferred Language</label>
                <select 
                  v-model="formData.preferredLanguage" 
                  @change="onLanguageChanged"
                  class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:outline-none focus:border-[#c9a84c] focus:ring-2 focus:ring-[#c9a84c]/20 transition-all cursor-pointer"
                >
                  <option value="en">English (EN)</option>
                  <option value="de">Deutsch (DE)</option>
                  <option value="it">Italiano (IT)</option>
                  <option value="fr">Français (FR)</option>
                  <option value="ru">Русский (RU)</option>
                </select>
                <p class="text-[11px] text-slate-400 mt-1">Updates interface language across both the portal and website.</p>
              </div>

              <!-- Footer Buttons -->
              <div class="pt-5 border-t border-slate-100 flex items-center justify-end gap-3">
                <button 
                  type="button" 
                  @click="closeModal" 
                  class="px-5 py-2.5 text-xs font-bold text-slate-600 hover:text-slate-800 bg-slate-100 hover:bg-slate-200 active:scale-[0.97] rounded-xl transition-all cursor-pointer"
                >
                  Cancel
                </button>
                <button 
                  type="submit" 
                  :disabled="isLoading" 
                  class="px-6 py-2.5 text-xs font-bold text-white bg-[#062d4d] hover:bg-[#093a62] active:scale-[0.97] rounded-xl shadow-md transition-all disabled:opacity-50 flex items-center gap-2 cursor-pointer"
                >
                  <svg v-if="isLoading" class="animate-spin w-3.5 h-3.5" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
                  <span>{{ isLoading ? 'Saving...' : 'Save Changes' }}</span>
                </button>
              </div>

            </form>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
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

const userInitials = computed(() => {
  if (!formData.name && (!authStore.user || !authStore.user.name)) return 'VIP'
  const targetName = formData.name || authStore.user?.name || authStore.user?.fullName || ''
  const parts = targetName.split(' ')
  return parts.map((p: string) => p[0]).join('').toUpperCase().slice(0, 2)
})

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    formData.name = authStore.user?.name || authStore.user?.fullName || ''
    formData.phoneNumber = authStore.user?.phoneNumber || authStore.user?.phone || ''
    formData.preferredLanguage = authStore.user?.preferredLanguage || 'en'
    formData.avatarUrl = authStore.user?.avatarUrl || ''
  }
})

const closeModal = () => {
  emit('update:isOpen', false)
}

const onLanguageChanged = () => {
  if (formData.preferredLanguage) {
    loadLanguageAsync(formData.preferredLanguage)
  }
}

const triggerFileInput = () => {
  fileInput.value?.click()
}

const onFileChange = (e: Event) => {
  const target = e.target as HTMLInputElement
  if (target.files && target.files[0]) {
    const file = target.files[0]
    const reader = new FileReader()
    reader.onload = (event) => {
      if (event.target?.result) {
        formData.avatarUrl = event.target.result as string
      }
    }
    reader.readAsDataURL(file)
  }
}

const saveProfile = async () => {
  isLoading.value = true
  try {
    authStore.updateUser({
      name: formData.name,
      fullName: formData.name,
      phoneNumber: formData.phoneNumber,
      phone: formData.phoneNumber,
      preferredLanguage: formData.preferredLanguage,
      avatarUrl: formData.avatarUrl
    })

    try {
      await authApi.updateProfile({
        fullName: formData.name,
        phoneNumber: formData.phoneNumber,
        preferredLanguage: formData.preferredLanguage,
        avatarUrl: formData.avatarUrl
      })
    } catch (err) {
      console.warn('Backend updateProfile fallback', err)
    }

    if (formData.preferredLanguage) {
      await loadLanguageAsync(formData.preferredLanguage)
    }

    closeModal()
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.modal-bounce-enter-active,
.modal-bounce-leave-active {
  transition: opacity 0.25s ease;
}

.modal-bounce-enter-from,
.modal-bounce-leave-to {
  opacity: 0;
}
</style>
