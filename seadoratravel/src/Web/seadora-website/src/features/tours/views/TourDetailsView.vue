<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useCurrencyStore } from '@/store/currency'
import { getSlug, getFullImageUrl } from '@/shared/utils/helpers'
import Footer from '@/shared/components/Footer.vue'
import GuestInfoForm from '@/features/tours/components/GuestInfoForm.vue'
import TourAvailabilityCalendar from '@/features/tours/components/TourAvailabilityCalendar.vue'

const route = useRoute()
const { locale, t } = useI18n()
const currencyStore = useCurrencyStore()

const routeSlug = computed(() => route.params.slug as string)

// Language & Currency Dropdowns state
const showLangDropdown = ref(false)
const showCurrencyDropdown = ref(false)

const languages = [
  { code: 'en', label: 'English', flag: '🇬🇧', iso: 'EN' },
  { code: 'de', label: 'Deutsch', flag: '🇩🇪', iso: 'DE' },
  { code: 'it', label: 'Italiano', flag: '🇮🇹', iso: 'IT' },
  { code: 'fr', label: 'Français', flag: '🇫🇷', iso: 'FR' },
  { code: 'ru', label: 'Русский', flag: '🇷🇺', iso: 'RU' }
]

const currencies = [
  { code: 'EUR', symbol: '€', label: 'EUR — Euro' },
  { code: 'USD', symbol: '$', label: 'USD — US Dollar' },
  { code: 'EGP', symbol: 'EGP', label: 'EGP — Egyptian Pound' }
]

const currentLangObj = computed(() => languages.find(l => l.code === locale.value) || languages[0])
const currentCurrencyObj = computed(() => currencies.find(c => c.code === currencyStore.selectedCurrency) || currencies[0])

const setLanguage = (code: string) => {
  locale.value = code
  showLangDropdown.value = false
}

const selectCurrency = (code: string) => {
  currencyStore.setCurrency(code)
  showCurrencyDropdown.value = false
}

// UI States
const activeTab = ref('overview')
const isSaved = ref(false)
const readMoreExpanded = ref(false)
const galleryModalOpen = ref(false)
const activeLightboxIndex = ref(0)
const showShareModal = ref(false)
const showToast = ref(false)
const toastMessage = ref('')
const activeFaq = ref<number | null>(0)
const timelineProgress = ref(0)
const timelineRef = ref<HTMLElement | null>(null)

// Booking Options State
const selectedDate = ref(new Date(Date.now() + 86400000).toISOString().split('T')[0])
const selectedOptionIndex = ref(0)
const adultsCount = ref(2)
const childrenCount = ref(0)
const selectedAddons = ref<number[]>([])

// Tour Data State
const tour = ref<any>(null)
const loading = ref(true)

