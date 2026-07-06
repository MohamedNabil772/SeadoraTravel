import { createRouter, createWebHistory } from 'vue-router'
import DashboardLayout from '../layouts/DashboardLayout.vue'
import DashboardView from '../views/DashboardView.vue'
import LoginView from '../views/LoginView.vue'
import { useAuthStore } from '../stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
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
          path: '',
          name: 'dashboard',
          component: DashboardView
        },
        {
          path: 'tours',
          name: 'tours',
          component: () => import('../views/ToursView.vue')
        },
        {
          path: 'destinations',
          name: 'destinations',
          component: () => import('../views/DestinationsView.vue')
        },
        {
          path: 'categories',
          name: 'categories',
          component: () => import('../views/CategoriesView.vue')
        },
        {
          path: 'bookings',
          name: 'bookings',
          component: () => import('../views/BookingsView.vue')
        },
        {
          path: 'feedback',
          name: 'feedback',
          component: () => import('../views/FeedbackView.vue')
        },
        {
          path: 'users',
          name: 'users',
          component: () => import('../views/UsersView.vue')
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
    next('/login')
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router
