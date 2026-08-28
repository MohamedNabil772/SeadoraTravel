<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { 
  ArrowLeft, Clock, MoreVertical, Send, Paperclip, CheckCircle, User, 
  Mail, Phone, Calendar, ShieldAlert 
} from 'lucide-vue-next'

const router = useRouter()
const route = useRoute()
const ticketId = route.params.id as string

const replyText = ref('')

const messages = ref([
  { id: 1, sender: 'Eleanor Vance', type: 'customer', text: 'Hello, I need to request a refund for my yacht tour that was cancelled last week due to the storm.', time: 'Oct 24, 10:30 AM', channel: 'Email' },
  { id: 2, sender: 'System', type: 'system', text: 'Ticket priority upgraded to High by SLA engine.', time: 'Oct 24, 10:35 AM' },
  { id: 3, sender: 'Alex (Support)', type: 'agent', text: 'Hi Eleanor, I apologize for the inconvenience. I will process your refund immediately. Could you please confirm the last 4 digits of the card you used?', time: 'Oct 24, 11:15 AM' },
  { id: 4, sender: 'Eleanor Vance', type: 'customer', text: 'Sure, it is 4092.', time: 'Oct 24, 11:45 AM', channel: 'Email' },
])
</script>

<template>
  <div class="h-full flex flex-col xl:flex-row gap-6 animate-fade-in relative max-w-[1400px] mx-auto">
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
              <h1 class="text-xl font-medium text-text-main">Refund request for cancelled Yacht tour</h1>
              <span class="px-2 py-0.5 rounded-full text-xs font-medium bg-amber-100 text-amber-800 border border-amber-200">Open</span>
              <span class="inline-flex items-center px-2 py-0.5 rounded-md text-xs font-medium bg-orange-50 text-orange-600 ring-1 ring-inset ring-orange-500/20">High</span>
            </div>
            <p class="text-sm text-text-muted mt-1 flex items-center gap-2">
              <span class="font-mono text-xs">{{ ticketId }}</span>
              <span>•</span>
              <Clock class="w-3.5 h-3.5" />
              <span>Created Oct 24</span>
              <span class="text-red-500 font-medium ml-2">2h left on SLA</span>
            </p>
          </div>
        </div>
        
        <div class="flex items-center gap-2">
          <button class="px-3 py-1.5 text-sm font-medium text-primary hover:bg-primary/10 rounded-md transition-colors flex items-center gap-1.5">
            <CheckCircle class="w-4 h-4" />
            Resolve
          </button>
          <button class="p-2 text-text-muted hover:text-text-main hover:bg-black/5 rounded-md transition-colors">
            <MoreVertical class="w-5 h-5" />
          </button>
        </div>
      </div>

      <!-- Messages Timeline -->
      <div class="flex-1 overflow-y-auto p-6 space-y-6 bg-[#f8f9fa] relative">
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

      <!-- Luxury Reply Composer -->
      <div class="p-4 bg-white border-t border-border/60">
        <div class="border border-border/80 focus-within:border-primary/50 focus-within:ring-1 focus-within:ring-primary/30 rounded-xl overflow-hidden transition-all bg-surface-sunken">
          <textarea 
            v-model="replyText" 
            rows="3" 
            placeholder="Write a reply..."
            class="w-full p-4 bg-transparent resize-none outline-none text-sm"
          ></textarea>
          
          <div class="px-4 py-2 border-t border-border/40 bg-white flex items-center justify-between">
            <div class="flex gap-1">
              <button class="p-2 text-text-muted hover:text-primary hover:bg-primary/5 rounded-md transition-colors" title="Attach file">
                <Paperclip class="w-4 h-4" />
              </button>
            </div>
            <div class="flex items-center gap-2">
              <select class="text-xs bg-transparent border-none text-text-muted cursor-pointer outline-none">
                <option>Reply as Email</option>
                <option>Internal Note</option>
              </select>
              <button class="inline-flex items-center gap-2 bg-primary hover:bg-primary-light text-text-inverse px-4 py-1.5 rounded-md transition-all shadow-sm active:scale-95 text-sm font-medium">
                <Send class="w-4 h-4" />
                Send
              </button>
            </div>
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
            EV
          </div>
          <div>
            <div class="font-medium text-text-main">Eleanor Vance</div>
            <div class="text-xs text-secondary font-medium">VIP Tier: Gold</div>
          </div>
        </div>

        <div class="space-y-3 text-sm">
          <div class="flex items-center gap-3 text-text-muted">
            <Mail class="w-4 h-4" />
            <a href="#" class="hover:text-primary transition-colors">eleanor.v@example.com</a>
          </div>
          <div class="flex items-center gap-3 text-text-muted">
            <Phone class="w-4 h-4" />
            <span>+1 (555) 123-4567</span>
          </div>
          <div class="flex items-center gap-3 text-text-muted">
            <User class="w-4 h-4" />
            <span>Member since 2024</span>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl border border-border/60 shadow-sm p-5">
        <h3 class="text-sm font-semibold uppercase tracking-wider text-text-muted mb-4 border-b border-border/40 pb-2">Related Booking</h3>
        
        <div class="group cursor-pointer">
          <div class="flex items-center justify-between mb-1">
            <span class="font-medium text-primary group-hover:underline">BKG-8892</span>
            <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-red-100 text-red-800">Cancelled</span>
          </div>
          <div class="text-sm font-medium text-text-main mb-1">Luxury Red Sea Yacht Tour</div>
          <div class="flex items-center gap-2 text-xs text-text-muted">
            <Calendar class="w-3.5 h-3.5" />
            <span>Oct 20 - Oct 22, 2024</span>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl border border-border/60 shadow-sm p-5">
        <h3 class="text-sm font-semibold uppercase tracking-wider text-text-muted mb-4 border-b border-border/40 pb-2">Ticket Properties</h3>
        
        <div class="space-y-4">
          <div>
            <label class="text-xs text-text-muted mb-1 block">Assignee</label>
            <select class="w-full bg-surface-sunken border border-border/80 rounded px-2 py-1.5 text-sm outline-none">
              <option>Alex (Support)</option>
              <option>Sarah (Manager)</option>
              <option>Unassigned</option>
            </select>
          </div>
          <div>
            <label class="text-xs text-text-muted mb-1 block">Status</label>
            <select class="w-full bg-surface-sunken border border-border/80 rounded px-2 py-1.5 text-sm outline-none">
              <option>Open</option>
              <option>In Progress</option>
              <option>Waiting on Customer</option>
              <option>Resolved</option>
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
