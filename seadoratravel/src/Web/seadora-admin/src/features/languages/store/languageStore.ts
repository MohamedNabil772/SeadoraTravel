import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface Language {
  code: string;
  name: string;
  flag: string;
  isRtl: boolean;
  isDefault: boolean;
}

export interface TranslationItem {
  key: string;
  namespace: string;
  values: Record<string, string>;
}

export const useLanguageStore = defineStore('languages', () => {
  const languages = ref<Language[]>([
    { code: 'en', name: 'English', flag: '🇬🇧', isRtl: false, isDefault: true },
    { code: 'ar', name: 'Arabic', flag: '🇸🇦', isRtl: true, isDefault: false },
    { code: 'es', name: 'Spanish', flag: '🇪🇸', isRtl: false, isDefault: false }
  ])

  const translations = ref<TranslationItem[]>([
    { key: 'welcome_message', namespace: 'common', values: { en: 'Welcome', ar: 'مرحباً', es: 'Bienvenido' } },
    { key: 'save', namespace: 'common', values: { en: 'Save', ar: 'حفظ', es: 'Guardar' } },
    { key: 'cancel', namespace: 'common', values: { en: 'Cancel', ar: 'إلغاء', es: 'Cancelar' } },
    { key: 'dashboard_title', namespace: 'dashboard', values: { en: 'Dashboard Overview', ar: 'نظرة عامة على لوحة القيادة', es: 'Descripción general del panel' } },
    { key: 'total_bookings', namespace: 'dashboard', values: { en: 'Total Bookings', ar: 'إجمالي الحجوزات', es: 'Reservas totales' } },
  ])

  let saveTimeout: ReturnType<typeof setTimeout> | null = null;
  const isSaving = ref(false);

  const updateTranslation = (key: string, namespace: string, langCode: string, value: string) => {
    const item = translations.value.find(t => t.key === key && t.namespace === namespace);
    if (item) {
      item.values[langCode] = value;
      triggerAutoSave();
    }
  };

  const triggerAutoSave = () => {
    isSaving.value = true;
    if (saveTimeout) clearTimeout(saveTimeout);
    saveTimeout = setTimeout(() => {
      // Mock API call to save translations
      console.log('Saved translations to server...');
      isSaving.value = false;
    }, 500);
  };

  const addLanguage = (lang: Language) => {
    languages.value.push(lang);
    translations.value.forEach(t => {
      t.values[lang.code] = '';
    });
  };

  const importTranslations = (jsonString: string, langCode: string) => {
    try {
      const parsed = JSON.parse(jsonString);
      Object.keys(parsed).forEach(key => {
        const existing = translations.value.find(t => t.key === key);
        if (existing) {
          existing.values[langCode] = parsed[key];
        } else {
          translations.value.push({
            key,
            namespace: 'common',
            values: { [langCode]: parsed[key] }
          });
        }
      });
      triggerAutoSave();
    } catch (e) {
      console.error('Failed to import JSON', e);
    }
  };

  const exportTranslations = (langCode: string) => {
    const exportData: Record<string, string> = {};
    translations.value.forEach(t => {
      exportData[t.key] = t.values[langCode] || '';
    });
    return JSON.stringify(exportData, null, 2);
  };

  const getLanguageProgress = (langCode: string) => {
    if (translations.value.length === 0) return 0;
    const filled = translations.value.filter(t => !!t.values[langCode]).length;
    return Math.round((filled / translations.value.length) * 100);
  };

  return {
    languages,
    translations,
    isSaving,
    updateTranslation,
    addLanguage,
    importTranslations,
    exportTranslations,
    getLanguageProgress
  }
})
