import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import FeedbackView from '../features/feedback/views/FeedbackView.vue'
import TourDetailsView from '../features/tours/views/TourDetailsView.vue'
import ToursView from '../features/tours/views/ToursView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
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
    }
  ]
})

export default router
