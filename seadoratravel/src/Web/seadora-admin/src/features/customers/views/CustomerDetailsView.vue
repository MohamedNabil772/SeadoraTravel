<script setup lang="ts">
// ponytail: calls @/services/api directly, matching the bookings sibling views. CRM i18n and
// document upload/download UI are deferred to their own tasks - documents are read-only here.
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'
import { useToast } from '@/composables/useToast'
import { ArrowLeft, Save } from 'lucide-vue-next'

interface CustomerDocument {
  id: string
  documentType: string
  fileName: string
  uploadedUtc: string
  retentionUntilUtc?: string | null
}

interface CustomerBookingHistory {
  id: string
  bookingId: string
  tourId: string
  tourDate?: string | null
  amount: number
  currency: string
  placedUtc: string
}

interface CustomerDetail {
  id: string
  fullName: string
  email: string
  phone?: string | null
  nationality?: string | null
  passportNumber?: string | null
  notes?: string | null
  marketingConsent: boolean
  consentUpdatedUtc?: string | null
  createdUtc: string
  updatedUtc: string
  documents: CustomerDocument[]
  bookingHistory: CustomerBookingHistory[]
}

const route = useRoute()
const toast = useToast()

const customer = ref<CustomerDetail | null>(null)
const loading = ref(true)
const notFound = ref(false)
const saving = ref(false)
const consentSaving = ref(false)

const form = ref({
  fullName: '',
  email: '',
  phone: '',
  nationality: '',
  passportNumber: '',
  notes: ''
})

const customerId = route.params.id as string

async function loadCustomer() {
  loading.value = true
  notFound.value = false
  try {
    const res = await api.get(`/api/customer/api/customers/${customerId}`)
    const data = res.data as CustomerDetail
    customer.value = data
    form.value = {
      fullName: data.fullName ?? '',
      email: data.email ?? '',
      phone: data.phone ?? '',
      nationality: data.nationality ?? '',
      passportNumber: data.passportNumber ?? '',
      notes: data.notes ?? ''
    }
  } catch (e: any) {
    console.error('Failed to load customer', e)
    if (e?.response?.status === 404) {
      notFound.value = true
    } else {
      toast.error('Failed to load customer profile.')
    }
  } finally {
    loading.value = false
  }
}

async function saveChanges() {
  if (saving.value) return
  saving.value = true
  try {
    await api.put(`/api/customer/api/customers/${customerId}`, {
      id: customerId,
      fullName: form.value.fullName,
      email: form.value.email,
      phone: form.value.phone || null,
      nationality: form.value.nationality || null,
      passportNumber: form.value.passportNumber || null,
      notes: form.value.notes || null
    })
    toast.success('Customer profile updated successfully')
    await loadCustomer()
  } catch (e) {
    console.error('Failed to update customer', e)
    toast.error('Failed to update customer profile.')
  } finally {
    saving.value = false
  }
}

async function toggleConsent() {
  if (!customer.value || consentSaving.value) return
  const next = !customer.value.marketingConsent
  consentSaving.value = true
  try {
    await api.put(`/api/customer/api/customers/${customerId}/consent`, { id: customerId, consent: next })
    toast.success(next ? 'Marketing consent granted' : 'Marketing consent withdrawn')
    await loadCustomer()
  } catch (e) {
    console.error('Failed to update marketing consent', e)
    toast.error('Failed to update marketing consent.')
  } finally {
    consentSaving.value = false
  }
}

function formatDate(dateStr?: string | null) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
}

function formatDateTime(dateStr?: string | null) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
  })
}

function formatAmount(amount: number, currency: string) {
  return `${amount.toFixed(2)} ${currency}`
}

onMounted(loadCustomer)
</script>

