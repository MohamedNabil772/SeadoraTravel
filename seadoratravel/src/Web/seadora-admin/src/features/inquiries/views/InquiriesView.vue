<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  Search, 
  Filter, 
  MessageSquare, 
  Mail, 
  Phone, 
  Eye, 
  CheckCircle, 
  Clock, 
  CheckCircle2, 
  X, 
  Trash2,
  Calendar,
  Users,
  MapPin
} from 'lucide-vue-next'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'

// Mock Data for Inquiries
const inquiries = ref([
  {
    id: 'INQ-1049',
    guestName: 'Eleanor Vance',
    email: 'eleanor.v@example.com',
    phone: '+447911123456',
    destination: 'Maldives Overwater Villa',
    date: '2026-11-15',
    guests: 2,
    message: 'We are looking for a secluded overwater villa for our 10th anniversary. Interested in private dining options.',
    status: 'Pending',
    createdAt: '2026-08-19T10:30:00Z',
    notes: ''
  },
  {
    id: 'INQ-1048',
    guestName: 'Mohammed Al-Fayed',
    email: 'm.alfayed@example.ae',
    phone: '+971501234567',
    destination: 'Swiss Alps Ski Chalet',
    date: '2026-12-20',
    guests: 6,
    message: 'Need a premium chalet with a private chef and ski-in/ski-out access for the family.',
    status: 'Replied',
    createdAt: '2026-08-18T14:15:00Z',
    notes: 'Sent brochure for Zermatt properties.'
  },
  {
    id: 'INQ-1047',
    guestName: 'Sofia Rossi',
    email: 's.rossi@example.it',
    phone: '+393123456789',
    destination: 'Amalfi Coast Yacht Charter',
    date: '2026-09-05',
    guests: 4,
    message: 'Looking to charter a yacht for 3 days along the Amalfi coast. Must include crew.',
    status: 'Resolved',
    createdAt: '2026-08-15T09:45:00Z',
    notes: 'Booked. Deposit received.'
  },
  {
    id: 'INQ-1046',
    guestName: 'James Chen',
    email: 'j.chen@example.sg',
    phone: '+6591234567',
    destination: 'Kyoto, Japan',
    travelDates: 'Nov 05 - Nov 18, 2026',
    guests: 4,
    budget: '$8,500',
    status: 'In Progress',
    createdAt: '1 day ago',
    message: 'Family trip focusing on cultural experiences and traditional ryokan stays. Needs kid-friendly activities.',
    notes: 'Sent initial itinerary proposal. Waiting for feedback.'
  },
  {
    id: 'INQ-1047',
    guestName: 'Sarah Jenkins',
    email: 'sjenkins@company.com',
    phone: '+1 212-555-0144',
    destination: 'Amalfi Coast, Italy',
    travelDates: 'Sep 01 - Sep 10, 2026',
    guests: 2,
    budget: '$6,000',
    status: 'Closed',
    createdAt: '3 days ago',
    message: 'Honeymoon trip! Interested in boat tours, cooking classes, and luxury dining reservations.',
    notes: 'Successfully booked. Sent confirmation details.'
  }
])

const searchQuery = ref('')
const statusFilter = ref('All')
const statuses = ['All', 'New', 'In Progress', 'Closed']

const isDrawerOpen = ref(false)
const selectedInquiry = ref<any>(null)
const adminNotes = ref('')
const newStatus = ref('')
const isSendingReply = ref(false)
const replyMessage = ref('')

const { confirm } = useConfirm()
const toast = useToast()

const sendReply = async () => {
  if (!selectedInquiry.value || !replyMessage.value.trim()) return

  isSendingReply.value = true
  try {
    const res = await fetch(`/api/booking/api/inquiries/${selectedInquiry.value.id}/reply`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ message: replyMessage.value })
    })

    if (!res.ok) throw new Error('Failed to send reply')

    toast.success('Reply sent successfully')
    
    const index = inquiries.value.findIndex(i => i.id === selectedInquiry.value.id)
    if (index !== -1) {
      inquiries.value[index].status = 'Replied'
      newStatus.value = 'Replied'
    }
    replyMessage.value = ''
  } catch (e: any) {
    toast.error('Failed to send reply', e.message)
  } finally {
    isSendingReply.value = false
  }
}

