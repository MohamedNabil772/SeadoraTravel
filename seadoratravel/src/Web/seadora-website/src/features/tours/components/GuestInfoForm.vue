<script setup lang="ts">
import { ref, watch } from 'vue'
import { Motion } from 'motion-v'
import CountrySelect from './CountrySelect.vue'
import DocumentUploader from './DocumentUploader.vue'

export interface GuestInfo {
  id: string
  fullName: string
  nationality: string
  documentUrl: string
  notes: string
}

export interface GuestError {
  fullName?: string
  nationality?: string
  documentUrl?: string
}

const props = defineProps<{
  guestCount: number
}>()

const guests = ref<GuestInfo[]>(Array.from({ length: Math.max(1, props.guestCount || 1) }, (_, i) => ({
  id: `guest-${i + 1}`,
  fullName: '',
  nationality: '',
  documentUrl: '',
  notes: ''
})))

watch(() => props.guestCount, (newCount) => {
  const count = Math.max(1, newCount || 1)
  if (guests.value.length < count) {
    const currentLen = guests.value.length
    for (let i = currentLen; i < count; i++) {
      guests.value.push({
        id: `guest-${i + 1}`,
        fullName: '',
        nationality: '',
        documentUrl: '',
        notes: ''
      })
    }
  } else if (guests.value.length > count) {
    guests.value = guests.value.slice(0, count)
  }
})

const activeTab = ref(0)
const guestErrors = ref<Record<number, GuestError>>({})

const springTransition = {
  type: "spring",
  stiffness: 350,
  damping: 25
}

const clearError = (index: number, field: keyof GuestError) => {
  if (guestErrors.value[index]) {
    guestErrors.value[index][field] = undefined
  }
}

const validate = (): boolean => {
  let allValid = true
  const newErrors: Record<number, GuestError> = {}
  let firstInvalidIndex = -1

  guests.value.forEach((guest, index) => {
    const err: GuestError = {}
    
    if (!guest.fullName || guest.fullName.trim().length < 2) {
      err.fullName = 'Full Name is required as it appears on passport'
      allValid = false
    }

    if (!guest.documentUrl || guest.documentUrl.trim().length === 0) {
      err.documentUrl = 'Passport or ID document photo is required for Egyptian security permits'
      allValid = false
    }

    if (Object.keys(err).length > 0) {
      newErrors[index] = err
      if (firstInvalidIndex === -1) {
        firstInvalidIndex = index
      }
    }
  })

  guestErrors.value = newErrors

  if (!allValid && firstInvalidIndex !== -1) {
    activeTab.value = firstInvalidIndex
  }

  return allValid
}

defineExpose({
  guests,
  validate
})
</script>

