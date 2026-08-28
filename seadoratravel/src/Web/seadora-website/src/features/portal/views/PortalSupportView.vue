<template>
  <div class="space-y-6">
    <!-- Header with Action Buttons -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-900">VIP Concierge & Support</h1>
        <p class="text-sm text-slate-500 mt-1">Submit custom VIP travel requests, manage inquiries, and track complaints.</p>
      </div>
      <div class="flex items-center gap-3">
        <button 
          @click="openVipModal" 
          class="px-5 py-2.5 bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold rounded-xl shadow-md hover:shadow-lg hover:shadow-[#c9a84c]/20 transition-all text-sm flex items-center gap-2"
        >
          <span>✨</span>
          <span>VIP Concierge Request</span>
        </button>
        <button 
          @click="openStandardModal" 
          class="px-4 py-2.5 bg-white border border-slate-200 text-slate-700 font-semibold rounded-xl hover:bg-slate-50 transition-colors text-sm shadow-sm"
        >
          + General Inquiry
        </button>
      </div>
    </div>

    <!-- VIP Concierge Highlight Banner -->
    <div class="bg-gradient-to-r from-[#062d4d] to-[#0a3e68] rounded-2xl p-6 text-white shadow-md relative overflow-hidden flex flex-col md:flex-row items-center justify-between gap-6">
      <div class="relative z-10">
        <div class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full bg-[#c9a84c]/20 border border-[#c9a84c]/30 text-[#c9a84c] text-xs font-bold uppercase tracking-wider mb-2">
          <span>✦</span> 24/7 Dedicated Concierge
        </div>
        <h2 class="text-xl font-bold mb-1">Tailor-Made Luxury Experiences</h2>
        <p class="text-white/80 text-sm max-w-xl">Need a private yacht, helicopter transfer, custom Red Sea safari, or celebration arrangement? Our concierge team responds within 2 hours.</p>
      </div>
      <button 
        @click="openVipModal" 
        class="relative z-10 px-6 py-3 bg-[#c9a84c] hover:bg-[#d8b85c] text-[#062d4d] font-bold rounded-xl shadow-lg transition-all text-sm whitespace-nowrap"
      >
        ✨ VIP Concierge Request
      </button>
    </div>

    <!-- Tickets Table Card -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-sm overflow-hidden">
      <div class="p-5 border-b border-slate-100 flex items-center justify-between">
        <h3 class="font-bold text-slate-900">Your Active Requests & Tickets</h3>
        <span class="text-xs text-slate-500 font-medium">{{ tickets.length }} Total Requests</span>
      </div>

      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 text-[11px] uppercase tracking-wider text-slate-500 border-b border-slate-200/60">
              <th class="p-4 font-semibold">Reference</th>
              <th class="p-4 font-semibold">Subject / Experience</th>
              <th class="p-4 font-semibold">Category</th>
              <th class="p-4 font-semibold">Status</th>
              <th class="p-4 font-semibold">SLA</th>
              <th class="p-4 font-semibold text-right">Action</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100 text-sm">
            <tr 
              v-for="ticket in tickets" 
              :key="ticket.id" 
              @click="openTicket(ticket)" 
              class="hover:bg-slate-50/80 transition-colors cursor-pointer group"
            >
              <td class="p-4 font-mono font-bold text-[#062d4d]">
                <span class="text-[#c9a84c]">#</span>{{ ticket.id }}
              </td>
              <td class="p-4">
                <div class="font-semibold text-slate-900 group-hover:text-[#062d4d] transition-colors">{{ ticket.subject }}</div>
                <div v-if="ticket.bookingId" class="text-xs text-slate-400 mt-0.5 flex items-center gap-1">
                  <span>Linked to Booking #{{ ticket.bookingId }}</span>
                </div>
              </td>
              <td class="p-4">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-md text-xs font-semibold" :class="ticket.category.includes('VIP') ? 'bg-[#c9a84c]/10 text-[#a38030] border border-[#c9a84c]/30' : 'bg-slate-100 text-slate-700'">
                  {{ ticket.category }}
                </span>
              </td>
              <td class="p-4">
                <span class="inline-flex items-center px-2.5 py-0.5 text-xs font-bold rounded-full" :class="getStatusBadgeClass(ticket.status)">
                  {{ ticket.status }}
                </span>
              </td>
              <td class="p-4 text-xs text-slate-500 font-medium">
                {{ ticket.sla }}
              </td>
              <td class="p-4 text-right">
                <button 
                  @click.stop="openTicket(ticket)" 
                  class="px-3 py-1.5 rounded-lg text-xs font-bold text-[#062d4d] bg-slate-100 hover:bg-[#c9a84c] hover:text-[#062d4d] transition-colors"
                >
                  View Thread →
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- ─── BESPOKE VIP REQUEST MODAL ─── -->
    <div v-if="showVipModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm" @click="showVipModal = false">
      <div class="bg-white rounded-3xl w-full max-w-2xl border border-slate-200 shadow-2xl overflow-hidden flex flex-col max-h-[90vh]" @click.stop>
        <!-- Modal Header -->
        <div class="p-6 bg-[#062d4d] text-white flex justify-between items-center">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-2xl bg-[#c9a84c] text-[#062d4d] flex items-center justify-center font-bold text-lg">
              ✨
            </div>
            <div>
              <h2 class="text-xl font-bold">Send Bespoke VIP Request</h2>
              <p class="text-xs text-white/70">Our VIP Concierge will craft your custom itinerary within 2 hours.</p>
            </div>
          </div>
          <button @click="showVipModal = false" class="text-white/70 hover:text-white text-2xl font-bold leading-none">&times;</button>
        </div>

        <!-- Form Body -->
        <form @submit.prevent="submitVipRequest" class="p-6 overflow-y-auto space-y-4 text-slate-800">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Service Type *</label>
              <select v-model="vipForm.serviceType" class="w-full p-3 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" required>
                <option value="Private Yacht Charter">🚤 Private Yacht & Sea Charter</option>
                <option value="Bespoke Desert Safari">🏜️ Private Desert & Stargazing Safari</option>
                <option value="Helicopter / Jet Transfer">🚁 Helicopter / VIP Flight Transfer</option>
                <option value="Luxury Villa / Suite Booking">🏰 Exclusive Luxury Villa / Suite</option>
                <option value="Special Occasion / Proposal">💍 Anniversary, Proposal & Celebration</option>
                <option value="Custom Multi-Day Itinerary">🗺️ Custom Tailored Itinerary</option>
              </select>
            </div>
            <div>
              <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Destination *</label>
              <select v-model="vipForm.destination" class="w-full p-3 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" required>
                <option value="Hurghada & Red Sea">Hurghada & Red Sea</option>
                <option value="Luxor & Nile Valley">Luxor & Nile Valley</option>
                <option value="Cairo & Giza Pyramids">Cairo & Giza Pyramids</option>
                <option value="Sharm El-Sheikh">Sharm El-Sheikh</option>
                <option value="Aswan & Abu Simbel">Aswan & Abu Simbel</option>
                <option value="El Gouna">El Gouna</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Preferred Date *</label>
              <input type="date" v-model="vipForm.preferredDate" class="w-full p-3 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" required />
            </div>
            <div>
              <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Number of Guests</label>
              <input type="number" min="1" max="50" v-model="vipForm.guestsCount" class="w-full p-3 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" />
            </div>
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Subject / Title *</label>
            <input type="text" v-model="vipForm.subject" placeholder="e.g. Private sunset yacht with chef for 4 guests" class="w-full p-3 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none" required />
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Special Requirements & Notes</label>
            <textarea v-model="vipForm.notes" rows="3" placeholder="Tell us your preferences (dietary requests, private transfers, champagne on arrival, timing)..." class="w-full p-3 bg-slate-50 border border-slate-200 rounded-xl text-sm font-medium focus:ring-2 focus:ring-[#c9a84c] focus:outline-none resize-none"></textarea>
          </div>

          <div class="pt-2 flex justify-end gap-3 border-t border-slate-100">
            <button type="button" @click="showVipModal = false" class="px-5 py-2.5 rounded-xl border border-slate-200 text-slate-600 font-semibold text-sm hover:bg-slate-50">Cancel</button>
            <button type="submit" :disabled="isSubmitting" class="px-6 py-2.5 bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold rounded-xl shadow-md hover:shadow-lg text-sm disabled:opacity-50">
              {{ isSubmitting ? 'Submitting...' : '✨ VIP Concierge Request' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- ─── TICKET DETAILS & THREAD MODAL ─── -->
    <div v-if="selectedTicket" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm" @click="closeTicket">
      <div class="bg-white rounded-3xl w-full max-w-3xl border border-slate-200 shadow-2xl flex flex-col max-h-[90vh] overflow-hidden" @click.stop>
        <!-- Modal Header -->
        <div class="p-6 border-b border-slate-100 bg-slate-50/70 flex justify-between items-start">
          <div>
            <div class="flex items-center gap-3 mb-1.5 flex-wrap">
              <span class="font-mono text-sm font-bold text-[#c9a84c]">#{{ selectedTicket.id }}</span>
              <h2 class="text-lg font-bold text-slate-900">{{ selectedTicket.subject }}</h2>
              <span class="px-2.5 py-0.5 text-xs font-bold rounded-full" :class="getStatusBadgeClass(selectedTicket.status)">{{ selectedTicket.status }}</span>
            </div>
            <div class="flex items-center gap-4 text-xs text-slate-500">
              <span class="font-medium bg-slate-200/70 px-2 py-0.5 rounded text-slate-700">{{ selectedTicket.category }}</span>
              <span v-if="selectedTicket.bookingId" class="text-slate-600 font-medium">Booking #{{ selectedTicket.bookingId }}</span>
              <span>SLA Response: {{ selectedTicket.sla }}</span>
            </div>
          </div>
          <button @click="closeTicket" class="text-slate-400 hover:text-slate-700 text-2xl font-bold leading-none">&times;</button>
        </div>

        <!-- Thread Timeline -->
        <div class="p-6 overflow-y-auto flex-1 space-y-4 bg-[#F8FAFC]">
          <div 
            v-for="msg in selectedTicket.timeline" 
            :key="msg.id" 
            class="flex flex-col" 
            :class="msg.sender === 'customer' ? 'items-end' : 'items-start'"
          >
            <div 
              class="max-w-[82%] rounded-2xl p-4 shadow-sm" 
              :class="msg.sender === 'customer' ? 'bg-[#062d4d] text-white rounded-tr-none' : 'bg-white text-slate-800 border border-slate-200/80 rounded-tl-none'"
            >
              <div class="text-xs font-bold mb-1 opacity-70">
                {{ msg.sender === 'customer' ? 'You' : 'VIP Concierge Team' }}
              </div>
              <p class="text-sm font-medium whitespace-pre-wrap leading-relaxed">{{ msg.text }}</p>
            </div>
            <span class="text-[11px] text-slate-400 mt-1 px-1">{{ msg.timestamp }}</span>
          </div>
        </div>

        <!-- Reply Composer -->
        <div class="p-4 border-t border-slate-100 bg-white">
          <div class="flex items-end gap-3">
            <textarea 
              v-model="replyText" 
              placeholder="Type your message to the concierge team..." 
              rows="2" 
              class="flex-1 bg-slate-50 border border-slate-200 rounded-xl p-3 text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-[#c9a84c] resize-none"
            ></textarea>
            <button 
              @click="sendReply" 
              :disabled="!replyText.trim() || isSending" 
              class="px-6 py-3 bg-[#062d4d] text-white font-bold rounded-xl hover:bg-[#062d4d]/90 shadow-md disabled:opacity-50 transition-all text-sm flex items-center gap-2 whitespace-nowrap"
            >
              <span>{{ isSending ? 'Sending...' : 'Send Reply' }}</span>
              <span>💬</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAuthStore } from '@/features/auth/store/auth'

const authStore = useAuthStore()

interface TicketMessage {
  id: number
  sender: 'customer' | 'agent'
  text: string
  timestamp: string
}

interface Ticket {
  id: string
  subject: string
  category: string
  status: 'Open' | 'In Progress' | 'Resolved'
  sla: string
  bookingId?: string
  timeline: TicketMessage[]
}

const tickets = ref<Ticket[]>([
  {
    id: 'VIP-9201',
    subject: 'Private Sunset Yacht Charter with Chef',
    category: 'VIP Yacht Charter',
    status: 'In Progress',
    sla: '< 1 hour',
    bookingId: 'BK-10293',
    timeline: [
      { id: 1, sender: 'customer', text: 'We would like to book a private yacht for 6 people on October 30th with fresh seafood dining.', timestamp: 'Today, 09:15 AM' },
      { id: 2, sender: 'agent', text: 'Good day! We have reserved our 55ft Majesty Luxury Yacht for you. Our executive chef has prepared a curated Red Sea menu. Would you prefer 4 PM or 5 PM departure?', timestamp: 'Today, 09:30 AM' }
    ]
  },
  {
    id: 'VIP-8943',
    subject: 'Special Anniversary Champagne & Flowers',
    category: 'VIP Celebration',
    status: 'Resolved',
    sla: 'Completed',
    bookingId: 'BK-10293',
    timeline: [
      { id: 1, sender: 'customer', text: 'Please arrange a bouquet of white orchids and chilled champagne in our suite.', timestamp: '2 days ago' },
      { id: 2, sender: 'agent', text: 'Delighted to confirm this has been scheduled with the hotel management.', timestamp: '2 days ago' }
    ]
  }
])

const selectedTicket = ref<Ticket | null>(null)
const showVipModal = ref(false)
const isSubmitting = ref(false)
const replyText = ref('')
const isSending = ref(false)

const vipForm = ref({
  serviceType: 'Private Yacht Charter',
  destination: 'Hurghada & Red Sea',
  preferredDate: new Date().toISOString().split('T')[0],
  guestsCount: 2,
  subject: '',
  notes: ''
})

const openVipModal = () => {
  vipForm.value.subject = `VIP ${vipForm.value.serviceType} in ${vipForm.value.destination}`
  showVipModal.value = true
}

const openStandardModal = () => {
  vipForm.value.serviceType = 'Custom Multi-Day Itinerary'
  vipForm.value.subject = 'General Support Inquiry'
  showVipModal.value = true
}

const submitVipRequest = async () => {
  isSubmitting.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    await fetch(`${API_URL}/api/support/api/tickets/customer`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${authStore.token}`
      },
      body: JSON.stringify({
        subject: `[VIP] ${vipForm.value.subject}`,
        description: `Service: ${vipForm.value.serviceType} | Destination: ${vipForm.value.destination} | Date: ${vipForm.value.preferredDate} | Guests: ${vipForm.value.guestsCount}\n\nNotes: ${vipForm.value.notes}`,
        category: vipForm.value.serviceType,
        priority: 'High'
      })
    }).catch(() => null)

    const newTicketId = `VIP-${Math.floor(1000 + Math.random() * 9000)}`
    tickets.value.unshift({
      id: newTicketId,
      subject: vipForm.value.subject,
      category: `VIP ${vipForm.value.serviceType}`,
      status: 'Open',
      sla: '< 2 hours',
      timeline: [
        {
          id: Date.now(),
          sender: 'customer',
          text: `Bespoke Request: ${vipForm.value.serviceType} in ${vipForm.value.destination} for ${vipForm.value.guestsCount} guests on ${vipForm.value.preferredDate}.\nNotes: ${vipForm.value.notes}`,
          timestamp: 'Just now'
        }
      ]
    })

    showVipModal.value = false
    alert('Your VIP Bespoke Request has been sent to our dedicated concierge team!')
  } finally {
    isSubmitting.value = false
  }
}

const openTicket = (ticket: Ticket) => {
  selectedTicket.value = ticket
}

const closeTicket = () => {
  selectedTicket.value = null
  replyText.value = ''
}

const getStatusBadgeClass = (status: string) => {
  switch (status) {
    case 'Open': return 'bg-amber-100 text-amber-800 border border-amber-200'
    case 'In Progress': return 'bg-blue-100 text-blue-800 border border-blue-200'
    case 'Resolved': return 'bg-emerald-100 text-emerald-800 border border-emerald-200'
    default: return 'bg-slate-100 text-slate-700'
  }
}

const sendReply = async () => {
  if (!replyText.value.trim() || !selectedTicket.value) return
  isSending.value = true
  
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    await fetch(`${API_URL}/api/support/api/tickets/customer/${selectedTicket.value.id}/reply`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${authStore.token}`
      },
      body: JSON.stringify({ message: replyText.value })
    }).catch(() => null)

    selectedTicket.value.timeline.push({
      id: Date.now(),
      sender: 'customer',
      text: replyText.value,
      timestamp: 'Just now'
    })
    replyText.value = ''
  } finally {
    isSending.value = false
  }
}
</script>