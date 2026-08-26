<script setup lang="ts">
import { ref, computed } from 'vue'
import { Shield, Plus, Edit2, Trash2, KeyRound, Users, X, Search, Sparkles } from 'lucide-vue-next'
import LuxuryPagination from '@/shared/components/LuxuryPagination.vue'
import { useToast } from '@/composables/useToast'

const toast = useToast()

export interface RolePermissionDto {
  module: string
  label: string
  canView: boolean
  canCreate: boolean
  canEdit: boolean
  canDelete: boolean
  canManageAccess: boolean
}

export interface ApplicationRole {
  id: string
  name: string
  description: string
  isSystemRole: boolean
  userCount: number
  permissions: RolePermissionDto[]
}

const defaultModules: { module: string; label: string }[] = [
  { module: 'Dashboard', label: 'Dashboard & Executive Metrics' },
  { module: 'Tours', label: 'Tours & Experiences Management' },
  { module: 'Destinations', label: 'Destinations & Geographic Locations' },
  { module: 'Categories', label: 'Experience Categories & Tags' },
  { module: 'Bookings', label: 'Bookings, Guest Lists & Vouchers' },
  { module: 'Inquiries', label: 'VIP Concierge Inquiries & Messaging' },
  { module: 'Users', label: 'Admin Users & Staff Accounts' },
  { module: 'Roles', label: 'Role & Module Access (RBAC)' },
  { module: 'Settings', label: 'Currencies, Languages & System Settings' },
]

const roles = ref<ApplicationRole[]>([
  {
    id: '1',
    name: 'SuperAdmin',
    description: 'Full unrestricted access to all platform operations, finances, and system settings.',
    isSystemRole: true,
    userCount: 2,
    permissions: defaultModules.map((m) => ({
      module: m.module,
      label: m.label,
      canView: true,
      canCreate: true,
      canEdit: true,
      canDelete: true,
      canManageAccess: true,
    })),
  },
  {
    id: '2',
    name: 'OperationsManager',
    description: 'Oversees scheduled tours, booking vouchers, drivers, and daily logistics.',
    isSystemRole: false,
    userCount: 3,
    permissions: defaultModules.map((m) => ({
      module: m.module,
      label: m.label,
      canView: true,
      canCreate: ['Tours', 'Bookings', 'Inquiries'].includes(m.module),
      canEdit: ['Tours', 'Bookings', 'Inquiries'].includes(m.module),
      canDelete: false,
      canManageAccess: false,
    })),
  },
  {
    id: '3',
    name: 'ConciergeSpecialist',
    description: 'Handles guest inquiries, customer WhatsApp communications, and hotel pickups.',
    isSystemRole: false,
    userCount: 4,
    permissions: defaultModules.map((m) => ({
      module: m.module,
      label: m.label,
      canView: ['Dashboard', 'Bookings', 'Inquiries'].includes(m.module),
      canCreate: ['Inquiries'].includes(m.module),
      canEdit: ['Bookings', 'Inquiries'].includes(m.module),
      canDelete: false,
      canManageAccess: false,
    })),
  },
])

const searchQuery = ref('')
const currentPage = ref(1)
const pageSize = ref(10)

const filteredRoles = computed(() => {
  if (!searchQuery.value.trim()) return roles.value
  const q = searchQuery.value.toLowerCase()
  return roles.value.filter(
    (r) => r.name.toLowerCase().includes(q) || r.description.toLowerCase().includes(q)
  )
})

const paginatedRoles = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredRoles.value.slice(start, start + pageSize.value)
})

// Modal states
const showRoleModal = ref(false)
const showPermissionsModal = ref(false)
const isEditing = ref(false)
const activeRole = ref<ApplicationRole | null>(null)

const roleForm = ref({
  id: '',
  name: '',
  description: '',
})

const permissionsForm = ref<RolePermissionDto[]>([])

