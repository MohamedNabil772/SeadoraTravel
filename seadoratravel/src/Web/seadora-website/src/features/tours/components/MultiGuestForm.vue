<template>
  <div class="multi-guest-form space-y-4">
    <TransitionGroup 
      name="guest-list" 
      tag="div" 
      class="space-y-4"
    >
      <div 
        v-for="(guest, index) in guests" 
        :key="guest.id"
        class="guest-card bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden"
        :class="{ 'ring-2 ring-red-400 shake-animation': hasError(guest.id) }"
      >
        <!-- Card Header (Accordion Toggle) -->
        <div 
          @click="toggleGuest(guest.id)"
          class="flex items-center justify-between p-4 bg-slate-50/50 cursor-pointer hover:bg-slate-50 transition-colors"
        >
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-[#062d4d] text-white flex items-center justify-center text-sm font-bold shadow-sm">
              {{ index + 1 }}
            </div>
            <div>
              <h4 class="text-sm font-bold text-slate-800">
                Guest {{ index + 1 }} <span v-if="index === 0" class="text-[#c9a84c] ml-1 text-xs uppercase tracking-wider">(Lead)</span>
              </h4>
              <p class="text-[11px] text-slate-500 font-medium">
                {{ guest.fullName || 'New Traveler' }}
              </p>
            </div>
          </div>
          
          <div class="flex items-center gap-3">
            <!-- Copy Lead Details Button for Guests > 1 -->
            <button 
              v-if="index > 0 && !guest.isFilledOut" 
              @click.stop="copyLeadDetails(index)"
              class="px-3 py-1.5 rounded-lg bg-emerald-50 text-emerald-600 text-[11px] font-bold hover:bg-emerald-100 transition-colors flex items-center gap-1.5"
            >
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7v8a2 2 0 002 2h6M8 7V5a2 2 0 012-2h4.586a1 1 0 01.707.293l4.414 4.414a1 1 0 01.293.707V15a2 2 0 01-2 2h-2M8 7H6a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2v-2"></path></svg>
              Same Hotel
            </button>
            
            <svg 
              class="w-5 h-5 text-slate-400 transition-transform duration-300"
              :class="{ 'rotate-180': expandedGuest === guest.id }"
              fill="none" stroke="currentColor" viewBox="0 0 24 24"
            >
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
            </svg>
          </div>
        </div>

        <!-- Card Body -->
        <div 
          class="grid-transition" 
          :class="expandedGuest === guest.id ? 'grid-rows-[1fr]' : 'grid-rows-[0fr]'"
        >
          <div class="overflow-hidden">
            <div class="p-4 sm:p-5 border-t border-slate-100 space-y-4">
              <!-- Grid for Inputs -->
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <!-- Full Name -->
                <div class="space-y-1.5">
                  <label class="block text-xs font-bold text-slate-700">Full Name <span class="text-red-500">*</span></label>
                  <input 
                    type="text" 
                    v-model="guest.fullName"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] transition-colors bg-white"
                    placeholder="As written on Passport"
                  />
                </div>
                
                <!-- Nationality -->
                <div class="space-y-1.5 z-20">
                  <label class="block text-xs font-bold text-slate-700">Nationality <span class="text-red-500">*</span></label>
                  <CountrySelect v-model="guest.nationality" />
                </div>

                <!-- Email & Phone (Lead Only) -->
                <template v-if="index === 0">
                  <div class="space-y-1.5">
                    <label class="block text-xs font-bold text-slate-700">Email Address <span class="text-red-500">*</span></label>
                    <input 
                      type="email" 
                      v-model="guest.email"
                      class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] transition-colors bg-white"
                      placeholder="your@email.com"
                    />
                  </div>
                  <div class="space-y-1.5">
                    <label class="block text-xs font-bold text-slate-700">WhatsApp Number <span class="text-red-500">*</span></label>
                    <input 
                      type="tel" 
                      v-model="guest.whatsapp"
                      class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] transition-colors bg-white"
                      placeholder="+1 234 567 8900"
                    />
                  </div>
                </template>

                <!-- Hotel Info -->
                <div class="space-y-1.5 sm:col-span-2">
                  <label class="block text-xs font-bold text-slate-700">Hotel Name & Room Number</label>
                  <div class="flex gap-2">
                    <input 
                      type="text" 
                      v-model="guest.hotelName"
                      class="flex-1 px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] transition-colors bg-white"
                      placeholder="Hotel Name"
                    />
                    <input 
                      type="text" 
                      v-model="guest.roomNumber"
                      class="w-32 px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] transition-colors bg-white"
                      placeholder="Room #"
                    />
                  </div>
                </div>
              </div>

              <!-- Passport Dropzone -->
              <div class="pt-2">
                <label class="block text-xs font-bold text-slate-700 mb-2">Passport / ID Photo (Required by Tourism Police) <span class="text-red-500">*</span></label>
                
                <div 
                  class="relative border-2 border-dashed rounded-xl p-6 transition-all duration-300 flex flex-col items-center justify-center text-center cursor-pointer group"
                  :class="[
                    dragActive === guest.id ? 'border-[#062d4d] bg-[#062d4d]/5 scale-[0.98]' : 'border-slate-300 hover:border-[#c9a84c] hover:bg-slate-50',
                    guest.passportPreview ? 'border-emerald-400 bg-emerald-50/30' : ''
                  ]"
                  @dragenter.prevent="dragActive = guest.id"
                  @dragleave.prevent="dragActive = null"
                  @dragover.prevent
                  @drop.prevent="handleDrop($event, guest)"
                  @click="triggerFileInput(guest.id)"
                >
                  <input 
                    type="file" 
                    :id="`file-${guest.id}`" 
                    class="hidden" 
                    accept="image/*,.pdf" 
                    @change="handleFileSelect($event, guest)"
                  />
                  
                  <!-- Preview State -->
                  <template v-if="guest.passportPreview">
                    <div class="absolute inset-0 w-full h-full p-2">
                      <div class="w-full h-full relative rounded-lg overflow-hidden bg-slate-100 flex items-center justify-center">
                        <img v-if="guest.passportPreview.type.startsWith('image/')" :src="guest.passportPreview.url" class="object-cover w-full h-full opacity-90 group-hover:opacity-100 transition-opacity" />
                        <div v-else class="text-slate-500 font-semibold text-sm">PDF Document Attached</div>
                        
                        <!-- Overlay -->
                        <div class="absolute inset-0 bg-slate-900/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                          <span class="text-white text-xs font-bold px-3 py-1.5 rounded-full bg-slate-900/60 backdrop-blur-sm">Replace File</span>
                        </div>
                      </div>
                    </div>
                  </template>
                  
                  <!-- Empty State -->
                  <template v-else>
                    <div class="w-10 h-10 rounded-full bg-slate-100 flex items-center justify-center mb-3 group-hover:scale-110 transition-transform duration-300 group-hover:bg-amber-50 group-hover:text-[#c9a84c] text-slate-400">
                      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12"></path></svg>
                    </div>
                    <p class="text-sm font-bold text-slate-700 mb-1 group-hover:text-[#062d4d] transition-colors">
                      Click or drag file here
                    </p>
                    <p class="text-[11px] text-slate-500">
                      JPG, PNG, or PDF up to 5MB
                    </p>
                  </template>
                </div>
              </div>

            </div>
          </div>
        </div>
      </div>
    </TransitionGroup>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import CountrySelect from './CountrySelect.vue'

