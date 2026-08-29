
import { useAuthStore } from '@/features/auth/store/auth';
import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import FeedbackView from '../features/feedback/views/FeedbackView.vue'
import TourDetailsView from '../features/tours/views/TourDetailsView.vue'
import ToursView from '../features/tours/views/ToursView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/portal',
      component: () => import('../features/portal/layouts/CustomerPortalLayout.vue'),
      meta: { requiresAuth: true },
      children: [
        { path: '', alias: ['dashboard', '/portal/dashboard'], name: 'portal-dashboard', component: () => import('../features/portal/views/PortalDashboardView.vue') },
        { path: 'favorites', name: 'portal-favorites', component: () => import('../features/portal/views/PortalFavoritesView.vue') },
        { path: 'bookings', name: 'portal-bookings', component: () => import('../features/portal/views/PortalBookingsView.vue') },
        { path: 'bookings/:id', name: 'portal-booking-detail', component: () => import('../features/portal/views/PortalBookingDetailView.vue') },
        { path: 'documents', name: 'portal-documents', component: () => import('../features/portal/views/PortalDocumentsView.vue') },
        { path: 'profile', name: 'portal-profile', component: () => import('../features/portal/views/PortalProfileView.vue') },
        { path: 'support', name: 'portal-support', component: () => import('../features/portal/views/PortalSupportView.vue') }
      ]
    },
    {
      path: '/dashboard',
      redirect: '/portal'
    },
    {
      path: '/portal/dashboard',
      redirect: '/portal'
    },
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/coming-soon',
      name: 'coming-soon',
      component: () => import('../views/ComingSoonView.vue')
    },
    {
      path: '/tours',
      name: 'tours',
      component: ToursView
    },
    {
      path: '/feedback',
      name: 'feedback',
      component: FeedbackView
    },
    {
      path: '/tour/:slug',
      name: 'tour-details',
      component: TourDetailsView,
      props: true
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/'
    }
  ]
})

export default router

router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore();
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    authStore.openAuthModal();
    next({ path: '/', query: { redirect: to.fullPath } });
  } else {
    next();
  }
});
