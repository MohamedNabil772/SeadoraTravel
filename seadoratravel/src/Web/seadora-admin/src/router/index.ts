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
          path: 'bookings',
          name: 'bookings',
          component: () => import('../features/bookings/views/BookingsView.vue')
        },
        {
          path: 'bookings/:id/details',
          name: 'booking-details',
          component: () => import('../features/bookings/views/BookingDetailsView.vue'),
          meta: { title: 'Booking Details' }
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
