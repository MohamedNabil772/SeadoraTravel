<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'
import { Plus } from 'lucide-vue-next'

interface PaymentAgreement {
  id: string
  name: string
}

interface Supplier {
  id?: string
  nameAr: string
  nameEn?: string
  bankAccountInfo: string
  paymentAgreementId: string
  paymentAgreement?: PaymentAgreement
}

const suppliers = ref<Supplier[]>([])
const agreements = ref<PaymentAgreement[]>([])
const loading = ref(true)

// Modals state
const showSupplierModal = ref(false)
const showAgreementModal = ref(false)
const isEditingSupplier = ref(false)

// Form state - Supplier
const supplierForm = ref<Supplier>({
  nameAr: '',
  nameEn: '',
  bankAccountInfo: '',
  paymentAgreementId: ''
})

// Form state - Agreement
const agreementForm = ref({
  id: '',
  name: ''
})

onMounted(async () => {
  await fetchData()
})

async function fetchData() {
  loading.value = true
  try {
    const results = await Promise.allSettled([
      api.get('/api/content/api/suppliers'),
      api.get('/api/content/api/paymentagreements')
    ])
    
    if (results[0].status === 'fulfilled') {
      const data = results[0].value.data
      suppliers.value = Array.isArray(data) ? data : (data?.items || [])
    }
    if (results[1].status === 'fulfilled') {
      const data = results[1].value.data
      agreements.value = Array.isArray(data) ? data : (data?.items || [])
    }
  } catch (err) {
    console.error('Error fetching data:', err)
  } finally {
    loading.value = false
  }
}

// Supplier operations
function openAddSupplier() {
  isEditingSupplier.value = false
  supplierForm.value = {
    nameAr: '',
    nameEn: '',
    bankAccountInfo: '',
    paymentAgreementId: agreements.value[0]?.id || ''
  }
  showSupplierModal.value = true
}

function openEditSupplier(supplier: Supplier) {
  isEditingSupplier.value = true
  supplierForm.value = { ...supplier }
  showSupplierModal.value = true
}

const { confirm } = useConfirm()
const toast = useToast()

async function saveSupplier() {
  try {
    if (isEditingSupplier.value) {
      await api.put(`/api/content/api/suppliers/${supplierForm.value.id}`, supplierForm.value)
      toast.success('Supplier updated successfully')
    } else {
      await api.post('/api/content/api/suppliers', supplierForm.value)
      toast.success('Supplier created successfully')
    }
    showSupplierModal.value = false
    await fetchData()
  } catch (err) {
    console.error('Error saving supplier:', err)
    toast.error('Failed to save supplier')
  }
}

async function deleteSupplier(id: string) {
  const ok = await confirm({
    title: 'Delete Supplier',
    message: 'Are you sure you want to delete this supplier?',
    confirmText: 'Delete',
    type: 'danger'
  })
  if (!ok) return

  try {
    await api.delete(`/api/content/api/suppliers/${id}`)
    toast.success('Supplier deleted successfully')
    await fetchData()
  } catch (err) {
    console.error('Error deleting supplier:', err)
    toast.error('Failed to delete supplier')
  }
}

// Payment Agreement operations
async function saveAgreement() {
  if (!agreementForm.value.name) return
  try {
    await api.post('/api/content/api/paymentagreements', {
      name: agreementForm.value.name
    })
    toast.success('Payment agreement created successfully')
    agreementForm.value.name = ''
    showAgreementModal.value = false
    await fetchData()
  } catch (err) {
    console.error('Error saving agreement:', err)
    toast.error('Failed to save agreement')
  }
}

async function deleteAgreement(id: string) {
  const ok = await confirm({
    title: 'Delete Payment Agreement',
    message: 'Delete this payment agreement? Tours/Suppliers referencing it might be affected.',
    confirmText: 'Delete',
    type: 'danger'
  })
  if (!ok) return

  try {
    await api.delete(`/api/content/api/paymentagreements/${id}`)
    toast.success('Payment agreement deleted successfully')
    await fetchData()
  } catch (err) {
    console.error('Error deleting agreement:', err)
    toast.error('Could not delete. Make sure it is not referenced by any supplier.')
  }
}
</script>

