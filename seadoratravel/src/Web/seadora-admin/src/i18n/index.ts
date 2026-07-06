import { createI18n } from 'vue-i18n'
import en from './en.json'
import fr from './fr.json'
import de from './de.json'
import it from './it.json'
import ru from './ru.json'

const i18n = createI18n({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
  messages: {
    en,
    fr,
    de,
    it,
    ru
  }
})

export default i18n
