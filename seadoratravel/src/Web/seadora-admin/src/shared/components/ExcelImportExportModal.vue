<script setup lang="ts">
import { ref } from 'vue'
import { X, UploadCloud, Download, FileSpreadsheet, FileText, CheckCircle2, AlertCircle } from 'lucide-vue-next'
import { useToast } from '@/composables/useToast'
import api from '@/services/api'

const props = defineProps<{
  isOpen: boolean
  entity: 'tours' | 'destinations' | 'categories'
  entityTitle: string
}>()

const emit = defineEmits(['close', 'import-complete'])

const toast = useToast()

const selectedFile = ref<File | null>(null)
const isUploading = ref(false)
const importResult = ref<{
  success: boolean
  totalRows?: number
  imported?: number
  updated?: number
  errors?: string[]
  message?: string
} | null>(null)

function handleFileChange(e: Event) {
  const target = e.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    selectedFile.value = target.files[0]
    importResult.value = null
  }
}

function handleDownloadTemplate() {
  const url = `${api.defaults.baseURL || ''}/api/content/api/admin/excel/template/${props.entity}`
  window.open(url, '_blank')
  toast.success('Template Download', `Downloading official ${props.entityTitle} Excel template.`)
}

function handleExportExcel() {
  const url = `${api.defaults.baseURL || ''}/api/content/api/admin/excel/export/${props.entity}`
  window.open(url, '_blank')
  toast.success('Export Started', `Exporting all ${props.entityTitle} to Excel spreadsheet.`)
}

function handleExportPdf() {
  const url = `${api.defaults.baseURL || ''}/api/content/api/admin/pdf/catalog`
  window.open(url, '_blank')
  toast.success('Catalog PDF', 'Generating luxury PDF catalog.')
}

