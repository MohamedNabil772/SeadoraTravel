<template>
  <div class="space-y-6">
    <!-- Header & Locale Bar -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 pb-4 border-b border-gray-100">
      <div>
        <h3 class="text-lg font-semibold text-gray-900 flex items-center gap-2">
          <span class="p-1.5 bg-amber-50 text-amber-600 rounded-lg text-sm">📦</span>
          Tour Tier Packages & Options
        </h3>
        <p class="text-xs text-gray-500 mt-1">
          Create tiered options for guests (e.g., Standard, VIP Yacht, All-Inclusive). If left empty, default base tour pricing applies.
        </p>
      </div>

      <div class="flex items-center gap-3">
        <LocaleSwitcher v-model="currentLocale" />
      </div>
    </div>

    <!-- Empty State -->
    <div 
      v-if="!form.packages || form.packages.length === 0" 
      class="border-2 border-dashed border-gray-200 rounded-2xl p-10 text-center bg-gray-50/50 hover:bg-gray-50 transition-colors"
    >
      <div class="w-16 h-16 rounded-2xl bg-amber-100/60 text-amber-600 flex items-center justify-center mx-auto mb-4 text-2xl shadow-sm">
        📦
      </div>
      <h4 class="text-base font-semibold text-gray-900 mb-1">No Packages Configured</h4>
      <p class="text-xs text-gray-500 max-w-md mx-auto mb-6 leading-relaxed">
        Add custom packages with tiered pricing, special badges, and exclusive features to offer higher-tier VIP experiences.
      </p>

      <div class="flex flex-wrap items-center justify-center gap-3">
        <button 
          type="button"
          @click="addPackage()"
          class="px-5 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-semibold rounded-xl shadow-sm hover:shadow transition-all duration-200 flex items-center gap-2 cursor-pointer active:scale-95"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
          Add First Package
        </button>
        <button 
          type="button"
          @click="loadPresets()"
          class="px-4 py-2.5 bg-white border border-gray-200 hover:border-amber-400 hover:text-amber-700 text-gray-700 text-sm font-medium rounded-xl shadow-sm transition-all duration-200 flex items-center gap-2 cursor-pointer active:scale-95"
        >
          <span class="text-amber-500">✦</span>
          Load Standard & VIP Presets
        </button>
      </div>
    </div>

    <!-- Package Cards List -->
    <div v-else class="space-y-6">
      <div 
        v-for="(pkg, index) in form.packages" 
        :key="pkg.id || index"
        class="border border-gray-200 hover:border-indigo-200 rounded-2xl p-6 bg-white shadow-sm hover:shadow-md transition-all duration-300 relative group"
      >
        <!-- Top bar of package card -->
        <div class="flex items-center justify-between pb-4 mb-5 border-b border-gray-100">
          <div class="flex items-center gap-3">
            <span class="w-7 h-7 rounded-lg bg-indigo-50 text-indigo-700 font-bold text-xs flex items-center justify-center">
              #{{ Number(index) + 1 }}
            </span>
            <div>
              <span class="font-semibold text-gray-900 text-sm">
                {{ getPackageTitle(pkg, currentLocale) || `Package #${Number(index) + 1}` }}
              </span>
              <span 
                v-if="pkg.badge || getLocalizedField(pkg.badges, currentLocale)" 
                class="ml-2 px-2 py-0.5 rounded-full text-[10px] font-bold bg-amber-100 text-amber-800 border border-amber-200 uppercase tracking-wide"
              >
                {{ pkg.badge || getLocalizedField(pkg.badges, currentLocale) }}
              </span>
            </div>
          </div>

          <div class="flex items-center gap-2">
            <!-- Duplicate Button -->
            <button 
              type="button"
              @click="duplicatePackage(Number(index))"
              title="Duplicate Package"
              class="p-1.5 text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors cursor-pointer"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"/></svg>
            </button>
            <!-- Delete Button -->
            <button 
              type="button"
              @click="removePackage(Number(index))"
              title="Delete Package"
              class="p-1.5 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors cursor-pointer"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/></svg>
            </button>
          </div>
        </div>

        <!-- Form fields grid -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-5">
          <!-- Package Title -->
          <div class="space-y-1.5 md:col-span-2">
            <label class="block text-xs font-semibold text-gray-700 uppercase tracking-wide">
              Package Title ({{ currentLocale.toUpperCase() }}) <span class="text-red-500">*</span>
            </label>
            <input 
              v-model="ensureField(pkg, 'titles')[currentLocale]"
              type="text" 
              class="w-full px-3.5 py-2 text-sm border border-gray-300 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all" 
              :placeholder="`e.g. ${index === 0 ? 'Standard Experience' : 'VIP Luxury Private Charter'}`" 
            />
          </div>

          <!-- Price -->
          <div class="space-y-1.5">
            <label class="block text-xs font-semibold text-gray-700 uppercase tracking-wide">
              Price (USD / EUR) <span class="text-red-500">*</span>
            </label>
            <div class="relative rounded-xl shadow-sm">
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400 font-bold text-sm">
                $
              </div>
              <input 
                v-model.number="pkg.price" 
                type="number" 
                min="0"
                step="any"
                class="w-full pl-8 pr-3.5 py-2 text-sm border border-gray-300 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 font-semibold text-gray-900" 
                placeholder="0.00" 
              />
            </div>
          </div>

          <!-- Badge / Tag -->
          <div class="space-y-1.5">
            <label class="block text-xs font-semibold text-gray-700 uppercase tracking-wide">
              Badge Label (e.g. Most Popular)
            </label>
            <input 
              v-model="pkg.badge" 
              type="text" 
              class="w-full px-3.5 py-2 text-sm border border-gray-300 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" 
              placeholder="e.g. VIP / Bestseller" 
            />
          </div>

          <!-- Description -->
          <div class="space-y-1.5 md:col-span-2">
            <label class="block text-xs font-semibold text-gray-700 uppercase tracking-wide">
              Description ({{ currentLocale.toUpperCase() }})
            </label>
            <textarea 
              v-model="ensureField(pkg, 'descriptions')[currentLocale]" 
              rows="2" 
              class="w-full px-3.5 py-2 text-sm border border-gray-300 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 resize-none" 
              placeholder="Detailed description of what makes this package unique..."
            ></textarea>
          </div>

          <!-- Features (Bullet points / tags) -->
          <div class="space-y-2 md:col-span-3">
            <div class="flex items-center justify-between">
              <label class="block text-xs font-semibold text-gray-700 uppercase tracking-wide">
                Key Inclusions & Features ({{ currentLocale.toUpperCase() }})
              </label>
              <span class="text-[11px] text-gray-400">Separate multiple features with commas</span>
            </div>
            
            <input 
              :value="getFeaturesString(pkg, currentLocale)"
              @input="updateFeatures($event, pkg, currentLocale)"
              type="text" 
              class="w-full px-3.5 py-2 text-sm border border-gray-300 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500" 
              placeholder="e.g. Private Yacht, Champagne & Canapés, VIP Hotel Transfer, Dedicated Guide" 
            />

            <!-- Live feature pills preview -->
            <div 
              v-if="getFeaturesArray(pkg, currentLocale).length > 0" 
              class="flex flex-wrap gap-1.5 pt-1"
            >
              <span 
                v-for="(feat, featIdx) in getFeaturesArray(pkg, currentLocale)" 
                :key="featIdx"
                class="inline-flex items-center gap-1 px-2.5 py-1 rounded-lg text-xs font-medium bg-indigo-50 text-indigo-700 border border-indigo-100"
              >
                <span class="text-indigo-400">✓</span> {{ feat }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Add Package Button -->
      <button 
        type="button"
        @click="addPackage()" 
        class="py-3 px-4 text-sm font-semibold text-indigo-600 bg-indigo-50 hover:bg-indigo-100/80 rounded-2xl transition-all border-2 border-indigo-200/60 border-dashed w-full flex justify-center items-center gap-2 cursor-pointer active:scale-[0.99]"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
        Add Another Package Option
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, inject, onMounted } from 'vue'
import LocaleSwitcher from './LocaleSwitcher.vue'
import { useLanguageStore } from '@/features/languages/store/languageStore'
import { storeToRefs } from 'pinia'

