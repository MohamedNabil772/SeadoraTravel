<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-900">{{ $t('portal.documents.title') }}</h1>
        <p class="text-xs text-slate-500 mt-1">{{ $t('portal.documents.subtitle') }}</p>
      </div>
      <button 
        @click="triggerUpload" 
        class="px-4 py-2.5 bg-slate-900 text-white rounded-xl text-xs font-bold shadow-sm hover:bg-slate-800 active:scale-[0.97] transition-[background-color,transform] duration-200 flex items-center gap-2"
      >
        <span>+</span>
        <span>{{ $t('portal.documents.uploadBtn') }}</span>
      </button>
    </div>

    <!-- Documents Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div 
        v-for="doc in documents" 
        :key="doc.id" 
        class="bg-white rounded-3xl border border-slate-200/80 p-6 flex items-center justify-between shadow-[0_8px_30px_rgb(0,0,0,0.04)] hover:shadow-md transition-[box-shadow,transform] duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] group"
      >
        <div class="flex items-center gap-4">
          <div class="w-12 h-12 rounded-2xl bg-slate-50 border border-slate-100 text-[#062d4d] flex items-center justify-center font-bold text-lg group-hover:bg-[#c9a84c]/15 group-hover:text-[#a38030] transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.75" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/></svg>
          </div>
          <div>
            <div class="text-xs font-bold text-[#c9a84c] uppercase tracking-wider">{{ doc.type }}</div>
            <div class="font-bold text-slate-900 text-sm mt-0.5">{{ doc.title }}</div>
            <div class="text-[11px] text-slate-400 mt-1 flex items-center gap-2">
              <span>{{ doc.fileSize }}</span>
              <span>•</span>
              <span class="text-emerald-600 font-semibold">{{ doc.status }}</span>
            </div>
          </div>
        </div>

        <button 
          @click="downloadDoc(doc.title)" 
          class="px-3.5 py-2 bg-slate-100 hover:bg-[#062d4d] hover:text-white active:scale-[0.95] rounded-xl text-xs font-bold text-slate-700 transition-[background-color,color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)] flex items-center gap-1.5"
          :title="'Download ' + doc.title"
        >
          <span>{{ $t('portal.documents.download') }}</span>
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"/></svg>
        </button>
      </div>
    </div>

    <!-- Passport Encrypted Vault Card -->
    <div class="bg-gradient-to-br from-slate-900 to-[#062d4d] text-white rounded-3xl p-7 shadow-lg flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
      <div class="max-w-xl">
        <div class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full bg-emerald-500/20 text-emerald-400 text-[10px] font-bold uppercase tracking-wider mb-2">
          <span>✓</span> {{ $t('portal.documents.encryptedBadge') }}
        </div>
        <h3 class="text-lg font-bold">{{ $t('portal.documents.coastGuardTitle') }}</h3>
        <p class="text-white/70 text-xs mt-1 leading-relaxed">
          {{ $t('portal.documents.coastGuardDesc') }}
        </p>
      </div>
      <button 
        @click="triggerUpload" 
        class="px-5 py-2.5 bg-[#c9a84c] hover:bg-[#d8b85c] text-[#062d4d] font-bold rounded-xl text-xs active:scale-[0.97] transition-[background-color,transform] duration-200 whitespace-nowrap shadow-md"
      >
        {{ $t('portal.documents.updatePassports') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

interface TravelDoc {
  id: string;
  type: string;
  title: string;
  fileSize: string;
  status: string;
}

const documents = ref<TravelDoc[]>([
  {
    id: '1',
    type: 'Official Boarding Voucher',
    title: '7-Night Dahabiya Nile Expedition Voucher',
    fileSize: 'PDF · 1.4 MB',
    status: 'Verified on File'
  },
  {
    id: '2',
    type: 'Marine Port Clearance',
    title: 'Hurghada Red Sea Marina Entry Permit',
    fileSize: 'PDF · 820 KB',
    status: 'Issued & Active'
  },
  {
    id: '3',
    type: 'Travel Protection',
    title: 'Allianz Global VIP Travel Insurance Policy',
    fileSize: 'PDF · 2.1 MB',
    status: 'Active Coverage'
  },
  {
    id: '4',
    type: 'Identification',
    title: 'Passport Copy Manifest (2 VIP Travelers)',
    fileSize: 'Encrypted · 3.6 MB',
    status: 'Verified ✓'
  }
]);

const downloadDoc = (title: string) => {
  alert(`Downloading encrypted ${title}...`);
};

const triggerUpload = () => {
  alert('Please select passport scans or flight manifests to upload into your encrypted vault.');
};
</script>