const openDrawer = (inquiry: any) => {
  selectedInquiry.value = { ...inquiry }
  adminNotes.value = inquiry.notes
  newStatus.value = inquiry.status
  isDrawerOpen.value = true
}

const closeDrawer = () => {
  isDrawerOpen.value = false
  setTimeout(() => {
    selectedInquiry.value = null
  }, 300)
}

const saveInquiryDetails = () => {
  if (selectedInquiry.value) {
    const index = inquiries.value.findIndex(i => i.id === selectedInquiry.value.id)
    if (index !== -1) {
      inquiries.value[index].notes = adminNotes.value
      inquiries.value[index].status = newStatus.value
      toast.success('Inquiry updated successfully', `Status changed to ${newStatus.value}`)
      closeDrawer()
    }
  }
}

const deleteInquiry = async () => {
  const ok = await confirm({
    title: 'Delete Inquiry',
    message: 'Are you sure you want to delete this inquiry?',
    confirmText: 'Delete',
    type: 'danger'
  })
  if (ok) {
    inquiries.value = inquiries.value.filter(i => i.id !== selectedInquiry.value.id)
    toast.success('Inquiry deleted')
    closeDrawer()
  }
}

const quickStatusUpdate = (id: string, status: string) => {
  const index = inquiries.value.findIndex(i => i.id === id)
  if (index !== -1) {
    inquiries.value[index].status = status
    toast.success(`Status updated to ${status}`)
  }
}

const filteredInquiries = computed(() => {
  return inquiries.value.filter(inq => {
    const matchesSearch = 
      inq.guestName?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      inq.email?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      inq.destination?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      inq.phone?.includes(searchQuery.value)
    
    const matchesStatus = statusFilter.value === 'All' || inq.status === statusFilter.value

    return matchesSearch && matchesStatus
  })
})

const currentPage = ref(1)
const pageSize = ref(10)

const paginatedInquiries = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredInquiries.value.slice(start, start + pageSize.value)
})

const totalRequests = computed(() => inquiries.value.length)
const pendingRequests = computed(() => inquiries.value.filter(i => i.status === 'Pending').length)
const repliedRequests = computed(() => inquiries.value.filter(i => i.status === 'Replied').length)
const resolvedRequests = computed(() => inquiries.value.filter(i => i.status === 'Resolved').length)

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleDateString('en-US', {
    month: 'short', day: 'numeric', year: 'numeric'
  })
}

const formatDateTime = (dateStr: string) => {
  return new Date(dateStr).toLocaleString('en-US', {
    month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
  })
}

