<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { X, Calendar, Clock, User, Compass, Sparkles } from 'lucide-vue-next'
import { useToast } from '@/composables/useToast'
import api from '@/services/api'

const props = defineProps<{
  isOpen: boolean
}>()

const emit = defineEmits(['close', 'booking-created'])

const toast = useToast()

const tours = ref<any[]>([])
const loadingTours = ref(false)
const isSubmitting = ref(false)

const form = ref({
  tourId: '',
  packageId: '',
  tourDate: new Date().toISOString().split('T')[0],
  pickupTime: '08:30',
  tripType: 'GROUP',
  guests: 2,
  customerName: '',
  customerEmail: '',
  whatsApp: '',
  hotelName: '',
  roomNumber: '',
  hotelPickup: true,
  status: 'Confirmed',
  totalPrice: 0,
  specialRequests: ''
})

const selectedTour = computed(() => {
  return tours.value.find(t => t.id === form.value.tourId)
})

const availablePackages = computed(() => {
  return selectedTour.value?.packages || []
})

// Auto calculate price
watch([() => form.value.tourId, () => form.value.packageId, () => form.value.guests], () => {
  if (!selectedTour.value) {
    form.value.totalPrice = 0
    return
  }

  let unitPrice = selectedTour.value.price || 0
  if (form.value.packageId) {
    const pkg = availablePackages.value.find((p: any) => p.id === form.value.packageId)
    if (pkg && pkg.price) unitPrice = pkg.price
  }

  form.value.totalPrice = unitPrice * (form.value.guests || 1)
})

async function fetchTours() {
  loadingTours.value = true
  try {
    const res = await api.get('/api/content/api/tours')
    tours.value = Array.isArray(res.data) ? res.data : []
    if (tours.value.length > 0 && !form.value.tourId) {
      form.value.tourId = tours.value[0].id
    }
  } catch (e) {
    console.error('Failed to load tours for booking modal', e)
  } finally {
    loadingTours.value = false
  }
}

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    fetchTours()
  }
})