// ==========================================
// COMPREHENSIVE MULTILINGUAL LOCALIZATION
// ==========================================
const i18nContent = computed(() => {
  const lang = locale.value || 'en'
  const dict: Record<string, any> = {
    en: {
      home: 'Home',
      allTours: 'All Tours',
      bestseller: 'Bestseller',
      reviewsCount: '4,288 reviews',
      duration: 'Duration',
      durationLabel: '5 Hours',
      egypt: 'Egypt',
      freeCancel: 'Free Cancellation (Up to 24h)',
      hotelTransfer: 'Hotel Transfer',
      included: 'Included',
      liveGuide: 'Live Guide',
      guideLangs: 'EN, DE, IT, FR, RU',
      mobileTicket: 'Mobile Ticket',
      instantVoucher: 'Instant Voucher',
      viewAllPhotos: 'View All 30 Photos',
      hookQuote: 'Trade the resort for open desert, and let one golden evening hold five unforgettable adventures.',
      totalPrice: 'Total Price',
      bookNow: 'Book Now',
      tabs: {
        overview: 'Overview',
        highlights: 'Highlights',
        itinerary: 'Itinerary',
        includes: 'Inclusions',
        info: 'Important Info',
        reviews: 'Reviews (4,288)',
        faq: 'FAQ'
      },
      descHeading: 'Experience Description',
      readMore: 'Read Full Description',
      showLess: 'Show Less',
      highlightsHeading: 'Tour Highlights',
      itineraryHeading: 'Experience Itinerary & Timeline',
      includesHeading: "What's Included & Excluded",
      includedTitle: 'Included in Tour',
      excludedTitle: 'Not Included',
      infoHeading: 'Important Information & Advice',
      whatToBringTitle: 'What to Bring',
      notSuitableTitle: 'Not Suitable For',
      notSuitableText: 'Pregnant women, travelers with severe back/neck problems, or children under 6 for quad driving.',
      reviewsHeading: 'Customer Reviews & Ratings',
      reviewsSub: 'Verified traveler reviews from recent safari departures',
      verifiedBooking: 'Verified Booking',
      faqHeading: 'Frequently Asked Questions',
      sidebar: {
        startingFrom: 'Starting from',
        saveBadge: 'SAVE 25%',
        perPerson: '/ person',
        step1: '1. Select Travel Date',
        step2: '2. Choose Safari Package',
        step3: '3. Number of Guests',
        adults: 'Adults',
        adultsAge: 'Age 12+',
        children: 'Children',
        childrenAge: 'Age 6-11 (50%)',
        step4: '4. Popular Add-ons (Optional)',
        totalAmount: 'Total Amount',
        taxesIncluded: 'Taxes & Fees Included',
        bookBtn: 'Check Availability & Book',
        instantConfirmation: 'Instant Confirmation · Reserve Now & Pay Later',
        trust1: '256-Bit SSL Encrypted & Secure Checkout',
        trust2: '24/7 Dedicated WhatsApp Concierge',
        trust3: 'Best Price Guarantee — Direct Operator'
      },
      whatToBring: [
        'Sunglasses and head scarf (Keffiyeh)',
        'Comfortable closed-toe sports shoes',
        'Camera or smartphone for sunset photos',
        'Warm jacket in winter months (Nov - Mar)',
        'Cash for optional souvenirs, drinks or tips'
      ],
      highlights: [
        'Ride automatic quad bikes across sweeping desert dunes with no prior experience needed.',
        'Hear your voice echo off towering sandstone cliffs at the famous Echo Mountains.',
        'Take a scenic sunset camel trek over golden dunes for breathtaking photographs.',
        'Feast on an open Bedouin BBQ buffet with grilled meats and freshly baked flatbread.',
        'Enjoy live Tanoura spinning dancers and a thrilling fire show under starry skies.',
        'Stress-free round-trip hotel pickup and drop-off in air-conditioned comfort.'
      ],
      itinerary: [
        { time: '15:00 - 15:30', title: 'Hotel Pickup & Transfer', desc: 'Comfortable air-conditioned coach pickup directly from your hotel in Sharm El-Sheikh.' },
        { time: '15:45 - 16:00', title: 'ATV Safety Briefing & Test Run', desc: 'Meet your desert safari instructors, wear safety gear, and complete a quick practice test.' },
        { time: '16:00 - 17:15', title: 'Adrenaline Quad Safari & Echo Mountains', desc: 'Ride into rugged Sinai canyons and stop at the Echo Mountains to shout and hear the canyon reply.' },
        { time: '17:15 - 18:00', title: 'Bedouin Tent & Sunset Camel Ride', desc: 'Sway gently atop desert camels over golden dunes as the desert sunset turns amber.' },
        { time: '18:00 - 19:30', title: 'Bedouin BBQ Buffet & Herbal Tea', desc: 'Savor grilled chicken, kofta, fresh salads, fresh-baked flatbread, and authentic Habak tea.' },
        { time: '19:30 - 20:30', title: 'Oriental Fire Show & Tanoura Dance', desc: 'Mesmerizing folkloric whirling Tanoura performance and an exhilarating fire-breathing show.' },
        { time: '20:30 - 21:00', title: 'Return Hotel Transfer', desc: 'Relax on the return ride back to your hotel with unforgettable memories.' }
      ],
      inclusions: [
        'Hotel pickup and drop-off in comfortable air-conditioned vehicle',
        'Automatic quad bike rental with safety helmet',
        'Professional desert safari guide (Multilingual)',
        'Camel ride over golden dunes (approx. 15-20 minutes)',
        'Stop at the Echo Mountains for photography and echo shout',
        'Open BBQ dinner buffet (grilled meats, rice, salads, fresh bread)',
        'Traditional Bedouin herbal tea (Habak)',
        'Live oriental entertainment (Tanoura dancer & fire show)',
        'All taxes, fuel surcharges, and service fees'
      ],
      exclusions: [
        'Desert scarf (Keffiyeh) and safety goggles (available for rent/purchase €3-€4)',
        'Souvenir photos and videos captured by the professional desert photographer',
        'Soft drinks and bottled beverages inside the Bedouin camp (nominal fee)',
        'Gratuities / tips for safari guides and drivers (optional)'
      ],
      faqs: [
        { q: 'Do I need a driver license or prior quad biking experience?', a: 'No driver license or prior experience is required! All our quad bikes are fully automatic and easy to ride. Our instructors provide a full safety briefing and practice session before starting.' },
        { q: 'What is the policy for children participating?', a: 'Children aged 16+ can drive their own single quad. Children aged 6-15 can ride as passengers on double quads with an adult. Children under 6 can participate in the Bedouin dinner and camel ride.' },
        { q: 'Can I cancel or change my booking date for free?', a: 'Yes! We offer 100% free cancellation and date changes up to 24 hours before your scheduled tour departure.' },
        { q: 'Is hotel pickup included from all resorts in the area?', a: 'Yes, round-trip pickup and drop-off is included from all hotels and resorts in the destination area.' }
      ],
      options: [
        { title: 'Single Quad ATV Safari + Camel & BBQ Show', subtitle: '1 Person on 1 Quad Bike (16+ yrs)', badge: 'BESTSELLER' },
        { title: 'Double Quad ATV Safari (2 Persons on 1 Quad)', subtitle: 'Driver + Passenger on 1 Quad Bike', badge: 'POPULAR FOR COUPLES' },
        { title: 'VIP Private Desert Safari + Stargazing Telescope', subtitle: 'Exclusive Private Guide & VIP Bedouin Seating', badge: 'LUXURY VIP' }
      ],
      addons: [
        { name: 'Bedouin Desert Scarf & Dust Goggles', priceEur: 4 },
        { name: 'VIP Front Row Dinner Seating & Fruit Basket', priceEur: 8 },
        { name: 'Professional Photographer HD Photo & Video Pack', priceEur: 15 }
      ]
    },
    de: {
      home: 'Startseite',
      allTours: 'Alle Touren',
      bestseller: 'Bestseller',
      reviewsCount: '4.288 Bewertungen',
      duration: 'Dauer',
      durationLabel: '5 Stunden',
      egypt: 'Ägypten',
      freeCancel: 'Kostenlose Stornierung (bis zu 24 Std.)',
      hotelTransfer: 'Hoteltransfer',
      included: 'Inklusive',
      liveGuide: 'Reiseleiter',
      guideLangs: 'DE, EN, IT, FR, RU',
      mobileTicket: 'Mobiles Ticket',
      instantVoucher: 'Sofortige Bestätigung',
      viewAllPhotos: 'Alle 30 Fotos ansehen',
      hookQuote: 'Tauschen Sie das Hotel gegen die Wüste und erleben Sie fünf Abenteuer an einem goldenen Abend.',
      totalPrice: 'Gesamtpreis',
      bookNow: 'Jetzt buchen',
      tabs: {
        overview: 'Übersicht',
        highlights: 'Höhepunkte',
        itinerary: 'Ablauf',
        includes: 'Inklusivleistungen',
        info: 'Wichtige Infos',
        reviews: 'Bewertungen (4.288)',
        faq: 'FAQ'
      },
      descHeading: 'Beschreibung des Erlebnisses',
      readMore: 'Vollständige Beschreibung lesen',
      showLess: 'Weniger anzeigen',
      highlightsHeading: 'Höhepunkte der Tour',
      itineraryHeading: 'Tourverlauf & Zeitplan',
      includesHeading: 'Was ist enthalten & nicht enthalten',
      includedTitle: 'In der Tour enthalten',
      excludedTitle: 'Nicht enthalten',
      infoHeading: 'Wichtige Informationen & Ratschläge',
      whatToBringTitle: 'Was Sie mitbringen sollten',
      notSuitableTitle: 'Nicht geeignet für',
      notSuitableText: 'Schwangere Frauen, Personen mit schweren Rückenproblemen oder Kinder unter 6 Jahren zum Quadfahren.',
      reviewsHeading: 'Kundenbewertungen & Erfahrungsberichte',
      reviewsSub: 'Verifizierte Reisebewertungen kürzlicher Wüstentouren',
      verifiedBooking: 'Verifizierte Buchung',
      faqHeading: 'Häufig gestellte Fragen (FAQ)',
      sidebar: {
        startingFrom: 'Ab',
        saveBadge: '25% SPAREN',
        perPerson: '/ Person',
        step1: '1. Reisedatum wählen',
        step2: '2. Safari-Paket wählen',
        step3: '3. Anzahl der Gäste',
        adults: 'Erwachsene',
        adultsAge: 'Ab 12 Jahren',
        children: 'Kinder',
        childrenAge: '6-11 Jahre (50%)',
        step4: '4. Beliebte Zusatzoptionen (Optional)',
        totalAmount: 'Gesamtbetrag',
        taxesIncluded: 'Inkl. Steuern & Gebühren',
        bookBtn: 'Verfügbarkeit prüfen & buchen',
        instantConfirmation: 'Sofortige Bestätigung · Jetzt reservieren, später zahlen',
        trust1: '256-Bit SSL-verschlüsselte sichere Zahlung',
        trust2: '24/7 WhatsApp-Kundenservice',
        trust3: 'Bestpreisgarantie — Direkter Veranstalter'
      },
      whatToBring: [
        'Sonnenbrille und Wüstentuch (Keffiyeh)',
        'Bequeme geschlossene Sportschuhe',
        'Kamera oder Smartphone für Sonnenuntergangsfotos',
        'Warme Jacke in den Wintermonaten (Nov - März)',
        'Bargeld für optionale Getränke, Souvenirs oder Trinkgelder'
      ],
      highlights: [
        'Fahren Sie automatische Quads durch atemberaubende Wüstendünen – keine Vorkenntnisse nötig.',
        'Hören Sie Ihre Stimme an den Felswänden der berühmten Echo-Berge widerhallen.',
        'Erleben Sie einen Kamelritt im warmen Abendlicht für perfekte Erinnerungsfotos.',
        'Genießen Sie ein reichhaltiges Beduinen-BBQ-Buffet mit gegrilltem Fleisch und frischem Fladenbrot.',
        'Erleben Sie eine traditionelle Tanoura-Tanzshow und eine spektakuläre Feuershow unter Sternen.',
        'Bequemer und stressfreier Hin- und Rücktransfer ab/bis Hotel im klimatisierten Fahrzeug.'
      ],
      itinerary: [
        { time: '15:00 - 15:30', title: 'Hotelabholung & Transfer', desc: 'Bequeme Abholung mit klimatisiertem Bus direkt von Ihrem Hotel in Sharm El-Sheikh.' },
        { time: '15:45 - 16:00', title: 'Sicherheitseinweisung & Probefahrt', desc: 'Begrüßung durch die Wüsten-Instruktoren, Helm anlegen und kurze Probefahrt.' },
        { time: '16:00 - 17:15', title: 'Quad-Safari & Echo-Berge', desc: 'Fahrt in die Sinai-Schluchten und Fotostopp an den Echo-Bergen mit Echo-Ruf.' },
        { time: '17:15 - 18:00', title: 'Beduinencamp & Kamelritt zum Sonnenuntergang', desc: 'Sanfter Ritt auf Kamelen über goldene Dünen bei Sonnenuntergang.' },
        { time: '18:00 - 19:30', title: 'Beduinen-BBQ-Buffet & Kräutertee', desc: 'Köstliches BBQ mit gegrilltem Hähnchen, Kofta, frischen Salaten und Habak-Tee.' },
        { time: '19:30 - 20:30', title: 'Orientalische Feuershow & Tanoura-Tanz', desc: 'Faszinierender Tanoura-Drehtanz und aufregende Feuershow unter dem Sternenhimmel.' },
        { time: '20:30 - 21:00', title: 'Rücktransfer zum Hotel', desc: 'Entspannte Rückfahrt zu Ihrem Ferienresort.' }
      ],
      inclusions: [
        'Hotelabholung und Rücktransfer im klimatisierten Fahrzeug',
        'Automatisches Quad-Bike mit Sicherheitshelm',
        'Professioneller mehrsprachiger Safari-Reiseleiter',
        'Kamelritt über Wüstendünen (ca. 15-20 Minuten)',
        'Stopp an den Echo-Bergen für Fotos und Echo-Rufe',
        'Offenes BBQ-Abendbuffet (Grillfleisch, Reis, Salate, frisches Brot)',
        'Traditioneller Beduinen-Kräutertee (Habak)',
        'Orientalische Live-Show (Tanoura-Tanz und Feuershow)',
        'Alle Steuern und Servicegebühren'
      ],
      exclusions: [
        'Wüstentuch (Keffiyeh) und Staubbrille (vor Ort leihbar/kaufbar ca. 3-4 €)',
        'Fotos & Videos vom professionellen Wüstenfotografen',
        'Softdrinks und Dosengetränke im Beduinencamp',
        'Trinkgelder für Reiseleiter und Fahrer (optional)'
      ],
      faqs: [
        { q: 'Brauche ich einen Führerschein oder Quad-Erfahrung?', a: 'Nein, kein Führerschein und keine Vorerfahrung erforderlich! Alle Quads sind vollautomatisch und sehr einfach zu steuern.' },
        { q: 'Wie ist die Regelung für Kinder?', a: 'Jugendliche ab 16 Jahren können ein eigenes Einzelquad fahren. Kinder von 6 bis 15 Jahren fahren als Beifahrer auf dem Doppelquad mit einem Erwachsenen mit.' },
        { q: 'Kann ich die Buchung kostenlos stornieren?', a: 'Ja! Sie können Ihre Buchung bis zu 24 Stunden vor Beginn 100% kostenlos stornieren oder umbuchen.' },
        { q: 'Ist die Hotelabholung von allen Hotels inklusive?', a: 'Ja, der Hin- und Rücktransfer von allen Hotels im Zielgebiet ist bereits im Preis enthalten.' }
      ],
      options: [
        { title: 'Einzel-Quad Safari + Kamel & BBQ Show', subtitle: '1 Person auf 1 Quad (ab 16 J.)', badge: 'BESTSELLER' },
        { title: 'Doppel-Quad Safari (2 Personen auf 1 Quad)', subtitle: 'Fahrer + Beifahrer auf 1 Quad', badge: 'BELIEBT BEI PAAREN' },
        { title: 'VIP Private Wüstensafari + Sternen-Teleskop', subtitle: 'Exklusiver Privatguide & VIP-Sitzplätze', badge: 'LUXUS VIP' }
      ],
      addons: [
        { name: 'Beduinentuch & Schutzbrille', priceEur: 4 },
        { name: 'VIP-Sitzplatz 1. Reihe & Obstkorb', priceEur: 8 },
        { name: 'HD Foto- & Videopaket vom Fotografen', priceEur: 15 }
      ]
    },
    it: {
      home: 'Home',
      allTours: 'Tutti i Tour',
      bestseller: 'Più Venduto',
      reviewsCount: '4.288 recensioni',
      duration: 'Durata',
      durationLabel: '5 Ore',
      egypt: 'Egitto',
      freeCancel: 'Cancellazione Gratuita (fino a 24h prima)',
      hotelTransfer: 'Trasferimento Hotel',
      included: 'Incluso',
      liveGuide: 'Guida Turistica',
      guideLangs: 'IT, EN, DE, FR, RU',
      mobileTicket: 'Biglietto Mobile',
      instantVoucher: 'Conferma Immediata',
      viewAllPhotos: 'Mostra tutte le 30 foto',
      hookQuote: "Lascia il resort per il deserto aperto e vivi cinque avventure indimenticabili in una serata d'oro.",
      totalPrice: 'Prezzo totale',
      bookNow: 'Prenota ora',
      tabs: {
        overview: 'Panoramica',
        highlights: 'Punti Forti',
        itinerary: 'Itinerario',
        includes: 'Inclusioni',
        info: 'Info Utili',
        reviews: 'Recensioni (4.288)',
        faq: 'FAQ'
      },
      descHeading: "Descrizione dell'Esperienza",
      readMore: 'Leggi descrizione completa',
      showLess: 'Mostra meno',
      highlightsHeading: 'Punti Forti del Tour',
      itineraryHeading: 'Itinerario e Programma',
      includesHeading: 'Cosa è incluso e non incluso',
      includedTitle: 'Incluso nel Tour',
      excludedTitle: 'Non Incluso',
      infoHeading: 'Informazioni Importanti & Consigli',
      whatToBringTitle: 'Cosa Portare',
      notSuitableTitle: 'Non Adatto a',
      notSuitableText: 'Donne in gravidanza, persone con gravi problemi alla schiena o bambini sotto i 6 anni per la guida del quad.',
      reviewsHeading: 'Recensioni dei Clienti',
      reviewsSub: 'Recensioni verificate da viaggiatori recenti',
      verifiedBooking: 'Prenotazione Verificata',
      faqHeading: 'Domande Frequenti (FAQ)',
      sidebar: {
        startingFrom: 'A partire da',
        saveBadge: 'RISPARMIA 25%',
        perPerson: '/ persona',
        step1: '1. Seleziona Data di Viaggio',
        step2: '2. Scegli Pacchetto Safari',
        step3: '3. Numero di Ospiti',
        adults: 'Adulti',
        adultsAge: 'Età 12+',
        children: 'Bambini',
        childrenAge: 'Età 6-11 (50%)',
        step4: '4. Opzioni Aggiuntive (Opzionale)',
        totalAmount: 'Importo Totale',
        taxesIncluded: 'Tasse e Costi Inclusi',
        bookBtn: 'Verifica Disponibilità e Prenota',
        instantConfirmation: 'Conferma Istantanea · Prenota Ora, Paga Dopo',
        trust1: 'Pagamento Sicuro con Crittografia SSL a 256 Bit',
        trust2: 'Assistenza WhatsApp Dedicata 24/7',
        trust3: 'Miglior Prezzo Garantito — Operatore Diretto'
      },
      whatToBring: [
        'Occhiali da sole e kefiah per il deserto',
        'Scarpe sportive chiuse comode',
        'Fotocamera o smartphone per le foto al tramonto',
        'Giacca calda nei mesi invernali (Nov - Mar)',
        'Contanti per bevande extra, souvenir o mance'
      ],
      highlights: [
        'Guida quad automatici tra le spettacolari dune del deserto, nessuna esperienza richiesta.',
        'Ascolta la tua voce echeggiare contro le maestose pareti delle Montagne dell’Eco.',
        'Fai una suggestiva passeggiata a dorso di cammello al tramonto.',
        'Gusta una ricca cena barbecue beduina con carne alla griglia e pane fresco.',
        'Spettacolo folcloristico dal vivo con danzatore Tanoura e mangiafuoco sotto le stelle.',
        'Comodo transfer di andata e ritorno con aria condizionata dal tuo hotel.'
      ],
      itinerary: [
        { time: '15:00 - 15:30', title: 'Pick-up in Hotel e Trasferimento', desc: 'Prelievo comodo con pullman climatizzato direttamente dal tuo resort a Sharm El-Sheikh.' },
        { time: '15:45 - 16:00', title: 'Briefing di Sicurezza e Guida di Prova', desc: 'Incontro con gli istruttori, consegna caschi e breve prova di guida sul quad.' },
        { time: '16:00 - 17:15', title: 'Safari in Quad e Montagne dell’Eco', desc: 'Guida nel canyon del Sinai con sosta fotografica e prova dell’eco.' },
        { time: '17:15 - 18:00', title: 'Accampamento Beduino e Giro in Cammello', desc: 'Passeggiata in cammello sulle dune durante la luce dorata del tramonto.' },
        { time: '18:00 - 19:30', title: 'Buffet BBQ Beduino e Tè Habak', desc: 'Cena con pollo grigliato, kofta, insalate orientali e tè tipico alle erbe.' },
        { time: '19:30 - 20:30', title: 'Spettacolo di Fuoco e Danza Tanoura', desc: 'Emozionante spettacolo con danzatrice rotante Tanoura e mangiafuoco.' },
        { time: '20:30 - 21:00', title: 'Rientro in Hotel', desc: 'Rientro rilassante al tuo resort.' }
      ],
      inclusions: [
        'Trasferimento di andata e ritorno in veicolo climatizzato',
        'Noleggio quad automatico con casco di sicurezza',
        'Guida safari professionista multilingue',
        'Giro in cammello sulle dune (15-20 min)',
        'Sosta alle Montagne dell’Eco per foto',
        'Cena a buffet BBQ aperta (carne, riso, insalate, pane fresco)',
        'Tè tradizionale beduino alle erbe (Habak)',
        'Spettacolo dal vivo (Danza Tanoura e mangiafuoco)',
        'Tutte le tasse e i costi di servizio'
      ],
      exclusions: [
        'Kefiah e occhiali antipolvere (noleggiabili/acquistabili a 3-4 €)',
        'Foto e video del fotografo professionista',
        'Bevande analcoliche in lattina al campo beduino',
        'Mance per guide e autisti (opzionali)'
      ],
      faqs: [
        { q: 'Serve la patente o esperienza precedente sui quad?', a: 'Nessuna patente o esperienza necessaria! Tutti i nostri quad sono completamente automatici e facili da guidare.' },
        { q: 'Qual è la regola per i bambini?', a: 'I ragazzi dai 16 anni in su possono guidare il proprio quad singolo. I bambini da 6 a 15 anni possono salire come passeggeri sul quad doppio con un adulto.' },
        { q: 'Posso cancellare gratuitamente?', a: 'Sì! Offriamo la cancellazione gratuita al 100% fino a 24 ore prima dell’orario di partenza.' },
        { q: 'Il prelievo in hotel è incluso ovunque?', a: 'Sì, il trasferimento di andata e ritorno da tutti gli hotel e resort della zona è già incluso nel prezzo.' }
      ],
      options: [
        { title: 'Safari Quad Singolo + Cammello & Show BBQ', subtitle: '1 Persona su 1 Quad (Età 16+)', badge: 'PIÙ VENDUTO' },
        { title: 'Safari Quad Doppio (2 Persone su 1 Quad)', subtitle: 'Pilota + Passeggero su 1 Quad', badge: 'IDEALE PER COPPIE' },
        { title: 'Safari VIP Privato + Telescopio Astronomico', subtitle: 'Guida Privata Esclusiva & Posti VIP', badge: 'LUSSO VIP' }
      ],
      addons: [
        { name: 'Kefiah da Deserto & Occhialini Antipolvere', priceEur: 4 },
        { name: 'Posto VIP in Prima Fila & Cesto di Frutta', priceEur: 8 },
        { name: 'Pacchetto Foto & Video HD con Fotografo', priceEur: 15 }
      ]
    },
    fr: {
      home: 'Accueil',
      allTours: 'Tous les Tours',
      bestseller: 'Meilleure Vente',
      reviewsCount: '4 288 avis',
      duration: 'Durée',
      durationLabel: '5 Heures',
      egypt: 'Égypte',
      freeCancel: 'Annulation Gratuite (jusqu’à 24h avant)',
      hotelTransfer: 'Transfert Hôtel',
      included: 'Inclus',
      liveGuide: 'Guide Touristique',
      guideLangs: 'FR, EN, DE, IT, RU',
      mobileTicket: 'Billet Mobile',
      instantVoucher: 'Confirmation Immédiate',
      viewAllPhotos: 'Voir les 30 photos',
      hookQuote: 'Quittez votre hôtel pour le désert ouvert et vivez cinq aventures magiques en une soirée dorée.',
      totalPrice: 'Prix total',
      bookNow: 'Réserver',
      tabs: {
        overview: 'Aperçu',
        highlights: 'Points Forts',
        itinerary: 'Itinéraire',
        includes: 'Inclusions',
        info: 'Infos Pratiques',
        reviews: 'Avis (4 288)',
        faq: 'FAQ'
      },
      descHeading: "Description de l'Expérience",
      readMore: 'Lire toute la description',
      showLess: 'Réduire',
      highlightsHeading: 'Points Forts du Tour',
      itineraryHeading: 'Itinéraire et Déroulement',
      includesHeading: 'Ce qui est inclus et non inclus',
      includedTitle: 'Inclus dans le tour',
      excludedTitle: 'Non inclus',
      infoHeading: 'Informations Importantes & Conseils',
      whatToBringTitle: 'À apporter',
      notSuitableTitle: 'Non adapté pour',
      notSuitableText: 'Femmes enceintes, personnes souffrant du dos ou enfants de moins de 6 ans pour la conduite du quad.',
      reviewsHeading: 'Avis Clients et Évaluations',
      reviewsSub: 'Avis vérifiés de voyageurs récents',
      verifiedBooking: 'Réservation Vérifiée',
      faqHeading: 'Foire Aux Questions (FAQ)',
      sidebar: {
        startingFrom: 'À partir de',
        saveBadge: 'ÉCONOMISEZ 25%',
        perPerson: '/ personne',
        step1: '1. Choisissez la Date',
        step2: '2. Choisissez la Formule',
        step3: '3. Nombre de Participants',
        adults: 'Adultes',
        adultsAge: '12 ans et +',
        children: 'Enfants',
        childrenAge: '6-11 ans (50%)',
        step4: '4. Options Supplémentaires',
        totalAmount: 'Montant Total',
        taxesIncluded: 'Taxes et Frais Inclus',
        bookBtn: 'Vérifier la Disponibilité & Réserver',
        instantConfirmation: 'Confirmation Immédiate · Réservez maintenant, payez plus tard',
        trust1: 'Paiement Sécurisé SSL 256-Bit',
        trust2: 'Service Client WhatsApp 24/7',
        trust3: 'Meilleur Prix Garanti — Opérateur Direct'
      },
      whatToBring: [
        'Lunettes de soleil et foulard bédouin (Keffieh)',
        'Chaussures de sport fermées et confortables',
        'Appareil photo ou smartphone pour les photos au coucher du soleil',
        'Veste chaude en hiver (novembre à mars)',
        'Espèces pour boissons supplémentaires ou pourboires'
      ],
      highlights: [
        'Conduisez des quads automatiques sur les dunes dorées, aucune expérience requise.',
        'Écoutez votre voix résonner contre les falaises des célèbres Montagnes de l’Écho.',
        'Balade à dos de chameau dans la lumière dorée du coucher de soleil.',
        'Savourez un buffet barbecue bédouin avec grillades et pain traditionnel cuit sur place.',
        'Spectacle oriental avec danseur Tanoura et cracheur de feu sous le ciel étoilé.',
        'Transfert aller-retour pratique et climatisé depuis votre hôtel.'
      ],
      itinerary: [
        { time: '15:00 - 15:30', title: 'Prise en charge à l’hôtel', desc: 'Transfert en bus climatisé confortable directement depuis votre hôtel à Charm el-Cheikh.' },
        { time: '15:45 - 16:00', title: 'Consignes de sécurité & Essai quad', desc: 'Accueil par les moniteurs, équipement des casques et court essai sur piste.' },
        { time: '16:00 - 17:15', title: 'Safari en Quad & Montagnes de l’Écho', desc: 'Balade dans les canyons du Sinaï et arrêt photo aux Montagnes de l’Écho.' },
        { time: '17:15 - 18:00', title: 'Camp Bédouin & Balade à Chameau', desc: 'Promenade à dos de chameau sur les dunes au coucher du soleil.' },
        { time: '18:00 - 19:30', title: 'Buffet Barbecue Bédouin & Thé Habak', desc: 'Dîner barbecue avec poulet grillé, kofta, salades fraîches et thé bédouin.' },
        { time: '19:30 - 20:30', title: 'Spectacle de Feu & Danse Tanoura', desc: 'Spectacle traditionnel avec danseur tournoyant Tanoura et cracheurs de feu.' },
        { time: '20:30 - 21:00', title: 'Retour à l’hôtel', desc: 'Trajet retour reposant jusqu’à votre hôtel.' }
      ],
      inclusions: [
        'Transfert aller-retour en véhicule climatisé',
        'Location du quad automatique avec casque',
        'Guide professionnel multilingue',
        'Balade à dos de chameau (15-20 min)',
        'Arrêt photo aux Montagnes de l’Écho',
        'Dîner buffet barbecue (grillades, riz, salades, pain frais)',
        'Thé bédouin traditionnel (Habak)',
        'Spectacle oriental en direct (Tanoura et feu)',
        'Toutes les taxes et frais de service'
      ],
      exclusions: [
        'Foulard bédouin et lunettes anti-poussière (disponibles à l’achat/location 3-4 €)',
        'Photos et vidéos du photographe professionnel',
        'Boissons gazeuses en canette au camp bédouin',
        'Pourboires pour le guide et chauffeur (facultatifs)'
      ],
      faqs: [
        { q: 'Faut-il un permis de conduire ou de l’expérience en quad ?', a: 'Aucun permis ni expérience préalable requis ! Tous nos quads sont entièrement automatiques et très faciles à conduire.' },
        { q: 'Quelle est la politique pour les enfants ?', a: 'Les jeunes dès 16 ans peuvent piloter leur propre quad solo. Les enfants de 6 à 15 ans voyagent en passagers sur un quad double avec un adulte.' },
        { q: 'Puis-je annuler gratuitement ?', a: 'Oui ! Vous pouvez annuler ou modifier votre réservation sans frais jusqu’à 24h avant le départ.' },
        { q: 'La prise en charge à l’hôtel est-elle incluse partout ?', a: 'Oui, le transfert aller-retour depuis tous les hôtels de la région est inclus dans le tarif.' }
      ],
      options: [
        { title: 'Safari Quad Solo + Chameau & Show BBQ', subtitle: '1 Personne sur 1 Quad (16 ans +)', badge: 'MEILLEURE VENTE' },
        { title: 'Safari Quad Double (2 Personnes sur 1 Quad)', subtitle: 'Conducteur + Passager sur 1 Quad', badge: 'IDÉAL COUPLES' },
        { title: 'Safari Privé VIP + Télescope Astronomique', subtitle: 'Guide Privé Exclusif & Places VIP', badge: 'LUXE VIP' }
      ],
      addons: [
        { name: 'Foulard Keffieh & Lunettes Anti-poussière', priceEur: 4 },
        { name: 'Place VIP 1er Rang & Corbeille de Fruits', priceEur: 8 },
        { name: 'Pack Photos & Vidéos HD Professionnel', priceEur: 15 }
      ]
    },
    ru: {
      home: 'Главная',
      allTours: 'Все туры',
      bestseller: 'Хит продаж',
      reviewsCount: '4 288 отзывов',
      duration: 'Длительность',
      durationLabel: '5 часов',
      egypt: 'Египет',
      freeCancel: 'Бесплатная отмена (за 24 ч.)',
      hotelTransfer: 'Трансфер из отеля',
      included: 'Включено',
      liveGuide: 'Гид',
      guideLangs: 'RU, EN, DE, IT, FR',
      mobileTicket: 'Мобильный билет',
      instantVoucher: 'Мгновенное подтверждение',
      viewAllPhotos: 'Все 30 фото',
      hookQuote: 'Смените отель на бескрайнюю пустыню и проживите 5 ярких приключений за один золотой вечер.',
      totalPrice: 'Итого',
      bookNow: 'Забронировать',
      tabs: {
        overview: 'Обзор',
        highlights: 'Главное',
        itinerary: 'Программа',
        includes: 'Включено',
        info: 'Важно знать',
        reviews: 'Отзывы (4 288)',
        faq: 'FAQ'
      },
      descHeading: 'Описание экскурсии',
      readMore: 'Читать полное описание',
      showLess: 'Свернуть',
      highlightsHeading: 'Главные впечатления',
      itineraryHeading: 'Программа тура и тайминг',
      includesHeading: 'Что включено и не включено',
      includedTitle: 'В стоимость входит',
      excludedTitle: 'Дополнительно оплачивается',
      infoHeading: 'Полезная информация и советы',
      whatToBringTitle: 'Что взять с собой',
      notSuitableTitle: 'Не рекомендуется',
      notSuitableText: 'Беременным женщинам, людям с травмами спины и детям до 6 лет для управления квадроциклом.',
      reviewsHeading: 'Отзывы путешественников',
      reviewsSub: 'Проверенные отзывы туристов о сафари',
      verifiedBooking: 'Подтвержденная поездка',
      faqHeading: 'Часто задаваемые вопросы',
      sidebar: {
        startingFrom: 'От',
        saveBadge: 'СКИДКА 25%',
        perPerson: '/ человек',
        step1: '1. Выберите дату',
        step2: '2. Выберите вариант сафари',
        step3: '3. Количество гостей',
        adults: 'Взрослые',
        adultsAge: 'От 12 лет',
        children: 'Дети',
        childrenAge: '6-11 лет (50%)',
        step4: '4. Дополнительные опции',
        totalAmount: 'Итого к оплате',
        taxesIncluded: 'Все налоги и сборы включены',
        bookBtn: 'Проверить наличие и забронировать',
        instantConfirmation: 'Мгновенно · Бронируйте сейчас, платите позже',
        trust1: 'Безопасная оплата с 256-битным SSL-шифрованием',
        trust2: 'Поддержка в WhatsApp 24/7',
        trust3: 'Гарантия лучшей цены от прямого организатора'
      },
      whatToBring: [
        'Солнцезащитные очки и арафатку (платок на голову)',
        'Удобную закрытую спортивную обувь',
        'Фотоаппарат или смартфон для красивых снимков на закате',
        'Теплую кофту или куртку в зимний сезон (ноябрь-март)',
        'Наличные деньги на сувениры, напитки или чаевые'
      ],
      highlights: [
        'Катание на автоматических квадроциклах по песчаным дюнам — права и опыт не нужны.',
        'Услышьте свой голос среди отвесных скал в знаменитом каньоне Эхо.',
        'Прогулка на верблюдах на закате с потрясающими фото на память.',
        'Аутентичный бедуинский ужин-барбекю со свежими лепешками и блюдами на гриле.',
        'Яркое восточное шоу с национальным танцем Танура и фаер-шоу под звездами.',
        'Комфортабельный трансфер из отеля и обратно на авто с кондиционером.'
      ],
      itinerary: [
        { time: '15:00 - 15:30', title: 'Трансфер из отеля', desc: 'Сбор туристов на комфортабельном кондиционированном микроавтобусе из отелей Шарм-эль-Шейха.' },
        { time: '15:45 - 16:00', title: 'Инструктаж и пробный заезд', desc: 'Знакомство с инструкторами, выдача шлемов и тест-драйв на квадроцикле.' },
        { time: '16:00 - 17:15', title: 'Сафари на квадроциклах и Скалы Эхо', desc: 'Драйвовый заезд по ущельям Синая и остановка у скал Эхо для фото.' },
        { time: '17:15 - 18:00', title: 'Деревня бедуинов и верблюды на закате', desc: 'Катание на верблюдах по дюнам в лучах заходящего солнца.' },
        { time: '18:00 - 19:30', title: 'Ужин-барбекю и бедуинский чай', desc: 'Шведский стол: курица на углях, люля-кебаб, салаты, свежие лепёшки и чай хабак.' },
        { time: '19:30 - 20:30', title: 'Шоу огня и танец Танура', desc: 'Зрелищный танец юбок Танура и захватывающее шоу факиров с огнем.' },
        { time: '20:30 - 21:00', title: 'Возвращение в отель', desc: 'Трансфер обратно в отель с морем впечатлений.' }
      ],
      inclusions: [
        'Трансфер из отеля и обратно на кондиционированном транспорте',
        'Аренда квадроцикла с защитным шлемом',
        'Профессиональный русскоязычный гид-инструктор',
        'Катание на верблюдах (15-20 минут)',
        'Остановка в ущелье Эхо для фотосессии',
        'Ужин-барбекю (мясо на гриле, рис, салаты, свежий хлеб)',
        'Традиционный бедуинский чай (хабак)',
        'Восточная шоу-программа (Танура и огненное шоу)',
        'Все налоги и сервисные сборы'
      ],
      exclusions: [
        'Арафатка и защитные очки от пыли (можно арендовать/купить на месте за 3-4 $)',
        'Фото и видео от профессионального фотографа',
        'Газированные напитки в банках в бедуинском лагере',
        'Чаевые гидам и водителям (по желанию)'
      ],
      faqs: [
        { q: 'Нужны ли водительские права или опыт вождения?', a: 'Нет, водительские права и опыт не требуются! Все квадроциклы автоматические и очень простые в управлении.' },
        { q: 'С какого возраста можно детям?', a: 'Подростки с 16 лет могут управлять отдельным квадроциклом. Дети 6-15 лет едут пассажирами на двухместном квадроцикле со взрослым.' },
        { q: 'Можно ли отменить бронирование бесплатно?', a: 'Да! Вы можете бесплатно отменить или перенести тур за 24 часа до выезда.' },
        { q: 'Трансфер входит в стоимость из всех отелей?', a: 'Да, трансфер в обе стороны включен в стоимость из любого отеля курорта.' }
      ],
      options: [
        { title: 'Одиночный квадроцикл + Верблюды и Шоу с ужином', subtitle: '1 человек на 1 квадроцикле (16+ лет)', badge: 'ХИТ ПРОДАЖ' },
        { title: 'Двухместный квадроцикл (2 человека на 1 квадроцикле)', subtitle: 'Водитель + Пассажир', badge: 'ПОПУЛЯРНО ДЛЯ ПАР' },
        { title: 'VIP Приватное сафари + Телескоп для звезд', subtitle: 'Индивидуальный гид и VIP места', badge: 'VIP ЛЮКС' }
      ],
      addons: [
        { name: 'Арафатка и пылезащитные очки', priceEur: 4 },
        { name: 'VIP места в 1-м ряду и фруктовая тарелка', priceEur: 8 },
        { name: 'Профессиональная фото и видеосъемка HD', priceEur: 15 }
      ]
    }
  }

  const base = dict[lang] || dict['en']
  return {
    ...base,
    home: t('tourDetails.home'),
    allTours: t('tourDetails.allTours'),
    bestseller: t('tourDetails.bestseller'),
    reviewsCount: t('tourDetails.reviewsCount'),
    durationLabel: t('tourDetails.durationLabel'),
    freeCancel: t('tourDetails.freeCancel'),
    hotelTransfer: t('tourDetails.hotelTransfer'),
    included: t('tourDetails.included'),
    liveGuide: t('tourDetails.liveGuide'),
    guideLangs: t('tourDetails.guideLangs'),
    mobileTicket: t('tourDetails.mobileTicket'),
    instantVoucher: t('tourDetails.instantVoucher'),
    viewAllPhotos: t('tourDetails.viewAllPhotos'),
    hookQuote: t('tourDetails.hookQuote'),
    tabs: {
      overview: t('tourDetails.navigation.overview'),
      highlights: t('tourDetails.navigation.highlights'),
      itinerary: t('tourDetails.navigation.itinerary'),
      includes: t('tourDetails.navigation.includes'),
      info: t('tourDetails.navigation.info'),
      reviews: t('tourDetails.navigation.reviews'),
      faq: t('tourDetails.navigation.faq')
    },
    descHeading: t('tourDetails.descHeading'),
    readMore: t('tourDetails.readMore'),
    showLess: t('tourDetails.showLess'),
    highlightsHeading: t('tourDetails.highlightsHeading'),
    itineraryHeading: t('tourDetails.itineraryHeading'),
    includesHeading: t('tourDetails.includesHeading'),
    includedTitle: t('tourDetails.includedTitle'),
    excludedTitle: t('tourDetails.excludedTitle'),
    infoHeading: t('tourDetails.infoHeading'),
    whatToBringTitle: t('tourDetails.whatToBringTitle'),
    notSuitableTitle: t('tourDetails.notSuitableTitle'),
    notSuitableText: t('tourDetails.notSuitableText'),
    reviewsHeading: t('tourDetails.reviewsHeading'),
    reviewsSub: t('tourDetails.reviewsSub'),
    verifiedBooking: t('tourDetails.verifiedBooking'),
    faqHeading: t('tourDetails.faqHeading'),
    sidebar: {
      startingFrom: t('tourDetails.sidebar.startingFrom'),
      saveBadge: t('tourDetails.sidebar.saveBadge'),
      perPerson: t('tourDetails.sidebar.perPerson'),
      step1: t('tourDetails.sidebar.step1'),
      step2: t('tourDetails.sidebar.step2'),
      step3: t('tourDetails.sidebar.step3'),
      adults: t('tourDetails.sidebar.adults'),
      adultsAge: t('tourDetails.sidebar.adultsAge'),
      children: t('tourDetails.sidebar.children'),
      childrenAge: t('tourDetails.sidebar.childrenAge'),
      step4: t('tourDetails.sidebar.step4'),
      totalAmount: t('tourDetails.sidebar.totalAmount'),
      taxesIncluded: t('tourDetails.sidebar.taxesIncluded'),
      bookBtn: t('tourDetails.sidebar.bookBtn'),
      instantConfirmation: t('tourDetails.sidebar.instantConfirmation'),
      trust1: t('tourDetails.sidebar.trust1'),
      trust2: t('tourDetails.sidebar.trust2'),
      trust3: t('tourDetails.sidebar.trust3')
    },
    reviewForm: {
      title: t('tourDetails.reviewForm.title'),
      yourRating: t('tourDetails.reviewForm.yourRating'),
      yourName: t('tourDetails.reviewForm.yourName'),
      reviewTitle: t('tourDetails.reviewForm.reviewTitle'),
      shareExperience: t('tourDetails.reviewForm.shareExperience'),
      submitReview: t('tourDetails.reviewForm.submitReview')
    }
  }
})

