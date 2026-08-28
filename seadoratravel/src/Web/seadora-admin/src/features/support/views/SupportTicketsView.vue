<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { Plus, Search, Filter, MessageSquare, Mail, MessageCircle, MoreVertical, Clock } from 'lucide-vue-next'
import CreateTicketModal from '../components/CreateTicketModal.vue'

const router = useRouter()
const isCreateModalOpen = ref(false)
const searchQuery = ref('')
const currentFilter = ref('All')

interface Ticket {
  id: string
  subject: string
  customer: string
  status: 'Open' | 'InProgress' | 'Waiting' | 'Resolved'
  priority: 'Urgent' | 'High' | 'Med' | 'Low'
  channel: 'Email' | 'Web' | 'WhatsApp' | 'Chat'
  createdAt: string
  slaHours: number
}

const tickets = ref<Ticket[]>([
  { id: 'TKT-1042', subject: 'Refund request for cancelled Yacht tour', customer: 'Eleanor Vance', status: 'Open', priority: 'High', channel: 'Email', createdAt: '2h ago', slaHours: 2 },
  { id: 'TKT-1043', subject: 'Change booking dates - VIP Safari', customer: 'Arthur Pendelton', status: 'InProgress', priority: 'Urgent', channel: 'WhatsApp', createdAt: '1h ago', slaHours: 1 },
  { id: 'TKT-1041', subject: 'Dietary requirements for dinner', customer: 'Sophia Rossi', status: 'Waiting', priority: 'Med', channel: 'Web', createdAt: '1d ago', slaHours: 12 },
  { id: 'TKT-1039', subject: 'Lost item during transfer', customer: 'James Cooper', status: 'Resolved', priority: 'Low', channel: 'Chat', createdAt: '2d ago', slaHours: 0 },
])

const filteredTickets = computed(() => {
  let result = tickets.value
  if (currentFilter.value !== 'All') {
    result = result.filter(t => t.status === currentFilter.value)
  }
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(t => t.subject.toLowerCase().includes(q) || t.customer.toLowerCase().includes(q) || t.id.toLowerCase().includes(q))
  }
  return result
})

function navigateToTicket(id: string) {
  router.push(`/support/${id}`)
}

function getStatusColor(status: string) {
  switch (status) {
    case 'Open': return 'bg-amber-100 text-amber-800 border-amber-200'
    case 'InProgress': return 'bg-blue-100 text-blue-800 border-blue-200'
    case 'Waiting': return 'bg-purple-100 text-purple-800 border-purple-200'
    case 'Resolved': return 'bg-emerald-100 text-emerald-800 border-emerald-200'
    default: return 'bg-gray-100 text-gray-800'
  }
}

function getPriorityColor(priority: string) {
  switch (priority) {
    case 'Urgent': return 'text-red-600 bg-red-50 ring-red-500/20'
    case 'High': return 'text-orange-600 bg-orange-50 ring-orange-500/20'
    case 'Med': return 'text-blue-600 bg-blue-50 ring-blue-500/20'
    case 'Low': return 'text-gray-600 bg-gray-50 ring-gray-500/20'
    default: return 'text-gray-600 bg-gray-50'
  }
}

function getChannelIcon(channel: string) {
  switch (channel) {
    case 'Email': return Mail
    case 'Web': return Filter // Placeholder
    case 'WhatsApp': return MessageCircle
    case 'Chat': return MessageSquare
    default: return MessageSquare
  }
}
</script>

