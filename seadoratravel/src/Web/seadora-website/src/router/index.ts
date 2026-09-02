
import { useAuthStore } from '@/features/auth/store/auth';
import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import FeedbackView from '../features/feedback/views/FeedbackView.vue'
import TourDetailsView from '../features/tours/views/TourDetailsView.vue'
import ToursView from '../features/tours/views/ToursView.vue'
import i18n, { loadLanguageAsync } from '@/i18n'
import { applyRouteSeo } from '@/shared/utils/seo'

/**
 * Optional locale prefix on all public content routes.
 * Default locale (en) lives on bare paths (/tours); the other locales are
 * served under /fr/, /de/, /it/, /ru/ so search engines can index and
 * cross-link each language (see hreflang in shared/utils/seo.ts).
 */
const LOCALE_PREFIX = ':locale(en|fr|de|it|ru)?'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/portal',
      component: () => import('../features/portal/layouts/CustomerPortalLayout.vue'),
      meta: { requiresAuth: true, noindex: true },
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
      path: `/${LOCALE_PREFIX}`,
      name: 'home',
      component: HomeView
    },
    {
      path: `/${LOCALE_PREFIX}/coming-soon`,
      name: 'coming-soon',
      component: () => import('../views/ComingSoonView.vue')
    },
    {
      path: `/${LOCALE_PREFIX}/tours`,
      name: 'tours',
      component: ToursView
    },
    {
      path: `/${LOCALE_PREFIX}/feedback`,
      name: 'feedback',
      component: FeedbackView
    },
    {
      path: `/${LOCALE_PREFIX}/tour/:slug`,
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

router.beforeEach(async (to, _from, next) => {
  const authStore = useAuthStore();
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    authStore.openAuthModal();
    next({ path: '/', query: { redirect: to.fullPath } });
  } else {
    // Sync the i18n locale with the URL locale prefix (/fr/, /de/, ...)
    const localeParam = to.params.locale as string | undefined;
    const currentLocale = (i18n.global.locale as any).value;
    if (localeParam && localeParam !== currentLocale) {
      await loadLanguageAsync(localeParam);
    }
    next();
  }
});

router.afterEach((to) => {
  applyRouteSeo(to);
});