const tourTitle = computed(() => {
  if (tour.value?.names?.[locale.value]) return tour.value.names[locale.value]
  if (tour.value?.names?.['en']) return tour.value.names['en']
  const titles: Record<string, string> = {
    en: 'Quad Bike Sharm El Sheikh: ATV, Camel, Echo Mountains & BBQ Dinner Show',
    de: 'Quad Safari Sharm El Sheikh: ATV, Kamelreiten, Echo-Berge & Beduinen-BBQ Show',
    it: 'Quad Safari Sharm El Sheikh: ATV, Cammelli, Montagne dell’Eco e Cena BBQ con Spettacolo',
    fr: 'Safari Quad Charm el-Cheikh : Quad, Chameau, Montagnes de l’Écho & Dîner Spectacle Bédouin',
    ru: 'Сафари на квадроциклах в Шарм-эль-Шейхе: катание, верблюды, скалы Эхо и ужин с шоу'
  }
  return titles[locale.value] || titles['en']
})

const tourDescription = computed(() => {
  if (tour.value?.descriptions?.[locale.value]) return tour.value.descriptions[locale.value]
  if (tour.value?.descriptions?.['en']) return tour.value.descriptions['en']
  const descs: Record<string, string> = {
    en: 'Five desert experiences in one golden evening — quad bike, camel ride, the Echo Mountains, a Bedouin BBQ feast and a live fire show under the Sinai stars. Free hotel pickup, and one of Sharm El Sheikh’s highest-rated safaris.',
    de: 'Fünf Wüstenerlebnisse an einem goldenen Abend – Quad-Bike, Kamelreiten, Echo-Berge, Beduinen-BBQ-Festmahl und Live-Feuershow unter den Sternen des Sinai. Kostenlose Hotelabholung und eine der beliebtesten Safaris in Sharm El Sheikh.',
    it: 'Cinque esperienze nel deserto in una magica serata: quad, giro in cammello, Montagne dell’Eco, banchetto barbecue beduino e spettacolo di fuoco sotto le stelle del Sinai. Pick-up gratuito e safari tra i più votati.',
    fr: 'Cinq expériences dans le désert en une soirée dorée : quad, balade à dos de chameau, montagnes de l’écho, festin barbecue bédouin et spectacle de feu sous les étoiles du Sinaï. Prise en charge gratuite à l’hôtel.',
    ru: 'Пять ярких приключений за один золотой вечер: сафари на квадроциклах, катание на верблюдах, каньон Эхо, бедуинский ужин-барбекю и огненное шоу под звездами Синая. Бесплатный трансфер из отеля.'
  }
  return descs[locale.value] || descs['en']
})



