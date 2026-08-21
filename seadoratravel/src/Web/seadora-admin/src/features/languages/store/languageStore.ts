import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '@/services/api'

export interface Language {
  id: string;
  code: string;
  name: string;
  nativeName?: string;
  flagEmoji?: string;
  flag?: string; // alias/fallback
  isRtl: boolean;
  isDefault: boolean;
  order?: number;
  isActive: boolean;
}

export interface TranslationItem {
  key: string;
  namespace: string;
  values: Record<string, string>;
}

export const useLanguageStore = defineStore('languages', () => {
  const languages = ref<Language[]>([])
  const translations = ref<TranslationItem[]>([])
  const isLoading = ref(false)
  const isSaving = ref(false)

  let saveTimeout: ReturnType<typeof setTimeout> | null = null;

  const activeLanguages = computed(() => {
    return languages.value.filter(lang => lang.isActive !== false)
  })

  const defaultLanguage = computed(() => {
    return languages.value.find(lang => lang.isDefault === true) || activeLanguages.value[0]
  })

  const fetchLanguages = async (includeInactive = true) => {
    try {
      const response = await api.get(`/api/content/api/v1/languages?includeInactive=${includeInactive}`)
      languages.value = response.data
    } catch (error) {
      console.error('Failed to fetch languages:', error)
      // fallback in case backend is not ready
      if (languages.value.length === 0) {
         languages.value = [
            { id: '1', code: 'en', name: 'English', flagEmoji: '🇬🇧', flag: '🇬🇧', isRtl: false, isDefault: true, isActive: true },
            { id: '2', code: 'de', name: 'German', flagEmoji: '🇩🇪', flag: '🇩🇪', isRtl: false, isDefault: false, isActive: true },
            { id: '3', code: 'it', name: 'Italian', flagEmoji: '🇮🇹', flag: '🇮🇹', isRtl: false, isDefault: false, isActive: true },
            { id: '4', code: 'fr', name: 'French', flagEmoji: '🇫🇷', flag: '🇫🇷', isRtl: false, isDefault: false, isActive: true },
            { id: '5', code: 'ru', name: 'Russian', flagEmoji: '🇷🇺', flag: '🇷🇺', isRtl: false, isDefault: false, isActive: true }
         ]
      }
    }
  }

  const fetchTranslations = async () => {
    try {
      const response = await api.get('/api/content/api/v1/languages/all-translations')
      if (Array.isArray(response.data)) {
        translations.value = response.data
      } else {
        translations.value = response.data.items || []
      }
    } catch (error) {
      console.error('Failed to fetch translations:', error)
    }
  }

  const init = async () => {
    isLoading.value = true
    await Promise.all([fetchLanguages(), fetchTranslations()])
    isLoading.value = false
  }

  const addLanguage = async (lang: Partial<Language>) => {
    try {
       const response = await api.post('/api/content/api/v1/languages', lang)
       languages.value.push(response.data || lang as Language);
       translations.value.forEach(t => {
         t.values[(response.data?.code || lang.code) as string] = '';
       });
    } catch (e) {
       console.warn('Failed to save language to backend', e)
    }
  };

  const findLang = (idOrCode: string) => languages.value.find(l => l.id === idOrCode || l.code === idOrCode);

  const updateLanguage = async (idOrCode: string, lang: Partial<Language>) => {
    const target = findLang(idOrCode)
    const targetId = target?.id || idOrCode
    try {
      await api.put(`/api/content/api/v1/languages/${targetId}`, { ...target, ...lang, id: targetId })
      const index = languages.value.findIndex(l => l.id === targetId || l.code === idOrCode)
      if (index !== -1) {
        languages.value[index] = { ...languages.value[index], ...lang }
      }
    } catch (e) {
      console.warn('Failed to update language', e)
    }
  }

  const deleteLanguage = async (idOrCode: string) => {
    const target = findLang(idOrCode)
    const targetId = target?.id || idOrCode
    try {
      await api.delete(`/api/content/api/v1/languages/${targetId}`)
      languages.value = languages.value.filter(l => l.id !== targetId && l.code !== idOrCode)
    } catch (e) {
      console.warn('Failed to delete language', e)
    }
  }

  const toggleStatus = async (idOrCode: string, isActive?: boolean) => {
    const target = findLang(idOrCode)
    if (!target) return
    const newActiveState = isActive !== undefined ? isActive : !target.isActive
    try {
      await api.patch(`/api/content/api/v1/languages/${target.id}/toggle-status`, { isActive: newActiveState })
      target.isActive = newActiveState
    } catch (e) {
      console.warn('Failed to toggle status', e)
    }
  }

  const toggleLanguageStatus = toggleStatus

  const setAsDefault = async (idOrCode: string) => {
    const target = findLang(idOrCode)
    if (!target) return
    try {
      await api.patch(`/api/content/api/v1/languages/${target.id}/set-default`)
      languages.value.forEach(l => {
        l.isDefault = l.id === target.id
      })
    } catch (e) {
      console.warn('Failed to set as default', e)
    }
  }

  const updateTranslation = (key: string, namespace: string, langCode: string, value: string) => {
    const item = translations.value.find(t => t.key === key && t.namespace === namespace);
    if (item) {
      item.values[langCode] = value;
      triggerAutoSave(key, namespace, langCode, value);
    }
  };

  const triggerAutoSave = (key: string, namespace: string, langCode: string, value: string) => {
    isSaving.value = true;
    if (saveTimeout) clearTimeout(saveTimeout);
    saveTimeout = setTimeout(async () => {
      try {
        await api.post(`/api/content/api/v1/languages/${langCode}/translations`, {
          key,
          namespace,
          value
        })
        console.log(`Saved translation [${key}] to server...`);
      } catch (error) {
         console.error('Failed to save translation', error)
      } finally {
        isSaving.value = false;
      }
    }, 500);
  };

  const addTranslationKey = async (key: string, namespace: string) => {
    const newTranslation: TranslationItem = {
      key,
      namespace,
      values: {}
    }
    languages.value.forEach(l => newTranslation.values[l.code] = '')
    translations.value.unshift(newTranslation)
  }

  const saveBulkTranslations = async (langCode: string, data: Record<string, string>) => {
    isSaving.value = true
    try {
      await api.post('/api/content/api/v1/languages/translations/bulk', {
        langCode,
        translations: data
      })
    } catch (e) {
      console.error('Failed to save bulk translations', e)
    } finally {
      isSaving.value = false
    }
  }

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
      // trigger save for bulk
      saveBulkTranslations(langCode, parsed)
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
    isLoading,
    isSaving,
    activeLanguages,
    defaultLanguage,
    fetchLanguages,
    fetchTranslations,
    init,
    updateTranslation,
    addLanguage,
    updateLanguage,
    deleteLanguage,
    toggleStatus,
    toggleLanguageStatus,
    setAsDefault,
    addTranslationKey,
    importTranslations,
    exportTranslations,
    getLanguageProgress
  }
})