<template>
  <div class="h-full flex flex-col gap-6 animate-fade-in">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
      <div>
        <h1 class="text-2xl font-medium text-text-main font-serif tracking-tight">Service Desk</h1>
        <p class="text-sm text-text-muted mt-1">Manage customer inquiries and support tickets.</p>
      </div>
      <button 
        @click="isCreateModalOpen = true"
        class="inline-flex items-center gap-2 bg-primary hover:bg-primary-light text-text-inverse px-4 py-2 rounded-md transition-all duration-300 shadow-sm shadow-primary/20 hover:shadow-md hover:shadow-primary/30 active:scale-95"
      >
        <Plus class="w-4 h-4" />
        <span class="font-medium text-sm tracking-wide">New Ticket</span>
      </button>
    </div>

    <!-- Toolbar -->
    <div class="bg-white/60 backdrop-blur-md rounded-xl p-4 border border-border/60 shadow-sm flex flex-col md:flex-row gap-4 justify-between items-center transition-all duration-300 hover:bg-white/80">
      
      <div class="flex items-center gap-2 w-full md:w-auto">
        <div class="relative w-full md:w-72 group">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-text-muted group-focus-within:text-primary transition-colors" />
          <input 
            v-model="searchQuery"
            type="text" 
            placeholder="Search tickets, customers..." 
            class="w-full bg-surface-sunken border border-border/80 focus:border-primary/50 focus:ring-2 focus:ring-primary/20 rounded-md py-2 pl-9 pr-4 text-sm outline-none transition-all duration-300"
          >
        </div>
      </div>

      <div class="flex items-center gap-2 overflow-x-auto w-full md:w-auto pb-2 md:pb-0 no-scrollbar">
        <button 
          v-for="filter in ['All', 'Open', 'InProgress', 'Waiting', 'Resolved']" 
          :key="filter"
          @click="currentFilter = filter"
          class="px-4 py-1.5 rounded-full text-xs font-medium transition-all duration-300 whitespace-nowrap"
          :class="currentFilter === filter ? 'bg-secondary text-white shadow-sm' : 'bg-surface-sunken text-text-muted hover:bg-black/5 hover:text-text-main'"
        >
          {{ filter === 'InProgress' ? 'In Progress' : filter }}
        </button>
      </div>
    </div>

    <!-- Ticket List -->
    <div class="bg-white rounded-xl border border-border/60 shadow-sm overflow-hidden flex-1 flex flex-col">
      <div class="overflow-x-auto flex-1">
        <table class="w-full text-left text-sm whitespace-nowrap">
          <thead class="bg-surface-sunken text-text-muted text-xs uppercase tracking-wider font-medium border-b border-border/60">
            <tr>
              <th class="px-6 py-4">Ticket</th>
              <th class="px-6 py-4">Customer</th>
              <th class="px-6 py-4">Status</th>
              <th class="px-6 py-4">Priority</th>
              <th class="px-6 py-4">SLA</th>
              <th class="px-6 py-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-border/40">
            <tr 
              v-for="ticket in filteredTickets" 
              :key="ticket.id"
              @click="navigateToTicket(ticket.id)"
              class="hover:bg-surface-sunken/50 transition-colors duration-200 cursor-pointer group"
            >
              <td class="px-6 py-4">
                <div class="flex flex-col gap-1">
                  <div class="font-medium text-text-main group-hover:text-primary transition-colors flex items-center gap-2">
                    {{ ticket.subject }}
                  </div>
                  <div class="flex items-center gap-2 text-xs text-text-muted">
                    <span class="font-mono text-[10px] bg-black/5 px-1.5 py-0.5 rounded">{{ ticket.id }}</span>
                    <span>•</span>
                    <component :is="getChannelIcon(ticket.channel)" class="w-3 h-3" />
                    <span>{{ ticket.channel }}</span>
                    <span>•</span>
                    <span>{{ ticket.createdAt }}</span>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-primary font-medium text-xs">
                    {{ ticket.customer.split(' ').map(n => n[0]).join('') }}
                  </div>
                  <span class="font-medium text-text-main">{{ ticket.customer }}</span>
                </div>
              </td>
              <td class="px-6 py-4">
                <span :class="['px-2.5 py-1 rounded-full text-xs font-medium border', getStatusColor(ticket.status)]">
                  {{ ticket.status === 'InProgress' ? 'In Progress' : ticket.status }}
                </span>
              </td>
              <td class="px-6 py-4">
                <span :class="['inline-flex items-center px-2 py-1 rounded-md text-xs font-medium ring-1 ring-inset', getPriorityColor(ticket.priority)]">
                  {{ ticket.priority }}
                </span>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-1.5" :class="ticket.slaHours <= 2 && ticket.status !== 'Resolved' ? 'text-red-500 font-medium' : 'text-text-muted'">
                  <Clock class="w-3.5 h-3.5" />
                  <span class="text-xs">{{ ticket.slaHours }}h left</span>
                </div>
              </td>
              <td class="px-6 py-4 text-right">
                <button class="p-2 text-text-muted hover:text-text-main hover:bg-black/5 rounded-md transition-colors" @click.stop>
                  <MoreVertical class="w-4 h-4" />
                </button>
              </td>
            </tr>
            <tr v-if="filteredTickets.length === 0">
              <td colspan="6" class="px-6 py-12 text-center text-text-muted">
                <div class="flex flex-col items-center justify-center gap-3">
                  <div class="w-12 h-12 rounded-full bg-surface-sunken flex items-center justify-center">
                    <Search class="w-5 h-5 text-text-muted/50" />
                  </div>
                  <p>No tickets found matching your criteria.</p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Pagination -->
      <div class="p-4 border-t border-border/60 flex items-center justify-between text-sm text-text-muted bg-surface-sunken/30">
        <span>Showing {{ filteredTickets.length }} tickets</span>
        <div class="flex gap-1">
          <button class="px-3 py-1 rounded border border-border/60 hover:bg-white disabled:opacity-50" disabled>Prev</button>
          <button class="px-3 py-1 rounded border border-border/60 hover:bg-white disabled:opacity-50" disabled>Next</button>
        </div>
      </div>
    </div>
    
    <CreateTicketModal :is-open="isCreateModalOpen" @close="isCreateModalOpen = false" />
  </div>
</template>