const getStatusColor = (status: string) => {
  switch(status) {
    case 'Pending': return 'bg-amber-100 text-amber-700 ring-amber-500/20'
    case 'Replied': return 'bg-blue-100 text-blue-700 ring-blue-500/20'
    case 'Resolved': return 'bg-emerald-100 text-emerald-700 ring-emerald-500/20'
    default: return 'bg-gray-100 text-gray-700 ring-gray-500/20'
  }
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header Metrics -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="bg-white rounded-xl p-5 border border-border/60 shadow-sm flex items-center justify-between">
        <div>
          <p class="text-sm font-medium text-text-muted">Total Requests</p>
          <p class="text-2xl font-bold text-primary mt-1">{{ totalRequests }}</p>
        </div>
        <div class="w-10 h-10 rounded-full bg-primary/5 flex items-center justify-center text-primary">
          <MessageSquare class="w-5 h-5" />
        </div>
      </div>
      
      <div class="bg-white rounded-xl p-5 border border-border/60 shadow-sm flex items-center justify-between">
        <div>
          <p class="text-sm font-medium text-text-muted">Pending VIP</p>
          <p class="text-2xl font-bold text-amber-600 mt-1">{{ pendingRequests }}</p>
        </div>
        <div class="w-10 h-10 rounded-full bg-amber-50 flex items-center justify-center text-amber-600">
          <Clock class="w-5 h-5" />
        </div>
      </div>

      <div class="bg-white rounded-xl p-5 border border-border/60 shadow-sm flex items-center justify-between">
        <div>
          <p class="text-sm font-medium text-text-muted">Replied</p>
          <p class="text-2xl font-bold text-blue-600 mt-1">{{ repliedRequests }}</p>
        </div>
        <div class="w-10 h-10 rounded-full bg-blue-50 flex items-center justify-center text-blue-600">
          <Mail class="w-5 h-5" />
        </div>
      </div>

      <div class="bg-white rounded-xl p-5 border border-border/60 shadow-sm flex items-center justify-between">
        <div>
          <p class="text-sm font-medium text-text-muted">Resolved</p>
          <p class="text-2xl font-bold text-emerald-600 mt-1">{{ resolvedRequests }}</p>
        </div>
        <div class="w-10 h-10 rounded-full bg-emerald-50 flex items-center justify-center text-emerald-600">
          <CheckCircle2 class="w-5 h-5" />
        </div>
      </div>
    </div>

    <!-- Toolbar -->
    <div class="bg-white rounded-xl p-4 border border-border/60 shadow-sm flex flex-col sm:flex-row gap-4 justify-between items-center">
      <div class="relative w-full sm:w-96">
        <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-text-muted" />
        <input 
          v-model="searchQuery"
          type="text" 
          placeholder="Search name, email, phone, destination..." 
          class="w-full pl-9 pr-4 py-2 text-sm bg-surface-sunken border border-border/60 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary/30 transition-all"
        >
      </div>
      
      <div class="flex items-center gap-3 w-full sm:w-auto">
        <Filter class="w-4 h-4 text-text-muted hidden sm:block" />
        <select 
          v-model="statusFilter"
          class="w-full sm:w-auto pl-3 pr-8 py-2 text-sm bg-surface-sunken border border-border/60 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary/30 transition-all"
        >
          <option v-for="status in statuses" :key="status" :value="status">{{ status }}</option>
        </select>
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-white rounded-xl border border-border/60 shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left text-sm whitespace-nowrap">
          <thead class="bg-surface-sunken border-b border-border/60 text-text-muted font-medium">
            <tr>
              <th class="px-6 py-4">Guest Info</th>
              <th class="px-6 py-4">Contact</th>
              <th class="px-6 py-4">Experience Requested</th>
              <th class="px-6 py-4">Status</th>
              <th class="px-6 py-4">Date</th>
              <th class="px-6 py-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-border/40">
            <tr 
              v-for="inquiry in paginatedInquiries" 
              :key="inquiry.id"
              class="hover:bg-black/[0.02] transition-colors group"
            >
              <td class="px-6 py-4">
                <div class="font-medium text-text-main">{{ inquiry.guestName }}</div>
                <div class="text-xs text-text-muted mt-0.5">{{ inquiry.id }}</div>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2">
                  <Mail class="w-3.5 h-3.5 text-text-muted" />
                  <a :href="'mailto:' + inquiry.email" class="text-primary hover:underline">{{ inquiry.email }}</a>
                </div>
                <div class="flex items-center gap-2 mt-1">
                  <Phone class="w-3.5 h-3.5 text-text-muted" />
                  <a v-if="inquiry.phone" :href="'https://wa.me/' + inquiry.phone.replace('+', '')" target="_blank" class="text-text-muted hover:text-[#25D366] transition-colors">
                    {{ inquiry.phone }}
                  </a>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="text-text-main truncate max-w-[200px]" :title="inquiry.destination">{{ inquiry.destination }}</div>
                <div class="text-xs text-text-muted mt-0.5">{{ formatDate(inquiry.date || '') }} • {{ inquiry.guests }} Guests</div>
              </td>
              <td class="px-6 py-4">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ring-1 ring-inset"
                  :class="getStatusColor(inquiry.status)"
                >
                  {{ inquiry.status }}
                </span>
              </td>
              <td class="px-6 py-4 text-text-muted">
                {{ formatDateTime(inquiry.createdAt) }}
              </td>
              <td class="px-6 py-4 text-right">
                <div class="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                  <button 
                    v-if="inquiry.status === 'Pending'"
                    @click="quickStatusUpdate(inquiry.id, 'Replied')"
                    class="p-1.5 text-blue-600 hover:bg-blue-50 rounded-md transition-colors tooltip-trigger"
                    title="Mark as Replied"
                  >
                    <CheckCircle class="w-4 h-4" />
                  </button>
                  <button 
                    @click="openDrawer(inquiry)"
                    class="p-1.5 text-text-main hover:bg-black/5 rounded-md transition-colors border border-border/60 bg-white"
                  >
                    <Eye class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="filteredInquiries.length === 0">
              <td colspan="6" class="px-6 py-12 text-center text-text-muted">
                No inquiries found matching your filters.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Luxury Pagination Component -->
      <LuxuryPagination
        v-if="filteredInquiries.length > 0"
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="filteredInquiries.length"
      />
    </div>

    <!-- Slide-over Drawer for Details -->
    <div v-if="isDrawerOpen" class="fixed inset-0 z-50 overflow-hidden" aria-labelledby="slide-over-title" role="dialog" aria-modal="true">
      <div class="absolute inset-0 bg-primary/20 backdrop-blur-sm transition-opacity" @click="closeDrawer"></div>
      <div class="fixed inset-y-0 right-0 pl-10 max-w-full flex">
        <div class="w-screen max-w-md transform transition-transform duration-300 ease-in-out bg-white shadow-2xl flex flex-col h-full border-l border-border/60">
          <!-- Drawer Header -->
          <div class="px-6 py-5 border-b border-border/60 bg-surface-sunken flex items-center justify-between">
            <div>
              <h2 class="text-lg font-serif font-medium text-text-main" id="slide-over-title">Inquiry Details</h2>
              <p class="text-sm text-text-muted mt-1">{{ selectedInquiry?.id }}</p>
            </div>
            <button @click="closeDrawer" class="rounded-full p-2 text-text-muted hover:bg-black/5 transition-colors focus:outline-none focus:ring-2 focus:ring-primary/20">
              <span class="sr-only">Close panel</span>
              <X class="w-5 h-5" />
            </button>
          </div>

          <!-- Drawer Content -->
          <div class="flex-1 overflow-y-auto p-6 space-y-6">
            <!-- Guest Profile -->
            <div class="bg-surface-sunken rounded-xl p-4 border border-border/60">
              <h3 class="text-xs font-bold text-text-muted uppercase tracking-wider mb-3">Guest Information</h3>
              <div class="space-y-3">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-full bg-secondary/10 text-secondary-dark flex items-center justify-center font-bold font-serif border border-secondary/20">
                    {{ selectedInquiry?.guestName.charAt(0) }}
                  </div>
                  <div>
                    <div class="font-medium text-text-main">{{ selectedInquiry?.guestName }}</div>
                  </div>
                </div>
                <div class="grid grid-cols-1 gap-2 pt-2 text-sm">
                  <a :href="'mailto:' + selectedInquiry?.email" class="flex items-center gap-2 text-primary hover:underline">
                    <Mail class="w-4 h-4 text-text-muted" /> {{ selectedInquiry?.email }}
                  </a>
                  <a :href="'https://wa.me/' + selectedInquiry?.phone.replace('+', '')" target="_blank" class="flex items-center gap-2 text-[#25D366] hover:underline font-medium">
                    <Phone class="w-4 h-4 text-[#25D366]" /> {{ selectedInquiry?.phone }} (WhatsApp)
                  </a>
                </div>
              </div>
            </div>

            <!-- Request Details -->
            <div>
              <h3 class="text-xs font-bold text-text-muted uppercase tracking-wider mb-3">Request Details</h3>
              <div class="space-y-4">
                <div class="flex items-start gap-3">
                  <MapPin class="w-4 h-4 text-secondary mt-0.5" />
                  <div>
                    <div class="text-xs text-text-muted">Destination / Experience</div>
                    <div class="font-medium text-text-main">{{ selectedInquiry?.destination }}</div>
                  </div>
                </div>
                <div class="flex items-start gap-3">
                  <Calendar class="w-4 h-4 text-secondary mt-0.5" />
                  <div>
                    <div class="text-xs text-text-muted">Requested Date</div>
                    <div class="font-medium text-text-main">{{ formatDate(selectedInquiry?.date) }}</div>
                  </div>
                </div>
                <div class="flex items-start gap-3">
                  <Users class="w-4 h-4 text-secondary mt-0.5" />
                  <div>
                    <div class="text-xs text-text-muted">Guests</div>
                    <div class="font-medium text-text-main">{{ selectedInquiry?.guests }} People</div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Message -->
            <div>
              <h3 class="text-xs font-bold text-text-muted uppercase tracking-wider mb-2">Message</h3>
              <div class="bg-gray-50 p-4 rounded-lg text-sm text-text-main italic border border-border/60">
                "{{ selectedInquiry?.message }}"
              </div>
              <div class="text-xs text-text-muted mt-2 text-right">
                Received: {{ formatDateTime(selectedInquiry?.createdAt) }}
              </div>
            </div>

            <!-- Email Reply -->
            <div class="pt-4 border-t border-border/60">
              <h3 class="text-xs font-bold text-text-muted uppercase tracking-wider mb-3">Reply to Guest</h3>
              <div class="space-y-3">
                <textarea 
                  v-model="replyMessage"
                  rows="4"
                  placeholder="Type your reply here. This will be sent as an email to the guest..."
                  class="w-full px-3 py-2 text-sm bg-white border border-border/60 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary/30 resize-none"
                ></textarea>
                <div class="flex justify-end">
                  <button 
                    @click="sendReply"
                    :disabled="isSendingReply || !replyMessage.trim()"
                    class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700 transition-colors shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 flex items-center gap-2"
                  >
                    <Mail v-if="!isSendingReply" class="w-4 h-4" />
                    <svg v-else class="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
                    {{ isSendingReply ? 'Sending...' : 'Send Email Reply' }}
                  </button>
                </div>
              </div>
            </div>

            <!-- Management -->
            <div class="pt-4 border-t border-border/60">
              <h3 class="text-xs font-bold text-text-muted uppercase tracking-wider mb-3">Management</h3>
              
              <div class="space-y-4">
                <div>
                  <label class="block text-sm font-medium text-text-main mb-1">Status</label>
                  <select 
                    v-model="newStatus"
                    class="w-full pl-3 pr-8 py-2 text-sm bg-white border border-border/60 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary/30"
                  >
                    <option value="Pending">Pending</option>
                    <option value="Replied">Replied</option>
                    <option value="Resolved">Resolved</option>
                  </select>
                </div>

                <div>
                  <label class="block text-sm font-medium text-text-main mb-1">Internal Notes</label>
                  <textarea 
                    v-model="adminNotes"
                    rows="3"
                    placeholder="Add private notes..."
                    class="w-full px-3 py-2 text-sm bg-white border border-border/60 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary/30 resize-none"
                  ></textarea>
                </div>
              </div>
            </div>
          </div>

          <!-- Drawer Footer -->
          <div class="px-6 py-4 border-t border-border/60 bg-surface-sunken flex items-center justify-between">
            <button 
              @click="deleteInquiry"
              class="text-red-600 hover:bg-red-50 p-2 rounded-md transition-colors"
              title="Delete Inquiry"
            >
              <Trash2 class="w-4 h-4" />
            </button>
            <div class="flex items-center gap-3">
              <button 
                @click="closeDrawer"
                class="px-4 py-2 text-sm font-medium text-text-main bg-white border border-border/60 rounded-md hover:bg-black/5 transition-colors focus:outline-none focus:ring-2 focus:ring-primary/20"
              >
                Cancel
              </button>
              <button 
                @click="saveInquiryDetails"
                class="px-4 py-2 text-sm font-medium text-white bg-primary rounded-md hover:bg-primary-light transition-colors shadow-sm focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2"
              >
                Save Changes
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