const defaultAddons = computed(() => {
  const baseAddons = [
    { id: 1, priceEur: 4 },
    { id: 2, priceEur: 8 },
    { id: 3, priceEur: 15 }
  ]
  const localizedAddons = i18nContent.value.addons || []
  return baseAddons.map((addon, idx) => ({
    ...addon,
    name: localizedAddons[idx]?.name || `Add-on ${idx + 1}`,
    names: { en: localizedAddons[idx]?.name || `Add-on ${idx + 1}` }
  }))
})

const availableAddons = computed(() => {
  if (tour.value?.addons && Array.isArray(tour.value.addons)) {
    return tour.value.addons;
  }
  if (tour.value && typeof tour.value.id === 'string' && tour.value.id.length > 10) {
    return [];
  }
  return defaultAddons.value;
})

const getLocalized = (dict: Record<string, string>, fallback: string) => {
  if (!dict) return fallback
  return dict[locale.value] || dict['en'] || fallback
}

const isBookingModalOpen = ref(false)
const bookingSubmitting = ref(false)
const guestInfoFormRef = ref<any>(null)

const bookingForm = ref({
  fullName: '',
  email: '',
  whatsapp: '',
  hotelName: '',
  roomNumber: '',
  pickupTime: '15:00 - 15:30 (Sunset Safari - Recommended)',
  specialRequests: ''
})

const formErrors = ref({
  fullName: '',
  email: '',
  whatsapp: '',
  hotelName: ''
})

const tourHighlights = computed(() => {
  if (tour.value?.highlights?.[locale.value]) return tour.value.highlights[locale.value]
  if (tour.value?.highlights?.['en']) return tour.value.highlights['en']
  return i18nContent.value.highlights
})
const tourItinerary = computed(() => {
  if (tour.value?.itinerary?.[locale.value]) return tour.value.itinerary[locale.value]
  if (tour.value?.itinerary?.['en']) return tour.value.itinerary['en']
  if (tour.value?.itinerary) return tour.value.itinerary
  return i18nContent.value.itinerary
})
const tourInclusions = computed(() => {
  if (tour.value?.inclusions?.[locale.value]) return tour.value.inclusions[locale.value]
  if (tour.value?.inclusions?.['en']) return tour.value.inclusions['en']
  return i18nContent.value.inclusions
})
const tourExclusions = computed(() => {
  if (tour.value?.exclusions?.[locale.value]) return tour.value.exclusions[locale.value]
  if (tour.value?.exclusions?.['en']) return tour.value.exclusions['en']
  return i18nContent.value.exclusions
})
const tourFaqs = computed(() => {
  if (tour.value?.faqs?.[locale.value]) return tour.value.faqs[locale.value]
  if (tour.value?.faqs?.['en']) return tour.value.faqs['en']
  if (tour.value?.faqs) return tour.value.faqs
  return i18nContent.value.faqs
})
const tourWhatToBring = computed(() => {
  if (tour.value?.whatToBring?.[locale.value]) return tour.value.whatToBring[locale.value]
  if (tour.value?.whatToBring?.['en']) return tour.value.whatToBring['en']
  return i18nContent.value.whatToBring
})
const tourNotSuitable = computed(() => {
  if (tour.value?.notSuitableText?.[locale.value]) return tour.value.notSuitableText[locale.value]
  if (tour.value?.notSuitableText?.['en']) return tour.value.notSuitableText['en']
  return i18nContent.value.notSuitableText
})

const tourNotes = computed(() => {
  if (tour.value?.notes?.[locale.value]) return tour.value.notes[locale.value]
  if (tour.value?.notes?.['en']) return tour.value.notes['en']
  return tour.value?.notes || ''
})

// Dynamic Pickup Timing based on Admin Settings
const pickupTimeType = computed(() => tour.value?.pickupTimeType || 'FixedSlots')
const availablePickupTimes = computed(() => {
  if (tour.value?.availablePickupTimes && Array.isArray(tour.value.availablePickupTimes) && tour.value.availablePickupTimes.length > 0) {
    return tour.value.availablePickupTimes
  }
  return [
    '15:00 - 15:30 (Sunset Safari - Recommended)',
    '10:00 - 10:30 (Morning Safari)',
    '17:00 - 17:30 (Dinner & Show Evening)'
  ]
})

// Current Selected Option / Tour Packages
const dynamicOptions = computed(() => {
  if (tour.value?.packages && Array.isArray(tour.value.packages) && tour.value.packages.length > 0) {
    return tour.value.packages.map((pkg: any) => {
      const title = pkg.titles ? (pkg.titles[locale.value] || pkg.titles['en'] || Object.values(pkg.titles)[0] || 'Package')
        : (typeof pkg.name === 'object' ? (pkg.name[locale.value] || pkg.name['en']) : (pkg.name || pkg.title || 'Package'))
      
      const subtitle = pkg.descriptions ? (pkg.descriptions[locale.value] || pkg.descriptions['en'] || Object.values(pkg.descriptions)[0] || '')
        : (typeof pkg.description === 'object' ? (pkg.description[locale.value] || pkg.description['en']) : (pkg.description || pkg.subtitle || ''))
      
      let rawFeatures = pkg.features
      if (typeof rawFeatures === 'object' && !Array.isArray(rawFeatures)) {
        rawFeatures = rawFeatures[locale.value] || rawFeatures['en'] || Object.values(rawFeatures)[0] || []
      }
      const features = Array.isArray(rawFeatures) 
        ? rawFeatures 
        : (typeof rawFeatures === 'string' ? rawFeatures.split(',').map((s: string) => s.trim()).filter(Boolean) : [])

      return {
        id: pkg.id,
        title,
        subtitle,
        features: features.map((f: any) => typeof f === 'object' ? (f[locale.value] || f['en'] || f) : String(f)),
        basePriceEur: Number(pkg.price) || Number(tour.value?.price) || 25,
        wasPriceEur: Number(pkg.originalPrice) || Math.round((Number(pkg.price) || Number(tour.value?.price) || 25) * 1.25),
        departureTime: '15:30',
        badge: pkg.badge || (pkg.badges ? (pkg.badges[locale.value] || pkg.badges['en']) : '')
      }
    })
  }
  return []
})

const currentOption = computed(() => {
  if (dynamicOptions.value.length > 0) {
    return dynamicOptions.value[selectedOptionIndex.value] || dynamicOptions.value[0]
  }
  const basePrice = Number(tour.value?.price) || 25
  const originalPrice = (tour.value?.originalPrice && Number(tour.value.originalPrice) > basePrice)
    ? Number(tour.value.originalPrice)
    : null
  return {
    basePriceEur: basePrice,
    wasPriceEur: originalPrice,
    departureTime: tour.value?.startTime ? tour.value.startTime.replace('Starts ', '') : '15:30'
  }
})

const tourRating = computed(() => tour.value?.rating || 4.9)
const tourReviewCount = computed(() => tour.value?.reviewCount || 1250)
const tourDuration = computed(() => {
  if (tour.value?.duration) {
    if (tour.value.duration === 'fullDay') return 'Full Day (8h)'
    if (tour.value.duration === 'halfDay') return 'Half Day (4-5h)'
    if (tour.value.duration === 'twoDays') return '2 Days / 1 Night'
    return tour.value.duration
  }
  return i18nContent.value.durationLabel
})

// Dynamic Discount Percentage & Localized Save Badge
const discountPercentage = computed(() => {
  if (tour.value?.discountPercentage && Number(tour.value.discountPercentage) > 0) {
    return Math.round(Number(tour.value.discountPercentage))
  }
  if (currentOption.value.wasPriceEur && currentOption.value.wasPriceEur > currentOption.value.basePriceEur) {
    const diff = currentOption.value.wasPriceEur - currentOption.value.basePriceEur
    return Math.round((diff / currentOption.value.wasPriceEur) * 100)
  }
  return 0
})

const saveBadgeText = computed(() => {
  if (!discountPercentage.value || discountPercentage.value <= 0) return ''
  const dict: Record<string, string> = {
    en: `SAVE ${discountPercentage.value}%`,
    de: `${discountPercentage.value}% SPAREN`,
    it: `RISPARMIA ${discountPercentage.value}%`,
    fr: `ÉCONOMISEZ ${discountPercentage.value}%`,
    ru: `СКИДКА ${discountPercentage.value}%`
  }
  return dict[locale.value] || `SAVE ${discountPercentage.value}%`
})

// Formatted Price Calculations with currencyStore
const basePriceFormatted = computed(() => currencyStore.formatPrice(currentOption.value.basePriceEur))
const wasPriceFormatted = computed(() => currentOption.value.wasPriceEur ? currencyStore.formatPrice(currentOption.value.wasPriceEur) : '')
const rawTotalPriceEur = computed(() => {
  let totalEur = currentOption.value.basePriceEur * Math.max(1, adultsCount.value)
  if (childrenCount.value > 0) {
    totalEur += (currentOption.value.basePriceEur * 0.5) * childrenCount.value
  }
  selectedAddons.value.forEach(id => {
    const addon = availableAddons.value.find((a: any) => a.id === id)
    if (addon) {
      const price = addon.priceEur || addon.price || 0
      const isPerPerson = addon.pricingType === 'PerPerson' || addon.isPerPerson === true
      if (isPerPerson) {
        totalEur += price * (Math.max(1, adultsCount.value) + childrenCount.value)
      } else {
        totalEur += price
      }
    }
  })
  return totalEur
})

const totalPriceFormatted = computed(() => currencyStore.formatPrice(rawTotalPriceEur.value))

const shareUrl = computed(() => typeof window !== 'undefined' ? window.location.href : '')

// Gallery Images
const defaultGalleryImages = [
  {
    url: 'https://images.unsplash.com/photo-1542362567-b07eac790947?auto=format&fit=crop&w=1400&q=80',
    title: 'Desert Quad ATV Safari',
    caption: 'Quad biking across the golden Sinai desert sands at sunset'
  },
  {
    url: 'https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9?auto=format&fit=crop&w=1000&q=80',
    title: 'Bedouin Camel Caravan',
    caption: 'Sunset camel trek across dramatic desert sand dunes'
  },
  {
    url: 'https://images.unsplash.com/photo-1534447677768-be436bb09401?auto=format&fit=crop&w=1000&q=80',
    title: 'Bedouin Camp Feast',
    caption: 'Authentic Bedouin camp with open-flame BBQ dinner'
  },
  {
    url: 'https://images.unsplash.com/photo-1473580044384-7ba9967e16a0?auto=format&fit=crop&w=1000&q=80',
    title: 'Echo Mountains Canyon',
    caption: 'Spectacular sandstone formations and mountain echo shouts'
  },
  {
    url: 'https://images.unsplash.com/photo-1570481662006-a3a1374699e8?auto=format&fit=crop&w=1000&q=80',
    title: 'Oriental Fire & Tanoura Show',
    caption: 'Live performance under the starlit Egyptian desert sky'
  }
]

const galleryImages = computed(() => {
  const urls: string[] = (tour.value?.mediaUrls && Array.isArray(tour.value.mediaUrls) && tour.value.mediaUrls.length > 0)
    ? tour.value.mediaUrls
    : (tour.value?.images && Array.isArray(tour.value.images) && tour.value.images.length > 0)
      ? tour.value.images
      : []

  if (urls.length > 0) {
    return urls.map((url: string, index: number) => ({
      url: getFullImageUrl(url),
      title: `${tourTitle.value} - Photo ${index + 1}`,
      caption: tourDescription.value
    }))
  }

  const cover = tour.value?.imageUrl || tour.value?.mainImage
  if (cover) {
    return [
      {
        url: getFullImageUrl(cover),
        title: tourTitle.value,
        caption: tourDescription.value
      }
    ]
  }

  return defaultGalleryImages
})

// Fetch Tour API
const fetchTourData = async () => {
  loading.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const currentSlug = String(routeSlug.value || '').trim()

    // 1. Direct ID fetch if GUID
    const isGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(currentSlug)
    if (isGuid) {
      try {
        const directRes = await fetch(`${API_URL}/api/content/api/tours/${currentSlug}`)
        if (directRes.ok) {
          tour.value = await directRes.json()
          return
        }
      } catch (err) {
        console.warn('Direct ID fetch error', err)
      }
    }

    // 2. Fetch all tours and match
    const res = await fetch(`${API_URL}/api/content/api/tours`)
    if (res.ok) {
      const tours = await res.json()
      if (Array.isArray(tours) && tours.length > 0) {
        // Try ID match
        let found = tours.find((t: any) => t.id === currentSlug)

        // Try exact normalized slug match across all locales
        if (!found && currentSlug) {
          const targetSlug = getSlug(currentSlug)
          found = tours.find((t: any) => {
            const names = t.names || {}
            return Object.values(names).some((n: any) => getSlug(String(n)) === targetSlug) ||
                   getSlug(t.name || '') === targetSlug
          })
        }

        // Try partial keyword match
        if (!found && currentSlug) {
          const targetSlug = getSlug(currentSlug)
          found = tours.find((t: any) => {
            const enSlug = getSlug(t.names?.['en'] || '')
            return enSlug.includes(targetSlug) || targetSlug.includes(enSlug)
          })
        }

        // Assign found tour, or fallback to first tour
        tour.value = found || tours[0]
      }
    }
  } catch (e) {
    console.warn('Using default tour display data', e)
  } finally {
    loading.value = false
  }
}

watch(() => route.params.slug, () => {
  fetchTourData()
})

const toggleAddon = (id: number) => {
  const index = selectedAddons.value.indexOf(id)
  if (index > -1) {
    selectedAddons.value.splice(index, 1)
  } else {
    selectedAddons.value.push(id)
  }
}

const validateForm = () => {
  let isValid = true
  formErrors.value = { fullName: '', email: '', whatsapp: '', hotelName: '' }
  
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!bookingForm.value.email || !emailRegex.test(bookingForm.value.email.trim())) {
    formErrors.value.email = t("validation.emailInvalid")
    isValid = false
  }
  
  if (!bookingForm.value.whatsapp || bookingForm.value.whatsapp.trim().length < 6) {
    formErrors.value.whatsapp = t("validation.whatsappInvalid")
    isValid = false
  }
  
  if (!bookingForm.value.hotelName || bookingForm.value.hotelName.trim().length < 2) {
    formErrors.value.hotelName = t("validation.hotelNameRequired")
    isValid = false
  }

  // Validate GuestInfoForm
  const guestsValid = guestInfoFormRef.value ? guestInfoFormRef.value.validate() : true
  if (!guestsValid) {
    isValid = false
  }

  // GuestInfoForm now handles guest-specific details like passport and name.
  // We bypass those checks here.
  bookingForm.value.fullName = bookingForm.value.fullName || "Primary Guest"
  
  return isValid
}

const handleBookNow = () => {
  formErrors.value = { fullName: '', email: '', whatsapp: '', hotelName: '' }
  isBookingModalOpen.value = true
}

