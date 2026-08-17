import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { MotionPlugin } from '@vueuse/motion'
import './style.css'
import App from './App.vue'
import router from './router'
import i18n from './i18n'

const app = createApp(App)
app.use(MotionPlugin)

// Custom directive for scroll reveal animations (L2 Fluid Interactive)
app.directive('reveal', {
  mounted(el, binding) {
    const animationClass = binding.value || 'reveal-fade-up';
    el.classList.add(animationClass);
    
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('in-view');
          observer.unobserve(entry.target);
        }
      });
    }, { 
      threshold: 0.10,
      rootMargin: '0px 0px -50px 0px' // Trigger slightly before it enters the viewport
    });
    observer.observe(el);
  }
});

app.use(createPinia())
app.use(router)
app.use(i18n)

app.mount('#app')