async function handleImport() {
  if (!selectedFile.value) {
    toast.error('Validation Error', 'Please select an .xlsx Excel file to upload.')
    return
  }

  isUploading.value = true
  importResult.value = null

  try {
    const formData = new FormData()
    formData.append('file', selectedFile.value)

    const res = await api.post(`/api/content/api/admin/excel/import/${props.entity}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })

    importResult.value = res.data
    if (res.data.success) {
      toast.success('Import Successful', `Imported ${res.data.imported || 0} new and updated ${res.data.updated || 0} existing records.`)
      emit('import-complete')
    } else {
      toast.error('Import Error', res.data.message || 'Failed to import records.')
    }
  } catch (e: any) {
    console.error('Import error', e)
    toast.error('Import Failed', e.response?.data?.message || 'Server error during spreadsheet parsing.')
  } finally {
    isUploading.value = false
  }
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal-fade">
      <div v-if="isOpen" class="fixed inset-0 z-[9999] flex items-center justify-center p-4 sm:p-6 overflow-y-auto">
        <div class="fixed inset-0 bg-navy-950/60 backdrop-blur-sm transition-opacity" @click="emit('close')"></div>

        <div
          class="relative w-full max-w-xl bg-white rounded-3xl shadow-2xl overflow-hidden flex flex-col my-8 border border-gray-100 animate-modal"
          role="dialog"
          aria-modal="true"
          aria-labelledby="excel-tools-title"
          v-dialog="() => emit('close')"
        >
          <!-- Header -->
          <div class="px-6 py-5 border-b border-gray-100 flex items-center justify-between bg-gradient-to-r from-navy-950 to-navy-900 text-white">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-white/10 border border-white/20 flex items-center justify-center text-xl">
                📊
              </div>
              <div>
                <h2 id="excel-tools-title" class="text-xl font-serif font-bold text-white tracking-wide">
                  Excel & PDF Tools
                </h2>
                <p class="text-xs text-white/70 mt-0.5">Bulk manage {{ entityTitle }} with spreadsheets and luxury PDF catalogs.</p>
              </div>
            </div>
            <button type="button" @click="emit('close')" aria-label="Close Excel and PDF tools" class="p-2 text-white/60 hover:text-white hover:bg-white/10 rounded-full transition-colors">
              <X class="w-5 h-5" />
            </button>
          </div>

          <!-- Body -->
          <div class="p-6 sm:p-8 space-y-6">
            <!-- Action Cards Grid -->
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
              <!-- Download Template -->
              <button
                type="button"
                @click="handleDownloadTemplate"
                class="p-4 rounded-2xl border border-gray-200 hover:border-secondary hover:bg-secondary/5 transition-all text-center flex flex-col items-center gap-2 group cursor-pointer"
              >
                <div class="w-10 h-10 rounded-xl bg-gray-100 group-hover:bg-secondary/20 text-navy-900 group-hover:text-secondary-dark flex items-center justify-center transition-colors">
                  <Download class="w-5 h-5" />
                </div>
                <div>
                  <div class="text-xs font-bold text-gray-900">Excel Template</div>
                  <div class="text-[10px] text-gray-500 mt-0.5">Blank template file</div>
                </div>
              </button>

              <!-- Export Excel -->
              <button
                type="button"
                @click="handleExportExcel"
                class="p-4 rounded-2xl border border-gray-200 hover:border-emerald-500 hover:bg-emerald-50/50 transition-all text-center flex flex-col items-center gap-2 group cursor-pointer"
              >
                <div class="w-10 h-10 rounded-xl bg-gray-100 group-hover:bg-emerald-100 text-navy-900 group-hover:text-emerald-700 flex items-center justify-center transition-colors">
                  <FileSpreadsheet class="w-5 h-5" />
                </div>
                <div>
                  <div class="text-xs font-bold text-gray-900">Export All Data</div>
                  <div class="text-[10px] text-gray-500 mt-0.5">Full .xlsx workbook</div>
                </div>
              </button>

              <!-- Export PDF Catalog -->
              <button
                type="button"
                @click="handleExportPdf"
                class="p-4 rounded-2xl border border-gray-200 hover:border-primary hover:bg-navy-50/50 transition-all text-center flex flex-col items-center gap-2 group cursor-pointer"
              >
                <div class="w-10 h-10 rounded-xl bg-gray-100 group-hover:bg-navy-100 text-navy-900 group-hover:text-primary flex items-center justify-center transition-colors">
                  <FileText class="w-5 h-5" />
                </div>
                <div>
                  <div class="text-xs font-bold text-gray-900">QuestPDF Catalog</div>
                  <div class="text-[10px] text-gray-500 mt-0.5">Luxury PDF portfolio</div>
                </div>
              </button>
            </div>

            <!-- Upload / Import Section -->
            <div class="border-t border-gray-100 pt-5 space-y-4">
              <div class="text-xs font-bold text-gray-800 uppercase tracking-wider flex items-center gap-2">
                <UploadCloud class="w-4 h-4 text-secondary-text" />
                <span>Upload & Import Spreadsheet</span>
              </div>

              <div class="border-2 border-dashed border-gray-200 hover:border-secondary rounded-2xl p-6 text-center bg-gray-50/50 hover:bg-gray-50 transition-all relative">
                <input
                  type="file"
                  accept=".xlsx"
                  @change="handleFileChange"
                  aria-label="Upload Excel spreadsheet"
                  class="absolute inset-0 opacity-0 cursor-pointer w-full h-full"
                />
                <div class="flex flex-col items-center justify-center gap-2">
                  <div class="w-12 h-12 rounded-2xl bg-secondary/15 text-secondary-dark flex items-center justify-center text-xl">
                    <FileSpreadsheet class="w-6 h-6" />
                  </div>
                  <div>
                    <p class="text-sm font-semibold text-gray-800">
                      {{ selectedFile ? selectedFile.name : 'Click or drop .xlsx spreadsheet here' }}
                    </p>
                    <p class="text-xs text-gray-400 mt-0.5">
                      {{ selectedFile ? `${(selectedFile.size / 1024).toFixed(1)} KB` : 'Supports Microsoft Excel (.xlsx) workbooks' }}
                    </p>
                  </div>
                </div>
              </div>

              <!-- Import Results Summary -->
              <div v-if="importResult" class="p-4 rounded-2xl border text-xs space-y-2" :class="importResult.success ? 'bg-emerald-50/60 border-emerald-200 text-emerald-900' : 'bg-red-50 border-red-200 text-red-900'">
                <div class="flex items-center gap-2 font-bold text-sm">
                  <CheckCircle2 v-if="importResult.success" class="w-4 h-4 text-emerald-600" />
                  <AlertCircle v-else class="w-4 h-4 text-red-600" />
                  <span>{{ importResult.success ? 'Spreadsheet Processed Successfully' : 'Processing Completed with Errors' }}</span>
                </div>
                <div class="grid grid-cols-3 gap-2 py-1 text-center font-medium">
                  <div class="bg-white/80 p-2 rounded-xl border border-gray-200/60">
                    <div class="text-[10px] text-gray-500 uppercase">Total Rows</div>
                    <div class="text-base font-bold text-gray-900">{{ importResult.totalRows || 0 }}</div>
                  </div>
                  <div class="bg-white/80 p-2 rounded-xl border border-gray-200/60">
                    <div class="text-[10px] text-emerald-600 uppercase">Imported</div>
                    <div class="text-base font-bold text-emerald-700">{{ importResult.imported || 0 }}</div>
                  </div>
                  <div class="bg-white/80 p-2 rounded-xl border border-gray-200/60">
                    <div class="text-[10px] text-primary uppercase">Updated</div>
                    <div class="text-base font-bold text-primary">{{ importResult.updated || 0 }}</div>
                  </div>
                </div>
                <div v-if="importResult.errors && importResult.errors.length > 0" class="pt-2 border-t border-red-100 text-red-700 space-y-1">
                  <div v-for="(err, idx) in importResult.errors" :key="idx" class="text-[11px]">
                    • {{ err }}
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="px-6 py-4 border-t border-gray-100 bg-gray-50/60 flex items-center justify-end gap-3">
            <button
              type="button"
              @click="emit('close')"
              class="px-4 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 rounded-xl transition-colors"
            >
              Close
            </button>
            <button
              type="button"
              @click="handleImport"
              :disabled="!selectedFile || isUploading"
              class="inline-flex items-center gap-2 px-6 py-2.5 bg-primary hover:bg-primary-light text-white font-medium text-sm rounded-xl shadow-sm hover:shadow-md transition-all active:scale-[0.98] disabled:opacity-50"
            >
              <div v-if="isUploading" class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
              <span>{{ isUploading ? 'Processing File...' : 'Start Import' }}</span>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
}
.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
  transform: scale(0.97);
}

.animate-modal {
  animation: modalEnter 0.25s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes modalEnter {
  from { opacity: 0; transform: scale(0.96) translateY(10px); }
  to { opacity: 1; transform: scale(1) translateY(0); }
}
</style>
