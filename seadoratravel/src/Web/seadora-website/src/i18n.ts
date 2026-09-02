import { createI18n } from 'vue-i18n'
import en from './locales/en.json'
import fr from './locales/fr.json'
import it from './locales/it.json'
import de from './locales/de.json'
import ru from './locales/ru.json'
import { nextTick } from 'vue'

const savedLang = localStorage.getItem('seadora_lang') || 'en'

const localMessages: Record<string, any> = {
  en,
  fr,
  it,
  de,
  ru
}

export const i18n = createI18n({
  legacy: false,
  locale: savedLang,
  fallbackLocale: 'en',
  messages: localMessages
})

// Deep merge utility
function isObject(item: any) {
  return (item && typeof item === 'object' && !Array.isArray(item));
}

function mergeDeep(target: any, source: any) {
  let output = Object.assign({}, target);
  if (isObject(target) && isObject(source)) {
    Object.keys(source).forEach(key => {
      if (isObject(source[key])) {
        if (!(key in target))
          Object.assign(output, { [key]: source[key] });
        else
          output[key] = mergeDeep(target[key], source[key]);
      } else {
        Object.assign(output, { [key]: source[key] });
      }
    });
  }
  return output;
}

export async function loadLanguageAsync(lang: string) {
  if (!lang) return;

  // Unconditionally set global locale (supports both ref & primitive modes)
  if (typeof (i18n.global.locale as any).value !== 'undefined') {
    (i18n.global.locale as any).value = lang;
  } else {
    (i18n.global.locale as any) = lang;
  }
  localStorage.setItem('seadora_lang', lang);

  // Keep the document language attribute in sync for SEO & accessibility
  if (typeof document !== 'undefined') {
    document.documentElement.setAttribute('lang', lang);
  }

  try {
    const response = await fetch(`/api/content/api/v1/languages/${lang}/translations`)
    if (response.ok) {
      const overrides = await response.json()
      const mergedMessages = mergeDeep(localMessages[lang] || {}, overrides)
      i18n.global.setLocaleMessage(lang, mergedMessages)
    }
  } catch (error) {
    // Local JSON fallback is already active
  }

  return nextTick()
}

// Trigger initial fetch
loadLanguageAsync(savedLang)

export default i18n
