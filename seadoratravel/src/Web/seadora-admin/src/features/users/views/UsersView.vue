<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'
import { useAuthStore } from '@/features/auth/store/auth'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'
import { Plus } from 'lucide-vue-next'

const authStore = useAuthStore()

interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  phoneNumber?: string
  roles: string[]
}

const users = ref<User[]>([])
const roles = ref<string[]>([])
const loading = ref(true)
const actionLoading = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const searchQuery = ref('')

const form = ref({
  id: '',
  email: '',
  firstName: '',
  lastName: '',
  phoneNumber: '',
  password: '',
  roles: [] as string[]
})

async function fetchData() {
  loading.value = true
  try {
    const [usersRes, rolesRes] = await Promise.all([
      api.get('/api/auth/api/users'),
      api.get('/api/auth/api/users/roles')
    ])
    users.value = usersRes.data
    roles.value = rolesRes.data
  } catch (e) {
    console.error('Failed to fetch users or roles', e)
  } finally {
    loading.value = false
  }
}

const filteredUsers = computed(() => {
  const query = searchQuery.value.toLowerCase().trim()
  if (!query) return users.value
  return users.value.filter(u => 
    u.email.toLowerCase().includes(query) ||
    u.firstName.toLowerCase().includes(query) ||
    u.lastName.toLowerCase().includes(query) ||
    (u.phoneNumber && u.phoneNumber.includes(query))
  )
})

const currentPage = ref(1)
const pageSize = ref(10)

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredUsers.value.slice(start, start + pageSize.value)
})

const stats = computed(() => {
  const total = users.value.length
  const admins = users.value.filter(u => u.roles.includes('Admin')).length
  const managers = users.value.filter(u => u.roles.includes('BookingManager')).length
  const customers = users.value.filter(u => u.roles.includes('Customer')).length
  return { total, admins, managers, customers }
})

function openCreateModal() {
  isEdit.value = false
  form.value = {
    id: '',
    email: '',
    firstName: '',
    lastName: '',
    phoneNumber: '',
    password: '',
    roles: ['Customer']
  }
  showModal.value = true
}

function openEditModal(user: User) {
  isEdit.value = true
  form.value = {
    id: user.id,
    email: user.email,
    firstName: user.firstName,
    lastName: user.lastName,
    phoneNumber: user.phoneNumber || '',
    password: '',
    roles: [...user.roles]
  }
  showModal.value = true
}

const { confirm } = useConfirm()
const toast = useToast()

async function saveUser() {
  actionLoading.value = true
  try {
    const payload = {
      email: form.value.email,
      firstName: form.value.firstName,
      lastName: form.value.lastName,
      phoneNumber: form.value.phoneNumber || null,
      password: form.value.password || null,
      roles: form.value.roles
    }

    if (isEdit.value) {
      await api.put(`/api/auth/api/users/${form.value.id}`, payload)
      toast.success('User updated successfully')
    } else {
      await api.post('/api/auth/api/users', payload)
      toast.success('User created successfully')
    }

    showModal.value = false
    await fetchData()
  } catch (e: any) {
    console.error('Failed to save user', e)
    const err = e.response?.data?.errors?.join(', ') || e.response?.data?.error || 'Failed to save user.'
    toast.error(err)
  } finally {
    actionLoading.value = false
  }
}

async function deleteUser(user: User) {
  if (user.email === authStore.user?.email) {
    toast.error('You cannot delete your own admin account.')
    return
  }

  const ok = await confirm({
    title: 'Delete User',
    message: `Are you sure you want to delete user ${user.firstName} ${user.lastName} (${user.email})?`,
    confirmText: 'Delete',
    type: 'danger'
  })
  if (!ok) return

  actionLoading.value = true
  try {
    await api.delete(`/api/auth/api/users/${user.id}`)
    toast.success('User deleted successfully')
    await fetchData()
  } catch (e: any) {
    console.error('Failed to delete user', e)
    const err = e.response?.data?.error || 'Failed to delete user.'
    toast.error(err)
  } finally {
    actionLoading.value = false
  }
}

function getRoleClass(role: string) {
  if (role === 'Admin') return 'role-badge-admin'
  if (role === 'BookingManager') return 'role-badge-manager'
  return 'role-badge-customer'
}

onMounted(() => {
  authStore.initAuth()
  fetchData()
})
</script>