const currentLocale = ref('en')
const form = inject<any>('tourForm')
const languageStore = useLanguageStore()
const { languages } = storeToRefs(languageStore)

const supportedLanguages = ['en', 'de', 'it', 'fr', 'ru', 'ar']

onMounted(() => {
  if (!form.value.packages) {
    form.value.packages = []
  }
  
  // Normalize existing packages structure
  form.value.packages.forEach((pkg: any) => {
    pkg.titles = pkg.titles || {}
    pkg.descriptions = pkg.descriptions || {}
    pkg.features = pkg.features || {}
    pkg.badge = pkg.badge || (pkg.badges ? pkg.badges[currentLocale.value] : '') || ''
  })
})

const ensureField = (pkg: any, fieldName: 'titles' | 'descriptions' | 'badges' | 'features') => {
  if (!pkg[fieldName]) {
    pkg[fieldName] = {}
  }
  return pkg[fieldName]
}

const getPackageTitle = (pkg: any, locale: string) => {
  if (pkg.titles && pkg.titles[locale]) return pkg.titles[locale]
  if (pkg.titles && pkg.titles['en']) return pkg.titles['en']
  return ''
}

const getLocalizedField = (dict: any, locale: string) => {
  if (!dict) return ''
  return dict[locale] || dict['en'] || ''
}

const addPackage = () => {
  if (!form.value.packages) form.value.packages = []

  const newPkg = {
    id: crypto.randomUUID ? crypto.randomUUID() : `pkg_${Date.now()}`,
    titles: {} as Record<string, string>,
    descriptions: {} as Record<string, string>,
    badge: '',
    price: form.value.price || 0,
    features: {} as Record<string, string | string[]>
  }

  const langs = languages.value?.length ? languages.value.map((l: any) => l.code) : supportedLanguages
  langs.forEach(code => {
    newPkg.titles[code] = ''
    newPkg.descriptions[code] = ''
    newPkg.features[code] = ''
  })

  form.value.packages.push(newPkg)
}

