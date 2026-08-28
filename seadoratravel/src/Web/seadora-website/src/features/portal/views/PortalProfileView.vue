<template>
  <div class="max-w-3xl space-y-8 pb-12">
    <!-- Header -->
    <div>
      <h1 class="text-2xl md:text-3xl font-bold text-slate-900">Guest Preferences & Security</h1>
      <p class="text-xs md:text-sm text-slate-500 mt-1">Manage personal contact details, dietary customs, and GDPR compliance settings.</p>
    </div>
    
    <!-- Profile Form Card -->
    <div class="bg-white rounded-3xl border border-slate-200/80 p-8 shadow-[0_8px_30px_rgb(0,0,0,0.04)] space-y-6">
      <div>
        <h2 class="text-lg font-bold text-slate-900">Personal Information</h2>
        <p class="text-xs text-slate-500 mt-0.5">Used for flight manifests, coast guard approvals, and luxury hotel reservations.</p>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">First Name</label>
          <input 
            type="text" 
            v-model="profile.firstName"
            class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" 
          />
        </div>
        <div>
          <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">Last Name</label>
          <input 
            type="text" 
            v-model="profile.lastName"
            class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" 
          />
        </div>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">Email Address</label>
          <input 
            type="email" 
            v-model="profile.email" 
            class="w-full bg-slate-100 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-500 font-medium cursor-not-allowed" 
            readonly 
          />
        </div>
        <div>
          <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">Phone / WhatsApp</label>
          <input 
            type="tel" 
            v-model="profile.phone"
            placeholder="+44 7123 456789"
            class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" 
          />
        </div>
      </div>

      <div>
        <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1.5">Dietary & Onboard Preferences</label>
        <select 
          v-model="profile.dietary"
          class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-900 font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none"
        >
          <option value="Standard Luxury">Standard Curated Menu (Seafood & International)</option>
          <option value="Vegetarian">Vegetarian Gourmet</option>
          <option value="Vegan">Plant-Based / Vegan Luxury</option>
          <option value="Halal">Certified Halal Dining</option>
          <option value="Gluten-Free">Gluten-Free Strict Preparation</option>
          <option value="Pescatarian">Pescatarian Fresh Red Sea Catch</option>
        </select>
      </div>

      <div class="pt-2">
        <button 
          @click="saveProfile" 
          :disabled="isSaving"
          class="px-6 py-3 bg-[#062d4d] text-white font-bold rounded-xl text-xs shadow-sm hover:bg-[#062d4d]/90 active:scale-[0.97] transition-[background-color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] disabled:opacity-50"
        >
          {{ isSaving ? 'Saving...' : 'Update Preferences' }}
        </button>
      </div>
    </div>

    <!-- GDPR & Privacy Consent Card (Light Luxury Theme) -->
    <div class="bg-white rounded-3xl border border-slate-200/80 p-8 shadow-[0_8px_30px_rgb(0,0,0,0.04)] space-y-6">
      <div>
        <div class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full bg-slate-100 text-slate-700 text-[10px] font-bold uppercase tracking-wider mb-2">
          <span>🔒</span> GDPR & European Data Protection
        </div>
        <h2 class="text-lg font-bold text-slate-900">Privacy & Consent Controls</h2>
        <p class="text-xs text-slate-500 mt-0.5">Control how your personal data is utilized by our concierge and travel teams.</p>
      </div>

      <div class="space-y-3.5">
        <label class="flex items-start gap-3.5 cursor-pointer p-4 bg-slate-50/70 rounded-2xl border border-slate-200/70 hover:bg-slate-50 transition-colors">
          <input 
            type="checkbox" 
            v-model="privacy.marketing"
            class="mt-1 w-4 h-4 rounded border-slate-300 text-[#062d4d] focus:ring-[#c9a84c]" 
          />
          <div class="flex flex-col">
            <span class="text-xs font-bold text-slate-800">Exclusive Travel Invitations & Private Itineraries</span>
            <span class="text-[11px] text-slate-500 mt-0.5">Receive private charter announcements, seasonal Nile voyages, and culinary previews.</span>
          </div>
        </label>

        <label class="flex items-start gap-3.5 cursor-pointer p-4 bg-slate-50/70 rounded-2xl border border-slate-200/70 hover:bg-slate-50 transition-colors">
          <input 
            type="checkbox" 
            v-model="privacy.conciergePersonalization"
            class="mt-1 w-4 h-4 rounded border-slate-300 text-[#062d4d] focus:ring-[#c9a84c]" 
          />
          <div class="flex flex-col">
            <span class="text-xs font-bold text-slate-800">Bespoke Concierge Personalization</span>
            <span class="text-[11px] text-slate-500 mt-0.5">Allow our VIP concierge to reference previous trip preferences for personalized yacht & hotel curation.</span>
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
            <span class="text-xs font-bold text-slate-700">Essential Marine & Port Clearance Data</span>
            <span class="text-[11px] text-slate-500 mt-0.5">Strictly required for Egyptian Coast Guard permits, port entry manifests, and guest insurance coverage.</span>
          </div>
        </label>
      </div>

      <!-- Compliance Self-Service Actions -->
      <div class="pt-6 border-t border-slate-100 flex flex-wrap gap-3">
        <button 
          @click="exportData"
          class="px-4 py-2.5 bg-slate-100 hover:bg-slate-200/80 active:scale-[0.97] text-slate-700 font-bold rounded-xl text-xs transition-[background-color,transform] duration-200 flex items-center gap-2"
        >
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"></path></svg>
          Export My Data (GDPR JSON)
        </button>
        <button 
          @click="requestDeletion"
          class="px-4 py-2.5 bg-red-50 hover:bg-red-100 active:scale-[0.97] text-red-600 border border-red-200/60 font-bold rounded-xl text-xs transition-[background-color,transform] duration-200 flex items-center gap-2"
        >
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
          Request Account & Data Deletion
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '@/features/auth/store/auth';

const authStore = useAuthStore();

const profile = ref({
  firstName: authStore.user?.name ? authStore.user.name.split(' ')[0] : 'John',
  lastName: authStore.user?.name && authStore.user.name.split(' ').length > 1 ? authStore.user.name.split(' ').slice(1).join(' ') : 'Doe',
  email: authStore.user?.email || 'customer@gmail.com',
  phone: '+20 106 894 0967',
  dietary: 'Standard Luxury'
});

const privacy = ref({
  marketing: true,
  conciergePersonalization: true
});

const isSaving = ref(false);

const saveProfile = () => {
  isSaving.value = true;
  setTimeout(() => {
    isSaving.value = false;
    alert('Your preferences have been successfully updated.');
  }, 400);
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
  if (confirm('Are you sure you want to request data deletion? Our data protection officer will review and purge your stored records in accordance with GDPR regulations.')) {
    alert('Data deletion request submitted to our compliance officer.');
  }
};
</script>