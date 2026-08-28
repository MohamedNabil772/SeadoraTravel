import { createRouter, createWebHistory } from 'vue-router'
import DashboardLayout from '../layouts/DashboardLayout.vue'
import DashboardView from '../views/DashboardView.vue'
import LoginView from '../features/auth/views/LoginView.vue'
import { useAuthStore } from '../features/auth/store/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'login',
      component: LoginView,
      meta: { requiresAuth: false }
    },
    {
      path: '/',
      component: DashboardLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: 'dashboard',
          name: 'dashboard',
          component: DashboardView
        },
        {
          path: 'tours',
          name: 'tours',
          component: () => import('../features/tours/views/ToursView.vue')
        },
        {
          path: 'tours/create',
          name: 'tour-create',
          component: () => import('../features/tours/views/TourBuilderView.vue'),
          meta: { title: 'Create Tour' }
        },
        {
          path: 'tours/:id/edit',
          name: 'tour-edit',
          component: () => import('../features/tours/views/TourBuilderView.vue'),
          meta: { title: 'Edit Tour' }
        },
        {
          path: 'destinations',
          name: 'destinations',
          component: () => import('../features/destinations/views/DestinationsView.vue'),
          meta: { title: 'Destinations' }
        },
        {
          path: 'categories',
          name: 'categories',
          component: () => import('../features/categories/views/CategoriesView.vue'),
          meta: { title: 'Categories' }
        },
        {
          path: 'tour-types',
          name: 'tour-types',
          component: () => import('../features/tour-types/views/TourTypesView.vue'),
          meta: { title: 'Tour & Trip Types' }
        },
        {
          path: 'bookings',
          name: 'bookings',
          component: () => import('../features/bookings/views/BookingsView.vue')
        },
        {
          path: 'bookings/create',
          name: 'booking-create',
          component: () => import('../features/bookings/views/CreateBookingView.vue'),
          meta: { title: 'Create VIP Booking' }
        },
        {
          path: 'bookings/:id/details',
          name: 'booking-details',
          component: () => import('../features/bookings/views/BookingDetailsView.vue'),
          meta: { title: 'Booking Details' }
        },
        {
          path: 'customers',
          name: 'customers',
          component: () => import('../features/customers/views/CustomersView.vue'),
          meta: { title: 'Customers (CRM)' }
        },
        {
          path: 'customers/:id',
          name: 'customer-details',
          component: () => import('../features/customers/views/CustomerDetailsView.vue'),
          meta: { title: 'Customer Profile' }
        },
        {
          path: 'feedback',
          name: 'feedback',
          component: () => import('../features/feedback/views/FeedbackView.vue')
        },
        {
          path: 'users',
          name: 'users',
          component: () => import('../features/users/views/UsersView.vue')
        },
        {
          path: 'roles',
          name: 'roles',
          component: () => import('../features/roles/views/RolesView.vue'),
          meta: { title: 'Roles & Permissions (RBAC)' }
        },
        {
          path: 'suppliers',
          name: 'suppliers',
          component: () => import('../features/suppliers/views/SuppliersView.vue')
        },
        {
          path: 'reports',
          name: 'reports',
          component: () => import('../features/reports/views/ReportsView.vue')
        },
        {
          path: 'finance/dashboard',
          name: 'finance-dashboard',
          component: () => import('../features/finance/views/FinanceDashboardView.vue'),
          meta: { title: 'Finance Dashboard' }
        },
        {
          path: 'finance/reports',
          name: 'finance-reports',
          component: () => import('../features/finance/views/FinanceReportsView.vue'),
          meta: { title: 'Financial Reports' }
        },
        {
          path: 'finance/payments',
          name: 'finance-payments',
          component: () => import('../features/finance/views/PaymentsView.vue'),
          meta: { title: 'Payments' }
        },
        {
          path: 'settings/languages',
          name: 'languages',
          component: () => import('../features/languages/views/LanguagesView.vue'),
          meta: { title: 'Languages & Localization' }
        },
        {
          path: 'settings/currencies',
          name: 'currencies',
          component: () => import('../features/currencies/views/CurrenciesView.vue'),
          meta: { title: 'Currencies' }
        },
        {
          path: 'settings/nationalities',
          name: 'nationalities',
          component: () => import('../features/nationalities/views/NationalitiesView.vue'),
          meta: { title: 'Nationalities' }
        },
        {
          path: 'inquiries',
          alias: 'contact-requests',
          name: 'inquiries',
          component: () => import('../features/inquiries/views/InquiriesView.vue'),
          meta: { title: 'VIP Inquiries & Contact Requests' }
        },
        {
          path: 'support',
          name: 'support-tickets',
          component: () => import('../features/support/views/SupportTicketsView.vue'),
          meta: { title: 'Service Desk' }
        },
        {
          path: 'support/:id',
          name: 'support-ticket-details',
          component: () => import('../features/support/views/TicketDetailsView.vue'),
          meta: { title: 'Ticket Details' }
        }
      ]
    }
  ]
})

router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()
  authStore.initAuth()

  const requiresAuth = to.matched.some(record => record.meta.requiresAuth !== false)

  if (requiresAuth && !authStore.isAuthenticated) {
    next('/')
  } else if (to.path === '/' && authStore.isAuthenticated) {
    next('/dashboard')
  } else {
    next()
  }
})

export default router
