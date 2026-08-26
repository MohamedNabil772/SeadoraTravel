import { computed } from 'vue'
import { useAuthStore } from '@/features/auth/store/auth'

export type PermissionAction = 'View' | 'Create' | 'Edit' | 'Delete' | 'ManageAccess' | 'Export'

export interface ModulePermission {
  module: string
  action: PermissionAction
}

export function usePermissions() {
  const auth = useAuthStore()

  const isSuperAdmin = computed(() => {
    return auth.user?.roles?.includes('SuperAdmin') || auth.user?.roles?.includes('Admin') || false
  })

  const userRoles = computed(() => auth.user?.roles || [])

  function hasRole(roleName: string): boolean {
    if (isSuperAdmin.value) return true
    return userRoles.value.includes(roleName)
  }

  function hasPermission(module: string, action: PermissionAction = 'View'): boolean {
    if (isSuperAdmin.value) return true
    // If specific permissions array exists on user profile/token
    const permissions: string[] = (auth.user as any)?.permissions || []
    if (permissions.length > 0) {
      return permissions.includes(`${module}.${action}`) || permissions.includes(`${module}.*`) || permissions.includes('*')
    }
    // Default fallback based on roles
    if (userRoles.value.includes('BookingManager')) {
      return ['Dashboard', 'Tours', 'Bookings', 'Inquiries'].includes(module)
    }
    if (userRoles.value.includes('ConciergeSpecialist')) {
      return ['Dashboard', 'Bookings', 'Inquiries'].includes(module) && (action === 'View' || action === 'Edit')
    }
    return false
  }

  function canAccessModule(module: string): boolean {
    return hasPermission(module, 'View')
  }

  return {
    isSuperAdmin,
    userRoles,
    hasRole,
    hasPermission,
    canAccessModule,
  }
}