<template>
  <div class="users-page">
    <!-- Page Header -->
    <div class="page-header flex flex-col sm:flex-row justify-between sm:items-center gap-4 mb-6">
      <div>
        <h2>User Management</h2>
        <p>Create, update, and manage admin users, managers, and customers.</p>
      </div>
      <button @click="openCreateModal" class="btn-create self-start sm:self-auto">
        <Plus class="w-4 h-4" />
        <span>Add New User</span>
      </button>
    </div>

    <!-- Stats Row -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="stats-card">
        <div class="stats-icon bg-blue-100 text-blue-600">👥</div>
        <div class="stats-info">
          <span class="stats-label">Total Users</span>
          <span class="stats-value">{{ stats.total }}</span>
        </div>
      </div>
      <div class="stats-card">
        <div class="stats-icon bg-red-100 text-red-600">🛡️</div>
        <div class="stats-info">
          <span class="stats-label">Administrators</span>
          <span class="stats-value">{{ stats.admins }}</span>
        </div>
      </div>
      <div class="stats-card">
        <div class="stats-icon bg-purple-100 text-purple-600">💼</div>
        <div class="stats-info">
          <span class="stats-label">Booking Managers</span>
          <span class="stats-value">{{ stats.managers }}</span>
        </div>
      </div>
      <div class="stats-card">
        <div class="stats-icon bg-emerald-100 text-emerald-600">👤</div>
        <div class="stats-info">
          <span class="stats-label">Customers</span>
          <span class="stats-value">{{ stats.customers }}</span>
        </div>
      </div>
    </div>

    <!-- Controls Row -->
    <div class="flex flex-col md:flex-row gap-4 justify-between items-stretch md:items-center mb-6">
      <div class="search-wrapper flex-1 max-w-md">
        <input 
          v-model="searchQuery" 
          type="text" 
          placeholder="Search by name, email or phone..." 
          aria-label="Search users"
          class="w-full px-4 py-2.5 bg-white border border-[#E2E8F0] rounded focus:outline-none focus:border-[#3C50E0]"
        />
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading users database...</p>
    </div>

    <!-- Data Table -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>User</th>
            <th>Email</th>
            <th>Phone Number</th>
            <th>Roles</th>
            <th class="text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in paginatedUsers" :key="user.id">
            <td>
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-full bg-slate-100 border border-slate-200 flex items-center justify-center font-bold text-slate-600 uppercase">
                  {{ user.firstName[0] }}{{ user.lastName[0] }}
                </div>
                <div>
                  <span class="tour-name block">{{ user.firstName }} {{ user.lastName }}</span>
                  <span v-if="user.email === authStore.user?.email" class="text-xs bg-blue-100 text-blue-700 px-1.5 py-0.5 rounded font-medium">You</span>
                </div>
              </div>
            </td>
            <td>
              <span class="customer-email">{{ user.email }}</span>
            </td>
            <td>
              <span>{{ user.phoneNumber || '—' }}</span>
            </td>
            <td>
              <div class="flex flex-wrap gap-1.5">
                <span 
                  v-for="role in user.roles" 
                  :key="role" 
                  :class="getRoleClass(role)"
                >
                  {{ role }}
                </span>
              </div>
            </td>
            <td>
              <div class="actions justify-end">
                <button @click="openEditModal(user)" class="btn-edit-action" title="Edit User">✏️</button>
                <button 
                  @click="deleteUser(user)" 
                  class="btn-delete-action" 
                  :disabled="user.email === authStore.user?.email"
                  :class="{ 'opacity-30 cursor-not-allowed': user.email === authStore.user?.email }"
                  title="Delete User"
                >
                  🗑️
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="filteredUsers.length === 0" class="empty-state">
        <p>No users found matching the query</p>
      </div>

      <LuxuryPagination
        v-if="filteredUsers.length > 0"
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="filteredUsers.length"
      />
    </div>

    <!-- Edit/Create Modal Overlay -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-card" role="dialog" aria-modal="true" aria-labelledby="user-modal-title" v-dialog="() => showModal = false">
        <div class="modal-header">
          <h3 id="user-modal-title">{{ isEdit ? 'Edit User Profile' : 'Create New User Account' }}</h3>
          <button type="button" @click="showModal = false" class="btn-close" aria-label="Close">✕</button>
        </div>

        <form @submit.prevent="saveUser" class="modal-form">
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div class="form-group">
              <label for="user-first-name">First Name</label>
              <input id="user-first-name" v-model="form.firstName" type="text" placeholder="e.g. John" required />
            </div>
            <div class="form-group">
              <label for="user-last-name">Last Name</label>
              <input id="user-last-name" v-model="form.lastName" type="text" placeholder="e.g. Doe" required />
            </div>
          </div>

          <div class="form-group">
            <label for="user-email">Email Address</label>
            <input id="user-email" v-model="form.email" type="email" placeholder="john.doe@seadoratravel.com" required />
          </div>

          <div class="form-group">
            <label for="user-phone">Phone Number</label>
            <input id="user-phone" v-model="form.phoneNumber" type="tel" placeholder="e.g. +201068940967" />
          </div>

          <div class="form-group">
            <label for="user-password">{{ isEdit ? 'New Password (leave blank to keep current)' : 'Password' }}</label>
            <input 
              id="user-password"
              v-model="form.password" 
              type="password" 
              placeholder="••••••••" 
              :required="!isEdit"
            />
          </div>

          <div class="form-group border-t border-[#E2E8F0] pt-4 mt-2">
            <span class="mb-2 block form-group-caption">Assigned Roles</span>
            <div class="flex flex-col sm:flex-row gap-4 mt-2">
              <label v-for="role in roles" :key="role" class="flex items-center gap-2 cursor-pointer text-sm normal-case font-normal text-slate-700">
                <input 
                  type="checkbox" 
                  :value="role" 
                  v-model="form.roles"
                  class="w-4 h-4 text-blue-600 rounded border-gray-300 focus:ring-blue-500"
                />
                {{ role }}
              </label>
            </div>
            <p v-if="form.roles.length === 0" class="text-xs text-red-500 mt-1">Please select at least one role.</p>
          </div>

          <div class="modal-actions border-t border-[#E2E8F0] pt-4 mt-4">
            <button type="button" @click="showModal = false" class="btn-cancel" :disabled="actionLoading">Cancel</button>
            <button type="submit" class="btn-save" :disabled="actionLoading || form.roles.length === 0">
              {{ actionLoading ? 'Saving...' : 'Save User' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.users-page {
  width: 100%;
}

.stats-card {
  display: flex;
  align-items: center;
  gap: 16px;
  background: #FFFFFF;
  border: 1px solid #E2E8F0;
  border-radius: 4px;
  padding: 20px;
  box-shadow: 0px 2px 4px rgba(0, 0, 0, 0.02);
}

.stats-icon {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  flex-shrink: 0;
}

.stats-info {
  display: flex;
  flex-direction: column;
}

.stats-label {
  font-size: 12px;
  font-weight: 600;
  color: #64748B;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.stats-value {
  font-size: 24px;
  font-weight: 700;
  color: #1C2434;
}

.actions {
  display: flex;
  gap: 8px;
}

.btn-edit-action, .btn-delete-action {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 18px;
  padding: 6px;
  border-radius: 4px;
  transition: all 0.2s;
}

/* Custom Role Badges */
.role-badge-admin {
  background: rgba(220, 38, 38, 0.1);
  color: #dc2626;
  border: 1px solid rgba(220, 38, 38, 0.2);
  padding: 3px 8px;
  border-radius: 4px;
  font-size: 11px;
  font-weight: 600;
  display: inline-block;
}

.role-badge-manager {
  background: rgba(37, 99, 235, 0.1);
  color: #2563eb;
  border: 1px solid rgba(37, 99, 235, 0.2);
  padding: 3px 8px;
  border-radius: 4px;
  font-size: 11px;
  font-weight: 600;
  display: inline-block;
}

.role-badge-customer {
  background: rgba(5, 150, 105, 0.1);
  color: #059669;
  border: 1px solid rgba(5, 150, 105, 0.2);
  padding: 3px 8px;
  border-radius: 4px;
  font-size: 11px;
  font-weight: 600;
  display: inline-block;
}

/* Modal Overlay & Card layout override */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  padding: 20px;
}

.modal-card {
  background: #FFFFFF;
  border: 1px solid #E2E8F0;
  border-radius: 4px;
  width: 100%;
  max-width: 550px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  box-shadow: 0px 10px 25px -5px rgba(0, 0, 0, 0.1);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #E2E8F0;
}

.modal-header h3 {
  font-size: 18px;
  font-weight: 700;
  color: #1C2434;
}

.btn-close {
  background: none;
  border: none;
  color: #64748B;
  cursor: pointer;
  font-size: 18px;
}

.modal-form {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  max-height: 80vh;
  overflow-y: auto;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  color: #64748B;
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.form-group .form-group-caption {
  color: #64748B;
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.form-group input {
  padding: 10px 12px;
  background: #FFFFFF;
  border: 1px solid #E2E8F0;
  border-radius: 4px;
  color: #24303F;
  outline: none;
  font-size: 14px;
  width: 100%;
}

.form-group input:focus {
  border-color: #3C50E0;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>
