<template>
  <div class="space-y-8">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-2xl md:text-3xl font-bold text-slate-900">Private Concierge & Support Desk</h1>
        <p class="text-xs md:text-sm text-slate-500 mt-1">Submit custom VIP arrangements, manage payment/refund inquiries, and track support requests.</p>
      </div>
      <button 
        @click="openStandardModal" 
        class="px-4 py-2.5 bg-slate-900 hover:bg-slate-800 text-white font-bold rounded-xl shadow-sm text-xs flex items-center gap-2 active:scale-[0.97] transition-all"
      >
        <span>+</span>
        <span>New Support Ticket</span>
      </button>
    </div>

    <!-- DEDICATED SECTION 1: Private VIP Concierge Lounge (Single Action Button) -->
    <div class="bg-gradient-to-r from-[#062d4d] via-[#093a62] to-[#062d4d] rounded-3xl p-8 text-white shadow-xl relative overflow-hidden flex flex-col lg:flex-row items-start lg:items-center justify-between gap-8 border border-white/10">
      <div class="absolute -right-20 -bottom-20 w-80 h-80 bg-[#c9a84c]/15 rounded-full blur-3xl pointer-events-none"></div>

      <div class="relative z-10 max-w-2xl">
        <div class="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-[#c9a84c]/20 border border-[#c9a84c]/30 text-[#c9a84c] text-[11px] font-bold uppercase tracking-widest mb-3">
          <span>✦</span> 24/7 Red Sea & Nile Private Concierge
        </div>
        <h2 class="text-2xl font-bold mb-2">Bespoke VIP Travel & Marine Charters</h2>
        <p class="text-white/80 text-xs md:text-sm leading-relaxed">
          Require a private luxury yacht charter, helicopter transfer from Cairo to Luxor, private desert stargazing majlis, or custom milestone celebration? Our VIP directors respond within 2 hours.
        </p>
      </div>

      <!-- ONE Single Prominent VIP Request Button -->
      <button 
        @click="openVipModal" 
        class="relative z-10 px-7 py-3.5 bg-gradient-to-r from-[#c9a84c] to-[#d8b85c] text-[#062d4d] font-bold rounded-2xl shadow-lg active:scale-[0.97] transition-all text-xs uppercase tracking-wider whitespace-nowrap"
      >
        ✨ VIP Concierge Request
      </button>
    </div>

    <!-- DEDICATED SECTION 2: General Inquiries & Categorized Ticket Channels -->
    <div class="bg-white rounded-3xl border border-slate-200/80 p-8 shadow-[0_8px_30px_rgb(0,0,0,0.04)] space-y-6">
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 border-b border-slate-100 pb-5">
        <div>
          <h2 class="text-lg font-bold text-slate-900">General Inquiries & Assistance</h2>
          <p class="text-xs text-slate-500 mt-0.5">Select a category below to open a direct assistance ticket with our operations team.</p>
        </div>
      </div>

      <!-- Category Action Grid -->
      <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3.5">
        <button 
          v-for="cat in standardCategories" 
          :key="cat.name"
          @click="openStandardModalWithCategory(cat.name)"
          class="p-4 rounded-2xl border border-slate-200/80 bg-slate-50/60 hover:bg-white hover:border-[#062d4d]/30 hover:shadow-sm active:scale-[0.97] transition-all text-center flex flex-col items-center justify-center gap-2 group cursor-pointer"
        >
          <span class="text-2xl group-hover:scale-110 transition-transform">{{ cat.icon }}</span>
          <span class="text-xs font-bold text-slate-800 leading-tight">{{ cat.name }}</span>
        </button>
      </div>
    </div>

    <!-- DEDICATED SECTION 3: Active Conversations & Requests Table -->
    <div class="bg-white rounded-3xl border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] overflow-hidden">
      <div class="p-6 border-b border-slate-100 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3">
        <div>
          <h3 class="text-base font-bold text-slate-900">Your Conversations & Ticket History</h3>
          <p class="text-xs text-slate-500">Track real-time responses and communicate with your dedicated coordinator.</p>
        </div>
        <span class="text-xs font-bold text-[#062d4d] bg-slate-100 px-3 py-1 rounded-full">{{ tickets.length }} Active Records</span>
      </div>

      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50/80 text-[11px] uppercase tracking-wider text-slate-500 border-b border-slate-200/60">
              <th class="p-4 font-bold">Ref #</th>
              <th class="p-4 font-bold">Subject / Experience</th>
              <th class="p-4 font-bold">Category</th>
              <th class="p-4 font-bold">Status</th>
              <th class="p-4 font-bold">Response Time</th>
              <th class="p-4 font-bold text-right">Action</th>
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
                <div class="font-bold text-slate-900 group-hover:text-[#062d4d] transition-colors text-xs">{{ ticket.subject }}</div>
                <div v-if="ticket.bookingId" class="text-[11px] text-slate-400 mt-0.5">
                  Linked to Booking #{{ ticket.bookingId }}
                </div>
              </td>
              <td class="p-4">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-[11px] font-bold" 
                  :class="ticket.category.includes('VIP') ? 'bg-[#c9a84c]/15 text-[#a38030] border border-[#c9a84c]/30' : 'bg-slate-100 text-slate-700'"
                >
                  {{ ticket.category }}
                </span>
              </td>
              <td class="p-4">
                <span class="inline-flex items-center px-2.5 py-0.5 text-[11px] font-bold rounded-full" :class="getStatusBadgeClass(ticket.status)">
                  {{ ticket.status }}
                </span>
              </td>
              <td class="p-4 text-xs text-slate-500 font-medium">
                {{ ticket.sla }}
              </td>
              <td class="p-4 text-right">
                <button 
                  @click.stop="openTicket(ticket)" 
                  class="px-3.5 py-1.5 rounded-xl text-xs font-bold text-[#062d4d] bg-slate-100 hover:bg-[#062d4d] hover:text-white active:scale-[0.95] transition-all"
                >
                  Open Thread →
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- VIP BESPOKE REQUEST MODAL -->
    <div v-if="showVipModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm" @click.self="showVipModal = false">
      <div class="bg-white rounded-3xl max-w-xl w-full p-8 shadow-2xl relative border border-slate-200">
        <div class="flex justify-between items-center mb-6">
          <div class="flex items-center gap-2.5">
            <div class="w-8 h-8 rounded-xl bg-[#c9a84c] text-[#062d4d] flex items-center justify-center font-bold text-sm">✦</div>
            <h3 class="text-xl font-bold text-slate-900">Bespoke VIP Travel Arrangement</h3>
          </div>
          <button @click="showVipModal = false" class="text-slate-400 hover:text-slate-600 font-bold text-lg">✕</button>
        </div>

        <form @submit.prevent="submitVipRequest" class="space-y-4">
          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">VIP Service Type</label>
            <select v-model="vipForm.serviceType" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-sm font-medium">
              <option value="Private Luxury Yacht Charter">Private Luxury Yacht Charter (Red Sea)</option>
              <option value="Helicopter / Private Aviation Transfer">Helicopter / Private Aviation Transfer</option>
              <option value="Custom Nile Dahabiya Itinerary">Custom Nile Dahabiya Itinerary</option>
              <option value="VIP Desert Safari & Stargazing Majlis">VIP Desert Safari & Stargazing Majlis</option>
              <option value="Private Villa & Security Detail">Private Villa & Security Detail</option>
            </select>
          </div>

          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Preferred Date</label>
              <input type="date" v-model="vipForm.date" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2.5 text-sm font-medium" />
            </div>
            <div>
              <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Guests Count</label>
              <input type="number" v-model="vipForm.guests" min="1" max="25" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2.5 text-sm font-medium" />
            </div>
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Bespoke Details & Preferences</label>
            <textarea v-model="vipForm.notes" rows="4" placeholder="Specify catering, preferred routes, private guides, or special champagne arrangements..." class="w-full bg-slate-50 border border-slate-200 rounded-xl p-3.5 text-sm font-medium"></textarea>
          </div>

          <div class="pt-4 flex justify-end gap-3">
            <button type="button" @click="showVipModal = false" class="px-5 py-2.5 bg-slate-100 text-slate-600 font-bold rounded-xl text-xs">Cancel</button>
            <button type="submit" class="px-6 py-2.5 bg-[#c9a84c] hover:bg-[#d8b85c] text-[#062d4d] font-bold rounded-xl text-xs shadow-md">Dispatch VIP Request</button>
          </div>
        </form>
      </div>
    </div>

    <!-- GENERAL INQUIRY / STANDARD TICKET MODAL -->
    <div v-if="showStandardModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm" @click.self="showStandardModal = false">
      <div class="bg-white rounded-3xl max-w-xl w-full p-8 shadow-2xl relative border border-slate-200">
        <div class="flex justify-between items-center mb-6">
          <h3 class="text-xl font-bold text-slate-900">Submit Support Ticket</h3>
          <button @click="showStandardModal = false" class="text-slate-400 hover:text-slate-600 font-bold text-lg">✕</button>
        </div>

        <form @submit.prevent="submitStandardTicket" class="space-y-4">
          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Inquiry Category</label>
            <select v-model="ticketForm.category" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-sm font-medium">
              <option value="Payment & Invoicing">Payment & Invoicing</option>
              <option value="Cancellation & Refund">Cancellation & Refund</option>
              <option value="Booking Modification">Booking Modification</option>
              <option value="Special Dietary / Accessibility">Special Dietary / Accessibility</option>
              <option value="General Inquiry">General Inquiry</option>
              <option value="Other">Other</option>
            </select>
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Subject</label>
            <input type="text" v-model="ticketForm.subject" placeholder="e.g. Invoice copy request or dietary update" required class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-sm font-medium" />
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Linked Booking # (Optional)</label>
            <input type="text" v-model="ticketForm.bookingId" placeholder="e.g. BK-10293" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-sm font-medium" />
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">Message Description</label>
            <textarea v-model="ticketForm.description" rows="4" placeholder="Provide full details of your request..." required class="w-full bg-slate-50 border border-slate-200 rounded-xl p-3.5 text-sm font-medium"></textarea>
          </div>

          <div class="pt-4 flex justify-end gap-3">
            <button type="button" @click="showStandardModal = false" class="px-5 py-2.5 bg-slate-100 text-slate-600 font-bold rounded-xl text-xs">Cancel</button>
            <button type="submit" class="px-6 py-2.5 bg-[#062d4d] hover:bg-[#093a62] text-white font-bold rounded-xl text-xs shadow-md">Submit Ticket</button>
          </div>
        </form>
      </div>
    </div>

    <!-- TICKET DETAILS & CONVERSATION THREAD MODAL -->
    <div v-if="activeTicket" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm" @click.self="activeTicket = null">
      <div class="bg-white rounded-3xl max-w-2xl w-full p-8 shadow-2xl relative border border-slate-200 flex flex-col max-h-[85vh]">
        <div class="flex justify-between items-start border-b border-slate-100 pb-4 mb-4">
          <div>
            <div class="flex items-center gap-2">
              <span class="text-xs font-mono font-bold text-[#c9a84c]">#{{ activeTicket.id }}</span>
              <span class="text-[10px] font-bold px-2 py-0.5 rounded-full" :class="getStatusBadgeClass(activeTicket.status)">{{ activeTicket.status }}</span>
            </div>
            <h3 class="text-lg font-bold text-slate-900 mt-1">{{ activeTicket.subject }}</h3>
          </div>
          <button @click="activeTicket = null" class="text-slate-400 hover:text-slate-600 font-bold text-lg">✕</button>
        </div>

        <div class="flex-1 overflow-y-auto space-y-4 p-2">
          <div v-for="(msg, idx) in activeTicket.messages" :key="idx" class="p-4 rounded-2xl" :class="msg.isCustomer ? 'bg-[#062d4d]/5 ml-8 border border-[#062d4d]/10' : 'bg-amber-50 mr-8 border border-amber-200/60'">
            <div class="flex justify-between text-[11px] font-bold mb-1" :class="msg.isCustomer ? 'text-[#062d4d]' : 'text-amber-800'">
              <span>{{ msg.author }}</span>
              <span class="text-slate-400 font-normal">{{ msg.time }}</span>
            </div>
            <p class="text-xs text-slate-700 leading-relaxed">{{ msg.text }}</p>
          </div>
        </div>

        <div class="pt-4 border-t border-slate-100 flex gap-2">
          <input type="text" v-model="replyText" placeholder="Type a message to your coordinator..." class="flex-1 bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-xs font-medium focus:ring-1 focus:ring-[#062d4d] focus:outline-none" @keydown.enter="sendReply" />
          <button @click="sendReply" class="px-5 py-2.5 bg-[#062d4d] text-white font-bold rounded-xl text-xs shadow-sm hover:bg-[#093a62] active:scale-[0.97] transition-all">Reply</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const showVipModal = ref(false);
