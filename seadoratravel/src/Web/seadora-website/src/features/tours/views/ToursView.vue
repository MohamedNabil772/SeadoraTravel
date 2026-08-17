<script setup lang="ts">
import { getSlug, getFullImageUrl } from '@/shared/utils/helpers'
import { ref, onMounted, computed, watch, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { motion, AnimatePresence } from 'motion-v'
import Navbar from '@/shared/components/Navbar.vue'
import Footer from '@/shared/components/Footer.vue'
import { useCurrencyStore } from '@/store/currency'
import { useAuthStore } from '@/features/auth/store/auth'

const { t, locale } = useI18n()
const router = useRouter()
const route = useRoute()
const currencyStore = useCurrencyStore()

interface Tour {
  id: string
  slug?: string
  title?: string
  description?: string
  categoryId: string
  destinationId: string
  price: number
  originalPrice: number | null
  discountPercentage: number | null
  currency: string
  
  names?: Record<string, string>
  descriptions?: Record<string, string>
  duration: string
  startTime?: string
  
  emoji?: string
  bgGradient?: string
  imageUrl?: string
  mainImage?: string
  images?: string[]
  mediaUrls?: string[]
  includes?: string[]
  badge?: string

  rating: number
  reviewCount: number

  isTopRated: boolean
  isBestseller: boolean
  isInHighDemand: boolean

  reserveAndPayLater: boolean
  hotelPickup: boolean
  freeCancellation: boolean
  isPrivateOption: boolean

  supplierId: string | null
  supplierPercentage: number
  maxAllocations: number
}

interface Category {
  id: string
  names: Record<string, string>
  icon?: string
}

interface Destination {
  id: string
  names: Record<string, string>
}

const tours = ref<Tour[]>([])
const categories = ref<Category[]>([])
const destinations = ref<Destination[]>([])
const loading = ref(true)
const authStore = useAuthStore()

// Filter states
const searchQuery = ref('')
const selectedDestinationId = ref('')
const selectedCategoryId = ref('')
const maxPrice = ref(500)
const selectedDate = ref('')
const selectedDateEnd = ref('')
const sortBy = ref('recommended')
const viewMode = ref('grid')
const showAdvancedFilters = ref(false)

const activeFiltersCount = computed(() => {
  let count = 0
  if (searchQuery.value) count++
  if (selectedDestinationId.value) count++
  if (selectedCategoryId.value) count++
  if (selectedDate.value) count++
  if (maxPrice.value !== 500) count++
  return count
})

// Toast state
const toastMessage = ref('')
const showToast = ref(false)

// Pagination states
const currentPage = ref(1)
const itemsPerPage = 6

// Booking Modal states
const showBookingModal = ref(false)
const bookingLoading = ref(false)
const bookingSuccess = ref(false)
const bookingReference = ref('')
const selectedTourForBooking = ref<Tour | null>(null)

const bookingForm = ref({
  name: '',
  email: '',
  phone: '',
  destination: '',
  date: '',
  guests: '2',
  notes: '',
  packageOption: 'premium',
  guideLanguage: 'en',
  pickupRequired: 'no',
  tripType: 'group',
  whatsapp: '',
  hotelName: '',
  roomNumber: '',
  passportFile: null as File | null,
  passportFileName: ''
})

const bookingErrors = ref({
  name: '',
  email: '',
  phone: '',
  destination: '',
  date: '',
  guests: ''
})

const fetchToursFromBackend = async (searchTxt?: string) => {
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const url = new URL(`${API_URL}/api/content/api/tours`)
    if (searchTxt && searchTxt.trim()) {
      url.searchParams.set('search', searchTxt.trim())
    }
    const res = await fetch(url.toString())
    if (res.ok) {
      tours.value = await res.json()
    }
  } catch (err) {
    console.error("Backend tours search failed:", err)
  }
}

let searchDebounceTimer: any = null
watch(searchQuery, (newVal) => {
  clearTimeout(searchDebounceTimer)
  searchDebounceTimer = setTimeout(() => {
    fetchToursFromBackend(newVal)
  }, 300)
})

const setFiltersFromRoute = () => {
  const loc = route.query.location
  const cat = route.query.category
  const search = route.query.search || route.query.q || route.query.text

  if (loc) {
    const dest = destinations.value.find(d => {
      const name = (d.names?.['en'] || '').toLowerCase()
      return name.includes(String(loc).toLowerCase()) || d.id === loc
    })
    if (dest) {
      selectedDestinationId.value = dest.id
    }
  } else {
    selectedDestinationId.value = ''
  }

  if (cat) {
    const category = categories.value.find(c => {
      const name = (c.names?.['en'] || '').toLowerCase()
      const normName = name.replace(/[^a-z0-9]/g, '-')
      const normCat = String(cat).toLowerCase().replace(/[^a-z0-9]/g, '-')
      return normName.includes(normCat) || normCat.includes(normName) || c.id === cat
    })
    if (category) {
      selectedCategoryId.value = category.id
    }
  } else {
    selectedCategoryId.value = ''
  }

  if (search) {
    searchQuery.value = String(search)
    fetchToursFromBackend(String(search))
  } else {
    searchQuery.value = ''
  }
}

const showAutocomplete = ref(false)
const autocompleteInputRef = ref<HTMLElement | null>(null)

// Generate luxury autocomplete suggestions
const autocompleteSuggestions = computed(() => {
  const currentLang = locale.value // triggers reactivity on language change!
  const query = searchQuery.value.toLowerCase().trim()
  
  // Destinations
  let dests = destinations.value.filter(d => 
    (d.names?.[currentLang] || d.names?.['en'] || '').toLowerCase().includes(query)
  )
  if (!query) dests = destinations.value.slice(0, 3)
  else dests = dests.slice(0, 3)

  // Categories
  let cats = categories.value.filter(c => 
    (c.names?.[currentLang] || c.names?.['en'] || '').toLowerCase().includes(query)
  )
  if (!query) cats = categories.value.slice(0, 3)
  else cats = cats.slice(0, 3)

  // Top Tours
  let matchingTours = tours.value.filter(t => 
    (t.names?.[currentLang] || t.names?.['en'] || '').toLowerCase().includes(query)
  )
  if (!query) {
    matchingTours = [...tours.value].sort((a, b) => b.rating - a.rating).slice(0, 3)
  } else {
    matchingTours = matchingTours.slice(0, 3)
  }

  return {
    destinations: dests,
    categories: cats,
    tours: matchingTours
  }
})



const localizedLabels = computed(() => {
  const l = locale.value
  return {
    // Hero Text
    heroTitle: l === 'ar' ? 'رحلات استثنائية وتجارب فريدة' 
      : l === 'fr' ? 'Voyages Extraordinaires & Expériences Uniques' 
      : l === 'de' ? 'Außergewöhnliche Reisen & Exklusive Erlebnisse' 
      : l === 'it' ? 'Viaggi Straordinari ed Esperienze Esclusive' 
      : l === 'ru' ? 'Необыкновенные путешествия и эксклюзивные впечатления' 
      : 'Extraordinary Journeys & Curated Experiences',
      
    heroSubtitle: l === 'ar' ? 'اكتشف أجمل الوجهات السياحية العالمية مع برامج سياحية فاخرة وتأكيد حجز فوري.' 
      : l === 'fr' ? 'Découvrez les plus belles destinations du monde avec des itinéraires luxueux et une confirmation immédiate.' 
      : l === 'de' ? 'Entdecken Sie die faszinierendsten Reiseziele der Welt mit luxuriösen Routen und sofortiger Bestätigung.' 
      : l === 'it' ? 'Scopri le destinazioni più affascinanti del mondo con itinerari di lusso e conferma immediata.' 
      : l === 'ru' ? 'Откройте для себя самые захватывающие направления мира с роскошными маршрутами и мгновенным подтверждением.' 
      : 'Discover the world\'s most captivating destinations. Handcrafted itineraries with guaranteed luxury, private transfers, and instant booking confirmation.',
      
    verifiedReviews: l === 'ar' ? '💬 أكثر من ٢٦ ألف تقييم موثق' 
      : l === 'fr' ? '💬 +26K Avis Vérifiés' 
      : l === 'de' ? '💬 26K+ Verifizierte Bewertungen' 
      : l === 'it' ? '💬 26K+ Recensioni Verificate' 
      : l === 'ru' ? '💬 26K+ Проверенных отзывов' 
      : '💬 26K+ Verified Reviews',
      
    curatedActivities: l === 'ar' ? '🛡️ أكثر من ١٢٠ نشاطاً مختاراً' 
      : l === 'fr' ? '🛡️ +120 Activités Soignées' 
      : l === 'de' ? '🛡️ 120+ Ausgewählte Aktivitäten' 
      : l === 'it' ? '🛡️ 120+ Attività Curate' 
      : l === 'ru' ? '🛡️ 120+ Отобранных мероприятий' 
      : '🛡️ 120+ Curated Activities',
      
    freeCancellation: l === 'ar' ? '↺ إلغاء مجاني' 
      : l === 'fr' ? '↺ Annulation gratuite' 
      : l === 'de' ? '↺ Kostenlose Stornierung' 
      : l === 'it' ? '↺ Cancellazione gratuita' 
      : l === 'ru' ? '↺ Бесплатная отмена' 
      : '↺ Free cancellation',
      
    flexibleDates: l === 'ar' ? '📅 مواعيد مرنة' 
      : l === 'fr' ? '📅 Dates flexibles' 
      : l === 'de' ? '📅 Flexible Daten' 
      : l === 'it' ? '📅 Date flessibili' 
      : l === 'ru' ? '📅 Гибкие даты' 
      : '📅 Flexible dates',
      
    reservePayLater: l === 'ar' ? '💵 احجز الآن وادفع لاحقاً' 
      : l === 'fr' ? '💵 Réservez et payez plus tard' 
      : l === 'de' ? '💵 Jetzt buchen, später zahlen' 
      : l === 'it' ? '💵 Prenota ora, paga dopo' 
      : l === 'ru' ? '💵 Бронируйте сейчас, платите позже' 
      : '💵 Reserve now, pay later',
      
    vipConcierge: l === 'ar' ? '🎧 خدمة كونسيرج VIP على مدار الساعة' 
      : l === 'fr' ? '🎧 Conciergerie VIP 24/7' 
      : l === 'de' ? '🎧 24/7 VIP-Concierge' 
      : l === 'it' ? '🎧 Assistenza Concierge VIP 24/7' 
      : l === 'ru' ? '🎧 VIP-консьерж 24/7' 
      : '🎧 24/7 VIP Concierge',

    // Filter & Card Labels
    destinations: l === 'ar' ? 'الوجهات' : l === 'fr' ? 'Destinations' : l === 'de' ? 'Reiseziele' : l === 'it' ? 'Destinazioni' : l === 'ru' ? 'Направления' : 'Destinations',
    experiences: l === 'ar' ? 'التجارب والأنشطة' : l === 'fr' ? 'Expériences' : l === 'de' ? 'Erlebnisse' : l === 'it' ? 'Esperienze' : l === 'ru' ? 'Впечатления' : 'Experiences',
    trending: l === 'ar' ? 'أشهر الجولات' : l === 'fr' ? 'Circuits populaires' : l === 'de' ? 'Beliebte Touren' : l === 'it' ? 'Tour di tendenza' : l === 'ru' ? 'Популярные туры' : 'Trending Tours',
    from: l === 'ar' ? 'من' : l === 'fr' ? 'À partir de' : l === 'de' ? 'Ab' : l === 'it' ? 'Da' : l === 'ru' ? 'От' : 'From',
    perPerson: l === 'ar' ? 'لكل شخص' : l === 'fr' ? 'par personne' : l === 'de' ? 'pro Person' : l === 'it' ? 'a persona' : l === 'ru' ? 'за человека' : 'per person',
    hotelPickup: l === 'ar' ? 'توصيل من وإلى الفندق' : l === 'fr' ? 'Prise en charge à l\'hôtel' : l === 'de' ? 'Hotelabholung' : l === 'it' ? 'Prelievo in hotel' : l === 'ru' ? 'Трансфер из отеля' : 'Hotel pickup',
    privateOption: l === 'ar' ? 'خيار خاص' : l === 'fr' ? 'Option privée' : l === 'de' ? 'Private Option' : l === 'it' ? 'Opzione privata' : l === 'ru' ? 'Индивидуальный вариант' : 'Private option',
    results: l === 'ar' ? 'نتائج' : l === 'fr' ? 'résultats' : l === 'de' ? 'Ergebnisse' : l === 'it' ? 'risultati' : l === 'ru' ? 'результатов' : 'results',
    allDestinations: l === 'ar' ? 'جميع الوجهات' : l === 'fr' ? 'Toutes les destinations' : l === 'de' ? 'Alle Reiseziele' : l === 'it' ? 'Tutte le destinazioni' : l === 'ru' ? 'Все направления' : 'All Destinations',
    allTours: l === 'ar' ? 'جميع الجولات' : l === 'fr' ? 'Tous les circuits' : l === 'de' ? 'Alle Touren' : l === 'it' ? 'Tutti i tour' : l === 'ru' ? 'Все туры' : 'All Tours',
    maxBudget: l === 'ar' ? 'الميزانية القصوى' : l === 'fr' ? 'Budget maximum' : l === 'de' ? 'Maximales Budget' : l === 'it' ? 'Budget massimo' : l === 'ru' ? 'Максимальный бюджет' : 'Maximum Budget',
    resetFilters: l === 'ar' ? 'إعادة ضبط الفلاتر' : l === 'fr' ? 'Réinitialiser les filtres' : l === 'de' ? 'Filter zurücksetzen' : l === 'it' ? 'Reimposta filtri' : l === 'ru' ? 'Сбросить фильтры' : 'Reset Filters',
    sortRecommended: l === 'ar' ? 'الموصى به' : l === 'fr' ? 'Recommandé' : l === 'de' ? 'Empfohlen' : l === 'it' ? 'Consigliato' : l === 'ru' ? 'Рекомендуемые' : 'Recommended',
    priceLowHigh: l === 'ar' ? 'السعر: من الأقل للأعلى' : l === 'fr' ? 'Prix: croissant' : l === 'de' ? 'Preis: aufsteigend' : l === 'it' ? 'Prezzo: dal più basso' : l === 'ru' ? 'Цена: по возрастанию' : 'Price: Low to High',
    priceHighLow: l === 'ar' ? 'السعر: من الأعلى للأقل' : l === 'fr' ? 'Prix: décroissant' : l === 'de' ? 'Preis: absteigend' : l === 'it' ? 'Prezzo: dal più alto' : l === 'ru' ? 'Цена: по убыванию' : 'Price: High to Low',
    durationSort: l === 'ar' ? 'المدة' : l === 'fr' ? 'Durée' : l === 'de' ? 'Dauer' : l === 'it' ? 'Durata' : l === 'ru' ? 'Длительность' : 'Duration',
    grid: l === 'ar' ? 'شبكة' : l === 'fr' ? 'Grille' : l === 'de' ? 'Raster' : l === 'it' ? 'Griglia' : l === 'ru' ? 'Сетка' : 'Grid',
    list: l === 'ar' ? 'قائمة' : l === 'fr' ? 'Liste' : l === 'de' ? 'Liste' : l === 'it' ? 'Elenco' : l === 'ru' ? 'Список' : 'List',
    topRated: l === 'ar' ? 'الأعلى تقييماً' : l === 'fr' ? 'MIEUX NOTÉ' : l === 'de' ? 'TOP BEWERTET' : l === 'it' ? 'PIÙ VOTATI' : l === 'ru' ? 'ЛУЧШИЙ РЕЙТИНГ' : 'TOP RATED',
    bestseller: l === 'ar' ? 'الأكثر مبيعاً' : l === 'fr' ? 'MEILLEURE VENTE' : l === 'de' ? 'BESTSELLER' : l === 'it' ? 'PIÙ VENDUTI' : l === 'ru' ? 'ХИТ ПРОДАЖ' : 'BESTSELLER',
    inHighDemand: l === 'ar' ? 'طلب مرتفع' : l === 'fr' ? 'TRÈS DEMANDÉ' : l === 'de' ? 'SEHR GEFRAGT' : l === 'it' ? 'MOLTO RICHIESTO' : l === 'ru' ? 'ВЫСОКИЙ СПРОС' : 'IN HIGH DEMAND',
    flexible: l === 'ar' ? 'مرن' : l === 'fr' ? 'Flexible' : l === 'de' ? 'Flexibel' : l === 'it' ? 'Flessibile' : l === 'ru' ? 'Гибко' : 'Flexible',
    viewDetails: l === 'ar' ? 'عرض التفاصيل' : l === 'fr' ? 'Voir les détails' : l === 'de' ? 'Details anzeigen' : l === 'it' ? 'Vedi dettagli' : l === 'ru' ? 'Подробнее' : 'View details',
    addToFavorites: l === 'ar' ? 'إضافة إلى المفضلة' : l === 'fr' ? 'Ajouter aux favoris' : l === 'de' ? 'Zu Favoriten hinzufügen' : l === 'it' ? 'Aggiungi ai preferiti' : l === 'ru' ? 'В избранное' : 'Add to favorites',
    removeFromFavorites: l === 'ar' ? 'إزالة من المفضلة' : l === 'fr' ? 'Retirer des favoris' : l === 'de' ? 'Aus Favoriten entfernen' : l === 'it' ? 'Rimuovi dai preferiti' : l === 'ru' ? 'Удалить из избранного' : 'Remove from favorites',
    shareTour: l === 'ar' ? 'مشاركة الجولة' : l === 'fr' ? 'Partager le circuit' : l === 'de' ? 'Tour teilen' : l === 'it' ? 'Condividi tour' : l === 'ru' ? 'Поделиться туром' : 'Share Tour',
    noToursFound: l === 'ar' ? 'لم يتم العثور على تجارب' : l === 'fr' ? 'Aucune expérience trouvée' : l === 'de' ? 'Keine Erlebnisse gefunden' : l === 'it' ? 'Nessuna esperienza trovata' : l === 'ru' ? 'Впечатлений не найдено' : 'No Experiences Found',
    noToursDesc: l === 'ar' ? 'لم نتمكن من العثور على أي جولات تطابق فلاترك الحالية. حاول تعديل المعايير.' : l === 'fr' ? 'Nous n\'avons trouvé aucun circuit correspondant à vos critères. Essayez d\'ajuster vos filtres.' : l === 'de' ? 'Wir konnten keine Touren finden, die Ihren aktuellen Filtern entsprechen. Passen Sie Ihre Kriterien an.' : l === 'it' ? 'Non abbiamo trovato tour corrispondenti ai tuoi filtri attuali. Prova a modificare i criteri.' : l === 'ru' ? 'Мы не нашли туров, соответствующих вашим фильтрам. Попробуйте изменить параметры поиска.' : 'We couldn\'t find any tours matching your current filters. Try adjusting your criteria.',
    viewAllTours: l === 'ar' ? 'عرض جميع الجولات' : l === 'fr' ? 'Voir tous les circuits' : l === 'de' ? 'Alle Touren anzeigen' : l === 'it' ? 'Vedi tutti i tour' : l === 'ru' ? 'Все туры' : 'View All Tours'
  }
})