const confirmBooking = async () => {
  if (!validateForm()) {
    toastMessage.value = t("toast.formErrors")
    showToast.value = true
    setTimeout(() => { showToast.value = false }, 3500)
    return
  }
  
  bookingSubmitting.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const addonsPayload = selectedAddons.value.map(id => {
      const a = availableAddons.value.find((x: any) => x.id === id)
      if (!a) return null
      return {
        addonId: a.id,
        title: a.name,
        unitPrice: a.priceEur || a.price,
        quantity: 1
      }
    }).filter(Boolean)

    const payload = {
      tourId: tour.value?.id || '00000000-0000-0000-0000-000000000000',
      customerName: bookingForm.value.fullName.trim(),
      customerEmail: bookingForm.value.email.trim(),
      whatsApp: bookingForm.value.whatsapp.trim(),
      hotelName: bookingForm.value.hotelName.trim(),
      roomNumber: bookingForm.value.roomNumber.trim(),
      pickupTime: bookingForm.value.pickupTime,
      passportFileName: null,
      tourDate: selectedDate.value,
      guests: adultsCount.value + childrenCount.value,
      totalPrice: rawTotalPriceEur.value,
      hotelPickup: true,
      selectedAddons: addonsPayload,
      guestsList: guestInfoFormRef.value?.guests?.map((g: any) => ({
        fullName: g.fullName,
        passportFileName: g.documentUrl || g.passportFileName || '',
        ageCategory: g.ageCategory || (g.isChild ? 'Child' : 'Adult'),
        nationality: g.nationality || '',
        specialRequests: g.notes || g.specialRequests || ''
      })) || []
    }
    
    try {
      await fetch(`${API_URL}/api/booking/api/bookings`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
    } catch (e) {
      console.warn('Backend booking submission fallback', e)
    }
    
    toastMessage.value = t("toast.bookingConfirmed")
    showToast.value = true
    isBookingModalOpen.value = false
    setTimeout(() => { showToast.value = false }, 5000)
  } finally {
    bookingSubmitting.value = false
  }
}

const toggleSave = () => {
  isSaved.value = !isSaved.value
  toastMessage.value = isSaved.value ? t("toast.tourSaved") : 'Removed from favorites'
  showToast.value = true
  setTimeout(() => { showToast.value = false }, 3000)
}

const openLightbox = (idx: number) => {
  activeLightboxIndex.value = idx
  galleryModalOpen.value = true
}

const copyShareLink = () => {
  if (typeof navigator !== 'undefined' && navigator.clipboard) {
    navigator.clipboard.writeText(shareUrl.value)
  }
  toastMessage.value = t("toast.linkCopied")
  showToast.value = true
  showShareModal.value = false
  setTimeout(() => { showToast.value = false }, 3000)
}

const showPickupDropdown = ref(false)

// Global click outside for dropdowns
const handleGlobalClick = (e: MouseEvent) => {
  const target = e.target as HTMLElement
  if (!target.closest('.lang-dropdown-container')) {
    showLangDropdown.value = false
  }
  if (!target.closest('.currency-dropdown-container')) {
    showCurrencyDropdown.value = false
  }
  if (!target.closest('.pickup-dropdown-container')) {
    showPickupDropdown.value = false
  }
}

const handleTimelineScroll = () => {
  if (!timelineRef.value) return
  const rect = timelineRef.value.getBoundingClientRect()
  const windowHeight = window.innerHeight
  
  if (rect.top > windowHeight) {
    timelineProgress.value = 0
  } else if (rect.bottom < 0) {
    timelineProgress.value = 1
  } else {
    // Calculate progress as it scrolls through the viewport
    const totalHeight = rect.height
    const visibleHeight = windowHeight - rect.top
    let progress = (visibleHeight - windowHeight * 0.2) / (totalHeight + windowHeight * 0.2)
    progress = Math.max(0, Math.min(1, progress))
    timelineProgress.value = progress
  }
}

onMounted(async () => {
  await fetchTourData()
  window.addEventListener('scroll', handleTimelineScroll)
  window.addEventListener('click', handleGlobalClick)
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleTimelineScroll)
  window.removeEventListener('click', handleGlobalClick)
})

watch(routeSlug, () => {
  fetchTourData()
})
</script>

