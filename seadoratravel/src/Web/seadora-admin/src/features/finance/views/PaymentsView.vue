<script setup lang="ts">
import { ref, computed } from 'vue'
import api from '@/services/api'
import { toast } from 'vue-sonner'

const bookingId = ref('')
const payments = ref<any[]>([])
const loading = ref(false)
const error = ref('')
const submitting = ref(false)

const form = ref({ amount: '', method: 'Card', reference: '', receivedUtc: '' })

const money = (v: number) =>
  new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2 }).format(v || 0)

const totalPaid = computed(() => payments.value.reduce((s, p) => s + (p.amount || 0), 0))
const validBooking = computed(() => /^[0-9a-fA-F-]{36}$/.test(bookingId.value.trim()))

async function loadPayments() {
  if (!validBooking.value) {
    error.value = 'Enter a valid booking id (GUID).'
    return
  }
  loading.value = true
  error.value = ''
  try {
    const res = await api.get(`/api/finance/api/payments/booking/${bookingId.value.trim()}`)
    payments.value = res.data
  } catch (e: any) {
    error.value = e?.response?.status === 403
      ? 'You do not have permission to manage payments.'
      : 'Failed to load payments.'
  } finally {
    loading.value = false
  }
}

async function recordPayment() {
  if (!validBooking.value) { error.value = 'Enter a valid booking id first.'; return }
  const amount = parseFloat(form.value.amount)
  if (!(amount > 0)) { error.value = 'Amount must be greater than zero.'; return }
  submitting.value = true
  error.value = ''
  try {
    await api.post(`/api/finance/api/payments/booking/${bookingId.value.trim()}`, {
      amount,
      method: form.value.method,
      reference: form.value.reference || null,
      receivedUtc: form.value.receivedUtc || null
    })
    toast.success('Payment recorded')
    form.value = { amount: '', method: 'Card', reference: '', receivedUtc: '' }
    await loadPayments()
  } catch (e: any) {
    if (e?.response?.status === 404) {
      error.value = 'No financial snapshot for this booking yet — revenue must be recognized first.'
    } else if (e?.response?.status === 403) {
      error.value = 'You do not have permission to record payments.'
    } else {
      error.value = 'Failed to record payment.'
    }
    toast.error(error.value)
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="p-6 space-y-6 max-w-5xl">
    <div>
      <h1 class="text-2xl font-bold text-dark">Payments</h1>
      <p class="text-body text-sm mt-1">Record customer receipts and review a booking's payment history.</p>
    </div>

    <!-- Booking lookup -->
    <div class="bg-white border border-stroke rounded-lg p-4 flex flex-wrap items-end gap-3">
      <label class="flex flex-col text-xs text-body font-semibold flex-1 min-w-[280px]">Booking ID
        <input v-model="bookingId" placeholder="00000000-0000-0000-0000-000000000000"
               class="mt-1 border border-stroke rounded-md px-3 py-2 text-dark font-mono text-sm" />
      </label>
      <button @click="loadPayments" :disabled="!validBooking"
        class="px-4 py-2 rounded-md bg-dark text-white text-sm font-semibold hover:opacity-90 transition-opacity disabled:opacity-40">
        Load
      </button>
    </div>

    <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 rounded-lg p-3 text-sm">{{ error }}</div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Record form -->
      <div class="bg-white border border-stroke rounded-lg shadow-sm p-5 space-y-3">
        <h3 class="font-bold text-dark">Record a Payment</h3>
        <label class="flex flex-col text-xs text-body font-semibold">Amount
          <input v-model="form.amount" type="number" min="0" step="0.01" placeholder="0.00"
                 class="mt-1 border border-stroke rounded-md px-3 py-2 text-dark" />
        </label>
        <label class="flex flex-col text-xs text-body font-semibold">Method
          <select v-model="form.method" class="mt-1 border border-stroke rounded-md px-3 py-2 text-dark">
            <option>Cash</option><option>Card</option><option>Bank</option><option>Other</option>
          </select>
        </label>
        <label class="flex flex-col text-xs text-body font-semibold">Reference
          <input v-model="form.reference" placeholder="Transaction / wire reference"
                 class="mt-1 border border-stroke rounded-md px-3 py-2 text-dark" />
        </label>
        <label class="flex flex-col text-xs text-body font-semibold">Received date (optional)
          <input v-model="form.receivedUtc" type="datetime-local"
                 class="mt-1 border border-stroke rounded-md px-3 py-2 text-dark" />
        </label>
        <button @click="recordPayment" :disabled="submitting || !validBooking"
          class="w-full mt-2 px-4 py-2.5 rounded-md bg-amber-500 text-white font-semibold hover:bg-amber-600 transition-colors disabled:opacity-40">
          {{ submitting ? 'Recording…' : 'Record Payment' }}
        </button>
      </div>

      <!-- History -->
      <div class="bg-white border border-stroke rounded-lg shadow-sm p-5">
        <div class="flex items-center justify-between mb-3">
          <h3 class="font-bold text-dark">Payment History</h3>
          <span class="text-sm text-body">Total: <strong class="text-dark">{{ money(totalPaid) }}</strong></span>
        </div>
        <div v-if="loading" class="py-10 text-center text-body animate-pulse">Loading…</div>
        <table v-else class="w-full text-sm">
          <thead>
            <tr class="text-left text-body border-b border-stroke">
              <th class="py-2 font-semibold">Date</th>
              <th class="py-2 font-semibold text-right">Amount</th>
              <th class="py-2 font-semibold">Method</th>
              <th class="py-2 font-semibold">Reference</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in payments" :key="p.id" class="border-b border-stroke/50 hover:bg-slate-50">
              <td class="py-2 text-dark">{{ new Date(p.receivedUtc).toLocaleDateString() }}</td>
              <td class="py-2 text-right tabular-nums text-dark">{{ money(p.amount) }}</td>
              <td class="py-2 text-body">{{ p.method }}</td>
              <td class="py-2 text-body truncate max-w-[140px]">{{ p.reference || '—' }}</td>
            </tr>
            <tr v-if="!payments.length">
              <td colspan="4" class="py-8 text-center text-body">No payments recorded.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
