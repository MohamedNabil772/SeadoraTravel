<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/features/auth/store/auth'
import { X, Lock, Camera, Save, Eye, EyeOff } from 'lucide-vue-next'
import { useToast } from '@/composables/useToast'

const toast = useToast()

const props = defineProps<{
  isOpen: boolean
}>()

const emit = defineEmits(['close'])

const auth = useAuthStore()

// Form Data
const fullName = ref('')
const phoneNumber = ref('')
const currentPassword = ref('')
const newPassword = ref('')
const showPassword = ref(false)

const email = computed(() => auth.user?.email || 'admin@seadora.com')

onMounted(() => {
  fullName.value = auth.user?.fullName || 'Administrator'
  phoneNumber.value = auth.user?.phoneNumber || ''
})

const isSaving = ref(false)

const handleSave = async () => {
  isSaving.value = true
  // Mock API Call
  await new Promise(resolve => setTimeout(resolve, 800))
  
  if (auth.user) {
    auth.user.fullName = fullName.value
    auth.user.phoneNumber = phoneNumber.value
  }

  isSaving.value = false
  toast.success('Profile settings updated successfully', 'Your changes have been saved to the system.')
  emit('close')
}

const togglePassword = () => {
  showPassword.value = !showPassword.value
}
</script>

<template>
  <Transition name="modal-bounce">
    <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6">
      <div class="fixed inset-0 bg-black/40 backdrop-blur-sm transition-opacity" @click="emit('close')"></div>
      
      <div class="relative w-full max-w-xl bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-full">
        <!-- Header -->
        <div class="px-6 py-5 border-b border-gray-100 flex items-center justify-between bg-gray-50/50">
          <div>
            <h2 class="text-xl font-serif font-bold text-gray-900 tracking-wide">Profile Settings</h2>
            <p class="text-sm text-gray-500 mt-0.5 font-sans">Manage your personal information and security</p>
          </div>
          <button @click="emit('close')" class="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-full transition-colors">
            <X class="w-5 h-5" />
          </button>
        </div>

        <!-- Body -->
        <div class="p-6 overflow-y-auto font-sans flex-1 no-scrollbar">
          
          <!-- Avatar Section -->
          <div class="flex items-center gap-6 mb-8">
            <div class="relative group">
              <div class="w-20 h-20 rounded-full bg-gradient-to-tr from-secondary/20 to-secondary/5 border border-secondary/20 flex items-center justify-center text-secondary-dark font-bold text-2xl shadow-inner">
                {{ fullName.substring(0, 2).toUpperCase() }}
              </div>
              <button class="absolute inset-0 bg-black/40 rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                <Camera class="w-6 h-6 text-white" />
              </button>
            </div>
            <div>
              <h3 class="text-sm font-medium text-gray-900">Profile Picture</h3>
              <p class="text-xs text-gray-500 mt-1">JPG, GIF or PNG. Max size of 2MB.</p>
              <button class="mt-2 text-sm font-semibold text-primary hover:text-primary-dark transition-colors">Change Picture</button>
            </div>
          </div>

          <div class="space-y-5">
            <!-- Email Read-Only -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Email Address</label>
              <div class="relative">
                <input 
                  type="email" 
                  :value="email" 
                  readonly 
                  class="w-full pl-3 pr-10 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-gray-500 text-sm focus:outline-none cursor-not-allowed"
                />
                <div class="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                  <Lock class="w-4 h-4 text-gray-400" />
                </div>
              </div>
              <p class="text-[11px] text-gray-400 mt-1">Primary login credential cannot be changed.</p>
            </div>

            <!-- Full Name -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Full Name</label>
              <input 
                v-model="fullName" 
                type="text" 
                class="w-full px-3 py-2.5 bg-white border border-gray-300 rounded-lg text-gray-900 text-sm focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all shadow-sm"
                placeholder="Enter your full name"
              />
            </div>

            <!-- Phone Number -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Phone Number</label>
              <input 
                v-model="phoneNumber" 
                type="tel" 
                class="w-full px-3 py-2.5 bg-white border border-gray-300 rounded-lg text-gray-900 text-sm focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all shadow-sm"
                placeholder="+1 (555) 000-0000"
              />
            </div>

            <div class="pt-4 pb-2 border-t border-gray-100">
              <h3 class="text-sm font-semibold text-gray-900 mb-4">Security</h3>
              
              <!-- Current Password -->
              <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Current Password</label>
                <div class="relative">
                  <input 
                    :type="showPassword ? 'text' : 'password'" 
                    v-model="currentPassword" 
                    class="w-full pl-3 pr-10 py-2.5 bg-white border border-gray-300 rounded-lg text-gray-900 text-sm focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all shadow-sm"
                    placeholder="Enter current password"
                  />
                  <button type="button" @click="togglePassword" class="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600">
                    <EyeOff v-if="showPassword" class="w-4 h-4" />
                    <Eye v-else class="w-4 h-4" />
                  </button>
                </div>
              </div>

              <!-- New Password -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">New Password</label>
                <input 
                  :type="showPassword ? 'text' : 'password'" 
                  v-model="newPassword" 
                  class="w-full px-3 py-2.5 bg-white border border-gray-300 rounded-lg text-gray-900 text-sm focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all shadow-sm"
                  placeholder="Enter new password"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="px-6 py-4 border-t border-gray-100 bg-gray-50 flex items-center justify-end gap-3">
          <button 
            @click="emit('close')"
            class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors shadow-sm"
          >
            Cancel
          </button>
          <button 
            @click="handleSave"
            :disabled="isSaving"
            class="px-6 py-2 flex items-center gap-2 text-sm font-medium text-white bg-primary rounded-lg hover:bg-primary-dark transition-all shadow-sm hover:shadow-md disabled:opacity-70"
          >
            <Save class="w-4 h-4" v-if="!isSaving" />
            <svg v-else class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
            {{ isSaving ? 'Saving...' : 'Save Changes' }}
          </button>
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.modal-bounce-enter-active {
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.modal-bounce-leave-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}
.modal-bounce-enter-from,
.modal-bounce-leave-to {
  opacity: 0;
  transform: scale(0.95) translateY(10px);
}
.modal-bounce-enter-from .bg-black\/40 {
  opacity: 0;
}
</style>
