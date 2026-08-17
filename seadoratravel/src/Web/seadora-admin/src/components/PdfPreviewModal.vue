<script setup lang="ts">
import { ref, watch } from 'vue'
import { X, Download, FileText, Loader2, Printer } from 'lucide-vue-next'

const props = defineProps<{
  isOpen: boolean
  title?: string
  documentType?: 'itinerary' | 'brochure'
  pdfUrl?: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'download'): void
}>()

const isLoading = ref(true)

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    isLoading.value = true
    // Simulate PDF generation/loading delay
    setTimeout(() => {
      isLoading.value = false
    }, 1500)
  }
})

function close() {
  emit('close')
}

function handleDownload() {
  emit('download')
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal-fade">
      <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center">
        <!-- Backdrop with glassmorphism -->
        <div 
          class="absolute inset-0 bg-black/40 backdrop-blur-sm transition-opacity"
          @click="close"
        ></div>

        <!-- Modal Content -->
        <div class="relative w-full max-w-5xl h-[85vh] bg-white rounded-2xl shadow-2xl flex flex-col overflow-hidden m-4 transform transition-all">
          
          <!-- Header -->
          <div class="flex items-center justify-between px-6 py-4 border-b border-gray-100 bg-gray-50/50">
            <div class="flex items-center gap-3">
              <div class="p-2 bg-blue-50 text-blue-600 rounded-lg">
                <FileText class="w-5 h-5" />
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900 tracking-tight">
                  {{ title || 'Document Preview' }}
                </h3>
                <p class="text-sm text-gray-500 font-medium">
                  {{ documentType === 'itinerary' ? 'Client Itinerary PDF' : 'Marketing Brochure PDF' }}
                </p>
              </div>
            </div>
            
            <div class="flex items-center gap-3">
              <button 
                @click="handleDownload"
                class="flex items-center gap-2 px-4 py-2 bg-gray-900 text-white text-sm font-medium rounded-lg hover:bg-gray-800 transition-colors active:scale-95 duration-200"
              >
                <Download class="w-4 h-4" />
                <span>Download PDF</span>
              </button>
              <button 
                class="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <Printer class="w-5 h-5" />
              </button>
              <div class="w-px h-6 bg-gray-200 mx-1"></div>
              <button 
                @click="close"
                class="p-2 text-gray-400 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <X class="w-5 h-5" />
              </button>
            </div>
          </div>

          <!-- Body / PDF Viewer -->
          <div class="flex-1 bg-gray-100/50 relative overflow-hidden flex items-center justify-center p-6">
            
            <!-- Loading State -->
            <Transition name="fade">
              <div v-if="isLoading" class="absolute inset-0 z-10 flex flex-col items-center justify-center bg-white/80 backdrop-blur-sm">
                <Loader2 class="w-8 h-8 text-blue-600 animate-spin mb-4" />
                <p class="text-gray-600 font-medium animate-pulse">Generating high-quality PDF...</p>
              </div>
            </Transition>

            <!-- PDF Embed (Mocked with a placeholder for now) -->
            <div 
              class="w-full h-full max-w-4xl bg-white shadow-sm border border-gray-200 rounded-xl overflow-hidden transition-opacity duration-500"
              :class="{'opacity-0': isLoading, 'opacity-100': !isLoading}"
            >
              <iframe 
                v-if="pdfUrl && !isLoading" 
                :src="pdfUrl" 
                class="w-full h-full"
                title="PDF Preview"
              ></iframe>
              <div v-else-if="!isLoading" class="w-full h-full flex flex-col items-center justify-center text-gray-400">
                <FileText class="w-16 h-16 mb-4 text-gray-300" />
                <p class="text-lg font-medium text-gray-500">Preview Available in Production</p>
                <p class="text-sm">QuestPDF Generator rendering mock</p>
              </div>
            </div>
          </div>
          
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
/* Emil Kowalski style polished animations */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-from .transform {
  opacity: 0;
  transform: scale(0.96) translateY(10px);
}

.modal-fade-leave-to .transform {
  opacity: 0;
  transform: scale(0.98) translateY(5px);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
