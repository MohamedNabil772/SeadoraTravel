import { createI18n } from 'vue-i18n'
import en from './locales/en.json'
import fr from './locales/fr.json'
import it from './locales/it.json'
import de from './locales/de.json'
import ru from './locales/ru.json'

const savedLang = localStorage.getItem('seadora_lang') || 'en'

const i18n = createI18n({
  legacy: false,
  locale: savedLang,
  fallbackLocale: 'en',
  messages: {
    en,
    fr,
    it,
    de,
    ru
  }
})

export default i18n
