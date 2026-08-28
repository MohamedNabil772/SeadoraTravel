<template>
  <div class="space-y-8">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-2xl md:text-3xl font-bold text-slate-900">{{ $t('portal.support.title') }}</h1>
        <p class="text-xs md:text-sm text-slate-500 mt-1">{{ $t('portal.support.subtitle') }}</p>
      </div>
      <button 
        @click="openStandardModal" 
        class="px-4 py-2.5 bg-slate-900 hover:bg-slate-800 text-white font-bold rounded-xl shadow-sm text-xs flex items-center gap-2 active:scale-[0.97] transition-all cursor-pointer"
      >
        <span>+</span>
        <span>{{ $t('portal.support.newTicket') }}</span>
      </button>
    </div>

    <!-- DEDICATED SECTION 1: Private VIP Concierge Lounge (Single Action Button) -->
    <div class="bg-gradient-to-r from-[#062d4d] via-[#093a62] to-[#062d4d] rounded-3xl p-8 text-white shadow-xl relative overflow-hidden flex flex-col lg:flex-row items-start lg:items-center justify-between gap-8 border border-white/10">
      <div class="absolute -right-20 -bottom-20 w-80 h-80 bg-[#c9a84c]/15 rounded-full blur-3xl pointer-events-none"></div>

      <div class="relative z-10 max-w-2xl">
        <div class="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-[#c9a84c]/20 border border-[#c9a84c]/30 text-[#c9a84c] text-[11px] font-bold uppercase tracking-widest mb-3">
          <span>✦</span> {{ $t('portal.support.vipBadge') }}
        </div>
        <h2 class="text-2xl font-bold mb-2">{{ $t('portal.support.vipTitle') }}</h2>
        <p class="text-white/80 text-xs md:text-sm leading-relaxed">
          {{ $t('portal.support.vipDesc') }}
        </p>
      </div>

      <!-- ONE Single Prominent VIP Request Button -->
      <button 
        @click="openVipModal" 
        class="relative z-10 px-7 py-3.5 bg-gradient-to-r from-[#c9a84c] to-[#d8b85c] text-[#062d4d] font-bold rounded-2xl shadow-lg active:scale-[0.97] transition-all text-xs uppercase tracking-wider whitespace-nowrap cursor-pointer"
      >
        ✨ {{ $t('portal.support.vipBtn') }}
      </button>
    </div>

    <!-- DEDICATED SECTION 2: General Inquiries & Categorized Ticket Channels -->
    <div class="bg-white rounded-3xl border border-slate-200/80 p-8 shadow-[0_8px_30px_rgb(0,0,0,0.04)] space-y-6">
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 border-b border-slate-100 pb-5">
        <div>
          <h2 class="text-lg font-bold text-slate-900">{{ $t('portal.support.inquiriesTitle') }}</h2>
          <p class="text-xs text-slate-500 mt-0.5">{{ $t('portal.support.inquiriesSubtitle') }}</p>
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
          <h3 class="text-base font-bold text-slate-900">{{ $t('portal.support.conversationsTitle') }}</h3>
          <p class="text-xs text-slate-500">{{ $t('portal.support.conversationsSubtitle') }}</p>
        </div>
        <span class="text-xs font-bold text-[#062d4d] bg-slate-100 px-3 py-1 rounded-full">{{ tickets.length }} {{ $t('portal.support.activeRecords') }}</span>
      </div>

      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50/80 text-[11px] uppercase tracking-wider text-slate-500 border-b border-slate-200/60">
              <th class="p-4 font-bold">{{ $t('portal.support.table.ref') }}</th>
              <th class="p-4 font-bold">{{ $t('portal.support.table.subject') }}</th>
              <th class="p-4 font-bold">{{ $t('portal.support.table.category') }}</th>
              <th class="p-4 font-bold">{{ $t('portal.support.table.status') }}</th>
              <th class="p-4 font-bold">{{ $t('portal.support.table.responseTime') }}</th>
              <th class="p-4 font-bold text-right">{{ $t('portal.support.table.action') }}</th>
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
                  {{ $t('portal.support.threadModal.linkedBooking') }}{{ ticket.bookingId }}
                </div>
              </td>
              <td class="p-4">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-[11px] font-bold" 
                  :class="ticket.categoryKey === 'vip' ? 'bg-[#c9a84c]/15 text-[#a38030] border border-[#c9a84c]/30' : 'bg-slate-100 text-slate-700'"
                >
                  {{ getLocalizedCategory(ticket) }}
                </span>
              </td>
              <td class="p-4">
                <span class="inline-flex items-center px-2.5 py-0.5 text-[11px] font-bold rounded-full" :class="getStatusBadgeClass(ticket.statusKey)">
                  {{ getLocalizedStatus(ticket.statusKey) }}
                </span>
              </td>
              <td class="p-4 text-xs text-slate-500 font-medium">
                {{ ticket.sla }}
              </td>
              <td class="p-4 text-right">
                <button 
                  @click.stop="openTicket(ticket)" 
                  class="px-3.5 py-1.5 rounded-xl text-xs font-bold text-[#062d4d] bg-slate-100 hover:bg-[#062d4d] hover:text-white active:scale-[0.95] transition-all cursor-pointer"
                >
                  {{ $t('portal.support.openThread') }} →
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
            <h3 class="text-xl font-bold text-slate-900">{{ $t('portal.support.vipModal.title') }}</h3>
          </div>
          <button @click="showVipModal = false" class="text-slate-400 hover:text-slate-600 font-bold text-lg cursor-pointer">✕</button>
        </div>

        <form @submit.prevent="submitVipRequest" class="space-y-4">
          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">{{ $t('portal.support.vipModal.serviceType') }}</label>
            <select v-model="vipForm.serviceType" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-sm font-medium cursor-pointer">
              <option value="Private Luxury Yacht Charter">{{ $t('portal.support.vipModal.optionYacht') }}</option>
              <option value="Helicopter / Private Aviation Transfer">{{ $t('portal.support.vipModal.optionAviation') }}</option>
              <option value="Custom Nile Dahabiya Itinerary">{{ $t('portal.support.vipModal.optionDahabiya') }}</option>
              <option value="VIP Desert Safari & Stargazing Majlis">{{ $t('portal.support.vipModal.optionSafari') }}</option>
              <option value="Private Villa & Security Detail">{{ $t('portal.support.vipModal.optionVilla') }}</option>
            </select>
          </div>

          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">{{ $t('portal.support.vipModal.preferredDate') }}</label>
              <input type="date" v-model="vipForm.date" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2.5 text-sm font-medium" />
            </div>
            <div>
              <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">{{ $t('portal.support.vipModal.guestsCount') }}</label>
              <input type="number" v-model="vipForm.guests" min="1" max="25" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2.5 text-sm font-medium" />
            </div>
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">{{ $t('portal.support.vipModal.notes') }}</label>
            <textarea v-model="vipForm.notes" rows="4" :placeholder="$t('portal.support.vipModal.notesPlaceholder')" class="w-full bg-slate-50 border border-slate-200 rounded-xl p-3.5 text-sm font-medium"></textarea>
          </div>

          <div class="pt-4 flex justify-end gap-3">
            <button type="button" @click="showVipModal = false" class="px-5 py-2.5 bg-slate-100 hover:bg-slate-200 text-slate-600 font-bold rounded-xl text-xs cursor-pointer">{{ $t('portal.support.vipModal.cancel') }}</button>
            <button type="submit" class="px-6 py-2.5 bg-[#c9a84c] hover:bg-[#d8b85c] text-[#062d4d] font-bold rounded-xl text-xs shadow-md cursor-pointer">{{ $t('portal.support.vipModal.submit') }}</button>
          </div>
        </form>
      </div>
    </div>

    <!-- GENERAL INQUIRY / STANDARD TICKET MODAL -->
    <div v-if="showStandardModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm" @click.self="showStandardModal = false">
      <div class="bg-white rounded-3xl max-w-xl w-full p-8 shadow-2xl relative border border-slate-200">
        <div class="flex justify-between items-center mb-6">
          <h3 class="text-xl font-bold text-slate-900">{{ $t('portal.support.ticketModal.title') }}</h3>
          <button @click="showStandardModal = false" class="text-slate-400 hover:text-slate-600 font-bold text-lg cursor-pointer">✕</button>
        </div>

        <form @submit.prevent="submitStandardTicket" class="space-y-4">
          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">{{ $t('portal.support.ticketModal.category') }}</label>
            <select v-model="ticketForm.categoryKey" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-sm font-medium cursor-pointer">
              <option value="payment">{{ $t('portal.categories.payment') }}</option>
              <option value="refund">{{ $t('portal.categories.refund') }}</option>
              <option value="modification">{{ $t('portal.categories.modification') }}</option>
              <option value="dietary">{{ $t('portal.categories.dietary') }}</option>
              <option value="general">{{ $t('portal.categories.general') }}</option>
              <option value="other">{{ $t('portal.categories.other') }}</option>
            </select>
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">{{ $t('portal.support.ticketModal.subject') }}</label>
            <input type="text" v-model="ticketForm.subject" :placeholder="$t('portal.support.ticketModal.subjectPlaceholder')" required class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-sm font-medium" />
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">{{ $t('portal.support.ticketModal.bookingId') }}</label>
            <input type="text" v-model="ticketForm.bookingId" :placeholder="$t('portal.support.ticketModal.bookingIdPlaceholder')" class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-sm font-medium" />
          </div>

          <div>
            <label class="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">{{ $t('portal.support.ticketModal.description') }}</label>
            <textarea v-model="ticketForm.description" rows="4" :placeholder="$t('portal.support.ticketModal.descriptionPlaceholder')" required class="w-full bg-slate-50 border border-slate-200 rounded-xl p-3.5 text-sm font-medium"></textarea>
          </div>

          <div class="pt-4 flex justify-end gap-3">
            <button type="button" @click="showStandardModal = false" class="px-5 py-2.5 bg-slate-100 hover:bg-slate-200 text-slate-600 font-bold rounded-xl text-xs cursor-pointer">{{ $t('portal.support.ticketModal.cancel') }}</button>
            <button type="submit" class="px-6 py-2.5 bg-[#062d4d] hover:bg-[#093a62] text-white font-bold rounded-xl text-xs shadow-md cursor-pointer">{{ $t('portal.support.ticketModal.submit') }}</button>
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
              <span class="text-[10px] font-bold px-2 py-0.5 rounded-full" :class="getStatusBadgeClass(activeTicket.statusKey)">{{ getLocalizedStatus(activeTicket.statusKey) }}</span>
            </div>
            <h3 class="text-lg font-bold text-slate-900 mt-1">{{ activeTicket.subject }}</h3>
          </div>
          <button @click="activeTicket = null" class="text-slate-400 hover:text-slate-600 font-bold text-lg cursor-pointer">✕</button>
        </div>

        <div class="flex-1 overflow-y-auto space-y-4 p-2">
          <div v-for="(msg, idx) in activeTicket.messages" :key="idx" class="p-4 rounded-2xl" :class="msg.isCustomer ? 'bg-[#062d4d]/5 ml-8 border border-[#062d4d]/10' : 'bg-amber-50 mr-8 border border-amber-200/60'">
            <div class="flex justify-between text-[11px] font-bold mb-1" :class="msg.isCustomer ? 'text-[#062d4d]' : 'text-amber-800'">
              <span>{{ msg.isCustomer ? $t('portal.support.threadModal.youLabel') : msg.author }}</span>
              <span class="text-slate-400 font-normal">{{ msg.time }}</span>
            </div>
            <p class="text-xs text-slate-700 leading-relaxed">{{ msg.text }}</p>
          </div>
        </div>

        <div class="pt-4 border-t border-slate-100 flex gap-2">
          <input type="text" v-model="replyText" :placeholder="$t('portal.support.threadModal.typePlaceholder')" class="flex-1 bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-xs font-medium focus:ring-1 focus:ring-[#062d4d] focus:outline-none" @keydown.enter="sendReply" />
          <button @click="sendReply" class="px-5 py-2.5 bg-[#062d4d] text-white font-bold rounded-xl text-xs shadow-sm hover:bg-[#093a62] active:scale-[0.97] transition-all cursor-pointer">{{ $t('portal.support.threadModal.replyBtn') }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const showVipModal = ref(false);
const showStandardModal = ref(false);
const activeTicket = ref<any>(null);
const replyText = ref('');

const standardCategories = computed(() => [
  { key: 'payment', name: t('portal.categories.payment'), icon: '💳' },
  { key: 'refund', name: t('portal.categories.refund'), icon: '🔄' },
  { key: 'modification', name: t('portal.categories.modification'), icon: '📅' },
  { key: 'dietary', name: t('portal.categories.dietary'), icon: '🥗' },
  { key: 'general', name: t('portal.categories.general'), icon: 'ℹ️' },
  { key: 'other', name: t('portal.categories.other'), icon: '💬' }
]);

const tickets = ref([
  {
    id: 'TKT-8921',
    subject: 'Private Felucca Sunset Sailing & Stargazing Majlis Setup',
    categoryKey: 'vip',
    categoryCustom: 'VIP Concierge',
    statusKey: 'inProgress',
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
    categoryKey: 'dietary',
    categoryCustom: '',
    statusKey: 'resolved',
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
  categoryKey: 'general',
  subject: '',
  bookingId: '',
  description: ''
});

const openVipModal = () => {
  showVipModal.value = true;
};

const openStandardModal = () => {
  ticketForm.value.categoryKey = 'general';
  showStandardModal.value = true;
};

const openStandardModalWithCategory = (catName: string) => {
  const found = standardCategories.value.find(c => c.name === catName);
  ticketForm.value.categoryKey = found ? found.key : 'general';
  showStandardModal.value = true;
};

const submitVipRequest = () => {
  tickets.value.unshift({
    id: `TKT-${Math.floor(1000 + Math.random() * 9000)}`,
    subject: vipForm.value.serviceType,
    categoryKey: 'vip',
    categoryCustom: 'VIP Concierge',
    statusKey: 'open',
    sla: '< 2 hrs',
    bookingId: 'BK-10293',
    messages: [
      { author: 'You (VIP Guest)', time: t('portal.support.threadModal.justNow'), isCustomer: true, text: vipForm.value.notes || 'VIP Request submitted.' }
    ]
  });
  showVipModal.value = false;
  alert(t('portal.support.vipModal.dispatchedAlert'));
};

const submitStandardTicket = () => {
  tickets.value.unshift({
    id: `TKT-${Math.floor(1000 + Math.random() * 9000)}`,
    subject: ticketForm.value.subject,
    categoryKey: ticketForm.value.categoryKey,
    categoryCustom: '',
    statusKey: 'open',
    sla: '< 4 hrs',
    bookingId: ticketForm.value.bookingId || '',
    messages: [
      { author: 'You (VIP Guest)', time: t('portal.support.threadModal.justNow'), isCustomer: true, text: ticketForm.value.description }
    ]
  });
  showStandardModal.value = false;
  alert(t('portal.support.ticketModal.submittedAlert') + ticketForm.value.subject);
};

const openTicket = (tkt: any) => {
  activeTicket.value = tkt;
};

const sendReply = () => {
  if (!replyText.value.trim() || !activeTicket.value) return;
  activeTicket.value.messages.push({
    author: 'You (VIP Guest)',
    time: t('portal.support.threadModal.justNow'),
    isCustomer: true,
    text: replyText.value.trim()
  });
  replyText.value = '';
};

const getLocalizedCategory = (ticket: any) => {
  if (ticket.categoryKey === 'vip') return t('portal.dropdown.vipElite');
  if (ticket.categoryKey && t(`portal.categories.${ticket.categoryKey}`)) {
    return t(`portal.categories.${ticket.categoryKey}`);
  }
  return ticket.categoryCustom || t('portal.categories.other');
};

const getLocalizedStatus = (statusKey: string) => {
  switch (statusKey) {
    case 'resolved': return t('portal.support.status.resolved');
    case 'inProgress': return t('portal.support.status.inProgress');
    case 'waiting': return t('portal.support.status.waiting');
    case 'closed': return t('portal.support.status.closed');
    default: return t('portal.support.status.open');
  }
};

const getStatusBadgeClass = (statusKey: string) => {
  switch (statusKey) {
    case 'resolved': return 'bg-emerald-100 text-emerald-800 border border-emerald-200';
    case 'inProgress': return 'bg-amber-100 text-amber-800 border border-amber-200';
    default: return 'bg-blue-100 text-blue-800 border border-blue-200';
  }
};
</script>