async function handleSubmit() {
  if (!form.value.tourId) {
    toast.error('Validation Error', 'Please select a tour.')
    return
  }
  if (!form.value.customerName.trim() || !form.value.customerEmail.trim()) {
    toast.error('Validation Error', 'Guest name and email are required.')
    return
  }

  isSubmitting.value = true
  try {
    const payload = {
      tourId: form.value.tourId,
      packageId: form.value.packageId || null,
      tourDate: form.value.tourDate ? new Date(form.value.tourDate).toISOString() : new Date().toISOString(),
      pickupTime: form.value.pickupTime,
      tripType: form.value.tripType,
      guests: form.value.guests,
      customerName: form.value.customerName.trim(),
      customerEmail: form.value.customerEmail.trim(),
      whatsApp: form.value.whatsApp.trim(),
      hotelName: form.value.hotelName.trim(),
      roomNumber: form.value.roomNumber.trim(),
      hotelPickup: form.value.hotelPickup,
      totalPrice: form.value.totalPrice,
      language: 'en',
      guestsList: [
        {
          fullName: form.value.customerName.trim(),
          ageCategory: 'Adult',
          specialRequests: form.value.specialRequests
        }
      ]
    }

    const res = await api.post('/api/booking/api/bookings', payload)
    toast.success('VIP Booking Created', `Booking reference #${res.data?.substring(0, 8).toUpperCase() || 'NEW'} created successfully.`)
    emit('booking-created')
    emit('close')
  } catch (e: any) {
    console.error('Failed to create booking', e)
    toast.error('Booking Creation Failed', e.response?.data?.message || 'Please check the required fields and try again.')
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal-fade">
      <div v-if="isOpen" class="fixed inset-0 z-[9999] flex items-center justify-center p-4 sm:p-6 overflow-y-auto">
        <div class="fixed inset-0 bg-navy-950/60 backdrop-blur-sm transition-opacity" @click="emit('close')"></div>

        <div class="relative w-full max-w-2xl bg-white rounded-3xl shadow-2xl overflow-hidden flex flex-col my-8 border border-gray-100 animate-modal">
          <!-- Header -->
          <div class="px-6 py-5 border-b border-gray-100 flex items-center justify-between bg-gradient-to-r from-navy-950 to-navy-900 text-white">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-white/10 border border-white/20 flex items-center justify-center text-xl">
                📅
              </div>
              <div>
                <h2 class="text-xl font-serif font-bold text-white tracking-wide flex items-center gap-2">
                  <span>Create VIP Booking</span>
                  <span class="text-[10px] font-sans font-bold uppercase tracking-wider px-2 py-0.5 rounded-full bg-secondary text-navy-950">Concierge Desk</span>
                </h2>
                <p class="text-xs text-white/70 mt-0.5">Direct manual booking entry for guests, travel agents, and VIP clients.</p>
              </div>
            </div>
            <button @click="emit('close')" class="p-2 text-white/60 hover:text-white hover:bg-white/10 rounded-full transition-colors">
              <X class="w-5 h-5" />
            </button>
          </div>

          <!-- Body -->
          <form @submit.prevent="handleSubmit" class="p-6 sm:p-8 space-y-6 max-h-[75vh] overflow-y-auto">
            <!-- Experience & Tour Selection -->
            <div class="space-y-3">
              <label class="block text-xs font-bold text-gray-700 uppercase tracking-wider flex items-center gap-2">
                <Compass class="w-4 h-4 text-secondary" />
                <span>Select Tour & Experience *</span>
              </label>
              <select
                v-model="form.tourId"
                class="w-full px-4 py-2.5 text-sm bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all font-medium text-gray-900"
              >
                <option v-for="t in tours" :key="t.id" :value="t.id">
                  {{ t.emoji || '⛵' }} {{ t.names?.en || t.title }} — {{ t.price }} {{ t.currency }} ({{ t.destinationName || 'Red Sea' }})
                </option>
              </select>
            </div>

            <!-- Package Options (If Available) -->
            <div v-if="availablePackages.length > 0" class="space-y-3">
              <label class="block text-xs font-bold text-gray-700 uppercase tracking-wider flex items-center gap-2">
                <Sparkles class="w-4 h-4 text-secondary" />
                <span>Select Package Tier</span>
              </label>
              <select
                v-model="form.packageId"
                class="w-full px-4 py-2.5 text-sm bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
              >
                <option value="">Base Tour Standard Option</option>
                <option v-for="p in availablePackages" :key="p.id" :value="p.id">
                  {{ p.titles?.en || p.title }} — ${{ p.price }} ({{ p.badge || p.tier || 'Package' }})
                </option>
              </select>
            </div>

            <!-- Schedule & Capacity -->
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-gray-700 uppercase tracking-wider flex items-center gap-1.5">
                  <Calendar class="w-3.5 h-3.5 text-gray-400" /> Tour Date *
                </label>
                <input
                  v-model="form.tourDate"
                  type="date"
                  required
                  class="w-full px-3.5 py-2 text-sm bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                />
              </div>

              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-gray-700 uppercase tracking-wider flex items-center gap-1.5">
                  <Clock class="w-3.5 h-3.5 text-gray-400" /> Pickup Slot
                </label>
                <input
                  v-model="form.pickupTime"
                  type="text"
                  placeholder="e.g. 08:30 AM"
                  class="w-full px-3.5 py-2 text-sm bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                />
              </div>

              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-gray-700 uppercase tracking-wider flex items-center gap-1.5">
                  <User class="w-3.5 h-3.5 text-gray-400" /> Guests Count
                </label>
                <input
                  v-model.number="form.guests"
                  type="number"
                  min="1"
                  max="100"
                  class="w-full px-3.5 py-2 text-sm bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-center font-bold"
                />
              </div>
            </div>

            <!-- Guest Contact Details -->
            <div class="p-5 rounded-2xl bg-gray-50/80 border border-gray-200/80 space-y-4">
              <div class="text-xs font-bold text-gray-800 uppercase tracking-wider">
                Guest & Contact Information
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-xs font-medium text-gray-600 mb-1">Lead Guest Full Name *</label>
                  <input
                    v-model="form.customerName"
                    type="text"
                    required
                    placeholder="e.g. Lord Alistair Vance"
                    class="w-full px-3.5 py-2 text-sm bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                  />
                </div>

                <div>
                  <label class="block text-xs font-medium text-gray-600 mb-1">Guest Email Address *</label>
                  <input
                    v-model="form.customerEmail"
                    type="email"
                    required
                    placeholder="guest@example.com"
                    class="w-full px-3.5 py-2 text-sm bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                  />
                </div>

                <div>
                  <label class="block text-xs font-medium text-gray-600 mb-1">WhatsApp / Phone</label>
                  <input
                    v-model="form.whatsApp"
                    type="text"
                    placeholder="+44 7700 900077"
                    class="w-full px-3.5 py-2 text-sm bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                  />
                </div>

                <div>
                  <label class="block text-xs font-medium text-gray-600 mb-1">Hotel / Resort Name</label>
                  <input
                    v-model="form.hotelName"
                    type="text"
                    placeholder="e.g. The Oberoi Sahl Hasheesh"
                    class="w-full px-3.5 py-2 text-sm bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                  />
                </div>
              </div>

              <div>
                <label class="block text-xs font-medium text-gray-600 mb-1">Room Number / Special Notes</label>
                <input
                  v-model="form.roomNumber"
                  type="text"
                  placeholder="e.g. Suite 402 • Vegetarian dining"
                  class="w-full px-3.5 py-2 text-sm bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-secondary/40 focus:border-secondary"
                />
              </div>
            </div>

            <!-- Price Summary & Submission Bar -->
            <div class="flex items-center justify-between p-4 rounded-2xl bg-gradient-to-r from-navy-900 to-navy-950 text-white border border-secondary/20 shadow-md">
              <div>
                <div class="text-[11px] text-white/70 uppercase tracking-wider">Estimated Total Value</div>
                <div class="text-2xl font-bold font-serif text-secondary mt-0.5">
                  ${{ form.totalPrice.toFixed(2) }}
                </div>
              </div>

              <div class="flex items-center gap-3">
                <button
                  type="button"
                  @click="emit('close')"
                  class="px-4 py-2 text-sm font-medium text-white/70 hover:text-white hover:bg-white/10 rounded-xl transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  :disabled="isSubmitting"
                  class="inline-flex items-center gap-2 px-6 py-2.5 bg-gradient-to-r from-secondary to-secondary-light text-navy-950 font-bold text-sm rounded-xl shadow-md hover:shadow-lg transition-all active:scale-[0.98] disabled:opacity-50"
                >
                  <div v-if="isSubmitting" class="w-4 h-4 border-2 border-navy-950 border-t-transparent rounded-full animate-spin"></div>
                  <span>{{ isSubmitting ? 'Creating Booking...' : 'Confirm Booking' }}</span>
                </button>
              </div>
            </div>
          </form>
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