const handleSuggestionClick = (type: string, item: any) => {
  showAutocomplete.value = false
  if (type === 'destination') {
    selectedDestinationId.value = item.id
    searchQuery.value = ''
  } else if (type === 'category') {
    selectedCategoryId.value = item.id
    searchQuery.value = ''
  } else if (type === 'tour') {
    openDetailsPage(item)
  }
  
  if (type !== 'tour') {
    const resultsBar = document.getElementById('results-bar')
    if (resultsBar) {
      resultsBar.scrollIntoView({ behavior: 'smooth', block: 'start' })
    }
  }
}

const showSortDropdown = ref(false)
const sortDropdownRef = ref<HTMLElement | null>(null)

const sortOptions = computed(() => [
  { value: 'recommended', label: localizedLabels.value.sortRecommended, icon: '✨' },
  { value: 'price-low', label: localizedLabels.value.priceLowHigh, icon: '🏷️' },
  { value: 'price-high', label: localizedLabels.value.priceHighLow, icon: '💎' },
  { value: 'duration', label: localizedLabels.value.durationSort, icon: '⏱️' }
])

const currentSortLabel = computed(() => {
  const found = sortOptions.value.find(o => o.value === sortBy.value)
  return found?.label || localizedLabels.value.sortRecommended
})

// Luxury Interactive Calendar Filter State (UI/UX Pro Max)
const showCalendarPopover = ref(false)
const calendarPopoverRef = ref<HTMLElement | null>(null)
const calendarViewDate = ref(new Date())

const currentMonthYearLabel = computed(() => {
  const loc = locale.value === 'ar' ? 'ar-EG' : locale.value === 'de' ? 'de-DE' : locale.value === 'fr' ? 'fr-FR' : locale.value === 'it' ? 'it-IT' : locale.value === 'ru' ? 'ru-RU' : 'en-US'
  return calendarViewDate.value.toLocaleDateString(loc, {
    month: 'long',
    year: 'numeric'
  })
})

const calendarWeekdays = computed(() => [
  t('calendar.weekdays.mo'),
  t('calendar.weekdays.tu'),
  t('calendar.weekdays.we'),
  t('calendar.weekdays.th'),
  t('calendar.weekdays.fr'),
  t('calendar.weekdays.sa'),
  t('calendar.weekdays.su')
])

const calendarDays = computed(() => {
  const year = calendarViewDate.value.getFullYear()
  const month = calendarViewDate.value.getMonth()
  
  const firstDayOfMonth = new Date(year, month, 1)
  const lastDayOfMonth = new Date(year, month + 1, 0)
  
  // Starting day of week (Monday = 0)
  let startDay = firstDayOfMonth.getDay() - 1
  if (startDay === -1) startDay = 6 // Sunday
  
  const days = []
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  // Previous month padding
  const prevMonthLastDay = new Date(year, month, 0).getDate()
  for (let i = startDay - 1; i >= 0; i--) {
    const d = new Date(year, month - 1, prevMonthLastDay - i)
    days.push({
      dateStr: formatDateISO(d),
      dayNumber: prevMonthLastDay - i,
      isCurrentMonth: false,
      isPast: d < today,
      isToday: false,
      isSelected: false,
      isInRange: false,
      isRangeStart: false,
      isRangeEnd: false
    })
  }

  // Current month days
  for (let i = 1; i <= lastDayOfMonth.getDate(); i++) {
    const d = new Date(year, month, i)
    const dateStr = formatDateISO(d)
    
    let isSelected = false
    let isInRange = false
    let isRangeStart = false
    let isRangeEnd = false
    
    if (selectedDate.value === dateStr) {
      isSelected = true
      isRangeStart = true
      if (!selectedDateEnd.value) isRangeEnd = true
    }
    if (selectedDateEnd.value === dateStr) {
      isSelected = true
      isRangeEnd = true
    }
    if (selectedDate.value && selectedDateEnd.value) {
      if (dateStr > selectedDate.value && dateStr < selectedDateEnd.value) {
        isInRange = true
      }
    }

    days.push({
      dateStr,
      dayNumber: i,
      isCurrentMonth: true,
      isPast: d < today,
      isToday: d.getTime() === today.getTime(),
      isSelected,
      isInRange,
      isRangeStart,
      isRangeEnd
    })
  }

  // Next month padding to complete grid
  const totalSlots = days.length <= 35 ? 35 : 42
  const remaining = totalSlots - days.length
  for (let i = 1; i <= remaining; i++) {
    const d = new Date(year, month + 1, i)
    days.push({
      dateStr: formatDateISO(d),
      dayNumber: i,
      isCurrentMonth: false,
      isPast: false,
      isToday: false,
      isSelected: false,
      isInRange: false,
      isRangeStart: false,
      isRangeEnd: false
    })
  }

  return days
})

const formatDateISO = (d: Date) => {
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

const formattedSelectedDate = computed(() => {
  if (!selectedDate.value) return ''
  const loc = locale.value === 'de' ? 'de-DE' : locale.value === 'fr' ? 'fr-FR' : locale.value === 'it' ? 'it-IT' : locale.value === 'ru' ? 'ru-RU' : 'en-US'
  
  const parts = selectedDate.value.split('-')
  if (parts.length !== 3) return selectedDate.value
  const d1 = new Date(Number(parts[0]), Number(parts[1]) - 1, Number(parts[2]))
  const s1 = d1.toLocaleDateString(loc, { weekday: 'short', month: 'short', day: 'numeric' })
  
  if (selectedDateEnd.value && selectedDateEnd.value !== selectedDate.value) {
    const parts2 = selectedDateEnd.value.split('-')
    const d2 = new Date(Number(parts2[0]), Number(parts2[1]) - 1, Number(parts2[2]))
    const s2 = d2.toLocaleDateString(loc, { month: 'short', day: 'numeric' })
    return `${s1} - ${s2}`
  }
  
  return s1
})

const prevMonth = () => {
  calendarViewDate.value = new Date(calendarViewDate.value.getFullYear(), calendarViewDate.value.getMonth() - 1, 1)
}

const nextMonth = () => {
  calendarViewDate.value = new Date(calendarViewDate.value.getFullYear(), calendarViewDate.value.getMonth() + 1, 1)
}

const toggleCalendarPopover = (e?: Event) => {
  if (e) e.stopPropagation()
  showCalendarPopover.value = !showCalendarPopover.value
}

const selectCalendarDay = (day: { dateStr: string; isPast: boolean }) => {
  if (day.isPast) return
  if (!selectedDate.value || (selectedDate.value && selectedDateEnd.value)) {
    // Start new selection
    selectedDate.value = day.dateStr
    selectedDateEnd.value = ''
  } else {
    // End selection
    if (day.dateStr < selectedDate.value) {
      selectedDateEnd.value = selectedDate.value
      selectedDate.value = day.dateStr
    } else {
      selectedDateEnd.value = day.dateStr
    }
  }
}

const setDatePreset = (preset: 'today' | 'tomorrow' | 'weekend' | 'nextWeek' | 'thisMonth') => {
  const d = new Date()
  if (preset === 'today') {
    selectedDate.value = formatDateISO(d)
    selectedDateEnd.value = ''
  } else if (preset === 'tomorrow') {
    d.setDate(d.getDate() + 1)
    selectedDate.value = formatDateISO(d)
    selectedDateEnd.value = ''
  } else if (preset === 'weekend') {
    const day = d.getDay()
    const diff = (6 - day + 7) % 7 || 7 // Upcoming Saturday
    d.setDate(d.getDate() + diff)
    selectedDate.value = formatDateISO(d)
    d.setDate(d.getDate() + 1)
    selectedDateEnd.value = formatDateISO(d)
  } else if (preset === 'nextWeek') {
    d.setDate(d.getDate() + 7)
    selectedDate.value = formatDateISO(d)
    d.setDate(d.getDate() + 6)
    selectedDateEnd.value = formatDateISO(d)
  } else if (preset === 'thisMonth') {
    const lastDay = new Date(d.getFullYear(), d.getMonth() + 1, 0)
    // If today is past first day, start from today
    selectedDate.value = formatDateISO(d)
    selectedDateEnd.value = formatDateISO(lastDay)
  }
  showCalendarPopover.value = false
}

const isInsideElement = (containerRef: any, target: Node | null): boolean => {
  if (!containerRef || !target) return false
  const domEl = containerRef instanceof HTMLElement ? containerRef : (containerRef.$el || containerRef)
  if (domEl && typeof domEl.contains === 'function') {
    return domEl.contains(target)
  }
  return false
}

const handleBodyClick = (e: Event) => {
  const target = (e.target || (e as any).composedPath?.()[0]) as Node
  if (!isInsideElement(autocompleteInputRef.value, target)) {
    showAutocomplete.value = false
  }
  if (!isInsideElement(sortDropdownRef.value, target)) {
    showSortDropdown.value = false
  }
  if (!isInsideElement(calendarPopoverRef.value, target)) {
    showCalendarPopover.value = false
  }
}

onMounted(async () => {
  document.addEventListener('pointerdown', handleBodyClick, { passive: true })
  document.addEventListener('click', handleBodyClick, { passive: true })
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    
    // Fetch Categories
    const catRes = await fetch(`${API_URL}/api/content/api/categories`)
    if (catRes.ok) {
      categories.value = await catRes.json()
    }

    // Fetch Tours
    const tourRes = await fetch(`${API_URL}/api/content/api/tours`)
    if (tourRes.ok) {
      tours.value = await tourRes.json()
    }

    // Fetch Destinations
    const destRes = await fetch(`${API_URL}/api/content/api/destinations`)
    if (destRes.ok) {
      destinations.value = await destRes.json()
    }

    // Apply filter state from URL query params
    setFiltersFromRoute()
  } catch (error) {
    console.error("Failed to load content in ToursView:", error)
  } finally {
    loading.value = false
  }
})