const duplicatePackage = (index: number | string) => {
  const idx = Number(index)
  const source = form.value.packages[idx]
  if (!source) return

  const cloned = JSON.parse(JSON.stringify(source))
  cloned.id = crypto.randomUUID ? crypto.randomUUID() : `pkg_${Date.now()}`
  if (cloned.titles && cloned.titles[currentLocale.value]) {
    cloned.titles[currentLocale.value] = `${cloned.titles[currentLocale.value]} (Copy)`
  }
  form.value.packages.splice(idx + 1, 0, cloned)
}

const removePackage = (index: number | string) => {
  form.value.packages.splice(Number(index), 1)
}

const loadPresets = () => {
  if (!form.value.packages) form.value.packages = []

  const basePrice = Number(form.value.price) || 120

  const standardPkg = {
    id: crypto.randomUUID ? crypto.randomUUID() : `pkg_${Date.now()}_1`,
    titles: {
      en: 'Standard Experience',
      de: 'Standard-Erlebnis',
      it: 'Esperienza Standard',
      fr: 'Expérience Standard',
      ru: 'Стандартный пакет'
    },
    descriptions: {
      en: 'Complete tour with shared group comfort and essential amenities.',
      de: 'Komplette Tour mit Gemeinschaftskomfort und Grundausstattung.',
      it: 'Tour completo con comfort di gruppo condiviso e servizi essenziali.',
      fr: 'Visite complète avec confort de groupe partagé et commodités de base.',
      ru: 'Полный тур в комфортной группе с базовыми удобствами.'
    },
    badge: 'Best Value',
    price: basePrice,
    features: {
      en: 'Hotel Pickup & Dropoff, Licensed Tour Guide, Entrance Fees Included, Soft Drinks',
      de: 'Hoteltransfer, Lizenzierter Reiseleiter, Eintrittsgelder inklusive, Erfrischungsgetränke',
      it: 'Trasferimento hotel, Guida turistica con licenza, Biglietti inclusi, Bevande analcoliche',
      fr: 'Transfert hôtel, Guide certifié, Billets inclus, Boissons sans alcool',
      ru: 'Трансфер от/до отеля, Лицензированный гид, Входные билеты, Безалкогольные напитки'
    }
  }

  const vipPkg = {
    id: crypto.randomUUID ? crypto.randomUUID() : `pkg_${Date.now()}_2`,
    titles: {
      en: 'VIP Private Luxury Tier',
      de: 'VIP Privater Luxus',
      it: 'Livello VIP Privato di Lusso',
      fr: 'Niveau VIP Luxe Privé',
      ru: 'VIP Индивидуальный Премиум'
    },
    descriptions: {
      en: 'Exclusive private vehicle, personal Egyptologist concierge, gourmet lunch, and priority skip-the-line access.',
      de: 'Exklusives Privatfahrzeug, persönlicher Ägyptologe, Gourmet-Mittagessen und bevorzugter Einlass ohne Anstehen.',
      it: 'Veicolo privato esclusivo, egittologo personale, pranzo gourmet e accesso prioritario.',
      fr: 'Véhicule privé exclusif, égyptologue personnel, déjeuner gastronomique et accès coupe-file prioritaire.',
      ru: 'Индивидуальный трансфер премиум-класса, персональный египтолог, обед для гурманов и проход без очереди.'
    },
    badge: 'VIP Top Choice',
    price: Math.round(basePrice * 1.8),
    features: {
      en: 'Private Luxury Fleet, Dedicated Egyptologist Guide, Gourmet Lunch, Fast-Track VIP Entry, Unlimited Premium Drinks',
      de: 'Privater Luxus-Fuhrpark, Eigener Ägyptologe-Reiseleiter, Gourmet-Mittagessen, VIP-Schnelleinlass, Unbegrenzte Premium-Getränke',
      it: 'Flotta di lusso privata, Guida egittologa dedicata, Pranzo gourmet, Ingresso prioritario VIP, Bevande premium illimitate',
      fr: 'Flotte de luxe privée, Guide égyptologue dédié, Déjeuner gastronomique, Entrée VIP coupe-file, Boissons premium illimitées',
      ru: 'Индивидуальный люкс-транспорт, Персональный гид-египтолог, Обед высокой кухни, VIP вход без очередей, Премиум напитки'
    }
  }

  form.value.packages = [standardPkg, vipPkg]
}

const getFeaturesString = (pkg: any, locale: string) => {
  if (!pkg.features) return ''
  const val = pkg.features[locale] || pkg.features['en'] || ''
  if (Array.isArray(val)) return val.join(', ')
  return String(val)
}

const getFeaturesArray = (pkg: any, locale: string): string[] => {
  const str = getFeaturesString(pkg, locale)
  if (!str) return []
  return str.split(',').map(s => s.trim()).filter(Boolean)
}

const updateFeatures = (e: Event, pkg: any, locale: string) => {
  const target = e.target as HTMLInputElement
  const val = target.value
  if (!pkg.features) pkg.features = {}
  pkg.features[locale] = val
}
</script>