const props = defineProps({
  totalGuests: {
    type: Number,
    required: true,
    default: 1
  }
})

const emit = defineEmits(['update:guests'])

interface GuestData {
  id: string
  fullName: string
  nationality: string
  email?: string
  whatsapp?: string
  hotelName: string
  roomNumber: string
  passportPreview: any
  file: File | null
  isFilledOut: boolean
}

const guests = ref<GuestData[]>([])
const expandedGuest = ref<string>('guest-0')
const dragActive = ref<string | null>(null)
const errorIds = ref<string[]>([])

// Initialize guests based on total count
const syncGuests = () => {
  const currentCount = guests.value.length
  
  if (props.totalGuests > currentCount) {
    // Add new guests
    for (let i = currentCount; i < props.totalGuests; i++) {
      guests.value.push({
        id: `guest-${i}`,
        fullName: '',
        nationality: '',
        email: i === 0 ? '' : undefined,
        whatsapp: i === 0 ? '' : undefined,
        hotelName: '',
        roomNumber: '',
        passportPreview: null,
        file: null,
        isFilledOut: false
      })
    }
    // Expand the newly added guest
    setTimeout(() => {
      expandedGuest.value = `guest-${currentCount}`
    }, 100)
  } else if (props.totalGuests < currentCount) {
    // Remove guests
    guests.value = guests.value.slice(0, props.totalGuests)
  }
}

