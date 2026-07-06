import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import FeedbackView from '../views/FeedbackView.vue'
import TourDetailsView from '../views/TourDetailsView.vue'
import ToursView from '../views/ToursView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
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