onUnmounted(() => {
  document.removeEventListener('pointerdown', handleBodyClick)
  document.removeEventListener('click', handleBodyClick)
})

watch(() => route.query, () => {
  setFiltersFromRoute()
}, { deep: true })

// Helpers

const getLocalized = (dict: any, fallback: string = '') => {
  if (!dict) return fallback
  if (typeof dict === 'string') return dict
  if (typeof dict === 'object') {
    return dict[locale.value] || dict['en'] || Object.values(dict)[0] || fallback
  }
  return fallback
}

const openDetailsPage = (tour: Tour) => {
  const slug = tour.slug || getSlug(tour.title || tour.names?.['en'] || 'tour')
  router.push(`/tour/${slug}`)
}

const triggerToast = (msg: string) => {
  toastMessage.value = msg
  showToast.value = true
  setTimeout(() => {
    showToast.value = false
  }, 3000)
}

const favoriteTourIds = ref<Set<string>>(new Set(JSON.parse(localStorage.getItem('seadora_guest_favorites') || '[]')))

const isTourFavorite = (tourId: string) => {
  if (authStore.isLoggedIn) {
    return authStore.isFavorite(tourId)
  }
  return favoriteTourIds.value.has(tourId)
}

const toggleFavoriteTour = (tourId: string) => {
  if (authStore.isLoggedIn) {
    authStore.toggleFavorite(tourId)
    const isFav = authStore.isFavorite(tourId)
    triggerToast(isFav ? "Saved to favorites!" : "Removed from favorites.")
  } else {
    if (favoriteTourIds.value.has(tourId)) {
      favoriteTourIds.value.delete(tourId)
      triggerToast("Removed from favorites.")
    } else {
      favoriteTourIds.value.add(tourId)
      triggerToast("Saved to favorites!")
    }
    // Force Vue reactivity trigger & persist
    favoriteTourIds.value = new Set(favoriteTourIds.value)
    localStorage.setItem('seadora_guest_favorites', JSON.stringify(Array.from(favoriteTourIds.value)))
  }
}

const shareTour = (tour: Tour) => {
  const slug = getSlug(tour.names?.['en'] || 'tour')
  const shareUrl = `${window.location.origin}/tour/${slug}`
  navigator.clipboard.writeText(shareUrl).then(() => {
    triggerToast("Tour link copied to clipboard!")
  }).catch(() => {
    triggerToast("Failed to copy link.")
  })
}

const getTourRatingDetails = (tourId: string) => {
  const ratings: Record<string, { rating: number, reviews: number }> = {
    '1': { rating: 4.8, reviews: 1042 },
    '2': { rating: 4.9, reviews: 673 },
    '3': { rating: 4.7, reviews: 577 },
    '4': { rating: 4.8, reviews: 476 },
    '5': { rating: 4.6, reviews: 322 }
  }
  const numericId = tourId.replace(/\D/g, '') || '1'
  return ratings[numericId] || { rating: 4.7, reviews: 148 }
}

const formatDuration = (dur: string) => {
  const durations: Record<string, Record<string, string>> = {
    fullDay: { en: 'Full Day', de: 'Ganztägig', it: 'Giornata intera', fr: 'Journée entière', ru: 'Полный день' },
    halfDay: { en: 'Half Day', de: 'Halbtägig', it: 'Mezza giornata', fr: 'Demi-journée', ru: 'Полдня' },
    twoDays: { en: '2 Days', de: '2 Tage', it: '2 Giorni', fr: '2 Jours', ru: '2 дня' },
    fiveDays: { en: '5 Days', de: '5 Tage', it: '5 Giorni', fr: '5 Jours', ru: '5 дней' },
    oneDay: { en: '1 Day', de: '1 Tag', it: '1 Giorno', fr: '1 Jour', ru: '1 день' },
    evening: { en: 'Evening', de: 'Abends', it: 'Serale', fr: 'Soirée', ru: 'Вечер' },
    threeHours: { en: '3 Hours', de: '3 Stunden', it: '3 Ore', fr: '3 Heures', ru: '3 часа' }
  }
  return durations[dur]?.[locale.value] || durations[dur]?.['en'] || dur
}

// Map Destination GUID or static ID to select values
const mapDestinationToValue = (destId: string) => {
  const dest = destinations.value.find(d => d.id === destId)
  if (!dest) return ''
  const name = (dest.names?.['en'] || '').toLowerCase()
  if (name.includes('hurghada')) return 'hurghada'
  if (name.includes('cairo')) return 'cairo'
  if (name.includes('luxor')) return 'luxor'
  if (name.includes('sharm')) return 'sharm'
  return ''
}

// Booking Modal trigger
const openBookingModal = (tour: Tour) => {
  selectedTourForBooking.value = tour
  const destVal = mapDestinationToValue(tour.destinationId)
  
  bookingForm.value = {
    name: '',
    email: '',
    phone: '',
    destination: destVal,
    date: selectedDate.value || '',
    guests: '2',
    notes: '',
    packageOption: 'premium',
    guideLanguage: locale.value || 'en',
    pickupRequired: 'no',
    tripType: 'group',
    whatsapp: '',
    hotelName: '',
    roomNumber: '',
    passportFile: null,
    passportFileName: ''
  }
  
  bookingErrors.value = {
    name: '',
    email: '',
    phone: '',
    destination: '',
    date: '',
    guests: ''
  }
  
  bookingSuccess.value = false
  showBookingModal.value = true
}

// Client validation
const validateField = (field: string) => {
  const form = bookingForm.value
  if (field === 'name') {
    if (!form.name) {
      bookingErrors.value.name = 'Full name is required.'
    } else if (form.name.trim().length < 3) {
      bookingErrors.value.name = 'Full name must be at least 3 characters.'
    } else if (!/^[A-Za-z\s]+$/.test(form.name.trim())) {
      bookingErrors.value.name = 'Full name must contain only letters and spaces.'
    } else {
      bookingErrors.value.name = ''
    }
  }
  
  if (field === 'email') {
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!form.email) {
      bookingErrors.value.email = 'Email address is required.'
    } else if (!emailPattern.test(form.email)) {
      bookingErrors.value.email = 'Please enter a valid email address.'
    } else {
      bookingErrors.value.email = ''
    }
  }
  
  if (field === 'phone') {
    const phonePattern = /^\+?[0-9\s\-()]{7,20}$/
    if (!form.phone) {
      bookingErrors.value.phone = 'Phone number is required.'
    } else if (!phonePattern.test(form.phone)) {
      bookingErrors.value.phone = 'Please enter a valid phone number (e.g. +1 555-0199).'
    } else {
      bookingErrors.value.phone = ''
    }
  }
  
  if (field === 'destination') {
    if (!form.destination) {
      bookingErrors.value.destination = 'Destination is required.'
    } else {
      bookingErrors.value.destination = ''
    }
  }
  
  if (field === 'date') {
    if (!form.date) {
      bookingErrors.value.date = 'Target date is required.'
    } else {
      const selectedDate = new Date(form.date)
      selectedDate.setHours(0, 0, 0, 0)
      const today = new Date()
      today.setHours(0, 0, 0, 0)
      if (selectedDate < today) {
        bookingErrors.value.date = 'Target date must be today or in the future.'
      } else {
        bookingErrors.value.date = ''
      }
    }
  }
  
  if (field === 'guests') {
    if (!form.guests) {
      bookingErrors.value.guests = 'Number of guests is required.'
    } else {
      bookingErrors.value.guests = ''
    }
  }
}

const validateForm = () => {
  validateField('name')
  validateField('email')
  
  return !bookingErrors.value.name && !bookingErrors.value.email
}

const generateReferenceCode = () => {
  const randomNum = Math.floor(1000 + Math.random() * 9000)
  return `SEADORA-${randomNum}-EG`
}

const submitBooking = async () => {
  if (!selectedTourForBooking.value) return
  
  if (!validateForm()) {
    return
  }
  
  bookingLoading.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const response = await fetch(`${API_URL}/api/booking/api/bookings`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        tourId: selectedTourForBooking.value.id,
        customerName: bookingForm.value.name.trim(),
        customerEmail: bookingForm.value.email.trim(),
        whatsApp: bookingForm.value.whatsapp.trim(),
        hotelName: bookingForm.value.hotelName.trim(),
        roomNumber: bookingForm.value.roomNumber.trim(),
        passportFileName: bookingForm.value.passportFileName,
        tripType: bookingForm.value.tripType
      })
    })
    
    if (response.ok) {
      bookingReference.value = generateReferenceCode()
      bookingSuccess.value = true
    } else {
      const errText = await response.text()
      alert("Booking failed. " + (errText || "Please check details and try again."))
    }
  } catch (error) {
    console.error("Booking error:", error)
    alert("Connection error. Please try again.")
  } finally {
    bookingLoading.value = false
  }
}

const calculateFinalPrice = () => {
  if (!selectedTourForBooking.value) return 0
  const base = selectedTourForBooking.value.price || 0
  const guests = parseInt(bookingForm.value.guests) || 1
  const pickup = bookingForm.value.pickupRequired === 'yes' ? 15 : 0
  const tier = bookingForm.value.packageOption === 'elite' ? 50 : (bookingForm.value.packageOption === 'premium' ? 25 : 0)
  return (base + tier + pickup) * guests
}

const getLanguageLabel = (lang: string) => {
  switch (lang) {
    case 'en': return '🇬🇧 English'
    case 'de': return '🇩🇪 German'
    case 'fr': return '🇫🇷 French'
    case 'it': return '🇮🇹 Italian'
    case 'ru': return '🇷🇺 Russian'
    default: return '🇬🇧 English'
  }
}

const triggerPrint = () => {
  window.print()
}

const handlePassportUpload = (e: Event) => {
  const target = e.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    bookingForm.value.passportFile = target.files[0]
    bookingForm.value.passportFileName = target.files[0].name
  }
}

// Watchers for reset pagination on filter change
watch([searchQuery, selectedDestinationId, selectedCategoryId, maxPrice, selectedDate, sortBy], () => {
  currentPage.value = 1
})

// Filtered list computed
const filteredTours = computed(() => {
  return tours.value.filter(tour => {
    // Search query matching
    if (searchQuery.value) {
      const q = searchQuery.value.toLowerCase()
      const name = getLocalized(tour.names, '').toLowerCase()
      const desc = getLocalized(tour.descriptions, '').toLowerCase()
      const matchesText = name.includes(q) || desc.includes(q)
      
      // Match custom tags
      const tags = tour.includes || []
      const matchesTags = tags.some((t: string) => t.toLowerCase().includes(q))
      
      // Category or destination direct name match
      const destName = getLocalized(destinations.value.find(d => d.id === tour.destinationId)?.names || {}, '').toLowerCase()
      const catName = getLocalized(categories.value.find(c => c.id === tour.categoryId)?.names || {}, '').toLowerCase()
      const matchesMeta = destName.includes(q) || catName.includes(q)

      if (!matchesText && !matchesTags && !matchesMeta) return false
    }

    // Destination matching
    if (selectedDestinationId.value && tour.destinationId !== selectedDestinationId.value) {
      return false
    }

    // Category matching
    if (selectedCategoryId.value && tour.categoryId !== selectedCategoryId.value) {
      return false
    }

    // Price matching
    if (tour.price > maxPrice.value) {
      return false
    }

    return true
  })
})

// Sorted list computed
const sortedTours = computed(() => {
  const list = [...filteredTours.value]
  if (sortBy.value === 'price-low') {
    return list.sort((a, b) => a.price - b.price)
  } else if (sortBy.value === 'price-high') {
    return list.sort((a, b) => b.price - a.price)
  } else if (sortBy.value === 'duration') {
    return list.sort((a, b) => {
      const durA = a.duration || ''
      const durB = b.duration || ''
      return durA.localeCompare(durB)
    })
  }
  // Default 'recommended' - sort by rating
  return list.sort((a, b) => {
    const rateA = getTourRatingDetails(a.id).rating
    const rateB = getTourRatingDetails(b.id).rating
    if (rateB !== rateA) return rateB - rateA
    return getTourRatingDetails(b.id).reviews - getTourRatingDetails(a.id).reviews
  })
})

// Paginated tours list
const paginatedTours = computed(() => {
  const startIndex = (currentPage.value - 1) * itemsPerPage
  return sortedTours.value.slice(startIndex, startIndex + itemsPerPage)
})

// Total page count
const totalPages = computed(() => {
  return Math.ceil(sortedTours.value.length / itemsPerPage)
})

// Lightbox Keyboard actions
const handleKeydown = (e: KeyboardEvent) => {
  if (e.key === 'Escape') {
    if (showBookingModal.value) showBookingModal.value = false
    if (showAutocomplete.value) showAutocomplete.value = false
  }
}

onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
  document.addEventListener('click', handleBodyClick)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown)
  document.removeEventListener('click', handleBodyClick)
})
</script>