onMounted(() => {
  syncGuests()
})

watch(() => props.totalGuests, () => {
  syncGuests()
})

watch(guests, (newVal) => {
  emit('update:guests', newVal)
}, { deep: true })

const toggleGuest = (id: string) => {
  if (expandedGuest.value === id) {
    expandedGuest.value = ''
  } else {
    expandedGuest.value = id
  }
}

const copyLeadDetails = (index: number) => {
  if (guests.value[0] && guests.value[index]) {
    guests.value[index].hotelName = guests.value[0].hotelName
    guests.value[index].roomNumber = guests.value[0].roomNumber
    guests.value[index].nationality = guests.value[0].nationality
    guests.value[index].isFilledOut = true
    
    // Tiny tactile vibration if supported
    if (navigator.vibrate) navigator.vibrate(50)
  }
}

const hasError = (id: string) => {
  return errorIds.value.includes(id)
}

const triggerErrorShake = (id: string) => {
  if (!errorIds.value.includes(id)) {
    errorIds.value.push(id)
    if (navigator.vibrate) navigator.vibrate([50, 50, 50])
    
    setTimeout(() => {
      errorIds.value = errorIds.value.filter(e => e !== id)
    }, 600)
  }
}

const handleFile = (file: File, guest: GuestData) => {
  if (file && file.size <= 5 * 1024 * 1024) {
    guest.file = file
    const reader = new FileReader()
    reader.onload = (e) => {
      guest.passportPreview = {
        url: e.target?.result,
        type: file.type
      }
    }
    reader.readAsDataURL(file)
  } else {
    triggerErrorShake(guest.id)
    alert('File must be smaller than 5MB')
  }
}

const handleDrop = (e: DragEvent, guest: GuestData) => {
  dragActive.value = null
  if (e.dataTransfer?.files.length) {
    handleFile(e.dataTransfer.files[0], guest)
  }
}

const handleFileSelect = (e: Event, guest: GuestData) => {
  const target = e.target as HTMLInputElement
  if (target.files?.length) {
    handleFile(target.files[0], guest)
  }
}

const triggerFileInput = (id: string) => {
  document.getElementById(`file-${id}`)?.click()
}

// Expose method for parent validation
defineExpose({
  triggerErrorShake,
  validateAll: () => {
    let isValid = true
    guests.value.forEach(g => {
      if (!g.fullName || !g.nationality || !g.file || (g.id === 'guest-0' && (!g.email || !g.whatsapp))) {
        triggerErrorShake(g.id)
        expandedGuest.value = g.id // expand first error
        isValid = false
      }
    })
    return isValid
  }
})
</script>

<style scoped>
.grid-transition {
  display: grid;
  transition: grid-template-rows 400ms cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.guest-list-enter-active,
.guest-list-leave-active {
  transition: all 400ms cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.guest-list-enter-from,
.guest-list-leave-to {
  opacity: 0;
  transform: translateY(20px) scale(0.95);
}

@keyframes shake {
  10%, 90% { transform: translate3d(-1px, 0, 0); }
  20%, 80% { transform: translate3d(2px, 0, 0); }
  30%, 50%, 70% { transform: translate3d(-4px, 0, 0); }
  40%, 60% { transform: translate3d(4px, 0, 0); }
}

.shake-animation {
  animation: shake 0.6s cubic-bezier(.36,.07,.19,.97) both;
}
</style>