const showStandardModal = ref(false);
const activeTicket = ref<any>(null);
const replyText = ref('');

const standardCategories = [
  { name: 'Payment & Invoicing', icon: '💳' },
  { name: 'Cancellation & Refund', icon: '🔄' },
  { name: 'Booking Modification', icon: '📅' },
  { name: 'Special Dietary / Needs', icon: '🥗' },
  { name: 'General Inquiry', icon: 'ℹ️' },
  { name: 'Other', icon: '💬' }
];

const tickets = ref([
  {
    id: 'TKT-8921',
    subject: 'Private Felucca Sunset Sailing & Stargazing Majlis Setup',
    category: 'VIP Concierge',
    status: 'In Progress',
    sla: '< 1 hr',
    bookingId: 'BK-10293',
    messages: [
      { author: 'You (VIP Guest)', time: 'Today at 09:15', isCustomer: true, text: 'We would love to arrange a private sunset felucca sailing in Luxor with private dinner for two.' },
      { author: 'Lead Concierge Karim', time: 'Today at 09:40', isCustomer: false, text: 'Good morning! We have reserved Dahabiya felucca with a private chef and acoustic oud player for Nov 2nd.' }
    ]
  },
  {
    id: 'TKT-7612',
    subject: 'Dietary Requirement Confirmation (Gluten-Free Chef)',
    category: 'Special Dietary / Needs',
    status: 'Resolved',
    sla: 'Resolved',
    bookingId: 'BK-10293',
    messages: [
      { author: 'You (VIP Guest)', time: 'Yesterday', isCustomer: true, text: 'Please ensure gluten-free preparation is confirmed with the onboard galley team.' },
      { author: 'Concierge Operations', time: 'Yesterday', isCustomer: false, text: 'Confirmed with executive chef. A dedicated gluten-free galley prep station is designated.' }
    ]
  }
]);