<template>
  <div class="min-h-screen bg-cream text-dark flex flex-col font-sans relative overflow-hidden">
    <!-- Navbar -->
    <Navbar />

    <!-- Luxury Portal Header (Spacious UI/UX Pro Max Hero) -->
    <header class="relative z-50 bg-sea-deep text-white pt-24 md:pt-32 pb-8 md:pb-10 px-4 sm:px-8 flex flex-col justify-center items-center text-center overflow-visible border-b border-gold/30" style="background: linear-gradient(180deg, rgba(6,30,50,0.65) 0%, rgba(6,30,50,0.4) 45%, rgba(6,30,50,0.85) 100%), url('https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=2560&q=88') center/cover no-repeat; text-rendering: optimizeLegibility;">
      <div class="relative z-10 w-full max-w-5xl mx-auto flex flex-col items-center mt-4 sm:mt-8">
        
        <!-- 1. Top Primary Trust Badges -->
        <motion.div 
          :initial="{ opacity: 0, y: 15 }" 
          :animate="{ opacity: 1, y: 0 }" 
          :transition="{ duration: 0.6, ease: 'easeOut', delay: 0.1 }"
          class="flex items-center justify-center gap-3 sm:gap-4 flex-wrap mb-6 sm:mb-8"
        >
          <span class="inline-flex items-center gap-2 px-6 py-2.5 rounded-full bg-[#062d4d]/90 backdrop-blur-md border border-white/20 text-xs sm:text-sm font-semibold text-white shadow-xl tracking-wide">
            {{ localizedLabels.verifiedReviews }}
          </span>
          <span class="inline-flex items-center gap-2 px-6 py-2.5 rounded-full bg-[#062d4d]/90 backdrop-blur-md border border-white/20 text-xs sm:text-sm font-semibold text-white shadow-xl tracking-wide">
            {{ localizedLabels.curatedActivities }}
          </span>
        </motion.div>

        <!-- 2. Dramatic Headline -->
        <motion.h1 
          :initial="{ opacity: 0, y: 15 }" 
          :animate="{ opacity: 1, y: 0 }" 
          :transition="{ duration: 0.6, ease: 'easeOut', delay: 0.2 }"
          class="font-playfair text-4xl sm:text-6xl md:text-7xl lg:text-8xl text-white font-extrabold leading-[1.08] tracking-[-0.02em] drop-shadow-2xl mb-5 sm:mb-6 text-center max-w-5xl" style="font-feature-settings: 'liga' 1, 'kern' 1;"
        >
          {{ localizedLabels.heroTitle }}
        </motion.h1>

        <!-- 3. Elegant Subtitle -->
        <motion.p 
          :initial="{ opacity: 0, y: 15 }" 
          :animate="{ opacity: 1, y: 0 }" 
          :transition="{ duration: 0.6, ease: 'easeOut', delay: 0.3 }"
          class="font-cormorant italic text-slate-100 text-xl sm:text-2xl md:text-3xl max-w-3xl mx-auto leading-relaxed text-center drop-shadow-md mb-8 sm:mb-10 font-normal" style="font-feature-settings: 'liga' 1, 'kern' 1;"
        >
          {{ localizedLabels.heroSubtitle }}
        </motion.p>

        <!-- 4. Massive Floating Search Bar -->
        <div ref="autocompleteInputRef" class="relative z-50 w-full max-w-4xl mx-auto px-2">
          <motion.div 
            :initial="{ opacity: 0, y: 15 }" 
            :animate="{ opacity: 1, y: 0 }" 
            :transition="{ duration: 0.6, ease: 'easeOut', delay: 0.4 }"
          >
          <motion.div 
            :whileHover="{ scale: 1.01 }"
            class="relative flex items-center bg-white rounded-full shadow-[0_30px_70px_rgba(0,0,0,0.4)] border-2 border-white/95 overflow-hidden pl-8 pr-4 py-5 sm:py-5.5 focus-within:ring-4 focus-within:ring-[#c9a84c]/50 transition-all z-50"
          >
            <svg class="w-7 sm:w-8 h-7 sm:h-8 text-[#062d4d] shrink-0 mr-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
            <input 
              v-model="searchQuery"
              @focus="showAutocomplete = true"
              type="text" 
              :placeholder="$t('placeholders.search')" 
              class="w-full bg-transparent border-none text-slate-800 placeholder-slate-400 text-lg sm:text-2xl focus:outline-none font-medium"
            />
            <button v-if="searchQuery" @click="searchQuery = ''" class="text-slate-400 hover:text-slate-600 p-2 mr-2 text-lg font-bold">
              ✕
            </button>
          </motion.div>

          <!-- Luxury Autocomplete Dropdown (UI/UX Pro Max) -->
          <AnimatePresence>
            <motion.div 
              v-if="showAutocomplete && (autocompleteSuggestions.destinations.length || autocompleteSuggestions.categories.length || autocompleteSuggestions.tours.length)" 
              :initial="{ opacity: 0, scale: 0.96, y: -6 }"
              :animate="{ opacity: 1, scale: 1, y: 0 }"
              :exit="{ opacity: 0, scale: 0.96, y: -6 }"
              :transition="{ type: 'spring', stiffness: 450, damping: 32 }"
              class="absolute top-full w-full max-w-4xl left-0 right-0 mx-auto max-h-[500px] overflow-y-auto mt-3 z-[100] bg-white/98 backdrop-blur-2xl rounded-3xl shadow-[0_30px_70px_rgba(6,45,77,0.35)] border border-[#c9a84c]/30 py-3 text-left divide-y divide-slate-100/90 custom-scrollbar"
            >
              <!-- Destinations Section -->
              <div v-if="autocompleteSuggestions.destinations.length" class="pb-2">
                <div class="px-6 sm:px-8 py-2 flex items-center justify-between">
                  <span class="text-[11px] font-extrabold text-[#c9a84c] uppercase tracking-widest">{{ localizedLabels.destinations }}</span>
                  <span class="text-[10px] text-slate-400 font-semibold">{{ autocompleteSuggestions.destinations.length }} locations</span>
                </div>
                <div class="px-2 space-y-1">
                  <button 
                    v-for="dest in autocompleteSuggestions.destinations" 
                    :key="dest.id" 
                    @click="handleSuggestionClick('destination', dest)" 
                    class="w-full text-left px-4 sm:px-6 py-2.5 rounded-2xl hover:bg-[#f0f9ff] flex items-center justify-between group transition-all cursor-pointer"
                  >
                    <div class="flex items-center gap-3.5 min-w-0 pr-2">
                      <div class="w-9 h-9 rounded-xl bg-slate-100 group-hover:bg-[#062d4d] group-hover:text-white flex items-center justify-center text-slate-600 shrink-0 transition-colors shadow-xs">
                        📍
                      </div>
                      <span class="text-sm font-bold text-slate-800 group-hover:text-[#062d4d] transition-colors truncate">
                        {{ getLocalized(dest.names, 'Destination') }}
                      </span>
                    </div>
                    <span class="text-xs font-semibold text-slate-400 group-hover:text-[#c9a84c] flex items-center gap-1 transition-colors">
                      <span>Explore</span>
                      <svg class="w-3.5 h-3.5 transform group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7"></path></svg>
                    </span>
                  </button>
                </div>
              </div>

              <!-- Categories Section -->
              <div v-if="autocompleteSuggestions.categories.length" class="py-2">
                <div class="px-6 sm:px-8 py-2 flex items-center justify-between">
                  <span class="text-[11px] font-extrabold text-[#c9a84c] uppercase tracking-widest">{{ localizedLabels.experiences }}</span>
                  <span class="text-[10px] text-slate-400 font-semibold">{{ autocompleteSuggestions.categories.length }} types</span>
                </div>
                <div class="px-2 space-y-1">
                  <button 
                    v-for="cat in autocompleteSuggestions.categories" 
                    :key="cat.id" 
                    @click="handleSuggestionClick('category', cat)" 
                    class="w-full text-left px-4 sm:px-6 py-2.5 rounded-2xl hover:bg-[#fdfaf2] flex items-center justify-between group transition-all cursor-pointer"
                  >
                    <div class="flex items-center gap-3.5 min-w-0 pr-2">
                      <div class="w-9 h-9 rounded-xl bg-amber-50 text-[#c9a84c] group-hover:bg-[#c9a84c] group-hover:text-white flex items-center justify-center shrink-0 transition-colors shadow-xs text-base">
                        {{ cat.icon || '✨' }}
                      </div>
                      <span class="text-sm font-bold text-slate-800 group-hover:text-[#062d4d] transition-colors truncate">
                        {{ getLocalized(cat.names, 'Category') }}
                      </span>
                    </div>
                    <span class="text-xs font-semibold text-slate-400 group-hover:text-[#c9a84c] flex items-center gap-1 transition-colors">
                      <span>Filter</span>
                      <svg class="w-3.5 h-3.5 transform group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7"></path></svg>
                    </span>
                  </button>
                </div>
              </div>

              <!-- Top Tours Section -->
              <div v-if="autocompleteSuggestions.tours.length" class="pt-2">
                <div class="px-6 sm:px-8 py-2 flex items-center justify-between">
                  <span class="text-[11px] font-extrabold text-[#c9a84c] uppercase tracking-widest">{{ localizedLabels.trending }}</span>
                  <span class="text-[10px] text-slate-400 font-semibold">Top Rated</span>
                </div>
                <div class="px-2 space-y-1 pb-1">
                  <button 
                    v-for="tour in autocompleteSuggestions.tours" 
                    :key="tour.id" 
                    @click="handleSuggestionClick('tour', tour)" 
                    class="w-full text-left px-3.5 sm:px-5 py-3 rounded-2xl hover:bg-slate-50/90 flex items-center gap-4 group transition-all cursor-pointer border border-transparent hover:border-slate-200"
                  >
                    <div class="w-14 h-14 rounded-xl overflow-hidden shrink-0 bg-slate-200 shadow-sm relative">
                      <img v-if="tour.imageUrl || tour.mainImage" :src="getFullImageUrl(tour.imageUrl || tour.mainImage)" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" />
                      <div v-else class="w-full h-full" :style="{ background: tour.bgGradient || 'linear-gradient(135deg,#063a5c,#c9a84c)' }"></div>
                      <span v-if="tour.originalPrice && tour.originalPrice > tour.price" class="absolute top-1 left-1 bg-red-600 text-white text-[9px] font-extrabold px-1 rounded-sm">
                        %
                      </span>
                    </div>
                    <div class="flex-1 min-w-0 pr-2">
                      <div class="text-sm font-extrabold text-slate-900 group-hover:text-[#062d4d] transition-colors leading-snug line-clamp-1">
                        {{ getLocalized(tour.names, 'Tour') }}
                      </div>
                      <div class="flex items-center gap-2.5 text-xs text-slate-500 mt-1">
                        <span class="inline-flex items-center gap-1 text-emerald-700 font-bold bg-emerald-50 px-2 py-0.5 rounded-md text-[11px]">
                          ★ {{ tour.rating || '4.9' }}
                        </span>
                        <span class="text-slate-300">·</span>
                        <span class="text-[11px] font-semibold text-slate-600">{{ tour.duration || 'Full Day' }}</span>
                      </div>
                    </div>
                    <div class="text-right shrink-0">
                      <div class="text-base font-extrabold text-[#062d4d]">
                        {{ currencyStore.formatPrice(tour.price) }}
                      </div>
                      <div v-if="tour.originalPrice && tour.originalPrice > tour.price" class="text-[11px] text-slate-400 line-through">
                        {{ currencyStore.formatPrice(tour.originalPrice) }}
                      </div>
                    </div>
                  </button>
                </div>
              </div>
            </motion.div>
          </AnimatePresence>
        </motion.div>
        </div>

        <!-- 5. Secondary Guarantee Row -->
        <motion.div 
          :initial="{ opacity: 0, y: 15 }" 
          :animate="{ opacity: 1, y: 0 }" 
          :transition="{ duration: 0.6, ease: 'easeOut', delay: 0.5 }"
          class="flex flex-wrap items-center justify-center gap-3 sm:gap-4 mt-5 sm:mt-6"
        >
          <span class="inline-flex items-center gap-2 text-xs sm:text-sm text-white/95 bg-[#062d4d]/75 backdrop-blur-md border border-white/20 px-5 py-2 rounded-full shadow-md">
            {{ localizedLabels.freeCancellation }}
          </span>
          <span class="inline-flex items-center gap-2 text-xs sm:text-sm text-white/95 bg-[#062d4d]/75 backdrop-blur-md border border-white/20 px-5 py-2 rounded-full shadow-md">
            {{ localizedLabels.flexibleDates }}
          </span>
          <span class="inline-flex items-center gap-2 text-xs sm:text-sm text-white/95 bg-[#062d4d]/75 backdrop-blur-md border border-white/20 px-5 py-2 rounded-full shadow-md">
            {{ localizedLabels.reservePayLater }}
          </span>
          <span class="inline-flex items-center gap-2 text-xs sm:text-sm text-white/95 bg-[#062d4d]/75 backdrop-blur-md border border-white/20 px-5 py-2 rounded-full shadow-md">
            {{ localizedLabels.vipConcierge }}
          </span>
        </motion.div>
      </div>
    </header>

    <!-- Horizontal Sticky Filter Bar (Enlarged & Luxury Styled) -->
    <div class="bg-white sticky top-16 md:top-20 z-30 border-b border-slate-200 shadow-[0_4px_25px_rgba(6,58,92,0.04)] p-0 transition-all">
      <div class="max-w-full md:max-w-7xl 2xl:max-w-[1720px] mx-auto px-4 sm:px-8 lg:px-12 2xl:px-16 py-2 sm:py-2.5 flex items-center gap-4 sm:gap-6 relative">
        
        <!-- Utility Buttons (Left - Visible Overflow) -->
        <div class="flex items-center gap-3 pr-4 sm:pr-6 border-r border-slate-200 shrink-0 relative z-40">
          <!-- Luxury Calendar Popover (UI/UX Pro Max) -->
          <div class="relative" ref="calendarPopoverRef">
            <button 
              type="button"
              @click.stop="toggleCalendarPopover"
              class="h-11 sm:h-12 px-4 rounded-full border transition-all flex items-center gap-2.5 shadow-xs cursor-pointer select-none"
              :class="selectedDate 
                ? 'bg-[#062d4d] border-[#062d4d] text-white shadow-md font-bold' 
                : (showCalendarPopover ? 'border-[#c9a84c] ring-2 ring-[#c9a84c]/30 bg-white text-slate-900' : 'bg-white border-slate-200 hover:border-[#c9a84c] text-slate-700 hover:bg-slate-50')"
              title="Select Travel Date"
            >
              <svg class="w-4 h-4 shrink-0" :class="selectedDate ? 'text-[#c9a84c]' : 'text-slate-600'" fill="none" stroke="currentColor" stroke-width="2.2" viewBox="0 0 24 24">
                <rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect>
                <line x1="16" y1="2" x2="16" y2="6"></line>
                <line x1="8" y1="2" x2="8" y2="6"></line>
                <line x1="3" y1="10" x2="21" y2="10"></line>
              </svg>
              <span class="text-xs sm:text-sm font-semibold whitespace-nowrap">
                {{ formattedSelectedDate || $t('tours.travelDate') || 'Travel Date' }}
              </span>
              <button 
                v-if="selectedDate" 
                @click.stop="selectedDate = ''" 
                class="ml-1 w-4 h-4 rounded-full bg-white/20 hover:bg-white/30 text-white flex items-center justify-center text-xs font-bold transition-colors"
                title="Clear date"
              >
                ✕
              </button>
            </button>

            <!-- Luxury Floating Calendar Popover -->
            <Transition
              enter-active-class="transition duration-200 ease-out"
              enter-from-class="transform scale-95 opacity-0 -translate-y-2"
              enter-to-class="transform scale-100 opacity-100 translate-y-0"
              leave-active-class="transition duration-150 ease-in"
              leave-from-class="transform scale-100 opacity-100 translate-y-0"
              leave-to-class="transform scale-95 opacity-0 -translate-y-2"
            >
              <div 
                v-if="showCalendarPopover" 
                @click.stop
                class="absolute left-0 top-full mt-3 w-80 sm:w-88 bg-white/98 backdrop-blur-2xl border border-slate-200/90 rounded-3xl shadow-[0_25px_60px_rgba(6,45,77,0.25)] p-4 sm:p-5 z-50 text-left"
              >
                <!-- Quick Date Presets -->
                <div class="flex items-center gap-1.5 pb-3 border-b border-slate-100 overflow-x-auto scrollbar-none">
                  <button 
                    type="button" 
                    @click="setDatePreset('today')"
                    class="px-3 py-1 rounded-full text-[11px] font-bold bg-slate-100 hover:bg-[#062d4d] hover:text-white text-slate-700 transition-colors cursor-pointer whitespace-nowrap shadow-2xs"
                  >
                    {{ $t('calendar.today') }}
                  </button>
                  <button 
                    type="button" 
                    @click="setDatePreset('tomorrow')"
                    class="px-3 py-1 rounded-full text-[11px] font-bold bg-slate-100 hover:bg-[#062d4d] hover:text-white text-slate-700 transition-colors cursor-pointer whitespace-nowrap shadow-2xs"
                  >
                    {{ $t('calendar.tomorrow') }}
                  </button>
                  <button 
                    type="button" 
                    @click="setDatePreset('weekend')"
                    class="px-3 py-1 rounded-full text-[11px] font-bold bg-amber-50 hover:bg-[#c9a84c] hover:text-white text-[#9a7d2e] transition-colors cursor-pointer whitespace-nowrap shadow-2xs"
                  >
                    {{ $t('calendar.weekend') }}
                  </button>
                  <button 
                    type="button" 
                    @click="setDatePreset('nextWeek')"
                    class="px-3 py-1 rounded-full text-[11px] font-bold bg-slate-100 hover:bg-[#062d4d] hover:text-white text-slate-700 transition-colors cursor-pointer whitespace-nowrap shadow-2xs"
                  >
                    {{ $t('calendar.nextWeek') }}
                  </button>
                </div>

                <!-- Month Navigation Header -->
                <div class="flex items-center justify-between py-3">
                  <span class="text-sm font-extrabold text-[#062d4d] capitalize tracking-wide font-serif">
                    {{ currentMonthYearLabel }}
                  </span>
                  <div class="flex items-center gap-1">
                    <button 
                      type="button" 
                      @click="prevMonth"
                      :title="$t('calendar.prevMonth')"
                      class="w-7 h-7 rounded-full border border-slate-200 hover:border-[#c9a84c] hover:bg-slate-50 flex items-center justify-center text-slate-600 transition-colors cursor-pointer"
                    >
                      ‹
                    </button>
                    <button 
                      type="button" 
                      @click="nextMonth"
                      :title="$t('calendar.nextMonth')"
                      class="w-7 h-7 rounded-full border border-slate-200 hover:border-[#c9a84c] hover:bg-slate-50 flex items-center justify-center text-slate-600 transition-colors cursor-pointer"
                    >
                      ›
                    </button>
                  </div>
                </div>

                <!-- Weekday Headers -->
                <div class="grid grid-cols-7 gap-1 text-center mb-1">
                  <span v-for="d in calendarWeekdays" :key="d" class="text-[10px] uppercase font-extrabold text-slate-400 py-1">
                    {{ d }}
                  </span>
                </div>

                <!-- Days Grid -->
                <div class="grid grid-cols-7 gap-y-1 text-center">
                  <div
                    v-for="(day, dIdx) in calendarDays"
                    :key="dIdx"
                    class="relative"
                  >
                    <!-- Background band for in-range dates -->
                    <div 
                      v-if="day.isInRange || day.isRangeStart || day.isRangeEnd"
                      class="absolute inset-y-0 bg-[#062d4d]/10"
                      :class="[
                        day.isRangeStart && day.isRangeEnd ? 'inset-x-1 rounded-xl' :
                        day.isRangeStart ? 'left-1/2 right-0' :
                        day.isRangeEnd ? 'left-0 right-1/2' : 'inset-x-0'
                      ]"
                    ></div>
                    
                    <button
                      type="button"
                      :disabled="day.isPast"
                      @click="selectCalendarDay(day)"
                      class="h-9 w-9 mx-auto rounded-xl text-xs font-bold flex flex-col items-center justify-center relative transition-all cursor-pointer"
                      :class="[
                        day.isSelected 
                          ? 'bg-[#062d4d] text-white shadow-md ring-2 ring-[#c9a84c] scale-105 z-10' 
                          : (day.isPast 
                            ? 'text-slate-300 cursor-not-allowed opacity-40' 
                            : (day.isCurrentMonth 
                              ? (day.isInRange ? 'text-[#062d4d] font-extrabold' : 'text-slate-800 hover:bg-[#f0f9ff] hover:text-[#0369a1]')
                              : 'text-slate-400 hover:bg-slate-50'))
                      ]"
                    >
                      <span>{{ day.dayNumber }}</span>
                      <span v-if="day.isToday && !day.isSelected" class="w-1 h-1 rounded-full bg-[#c9a84c] absolute bottom-1"></span>
                    </button>
                  </div>
                </div>

                <!-- Footer Actions -->
                <div class="flex items-center justify-between pt-3 mt-2 border-t border-slate-100">
                  <button 
                    type="button" 
                    @click="selectedDate = ''; showCalendarPopover = false" 
                    class="text-[11px] font-bold text-slate-500 hover:text-red-600 transition-colors cursor-pointer"
                  >
                    {{ $t('calendar.resetDate') }}
                  </button>
                  <button 
                    type="button" 
                    @click="showCalendarPopover = false" 
                    class="px-3.5 py-1.5 bg-[#062d4d] text-white text-xs font-bold rounded-xl shadow-xs hover:bg-[#083b66] transition-colors cursor-pointer"
                  >
                    {{ $t('calendar.done') }}
                  </button>
                </div>
              </div>
            </Transition>
          </div>
          <!-- Filters -->
          <button @click="showAdvancedFilters = !showAdvancedFilters" class="w-11 h-11 sm:w-12 sm:h-12 rounded-full border border-slate-200 bg-white hover:border-[#c9a84c] shadow-sm flex items-center justify-center transition-all hover:scale-105 active:scale-95 text-slate-700 relative cursor-pointer" title="Filters">
            <svg class="w-5 h-5 text-slate-700" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4"></path></svg>
            <span v-if="activeFiltersCount > 0" class="absolute -top-1 -right-1 w-5 h-5 rounded-full bg-[#c9a84c] text-white flex items-center justify-center text-[10px] font-bold shadow-sm">{{ activeFiltersCount }}</span>
          </button>
        </div>
        
        <!-- Category Pill Tags (Dynamic & Localized - Scrollable) -->
        <div class="flex items-center gap-3 sm:gap-3.5 flex-1 min-w-0 overflow-x-auto scrollbar-none py-1" style="scrollbar-width: none; -ms-overflow-style: none;">
          <motion.button 
            @click="selectedCategoryId = ''"
            :whileHover="{ scale: 1.03 }"
            :whilePress="{ scale: 0.96 }"
            class="px-5 py-2.5 sm:px-6 sm:py-3 rounded-full text-sm font-semibold border transition-colors cursor-pointer whitespace-nowrap flex items-center gap-2 shadow-sm shrink-0"
            :class="!selectedCategoryId ? 'bg-[#062d4d] border-[#062d4d] text-white shadow-md font-bold' : 'bg-white border-slate-200 text-slate-700 hover:border-slate-300 hover:bg-slate-50'"
          >
            <span class="text-base">✨</span> {{ localizedLabels.allTours }}
          </motion.button>
          
          <motion.button 
            v-for="cat in categories" 
            :key="cat.id"
            @click="selectedCategoryId = selectedCategoryId === cat.id ? '' : cat.id"
            :whileHover="{ scale: 1.03 }"
            :whilePress="{ scale: 0.96 }"
            class="px-5 py-2.5 sm:px-6 sm:py-3 rounded-full text-sm font-semibold border transition-colors cursor-pointer whitespace-nowrap flex items-center gap-2 shadow-sm shrink-0"
            :class="selectedCategoryId === cat.id ? 'bg-[#062d4d] border-[#062d4d] text-white shadow-md font-bold' : 'bg-white border-slate-200 text-slate-700 hover:border-slate-300 hover:bg-slate-50'"
          >
            <span class="text-base">{{ cat.icon || '🏷️' }}</span>
            {{ getLocalized(cat.names, 'Category') }}
          </motion.button>
        </div>
      </div>
    </div>

    <!-- Main Booking Dashboard -->
    <main class="flex-1 w-full max-w-full md:max-w-7xl 2xl:max-w-[1720px] mx-auto px-4 sm:px-8 lg:px-12 2xl:px-16 py-8 sm:py-10 relative z-10">
      <div class="w-full">

        <!-- Advanced Collapsible Filter Panel (Price Range) -->
        <AnimatePresence>
          <motion.div 
            v-if="showAdvancedFilters" 
            :initial="{ opacity: 0, height: 0, marginTop: 0, marginBottom: 0 }"
            :animate="{ opacity: 1, height: 'auto', marginTop: 16, marginBottom: 40 }"
            :exit="{ opacity: 0, height: 0, marginTop: 0, marginBottom: 0 }"
            :transition="{ duration: 0.3, ease: 'easeInOut' }"
            class="bg-white border border-gold/20 rounded-3xl p-6 sm:p-8 shadow-lg flex flex-col md:flex-row md:items-center justify-between gap-6 overflow-hidden"
          >
            <div class="flex-1 max-w-md">
              <div class="flex justify-between items-center mb-3">
                <label for="filter-price-slider" class="text-xs font-bold tracking-wider text-muted uppercase font-sans">{{ localizedLabels.maxBudget }}</label>
                <span class="text-base font-bold text-dark font-mono bg-cream px-3.5 py-1 rounded-full shadow-inner">{{ currencyStore.formatPrice(maxPrice) }}</span>
              </div>
              <div class="flex items-center gap-4">
                <span class="text-xs text-muted font-mono font-semibold">{{ currencyStore.formatPrice(30) }}</span>
                <input 
                  id="filter-price-slider"
                  type="range" 
                  v-model.number="maxPrice" 
                  min="30" 
                  max="500" 
                  step="10"
                  class="flex-1 accent-sea-deep h-2.5 bg-gray-200 rounded-lg appearance-none cursor-pointer"
                />
                <span class="text-xs text-muted font-mono font-semibold">{{ currencyStore.formatPrice(500) }}</span>
              </div>
            </div>
            
            <button 
              @click="searchQuery = ''; selectedDestinationId = ''; selectedCategoryId = ''; selectedDate = ''; maxPrice = 500"
              class="bg-cream hover:bg-gold/20 border border-gold/40 text-dark py-3.5 px-7 rounded-2xl text-xs tracking-wider uppercase font-bold cursor-pointer font-sans transition-all shadow-sm active:scale-95"
            >
              {{ localizedLabels.resetFilters }}
            </button>
          </motion.div>
        </AnimatePresence>

        <!-- Results & Sorting Bar (Spacious Spacing Before Cards) -->
        <div id="results-bar" class="py-5 max-w-full md:max-w-7xl 2xl:max-w-[1720px] mx-auto flex flex-wrap items-center justify-between gap-4 font-sans mb-8 sm:mb-10 border-b border-slate-200/60 pb-5">
          <!-- Left: Results Count + Location -->
          <div class="flex items-center gap-3.5">
            <div class="text-lg sm:text-xl text-slate-900 font-extrabold font-serif">
              {{ sortedTours.length }} {{ localizedLabels.results }}
            </div>
            <div class="px-3.5 py-1.5 rounded-full bg-slate-100 text-slate-700 text-xs sm:text-sm font-semibold flex items-center gap-2 shadow-sm border border-slate-200/60 cursor-pointer hover:bg-slate-200 transition-colors" @click="selectedDestinationId = ''">
              <span>📍</span> {{ selectedDestinationId ? getLocalized(destinations.find(d => d.id === selectedDestinationId)?.names || {}, 'Destination') : (searchQuery || localizedLabels.allDestinations) }}
              <button v-if="selectedDestinationId || searchQuery" @click.stop="selectedDestinationId = ''; searchQuery = ''" class="ml-1 w-4 h-4 rounded-full bg-slate-300 hover:bg-slate-400 flex items-center justify-center transition-colors text-slate-700 font-bold text-xs">×</button>
            </div>
          </div>
          
          <!-- Right: Sort & Views -->
          <div class="flex items-center gap-4 sm:gap-6">
            <!-- Elegant Luxury Sort Dropdown -->
            <div class="relative" ref="sortDropdownRef">
              <button 
                @click="showSortDropdown = !showSortDropdown"
                type="button"
                class="flex items-center gap-2.5 bg-white border border-slate-200 hover:border-[#c9a84c] rounded-full px-4 py-2 sm:px-5 sm:py-2.5 shadow-xs hover:shadow-md transition-all cursor-pointer select-none"
                :class="showSortDropdown ? 'ring-2 ring-[#c9a84c]/40 border-[#c9a84c]' : ''"
              >
                <svg class="w-4 h-4 text-[#062d4d]" fill="none" stroke="currentColor" stroke-width="2.2" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M3 7.5L7.5 3m0 0L12 7.5M7.5 3v13.5m13.5 0L16.5 21m0 0L12 16.5m4.5 4.5V7.5"></path>
                </svg>
                <span class="text-xs sm:text-sm font-bold text-slate-800">{{ currentSortLabel }}</span>
                <svg class="w-3.5 h-3.5 text-slate-400 transition-transform duration-200" :class="{ 'rotate-180 text-[#062d4d]': showSortDropdown }" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M19 9l-7 7-7-7"></path>
                </svg>
              </button>

              <!-- Luxury Floating Sort Menu -->
              <Transition
                enter-active-class="transition duration-150 ease-out"
                enter-from-class="transform scale-95 opacity-0 -translate-y-2"
                enter-to-class="transform scale-100 opacity-100 translate-y-0"
                leave-active-class="transition duration-100 ease-in"
                leave-from-class="transform scale-100 opacity-100 translate-y-0"
                leave-to-class="transform scale-95 opacity-0 -translate-y-2"
              >
                <div 
                  v-if="showSortDropdown" 
                  class="absolute right-0 mt-2.5 w-60 bg-white/98 backdrop-blur-xl border border-slate-200/90 rounded-2xl shadow-[0_15px_35px_rgba(6,45,77,0.12)] py-2 z-50 text-left"
                >
                  <div class="px-4 py-1.5 text-[10px] uppercase font-extrabold tracking-wider text-slate-400 border-b border-slate-100">
                    {{ $t('tours.sortBy') || 'Sort Experiences By' }}
                  </div>
                  <div class="p-1 space-y-0.5">
                    <button 
                      v-for="opt in sortOptions" 
                      :key="opt.value"
                      @click="sortBy = opt.value; showSortDropdown = false"
                      class="w-full px-3.5 py-2.5 rounded-xl text-xs sm:text-sm font-semibold flex items-center justify-between transition-colors cursor-pointer group"
                      :class="sortBy === opt.value ? 'bg-[#f0f9ff] text-[#0369a1] font-bold' : 'text-slate-700 hover:bg-slate-50 hover:text-slate-900'"
                    >
                      <div class="flex items-center gap-2.5">
                        <span class="text-sm">{{ opt.icon }}</span>
                        <span>{{ opt.label }}</span>
                      </div>
                      <svg v-if="sortBy === opt.value" class="w-4 h-4 text-[#0284c7] shrink-0" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                        <polyline points="20 6 9 17 4 12"></polyline>
                      </svg>
                    </button>
                  </div>
                </div>
              </Transition>
            </div>

            <!-- Luxury Segmented Grid/List Switcher -->
            <div class="flex items-center bg-slate-100/90 border border-slate-200/80 rounded-full p-1 shadow-inner gap-1">
              <button 
                @click="viewMode = 'grid'"
                class="px-4 py-1.5 sm:px-5 sm:py-2 rounded-full text-xs sm:text-sm transition-all duration-300 cursor-pointer font-bold flex items-center gap-1.5"
                :class="viewMode === 'grid' ? 'bg-[#062d4d] text-white shadow-md scale-100' : 'text-slate-600 hover:text-slate-900 hover:bg-slate-200/60'"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                  <rect x="3" y="3" width="7" height="7" rx="1.5"></rect>
                  <rect x="14" y="3" width="7" height="7" rx="1.5"></rect>
                  <rect x="14" y="14" width="7" height="7" rx="1.5"></rect>
                  <rect x="3" y="14" width="7" height="7" rx="1.5"></rect>
                </svg>
                <span>{{ localizedLabels.grid }}</span>
              </button>
              <button 
                @click="viewMode = 'list'"
                class="px-4 py-1.5 sm:px-5 sm:py-2 rounded-full text-xs sm:text-sm transition-all duration-300 cursor-pointer font-bold flex items-center gap-1.5"
                :class="viewMode === 'list' ? 'bg-[#062d4d] text-white shadow-md scale-100' : 'text-slate-600 hover:text-slate-900 hover:bg-slate-200/60'"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                  <line x1="8" y1="6" x2="21" y2="6" stroke-linecap="round"></line>
                  <line x1="8" y1="12" x2="21" y2="12" stroke-linecap="round"></line>
                  <line x1="8" y1="18" x2="21" y2="18" stroke-linecap="round"></line>
                  <circle cx="4" cy="6" r="1.5" fill="currentColor"></circle>
                  <circle cx="4" cy="12" r="1.5" fill="currentColor"></circle>
                  <circle cx="4" cy="18" r="1.5" fill="currentColor"></circle>
                </svg>
                <span>{{ localizedLabels.list }}</span>
              </button>
            </div>
          </div>
        </div>

        <!-- MAIN CONTENT LIST/GRID -->
        <div v-if="loading" class="flex flex-col items-center justify-center py-24 text-center">
          <svg class="animate-spin h-10 w-10 text-gold mb-4" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
        </div>

        <div v-else>
          <!-- Empty State -->
          <div v-if="sortedTours.length === 0" class="bg-white border border-gray-200 rounded-2xl py-24 text-center px-6 shadow-sm">
            <span class="text-5xl block mb-5">🐪</span>
            <h3 class="font-sans text-2xl font-bold text-dark mb-3">{{ localizedLabels.noToursFound }}</h3>
            <p class="text-muted text-sm max-w-sm mx-auto leading-relaxed font-sans mb-6">
              {{ localizedLabels.noToursDesc }}
            </p>
            <button @click="searchQuery = ''; selectedDestinationId = ''; selectedCategoryId = ''; maxPrice = 500" class="bg-sea-deep text-white px-6 py-3 rounded-full text-sm font-bold shadow-md hover:bg-sea transition-colors cursor-pointer">
              {{ localizedLabels.viewAllTours }}
            </button>
          </div>

          <!-- Tour Cards rendering (Exact Reference Match) -->
          <div v-else :class="viewMode === 'grid' ? 'grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 2xl:grid-cols-5 gap-5 sm:gap-6' : 'flex flex-col gap-5 sm:gap-6'">
            <motion.div 
              v-for="trip in paginatedTours" 
              :key="trip.id"
              :whileHover="{ y: -6 }"
              :transition="{ type: 'spring', stiffness: 300, damping: 25 }"
              class="group bg-white rounded-2xl border border-slate-100/90 shadow-[0_4px_20px_rgba(6,58,92,0.06)] hover:shadow-[0_16px_36px_rgba(6,58,92,0.12)] transition-colors duration-300 flex flex-col h-full cursor-pointer pb-1"
              :class="viewMode === 'grid' ? '' : 'md:flex-row'"
              @click="openDetailsPage(trip)"
            >
              <!-- Image Header -->
              <div class="relative w-full overflow-hidden rounded-t-2xl bg-slate-100 shrink-0" :class="viewMode === 'grid' ? 'aspect-[4/3]' : 'md:w-[40%] aspect-[4/3] md:aspect-auto md:h-full min-h-[250px]'">
                <div 
                  class="absolute inset-0 bg-cover bg-center transition-transform duration-500 group-hover:scale-105"
                  :style="{ backgroundImage: (trip.imageUrl || trip.mainImage) ? `url(${getFullImageUrl(trip.imageUrl || trip.mainImage)})` : (trip.bgGradient || 'linear-gradient(135deg,#063a5c,#c9a84c)') }"
                ></div>
                
                <!-- Top-Left Badges -->
                <div class="absolute top-3 left-3 z-10 flex flex-col gap-1.5 items-start">
                  <div v-if="trip.isTopRated" class="px-2.5 py-1 bg-[#062d4d] rounded shadow-sm">
                    <span class="text-[9px] font-extrabold uppercase tracking-widest text-white">{{ localizedLabels.topRated }}</span>
                  </div>
                  <div v-if="trip.isBestseller" class="px-2.5 py-1 bg-[#d89c25] rounded shadow-sm">
                    <span class="text-[9px] font-extrabold uppercase tracking-widest text-slate-900">{{ localizedLabels.bestseller }}</span>
                  </div>
                  <div v-if="trip.isInHighDemand" class="px-2.5 py-1 bg-orange-500 rounded shadow-sm flex items-center gap-1">
                    <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" stroke-width="3" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path></svg>
                    <span class="text-[9px] font-extrabold uppercase tracking-widest text-white">{{ localizedLabels.inHighDemand }}</span>
                  </div>
                </div>
                
                <!-- Top-Right Wishlist & Share -->
                <div class="absolute top-3 right-3 z-10 flex flex-col gap-2">
                  <button 
                    @click.stop="toggleFavoriteTour(trip.id)"
                    class="w-8 h-8 rounded-full flex items-center justify-center transition-all duration-300 shadow-sm cursor-pointer"
                    :class="isTourFavorite(trip.id) ? 'bg-white text-rose-500 shadow-md scale-110' : 'bg-white/85 backdrop-blur-sm text-slate-500 hover:text-rose-500 hover:bg-white'"
                    :title="isTourFavorite(trip.id) ? localizedLabels.removeFromFavorites : localizedLabels.addToFavorites"
                  >
                    <svg class="w-4 h-4 transition-transform duration-300" :class="isTourFavorite(trip.id) ? 'fill-rose-500 text-rose-500 scale-110' : 'fill-none'" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"></path>
                    </svg>
                  </button>
                  <button 
                    @click.stop="shareTour(trip)"
                    class="w-8 h-8 rounded-full bg-white/80 backdrop-blur-md flex items-center justify-center text-dark hover:bg-white transition-all shadow-sm premium-press cursor-pointer"
                    :title="localizedLabels.shareTour"
                  >
                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                      <circle cx="18" cy="5" r="3"></circle>
                      <circle cx="6" cy="12" r="3"></circle>
                      <circle cx="18" cy="19" r="3"></circle>
                      <line x1="8.59" y1="13.51" x2="15.42" y2="17.49"></line>
                      <line x1="15.41" y1="6.51" x2="8.59" y2="10.49"></line>
                    </svg>
                  </button>
                </div>
              </div>

              <!-- Card Body -->
              <div class="p-6 sm:p-7 flex flex-col flex-1 gap-3.5 pb-6" :class="viewMode === 'grid' ? '' : 'md:w-[60%]' ">
                <!-- Metadata Row -->
                <div class="flex items-center gap-3 text-xs text-slate-500 font-sans font-medium">
                  <span class="flex items-center gap-1.5">
                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
                    {{ formatDuration(trip.duration) }}
                  </span>
                  <span class="flex items-center gap-1.5">
                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 12 18"></polyline></svg>
                    {{ trip.startTime || localizedLabels.flexible }}
                  </span>
                </div>

                <h4 class="font-playfair text-base sm:text-lg font-bold text-slate-900 leading-snug line-clamp-2 min-h-[44px]">
                  {{ getLocalized(trip.names, trip.title || 'Unnamed Tour') }}
                </h4>
                
                <!-- Rating Row -->
                <div class="flex items-center gap-1.5 text-xs text-slate-600">
                  <span class="text-emerald-600 font-bold flex items-center gap-1">
                    <svg class="w-3.5 h-3.5 fill-current" viewBox="0 0 24 24">
                      <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
                    </svg>
                    {{ trip.rating || getTourRatingDetails(trip.id).rating }}
                  </span>
                  <span class="text-slate-400 font-bold">·</span>
                  <span class="text-slate-500 font-medium">{{ trip.reviewCount || getTourRatingDetails(trip.id).reviews }}</span>
                </div>

                <!-- Tags Row -->
                <div class="flex items-center justify-between text-xs pt-2 border-t border-slate-100 font-medium">
                  <span v-if="trip.isPrivateOption" class="flex items-center gap-1 text-amber-600">👑 {{ localizedLabels.privateOption }}</span>
                  <span v-else></span>
                  <span class="bg-slate-50 px-2 py-0.5 rounded text-[10px] uppercase font-bold tracking-wider text-slate-600 border border-slate-100 shadow-sm">🏷️ {{ getLocalized(categories.find(c => c.id === trip.categoryId)?.names || {}, 'Tour') }}</span>
                </div>

                <!-- Footer (Anchored) -->
                <div class="mt-auto pt-3 border-t border-slate-100 flex items-center justify-between">
                  <div class="flex flex-col">
                    <div class="flex items-baseline gap-1">
                      <span class="text-xs text-slate-500 font-medium">{{ localizedLabels.from }}</span>
                      <span v-if="trip.originalPrice" class="text-xs line-through text-slate-400">{{ currencyStore.formatPrice(trip.originalPrice) }}</span>
                      <span class="text-lg font-extrabold text-slate-900">{{ currencyStore.formatPrice(trip.price) }}</span>
                    </div>
                    <div class="flex items-center gap-1 mt-0.5">
                      <span v-if="trip.discountPercentage" class="bg-red-600 text-white text-[10px] font-bold px-1.5 py-0.5 rounded mr-1.5">-{{ Math.round(trip.discountPercentage) }}%</span>
                      <span class="text-xs text-slate-500 font-medium leading-normal">{{ localizedLabels.perPerson }}</span>
                    </div>
                  </div>
                  <button @click.stop="openBookingModal(trip)" class="w-10 h-10 rounded-full bg-[#062d4d] text-white hover:bg-[#c9a84c] flex items-center justify-center transition-all shadow-md hover:scale-105 shrink-0 cursor-pointer" :title="localizedLabels.viewDetails">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7"></path></svg>
                  </button>
                </div>
              </div>
            </motion.div>
          </div>

          <!-- PAGINATION CONTROLS -->
          <div v-if="totalPages > 1" class="mt-14 flex justify-center items-center gap-3">
            <button 
              @click="currentPage = Math.max(1, currentPage - 1)"
              :disabled="currentPage === 1"
              class="w-11 h-11 rounded-full border border-gray-200 bg-white hover:bg-gray-50 flex items-center justify-center text-dark transition-all disabled:opacity-40 disabled:cursor-not-allowed shadow-sm premium-press"
              aria-label="Previous page"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                <polyline points="15 18 9 12 15 6"></polyline>
              </svg>
            </button>

            <button 
              v-for="page in totalPages" 
              :key="page"
              @click="currentPage = page"
              class="w-11 h-11 rounded-full border text-sm font-bold font-sans transition-all premium-press shadow-sm flex items-center justify-center"
              :class="currentPage === page ? 'bg-sea-deep border-sea-deep text-white' : 'border-gray-200 bg-white text-dark hover:bg-gray-50'"
            >
              {{ page }}
            </button>

            <button 
              @click="currentPage = Math.min(totalPages, currentPage + 1)"
              :disabled="currentPage === totalPages"
              class="w-11 h-11 rounded-full border border-gray-200 bg-white hover:bg-gray-50 flex items-center justify-center text-dark transition-all disabled:opacity-40 disabled:cursor-not-allowed shadow-sm premium-press"
              aria-label="Next page"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                <polyline points="9 18 15 12 9 6"></polyline>
              </svg>
            </button>
          </div>
        </div>

      </div>
    </main>

    <!-- BOOKING FORM POPUP MODAL (Double column spec layout) -->
    <Transition name="fade">
      <div v-if="showBookingModal" class="fixed inset-0 z-[2000] flex items-center justify-center p-4 bg-[#0d1f2d]/85 backdrop-blur-md" @click="showBookingModal = false">
        
        <!-- SUCCESS CUE WINDOW (Luxury Boarding Pass Ticket & Invoice Receipt) -->
        <div v-if="bookingSuccess" class="relative w-full max-w-4xl overflow-hidden rounded-2xl bg-[#faf7f2] border border-[#c9a84c]/30 shadow-2xl flex flex-col md:flex-row transition-all transform duration-500 font-sans" @click.stop>
          
          <!-- Close button -->
          <button type="button" @click="showBookingModal = false" aria-label="Close success modal" class="absolute top-4 right-4 z-50 p-2 text-[#6b8a9a] hover:text-[#063a5c] hover:bg-black/5 rounded-full transition-all focus:outline-none focus:ring-2 focus:ring-[#c9a84c] cursor-pointer">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- Left side: Digital Boarding Pass Ticket -->
          <div class="w-full md:w-7/12 p-8 md:p-10 flex flex-col justify-between border-r border-dashed border-[#c9a84c]/30 relative bg-white">
            <!-- Ticket notch cutouts at the border intersection -->
            <div class="hidden md:block absolute -right-3 -top-3 w-6 h-6 rounded-full bg-[#0d1f2d]"></div>
            <div class="hidden md:block absolute -right-3 -bottom-3 w-6 h-6 rounded-full bg-[#0d1f2d]"></div>

            <div>
              <div class="flex items-center gap-3 mb-6">
                <div class="w-12 h-12 rounded-full bg-emerald-500/10 border border-emerald-500/30 flex items-center justify-center text-emerald-500 flex-shrink-0 animate-scale-up">
                  <svg class="w-6 h-6" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <div>
                  <span class="text-[9px] font-bold tracking-[0.2em] uppercase text-emerald-600">Booking Confirmed</span>
                  <h3 class="text-xl font-bold font-serif text-[#063a5c]">Your Ticket is Ready</h3>
                </div>
              </div>

              <!-- Ticket Grid details -->
              <div class="grid grid-cols-2 gap-x-6 gap-y-5 border-t border-b border-gray-100 py-6 my-6 text-sm">
                <div>
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">VIP Lead Passenger</span>
                  <span class="font-semibold text-dark font-jost">{{ bookingForm.name }}</span>
                </div>
                <div>
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">Booking Reference</span>
                  <span class="font-bold text-[#c9a84c] font-mono uppercase">{{ bookingReference }}</span>
                </div>
                <div>
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">Adventure / Tour</span>
                  <span class="font-semibold text-dark font-serif">{{ selectedTourForBooking ? getLocalized(selectedTourForBooking.names, 'Luxury Tour') : 'Egypt Adventure' }}</span>
                </div>
                <div>
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">Departure Date</span>
                  <span class="font-semibold text-dark font-jost">{{ bookingForm.date || 'TBD (Coord. by Concierge)' }}</span>
                </div>
                <div>
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">Group Size & Type</span>
                  <span class="font-semibold text-dark font-jost">
                    {{ bookingForm.guests }} {{ Number(bookingForm.guests) === 1 ? 'Guest' : 'Guests' }} 
                    <span class="text-gold font-bold">({{ bookingForm.tripType === 'private' ? 'Private' : 'Group' }})</span>
                  </span>
                </div>
                <div>
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">Guide Language</span>
                  <span class="font-semibold text-dark font-jost">{{ getLanguageLabel(bookingForm.guideLanguage) }}</span>
                </div>
                <div>
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">WhatsApp & Phone</span>
                  <span class="font-semibold text-dark font-jost block truncate">
                    📞 {{ bookingForm.phone }}
                    <span v-if="bookingForm.whatsapp" class="block text-[11px] text-emerald-600">💬 WA: {{ bookingForm.whatsapp }}</span>
                  </span>
                </div>
                <div>
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">Pickup service</span>
                  <span class="font-semibold text-dark font-jost capitalize">
                    {{ bookingForm.pickupRequired === 'yes' ? 'Yes' : 'No' }}
                    <span v-if="bookingForm.pickupRequired === 'yes' && bookingForm.hotelName" class="block text-[10px] text-gold font-semibold truncate">
                      🏨 {{ bookingForm.hotelName }} {{ bookingForm.roomNumber ? `(Rm ${bookingForm.roomNumber})` : '' }}
                    </span>
                  </span>
                </div>
                <div v-if="bookingForm.passportFileName" class="col-span-2">
                  <span class="block text-[9px] uppercase tracking-wider text-muted font-bold mb-1">Passport Attachment</span>
                  <span class="font-semibold text-emerald-600 font-jost text-xs block truncate">📄 {{ bookingForm.passportFileName }} (Uploaded Successfully)</span>
                </div>
              </div>

              <!-- Special Request note -->
              <div v-if="bookingForm.notes" class="bg-cream/35 border border-gold/10 rounded-lg p-3 text-xs text-muted italic mb-6">
                <strong>Concierge Note:</strong> "{{ bookingForm.notes }}"
              </div>
            </div>

            <!-- SVG Barcode representation at the bottom -->
            <div class="flex flex-col items-center pt-4">
              <div class="w-full max-w-[280px] h-12 flex items-center justify-between opacity-85">
                <!-- Simple barcode simulation lines -->
                <svg viewBox="0 0 100 20" width="100%" height="100%" class="text-dark">
                  <rect x="0" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="3" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="5" y="0" width="4" height="20" fill="currentColor"/>
                  <rect x="10" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="12" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="15" y="0" width="3" height="20" fill="currentColor"/>
                  <rect x="19" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="21" y="0" width="4" height="20" fill="currentColor"/>
                  <rect x="26" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="29" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="31" y="0" width="3" height="20" fill="currentColor"/>
                  <rect x="35" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="38" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="40" y="0" width="4" height="20" fill="currentColor"/>
                  <rect x="45" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="48" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="50" y="0" width="3" height="20" fill="currentColor"/>
                  <rect x="54" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="57" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="59" y="0" width="4" height="20" fill="currentColor"/>
                  <rect x="64" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="67" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="69" y="0" width="3" height="20" fill="currentColor"/>
                  <rect x="73" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="76" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="78" y="0" width="4" height="20" fill="currentColor"/>
                  <rect x="83" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="86" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="88" y="0" width="3" height="20" fill="currentColor"/>
                  <rect x="92" y="0" width="2" height="20" fill="currentColor"/>
                  <rect x="95" y="0" width="1" height="20" fill="currentColor"/>
                  <rect x="97" y="0" width="3" height="20" fill="currentColor"/>
                </svg>
              </div>
              <span class="text-[9px] text-[#6b8a9a] uppercase tracking-[0.2em] font-mono mt-1">*{{ bookingReference }}*</span>
            </div>

          </div>

          <!-- Right side: Payment Details / Receipt invoice -->
          <div class="w-full md:w-5/12 bg-[#063a5c] text-white p-8 md:p-10 flex flex-col justify-between">
            <div class="absolute inset-0 opacity-5 mix-blend-overlay bg-repeat bg-center" style="background-image: radial-gradient(#c9a84c 1px, transparent 1px); background-size: 20px 20px;"></div>
            
            <div class="relative z-10 flex-1 flex flex-col justify-between">
              <div>
                <span class="inline-block px-3 py-1 text-[9px] font-bold tracking-[0.2em] uppercase text-[#c9a84c] border border-[#c9a84c]/30 rounded-full mb-6 bg-[#c9a84c]/10">
                  Invoice Receipt
                </span>
                
                <h4 class="text-xs font-semibold uppercase tracking-wider text-[#8eafc2] mb-4">Payment Summary</h4>
                
                <!-- Invoice lines -->
                <div class="space-y-3.5 text-sm text-[#8eafc2]">
                  <div class="flex justify-between items-center">
                    <span>Base Excursion Price (per person)</span>
                    <span class="text-white font-semibold font-mono">{{ selectedTourForBooking ? currencyStore.formatPrice(selectedTourForBooking.price) : '' }}</span>
                  </div>
                  <div class="flex justify-between items-center">
                    <span>Passengers</span>
                    <span class="text-white font-semibold font-mono">x{{ bookingForm.guests }}</span>
                  </div>
                  <div class="flex justify-between items-center">
                    <span>Package Tier Premium</span>
                    <span class="text-white font-semibold font-mono">
                      {{ currencyStore.formatPrice(bookingForm.packageOption === 'elite' ? 50 : bookingForm.packageOption === 'premium' ? 25 : 0) }}
                    </span>
                  </div>
                  <div class="flex justify-between items-center">
                    <span>Airport Transfer Pickup Fee</span>
                    <span class="text-white font-semibold font-mono">
                      {{ currencyStore.formatPrice(bookingForm.pickupRequired === 'yes' ? 15 : 0) }}
                    </span>
                  </div>
                  
                  <div class="border-t border-[#8eafc2]/20 my-4 pt-4 flex justify-between items-center text-white">
                    <span class="font-bold text-base font-serif">Total Due</span>
                    <span class="text-2xl font-bold font-serif text-gold">{{ currencyStore.formatPrice(calculateFinalPrice()) }}</span>
                  </div>
                </div>
              </div>

              <!-- Payment status / Stripe validation -->
              <div class="my-6 bg-black/25 border border-white/10 rounded-xl p-4 flex items-center justify-between text-xs">
                <div>
                  <span class="text-[#8eafc2]/60 uppercase text-[9px] tracking-wider block">Status</span>
                  <span class="text-emerald-400 font-bold uppercase tracking-wider">Pending Verification</span>
                </div>
                <div class="text-right">
                  <span class="text-[#8eafc2]/60 uppercase text-[9px] tracking-wider block">Secured Link</span>
                  <span class="text-[#c9a84c] font-semibold">Concierge Review</span>
                </div>
              </div>

              <!-- VIP action buttons -->
              <div class="space-y-3 pt-6 border-t border-[#8eafc2]/20">
                <button type="button" @click="triggerPrint" class="w-full bg-[#c9a84c] hover:bg-[#bfa044] text-dark font-bold text-xs tracking-widest uppercase py-3.5 px-6 rounded-full transition-all duration-200 shadow-md hover:-translate-y-[1px] cursor-pointer">
                  {{ $t('booking.actions.print') }}
                </button>
                <button type="button" @click="showBookingModal = false" class="w-full bg-white/10 hover:bg-white/20 text-white font-bold text-xs tracking-widest uppercase py-3.5 px-6 rounded-full transition-all duration-200 border border-white/20 cursor-pointer">
                  {{ $t('booking.actions.back') }}
                </button>
              </div>

            </div>
          </div>
          
        </div>

        <!-- TWO COLUMN RESERVATION FORM -->
        <div v-else class="relative w-full max-w-4xl overflow-hidden rounded-2xl bg-[#faf7f2] border border-[#c9a84c]/30 shadow-2xl flex flex-col md:flex-row transition-all transform duration-300" @click.stop>
          
          <!-- Close button -->
          <button type="button" @click="showBookingModal = false" aria-label="Close booking modal" class="absolute top-4 right-4 z-10 p-2 text-[#6b8a9a] hover:text-[#063a5c] hover:bg-black/5 rounded-full transition-all focus:outline-none focus:ring-2 focus:ring-[#c9a84c]">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- Column 1: Brand Reassurance & Selected Excursion Info -->
          <div class="w-full md:w-5/12 bg-[#063a5c] text-white p-8 md:p-10 flex flex-col justify-between relative overflow-hidden">
            <div class="absolute inset-0 opacity-5 mix-blend-overlay bg-repeat bg-center" style="background-image: radial-gradient(#c9a84c 1px, transparent 1px); background-size: 20px 20px;"></div>
            
            <div class="relative z-10">
              <span class="inline-block px-3 py-1 text-[10px] font-medium tracking-[0.2em] uppercase text-[#c9a84c] border border-[#c9a84c]/40 rounded-full mb-6 bg-[#c9a84c]/10">
                Exclusive Experiences
              </span>
              
              <h2 class="text-3xl font-extrabold font-serif tracking-tight leading-tight text-white mb-4">
                Begin Your <br>Luxury Egypt <br>Journey
              </h2>
              
              <p class="text-sm text-[#8eafc2] leading-relaxed mb-6">
                Crafted by certified Egyptologists and luxury hospitality specialists, our tours offer unmatched access and elite accommodations.
              </p>

              <!-- Selected tour card info summary box -->
              <div v-if="selectedTourForBooking" class="bg-black/20 border border-white/10 rounded-xl p-4 mb-6">
                <span class="text-[9px] uppercase tracking-wider text-gold font-bold block mb-1">{{ $t('booking.modal.selectedPackage') }}</span>
                <span class="text-sm font-semibold block font-serif">{{ getLocalized(selectedTourForBooking.names, 'Unnamed Tour') }}</span>
                <span class="text-xs text-[#8eafc2] mt-1 block">{{ $t('booking.modal.value') }}: {{ currencyStore.formatPrice(selectedTourForBooking.price) }} {{ $t('booking.modal.perPerson') }}</span>
              </div>

              <!-- Reassurance list -->
              <div class="space-y-4">
                <div class="flex items-start space-x-3">
                  <div class="flex-shrink-0 w-5 h-5 rounded-full bg-[#c9a84c]/20 flex items-center justify-center text-[#c9a84c]">
                    <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-xs font-semibold uppercase tracking-wider text-white">{{ $t('booking.modal.eliteGuides') }}</h4>
                    <p class="text-xs text-[#8eafc2] mt-0.5">{{ $t('booking.modal.eliteGuidesDesc') }}</p>
                  </div>
                </div>
                
                <div class="flex items-start space-x-3">
                  <div class="flex-shrink-0 w-5 h-5 rounded-full bg-[#c9a84c]/20 flex items-center justify-center text-[#c9a84c]">
                    <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-xs font-semibold uppercase tracking-wider text-white">{{ $t('booking.modal.flexibleCancel') }}</h4>
                    <p class="text-xs text-[#8eafc2] mt-0.5">{{ $t('booking.modal.flexibleCancelDesc') }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Stripe / Gold Badge -->
            <div class="relative z-10 mt-8 pt-6 border-t border-[#8eafc2]/20 flex items-center justify-between">
              <div class="text-left">
                <div class="text-xs text-[#8eafc2] uppercase tracking-widest font-semibold">{{ $t('booking.modal.stripeSecured') }}</div>
                <div class="text-[10px] text-[#8eafc2]/60 mt-0.5">{{ $t('booking.modal.encryptedConnection') }}</div>
              </div>
              <div class="text-right">
                <div class="text-xs text-[#c9a84c] font-bold tracking-widest">{{ $t('booking.modal.seedoraGold') }}</div>
                <div class="text-[10px] text-[#8eafc2]/60 mt-0.5">{{ $t('booking.modal.signatureService') }}</div>
              </div>
            </div>
          </div>

          <!-- Column 2: Inputs Form -->
          <form @submit.prevent="submitBooking" class="w-full md:w-7/12 p-10 md:p-14 flex flex-col justify-between">
            <div>
              <span class="text-[10px] tracking-[0.2em] text-gold uppercase font-bold text-center block mb-2.5 font-sans">{{ $t('booking.modal.subtitle') }}</span>
              <h3 class="text-3xl font-bold text-dark mb-3 text-center font-serif">{{ $t('booking.modal.title') }}</h3>
              <p class="text-sea font-serif text-lg text-center mb-8 max-w-sm mx-auto border-b border-gold/15 pb-5">
                {{ selectedTourForBooking ? getLocalized(selectedTourForBooking.names, '') : '' }}
              </p>

              <div class="space-y-6">
                <div>
                  <label class="block text-[10px] font-bold text-muted mb-2 uppercase tracking-widest font-sans">{{ $t('booking.labels.fullName') }}</label>
                  <input v-model="bookingForm.name" type="text" required class="w-full px-5 py-4 bg-cream/20 border border-gold/25 rounded-lg outline-none text-sm font-jost text-dark placeholder-muted/65 transition-all duration-300 hover:border-gold/50 focus:border-gold focus:bg-white focus:ring-1 focus:ring-gold focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]" :placeholder="$t('booking.placeholders.fullName')">
                  <span v-if="bookingErrors.name" class="text-xs text-rose-600 mt-1.5 font-sans">{{ bookingErrors.name }}</span>
                </div>
                
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
                  <div>
                    <label class="block text-[10px] font-bold text-muted mb-2 uppercase tracking-widest font-sans">{{ $t('booking.labels.email') }}</label>
                    <input v-model="bookingForm.email" type="email" required class="w-full px-5 py-4 bg-cream/20 border border-gold/25 rounded-lg outline-none text-sm font-jost text-dark placeholder-muted/65 transition-all duration-300 hover:border-gold/50 focus:border-gold focus:bg-white focus:ring-1 focus:ring-gold focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]" :placeholder="$t('booking.placeholders.email')">
                    <span v-if="bookingErrors.email" class="text-xs text-rose-600 mt-1.5 font-sans">{{ bookingErrors.email }}</span>
                  </div>
                  <div>
                    <label class="block text-[10px] font-bold text-muted mb-2 uppercase tracking-widest font-sans">{{ $t('booking.labels.whatsapp') }}</label>
                    <input v-model="bookingForm.whatsapp" type="tel" required class="w-full px-5 py-4 bg-cream/20 border border-gold/25 rounded-lg outline-none text-sm font-jost text-dark placeholder-muted/65 transition-all duration-300 hover:border-gold/50 focus:border-gold focus:bg-white focus:ring-1 focus:ring-gold focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]" :placeholder="$t('booking.placeholders.whatsapp')">
                  </div>
                </div>

                <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
                  <div>
                    <label class="block text-[10px] font-bold text-muted mb-2 uppercase tracking-widest font-sans">{{ $t('booking.labels.tripType') }}</label>
                    <div class="relative">
                      <select v-model="bookingForm.tripType" class="w-full px-5 py-4 bg-cream/20 border border-gold/25 rounded-lg outline-none text-sm font-jost text-dark transition-all duration-300 hover:border-gold/50 focus:border-gold focus:bg-white focus:ring-1 focus:ring-gold focus:shadow-[0_0_12px_rgba(201,168,76,0.15)] appearance-none cursor-pointer pr-12">
                        <option value="group">{{ $t('booking.options.tripType.group') }}</option>
                        <option value="private">{{ $t('booking.options.tripType.private') }}</option>
                      </select>
                      <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-gold">
                        <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                          <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/>
                        </svg>
                      </div>
                    </div>
                  </div>
                  <div class="grid grid-cols-3 gap-3">
                    <div class="col-span-2">
                      <label class="block text-[10px] font-bold text-muted mb-2 uppercase tracking-widest font-sans">{{ $t('booking.labels.hotelName') }}</label>
                      <input v-model="bookingForm.hotelName" type="text" required class="w-full px-5 py-4 bg-cream/20 border border-gold/25 rounded-lg outline-none text-sm font-jost text-dark placeholder-muted/65 transition-all duration-300 hover:border-gold/50 focus:border-gold focus:bg-white focus:ring-1 focus:ring-gold focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]" :placeholder="$t('booking.placeholders.hotelName')">
                    </div>
                    <div class="col-span-1">
                      <label class="block text-[10px] font-bold text-muted mb-2 uppercase tracking-widest font-sans">{{ $t('booking.labels.roomNo') }}</label>
                      <input v-model="bookingForm.roomNumber" type="text" required class="w-full px-5 py-4 bg-cream/20 border border-gold/25 rounded-lg outline-none text-sm font-jost text-dark placeholder-muted/65 transition-all duration-300 hover:border-gold/50 focus:border-gold focus:bg-white focus:ring-1 focus:ring-gold focus:shadow-[0_0_12px_rgba(201,168,76,0.15)]" :placeholder="$t('booking.placeholders.roomNo')">
                    </div>
                  </div>
                </div>

                <div>
                  <label class="block text-[10px] font-bold text-muted mb-2 uppercase tracking-widest font-sans">{{ $t('booking.labels.passportPhoto') }}</label>
                  <div class="relative flex items-center justify-center border border-dashed border-gold/45 rounded-lg p-5 bg-cream/5 hover:bg-cream/15 transition-all duration-300">
                    <input type="file" @change="handlePassportUpload" accept="image/*" class="absolute inset-0 opacity-0 cursor-pointer w-full h-full">
                    <div class="text-center text-xs text-muted/80">
                      <span class="text-gold font-bold block mb-1">📎 {{ $t('booking.modal.uploadPassport') }}</span>
                      <span class="text-[10px] text-muted block">{{ bookingForm.passportFileName || $t('booking.modal.maxSize') }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Footer Action -->
            <div class="mt-10 pt-6 border-t border-gold/15 flex justify-between items-center bg-cream/30 p-6 rounded-xl border border-gold/10">
              <div class="text-left font-jost">
                <div class="text-[9px] text-muted tracking-widest uppercase font-bold">{{ $t('booking.modal.totalValue') }}</div>
                <div class="text-3xl text-gold font-bold font-serif">{{ selectedTourForBooking ? currencyStore.formatPrice(selectedTourForBooking.price) : '' }}</div>
                <div class="text-[9px] text-muted">{{ $t('booking.modal.perPerson') }}</div>
              </div>
              <button type="submit" :disabled="bookingLoading" class="bg-gradient-to-r from-gold to-[#bfa044] text-dark px-8 py-4 rounded-full font-bold uppercase text-[11px] tracking-widest shadow-md hover:translate-y-[-2px] hover:shadow-[0_6px_20px_rgba(201,168,76,0.3)] disabled:opacity-50 transition-all cursor-pointer">
                <span v-if="bookingLoading" class="flex items-center gap-1.5 justify-center">
                  <svg class="animate-spin h-3.5 w-3.5 text-dark" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  {{ $t('booking.actions.processing') }}
                </span>
                <span v-else>{{ $t('booking.actions.confirm') }}</span>
              </button>
            </div>
            
            <p class="text-[10px] text-muted text-center mt-4 italic tracking-wide leading-relaxed">
              {{ $t('booking.modal.noPayment') }}
            </p>
          </form>
        </div>

      </div>
    </Transition>

    <!-- Footer -->
    <Footer />
    
    <!-- Luxury Toast Notification -->
    <Transition name="toast">
      <div v-if="showToast" class="fixed bottom-8 right-8 z-[9999] bg-[#0d1f2d] border border-[#c9a84c]/30 rounded-md px-5 py-3.5 shadow-2xl text-white font-sans text-xs flex items-center gap-3">
        <svg class="w-4 h-4 text-gold flex-shrink-0 animate-bounce" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <span>{{ toastMessage }}</span>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* Core Apple / Emil Kowalski Tokens */
:root {
  --ease-out: cubic-bezier(0.23, 1, 0.32, 1);
  --ease-in-out: cubic-bezier(0.77, 0, 0.175, 1);
}

/* Page transition custom styling */
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;  
  overflow: hidden;
}
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;  
  overflow: hidden;
}
.line-clamp-1 {
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;  
  overflow: hidden;
}
.font-playfair {
  font-family: 'Playfair Display', serif;
}
.font-jost {
  font-family: 'Jost', sans-serif;
}
.font-cormorant {
  font-family: 'Cormorant Garamond', serif;
}

