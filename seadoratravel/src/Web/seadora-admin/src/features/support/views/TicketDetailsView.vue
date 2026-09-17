<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { 
  ArrowLeft, Clock, Send, CheckCircle, Mail, ShieldAlert 
} from 'lucide-vue-next'
import api from '@/services/api'
import { useToast } from '@/composables/useToast'

const router = useRouter()
const route = useRoute()
const toast = useToast()
const ticketId = route.params.id as string

const ticket = ref<any>(null)
const loading = ref(true)
const replyText = ref('')
const isSendingReply = ref(false)

interface DisplayMessage {
  id: string | number
  sender: string
  type: 'agent' | 'customer' | 'system'
  text: string
  time: string
  channel?: string
}

const messages = ref<DisplayMessage[]>([])

const fetchTicketDetails = async () => {
  loading.value = true
  try {
    const res = await api.get(`/api/support/api/tickets/${ticketId}`)
    ticket.value = res.data
    if (res.data?.messages) {
      messages.value = res.data.messages.map((m: any) => ({
        id: m.id,
        sender: m.sender,
        type: m.isFromAgent ? 'agent' : 'customer',
        text: m.body,
        time: m.sentAt ? new Date(m.sentAt).toLocaleString('en-US', {
          month: 'short',
          day: 'numeric',
          hour: '2-digit',
          minute: '2-digit'
        }) : ''
      }))
    } else {
      messages.value = []
    }
  } catch (err: any) {
    console.error('Failed to load ticket:', err)
    toast.error('Ticket not found', err.message)
    ticket.value = null
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchTicketDetails()
})

const sendReply = async () => {
  if (!replyText.value.trim() || isSendingReply.value) return

  isSendingReply.value = true
  try {
    await api.post(`/api/support/api/tickets/${ticketId}/messages`, {
      sender: 'Admin Support',
      isFromAgent: true,
      body: replyText.value.trim(),
      messageId: null
    })
    toast.success('Reply sent successfully')
    replyText.value = ''
    await fetchTicketDetails()
  } catch (err: any) {
    toast.error('Failed to send reply', err.message)
  } finally {
    isSendingReply.value = false
  }
}

const updateStatus = async (newStatus: string) => {
  try {
    await api.put(`/api/support/api/tickets/${ticketId}/status`, {
      status: newStatus
    })
    if (ticket.value) {
      ticket.value.status = newStatus
    }
    toast.success(`Ticket marked as ${newStatus}`)
  } catch (err: any) {
    toast.error('Failed to update status', err.message)
  }
}

const customerInitials = computed(() => {
  const name = ticket.value?.customerName || 'Guest'
  return name.split(' ').map((n: string) => n[0]).join('').slice(0, 2).toUpperCase()
})
</script>

