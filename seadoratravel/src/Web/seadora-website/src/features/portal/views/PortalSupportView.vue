<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold">Support Tickets</h1>
      <button class="px-4 py-2 bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold rounded-xl shadow-lg shadow-[#c9a84c]/20">+ New Request</button>
    </div>
    <div class="bg-[#062d4d]/40 backdrop-blur-md rounded-2xl border border-white/10 overflow-hidden">
      <table class="w-full text-left border-collapse">
        <thead>
          <tr class="bg-white/5 text-xs uppercase tracking-wider text-white/60">
            <th class="p-4 font-medium">Ticket ID</th>
            <th class="p-4 font-medium">Subject</th>
            <th class="p-4 font-medium">Status</th>
            <th class="p-4 font-medium text-right">Action</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-white/5">
          <tr v-for="ticket in tickets" :key="ticket.id" @click="openTicket(ticket)" class="hover:bg-white/5 transition-colors cursor-pointer">
            <td class="p-4 text-sm text-[#c9a84c]">#{{ ticket.id }}</td>
            <td class="p-4 font-medium">{{ ticket.subject }}</td>
            <td class="p-4">
              <span class="px-2 py-1 text-[10px] font-bold rounded" :class="getStatusClass(ticket.status)">{{ ticket.status.toUpperCase() }}</span>
            </td>
            <td class="p-4 text-right">
              <button class="text-xs text-white/50 hover:text-white" @click.stop="openTicket(ticket)">View Thread</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Ticket Details Modal -->
    <div v-if="selectedTicket" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm" @click="closeTicket">
      <div class="bg-[#062d4d] rounded-2xl w-full max-w-3xl border border-white/10 flex flex-col max-h-[90vh] shadow-2xl" @click.stop>
        <!-- Modal Header -->
        <div class="p-6 border-b border-white/10 flex justify-between items-start">
          <div>
            <div class="flex items-center gap-3 mb-2">
              <h2 class="text-xl font-bold">#{{ selectedTicket.id }}: {{ selectedTicket.subject }}</h2>
              <span class="px-2 py-1 text-[10px] font-bold rounded bg-white/10 text-white/80">{{ selectedTicket.category }}</span>
              <span class="px-2 py-1 text-[10px] font-bold rounded" :class="getStatusClass(selectedTicket.status)">{{ selectedTicket.status.toUpperCase() }}</span>
            </div>
            <div class="flex items-center gap-4 text-sm text-white/60">
              <span v-if="selectedTicket.bookingId" class="flex items-center gap-1">
                <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1" /></svg>
                Booking #{{ selectedTicket.bookingId }}
              </span>
              <span class="flex items-center gap-1">
                <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                SLA: {{ selectedTicket.sla }}
              </span>
            </div>
          </div>
          <button @click="closeTicket" class="text-white/50 hover:text-white">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>

        <!-- Timeline -->
        <div class="p-6 overflow-y-auto flex-1 space-y-6 bg-black/20">
          <div v-for="msg in selectedTicket.timeline" :key="msg.id" class="flex flex-col" :class="msg.sender === 'customer' ? 'items-end' : 'items-start'">
            <div class="max-w-[80%] rounded-2xl p-4" :class="msg.sender === 'customer' ? 'bg-[#c9a84c] text-[#062d4d] rounded-tr-none' : 'bg-white/10 text-white rounded-tl-none'">
              <p class="text-sm font-medium">{{ msg.text }}</p>
            </div>
            <span class="text-xs text-white/40 mt-1">{{ msg.sender === 'customer' ? 'You' : 'Support Agent' }} &bull; {{ msg.timestamp }}</span>
          </div>
        </div>

        <!-- Reply Composer -->
        <div class="p-4 border-t border-white/10 bg-[#062d4d]">
          <div class="flex items-end gap-3">
            <textarea v-model="replyText" placeholder="Type your reply here..." rows="2" class="flex-1 bg-black/30 border border-white/10 rounded-xl p-3 text-white placeholder-white/40 focus:outline-none focus:ring-2 focus:ring-[#c9a84c]/50 resize-none"></textarea>
            <button @click="sendReply" :disabled="!replyText.trim() || isSending" class="px-6 py-3 bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold rounded-xl shadow-lg shadow-[#c9a84c]/20 disabled:opacity-50 transition-opacity flex items-center gap-2">
              <span v-if="isSending">Sending...</span>
              <template v-else>
                Send Reply
                <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" /></svg>
              </template>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

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
    id: 'REQ-8943',
    subject: 'Special Anniversary Arrangement',
    category: 'Concierge',
    status: 'In Progress',
    sla: '2h',
    bookingId: 'BK-10293',
    timeline: [
      { id: 1, sender: 'customer', text: 'I would like to arrange some flowers for our anniversary.', timestamp: '2023-10-25 09:00 AM' },
      { id: 2, sender: 'agent', text: 'We would be happy to help. What kind of flowers would you prefer?', timestamp: '2023-10-25 09:15 AM' }
    ]
  },
  {
    id: 'REQ-8944',
    subject: 'Flight Upgrade Inquiry',
    category: 'Flights',
    status: 'Open',
    sla: '4h',
    timeline: [
      { id: 1, sender: 'customer', text: 'How much would it cost to upgrade to business class?', timestamp: '2023-10-26 10:00 AM' }
    ]
  }
])

const selectedTicket = ref<Ticket | null>(null)
const replyText = ref('')
const isSending = ref(false)

const openTicket = (ticket: Ticket) => {
  selectedTicket.value = ticket
}

const closeTicket = () => {
  selectedTicket.value = null
  replyText.value = ''
}

const getStatusClass = (status: string) => {
  switch (status) {
    case 'Open': return 'bg-yellow-500/20 text-yellow-300'
    case 'In Progress': return 'bg-blue-500/20 text-blue-300'
    case 'Resolved': return 'bg-green-500/20 text-green-300'
    default: return 'bg-gray-500/20 text-gray-300'
  }
}

const sendReply = async () => {
  if (!replyText.value.trim() || !selectedTicket.value) return
  
  isSending.value = true
  
  try {
    await fetch(`/api/support/tickets/customer/${selectedTicket.value.id}/reply`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message: replyText.value })
    })
    
    selectedTicket.value.timeline.push({
      id: Date.now(),
      sender: 'customer',
      text: replyText.value,
      timestamp: new Date().toLocaleString()
    })
    
    replyText.value = ''
  } catch (error) {
    console.error('Error sending reply:', error)
  } finally {
    isSending.value = false
  }
}
</script>