const vipForm = ref({
  serviceType: 'Private Luxury Yacht Charter',
  date: '2026-11-02',
  guests: 2,
  notes: ''
});

const ticketForm = ref({
  category: 'General Inquiry',
  subject: '',
  bookingId: '',
  description: ''
});

const openVipModal = () => {
  showVipModal.value = true;
};

const openStandardModal = () => {
  ticketForm.value.category = 'General Inquiry';
  showStandardModal.value = true;
};

const openStandardModalWithCategory = (catName: string) => {
  ticketForm.value.category = catName;
  showStandardModal.value = true;
};

const submitVipRequest = () => {
  tickets.value.unshift({
    id: `TKT-${Math.floor(1000 + Math.random() * 9000)}`,
    subject: vipForm.value.serviceType,
    category: 'VIP Concierge',
    status: 'Open',
    sla: '< 2 hrs',
    bookingId: 'BK-10293',
    messages: [
      { author: 'You (VIP Guest)', time: 'Just now', isCustomer: true, text: vipForm.value.notes || 'VIP Request submitted.' }
    ]
  });
  showVipModal.value = false;
  alert('Your VIP Bespoke Concierge request has been dispatched. A director will contact you within 2 hours.');
};

const submitStandardTicket = () => {
  tickets.value.unshift({
    id: `TKT-${Math.floor(1000 + Math.random() * 9000)}`,
    subject: ticketForm.value.subject,
    category: ticketForm.value.category,
    status: 'Open',
    sla: '< 4 hrs',
    bookingId: ticketForm.value.bookingId || '',
    messages: [
      { author: 'You (VIP Guest)', time: 'Just now', isCustomer: true, text: ticketForm.value.description }
    ]
  });
  showStandardModal.value = false;
  alert('Your support ticket has been submitted. Reference: ' + ticketForm.value.subject);
};

const openTicket = (t: any) => {
  activeTicket.value = t;
};

const sendReply = () => {
  if (!replyText.value.trim() || !activeTicket.value) return;
  activeTicket.value.messages.push({
    author: 'You (VIP Guest)',
    time: 'Just now',
    isCustomer: true,
    text: replyText.value.trim()
  });
  replyText.value = '';
};

const getStatusBadgeClass = (status: string) => {
  switch (status) {
    case 'Resolved': return 'bg-emerald-100 text-emerald-800 border border-emerald-200';
    case 'In Progress': return 'bg-amber-100 text-amber-800 border border-amber-200';
    default: return 'bg-blue-100 text-blue-800 border border-blue-200';
  }
};
</script>