import { storeToRefs } from 'pinia'
import { useLanguageStore } from '@/features/languages/store/languageStore'

export function useActiveLanguages() {
  const languageStore = useLanguageStore()
  const { languages, activeLanguages, defaultLanguage, isLoading } = storeToRefs(languageStore)

  return {
    languages,
    activeLanguages,
    defaultLanguage,
    fetchLanguages: languageStore.fetchLanguages,
    isLoading
  }
}
