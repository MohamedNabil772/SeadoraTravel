import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import App from './App.vue'
import router from './router'
import i18n from './i18n'
import { vDialog } from './shared/directives/dialog'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(i18n)
app.directive('dialog', vDialog)

app.mount('#app')