function openCreateModal() {
  isEditing.value = false
  roleForm.value = { id: '', name: '', description: '' }
  permissionsForm.value = defaultModules.map((m) => ({
    module: m.module,
    label: m.label,
    canView: true,
    canCreate: false,
    canEdit: false,
    canDelete: false,
    canManageAccess: false,
  }))
  showRoleModal.value = true
}

function openEditModal(role: ApplicationRole) {
  isEditing.value = true
  activeRole.value = role
  roleForm.value = {
    id: role.id,
    name: role.name,
    description: role.description,
  }
  showRoleModal.value = true
}

function openPermissionsMatrix(role: ApplicationRole) {
  activeRole.value = role
  permissionsForm.value = JSON.parse(JSON.stringify(role.permissions))
  showPermissionsModal.value = true
}

function saveRole() {
  if (!roleForm.value.name.trim()) {
    toast.error('Role name is required')
    return
  }

  if (isEditing.value && activeRole.value) {
    activeRole.value.name = roleForm.value.name
    activeRole.value.description = roleForm.value.description
    toast.success(`Role '${roleForm.value.name}' updated successfully`)
  } else {
    const newRole: ApplicationRole = {
      id: Date.now().toString(),
      name: roleForm.value.name,
      description: roleForm.value.description,
      isSystemRole: false,
      userCount: 0,
      permissions: permissionsForm.value,
    }
    roles.value.push(newRole)
    toast.success(`Role '${newRole.name}' created successfully`)
  }
  showRoleModal.value = false
}

function savePermissions() {
  if (activeRole.value) {
    activeRole.value.permissions = JSON.parse(JSON.stringify(permissionsForm.value))
    toast.success(`Permissions updated for '${activeRole.value.name}'`)
  }
  showPermissionsModal.value = false
}

function deleteRole(role: ApplicationRole) {
  if (role.isSystemRole) {
    toast.error('System protected roles cannot be deleted')
    return
  }
  if (role.userCount > 0) {
    toast.error(`Cannot delete role with ${role.userCount} active assigned users`)
    return
  }
  if (confirm(`Are you sure you want to delete the role '${role.name}'?`)) {
    roles.value = roles.value.filter((r) => r.id !== role.id)
    toast.success(`Role '${role.name}' removed`)
  }
}

function toggleAllInRow(perm: RolePermissionDto, value: boolean) {
  perm.canView = value
  perm.canCreate = value
  perm.canEdit = value
  perm.canDelete = value
  perm.canManageAccess = value
}

