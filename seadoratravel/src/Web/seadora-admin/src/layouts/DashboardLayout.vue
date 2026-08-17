<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store/auth'
import { Toaster } from 'vue-sonner'
import { Upload, Download, FileText, BookOpen } from 'lucide-vue-next'
import PdfPreviewModal from '@/shared/components/PdfPreviewModal.vue'
import LocaleSwitcher from '@/shared/components/locale/LocaleSwitcher.vue'

const auth = useAuthStore()
const router = useRouter()

const isPdfModalOpen = ref(false)
const pdfDocumentType = ref<'itinerary' | 'brochure'>('itinerary')

function openPdfModal(type: 'itinerary' | 'brochure') {
  pdfDocumentType.value = type
  isPdfModalOpen.value = true
}



function handleLogout() {
  auth.logout()
  router.push('/')
}
</script>

<template>
  <div class="flex h-screen bg-surface-sunken overflow-hidden font-sans text-text-main selection:bg-secondary/30">
    <!-- Sidebar (Luxury Deep Navy) -->
    <aside class="w-64 bg-primary text-text-inverse flex-shrink-0 hidden md:flex flex-col duration-300 ease-linear border-r border-primary-light">
      <!-- Sidebar Header / Logo -->
      <div class="flex items-center justify-between gap-2 px-6 py-6">
        <RouterLink to="/dashboard" class="flex items-center gap-2.5 transition-transform hover:scale-105 duration-300 ease-out">
          <img src="/logo-emblem.png" alt="Seadora" class="w-10 h-10 object-contain drop-shadow-sm" />
          <span class="text-xl font-bold font-serif tracking-widest text-secondary">SEADORA <span class="text-white text-sm tracking-widest font-sans opacity-80">ADMIN</span></span>
        </RouterLink>
      </div>

      <!-- Navigation Links -->
      <nav class="mt-4 flex-1 px-4 space-y-6 overflow-y-auto no-scrollbar pb-6">
        
        <!-- CORE -->
        <div>
          <div class="px-4 mb-2 text-[10px] font-bold text-white/40 uppercase tracking-widest">Core</div>
          <div class="space-y-1">
            <RouterLink 
              to="/dashboard" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
              exact
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">📊</span>
              <span>Dashboard</span>
            </RouterLink>
          </div>
        </div>

        <!-- COMMERCE -->
        <div>
          <div class="px-4 mb-2 text-[10px] font-bold text-white/40 uppercase tracking-widest">Commerce</div>
          <div class="space-y-1">
            <RouterLink 
              to="/tours" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">⛵</span>
              <span>Tours</span>
            </RouterLink>
            <RouterLink 
              to="/bookings" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">📅</span>
              <span>Bookings</span>
            </RouterLink>
          </div>
        </div>

        <!-- TAXONOMY -->
        <div>
          <div class="px-4 mb-2 text-[10px] font-bold text-white/40 uppercase tracking-widest">Taxonomy</div>
          <div class="space-y-1">
            <RouterLink 
              to="/destinations" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">🗺️</span>
              <span>Destinations</span>
            </RouterLink>
            <RouterLink 
              to="/categories" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">🏷️</span>
              <span>Categories</span>
            </RouterLink>
          </div>
        </div>

        <!-- SETTINGS & SYSTEM -->
        <div>
          <div class="px-4 mb-2 text-[10px] font-bold text-white/40 uppercase tracking-widest">System</div>
          <div class="space-y-1">
            <RouterLink 
              to="/users" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">👥</span>
              <span>Users</span>
            </RouterLink>
            <RouterLink 
              to="/localization" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">🌍</span>
              <span>Localization</span>
            </RouterLink>
            <RouterLink 
              to="/currencies" 
              class="group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-white/10 hover:text-white"
              active-class="bg-white/10 text-white shadow-sm ring-1 ring-white/10"
            >
              <span class="opacity-70 group-hover:opacity-100 transition-opacity">💰</span>
              <span>Currencies</span>
            </RouterLink>
          </div>
        </div>
      </nav>

      <!-- Sidebar Footer / Logout -->
      <div class="p-4 border-t border-white/5">
        <button 
          @click="handleLogout" 
          class="w-full group relative flex items-center gap-3 rounded-md py-2 px-4 font-medium text-white/70 transition-all duration-300 hover:bg-red-500/10 hover:text-red-400 text-left cursor-pointer"
        >
          <span class="opacity-70 group-hover:opacity-100">🚪</span>
          <span>Logout</span>
        </button>
      </div>
    </aside>

    <!-- Main Shell -->
    <div class="flex-1 flex flex-col overflow-hidden relative">
      <!-- Header -->
      <header class="h-16 bg-white/80 backdrop-blur-md border-b border-border/60 flex items-center justify-between px-8 z-10">
        <div class="flex items-center gap-4">
          <h2 class="text-xl font-medium text-text-main tracking-tight animate-fade-in">
            {{ $route.name || 'Dashboard' }}
          </h2>
          <!-- Localized content editing indicator -->
          <div class="flex items-center gap-3">
            <LocaleSwitcher />
          </div>
        </div>
        
        <div class="flex items-center gap-5">
          
          <!-- Global Toolbar Actions -->
          <div class="flex items-center gap-2 border-r border-border/60 pr-5 mr-1">
            
            <!-- Excel Import/Export -->
            <div class="flex items-center gap-1 bg-surface-sunken p-1 rounded-lg border border-border/50">
              <button class="flex items-center gap-1.5 text-xs font-medium text-text-muted hover:text-text-main transition-colors px-2.5 py-1.5 rounded-md hover:bg-white hover:shadow-sm">
                <Upload class="w-3.5 h-3.5" />
                <span>Import</span>
              </button>
              <button class="flex items-center gap-1.5 text-xs font-medium text-text-muted hover:text-text-main transition-colors px-2.5 py-1.5 rounded-md hover:bg-white hover:shadow-sm">
                <Download class="w-3.5 h-3.5" />
                <span>Export</span>
              </button>
            </div>

            <div class="w-px h-5 bg-border/60 mx-1"></div>

            <!-- PDF Triggers -->
            <div class="flex items-center gap-1">
              <button @click="openPdfModal('itinerary')" class="group flex items-center gap-1.5 text-xs font-medium text-blue-600/80 hover:text-blue-700 bg-blue-50/50 hover:bg-blue-50 transition-colors px-3 py-1.5 rounded-md border border-blue-100/50 hover:border-blue-200">
                <FileText class="w-3.5 h-3.5 group-hover:scale-110 transition-transform" />
                <span>Itinerary PDF</span>
              </button>
              <button @click="openPdfModal('brochure')" class="group flex items-center gap-1.5 text-xs font-medium text-purple-600/80 hover:text-purple-700 bg-purple-50/50 hover:bg-purple-50 transition-colors px-3 py-1.5 rounded-md border border-purple-100/50 hover:border-purple-200">
                <BookOpen class="w-3.5 h-3.5 group-hover:scale-110 transition-transform" />
                <span>Brochure PDF</span>
              </button>
            </div>
            
          </div>

          <div class="text-right">
            <span class="block text-sm font-semibold text-text-main leading-tight">Administrator</span>
            <span class="block text-[11px] text-text-muted font-sans mt-0.5">{{ auth.user?.email || 'admin@seadora.com' }}</span>
          </div>
          <div class="w-10 h-10 rounded-full bg-gradient-to-tr from-secondary/20 to-secondary/5 border border-secondary/20 flex items-center justify-center text-secondary-dark font-bold text-sm ring-2 ring-white shadow-sm cursor-pointer transition-transform hover:scale-105">
            AD
          </div>
        </div>
      </header>

      <!-- Content Area with simple fade transition -->
      <main class="flex-1 overflow-x-hidden overflow-y-auto p-6 md:p-8 2xl:p-10 bg-surface-sunken">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </main>
    </div>
    
    
    <!-- PDF Preview Modal Component -->
    <PdfPreviewModal
      v-model="isPdfModalOpen"
      pdf-url="" 
      title="Seadora Document Preview"
    />

    <!-- Sonner Toaster for global elegant toasts -->
    <Toaster position="top-right" richColors />
  </div>
</template>

<style>
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

/* Page Transitions */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease-out, transform 0.2s ease-out;
}
.fade-enter-from {
  opacity: 0;
  transform: translateY(4px);
}
.fade-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>