<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-xs uppercase font-extrabold tracking-wider text-[#062d4d] flex items-center gap-1.5">
        <span>👥</span> Guest Information & Documents
      </h3>
      <span class="text-[10px] bg-[#f0f7fc] border border-[#bae6fd] text-[#062d4d] font-bold px-2.5 py-0.5 rounded-full">
        Required for Permits
      </span>
    </div>
    
    <!-- Multi-Guest Tabs with Error Badges -->
    <div v-if="guestCount > 1" class="flex gap-2 p-1.5 bg-slate-100/90 rounded-2xl w-fit flex-wrap border border-slate-200/80 shadow-2xs">
      <button
        v-for="(guest, index) in guests"
        :key="guest.id"
        type="button"
        @click="activeTab = index"
        class="relative px-3.5 py-1.5 text-xs font-bold rounded-xl transition-all duration-200 outline-none flex items-center gap-1.5 cursor-pointer"
        :class="activeTab === index ? 'text-[#062d4d]' : 'text-slate-500 hover:text-slate-800'"
      >
        <span class="relative z-10">Guest {{ index + 1 }}</span>
        <span 
          v-if="guestErrors[index] && (guestErrors[index].fullName || guestErrors[index].documentUrl)" 
          class="relative z-10 w-2 h-2 rounded-full bg-rose-500 animate-pulse"
        ></span>
        <span
          v-else-if="guest.fullName && guest.documentUrl"
          class="relative z-10 text-emerald-600 text-[10px]"
        >✓</span>
        <Motion
          v-if="activeTab === index"
          layoutId="activeGuestTab"
          class="absolute inset-0 bg-white rounded-xl shadow-xs border border-slate-200/60"
          :transition="springTransition"
        />
      </button>
    </div>

    <!-- Active Guest Form Card -->
    <div class="relative overflow-visible bg-white border border-[#e2e8f0] rounded-2xl p-5 sm:p-6 shadow-xs">
      <template v-for="(guest, index) in guests" :key="guest.id">
        <Motion
          v-if="activeTab === index"
          initial="{ opacity: 0, y: 8 }"
          animate="{ opacity: 1, y: 0 }"
          exit="{ opacity: 0, y: -8 }"
          :transition="springTransition"
          class="flex flex-col gap-4.5"
        >
          <!-- Guest Header Indicator -->
          <div class="flex items-center justify-between pb-2 border-b border-slate-100">
            <span class="text-xs font-bold text-slate-700">
              Guest {{ index + 1 }} Details {{ index === 0 ? '(Primary Guest)' : '' }}
            </span>
            <span v-if="guest.documentUrl" class="text-[11px] text-emerald-600 font-semibold flex items-center gap-1">
              <span>🛡️</span> Passport Uploaded
            </span>
          </div>

          <!-- Full Name -->
          <div class="flex flex-col gap-1.5">
            <label :for="'name-' + guest.id" class="text-xs font-bold text-[#334155] flex items-center gap-1">
              <span>Full Name</span>
              <span class="text-rose-500">*</span>
            </label>
            <input 
              :id="'name-' + guest.id"
              v-model="guest.fullName"
              @input="clearError(index, 'fullName')"
              type="text" 
              placeholder="e.g. Alexander Wright (as on passport)"
              class="px-3.5 py-2.5 bg-slate-50 border rounded-xl focus:outline-none focus:bg-white transition-all text-xs sm:text-sm font-medium text-slate-900 placeholder:text-slate-400"
              :class="guestErrors[index]?.fullName ? 'border-rose-400 focus:border-rose-500 focus:ring-1 focus:ring-rose-500 bg-rose-50/30' : 'border-[#cbd5e1] focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d]'"
            />
            <p v-if="guestErrors[index]?.fullName" class="text-[11px] text-rose-500 font-semibold mt-0.5 flex items-center gap-1">
              <span>⚠️</span> {{ guestErrors[index].fullName }}
            </p>
          </div>
          
          <!-- Nationality -->
          <div class="flex flex-col gap-1.5 z-20">
            <label class="text-xs font-bold text-[#334155]">Nationality</label>
            <CountrySelect v-model="guest.nationality" />
          </div>

          <!-- Passport Document Uploader (Per Person) -->
          <div class="flex flex-col gap-1.5">
            <div class="flex items-center justify-between">
              <label class="text-xs font-bold text-[#334155] flex items-center gap-1">
                <span>Passport / ID Photo</span>
                <span class="text-rose-500">*</span>
              </label>
              <span class="text-[10px] text-slate-500 font-medium">JPEG, PNG, WEBP or PDF</span>
            </div>
            
            <div :class="guestErrors[index]?.documentUrl ? 'ring-2 ring-rose-400/40 rounded-xl' : ''">
              <DocumentUploader 
                v-model="guest.documentUrl" 
                @update:modelValue="clearError(index, 'documentUrl')"
              />
            </div>

            <p v-if="guestErrors[index]?.documentUrl" class="text-[11px] text-rose-500 font-semibold mt-1 flex items-center gap-1.5 bg-rose-50 border border-rose-200/80 px-3 py-1.5 rounded-lg w-fit">
              <span>⚠️</span> {{ guestErrors[index].documentUrl }}
            </p>
          </div>
        </Motion>
      </template>
    </div>
  </div>
</template>
