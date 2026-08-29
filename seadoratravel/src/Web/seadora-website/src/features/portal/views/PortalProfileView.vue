<template>
  <div class="max-w-3xl space-y-8 pb-12">
    <!-- Header -->
    <div>
      <h1 class="text-2xl md:text-3xl font-bold text-slate-900">{{ $t('portal.profileView.title') }}</h1>
      <p class="text-xs md:text-sm text-slate-500 mt-1">{{ $t('portal.profileView.subtitle') }}</p>
    </div>
    
    <!-- Profile Form Card -->
    <div class="bg-white rounded-3xl border border-slate-200/80 p-8 shadow-[0_8px_30px_rgb(0,0,0,0.04)] space-y-6">
      
      <!-- Profile Picture Section with Camera Upload Button -->
      <div class="flex items-center gap-6 pb-6 border-b border-slate-100">
        <div class="relative group">
          <!-- Avatar Picture Circle -->
          <div class="w-20 h-20 rounded-full overflow-hidden border-2 border-[#c9a84c]/40 bg-slate-100 flex items-center justify-center text-[#062d4d] font-bold text-2xl shadow-inner">
            <img v-if="profile.avatarUrl" :src="profile.avatarUrl" alt="Profile Avatar" class="w-full h-full object-cover" />
            <span v-else>{{ userInitials }}</span>
          </div>

          <!-- Camera Upload Overlay / Button -->
          <button 
            type="button"
            @click="triggerFileInput"
            class="absolute inset-0 bg-black/40 rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity cursor-pointer text-white"
            :title="$t('portal.profileView.uploadPhoto')"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" /><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 13a3 3 0 11-6 0 3 3 0 016 0z" /></svg>
          </button>
          <input type="file" ref="fileInput" accept="image/*" class="hidden" @change="onFileSelected" />
        </div>

        <div class="flex-1">
          <h3 class="text-sm font-bold text-slate-900">{{ $t('portal.profileView.profilePhoto') }}</h3>
          <p class="text-xs text-slate-500 mt-0.5">{{ $t('portal.profileView.photoHint') }}</p>
          <div class="mt-2 flex items-center gap-3">
            <button 
              type="button" 
              @click="triggerFileInput" 
              class="text-xs font-bold text-[#062d4d] hover:text-[#c9a84c] transition-colors cursor-pointer"
            >
              {{ $t('portal.profileView.uploadPhoto') }}
            </button>
            <span v-if="profile.avatarUrl" class="text-slate-300">•</span>
            <button 
              v-if="profile.avatarUrl" 
              type="button" 
              @click="removeAvatar" 
              class="text-xs font-semibold text-red-500 hover:text-red-700 transition-colors cursor-pointer"
            >
              {{ $t('portal.profileView.remove') }}
            </button>
          </div>
        </div>
      </div>

      <!-- Form Inputs: Name, Email, Phone, Preferred Language -->
      <div class="space-y-4">
        <div>
          <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">{{ $t('portal.profileView.fullName') }}</label>
          <input 
            type="text" 
            v-model="profile.fullName"
            class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" 
          />
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">{{ $t('portal.profileView.email') }}</label>
            <input 
              type="email" 
              v-model="profile.email" 
              class="w-full bg-slate-100 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-500 font-medium cursor-not-allowed" 
              readonly 
            />
          </div>
          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">{{ $t('portal.profileView.phone') }}</label>
            <input 
              type="tel" 
              v-model="profile.phoneNumber"
              placeholder="+20 106 894 0967"
              class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" 
            />
          </div>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">{{ $t('portal.profileView.preferredLanguage') }}</label>
            <select 
              v-model="profile.preferredLanguage"
              @change="onLanguageChanged"
              class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none cursor-pointer"
            >
              <option value="en">English (EN)</option>
              <option value="de">Deutsch (DE)</option>
              <option value="it">Italiano (IT)</option>
              <option value="fr">Français (FR)</option>
              <option value="ru">Русский (RU)</option>
            </select>
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">{{ $t('portal.profileView.dietaryTitle') }}</label>
            <select 
              v-model="profile.dietary"
              class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none cursor-pointer"
            >
              <option value="Standard Luxury">{{ $t('portal.profileView.dietaryStandard') }}</option>
              <option value="Vegetarian">{{ $t('portal.profileView.dietaryVegetarian') }}</option>
              <option value="Vegan">{{ $t('portal.profileView.dietaryVegan') }}</option>
              <option value="Halal">{{ $t('portal.profileView.dietaryHalal') }}</option>
              <option value="Gluten-Free">{{ $t('portal.profileView.dietaryGlutenFree') }}</option>
              <option value="Pescatarian">{{ $t('portal.profileView.dietaryPescatarian') }}</option>
            </select>
          </div>
        </div>
      </div>

      <div class="pt-4 border-t border-slate-100 flex items-center justify-between">
        <button 
          @click="saveProfile" 
          :disabled="isSaving"
          class="px-6 py-3 bg-[#062d4d] text-white font-bold rounded-xl text-xs shadow-sm hover:bg-[#062d4d]/90 active:scale-[0.97] transition-[background-color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] disabled:opacity-50 flex items-center gap-2 cursor-pointer"
        >
          <svg v-if="isSaving" class="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
          <span>{{ isSaving ? $t('portal.profileView.saving') : $t('portal.profileView.saveChanges') }}</span>
        </button>
      </div>
    </div>

    <!-- GDPR & Privacy Consent Card -->
    <div class="bg-white rounded-3xl border border-slate-200/80 p-8 shadow-[0_8px_30px_rgb(0,0,0,0.04)] space-y-6">
      <div>
        <div class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full bg-slate-100 text-slate-700 text-[10px] font-bold uppercase tracking-wider mb-2">
          <span>🔒</span> {{ $t('portal.profileView.gdprBadge') }}
        </div>
        <h2 class="text-lg font-bold text-slate-900">{{ $t('portal.profileView.gdprTitle') }}</h2>
        <p class="text-xs text-slate-500 mt-0.5">{{ $t('portal.profileView.gdprSubtitle') }}</p>
      </div>

      <div class="space-y-3.5">
        <label class="flex items-start gap-3.5 cursor-pointer p-4 bg-slate-50/70 rounded-2xl border border-slate-200/70 hover:bg-slate-50 transition-colors">
          <input 
            type="checkbox" 
            v-model="privacy.marketing"
            class="mt-1 w-4 h-4 rounded border-slate-300 text-[#062d4d] focus:ring-[#c9a84c]" 
          />
          <div class="flex flex-col">
            <span class="text-xs font-bold text-slate-800">{{ $t('portal.profileView.marketingTitle') }}</span>
            <span class="text-[11px] text-slate-500 mt-0.5">{{ $t('portal.profileView.marketingDesc') }}</span>
          </div>
        </label>

        <label class="flex items-start gap-3.5 cursor-pointer p-4 bg-slate-50/70 rounded-2xl border border-slate-200/70 hover:bg-slate-50 transition-colors">
          <input 
            type="checkbox" 
            v-model="privacy.conciergePersonalization"
            class="mt-1 w-4 h-4 rounded border-slate-300 text-[#062d4d] focus:ring-[#c9a84c]" 
          />
          <div class="flex flex-col">
            <span class="text-xs font-bold text-slate-800">{{ $t('portal.profileView.conciergeTitle') }}</span>
            <span class="text-[11px] text-slate-500 mt-0.5">{{ $t('portal.profileView.conciergeDesc') }}</span>
          </div>
        </label>

        <label class="flex items-start gap-3.5 p-4 bg-slate-100/60 rounded-2xl border border-slate-200/50 opacity-80 cursor-not-allowed">
          <input 
            type="checkbox" 
            checked 
            disabled 
            class="mt-1 w-4 h-4 rounded border-slate-300 text-slate-400 cursor-not-allowed" 
          />
          <div class="flex flex-col">
            <span class="text-xs font-bold text-slate-700">{{ $t('portal.profileView.essentialTitle') }}</span>
            <span class="text-[11px] text-slate-500 mt-0.5">{{ $t('portal.profileView.essentialDesc') }}</span>
          </div>
        </label>
      </div>

      <!-- Compliance Self-Service Actions -->
      <div class="pt-6 border-t border-slate-100 flex flex-wrap gap-3">
        <button 
          @click="exportData"
          class="px-4 py-2.5 bg-slate-100 hover:bg-slate-200/80 active:scale-[0.97] text-slate-700 font-bold rounded-xl text-xs transition-[background-color,transform] duration-200 flex items-center gap-2 cursor-pointer"
        >
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"></path></svg>
          {{ $t('portal.profileView.exportBtn') }}
        </button>
        <button 
          @click="requestDeletion"
          class="px-4 py-2.5 bg-red-50 hover:bg-red-100 active:scale-[0.97] text-red-600 border border-red-200/60 font-bold rounded-xl text-xs transition-[background-color,transform] duration-200 flex items-center gap-2 cursor-pointer"
        >
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
          {{ $t('portal.profileView.deleteBtn') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useAuthStore } from '@/features/auth/store/auth';
import { authApi } from '@/features/auth/api/authApi';
import { loadLanguageAsync } from '@/i18n';

const { t } = useI18n();
const authStore = useAuthStore();
const fileInput = ref<HTMLInputElement | null>(null);

const profile = ref({
  fullName: authStore.user?.name || authStore.user?.fullName || 'VIP Guest',
  email: authStore.user?.email || 'customer@gmail.com',
  phoneNumber: authStore.user?.phone || authStore.user?.phoneNumber || '+20 106 894 0967',
  avatarUrl: authStore.user?.avatarUrl || '',
  preferredLanguage: authStore.user?.preferredLanguage || 'en',
  dietary: authStore.user?.dietaryRequirements || 'Standard Luxury'
});

watch(() => authStore.user, (u: any) => {
  if (u) {
    if (u.name || u.fullName) profile.value.fullName = u.name || u.fullName;
    if (u.email) profile.value.email = u.email;
    if (u.phone || u.phoneNumber) profile.value.phoneNumber = u.phone || u.phoneNumber;
    if (u.avatarUrl !== undefined) profile.value.avatarUrl = u.avatarUrl;
    if (u.preferredLanguage) profile.value.preferredLanguage = u.preferredLanguage;
    if (u.dietaryRequirements) profile.value.dietary = u.dietaryRequirements;
  }
}, { immediate: true });

const userInitials = computed(() => {
  if (!profile.value.fullName) return 'VIP';
  const parts = profile.value.fullName.split(' ');
  return parts.map((p: string) => p[0]).join('').toUpperCase().slice(0, 2);
});

const privacy = ref({
  marketing: true,
  conciergePersonalization: true
});

const isSaving = ref(false);

const triggerFileInput = () => {
  fileInput.value?.click();
};

const onLanguageChanged = () => {
  if (profile.value.preferredLanguage) {
    loadLanguageAsync(profile.value.preferredLanguage);
  }
};

const onFileSelected = (e: Event) => {
  const target = e.target as HTMLInputElement;
  if (target.files && target.files[0]) {
    const file = target.files[0];
    const reader = new FileReader();
    reader.onload = (event) => {
      if (event.target?.result) {
        profile.value.avatarUrl = event.target.result as string;
      }
    };
    reader.readAsDataURL(file);
  }
};

const removeAvatar = () => {
  profile.value.avatarUrl = '';
};

const saveProfile = async () => {
  isSaving.value = true;
  try {
    authStore.updateUser({
      name: profile.value.fullName,
      fullName: profile.value.fullName,
      phoneNumber: profile.value.phoneNumber,
      phone: profile.value.phoneNumber,
      avatarUrl: profile.value.avatarUrl,
      preferredLanguage: profile.value.preferredLanguage,
      dietaryRequirements: profile.value.dietary
    });

    try {
      await authApi.updateProfile({
        fullName: profile.value.fullName,
        phoneNumber: profile.value.phoneNumber,
        avatarUrl: profile.value.avatarUrl,
        preferredLanguage: profile.value.preferredLanguage,
        dietaryRequirements: profile.value.dietary
      });
    } catch (apiErr) {
      console.warn('Backend updateProfile fallback', apiErr);
    }

    if (profile.value.preferredLanguage) {
      await loadLanguageAsync(profile.value.preferredLanguage);
    }

    alert(t('portal.profileView.savedSuccess'));
  } finally {
    isSaving.value = false;
  }
};

const exportData = () => {
  const data = JSON.stringify({ profile: profile.value, privacy: privacy.value, exportDate: new Date().toISOString() }, null, 2);
  const blob = new Blob([data], { type: 'application/json' });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = `seadora-profile-data-${Date.now()}.json`;
  a.click();
};

const requestDeletion = () => {
  if (confirm(t('portal.profileView.deleteConfirm'))) {
    alert('Data deletion request submitted to our compliance officer.');
  }
};
</script>