<template>
  <div v-if="loading" class="h-full flex items-center justify-center py-20">
    <div class="flex flex-col items-center gap-3">
      <div class="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin"></div>
      <p class="text-sm text-text-muted">Loading ticket details...</p>
    </div>
  </div>

  <div v-else-if="!ticket" class="h-full flex flex-col items-center justify-center py-20 text-center">
    <div class="w-14 h-14 rounded-full bg-surface-sunken flex items-center justify-center text-text-muted/50 mb-3">
      <ShieldAlert class="w-6 h-6" />
    </div>
    <h2 class="text-lg font-medium text-text-main">Ticket not found</h2>
    <p class="text-sm text-text-muted mt-1 mb-4">This ticket may have been removed or does not exist.</p>
    <button @click="router.push('/support')" class="px-4 py-2 bg-primary text-white rounded-md text-sm font-medium hover:bg-primary-light transition-colors">
      Back to Service Desk
    </button>
  </div>

  <div v-else class="h-full flex flex-col xl:flex-row gap-6 animate-fade-in relative max-w-[1400px] mx-auto">
    <!-- Main Thread Area -->
    <div class="flex-1 flex flex-col bg-white rounded-xl border border-border/60 shadow-sm overflow-hidden h-[calc(100vh-8rem)] xl:h-auto">
      
      <!-- Thread Header -->
      <div class="px-6 py-4 border-b border-border/60 bg-surface-sunken flex items-center justify-between sticky top-0 z-10">
        <div class="flex items-center gap-4">
          <button @click="router.back()" class="p-2 -ml-2 text-text-muted hover:text-text-main hover:bg-black/5 rounded-md transition-colors">
            <ArrowLeft class="w-5 h-5" />
          </button>
          <div>
            <div class="flex items-center gap-3">
              <h1 class="text-xl font-medium text-text-main">{{ ticket.subject || 'Support Ticket' }}</h1>
              <span class="px-2 py-0.5 rounded-full text-xs font-medium bg-amber-100 text-amber-800 border border-amber-200">
                {{ ticket.status }}
              </span>
              <span class="inline-flex items-center px-2 py-0.5 rounded-md text-xs font-medium bg-orange-50 text-orange-600 ring-1 ring-inset ring-orange-500/20">
                {{ ticket.priority }}
              </span>
            </div>
            <p class="text-sm text-text-muted mt-1 flex items-center gap-2">
              <span class="font-mono text-xs">{{ ticketId }}</span>
              <span>•</span>
              <Clock class="w-3.5 h-3.5" />
              <span>Created {{ ticket.createdAt ? new Date(ticket.createdAt).toLocaleDateString() : 'Recently' }}</span>
            </p>
          </div>
        </div>
        
        <div class="flex items-center gap-2">
          <button 
            v-if="ticket.status !== 'Resolved'"
            @click="updateStatus('Resolved')" 
            class="px-3 py-1.5 text-sm font-medium text-primary hover:bg-primary/10 rounded-md transition-colors flex items-center gap-1.5"
          >
            <CheckCircle class="w-4 h-4" />
            Resolve
          </button>
        </div>
      </div>

      <!-- Messages Timeline -->
      <div class="flex-1 overflow-y-auto p-6 space-y-6 bg-[#f8f9fa] relative">
        <div v-if="messages.length === 0" class="text-center py-12 text-text-muted text-sm">
          No messages in this ticket thread yet.
        </div>

        <div 
          v-for="msg in messages" 
          :key="msg.id"
          class="flex flex-col animate-slide-up"
          :class="[
            msg.type === 'agent' ? 'items-end' : msg.type === 'system' ? 'items-center' : 'items-start'
          ]"
        >
          <div v-if="msg.type === 'system'" class="px-4 py-1.5 rounded-full bg-black/5 text-xs text-text-muted flex items-center gap-2 font-medium">
            <ShieldAlert class="w-3.5 h-3.5 text-orange-500" />
            {{ msg.text }}
            <span class="opacity-50 font-normal ml-1">{{ msg.time }}</span>
          </div>

          <div v-else class="max-w-[80%] md:max-w-[70%]">
            <div class="flex items-center gap-2 mb-1" :class="msg.type === 'agent' ? 'justify-end' : 'justify-start'">
              <span class="text-xs font-medium text-text-main">{{ msg.sender }}</span>
              <span class="text-[10px] text-text-muted">{{ msg.time }}</span>
              <span v-if="msg.channel" class="text-[10px] bg-black/5 px-1.5 py-0.5 rounded text-text-muted">{{ msg.channel }}</span>
            </div>
            
            <div 
              class="p-4 rounded-2xl text-sm leading-relaxed shadow-sm transition-all hover:shadow-md"
              :class="[
                msg.type === 'agent' 
                  ? 'bg-primary text-white rounded-tr-none' 
                  : 'bg-white border border-border/50 text-text-main rounded-tl-none'
              ]"
            >
              {{ msg.text }}
            </div>
          </div>
        </div>
      </div>

      <!-- Reply Composer -->
      <div class="p-4 bg-white border-t border-border/60">
        <div class="border border-border/80 focus-within:border-primary/50 focus-within:ring-1 focus-within:ring-primary/30 rounded-xl overflow-hidden transition-all bg-surface-sunken">
          <textarea 
            v-model="replyText" 
            rows="3" 
            placeholder="Write a reply..."
            class="w-full p-4 bg-transparent resize-none outline-none text-sm"
          ></textarea>
          
          <div class="px-4 py-2 border-t border-border/40 bg-white flex items-center justify-end">
            <button 
              @click="sendReply"
              :disabled="!replyText.trim() || isSendingReply"
              class="inline-flex items-center gap-2 bg-primary hover:bg-primary-light disabled:opacity-50 text-text-inverse px-4 py-1.5 rounded-md transition-all shadow-sm active:scale-95 text-sm font-medium"
            >
              <Send class="w-4 h-4" />
              <span>{{ isSendingReply ? 'Sending...' : 'Send' }}</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Right Sidebar (Customer Meta) -->
    <div class="w-full xl:w-80 flex flex-col gap-4">
      <div class="bg-white rounded-xl border border-border/60 shadow-sm p-5">
        <h3 class="text-sm font-semibold uppercase tracking-wider text-text-muted mb-4 border-b border-border/40 pb-2">Customer Profile</h3>
        
        <div class="flex items-center gap-4 mb-6">
          <div class="w-12 h-12 rounded-full bg-secondary/10 flex items-center justify-center text-secondary font-medium text-lg">
            {{ customerInitials }}
          </div>
          <div>
            <div class="font-medium text-text-main">{{ ticket.customerName || 'Customer' }}</div>
          </div>
        </div>

        <div class="space-y-3 text-sm">
          <div class="flex items-center gap-3 text-text-muted">
            <Mail class="w-4 h-4" />
            <a v-if="ticket.customerEmail" :href="'mailto:' + ticket.customerEmail" class="hover:text-primary transition-colors">{{ ticket.customerEmail }}</a>
            <span v-else class="text-xs text-text-muted">No email provided</span>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl border border-border/60 shadow-sm p-5">
        <h3 class="text-sm font-semibold uppercase tracking-wider text-text-muted mb-4 border-b border-border/40 pb-2">Ticket Properties</h3>
        
        <div class="space-y-4">
          <div>
            <label class="text-xs text-text-muted mb-1 block">Status</label>
            <select 
              :value="ticket.status"
              @change="(e: any) => updateStatus(e.target.value)"
              class="w-full bg-surface-sunken border border-border/80 rounded px-2 py-1.5 text-sm outline-none"
            >
              <option value="Open">Open</option>
              <option value="InProgress">In Progress</option>
              <option value="Waiting">Waiting</option>
              <option value="Resolved">Resolved</option>
            </select>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.animate-slide-up {
  animation: slideUp 0.3s ease-out forwards;
  opacity: 0;
  transform: translateY(10px);
}

.animate-slide-up:nth-child(1) { animation-delay: 0.05s; }
.animate-slide-up:nth-child(2) { animation-delay: 0.1s; }
.animate-slide-up:nth-child(3) { animation-delay: 0.15s; }
.animate-slide-up:nth-child(4) { animation-delay: 0.2s; }

@keyframes slideUp {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