/* Scrollbar hidden for horizontal pills */
.scrollbar-none::-webkit-scrollbar {
  display: none;
}
.scrollbar-none {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

/* Toast notifications animate styles */
.fade-enter-active, .fade-leave-active {
  transition: opacity 250ms var(--ease-out);
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

.toast-enter-active, .toast-leave-active {
  transition: opacity 250ms var(--ease-out), transform 250ms var(--ease-out);
}
.toast-enter-from, .toast-leave-to {
  opacity: 0;
  transform: translateY(20px) scale(0.95);
}

/* Premium Component States */
.premium-btn {
  transition: transform 160ms var(--ease-out), opacity 160ms var(--ease-out), background-color 200ms ease, box-shadow 200ms ease;
}
.premium-btn:active {
  transform: scale(0.97);
}

/* Tour Cards Animation */
.tour-card {
  transition: transform 250ms var(--ease-out), box-shadow 250ms var(--ease-out);
  transform: scale(1);
}
@media (hover: hover) and (pointer: fine) {
  .tour-card:hover {
    transform: translateY(-4px) scale(1.005);
    box-shadow: 0 20px 40px rgba(0,0,0,0.06);
  }
}
.tour-card:active {
  transform: scale(0.98);
}
.tour-card-img-wrap {
  overflow: hidden;
  clip-path: inset(0 0 0 0); /* Force GPU */
}
.tour-card-img {
  transition: transform 600ms var(--ease-out);
}
@media (hover: hover) and (pointer: fine) {
  .tour-card:hover .tour-card-img {
    transform: scale(1.05);
  }
}

/* Stagger animations for cards */
.stagger-item {
  opacity: 0;
  transform: translateY(16px);
  animation: fadeInStagger 400ms var(--ease-out) forwards;
}
.stagger-item:nth-child(1) { animation-delay: 40ms; }
.stagger-item:nth-child(2) { animation-delay: 80ms; }
.stagger-item:nth-child(3) { animation-delay: 120ms; }
.stagger-item:nth-child(4) { animation-delay: 160ms; }
.stagger-item:nth-child(5) { animation-delay: 200ms; }
.stagger-item:nth-child(6) { animation-delay: 240ms; }
.stagger-item:nth-child(7) { animation-delay: 280ms; }
.stagger-item:nth-child(8) { animation-delay: 320ms; }
.stagger-item:nth-child(n+9) { animation-delay: 360ms; }

@keyframes fadeInStagger {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Reduced Motion */
@media (prefers-reduced-motion: reduce) {
  .fade-enter-active, .fade-leave-active {
    transition: opacity 250ms ease;
  }
  .fade-enter-from, .fade-leave-to {
    transform: none;
  }
  .tour-card:hover {
    transform: none;
  }
  .tour-card:active {
    transform: none;
  }
  .tour-card-img {
    transition: none;
  }
  .tour-card:hover .tour-card-img {
    transform: none;
  }
  .premium-btn:active {
    transform: none;
  }
  .stagger-item {
    animation: fadeInReduce 300ms ease forwards;
  }
  @keyframes fadeInReduce {
    to { opacity: 1; }
  }
}
</style>