<template>
  <div class="customer-details-page">
    <div class="page-header">
      <div>
        <h2>{{ customer?.fullName || 'Customer Profile' }}</h2>
        <p v-if="customer">Customer since {{ formatDate(customer.createdUtc) }}</p>
        <p v-else>Contact details, marketing consent, booking history and documents.</p>
      </div>
      <router-link to="/customers" class="btn-secondary" aria-label="Back to Customers">
        <ArrowLeft class="w-4 h-4" aria-hidden="true" />
        <span>Back to Customers</span>
      </router-link>
    </div>

    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading customer profile...</p>
    </div>

    <div v-else-if="notFound || !customer" class="card empty-card">
      <h3>Customer not found</h3>
      <p>This customer does not exist or is not part of your branch.</p>
    </div>

    <div v-else class="detail-grid">
      <!-- Profile -->
      <section class="card">
        <h3 class="card-title">Profile</h3>
        <form class="form-grid" @submit.prevent="saveChanges">
          <div class="field">
            <label for="fullName">Full Name</label>
            <input id="fullName" v-model="form.fullName" type="text" required />
          </div>
          <div class="field">
            <label for="email">Email</label>
            <input id="email" v-model="form.email" type="email" required />
          </div>
          <div class="field">
            <label for="phone">Phone</label>
            <input id="phone" v-model="form.phone" type="tel" />
          </div>
          <div class="field">
            <label for="nationality">Nationality</label>
            <input id="nationality" v-model="form.nationality" type="text" />
          </div>
          <div class="field">
            <label for="passportNumber">Passport Number</label>
            <input id="passportNumber" v-model="form.passportNumber" type="text" />
          </div>
          <div class="field field-full">
            <label for="notes">Notes</label>
            <textarea id="notes" v-model="form.notes" rows="3"></textarea>
          </div>
          <div class="field-full form-actions">
            <button type="submit" class="btn-primary" :disabled="saving">
              <Save class="w-4 h-4" aria-hidden="true" />
              <span>{{ saving ? 'Saving...' : 'Save Changes' }}</span>
            </button>
          </div>
        </form>
      </section>

      <!-- Marketing consent -->
      <section class="card">
        <h3 class="card-title">Marketing Consent</h3>
        <div class="consent-row">
          <div>
            <span :class="['status-badge', customer.marketingConsent ? 'consent-yes' : 'consent-no']">
              {{ customer.marketingConsent ? 'Opted-in' : 'No consent' }}
            </span>
            <p class="consent-meta">
              <template v-if="customer.consentUpdatedUtc">
                Last changed {{ formatDateTime(customer.consentUpdatedUtc) }}
              </template>
              <template v-else>No consent decision recorded yet.</template>
            </p>
          </div>
          <button
            type="button"
            role="switch"
            :aria-checked="customer.marketingConsent"
            :aria-label="customer.marketingConsent ? 'Withdraw marketing consent' : 'Grant marketing consent'"
            class="consent-toggle"
            :class="{ on: customer.marketingConsent }"
            :disabled="consentSaving"
            @click="toggleConsent"
          >
            <span class="knob"></span>
          </button>
        </div>
      </section>

      <!-- Booking history -->
      <section class="card card-full">
        <h3 class="card-title">Booking History</h3>
        <!-- ponytail: shows the raw TourId - a tour-name join across services is deferred. -->
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Tour</th>
                <th>Trip Date</th>
                <th>Amount</th>
                <th>Placed</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="h in customer.bookingHistory" :key="h.id">
                <td class="font-mono text-xs">{{ h.tourId }}</td>
                <td>{{ formatDate(h.tourDate) }}</td>
                <td class="font-semibold">{{ formatAmount(h.amount, h.currency) }}</td>
                <td class="font-mono text-xs text-body">{{ formatDateTime(h.placedUtc) }}</td>
              </tr>
              <tr v-if="customer.bookingHistory.length === 0">
                <td colspan="4" class="text-center py-8 text-body">No bookings recorded for this customer.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- Documents -->
      <section class="card card-full">
        <h3 class="card-title">Documents</h3>
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Type</th>
                <th>File Name</th>
                <th>Uploaded</th>
                <th>Retention Until</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="d in customer.documents" :key="d.id">
                <td class="font-semibold">{{ d.documentType }}</td>
                <td>{{ d.fileName }}</td>
                <td class="font-mono text-xs text-body">{{ formatDate(d.uploadedUtc) }}</td>
                <td class="font-mono text-xs text-body">{{ formatDate(d.retentionUntilUtc) }}</td>
              </tr>
              <tr v-if="customer.documents.length === 0">
                <td colspan="4" class="text-center py-8 text-body">No documents on file.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