function toggleAllInColumn(action: keyof Omit<RolePermissionDto, 'module' | 'label'>, value: boolean) {
  permissionsForm.value.forEach((p) => {
    p[action] = value
  })
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header with Action -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div>
        <h1 class="text-2xl font-serif font-bold text-text-main flex items-center gap-2">
          <Shield class="w-6 h-6 text-secondary-text" />
          Roles & Permissions Matrix (RBAC)
        </h1>
        <p class="text-sm text-text-muted mt-1">
          Manage system roles, module-level privileges, and granular access controls.
        </p>
      </div>

      <button
        @click="openCreateModal"
        class="btn-create"
      >
        <Plus class="w-4 h-4" />
        <span>Create New Role</span>
      </button>
    </div>

    <!-- Filter & Search Bar -->
    <div class="flex items-center gap-4 bg-white p-4 rounded-xl border border-border/60 shadow-sm">
      <div class="relative flex-1">
        <Search class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-text-muted" />
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search roles by title or description..."
          aria-label="Search roles"
          class="w-full pl-10 pr-4 py-2 text-sm bg-surface-sunken border border-border/70 rounded-lg focus:outline-none focus:ring-2 focus:ring-secondary/30 focus:border-secondary/40 transition-colors"
        />
      </div>
    </div>

    <!-- Roles Grid Table -->
    <div class="bg-white rounded-xl border border-border/60 shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left text-sm border-collapse">
          <thead>
            <tr class="bg-surface-sunken/70 text-text-muted font-bold text-xs uppercase tracking-wider border-b border-border/60">
              <th class="py-3.5 px-6">Role Name & Badge</th>
              <th class="py-3.5 px-6">Description</th>
              <th class="py-3.5 px-6 text-center">Assigned Users</th>
              <th class="py-3.5 px-6 text-center">Privileges Preview</th>
              <th class="py-3.5 px-6 text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-border/60">
            <tr
              v-for="role in paginatedRoles"
              :key="role.id"
              class="hover:bg-surface-sunken/40 transition-colors group"
            >
              <td class="py-4 px-6">
                <div class="flex items-center gap-3">
                  <div class="w-9 h-9 rounded-lg bg-secondary/10 border border-secondary/20 flex items-center justify-center text-secondary font-bold text-sm">
                    <Shield class="w-4 h-4 text-secondary-dark" />
                  </div>
                  <div>
                    <span class="font-bold text-text-main block">{{ role.name }}</span>
                    <span
                      v-if="role.isSystemRole"
                      class="inline-flex items-center gap-1 text-[10px] font-bold text-amber-700 bg-amber-50 px-2 py-0.5 rounded-full border border-amber-200"
                    >
                      <Sparkles class="w-2.5 h-2.5" /> Protected System Role
                    </span>
                    <span
                      v-else
                      class="inline-flex items-center text-[10px] font-bold text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-full border border-emerald-200"
                    >
                      Custom Role
                    </span>
                  </div>
                </div>
              </td>
              <td class="py-4 px-6 text-text-muted max-w-xs text-xs">
                {{ role.description }}
              </td>
              <td class="py-4 px-6 text-center">
                <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-surface-sunken text-xs font-semibold text-text-main border border-border/60">
                  <Users class="w-3.5 h-3.5 text-secondary-text" />
                  {{ role.userCount }}
                </span>
              </td>
              <td class="py-4 px-6 text-center">
                <button
                  @click="openPermissionsMatrix(role)"
                  class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-primary/5 hover:bg-primary/10 text-primary text-xs font-bold transition-colors cursor-pointer border border-primary/15"
                >
                  <KeyRound class="w-3.5 h-3.5 text-secondary-text" />
                  <span>Configure Matrix</span>
                </button>
              </td>
              <td class="py-4 px-6 text-right space-x-2">
                <button
                  @click="openEditModal(role)"
                  title="Edit Role Details"
                  class="p-1.5 rounded-md hover:bg-surface-sunken text-text-muted hover:text-text-main transition-colors cursor-pointer"
                >
                  <Edit2 class="w-4 h-4" />
                </button>
                <button
                  v-if="!role.isSystemRole"
                  @click="deleteRole(role)"
                  title="Delete Role"
                  class="p-1.5 rounded-md hover:bg-red-50 text-text-muted hover:text-red-600 transition-colors cursor-pointer"
                >
                  <Trash2 class="w-4 h-4" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Luxury Pagination Component Integration -->
      <LuxuryPagination
        v-model:currentPage="currentPage"
        v-model:pageSize="pageSize"
        :totalItems="filteredRoles.length"
      />
    </div>

    <!-- CREATE / EDIT ROLE MODAL -->
    <div
      v-if="showRoleModal"
      class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4"
    >
      <div class="bg-white rounded-2xl max-w-md w-full p-6 shadow-2xl border border-border/80 space-y-5 animate-fade-in" role="dialog" aria-modal="true" aria-labelledby="role-modal-title" v-dialog="() => showRoleModal = false">
        <div class="flex items-center justify-between border-b border-border/60 pb-4">
          <h3 id="role-modal-title" class="text-lg font-serif font-bold text-text-main flex items-center gap-2">
            <Shield class="w-5 h-5 text-secondary-dark" />
            {{ isEditing ? 'Edit Role Details' : 'Create New Role' }}
          </h3>
          <button type="button" @click="showRoleModal = false" aria-label="Close" class="p-1 text-text-muted hover:text-text-main rounded-md">
            <X class="w-5 h-5" />
          </button>
        </div>

        <div class="space-y-4">
          <div>
            <label for="role-name" class="block text-xs font-bold uppercase text-text-muted mb-1.5">Role Name</label>
            <input
              id="role-name"
              v-model="roleForm.name"
              type="text"
              placeholder="e.g. ConciergeManager"
              class="w-full px-3.5 py-2 text-sm bg-surface-sunken border border-border/70 rounded-lg focus:outline-none focus:ring-2 focus:ring-secondary/30 focus:border-secondary"
            />
          </div>

          <div>
            <label for="role-description" class="block text-xs font-bold uppercase text-text-muted mb-1.5">Description</label>
            <textarea
              id="role-description"
              v-model="roleForm.description"
              rows="3"
              placeholder="Brief summary of duties and responsibilities..."
              class="w-full px-3.5 py-2 text-sm bg-surface-sunken border border-border/70 rounded-lg focus:outline-none focus:ring-2 focus:ring-secondary/30 focus:border-secondary resize-none"
            ></textarea>
          </div>
        </div>

        <div class="flex items-center justify-end gap-3 pt-3 border-t border-border/60">
          <button
            type="button"
            @click="showRoleModal = false"
            class="px-4 py-2 text-sm font-semibold text-text-muted hover:text-text-main rounded-lg"
          >
            Cancel
          </button>
          <button
            type="button"
            @click="saveRole"
            class="px-5 py-2 text-sm font-bold bg-secondary text-primary rounded-lg shadow-sm hover:brightness-105"
          >
            {{ isEditing ? 'Save Changes' : 'Create Role' }}
          </button>
        </div>
      </div>
    </div>

    <!-- PERMISSIONS MATRIX MODAL -->
    <div
      v-if="showPermissionsModal"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4"
    >
      <div class="bg-white rounded-2xl max-w-4xl w-full p-6 shadow-2xl border border-border/80 space-y-5 max-h-[90vh] flex flex-col" role="dialog" aria-modal="true" aria-labelledby="permissions-modal-title" v-dialog="() => showPermissionsModal = false">
        <!-- Header -->
        <div class="flex items-center justify-between border-b border-border/60 pb-4">
          <div>
            <h3 id="permissions-modal-title" class="text-lg font-serif font-bold text-text-main flex items-center gap-2">
              <KeyRound class="w-5 h-5 text-secondary-dark" />
              Module Permissions Matrix — <span class="text-secondary-text">{{ activeRole?.name }}</span>
            </h3>
            <p class="text-xs text-text-muted mt-0.5">Toggle granular CRUD and administrative capabilities per module.</p>
          </div>
          <button type="button" @click="showPermissionsModal = false" aria-label="Close" class="p-1 text-text-muted hover:text-text-main rounded-md">
            <X class="w-5 h-5" />
          </button>
        </div>

        <!-- Matrix Table -->
        <div class="flex-1 overflow-y-auto border border-border/70 rounded-xl">
          <table class="w-full text-left text-sm border-collapse">
            <thead class="sticky top-0 bg-surface-sunken text-text-muted font-bold text-xs uppercase tracking-wider z-10 border-b border-border/70">
              <tr>
                <th class="py-3 px-4">Module / Section</th>
                <th class="py-3 px-3 text-center">
                  <div class="flex flex-col items-center">
                    <span>View</span>
                    <button @click="toggleAllInColumn('canView', true)" class="text-[9px] text-secondary-text hover:underline cursor-pointer">All</button>
                  </div>
                </th>
                <th class="py-3 px-3 text-center">
                  <div class="flex flex-col items-center">
                    <span>Create</span>
                    <button @click="toggleAllInColumn('canCreate', true)" class="text-[9px] text-secondary-text hover:underline cursor-pointer">All</button>
                  </div>
                </th>
                <th class="py-3 px-3 text-center">
                  <div class="flex flex-col items-center">
                    <span>Edit</span>
                    <button @click="toggleAllInColumn('canEdit', true)" class="text-[9px] text-secondary-text hover:underline cursor-pointer">All</button>
                  </div>
                </th>
                <th class="py-3 px-3 text-center">
                  <div class="flex flex-col items-center">
                    <span>Delete</span>
                    <button @click="toggleAllInColumn('canDelete', true)" class="text-[9px] text-secondary-text hover:underline cursor-pointer">All</button>
                  </div>
                </th>
                <th class="py-3 px-3 text-center">
                  <div class="flex flex-col items-center">
                    <span>Full Admin</span>
                    <button @click="toggleAllInColumn('canManageAccess', true)" class="text-[9px] text-secondary-text hover:underline cursor-pointer">All</button>
                  </div>
                </th>
                <th class="py-3 px-3 text-right">Row Quick</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-border/60">
              <tr
                v-for="perm in permissionsForm"
                :key="perm.module"
                class="hover:bg-surface-sunken/30 transition-colors"
              >
                <td class="py-3.5 px-4 font-semibold text-text-main">
                  <span class="block">{{ perm.label }}</span>
                  <span class="text-[10px] text-text-muted font-mono font-normal">({{ perm.module }})</span>
                </td>
                <td class="py-3.5 px-3 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.canView"
                    :aria-label="`${perm.label} - View`"
                    class="w-4 h-4 rounded text-secondary focus:ring-secondary/40 cursor-pointer"
                  />
                </td>
                <td class="py-3.5 px-3 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.canCreate"
                    :aria-label="`${perm.label} - Create`"
                    class="w-4 h-4 rounded text-secondary focus:ring-secondary/40 cursor-pointer"
                  />
                </td>
                <td class="py-3.5 px-3 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.canEdit"
                    :aria-label="`${perm.label} - Edit`"
                    class="w-4 h-4 rounded text-secondary focus:ring-secondary/40 cursor-pointer"
                  />
                </td>
                <td class="py-3.5 px-3 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.canDelete"
                    :aria-label="`${perm.label} - Delete`"
                    class="w-4 h-4 rounded text-secondary focus:ring-secondary/40 cursor-pointer"
                  />
                </td>
                <td class="py-3.5 px-3 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.canManageAccess"
                    :aria-label="`${perm.label} - Full Admin`"
                    class="w-4 h-4 rounded text-secondary focus:ring-secondary/40 cursor-pointer"
                  />
                </td>
                <td class="py-3.5 px-3 text-right space-x-1">
                  <button
                    @click="toggleAllInRow(perm, true)"
                    class="px-2 py-0.5 text-[10px] bg-secondary/15 text-secondary-text rounded font-bold hover:bg-secondary/25"
                  >
                    Grant
                  </button>
                  <button
                    @click="toggleAllInRow(perm, false)"
                    class="px-2 py-0.5 text-[10px] bg-gray-100 text-text-muted rounded font-bold hover:bg-gray-200"
                  >
                    Clear
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Footer -->
        <div class="flex items-center justify-between pt-3 border-t border-border/60">
          <span class="text-xs text-text-muted italic">Changes take effect across the platform for users with this role.</span>
          <div class="flex items-center gap-3">
            <button
              @click="showPermissionsModal = false"
              class="px-4 py-2 text-sm font-semibold text-text-muted hover:text-text-main rounded-lg"
            >
              Cancel
            </button>
            <button
              @click="savePermissions"
              class="px-5 py-2 text-sm font-bold bg-secondary text-primary rounded-lg shadow-sm hover:brightness-105"
            >
              Save Permissions Matrix
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

