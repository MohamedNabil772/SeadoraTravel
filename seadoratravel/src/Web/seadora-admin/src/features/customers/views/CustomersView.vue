<script setup lang="ts">
// ponytail: calls @/services/api directly, matching the bookings sibling views - no
// repository/interface layer for two endpoints. English strings are inline like BookingsView;
// CRM i18n across the 5 locale files is deferred to the i18n task.
import { ref, onMounted, onBeforeUnmount } from 'vue'
import api from '@/services/api'
import { useToast } from '@/composables/useToast'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'
import { Search, Eye } from 'lucide-vue-next'

interface Customer {
  id: string
  fullName: string
  email: string
  phone?: string | null
  nationality?: string | null
  marketingConsent: boolean
  createdUtc: string
}

const toast = useToast()

const customers = ref<Customer[]>([])
const totalCount = ref(0)
const loading = ref(true)
const search = ref('')
const currentPage = ref(1)
const pageSize = ref(10)

async function loadCustomers() {
  loading.value = true
  try {
    const res = await api.get('/api/customer/api/customers', {
      params: {
        search: search.value || undefined,
        pageNumber: currentPage.value,
        pageSize: pageSize.value
      }
    })
    const raw = res.data
    customers.value = Array.isArray(raw) ? raw : (raw?.items || [])
    totalCount.value = Array.isArray(raw) ? raw.length : (raw?.totalCount ?? customers.value.length)
  } catch (e) {
    console.error('Failed to load customers', e)
    toast.error('Failed to load customers.')
    customers.value = []
    totalCount.value = 0
  } finally {
    loading.value = false
  }
}

let searchTimer: ReturnType<typeof setTimeout> | undefined
function onSearchInput() {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    currentPage.value = 1
    loadCustomers()
  }, 300)
}
onBeforeUnmount(() => clearTimeout(searchTimer))

function formatDate(dateStr?: string) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
}

onMounted(loadCustomers)
</script>

<template>
  <div class="customers-page">
    <div class="page-header">
      <div>
        <h2>Customers (CRM)</h2>
        <p>Browse customer profiles, contact details, marketing consent, and booking history.</p>
      </div>
      <div class="header-actions">
        <label for="customerSearch" class="search-label">Search customers</label>
        <div class="search-box">
          <Search class="search-icon" aria-hidden="true" />
          <input
            id="customerSearch"
            v-model="search"
            @input="onSearchInput"
            type="search"
            class="search-input"
            placeholder="Name, email or phone..."
          />
        </div>
      </div>
    </div>

    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading customers...</p>
    </div>

    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Customer</th>
            <th>Phone</th>
            <th>Nationality</th>
            <th>Marketing Consent</th>
            <th>Created</th>
            <th class="text-right pr-6">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="c in customers" :key="c.id">
            <td>
              <div class="customer-name text-black">{{ c.fullName }}</div>
              <div class="customer-email">{{ c.email }}</div>
            </td>
            <td class="font-mono text-xs">{{ c.phone || '—' }}</td>
            <td>{{ c.nationality || '—' }}</td>
            <td>
              <span :class="['status-badge', c.marketingConsent ? 'consent-yes' : 'consent-no']">
                {{ c.marketingConsent ? 'Opted-in' : 'No consent' }}
              </span>
            </td>
            <td class="font-mono text-xs text-body">{{ formatDate(c.createdUtc) }}</td>
            <td class="text-right pr-6">
              <div class="actions justify-end">
                <router-link
                  :to="'/customers/' + c.id"
                  class="btn-action view"
                  :aria-label="'View profile of ' + c.fullName"
                >
                  <Eye class="w-4 h-4" aria-hidden="true" />
                  <span>View</span>
                </router-link>
              </div>
            </td>
          </tr>
          <tr v-if="customers.length === 0">
            <td colspan="6" class="text-center py-8 text-body">No customers found.</td>
          </tr>
        </tbody>
      </table>

      <LuxuryPagination
        v-if="totalCount > 0"
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="totalCount"
        @pageChange="loadCustomers"
      />
    </div>
  </div>
</template>

<style scoped>
.customers-page { color: #24303F; }
.page-header { display: flex; justify-content: space-between; align-items: flex-end; gap: 16px; flex-wrap: wrap; margin-bottom: 20px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #1C2434; margin-bottom: 4px; }
.page-header p { color: #64748B; font-size: 14px; }

.search-label { display: block; font-size: 12px; font-weight: 600; color: #64748B; margin-bottom: 6px; }
.search-box { position: relative; }
.search-icon { position: absolute; left: 12px; top: 50%; transform: translateY(-50%); width: 16px; height: 16px; color: #64748B; pointer-events: none; }
.search-input {
  min-height: 44px;
  width: 280px;
  max-width: 100%;
  padding: 10px 14px 10px 36px;
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  background: #FFFFFF;
  color: #24303F;
  font-size: 14px;
  transition: border-color 0.2s ease-out;
}
.search-input:focus { border-color: #3C50E0; }

.table-container { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; overflow: hidden; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); min-height: 240px; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 16px 24px; text-align: left; font-size: 12px; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748B; background: #F7F9FC; border-bottom: 1px solid #E2E8F0; }
.data-table td { padding: 16px 24px; border-bottom: 1px solid #E2E8F0; font-size: 14px; color: #24303F; }
.data-table tr:hover { background: #F9FAFB; }

.customer-name { font-weight: 600; }
.customer-email { font-size: 12px; color: #64748B; margin-top: 2px; }

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

.actions { display: flex; gap: 8px; align-items: center; }
.btn-action {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  min-height: 44px;
  padding: 8px 14px;
  border: none;
  border-radius: 6px;
  color: #fff;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 1px 2px rgba(0,0,0,0.05);
  transition: transform 0.2s ease-out, box-shadow 0.2s ease-out;
}
.btn-action.view { background: #3C50E0; }
.btn-action:hover { transform: translateY(-1px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
.btn-action:active { transform: scale(0.97); }

.loading { text-align: center; padding: 60px; color: #64748B; min-height: 240px; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(60, 80, 224, 0.1); border-top-color: #3C50E0; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }

@media (prefers-reduced-motion: reduce) {
  * { animation: none !important; transition: none !important; }
}
</style>