<template>
  <div class="product-details-root bg-[#f8f9fa] min-h-screen w-full text-[#0f172a] font-sans antialiased">
    
    <!-- TOP CLEAN NAVIGATION BAR (Full Width Header) -->
    <header class="sticky top-0 z-50 bg-white border-b border-[#e2e8f0] shadow-xs w-full">
      <div class="w-full max-w-[1480px] mx-auto px-3 sm:px-8 xl:px-12 h-16 flex items-center justify-between">
        
        <!-- Logo & Back link -->
        <div class="flex items-center gap-2 sm:gap-4 flex-shrink-0">
          <router-link to="/" class="flex items-center gap-1.5 sm:gap-2.5 text-decoration-none group flex-shrink-0">
            <img src="/logo-emblem.png" alt="Seadora" class="w-8 h-8 sm:w-10 sm:h-10 object-contain drop-shadow-sm group-hover:scale-105 transition-transform shrink-0" />
            <div class="flex flex-col shrink-0">
              <span class="font-serif text-sm sm:text-lg font-bold text-[#062d4d] leading-none tracking-wider whitespace-nowrap">SEADORA TRAVEL</span>
              <span class="text-[7px] sm:text-[9px] uppercase font-bold tracking-widest text-[#c9a84c] leading-none mt-0.5 whitespace-nowrap">Egypt</span>
            </div>
          </router-link>

          <div class="hidden sm:block h-5 w-[1px] bg-[#e2e8f0]"></div>

          <router-link to="/tours" class="hidden sm:flex items-center gap-1.5 text-xs font-semibold text-[#475569] hover:text-[#062d4d] transition-colors">
            <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 19l-7-7 7-7" />
            </svg>
            {{ i18nContent.allTours }}
          </router-link>
        </div>

        <!-- Right Header Actions (Language & Currency Dropdown Buttons) -->
        <div class="flex items-center gap-1 sm:gap-2.5 shrink-0">
          
          <!-- Language Selector Button with Dropdown -->
          <div class="relative lang-dropdown-container">
            <button 
              @click.stop="showLangDropdown = !showLangDropdown; showCurrencyDropdown = false"
              class="flex items-center gap-1 sm:gap-1.5 px-2 py-1.5 rounded-lg border border-[#e2e8f0] bg-white hover:bg-[#f1f5f9] text-xs font-bold text-[#0f172a] transition-all shadow-xs cursor-pointer"
              aria-label="Select Language"
            >
              <span class="text-sm leading-none">{{ currentLangObj.flag }}</span>
              <span class="hidden sm:inline">{{ currentLangObj.iso }}</span>
              <svg class="w-3 h-3 text-[#64748b] transition-transform hidden sm:block" :class="{ 'rotate-180': showLangDropdown }" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                <path stroke-linecap="round" stroke-linejoin="round" d="M19 9l-7 7-7-7" />
              </svg>
            </button>

            <!-- Language Dropdown Menu -->
            <div 
              v-if="showLangDropdown" 
              class="absolute right-0 mt-2 w-44 bg-white border border-[#e2e8f0] rounded-xl shadow-xl py-1.5 z-50 animate-in fade-in slide-in-from-top-2 duration-150"
            >
              <div class="px-3 py-1.5 text-[10px] uppercase font-bold tracking-wider text-[#94a3b8] border-b border-[#f1f5f9]">
                Choose Language
              </div>
              <button 
                v-for="l in languages" 
                :key="l.code"
                @click="setLanguage(l.code)"
                class="w-full px-3 py-2 text-left text-xs flex items-center justify-between hover:bg-[#f8fafc] transition-colors cursor-pointer"
                :class="{ 'bg-[#f0f9ff] text-[#0369a1] font-bold': locale === l.code }"
              >
                <div class="flex items-center gap-2">
                  <span class="text-sm">{{ l.flag }}</span>
                  <span>{{ l.label }}</span>
                </div>
                <span v-if="locale === l.code" class="text-xs text-[#0284c7]">✓</span>
              </button>
            </div>
          </div>

          <!-- Currency Selector Button with Dropdown -->
          <div class="relative currency-dropdown-container">
            <button 
              @click.stop="showCurrencyDropdown = !showCurrencyDropdown; showLangDropdown = false"
              class="flex items-center gap-1 sm:gap-1.5 px-2 sm:px-3 py-1.5 rounded-lg border border-[#e2e8f0] bg-white hover:bg-[#f1f5f9] text-xs font-bold text-[#0f172a] transition-all shadow-xs cursor-pointer"
              aria-label="Select Currency"
            >
              <span class="text-[#047857] font-extrabold leading-none">{{ currentCurrencyObj.symbol }}</span>
              <span class="hidden sm:inline">{{ currentCurrencyObj.code }}</span>
              <svg class="w-3 h-3 text-[#64748b] transition-transform hidden sm:block" :class="{ 'rotate-180': showCurrencyDropdown }" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                <path stroke-linecap="round" stroke-linejoin="round" d="M19 9l-7 7-7-7" />
              </svg>
            </button>

            <!-- Currency Dropdown Menu -->
            <div 
              v-if="showCurrencyDropdown" 
              class="absolute right-0 mt-2 w-48 bg-white border border-[#e2e8f0] rounded-xl shadow-xl py-1.5 z-50 animate-in fade-in slide-in-from-top-2 duration-150"
            >
              <div class="px-3 py-1.5 text-[10px] uppercase font-bold tracking-wider text-[#94a3b8] border-b border-[#f1f5f9]">
                Choose Currency
              </div>
              <button 
                v-for="c in currencies" 
                :key="c.code"
                @click="selectCurrency(c.code)"
                class="w-full px-3 py-2 text-left text-xs flex items-center justify-between hover:bg-[#f8fafc] transition-colors cursor-pointer"
                :class="{ 'bg-[#ecfdf5] text-[#047857] font-bold': currencyStore.selectedCurrency === c.code }"
              >
                <div class="flex items-center gap-2">
                  <span class="w-6 text-center font-extrabold text-[#047857]">{{ c.symbol }}</span>
                  <span>{{ c.code }}</span>
                </div>
                <span v-if="currencyStore.selectedCurrency === c.code" class="text-xs text-[#059669]">✓</span>
              </button>
            </div>
          </div>

          <!-- Share & Save Buttons -->
          <button 
            @click="showShareModal = true"
            class="p-2 rounded-lg border border-[#e2e8f0] hover:bg-[#f1f5f9] text-[#475569] transition-colors cursor-pointer"
            title="Share this experience"
          >
            <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
            </svg>
          </button>

          <button 
            @click="toggleSave"
            class="p-2 rounded-lg border border-[#e2e8f0] hover:bg-[#f1f5f9] transition-colors cursor-pointer"
            :class="isSaved ? 'text-[#e11d48] border-[#fecdd3] bg-[#fff1f2]' : 'text-[#475569]'"
            title="Save to favorites"
          >
            <svg class="w-4 h-4" :fill="isSaved ? 'currentColor' : 'none'" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
            </svg>
          </button>

        </div>
      </div>
    </header>

    <!-- FULL WIDTH PRODUCT CONTAINER -->
    <main class="w-full max-w-[1480px] mx-auto px-4 sm:px-6 xl:px-12 py-6 pb-28 sm:pb-12">
      
      <!-- DESKTOP TOP TITLE & BADGES ROW -->
      <div class="mb-5">
        <div class="flex items-center gap-2 text-xs text-[#64748b] mb-2 font-medium">
          <router-link to="/" class="hover:text-[#062d4d]">{{ i18nContent.home }}</router-link>
          <span>›</span>
          <router-link to="/tours" class="hover:text-[#062d4d]">{{ i18nContent.allTours }}</router-link>
          <span>›</span>
          <span class="text-[#0f172a] font-semibold truncate max-w-[280px]">{{ tourTitle }}</span>
        </div>

        <h1 class="font-serif text-2xl sm:text-3xl lg:text-[34px] font-bold text-[#0f172a] leading-snug tracking-tight mb-3">
          {{ tourTitle }}
        </h1>

        <!-- Rating & Trust Pills Header Row -->
        <div class="flex flex-wrap items-center gap-x-3 gap-y-2 text-[11px] sm:text-xs">
          <!-- Rating -->
          <div class="flex items-center gap-1.5 bg-[#ecfdf5] border border-[#a7f3d0] px-2.5 py-1 rounded-full text-[#065f46] font-bold">
            <span class="text-amber-500 text-sm">★</span>
            <span>{{ tourRating }}</span>
            <span class="text-[#047857] font-normal underline cursor-pointer">({{ tourReviewCount.toLocaleString() }} {{ $t('tourDetails.navigation.reviews') || 'reviews' }})</span>
          </div>

          <!-- Location -->
          <span class="bg-gray-100 border border-gray-200 text-[#475569] px-2.5 py-1 rounded-full flex items-center gap-1 font-medium">
            <span>📍</span> {{ i18nContent.egypt || 'Egypt' }}
          </span>

          <!-- Duration -->
          <span class="bg-gray-100 border border-gray-200 text-[#475569] px-2.5 py-1 rounded-full flex items-center gap-1 font-medium">
            <span>⏱️</span> {{ tourDuration }}
          </span>

          <!-- Badges -->
          <span v-if="tour?.isBestseller !== false" class="bg-[#fef3c7] text-[#92400e] border border-[#fde68a] px-2.5 py-1 rounded-full font-bold flex items-center gap-1">
            <span>👑</span> {{ i18nContent.bestseller || 'Bestseller' }}
          </span>
          
          <span v-if="tour?.isTopRated" class="bg-[#e0e7ff] text-[#3730a3] border border-[#c7d2fe] px-2.5 py-1 rounded-full font-bold flex items-center gap-1">
            <span>⭐</span> Top Rated
          </span>
          
          <span v-if="tour?.reserveAndPayLater !== false" class="bg-[#f0f9ff] text-[#0284c7] border border-[#bae6fd] px-2.5 py-1 rounded-full font-bold flex items-center gap-1">
            <span>💳</span> Reserve & Pay Later
          </span>

          <span v-if="tour?.freeCancellation !== false" class="bg-[#ecfdf5] text-[#059669] border border-[#a7f3d0] px-2.5 py-1 rounded-full font-bold flex items-center gap-1">
            <span>✓</span> {{ i18nContent.freeCancel || 'Free Cancellation' }}
          </span>
          
          <span v-if="tour?.hotelPickup !== false" class="bg-gray-100 text-[#475569] border border-gray-200 px-2.5 py-1 rounded-full font-bold flex items-center gap-1">
            <span>🚐</span> Hotel Pickup
          </span>
        </div>
      </div>

      <!-- AIRBNB-STYLE 5-PHOTO MOSAIC GALLERY -->
      <section class="mb-8 relative rounded-2xl overflow-hidden shadow-md">
        <div class="grid grid-cols-1 md:grid-cols-4 md:grid-rows-2 gap-2 h-[260px] sm:h-[340px] md:h-[450px] lg:h-[500px]">
          
          <!-- Large Hero Photo (Spans 2 cols, 2 rows on desktop) -->
          <div 
            @click="openLightbox(0)" 
            class="md:col-span-2 md:row-span-2 w-full h-full relative group cursor-pointer overflow-hidden bg-[#e2e8f0]"
          >
            <img 
              :src="galleryImages[0].url" 
              :alt="galleryImages[0].title"
              class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
            />
            <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity flex items-end p-5 text-white">
              <span class="text-base font-semibold">{{ galleryImages[0].title }}</span>
            </div>
          </div>

          <!-- 4 Thumbnail Grid Photos -->
          <div 
            v-for="(img, i) in galleryImages.slice(1, 5)" 
            :key="i"
            @click="openLightbox(Number(i) + 1)"
            class="hidden md:block relative group cursor-pointer overflow-hidden bg-[#e2e8f0]"
          >
            <img 
              :src="img.url" 
              :alt="img.title"
              class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
            />
            <div class="absolute inset-0 bg-black/20 group-hover:bg-transparent transition-colors"></div>

            <!-- "Show All Photos" Trigger on the last thumbnail -->
            <button 
              v-if="i === 3" 
              @click.stop="openLightbox(0)"
              class="absolute bottom-4 right-4 bg-white/95 backdrop-blur-md hover:bg-white text-[#0f172a] px-4 py-2.5 rounded-xl text-xs font-bold shadow-lg flex items-center gap-2 transition-all hover:scale-105 cursor-pointer"
            >
              <span>📸</span>
              <span>{{ i18nContent.viewAllPhotos }}</span>
            </button>
          </div>

        </div>
      </section>

      <!-- TWO-COLUMN RESPONSIVE LAYOUT (Left Content + Right Fixed 380-420px Sidebar) -->
      <div class="grid grid-cols-1 lg:grid-cols-[1fr_380px] xl:grid-cols-[1fr_420px] gap-8 xl:gap-12 items-start">
        
        <!-- LEFT COLUMN (Main Content) -->
        <div class="space-y-8 min-w-0">
          
          <!-- Experience Hook & Summary Teaser Card -->
          <div class="bg-white rounded-2xl p-6 sm:p-8 border border-[#e2e8f0] shadow-xs">
            <p class="text-base sm:text-lg font-semibold text-[#062d4d] italic mb-3">
              "{{ i18nContent.hookQuote }}"
            </p>
            <p class="text-sm sm:text-base text-[#475569] leading-relaxed">
              {{ tourDescription }}
            </p>

            <!-- Quick highlights grid -->
            <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 mt-6 pt-6 border-t border-[#f1f5f9]">
              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-xl bg-[#f0f9ff] text-[#0284c7] flex items-center justify-center text-lg flex-shrink-0">
                  ⏱️
                </div>
                <div>
                  <div class="text-[10px] uppercase font-bold text-[#94a3b8]">{{ i18nContent.duration || 'Duration' }}</div>
                  <div class="text-xs sm:text-sm font-bold text-[#0f172a]">{{ i18nContent.durationLabel }}</div>
                </div>
              </div>

              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-xl bg-[#ecfdf5] text-[#059669] flex items-center justify-center text-lg flex-shrink-0">
                  🚐
                </div>
                <div>
                  <div class="text-[10px] uppercase font-bold text-[#94a3b8]">{{ i18nContent.hotelTransfer }}</div>
                  <div class="text-xs sm:text-sm font-bold text-[#0f172a]">{{ i18nContent.included }}</div>
                </div>
              </div>

              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-xl bg-[#fef3c7] text-[#d97706] flex items-center justify-center text-lg flex-shrink-0">
                  🗣️
                </div>
                <div>
                  <div class="text-[10px] uppercase font-bold text-[#94a3b8]">{{ i18nContent.liveGuide }}</div>
                  <div class="text-xs sm:text-sm font-bold text-[#0f172a]">{{ i18nContent.guideLangs }}</div>
                </div>
              </div>

              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-xl bg-[#fdf2f8] text-[#db2777] flex items-center justify-center text-lg flex-shrink-0">
                  📱
                </div>
                <div>
                  <div class="text-[10px] uppercase font-bold text-[#94a3b8]">{{ i18nContent.mobileTicket }}</div>
                  <div class="text-xs sm:text-sm font-bold text-[#0f172a]">{{ i18nContent.instantVoucher }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- INTERACTIVE TABS BAR -->
          <div class="sticky top-16 z-40 bg-white/95 backdrop-blur-md border-b border-[#e2e8f0] -mx-4 px-4 sm:mx-0 sm:px-0 flex items-center gap-1.5 overflow-x-auto scrollbar-none py-1.5">
            <button 
              v-for="tab in [
                { id: 'overview', label: i18nContent.tabs.overview },
                { id: 'highlights', label: i18nContent.tabs.highlights },
                { id: 'itinerary', label: i18nContent.tabs.itinerary },
                { id: 'includes', label: i18nContent.tabs.includes },
                { id: 'info', label: i18nContent.tabs.info },
                { id: 'reviews', label: i18nContent.tabs.reviews },
                { id: 'faq', label: i18nContent.tabs.faq }
              ]"
              :key="tab.id"
              @click="activeTab = tab.id"
              class="px-4 py-2.5 text-xs sm:text-sm font-bold rounded-xl whitespace-nowrap transition-all cursor-pointer"
              :class="activeTab === tab.id ? 'bg-[#062d4d] text-white shadow-xs' : 'text-[#64748b] hover:text-[#0f172a] hover:bg-[#f1f5f9]'"
            >
              {{ tab.label }}
            </button>
          </div>

          <!-- TAB CONTENT PANELS -->

          <!-- 1. Overview -->
          <div v-show="activeTab === 'overview'" class="bg-white rounded-2xl p-6 sm:p-8 border border-[#e2e8f0] shadow-xs space-y-4">
            <h3 class="text-lg sm:text-xl font-bold text-[#0f172a]">{{ i18nContent.descHeading }}</h3>
            <div class="text-sm sm:text-base text-[#475569] leading-relaxed space-y-3.5">
              <p>
                {{ tourDescription }}
              </p>
              <div v-show="readMoreExpanded" class="space-y-3.5 animate-in fade-in">
                <p>
                  As the sun sinks behind the Sinai mountains, your quad bike is fueled and waiting. Twist the throttle and let your automatic ATV carry you across rolling sand dunes. At the Echo Mountains, shout into the canyon and hear the rock reply.
                </p>
                <p>
                  Arrive at the Bedouin camp to be welcomed with fragrant Habak herbal tea. Relish a rich open BBQ buffet under starry skies, followed by a breathtaking whirling Tanoura dance and an adrenaline-fueled fire show.
                </p>
              </div>
            </div>
            <button 
              @click="readMoreExpanded = !readMoreExpanded"
              class="text-xs sm:text-sm font-bold text-[#062d4d] hover:underline flex items-center gap-1.5 pt-2 cursor-pointer"
            >
              <span>{{ readMoreExpanded ? i18nContent.showLess : i18nContent.readMore }}</span>
              <span>{{ readMoreExpanded ? '↑' : '↓' }}</span>
            </button>
          </div>

          <!-- 2. Highlights -->
          <div v-show="activeTab === 'highlights' || activeTab === 'overview'" class="bg-white rounded-2xl p-6 sm:p-8 border border-[#e2e8f0] shadow-xs">
            <h3 class="text-lg sm:text-xl font-bold text-[#0f172a] mb-5">{{ i18nContent.highlightsHeading }}</h3>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div v-for="(h, idx) in tourHighlights" :key="idx" class="flex items-start gap-3 bg-[#f8fafc] p-4 rounded-xl border border-[#f1f5f9]">
                <span class="text-emerald-600 text-lg font-bold">★</span>
                <span class="text-xs sm:text-sm text-[#334155] font-semibold leading-relaxed">{{ h }}</span>
              </div>
            </div>
          </div>

          <!-- 3. Itinerary Timeline -->
          <div v-show="activeTab === 'itinerary' || activeTab === 'overview'" class="bg-white rounded-2xl p-6 sm:p-8 border border-[#e2e8f0] shadow-xs">
            <h3 class="text-lg sm:text-xl font-bold text-[#0f172a] mb-6">{{ i18nContent.itineraryHeading }}</h3>
            <div class="relative pl-8 space-y-7" ref="timelineRef">
              <!-- Animated Timeline Progress Line -->
              <div class="absolute left-3 top-2 bottom-2 w-0.5 bg-[#e2e8f0] rounded-full overflow-hidden">
                <div class="w-full bg-[#062d4d] origin-top transition-transform duration-300 ease-out"
                     :style="{ transform: `scaleY(${timelineProgress})`, height: '100%' }"></div>
              </div>

              <div v-for="(step, idx) in tourItinerary" :key="idx" class="relative group">
                <!-- Marker Dot -->
                <div class="absolute -left-[35px] top-0.5 w-6 h-6 rounded-full bg-[#062d4d] text-white text-xs font-bold flex items-center justify-center ring-4 ring-white shadow-xs transition-transform duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] group-hover:scale-125">
                  {{ Number(idx) + 1 }}
                </div>
                <div>
                  <span class="text-xs font-bold uppercase tracking-wider text-[#0284c7] block">{{ step.time }}</span>
                  <h4 class="text-sm sm:text-base font-bold text-[#0f172a] mt-0.5">{{ step.title }}</h4>
                  <p class="text-xs sm:text-sm text-[#64748b] mt-1 leading-relaxed">{{ step.desc }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- 4. Inclusions & Exclusions -->
          <div v-show="activeTab === 'includes' || activeTab === 'overview'" class="bg-white rounded-2xl p-6 sm:p-8 border border-[#e2e8f0] shadow-xs">
            <h3 class="text-lg sm:text-xl font-bold text-[#0f172a] mb-6">{{ i18nContent.includesHeading }}</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
              
              <!-- Included -->
              <div class="space-y-3.5">
                <h4 class="text-xs sm:text-sm uppercase font-extrabold tracking-wider text-[#059669] flex items-center gap-2 pb-2.5 border-b border-[#ecfdf5]">
                  <span>✓</span> {{ i18nContent.includedTitle }}
                </h4>
                <ul class="space-y-3">
                  <li v-for="(item, i) in tourInclusions" :key="i" class="flex items-start gap-2.5 text-xs sm:text-sm text-[#334155]">
                    <span class="text-emerald-500 font-bold mt-0.5">✓</span>
                    <span>{{ item }}</span>
                  </li>
                </ul>
              </div>

              <!-- Excluded -->
              <div class="space-y-3.5">
                <h4 class="text-xs sm:text-sm uppercase font-extrabold tracking-wider text-[#dc2626] flex items-center gap-2 pb-2.5 border-b border-[#fef2f2]">
                  <span>✕</span> {{ i18nContent.excludedTitle }}
                </h4>
                <ul class="space-y-3">
                  <li v-for="(item, i) in tourExclusions" :key="i" class="flex items-start gap-2.5 text-xs sm:text-sm text-[#64748b]">
                    <span class="text-red-400 font-bold mt-0.5">✕</span>
                    <span>{{ item }}</span>
                  </li>
                </ul>
              </div>

            </div>
          </div>

          <!-- 5. Important Info / What to bring -->
          <div v-show="activeTab === 'info' || activeTab === 'overview'" class="bg-white rounded-2xl p-6 sm:p-8 border border-[#e2e8f0] shadow-xs space-y-5">
            <h3 class="text-lg sm:text-xl font-bold text-[#0f172a]">{{ i18nContent.infoHeading }}</h3>
            
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
              <div class="bg-[#f8fafc] p-5 rounded-2xl border border-[#e2e8f0]">
                <h4 class="text-xs sm:text-sm font-bold text-[#0f172a] mb-2.5 flex items-center gap-2">
                  <span>🎒</span> {{ i18nContent.whatToBringTitle }}
                </h4>
                <ul class="text-xs sm:text-sm text-[#475569] space-y-2 list-disc list-inside">
                  <li v-for="(wb, i) in tourWhatToBring" :key="i">{{ wb }}</li>
                </ul>
              </div>

              <div class="flex flex-col gap-5">
                <div class="bg-[#fff7ed] p-5 rounded-2xl border border-[#ffedd5] flex-1">
                  <h4 class="text-xs sm:text-sm font-bold text-[#c2410c] mb-2.5 flex items-center gap-2">
                    <span>⚠️</span> {{ i18nContent.notSuitableTitle }}
                  </h4>
                  <p class="text-xs sm:text-sm text-[#9a3412] leading-relaxed">
                    {{ tourNotSuitable }}
                  </p>
                </div>

                <div v-if="tourNotes" class="bg-[#f0fdfa] p-5 rounded-2xl border border-[#ccfbf1] flex-1">
                  <h4 class="text-xs sm:text-sm font-bold text-[#0f766e] mb-2.5 flex items-center gap-2">
                    <span>💡</span> Notes / Need to Know
                  </h4>
                  <p class="text-xs sm:text-sm text-[#115e59] leading-relaxed">
                    {{ tourNotes }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- 6. Customer Reviews -->
          <div v-show="activeTab === 'reviews' || activeTab === 'overview'" class="bg-white rounded-2xl p-6 sm:p-8 border border-[#e2e8f0] shadow-xs space-y-6">
            <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 pb-6 border-b border-[#f1f5f9]">
              <div>
                <h3 class="text-lg sm:text-xl font-bold text-[#0f172a]">{{ i18nContent.reviewsHeading }}</h3>
                <p class="text-xs sm:text-sm text-[#64748b] mt-0.5">{{ i18nContent.reviewsSub }}</p>
              </div>

              <div class="flex items-center gap-3">
                <div class="text-3xl sm:text-4xl font-extrabold text-[#062d4d]">4.9</div>
                <div>
                  <div class="flex text-amber-400 text-base">★★★★★</div>
                  <div class="text-xs text-[#64748b] font-medium">{{ i18nContent.reviewsCount }}</div>
                </div>
              </div>
            </div>

            <!-- Review Cards -->
            <div class="space-y-4">
              <div class="bg-[#f8fafc] p-4 sm:p-5 rounded-xl border border-[#f1f5f9] space-y-2.5">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-2.5">
                    <div class="w-8 h-8 rounded-full bg-[#062d4d] text-white text-xs font-bold flex items-center justify-center">
                      MK
                    </div>
                    <div>
                      <span class="text-xs sm:text-sm font-bold text-[#0f172a] block">Maximilian Klein 🇩🇪</span>
                      <span class="text-[10px] sm:text-xs text-[#94a3b8]">August 2026 · {{ i18nContent.verifiedBooking }}</span>
                    </div>
                  </div>
                  <div class="text-amber-400 text-xs sm:text-sm">★★★★★</div>
                </div>
                <p class="text-xs sm:text-sm text-[#475569] leading-relaxed">
                  "The sunset ATV ride was sensational! The instructors were very attentive and patient. The Bedouin camp dinner was delicious and the fire show was mesmerizing. Excellent value for money!"
                </p>
              </div>

              <div class="bg-[#f8fafc] p-4 sm:p-5 rounded-xl border border-[#f1f5f9] space-y-2.5">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-2.5">
                    <div class="w-8 h-8 rounded-full bg-[#c9a84c] text-white text-xs font-bold flex items-center justify-center">
                      EL
                    </div>
                    <div>
                      <span class="text-xs sm:text-sm font-bold text-[#0f172a] block">Elena Rossi 🇮🇹</span>
                      <span class="text-[10px] sm:text-xs text-[#94a3b8]">July 2026 · {{ i18nContent.verifiedBooking }}</span>
                    </div>
                  </div>
                  <div class="text-amber-400 text-xs sm:text-sm">★★★★★</div>
                </div>
                <p class="text-xs sm:text-sm text-[#475569] leading-relaxed">
                  "Un'esperienza fantastica! Il giro in quad è divertentissimo, i cammelli docili e il cibo ottimo. Consiglio a tutti di portare una sciarpa per la polvere."
                </p>
              </div>
            </div>

            <!-- Review Submission Form -->
            <div class="mt-8 pt-6 border-t border-[#e2e8f0]">
              <h4 class="text-base sm:text-lg font-bold text-[#0f172a] mb-4">{{ i18nContent.reviewForm.title }}</h4>
              <form @submit.prevent="" class="space-y-4 bg-[#f8fafc] p-4 sm:p-5 rounded-xl border border-[#f1f5f9]">
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-xs font-bold text-[#334155] mb-1.5">{{ i18nContent.reviewForm.yourName }}</label>
                    <input type="text" class="w-full px-3.5 py-2.5 rounded-xl border border-[#cbd5e1] text-xs sm:text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] bg-white" />
                  </div>
                  <div>
                    <label class="block text-xs font-bold text-[#334155] mb-1.5">{{ i18nContent.reviewForm.yourRating }}</label>
                    <select class="w-full px-3.5 py-2.5 rounded-xl border border-[#cbd5e1] text-xs sm:text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] bg-white text-[#475569]">
                      <option value="5">★★★★★ (5/5)</option>
                      <option value="4">★★★★☆ (4/5)</option>
                      <option value="3">★★★☆☆ (3/5)</option>
                      <option value="2">★★☆☆☆ (2/5)</option>
                      <option value="1">★☆☆☆☆ (1/5)</option>
                    </select>
                  </div>
                </div>
                <div>
                  <label class="block text-xs font-bold text-[#334155] mb-1.5">{{ i18nContent.reviewForm.reviewTitle }}</label>
                  <input type="text" class="w-full px-3.5 py-2.5 rounded-xl border border-[#cbd5e1] text-xs sm:text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] bg-white" />
                </div>
                <div>
                  <label class="block text-xs font-bold text-[#334155] mb-1.5">{{ i18nContent.reviewForm.shareExperience }}</label>
                  <textarea rows="4" class="w-full px-3.5 py-2.5 rounded-xl border border-[#cbd5e1] text-xs sm:text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] bg-white resize-none" :placeholder="i18nContent.reviewForm.shareExperience"></textarea>
                </div>
                <div class="flex justify-end pt-2">
                  <button type="submit" class="px-6 py-3 bg-[#062d4d] hover:bg-[#0a3f6b] text-white text-xs sm:text-sm font-bold rounded-xl shadow-md transition-colors cursor-pointer">
                    {{ i18nContent.reviewForm.submitReview }}
                  </button>
                </div>
              </form>
            </div>
          </div>

          <!-- 7. FAQ Accordion -->
          <div v-show="activeTab === 'faq' || activeTab === 'overview'" class="bg-white rounded-2xl p-6 sm:p-8 border border-[#e2e8f0] shadow-xs space-y-4">
            <h3 class="text-lg sm:text-xl font-bold text-[#0f172a]">{{ i18nContent.faqHeading }}</h3>
            <div class="space-y-3">
              <div 
                v-for="(f, idx) in tourFaqs" 
                :key="idx" 
                class="border border-[#e2e8f0] rounded-xl overflow-hidden"
              >
                <button 
                  @click="activeFaq = activeFaq === Number(idx) ? null : Number(idx)"
                  class="w-full px-4 sm:px-5 py-3.5 text-left text-xs sm:text-sm font-bold text-[#0f172a] flex items-center justify-between hover:bg-[#f8fafc] transition-colors cursor-pointer"
                >
                  <span>{{ f.q }}</span>
                  <svg 
                    xmlns="http://www.w3.org/2000/svg" 
                    fill="none" 
                    viewBox="0 0 24 24" 
                    stroke-width="2" 
                    stroke="currentColor" 
                    class="w-4 h-4 text-[#64748b] transition-transform duration-300 ease-[cubic-bezier(0.16,1,0.3,1)]"
                    :class="activeFaq === Number(idx) ? 'rotate-180' : ''"
                  >
                    <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
                  </svg>
                </button>
                <div 
                  class="grid transition-[grid-template-rows] duration-300 ease-[cubic-bezier(0.16,1,0.3,1)]"
                  :class="activeFaq === Number(idx) ? 'grid-rows-[1fr]' : 'grid-rows-[0fr]'"
                >
                  <div class="overflow-hidden">
                    <div class="px-4 sm:px-5 pb-4 text-xs sm:text-sm text-[#475569] leading-relaxed bg-[#f8fafc]/50">
                      {{ f.a }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

        </div>

        <!-- RIGHT COLUMN (Sticky Booking Sidebar Widget - Fixed Width 380px/420px) -->
        <aside class="sticky top-20 z-30 w-full">
          
          <div class="bg-white rounded-2xl p-6 sm:p-7 border border-[#cbd5e1] shadow-xl">
            
            <!-- Price Display -->
            <div class="pb-4 border-b border-[#f1f5f9] mb-5">
              <div class="flex items-baseline justify-between">
                <span class="text-[11px] uppercase font-extrabold tracking-wider text-[#64748b]">{{ i18nContent.sidebar.startingFrom }}</span>
                <span v-if="saveBadgeText" class="bg-[#dcfce7] text-[#15803d] text-[10px] font-extrabold px-2.5 py-0.5 rounded-md">{{ saveBadgeText }}</span>
              </div>
              <div class="flex items-baseline gap-2 mt-1">
                <span class="text-3xl sm:text-4xl font-extrabold text-[#062d4d]">{{ basePriceFormatted }}</span>
                <span v-if="currentOption.wasPriceEur" class="text-sm text-[#94a3b8] line-through">{{ wasPriceFormatted }}</span>
                <span class="text-xs text-[#64748b]">{{ i18nContent.sidebar.perPerson }}</span>
              </div>
            </div>

            <!-- Booking Form Inputs -->
            <div class="space-y-4">
              
              <!-- 1. Date Picker -->
              <div>
                <label class="block text-[11px] font-bold text-[#0f172a] uppercase tracking-wider mb-1.5">{{ i18nContent.sidebar.step1 }}</label>
                <TourAvailabilityCalendar 
                  v-model="selectedDate" 
                  :base-price-eur="tour?.price || 45" 
                  :available-dates="tour?.availableDates"
                />
              </div>

              <!-- 2. Tour Package Variant Option -->
              <div v-if="dynamicOptions.length > 0">
                <label class="block text-[11px] font-bold text-[#0f172a] uppercase tracking-wider mb-1.5">{{ i18nContent.sidebar.step2 }}</label>
                <div class="space-y-2">
                  <div 
                    v-for="(opt, idx) in dynamicOptions" 
                    :key="idx"
                    @click="selectedOptionIndex = Number(idx)"
                    class="p-3.5 rounded-xl border cursor-pointer transition-all"
                    :class="selectedOptionIndex === Number(idx) ? 'border-[#062d4d] bg-[#f0f9ff] ring-1 ring-[#062d4d]' : 'border-[#e2e8f0] hover:border-[#cbd5e1]'"
                  >
                    <div class="flex items-start justify-between gap-2">
                      <div>
                        <div class="text-xs sm:text-sm font-bold text-[#0f172a]">{{ opt.title }}</div>
                        <div class="text-[10px] sm:text-xs text-[#64748b] mt-0.5">{{ opt.subtitle }}</div>
                      </div>
                      <div class="text-right flex-shrink-0">
                        <span class="text-xs sm:text-sm font-extrabold text-[#062d4d] block">{{ currencyStore.formatPrice(opt.basePriceEur) }}</span>
                      </div>
                    </div>
                    <ul v-if="opt.features && opt.features.length" class="mt-2 space-y-1">
                      <li v-for="(feat, fIdx) in opt.features" :key="fIdx" class="text-[10px] sm:text-xs text-[#475569] flex items-start gap-1">
                        <span class="text-emerald-500 font-bold">✓</span> {{ feat }}
                      </li>
                    </ul>
                  </div>
                </div>
              </div>

              <!-- 3. Guest Counters -->
              <div>
                <label class="block text-[11px] font-bold text-[#0f172a] uppercase tracking-wider mb-1.5">{{ i18nContent.sidebar.step3 }}</label>
                <div class="grid grid-cols-2 gap-3">
                  <!-- Adults -->
                  <div class="p-3 rounded-xl border border-[#e2e8f0] flex items-center justify-between">
                    <div>
                      <span class="text-xs sm:text-sm font-bold block text-[#0f172a]">{{ i18nContent.sidebar.adults }}</span>
                      <span class="text-[9px] sm:text-[10px] text-[#94a3b8]">{{ i18nContent.sidebar.adultsAge }}</span>
                    </div>
                    <div class="flex items-center gap-2">
                      <button 
                        @click="adultsCount = Math.max(1, adultsCount - 1)" 
                        class="w-6 h-6 rounded-md bg-[#f1f5f9] font-bold text-xs hover:bg-[#e2e8f0] cursor-pointer"
                      >-</button>
                      <span class="text-xs sm:text-sm font-bold w-4 text-center">{{ adultsCount }}</span>
                      <button 
                        @click="adultsCount++" 
                        class="w-6 h-6 rounded-md bg-[#f1f5f9] font-bold text-xs hover:bg-[#e2e8f0] cursor-pointer"
                      >+</button>
                    </div>
                  </div>

                  <!-- Children -->
                  <div class="p-3 rounded-xl border border-[#e2e8f0] flex items-center justify-between">
                    <div>
                      <span class="text-xs sm:text-sm font-bold block text-[#0f172a]">{{ i18nContent.sidebar.children }}</span>
                      <span class="text-[9px] sm:text-[10px] text-[#94a3b8]">{{ i18nContent.sidebar.childrenAge }}</span>
                    </div>
                    <div class="flex items-center gap-2">
                      <button 
                        @click="childrenCount = Math.max(0, childrenCount - 1)" 
                        class="w-6 h-6 rounded-md bg-[#f1f5f9] font-bold text-xs hover:bg-[#e2e8f0] cursor-pointer"
                      >-</button>
                      <span class="text-xs sm:text-sm font-bold w-4 text-center">{{ childrenCount }}</span>
                      <button 
                        @click="childrenCount++" 
                        class="w-6 h-6 rounded-md bg-[#f1f5f9] font-bold text-xs hover:bg-[#e2e8f0] cursor-pointer"
                      >+</button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- 4. Optional Add-ons -->
              <div v-if="availableAddons && availableAddons.length > 0">
                <label class="block text-[11px] font-bold text-[#0f172a] uppercase tracking-wider mb-1.5">{{ i18nContent.sidebar.step4 }}</label>
                <div class="space-y-2">
                  <label 
                    v-for="addon in availableAddons" 
                    :key="addon.id"
                    class="flex items-center justify-between p-2.5 rounded-xl border border-[#f1f5f9] hover:bg-[#f8fafc] cursor-pointer text-xs sm:text-sm"
                  >
                    <div class="flex items-center gap-2.5">
                      <input 
                        type="checkbox" 
                        :checked="selectedAddons.includes(addon.id)"
                        @change="toggleAddon(addon.id)"
                        class="rounded text-[#062d4d] focus:ring-[#062d4d] cursor-pointer"
                      />
                      <span class="text-[#334155]">{{ getLocalized(addon.names, addon.name) }}</span>
                    </div>
                    <span class="font-bold text-[#047857] flex-shrink-0">+{{ currencyStore.formatPrice(addon.priceEur || addon.price) }}</span>
                  </label>
                </div>
              </div>

              <!-- Total Price Breakdown -->
              <div class="pt-4 border-t border-[#f1f5f9] flex items-center justify-between">
                <div>
                  <span class="text-xs sm:text-sm font-bold text-[#64748b] block">{{ i18nContent.sidebar.totalAmount }}</span>
                  <span class="text-[10px] sm:text-xs text-[#059669] font-semibold">{{ i18nContent.sidebar.taxesIncluded }}</span>
                </div>
                <div class="text-right">
                  <span class="text-2xl sm:text-3xl font-extrabold text-[#062d4d]">{{ totalPriceFormatted }}</span>
                </div>
              </div>

              <!-- Primary Book CTA Button -->
              <button 
                @click="handleBookNow"
                class="w-full py-4 rounded-xl bg-gradient-to-r from-[#062d4d] to-[#0d4f8b] hover:from-[#0a3f6b] hover:to-[#0f5c9e] text-white text-sm sm:text-base font-extrabold tracking-wide shadow-md hover:shadow-lg transition-all hover:scale-[1.01] active:scale-[0.99] flex items-center justify-center gap-2 cursor-pointer"
              >
                <span>{{ i18nContent.sidebar.bookBtn }}</span>
                <span>→</span>
              </button>

              <p class="text-center text-[10px] sm:text-xs text-[#059669] font-bold flex items-center justify-center gap-1.5">
                <span>⚡</span> {{ i18nContent.sidebar.instantConfirmation }}
              </p>

            </div>

            <!-- Trust Badges in Sidebar -->
            <div class="mt-6 pt-4 border-t border-[#f1f5f9] space-y-2 text-[11px] sm:text-xs text-[#64748b]">
              <div class="flex items-center gap-2.5">
                <span class="text-[#047857]">🔒</span>
                <span>{{ i18nContent.sidebar.trust1 }}</span>
              </div>
              <div class="flex items-center gap-2.5">
                <span class="text-[#0284c7]">💬</span>
                <span>{{ i18nContent.sidebar.trust2 }}</span>
              </div>
              <div class="flex items-center gap-2.5">
                <span class="text-[#d97706]">🏆</span>
                <span>{{ i18nContent.sidebar.trust3 }}</span>
              </div>
            </div>

          </div>

        </aside>

      </div>

    </main>

    <!-- MOBILE STICKY BOTTOM BAR -->
    <div class="lg:hidden fixed bottom-0 left-0 right-0 z-40 bg-white/95 backdrop-blur-md border-t border-slate-200/80 px-4 py-3 pb-safe flex items-center justify-between shadow-[0_-8px_25px_rgba(6,45,77,0.08)]">
      <div>
        <span class="text-[10px] uppercase font-bold text-slate-500 block">{{ i18nContent.totalPrice || $t('tourDetails.sidebar.totalPrice') || 'Total Price' }}</span>
        <div class="flex items-baseline gap-1">
          <span class="text-lg font-extrabold text-[#062d4d]">{{ totalPriceFormatted }}</span>
        </div>
      </div>
      <button 
        @click="handleBookNow"
        class="px-6 py-2.5 rounded-xl bg-[#d97706] hover:bg-[#b45309] active:scale-95 transition-transform duration-300 ease-out text-white text-xs font-bold shadow-lg shadow-[#d97706]/20 cursor-pointer"
      >
        {{ i18nContent.bookNow || $t('tourDetails.sidebar.bookNow') || 'Book Now' }}
      </button>
    </div>

    <!-- PHOTO GALLERY LIGHTBOX MODAL -->
    <div 
      v-if="galleryModalOpen" 
      class="fixed inset-0 z-50 bg-black/95 flex flex-col justify-between p-4 sm:p-8 animate-in fade-in"
    >
      <div class="flex items-center justify-between text-white">
        <span class="text-xs font-bold tracking-wider">{{ activeLightboxIndex + 1 }} / {{ galleryImages.length }}</span>
        <button 
          @click="galleryModalOpen = false" 
          class="text-white text-sm font-bold bg-white/10 hover:bg-white/20 px-3 py-1.5 rounded-lg cursor-pointer"
        >
          ✕ Close
        </button>
      </div>

      <div class="flex-1 flex items-center justify-center p-4">
        <img 
          :src="galleryImages[activeLightboxIndex].url" 
          :alt="galleryImages[activeLightboxIndex].title" 
          class="max-h-[80vh] max-w-full rounded-xl object-contain shadow-2xl"
        />
      </div>

      <div class="text-center text-white text-xs font-medium">
        <p class="font-bold text-sm">{{ galleryImages[activeLightboxIndex].title }}</p>
        <p class="text-white/70 text-[11px]">{{ galleryImages[activeLightboxIndex].caption }}</p>
      </div>

      <div class="flex justify-center gap-4 mt-2">
        <button 
          @click="activeLightboxIndex = (activeLightboxIndex - 1 + galleryImages.length) % galleryImages.length"
          class="px-4 py-2 bg-white/10 hover:bg-white/20 text-white rounded-lg text-xs font-bold cursor-pointer"
        >
          ← Prev
        </button>
        <button 
          @click="activeLightboxIndex = (activeLightboxIndex + 1) % galleryImages.length"
          class="px-4 py-2 bg-white/10 hover:bg-white/20 text-white rounded-lg text-xs font-bold cursor-pointer"
        >
          Next →
        </button>
      </div>
    </div>

    <!-- SHARE MODAL -->
    <div 
      v-if="showShareModal" 
      class="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4"
    >
      <div class="bg-white rounded-2xl p-6 max-w-md w-full shadow-2xl space-y-4">
        <div class="flex items-center justify-between">
          <h3 class="text-base font-bold text-[#0f172a]">Share this Tour</h3>
          <button @click="showShareModal = false" class="text-gray-400 hover:text-gray-600 cursor-pointer">✕</button>
        </div>
        <p class="text-xs text-[#64748b]">Copy the direct link to share this experience with friends or family:</p>
        <div class="flex items-center gap-2">
          <input 
            type="text" 
            readonly 
            :value="shareUrl" 
            class="flex-1 px-3 py-2 text-xs bg-[#f8fafc] border rounded-lg"
          />
          <button 
            @click="copyShareLink"
            class="px-4 py-2 bg-[#062d4d] text-white text-xs font-bold rounded-lg hover:bg-[#0a3f6b] cursor-pointer"
          >
            Copy
          </button>
        </div>
      </div>
    </div>

    <!-- BOOKING POPUP MODAL (Glassmorphism & High-End Typography) -->
    <Transition name="fade">
      <div v-if="isBookingModalOpen" class="fixed inset-0 z-[60] flex items-center justify-center p-4">
        <!-- Frosted Glass Backdrop -->
        <div class="absolute inset-0 bg-black/50 backdrop-blur-md cursor-pointer" @click="isBookingModalOpen = false"></div>
        
        <!-- Modal Content -->
        <div class="relative bg-white w-full max-w-lg rounded-3xl shadow-2xl overflow-hidden flex flex-col max-h-[92vh] border border-[#e2e8f0]">
          
          <!-- Header -->
          <div class="px-6 py-4.5 border-b border-[#f1f5f9] flex items-center justify-between bg-white z-10">
            <div>
              <h2 class="text-xl font-bold text-[#0f172a] tracking-tight">{{ $t("bookingPopup.header") }}</h2>
              <p class="text-xs text-[#64748b] mt-0.5">{{ $t("bookingPopup.subHeader") }}</p>
            </div>
            <button @click="isBookingModalOpen = false" class="w-8 h-8 rounded-full bg-[#f1f5f9] hover:bg-[#e2e8f0] text-[#64748b] flex items-center justify-center transition-colors cursor-pointer">
              ✕
            </button>
          </div>

          <!-- Scrollable Form Area -->
          <div class="p-6 overflow-y-auto space-y-4.5 flex-1 bg-[#f8fafc]">
            
            <!-- 1. Contact Information -->
            <div class="space-y-3.5 bg-white p-5 rounded-2xl border border-[#e2e8f0] shadow-xs">
              <div class="flex items-center justify-between">
                <h3 class="text-xs uppercase font-extrabold tracking-wider text-[#062d4d] flex items-center gap-1.5">
                  <span>📞</span> Contact Information
                </h3>
                <span class="text-[10px] bg-[#dcfce7] text-[#15803d] font-bold px-2 py-0.5 rounded-full flex items-center gap-1">
                  <span>💬</span> {{ $t("bookingPopup.whatsappVoucher") }}
                </span>
              </div>
              
              <div class="space-y-3">
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  <div>
                    <label class="block text-xs font-bold text-[#334155] mb-1">{{ $t("bookingPopup.email") }} <span class="text-red-500">*</span></label>
                    <input 
                      type="email" 
                      v-model="bookingForm.email" 
                      @input="formErrors.email = ''"
                      :placeholder="$t('placeholders.email')" 
                      class="w-full px-3.5 py-2.5 rounded-xl border text-xs sm:text-sm font-medium focus:outline-none transition-colors bg-white"
                      :class="formErrors.email ? 'border-red-400 focus:border-red-500 focus:ring-1 focus:ring-red-500 bg-red-50/30' : 'border-[#cbd5e1] focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d]'"
                    />
                    <p v-if="formErrors.email" class="text-[11px] text-red-500 font-semibold mt-1 flex items-center gap-1">
                      <span>⚠️</span> {{ formErrors.email }}
                    </p>
                  </div>

                  <div>
                    <label class="block text-xs font-bold text-[#334155] mb-1">{{ $t("bookingPopup.whatsapp") }} <span class="text-red-500">*</span></label>
                    <input 
                      type="tel" 
                      v-model="bookingForm.whatsapp" 
                      @input="formErrors.whatsapp = ''"
                      :placeholder="$t('placeholders.whatsapp')" 
                      class="w-full px-3.5 py-2.5 rounded-xl border text-xs sm:text-sm font-medium focus:outline-none transition-colors bg-white"
                      :class="formErrors.whatsapp ? 'border-red-400 focus:border-red-500 focus:ring-1 focus:ring-red-500 bg-red-50/30' : 'border-[#cbd5e1] focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d]'"
                    />
                    <p v-if="formErrors.whatsapp" class="text-[11px] text-red-500 font-semibold mt-1 flex items-center gap-1">
                      <span>⚠️</span> {{ formErrors.whatsapp }}
                    </p>
                  </div>
                </div>
                <p class="text-[11px] text-[#64748b]">{{ $t("bookingPopup.whatsappNotice") }}</p>
              </div>
            </div>

            <!-- 1.5 Guest Information -->
            <GuestInfoForm ref="guestInfoFormRef" :guest-count="adultsCount + childrenCount" />

            <!-- 2. Hotel Name, Room Number & Pickup Time -->
            <div class="space-y-3.5 bg-white p-5 rounded-2xl border border-[#e2e8f0] shadow-xs">
              <h3 class="text-xs uppercase font-extrabold tracking-wider text-[#062d4d] flex items-center gap-1.5">
                <span>🚐</span> {{ $t("bookingPopup.hotelPickup") }}
              </h3>
              
              <div class="space-y-3">
                <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                  <div class="sm:col-span-2">
                    <label class="block text-xs font-bold text-[#334155] mb-1">{{ $t("bookingPopup.hotelName") }} <span class="text-red-500">*</span></label>
                    <input 
                      type="text" 
                      v-model="bookingForm.hotelName" 
                      @input="formErrors.hotelName = ''"
                      :placeholder="$t('placeholders.hotelName')" 
                      class="w-full px-3.5 py-2.5 rounded-xl border text-xs sm:text-sm font-medium focus:outline-none transition-colors bg-white"
                      :class="formErrors.hotelName ? 'border-red-400 focus:border-red-500 focus:ring-1 focus:ring-red-500 bg-red-50/30' : 'border-[#cbd5e1] focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d]'"
                    />
                    <p v-if="formErrors.hotelName" class="text-[11px] text-red-500 font-semibold mt-1 flex items-center gap-1">
                      <span>⚠️</span> {{ formErrors.hotelName }}
                    </p>
                  </div>

                  <div>
                    <label class="block text-xs font-bold text-[#334155] mb-1">{{ $t("bookingPopup.roomNumber") }}</label>
                    <input 
                      type="text" 
                      v-model="bookingForm.roomNumber" 
                      :placeholder="$t('placeholders.roomNumber')" 
                      class="w-full px-3.5 py-2.5 rounded-xl border border-[#cbd5e1] text-xs sm:text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] bg-white" 
                    />
                  </div>
                </div>

                <!-- 1. Fixed Slots Mode -->
                <div v-if="pickupTimeType === 'FixedSlots'">
                  <label class="block text-xs font-bold text-[#334155] mb-1.5">{{ $t("bookingPopup.pickupTime") }}</label>
                  <div class="relative pickup-dropdown-container">
                    <button
                      type="button"
                      @click.stop="showPickupDropdown = !showPickupDropdown"
                      class="w-full px-3.5 py-2.5 rounded-xl border text-xs sm:text-sm font-semibold text-left flex items-center justify-between transition-all bg-white cursor-pointer select-none"
                      :class="showPickupDropdown ? 'border-[#062d4d] ring-2 ring-[#062d4d]/20 shadow-sm' : 'border-[#cbd5e1] hover:border-[#94a3b8]'"
                    >
                      <div class="flex items-center gap-2 truncate">
                        <span class="text-sm">🕒</span>
                        <span class="text-[#0f172a] font-bold truncate">{{ bookingForm.pickupTime || availablePickupTimes[0] || 'Select Time' }}</span>
                      </div>
                      <svg class="w-4 h-4 text-[#64748b] shrink-0 transition-transform duration-200" :class="{ 'rotate-180 text-[#062d4d]': showPickupDropdown }" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" d="M19 9l-7 7-7-7" />
                      </svg>
                    </button>

                    <Transition
                      enter-active-class="transition duration-150 ease-out"
                      enter-from-class="transform scale-95 opacity-0 -translate-y-1"
                      enter-to-class="transform scale-100 opacity-100 translate-y-0"
                      leave-active-class="transition duration-100 ease-in"
                      leave-from-class="transform scale-100 opacity-100 translate-y-0"
                      leave-to-class="transform scale-95 opacity-0 -translate-y-1"
                    >
                      <div 
                        v-if="showPickupDropdown"
                        class="absolute left-0 right-0 mt-1.5 bg-white/98 backdrop-blur-xl border border-slate-200/90 rounded-2xl shadow-[0_15px_35px_rgba(6,45,77,0.15)] py-1.5 z-50 max-h-56 overflow-y-auto custom-scrollbar text-left"
                      >
                        <button
                          v-for="(slot, sIdx) in availablePickupTimes"
                          :key="sIdx"
                          type="button"
                          @click="bookingForm.pickupTime = slot; showPickupDropdown = false"
                          class="w-full px-3.5 py-2.5 text-left text-xs sm:text-sm font-semibold flex items-center justify-between hover:bg-[#f0f9ff] transition-colors cursor-pointer group"
                          :class="bookingForm.pickupTime === slot ? 'bg-[#f0f9ff] text-[#0369a1] font-bold' : 'text-slate-700 hover:text-slate-900'"
                        >
                          <div class="flex items-center gap-2.5 truncate">
                            <span class="text-sm text-slate-400 group-hover:text-[#c9a84c] transition-colors">⏱️</span>
                            <span class="truncate">{{ slot }}</span>
                          </div>
                          <svg v-if="bookingForm.pickupTime === slot" class="w-4 h-4 text-[#0284c7] shrink-0" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
                            <polyline points="20 6 9 17 4 12"></polyline>
                          </svg>
                        </button>
                      </div>
                    </Transition>
                  </div>
                </div>

                <!-- 2. Flexible / Free Time Mode -->
                <div v-else-if="pickupTimeType === 'Flexible'">
                  <label class="block text-xs font-bold text-[#334155] mb-1 flex items-center justify-between">
                    <span>{{ $t("bookingPopup.preferredTime") }}</span>
                    <span class="text-[10px] text-emerald-600 font-bold">{{ $t("bookingPopup.freeChoice") }}</span>
                  </label>
                  <input 
                    type="time" 
                    v-model="bookingForm.pickupTime"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-[#cbd5e1] text-xs sm:text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] bg-white"
                  />
                  <p class="text-[11px] text-[#64748b] mt-1">{{ $t("bookingPopup.flexibleNotice") }}</p>
                </div>

                <!-- 3. Driver Assigned Mode -->
                <div v-else class="p-3.5 bg-[#f0f9ff] border border-[#bae6fd] rounded-xl text-xs text-[#0369a1] space-y-1">
                  <div class="font-bold flex items-center gap-1.5">
                    <span>⏱️</span> {{ $t("bookingPopup.conciergeCoordination") }}
                  </div>
                  <p class="text-[11px] text-[#0284c7]">
                    {{ $t("bookingPopup.conciergeNotice") }}
                  </p>
                </div>
              </div>
            </div>

            <!-- 3. Special Requests -->
            <div class="space-y-2.5 bg-white p-5 rounded-2xl border border-[#e2e8f0] shadow-xs">
              <h3 class="text-xs uppercase font-extrabold tracking-wider text-[#062d4d] flex items-center gap-1.5">
                <span>💬</span> {{ $t("bookingPopup.specialRequests") }}
              </h3>
              <div>
                <textarea 
                  v-model="bookingForm.specialRequests" 
                  rows="2" 
                  :placeholder="$t('placeholders.specialRequests')" 
                  class="w-full px-3.5 py-2.5 rounded-xl border border-[#cbd5e1] text-xs sm:text-sm font-medium focus:outline-none focus:border-[#062d4d] focus:ring-1 focus:ring-[#062d4d] resize-none bg-white"
                ></textarea>
              </div>
            </div>

            <!-- 4. Order Summary Card -->
            <div class="bg-gradient-to-br from-[#062d4d] to-[#0a4878] p-5 rounded-2xl text-white shadow-md">
              <h3 class="text-xs uppercase font-extrabold tracking-wider mb-3 text-white/90">{{ $t("bookingPopup.bookingSummary") }}</h3>
              <div class="space-y-2 text-xs sm:text-sm text-white/80">
                <div class="flex justify-between">
                  <span>{{ $t("bookingPopup.travelDate") }}:</span>
                  <span class="font-bold text-white">{{ selectedDate }}</span>
                </div>
                <div class="flex justify-between">
                  <span>{{ $t("bookingPopup.pickupTime") }}:</span>
                  <span class="font-bold text-emerald-300">{{ (bookingForm.pickupTime || '').split(' ')[0] || '15:30' }}</span>
                </div>
                <div class="flex justify-between">
                  <span>{{ $t("bookingPopup.guests") }}:</span>
                  <span class="font-bold text-white">{{ adultsCount }} {{ $t("bookingPopup.adults") }}<span v-if="childrenCount">, {{ childrenCount }} {{ $t("bookingPopup.children") }}</span></span>
                </div>
                <div v-if="selectedAddons.length > 0" class="flex justify-between">
                  <span>{{ $t("bookingPopup.addons") }}:</span>
                  <span class="font-bold text-amber-300">{{ selectedAddons.length }} {{ $t("bookingPopup.selected") }}</span>
                </div>
                <div class="pt-3 mt-3 border-t border-white/20 flex justify-between items-end">
                  <div>
                    <span class="block text-[10px] text-emerald-300 font-bold mb-0.5">{{ $t("bookingPopup.taxesIncluded") }}</span>
                    <span class="text-base sm:text-lg font-bold">{{ $t("bookingPopup.totalToPay") }}:</span>
                  </div>
                  <span class="text-2xl sm:text-3xl font-extrabold text-white">{{ totalPriceFormatted }}</span>
                </div>
              </div>
            </div>

          </div>

          <!-- Footer Actions -->
          <div class="px-6 py-4.5 border-t border-[#f1f5f9] bg-white z-10">
            <button 
              @click="confirmBooking"
              :disabled="bookingSubmitting"
              class="w-full py-4 rounded-xl bg-gradient-to-r from-[#062d4d] to-[#0d4f8b] hover:from-[#0a3f6b] hover:to-[#0f5c9e] text-white font-extrabold text-sm sm:text-base shadow-lg transition-all hover:scale-[1.01] active:scale-[0.99] flex items-center justify-center gap-2 cursor-pointer disabled:opacity-60 disabled:pointer-events-none"
            >
              <span v-if="bookingSubmitting" class="animate-spin text-lg">⏳</span>
              <span>{{ bookingSubmitting ? $t('bookingPopup.confirming') : $t('bookingPopup.confirmReservation') }}</span>
              <span v-if="!bookingSubmitting">→</span>
            </button>
            <p class="text-center text-[10px] text-[#059669] font-bold mt-2 flex items-center justify-center gap-1">
              <span>🔒</span> {{ $t("bookingPopup.sslEncrypted") }}
            </p>
          </div>

        </div>
      </div>
    </Transition>

    <!-- TOAST NOTIFICATION -->
    <Transition name="fade">
      <div 
        v-if="showToast" 
        class="fixed bottom-6 right-6 z-50 bg-[#062d4d] text-white border border-[#c9a84c]/40 px-5 py-3 rounded-xl shadow-2xl text-xs font-bold flex items-center gap-3 animate-in slide-in-from-bottom-3"
      >
        <span class="text-emerald-400">✓</span>
        <span>{{ toastMessage }}</span>
      </div>
    </Transition>

    <!-- FOOTER -->
    <Footer class="mt-16" />

  </div>
</template>

<style scoped>
@keyframes progressBar {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(200%); }
}

.shadow-xs {
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
}

.scrollbar-none::-webkit-scrollbar {
  display: none;
}
.scrollbar-none {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.4s cubic-bezier(0.16, 1, 0.3, 1), transform 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(10px) scale(0.98);
}
</style>