.customer-details-page { color: #24303F; }
.page-header { display: flex; justify-content: space-between; align-items: flex-start; gap: 16px; flex-wrap: wrap; margin-bottom: 20px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #1C2434; margin-bottom: 4px; }
.page-header p { color: #64748B; font-size: 14px; }

.detail-grid { display: grid; grid-template-columns: 2fr 1fr; gap: 24px; align-items: start; }
@media (max-width: 1024px) { .detail-grid { grid-template-columns: 1fr; } }
.card { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; padding: 24px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); }
.card-full { grid-column: 1 / -1; }
.card-title { font-size: 16px; font-weight: 700; color: #1C2434; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 1px solid #E2E8F0; }
.empty-card { text-align: center; padding: 48px; }
.empty-card h3 { font-size: 18px; font-weight: 700; color: #1C2434; margin-bottom: 8px; }
.empty-card p { color: #64748B; font-size: 14px; }

.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
@media (max-width: 640px) { .form-grid { grid-template-columns: 1fr; } }
.field { display: flex; flex-direction: column; gap: 6px; }
.field-full { grid-column: 1 / -1; }
.field label { font-size: 12px; font-weight: 600; color: #64748B; text-transform: uppercase; letter-spacing: 0.05em; }
.field input, .field textarea {
  min-height: 44px;
  padding: 10px 14px;
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  background: #FFFFFF;
  color: #24303F;
  font-size: 14px;
  font-family: inherit;
  transition: border-color 0.2s ease-out;
}
.field input:focus, .field textarea:focus { border-color: #3C50E0; }
.form-actions { display: flex; justify-content: flex-end; gap: 8px; }

.btn-primary, .btn-secondary {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  min-height: 44px;
  padding: 10px 18px;
  border: none;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 1px 2px rgba(0,0,0,0.05);
  transition: transform 0.2s ease-out, box-shadow 0.2s ease-out;
}
.btn-primary { background: #3C50E0; color: #FFFFFF; }
.btn-secondary { background: #FFFFFF; color: #1C2434; border: 1px solid #E2E8F0; }
.btn-primary:hover, .btn-secondary:hover { transform: translateY(-1px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
.btn-primary:active, .btn-secondary:active { transform: scale(0.97); }
.btn-primary:disabled { opacity: 0.5; cursor: not-allowed; transform: none; box-shadow: none; }

.consent-row { display: flex; justify-content: space-between; align-items: center; gap: 16px; }
.consent-meta { color: #64748B; font-size: 12px; margin-top: 8px; }
.consent-toggle {
  position: relative;
  flex-shrink: 0;
  width: 56px;
  height: 44px;
  padding: 0 4px;
  border: 1px solid #E2E8F0;
  border-radius: 22px;
  background: #E2E8F0;
  cursor: pointer;
  transition: background-color 0.2s ease-out;
}
.consent-toggle.on { background: #10B981; border-color: #10B981; }
.consent-toggle:disabled { opacity: 0.5; cursor: not-allowed; }
.consent-toggle:active { transform: scale(0.97); }
.knob {
  display: block;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: #FFFFFF;
  box-shadow: 0 1px 3px rgba(0,0,0,0.2);
  transform: translateX(0);
  transition: transform 0.2s ease-out;
}
.consent-toggle.on .knob { transform: translateX(24px); }

.status-badge {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  display: inline-block;
  box-shadow: 0 1px 2px rgba(0,0,0,0.02);
}
.status-badge.consent-yes { background: rgba(16, 185, 129, 0.1); color: #047857; border: 1px solid rgba(16, 185, 129, 0.2); }
.status-badge.consent-no { background: #F1F5F9; color: #475569; border: 1px solid #E2E8F0; }

.table-container { border: 1px solid #E2E8F0; border-radius: 4px; overflow: hidden; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 12px 16px; text-align: left; font-size: 12px; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748B; background: #F7F9FC; border-bottom: 1px solid #E2E8F0; }
.data-table td { padding: 12px 16px; border-bottom: 1px solid #E2E8F0; font-size: 14px; color: #24303F; }
.data-table tr:hover { background: #F9FAFB; }

.loading { text-align: center; padding: 60px; color: #64748B; min-height: 240px; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(60, 80, 224, 0.1); border-top-color: #3C50E0; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }

@media (prefers-reduced-motion: reduce) {
  * { animation: none !important; transition: none !important; }
}
</style>
