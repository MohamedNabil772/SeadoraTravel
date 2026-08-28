<template>
  <div v-if="isOpen" class="fixed inset-0 z-[200] flex items-center justify-center p-4 bg-slate-900/80 backdrop-blur-sm" @click.self="closeModal">
    <div class="relative bg-white rounded-xl shadow-2xl w-full max-w-4xl max-h-[95vh] flex flex-col overflow-hidden">
      <!-- Toolbar -->
      <div class="px-6 py-4 border-b border-slate-100 flex items-center justify-between bg-slate-50">
        <h2 class="text-lg font-bold text-[#062d4d]">Luxury Travel Voucher</h2>
        <div class="flex items-center gap-3">
          <button @click="printVoucher" class="px-4 py-2 bg-[#c9a84c] text-[#062d4d] text-sm font-bold rounded-lg shadow-sm hover:bg-[#d8b85c] transition-colors flex items-center gap-2">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z" /></svg>
            Print / Save PDF
          </button>
          <button @click="closeModal" class="text-slate-400 hover:text-slate-700 bg-white border border-slate-200 rounded-lg p-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
      </div>

      <!-- Printable Area -->
      <div id="printable-voucher" class="flex-1 overflow-y-auto bg-slate-200 p-4 md:p-8">
        <div class="max-w-[800px] mx-auto bg-white p-10 relative overflow-hidden shadow-md" style="border: 2px solid #c9a84c;">
          <!-- Corner Ornaments -->
          <div class="absolute top-0 left-0 w-8 h-8 border-t-4 border-l-4 border-[#062d4d] m-2"></div>
          <div class="absolute top-0 right-0 w-8 h-8 border-t-4 border-r-4 border-[#062d4d] m-2"></div>
          <div class="absolute bottom-0 left-0 w-8 h-8 border-b-4 border-l-4 border-[#062d4d] m-2"></div>
          <div class="absolute bottom-0 right-0 w-8 h-8 border-b-4 border-r-4 border-[#062d4d] m-2"></div>

          <!-- Watermark -->
          <div class="absolute inset-0 flex items-center justify-center opacity-[0.03] pointer-events-none">
            <svg class="w-96 h-96" viewBox="0 0 100 100" fill="currentColor"><path d="M50 0 L100 50 L50 100 L0 50 Z" /></svg>
          </div>

          <!-- Header -->
          <div class="flex items-start justify-between border-b-2 border-[#c9a84c]/20 pb-8 mb-8 relative z-10">
            <div>
              <h1 class="text-3xl font-serif font-bold text-[#062d4d] tracking-wider uppercase mb-1">SEADORA</h1>
              <div class="text-sm tracking-[0.3em] text-[#a38030] font-semibold uppercase">Luxury Travel</div>
              <div class="mt-4 text-xs text-slate-500 max-w-[200px] leading-relaxed">
                Hurghada Marina, Egypt<br>
                +20 106 894 0967<br>
                info@seadoratravel.com
              </div>
            </div>
            <div class="text-right">
              <div class="inline-block border-2 border-[#c9a84c] p-2 bg-white">
                <img :src="`https://api.qrserver.com/v1/create-qr-code/?size=100x100&data=SEADORA-${booking?.id}`" alt="Booking QR" class="w-24 h-24" />
              </div>
              <div class="mt-2 text-xs font-mono font-bold text-slate-700">{{ booking?.id }}</div>
            </div>
          </div>

          <!-- Title -->
          <div class="text-center mb-10 relative z-10">
            <h2 class="text-2xl font-serif text-[#062d4d] border-y border-[#062d4d]/10 py-3 inline-block px-12 uppercase tracking-widest font-bold">
              Official Boarding Voucher
            </h2>
          </div>

          <!-- Guest & Trip Info Grid -->
          <div class="grid grid-cols-2 gap-8 mb-10 relative z-10 text-sm">
            <div>
              <div class="text-[10px] text-slate-400 font-bold uppercase tracking-widest mb-1">Primary Guest</div>
              <div class="font-bold text-lg text-slate-900 mb-4">{{ authStore.user?.name || 'VIP Guest' }}</div>

              <div class="text-[10px] text-slate-400 font-bold uppercase tracking-widest mb-1">Journey Experience</div>
              <div class="font-semibold text-slate-800">{{ booking?.title || 'Private Dahabiya Expedition' }}</div>
            </div>
            <div class="bg-[#F8FAFC] border border-slate-100 p-4 rounded-sm">
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <div class="text-[10px] text-slate-400 font-bold uppercase tracking-widest mb-1">Embarkation</div>
                  <div class="font-bold text-slate-800">{{ booking?.date || 'Oct 30, 2026' }}</div>
                  <div class="text-xs text-slate-500">14:00 (Local Time)</div>
                </div>
                <div>
                  <div class="text-[10px] text-slate-400 font-bold uppercase tracking-widest mb-1">Disembarkation</div>
                  <div class="font-bold text-slate-800">Nov 6, 2026</div>
                  <div class="text-xs text-slate-500">10:00 (Local Time)</div>
                </div>
                <div class="col-span-2 pt-2 border-t border-slate-200">
                  <div class="text-[10px] text-slate-400 font-bold uppercase tracking-widest mb-1">Status</div>
                  <div class="font-bold text-emerald-600 uppercase tracking-widest">Confirmed & Paid</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Passenger Manifest -->
          <div class="mb-10 relative z-10">
            <h3 class="text-sm font-bold text-[#062d4d] border-b border-[#062d4d]/20 pb-2 mb-4 uppercase tracking-widest">Passenger Manifest & Allocation</h3>
            <table class="w-full text-sm text-left">
              <thead>
                <tr class="bg-slate-50 text-slate-500 text-xs uppercase tracking-wider">
                  <th class="py-2 px-3">Passenger Name</th>
                  <th class="py-2 px-3">Age Group</th>
                  <th class="py-2 px-3">Accommodation</th>
                  <th class="py-2 px-3">Dietary</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100 font-medium text-slate-800">
                <tr v-for="(p, i) in (booking?.manifest || defaultManifest)" :key="i">
                  <td class="py-3 px-3 flex items-center gap-2">
                    <svg v-if="i === 0" class="w-4 h-4 text-[#c9a84c]" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/></svg>
                    {{ p.name }}
                  </td>
                  <td class="py-3 px-3">{{ p.age }}</td>
                  <td class="py-3 px-3">{{ p.room }}</td>
                  <td class="py-3 px-3 text-slate-500">{{ p.dietary || 'None' }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Footer Legal -->
          <div class="text-[9px] text-slate-400 text-justify leading-relaxed relative z-10 border-t border-slate-100 pt-6">
            This voucher acts as your official travel document and receipt of payment. Please present this document (digital or printed) along with matching government-issued identification upon embarkation. All services are subject to Seadora Travel's Terms & Conditions. For 24/7 concierge assistance during your journey, contact +20 106 894 0967.
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/features/auth/store/auth'

const props = defineProps<{ isOpen: boolean, booking?: any }>()
const emit = defineEmits<{ (e: 'close'): void }>()
const authStore = useAuthStore()

const closeModal = () => emit('close')

const defaultManifest = [
  { name: authStore.user?.name || 'VIP Guest', age: 'Adult', room: 'Royal Suite 1 (King)', dietary: 'Vegan' },
  { name: 'Guest Two', age: 'Adult', room: 'Royal Suite 1 (King)', dietary: 'None' }
]

const printVoucher = () => {
  const element = document.getElementById('printable-voucher')
  if (!element) return

  const printWindow = window.open('', '', 'width=900,height=800')
  if (printWindow) {
    printWindow.document.write(`
      <html>
        <head>
          <title>Seadora Luxury Voucher</title>
          <script src="https://cdn.tailwindcss.com"><\/script>
          <style>
            @media print {
              body { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
              @page { size: auto; margin: 0; }
            }
          </style>
        </head>
        <body class="p-8">
          ${element.innerHTML}
          <script>
            setTimeout(() => {
              window.print();
              window.close();
            }, 500);
          <\/script>
        </body>
      </html>
    `)
    printWindow.document.close()
  }
}
</script>