<template>
  <div class="suppliers-page">
    <!-- Breadcrumbs / Page Header -->
    <div class="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
      <div class="page-header">
        <h2>Suppliers & Agreements</h2>
        <p>Configure travel operators, payout schedules, and bank account information.</p>
      </div>
      <div class="flex items-center gap-3">
        <button @click="showAgreementModal = true" class="btn-action-secondary">
          ⚙️ Manage Agreements
        </button>
        <button @click="openAddSupplier" class="btn-action-primary">
          <Plus class="w-4 h-4" />
          <span>Add Supplier</span>
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading suppliers...</p>
    </div>

    <!-- Data Table Card (TailAdmin white card with border & shadow) -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Arabic Name</th>
            <th>English Name</th>
            <th>Payment Cycle</th>
            <th>Bank details</th>
            <th class="text-right pr-6">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="sup in suppliers" :key="sup.id">
            <td class="font-semibold text-black rtl text-right">{{ sup.nameAr }}</td>
            <td class="text-black font-medium">{{ sup.nameEn || '—' }}</td>
            <td>
              <span class="cycle-badge">
                {{ sup.paymentAgreement?.name || 'TBD' }}
              </span>
            </td>
            <td class="font-mono text-xs text-body max-w-xs truncate" :title="sup.bankAccountInfo">
              {{ sup.bankAccountInfo }}
            </td>
            <td class="text-right pr-6">
              <div class="actions justify-end">
                <button @click="openEditSupplier(sup)" class="btn-edit-action" title="Edit">✏️</button>
                <button @click="sup.id && deleteSupplier(sup.id)" class="btn-delete-action" title="Delete">🗑️</button>
              </div>
            </td>
          </tr>
          <tr v-if="suppliers.length === 0">
            <td colspan="5" class="empty-state">
              No suppliers registered yet. Click "Add Supplier" to configure one.
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Supplier Form Modal (TailAdmin Modal Styled) -->
    <div v-if="showSupplierModal" class="modal-overlay" @click="showSupplierModal = false">
      <div class="modal-card" @click.stop>
        <div class="modal-header">
          <h3>{{ isEditingSupplier ? 'Edit Supplier' : 'Add New Supplier' }}</h3>
          <button @click="showSupplierModal = false" class="btn-close">✕</button>
        </div>
        <form @submit.prevent="saveSupplier" class="modal-form">
          <div class="form-group">
            <label>Arabic Name (Required)</label>
            <input v-model="supplierForm.nameAr" type="text" required placeholder="مثال: شركة ترافل" class="rtl text-right">
          </div>
          <div class="form-group">
            <label>English Name (Optional)</label>
            <input v-model="supplierForm.nameEn" type="text" placeholder="e.g. Travel Supplier Ltd.">
          </div>
          <div class="form-group">
            <label>Payment Cycle Agreement</label>
            <select v-model="supplierForm.paymentAgreementId" required>
              <option v-for="agr in agreements" :key="agr.id" :value="agr.id">
                {{ agr.name }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label>Bank Account Information</label>
            <textarea v-model="supplierForm.bankAccountInfo" required rows="3" placeholder="IBAN: EG...&#10;Bank Name: ...&#10;Swift Code: ..."></textarea>
          </div>
          <div class="modal-actions">
            <button type="button" @click="showSupplierModal = false" class="btn-cancel">Cancel</button>
            <button type="submit" class="btn-save">Save</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Agreements Manage Modal (TailAdmin Modal Styled) -->
    <div v-if="showAgreementModal" class="modal-overlay" @click="showAgreementModal = false">
      <div class="modal-card" @click.stop>
        <div class="modal-header">
          <h3>Configure Cycles</h3>
          <button @click="showAgreementModal = false" class="btn-close">✕</button>
        </div>
        <div class="modal-form">
          <!-- Add form -->
          <form @submit.prevent="saveAgreement" class="flex gap-2">
            <input v-model="agreementForm.name" type="text" placeholder="e.g. Semi-Monthly" required class="flex-1">
            <button type="submit" class="btn-save !mt-0">Add</button>
          </form>

          <!-- List -->
          <div class="space-y-2 mt-4 max-h-60 overflow-y-auto">
            <div v-for="agr in agreements" :key="agr.id" class="agreement-item">
              <span class="font-semibold text-black">{{ agr.name }}</span>
              <button @click="deleteAgreement(agr.id)" class="text-rose-500 hover:text-rose-600 text-xs font-bold cursor-pointer">
                Delete
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<style scoped>
.suppliers-page { color: #24303F; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #1C2434; margin-bottom: 4px; }
.page-header p { color: #64748B; font-size: 14px; }

.btn-action-primary { padding: 10px 22px; background: #3C50E0; border: none; border-radius: 4px; color: #fff; font-weight: 600; cursor: pointer; font-size: 14px; transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1); will-change: transform; }
.btn-action-primary:hover { background: #2B3CA6; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(60, 80, 224, 0.2); }
.btn-action-primary:active { transform: scale(0.97); box-shadow: 0 2px 4px rgba(60, 80, 224, 0.1); }

.btn-action-secondary { padding: 10px 22px; background: #fff; border: 1px solid #E2E8F0; border-radius: 4px; color: #64748B; font-weight: 600; cursor: pointer; font-size: 14px; transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1); box-shadow: 0px 1px 2px rgba(0, 0, 0, 0.05); will-change: transform; }
.btn-action-secondary:hover { background: #F7F9FC; color: #1C2434; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05); }
.btn-action-secondary:active { transform: scale(0.97); box-shadow: 0 2px 4px rgba(0, 0, 0, 0.02); }

.table-container { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); overflow: hidden; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 16px 24px; text-align: left; font-size: 12px; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748B; background: #F7F9FC; border-bottom: 1px solid #E2E8F0; }
.data-table td { padding: 16px 24px; border-bottom: 1px solid #E2E8F0; font-size: 14px; color: #24303F; transition: background 0.2s cubic-bezier(0.16, 1, 0.3, 1); }
.data-table tr { transition: transform 0.2s cubic-bezier(0.16, 1, 0.3, 1); }
.data-table tr:hover { background: #F9FAFB; }

.cycle-badge { padding: 4px 10px; background: rgba(60, 80, 224, 0.08); color: #3C50E0; border-radius: 4px; font-size: 12px; font-weight: 600; display: inline-block; }

.actions { display: flex; gap: 8px; }
.btn-edit-action, .btn-delete-action { background: none; border: none; cursor: pointer; font-size: 18px; padding: 4px; border-radius: 4px; transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1); will-change: transform; }
.btn-edit-action:hover { background: #EFF4FB; transform: translateY(-1px); }
.btn-edit-action:active { transform: scale(0.95); }
.btn-delete-action:hover { background: rgba(211, 64, 83, 0.1); transform: translateY(-1px); }
.btn-delete-action:active { transform: scale(0.95); }

/* Modal overlay styles */
.modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 2000; padding: 20px; transition: opacity 0.2s ease-out; }
.modal-card { background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 4px; box-shadow: 0px 8px 13px -3px rgba(0, 0, 0, 0.07); width: 100%; max-width: 480px; display: flex; flex-direction: column; overflow: hidden; color: #24303F; transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1); }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 20px; border-bottom: 1px solid #E2E8F0; }
.modal-header h3 { font-size: 18px; font-weight: 700; color: #1C2434; }
.btn-close { background: none; border: none; color: #8A99AD; cursor: pointer; font-size: 18px; transition: transform 0.2s cubic-bezier(0.16, 1, 0.3, 1); will-change: transform; }
.btn-close:active { transform: scale(0.95); }

.modal-form { padding: 20px; display: flex; flex-direction: column; gap: 16px; max-height: 80vh; overflow-y: auto; }
.form-group { display: flex; flex-direction: column; gap: 6px; }
.form-group label { color: #64748B; font-size: 12px; font-weight: 600; text-transform: uppercase; }
.form-group input, .form-group select, .form-group textarea {
  padding: 12px 14px; background: #FFFFFF; border: 1.5px solid #E2E8F0; border-radius: 4px; color: #24303F; outline: none; font-size: 14px; transition: border-color 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}
.form-group input:focus, .form-group select:focus, .form-group textarea:focus { border-color: #3C50E0; }

.modal-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 10px; }
.btn-cancel { padding: 10px 22px; background: #fff; border: 1px solid #E2E8F0; border-radius: 4px; color: #64748B; cursor: pointer; font-weight: 600; transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1); will-change: transform; }
.btn-cancel:hover { background: #F7F9FC; transform: translateY(-1px); }
.btn-cancel:active { transform: scale(0.97); }
.btn-save { padding: 10px 24px; background: #3C50E0; border: none; border-radius: 4px; color: #fff; font-weight: 600; cursor: pointer; transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1); will-change: transform; }
.btn-save:hover { background: #2B3CA6; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(60, 80, 224, 0.2); }
.btn-save:active { transform: scale(0.97); }

.agreement-item { display: flex; justify-content: space-between; align-items: center; padding: 12px 16px; background: #F7F9FC; border: 1px solid #E2E8F0; border-radius: 4px; margin-bottom: 8px; width: 100%; }

.loading { text-align: center; padding: 60px; color: #64748B; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(60, 80, 224, 0.1); border-top-color: #3C50E0; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state { text-align: center; padding: 48px; color: #64748B; }
</style>
