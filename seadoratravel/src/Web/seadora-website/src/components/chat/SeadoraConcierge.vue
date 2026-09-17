<script setup lang="ts">
import { ref, nextTick, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useCurrencyStore } from '@/store/currency'
import { useWindowSize, useSwipe } from '@vueuse/core'
import { API_BASE_URL } from '@/shared/utils/helpers'
import { localizedPath } from '@/shared/utils/seo'

const router = useRouter()
const { locale } = useI18n()
const currencyStore = useCurrencyStore()
const { width } = useWindowSize()
const isMobile = computed(() => width.value < 640)

// Movable Draggable Bubble State
const bubblePos = ref<{ x: number | null, y: number | null }>({ x: null, y: null })
const isDragging = ref(false)
const hasDragged = ref(false)
const dragStart = { x: 0, y: 0, posX: 0, posY: 0 }

onMounted(() => {
  try {
    const saved = sessionStorage.getItem('seadora_bubble_pos')
    if (saved) {
      const parsed = JSON.parse(saved)
      if (typeof parsed.x === 'number' && typeof parsed.y === 'number') {
        const maxX = window.innerWidth - 68
        const maxY = window.innerHeight - 76
        bubblePos.value = {
          x: Math.max(12, Math.min(maxX, parsed.x)),
          y: Math.max(64, Math.min(maxY, parsed.y))
        }
      }
    }
  } catch (e) {
    console.warn('Bubble pos restore error', e)
  }
})

const onBubblePointerDown = (e: PointerEvent) => {
  const target = e.currentTarget as HTMLElement
  target.setPointerCapture(e.pointerId)
  
  const rect = target.getBoundingClientRect()
  dragStart.x = e.clientX
  dragStart.y = e.clientY
  dragStart.posX = rect.left
  dragStart.posY = rect.top
  hasDragged.value = false
  isDragging.value = true
}

const onBubblePointerMove = (e: PointerEvent) => {
  if (!isDragging.value) return
  const dx = e.clientX - dragStart.x
  const dy = e.clientY - dragStart.y
  
  if (Math.hypot(dx, dy) > 5) {
    hasDragged.value = true
  }
  
  const btnSize = isMobile.value ? 56 : 64
  const minX = 12
  const maxX = window.innerWidth - btnSize - 12
  const minY = 64
  const maxY = window.innerHeight - btnSize - 16
  
  bubblePos.value = {
    x: Math.max(minX, Math.min(maxX, dragStart.posX + dx)),
    y: Math.max(minY, Math.min(maxY, dragStart.posY + dy))
  }
}

const onBubblePointerUp = (e: PointerEvent) => {
  if (!isDragging.value) return
  isDragging.value = false
  try {
    const target = e.currentTarget as HTMLElement
    if (target.hasPointerCapture(e.pointerId)) {
      target.releasePointerCapture(e.pointerId)
    }
  } catch (_) {}

  if (hasDragged.value) {
    // Edge Snapping (iOS Style): snap to nearest screen edge
    const btnSize = isMobile.value ? 56 : 64
    const screenMidX = window.innerWidth / 2
    const snapX = (bubblePos.value.x ?? 0) < screenMidX ? 16 : window.innerWidth - btnSize - 16
    bubblePos.value.x = snapX
    
    // Save to session storage
    sessionStorage.setItem('seadora_bubble_pos', JSON.stringify(bubblePos.value))
  } else {
    // Instant tap / click
    toggleChat()
  }
}

interface ChatMessage {
  role: 'system' | 'user' | 'assistant';
  text: string;
  type?: 'text' | 'tours' | 'calendar' | 'handoff' | 'handoff_success';
  data?: any;
  copied?: boolean;
}

interface QuickAction {
  label: string;
  key: string;
}

const isOpen = ref(false)
const isMinimized = ref(false)
const showNotification = ref(true)
const soundEnabled = ref(true)

const conciergeI18n: Record<string, any> = {
  en: {
    welcome: "Welcome to Seadora Travel! How may I assist you with your luxury Red Sea experience today?",
    title: "Seadora VIP Concierge",
    online: "Online · Ready to Assist",
    bookNow: "Book Now",
    placeholder: "Message your concierge...",
    sendRequest: "Send Request",
    submitting: "Submitting Request...",
    namePlaceholder: "Your Full Name",
    emailPlaceholder: "Your Email Address",
    messagePlaceholder: "How can our concierge team assist you?",
    ticketCreated: "Support Ticket Created!",
    teamContactSoon: "Our concierge team will contact you shortly at",
    explorePrompt: "We offer a handpicked portfolio of 35 luxury excursions in Egypt. What type of adventure are you looking for?",
    humanDesc: "💬 **VIP Concierge Direct Line**\n\nPlease enter your details below. Our senior concierge manager will contact you directly via WhatsApp or email within minutes.",
    availabilityReply: "🗓️ **Availability & Schedules**\n\nAll our excursions run daily with morning and afternoon departures. You can select your preferred date in the real-time availability calendar on any tour page. We recommend booking at least 24 hours in advance.",
    paymentReply: "💳 **Payment & Booking Policies**\n\n• **Zero Deposit / Pay on Pickup**: Reserve your spot now and pay securely upon hotel pickup in cash (EUR/USD/EGP) or online via credit card.\n• **100% Free Cancellation**: Full refund guarantee when cancelling up to 24 hours before departure.\n• **Instant WhatsApp Voucher**: Digital tickets and driver pickup times are delivered straight to your WhatsApp.",
    passportsReply: "🛂 **Passports & Security Permits**\n\nFor Red Sea sea trips and long-distance excursions (Cairo, Luxor), the Egyptian Coast Guard and Tourism Police require passenger manifest approval. Please keep a clear photo of your passport page handy when booking.",
    transfersReply: "🚐 **Hotel Transfers Included**\n\nEvery excursion includes roundtrip door-to-door transfer in a modern, air-conditioned vehicle from all hotels in Hurghada. For resorts in El Gouna, Makadi Bay, Soma Bay, or Safaga, a small zone transfer fee applies.",
    tourSeaReply: "🌊 **Sea & Islands Excursions**\n\nImmerse yourself in crystal-clear turquoise waters! Popular choices: Mahmya Island, Orange Bay, Eden Island, and Seascope Submarine. Guided reef snorkeling, sunbeds, and buffet lunch included.",
    tourSafariReply: "🏜️ **Desert Safari Adventures**\n\nPure adrenaline in the Sahara! Ride Quad ATVs, drive spider buggies, and experience an authentic Bedouin dinner under the stars with oriental folk shows.",
    tourHistoryReply: "🏛️ **Historical Day Trips (Luxor & Cairo)**\n\nWalk among pharaohs with our licensed Egyptologist guides: Karnak & Valley of the Kings in Luxor, or the Great Pyramids & Sphinx in Cairo. Available as day trips or flights.",
    tourDivingReply: "🤿 **Diving & Marine Encounters**\n\nSwim with wild dolphins in their natural habitat, dive world-class coral gardens, or visit Abu Dabbab turtle bay with certified dive masters.",
    menu: {
      explore: "🏝️ Explore Tours",
      availability: "📅 Availability & Dates",
      payment: "💳 Payment & Booking",
      passports: "🛂 Passports & Permits",
      transfers: "🚐 Hotel Transfers",
      human: "💬 Talk to Human Concierge",
      tour_sea: "🌊 Sea & Islands",
      tour_safari: "🏜️ Desert Safari",
      tour_history: "🏛️ Historical Excursions",
      tour_diving: "🤿 Diving & Marine",
      main: "⬅️ Back to Menu"
    }
  },
  de: {
    welcome: "Willkommen bei Seadora Travel! Wie kann ich Ihnen heute bei Ihrem Luxus-Erlebnis am Roten Meer helfen?",
    title: "Seadora VIP-Concierge",
    online: "Online · Jederzeit bereit",
    bookNow: "Jetzt buchen",
    placeholder: "Nachricht an den Concierge...",
    sendRequest: "Anfrage senden",
    submitting: "Wird gesendet...",
    namePlaceholder: "Ihr vollständiger Name",
    emailPlaceholder: "Ihre E-Mail-Adresse",
    messagePlaceholder: "Wie kann unser Concierge-Team Ihnen helfen?",
    ticketCreated: "Support-Ticket erstellt!",
    teamContactSoon: "Unser Concierge-Team wird Sie in Kürze kontaktieren unter",
    explorePrompt: "Wir bieten ein exklusives Portfolio von 35 Luxus-Ausflügen in Ägypten. Welche Art von Erlebnis suchen Sie?",
    humanDesc: "💬 **VIP-Concierge Direktservice**\n\nBitte hinterlassen Sie Ihre Kontaktdaten unten. Unser persönlicher Concierge-Koordinator wird Sie innerhalb weniger Minuten per WhatsApp oder E-Mail kontaktieren.",
    availabilityReply: "🗓️ **Verfügbarkeit & Termine**\n\nAlle unsere Ausflüge finden täglich statt. Sie können Ihr Wunschdatum direkt im Live-Kalender auf der jeweiligen Tourenseite auswählen. Wir empfehlen eine Buchung mindestens 24 Stunden im Voraus.",
    paymentReply: "💳 **Zahlung & Buchungsbedingungen**\n\n• **Ohne Vorauszahlung / Vor Ort zahlen**: Jetzt reservieren und bequem am Tag der Abholung in bar (EUR/USD/EGP) oder online per Karte zahlen.\n• **Kostenlose Stornierung**: Bis zu 24 Stunden vor Abfahrt kostenlos stornierbar mit voller Rückerstattung.\n• **Sofort-Gutschein per WhatsApp**: Sie erhalten Ihre digitale Bestätigung und Abholzeit sofort auf Ihr Smartphone.",
    passportsReply: "🛂 **Pässe & Genehmigungen**\n\nFür Bootstouren auf dem Roten Meer sowie Ausflüge nach Kairo und Luxor verlangen die ägyptische Küstenwache und Touristenpolizei Passkontrollen. Bitte halten Sie bei der Buchung ein Foto Ihrer Passseite bereit.",
    transfersReply: "🚐 **Hoteltransfers inklusive**\n\nAlle Touren beinhalten den klimatisierten Hin- und Rücktransfer direkt von Ihrem Hotel in Hurghada. Für Resorts in El Gouna, Makadi Bay, Soma Bay oder Safaga fällt lediglich ein kleiner Zonenzuschlag an.",
    tourSeaReply: "🌊 **Meer & Inseln**\n\nErleben Sie paradiesische Sandstrände und Korallenriffe: Insel Mahmya, Orange Bay, Eden Island und das Seascope U-Boot. Geführtes Schnorcheln, Sonnenliegen und Mittagsbuffet inklusive.",
    tourSafariReply: "🏜️ **Wüstensafari-Abenteuer**\n\nAdrenalin pur in der Sahara: Quad-Bikes (ATV), Spider Buggies und Super Safari mit Beduinen-Abendessen, Sternenbeobachtung und Showprogramm.",
    tourHistoryReply: "🏛️ **Kultur- & Geschichtsausflüge**\n\nBegeben Sie sich auf die Spuren der Pharaonen mit lizenzierten Ägyptologen: Karnak & Tal der Könige in Luxor oder die Pyramiden von Gizeh & Kairo.",
    tourDivingReply: "🤿 **Tauchen & Meeresabenteuer**\n\nSchwimmen Sie mit Delfinen im offenen Meer, tauchen Sie an weltberühmten Riffen oder entdecken Sie Riesenschildkröten in der Bucht Abu Dabbab.",
    menu: {
      explore: "🏝️ Touren entdecken",
      availability: "📅 Verfügbarkeit & Termine",
      payment: "💳 Zahlung & Buchung",
      passports: "🛂 Pässe & Genehmigungen",
      transfers: "🚐 Hoteltransfers",
      human: "💬 Mit Berater sprechen",
      tour_sea: "🌊 Meer & Inseln",
      tour_safari: "🏜️ Wüstensafari",
      tour_history: "🏛️ Historische Touren",
      tour_diving: "🤿 Tauchen & Schnorcheln",
      main: "⬅️ Zurück zum Menü"
    }
  },
  fr: {
    welcome: "Bienvenue chez Seadora Travel ! Comment puis-je vous aider aujourd'hui pour votre expérience de luxe en mer Rouge ?",
    title: "Concierge VIP Seadora",
    online: "En ligne · À votre service",
    bookNow: "Réserver",
    placeholder: "Message au concierge...",
    sendRequest: "Envoyer la demande",
    submitting: "Envoi en cours...",
    namePlaceholder: "Votre nom complet",
    emailPlaceholder: "Votre adresse e-mail",
    messagePlaceholder: "Comment notre équipe peut-elle vous aider ?",
    ticketCreated: "Ticket d'assistance créé !",
    teamContactSoon: "Notre équipe de conciergerie vous contactera sous peu à",
    explorePrompt: "Nous proposons une sélection prestigieuse de 35 excursions en Égypte. Quel genre d'aventure recherchez-vous ?",
    humanDesc: "💬 **Ligne directe Concierge VIP**\n\nVeuillez renseigner vos coordonnées ci-dessous. Notre responsable de conciergerie vous contactera directement via WhatsApp ou e-mail en quelques minutes.",
    availabilityReply: "🗓️ **Disponibilités et horaires**\n\nToutes nos excursions partent tous les jours. Vous pouvez sélectionner votre date directement sur le calendrier interactif de chaque page excursion. Réservation conseillée au moins 24h à l'avance.",
    paymentReply: "💳 **Paiement et conditions de réservation**\n\n• **Sans avance / Paiement sur place** : Réservez votre place sans acompte et payez le jour de l'excursion en espèces (EUR/USD/EGP) ou en ligne par carte.\n• **Annulation 100% gratuite** : Remboursement intégral garanti pour toute annulation jusqu'à 24h avant le départ.\n• **Bon de confirmation immédiat** : Reçu numérique et heure exacte de prise en charge envoyés sur WhatsApp.",
    passportsReply: "🛂 **Passeports et autorisations de sécurité**\n\nPour les excursions en bateau et les trajets lointains (Le Caire, Louxor), les garde-côtes égyptiens et la police touristique exigent une vérification d'identité. Prévoyez une photo claire de votre passeport.",
    transfersReply: "🚐 **Transferts hôtel inclus**\n\nToutes nos prestations incluent l'aller-retour climatisé depuis votre hôtel à Hurghada. Pour les complexes à El Gouna, Makadi Bay ou Safaga, un petit supplément de zone s'applique.",
    tourSeaReply: "🌊 **Mer Rouge & Îles paradisiaques**\n\nBaignades dans des eaux cristallines : Île de Mahmya, Orange Bay, Eden Island et bateau panoramique Seascope. Snorkeling guidé, transats et buffet inclus.",
    tourSafariReply: "🏜️ **Safaris dans le Désert**\n\nSensations fortes au Sahara : Quads tout-terrain (ATV), buggies et Super Safari avec dîner bédouin traditionnel et spectacle oriental sous les étoiles.",
    tourHistoryReply: "🏛️ **Excursions Historiques (Louxor & Le Caire)**\n\nRemontez le temps avec nos guides égyptologues certifiés : Vallée des Rois et Karnak à Louxor, ou Grandes Pyramides de Gizeh au Caire.",
    tourDivingReply: "🤿 **Plongée et faune marine**\n\nNagez avec les dauphins en liberté, explorez des récifs coralliens spectaculaires ou découvrez les tortues géantes de la baie d'Abu Dabbab.",
    menu: {
      explore: "🏝️ Explorer les excursions",
      availability: "📅 Disponibilités et dates",
      payment: "💳 Paiement et réservation",
      passports: "🛂 Passeports et permis",
      transfers: "🚐 Transferts hôtel",
      human: "💬 Parler à un conseiller",
      tour_sea: "🌊 Mer & Îles",
      tour_safari: "🏜️ Safari dans le désert",
      tour_history: "🏛️ Excursions historiques",
      tour_diving: "🤿 Plongée & Snorkeling",
      main: "⬅️ Retour au menu"
    }
  },
  it: {
    welcome: "Benvenuti a Seadora Travel! Come posso aiutarvi oggi con la vostra esperienza di lusso sul Mar Rosso?",
    title: "Concierge VIP Seadora",
    online: "Online · Al vostro servizio",
    bookNow: "Prenota ora",
    placeholder: "Scrivi al concierge...",
    sendRequest: "Invia richiesta",
    submitting: "Invio in corso...",
    namePlaceholder: "Nome e cognome",
    emailPlaceholder: "Indirizzo e-mail",
    messagePlaceholder: "Come può aiutarla il nostro team?",
    ticketCreated: "Ticket di assistenza creato!",
    teamContactSoon: "Il nostro team di concierge la ricontatterà a breve su",
    explorePrompt: "Offriamo una collezione di 35 escursioni esclusive in Egitto. Che tipo di avventura desidera vivere?",
    humanDesc: "💬 **Linea Diretta Concierge VIP**\n\nInserisca i suoi recapiti qui sotto: il nostro concierge manager la contatterà direttamente via WhatsApp o e-mail entro pochi minuti.",
    availabilityReply: "🗓️ **Disponibilità e orari**\n\nTutti i nostri tour partono quotidianamente. Può selezionare la data preferita dal calendario in tempo reale nella pagina di ciascun tour. Consigliamo di prenotare con almeno 24 ore di anticipo.",
    paymentReply: "💳 **Pagamento e condizioni di prenotazione**\n\n• **Nessun anticipo / Paga al ritiro**: Prenoti ora e paghi comodamente alla partenza in contanti (EUR/USD/EGP) o online con carta di credito.\n• **Cancellazione gratuita al 100%**: Rimborso completo fino a 24 ore prima dell'escursione.\n• **Voucher istantaneo su WhatsApp**: Riceverà biglietto digitale e orario di pick-up immediatamente sul suo telefono.",
    passportsReply: "🛂 **Passaporti e permessi marittimi**\n\nPer le uscite in barca e i tour a lungo raggio (Cairo, Luxor), la Guardia Costiera e la Polizia Turistica richiedono i dati del passaporto per i permessi di sicurezza. Tenga pronta una foto del documento.",
    transfersReply: "🚐 **Transfer dall'hotel inclusi**\n\nTutte le escursioni includono il transfer di andata e ritorno con veicoli moderni e climatizzati da tutti gli hotel di Hurghada. Per strutture a El Gouna, Makadi Bay o Safaga è previsto un piccolo supplemento.",
    tourSeaReply: "🌊 **Mare e Isole del Mar Rosso**\n\nLagune turchesi e barriere coralline incontaminate: Isola di Mahmya, Orange Bay, Eden Island e sottomarino Seascope. Snorkeling guidato, lettini e pranzo inclusi.",
    tourSafariReply: "🏜️ **Safari nel Deserto**\n\nAdrenalina nel Sahara: Quad ATV, Spider Buggy e Super Safari con cena tipica beduina, osservazione del cielo stellato e spettacoli orientali.",
    tourHistoryReply: "🏛️ **Escursioni Storiche (Luxor e Il Cairo)**\n\nUn viaggio nell'Antico Egitto con le nostre guide egittologhe ufficiali: Valle dei Re e Karnak a Luxor, o le Piramidi di Giza e il Museo al Cairo.",
    tourDivingReply: "🤿 **Immersioni e Snorkeling**\n\nNuotate con i delfini nel loro habitat naturale, immergetevi in giardini di corallo unici al mondo o visitate la baia delle tartarughe di Abu Dabbab.",
    menu: {
      explore: "🏝️ Esplora i tour",
      availability: "📅 Disponibilità e date",
      payment: "💳 Pagamento e prenotazione",
      passports: "🛂 Passaporti e permessi",
      transfers: "🚐 Transfer hotel",
      human: "💬 Parla con il Concierge",
      tour_sea: "🌊 Mare e Isole",
      tour_safari: "🏜️ Safari nel deserto",
      tour_history: "🏛️ Escursioni storiche",
      tour_diving: "🤿 Immersioni e snorkeling",
      main: "⬅️ Torna al menu"
    }
  },
  ru: {
    welcome: "Добро пожаловать в Seadora Travel! Чем я могу помочь вам сегодня с вашим отдыхом на Красном море?",
    title: "VIP-Консьерж Seadora",
    online: "Онлайн · Готов помочь",
    bookNow: "Забронировать",
    placeholder: "Напишите консьержу...",
    sendRequest: "Отправить запрос",
    submitting: "Отправка...",
    namePlaceholder: "Ваше имя и фамилия",
    emailPlaceholder: "Ваш адрес эл. почты",
    messagePlaceholder: "Какой вопрос вас интересует?",
    ticketCreated: "Заявка успешно создана!",
    teamContactSoon: "Наш персональный координатор свяжется с вами в ближайшее время по адресу",
    explorePrompt: "У нас представлено 35 проверенных экскурсий в Египте. Какой формат отдыха вы ищете?",
    humanDesc: "💬 **Прямая связь с VIP-консьержем**\n\nОставьте свои контакты ниже, и наш менеджер оперативно свяжется с вами в WhatsApp или по почте для консультации.",
    availabilityReply: "🗓️ **Расписание и даты выездов**\n\nВсе наши экскурсии проводятся ежедневно с утренними и дневными выездами. Вы можете выбрать дату в живом календаре на странице тура. Рекомендуем бронировать минимум за 24 часа.",
    paymentReply: "💳 **Оплата и правила бронирования**\n\n• **Без предоплаты / Оплата при посадке**: Бронируйте сейчас и оплачивайте наличными гиду при выезде (EUR/USD/EGP/RUB) или онлайн картой.\n• **100% бесплатная отмена**: Бесплатная отмена за 24 часа до поездки с полным возвратом денег.\n• **Электронный ваучер в WhatsApp**: Билеты и точное время трансфера из отеля приходят вам прямо в мессенджер.",
    passportsReply: "🛂 **Паспорта и разрешения береговой охраны**\n\nДля морских прогулок и дальних поездок (Каир, Луксор) туристическая полиция требует регистрацию списков пассажиров. При бронировании потребуется четкое фото главного разворота паспорта.",
    transfersReply: "🚐 **Трансфер из отеля включен**\n\nВсе экскурсии включают трансфер на комфортабельном кондиционированном микроавтобусе от дверей вашего отеля в Хургаде и обратно. Для отелей в районах Эль-Гуна, Макади-Бэй или Сафага действует небольшая доплата за зону.",
    tourSeaReply: "🌊 **Морские прогулки и райские острова**\n\nБирюзовые лагуны и коралловые рифы: Остров Махмея, Оранж Бэй, Остров Эдем и панорамный батискаф Seascope. Снорклинг с гидом, шезлонги и обед 'шведский стол' включены.",
    tourSafariReply: "🏜️ **Сафари в пустыне на квадроциклах**\n\nНастоящие приключения в Сахаре: катание на квадроциклах (ATV), скоростных багги, закат в пустыне, ужин в бедуинской деревне и восточное шоу.",
    tourHistoryReply: "🏛️ **Исторические экскурсии (Луксор и Каир)**\n\nВеличие древней цивилизации с сертифицированными гидами-египтологами: Карнакский храм и Долина Царей в Луксоре или Пирамиды Гизы и музей в Каире. Доступны на автобусе и самолете.",
    tourDivingReply: "🤿 **Дайвинг и морские обитатели**\n\nПлавание с дельфинами в открытом море, погружения у лучших рифов Красного моря с русскоязычными инструкторами и бухта гигантских черепах Абу-Дабаб.",
    menu: {
      explore: "🏝️ Выбрать экскурсию",
      availability: "📅 Расписание и даты",
      payment: "💳 Оплата и бронирование",
      passports: "🛂 Паспорта и разрешения",
      transfers: "🚐 Трансфер из отеля",
      human: "💬 Связаться с оператором",
      tour_sea: "🌊 Острова и море",
      tour_safari: "🏜️ Сафари в пустыне",
      tour_history: "🏛️ Каир и Луксор",
      tour_diving: "🤿 Дайвинг и снорклинг",
      main: "⬅️ Назад в меню"
    }
  }
}

const currentI18n = computed(() => {
  return conciergeI18n[locale.value] || conciergeI18n['en']
})

const handoffForm = ref({ name: '', email: '', message: '' })
const isSubmittingHandoff = ref(false)

const submitHandoff = async () => {
  if (!handoffForm.value.name || !handoffForm.value.email) return;
  isSubmittingHandoff.value = true;
  try {
    const API_URL = API_BASE_URL;
    const res = await fetch(`${API_URL}/api/concierge/api/handoff`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...handoffForm.value, language: locale.value })
    });
    if (res.ok) {
      const data = await res.json();
      const ticketId = data.ticketId || Math.floor(Math.random() * 10000);
      messages.value.push({
        role: 'assistant',
        text: `✅ ${currentI18n.value.ticketCreated} (#${ticketId})\n\n${currentI18n.value.teamContactSoon} ${handoffForm.value.email}.`,
        type: 'handoff_success'
      });
      handoffForm.value = { name: '', email: '', message: '' };
      currentMenuState.value = 'main';
    }
  } catch (err) {
    console.error(err);
  } finally {
    isSubmittingHandoff.value = false;
    scrollToBottom();
  }
}

const messages = ref<ChatMessage[]>([
  { role: 'assistant', text: currentI18n.value.welcome }
])

// React immediately to language switcher changes in the website header
watch(locale, () => {
  if (messages.value.length === 1 && messages.value[0].role === 'assistant') {
    messages.value[0].text = currentI18n.value.welcome
  }
}, { immediate: true })

const inputQuery = ref('')
const isTyping = ref(false)
const chatContainer = ref<HTMLElement | null>(null)
const chatSheet = ref<HTMLElement | null>(null)

// Mobile Swipe to Dismiss
const { lengthY } = useSwipe(chatSheet, {
  onSwipeEnd(_e, dir) {
    if (dir === 'down' && lengthY.value < -100) {
      closeChat()
    }
  }
})

const toggleChat = () => {
  if (isOpen.value) {
    closeChat()
  } else {
    openChat()
  }
}

const openChat = () => {
  isOpen.value = true
  isMinimized.value = false
  showNotification.value = false
  scrollToBottom()
  if (isMobile.value) document.body.style.overflow = 'hidden'
}

const closeChat = () => {
  isOpen.value = false
  isMinimized.value = false
  if (isMobile.value) document.body.style.overflow = ''
}

const toggleMinimize = () => {
  isMinimized.value = !isMinimized.value
}

const clearChat = () => {
  messages.value = [{ role: 'assistant', text: currentI18n.value.welcome }]
  currentMenuState.value = 'main'
}

const scrollToBottom = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}

const currentMenuState = ref('main')

const currentOptions = computed<QuickAction[]>(() => {
  const m = currentI18n.value.menu
  if (currentMenuState.value === 'main') {
    return [
      { label: m.explore, key: 'explore' },
      { label: m.availability, key: 'availability' },
      { label: m.payment, key: 'payment' },
      { label: m.passports, key: 'passports' },
      { label: m.transfers, key: 'transfers' },
      { label: m.human, key: 'human' }
    ]
  } else if (currentMenuState.value === 'explore') {
    return [
      { label: m.tour_sea, key: 'tour_sea' },
      { label: m.tour_safari, key: 'tour_safari' },
      { label: m.tour_history, key: 'tour_history' },
      { label: m.tour_diving, key: 'tour_diving' },
      { label: m.main, key: 'main' },
      { label: m.human, key: 'human' }
    ]
  } else {
    return [
      { label: m.main, key: 'main' },
      { label: m.human, key: 'human' }
    ]
  }
})

const handleMenuClick = async (option: QuickAction) => {
  messages.value.push({ role: 'user', text: option.label })
  scrollToBottom()
  
  if (option.key === 'main') {
    currentMenuState.value = 'main'
    messages.value.push({ role: 'assistant', text: currentI18n.value.welcome })
    scrollToBottom()
    return
  }

  if (option.key === 'explore') {
    currentMenuState.value = 'explore'
    messages.value.push({ role: 'assistant', text: currentI18n.value.explorePrompt })
    scrollToBottom()
    return
  }

  if (option.key === 'human') {
    currentMenuState.value = 'chat'
    messages.value.push({ role: 'assistant', text: currentI18n.value.humanDesc, type: 'handoff' })
    scrollToBottom()
    return
  }

  currentMenuState.value = 'info'
  isTyping.value = true
  scrollToBottom()

  setTimeout(async () => {
    isTyping.value = false
    let replyText = ''
    let queryForTours = ''

    switch (option.key) {
      case 'availability':
        replyText = currentI18n.value.availabilityReply
        break;
      case 'payment':
        replyText = currentI18n.value.paymentReply
        break;
      case 'passports':
        replyText = currentI18n.value.passportsReply
        break;
      case 'transfers':
        replyText = currentI18n.value.transfersReply
        break;
      case 'tour_sea':
        replyText = currentI18n.value.tourSeaReply
        queryForTours = 'sea'
        break;
      case 'tour_safari':
        replyText = currentI18n.value.tourSafariReply
        queryForTours = 'safari'
        break;
      case 'tour_history':
        replyText = currentI18n.value.tourHistoryReply
        queryForTours = 'luxor'
        break;
      case 'tour_diving':
        replyText = currentI18n.value.tourDivingReply
        queryForTours = 'diving'
        break;
    }

    if (queryForTours) {
      const tours = await fetchTours(queryForTours)
      messages.value.push({ role: 'assistant', text: replyText, type: 'tours', data: tours.slice(0, 4) })
    } else {
      messages.value.push({ role: 'assistant', text: replyText })
    }
    scrollToBottom()
  }, 500)
}

const formatPrice = (eur: number | undefined) => {
  const val = eur ?? 0;
  if (currencyStore.selectedCurrency === 'USD') return `$${(val * 1.08).toFixed(2)}`
  if (currencyStore.selectedCurrency === 'EGP') return `EGP ${(val * 50).toFixed(0)}`
  return `€${val.toFixed(2)}`
}

const fetchTours = async (query: string) => {
  try {
    const API_URL = API_BASE_URL;
    const res = await fetch(`${API_URL}/api/content/api/tours?search=${encodeURIComponent(query)}&lang=${locale.value}`)
    if (res.ok) {
      const data = await res.json()
      const list = Array.isArray(data) ? data : (data.items || [])
      return list
    }
    return []
  } catch (err) {
    console.error('Failed to fetch tours', err)
    return []
  }
}

const handleSend = async (text: string) => {
  if (!text.trim()) return
  
  if (isMinimized.value) isMinimized.value = false
  if (currentMenuState.value !== 'chat') {
    currentMenuState.value = 'chat'
  }
  
  messages.value.push({ role: 'user', text })
  inputQuery.value = ''
  scrollToBottom()
  
  isTyping.value = true
  scrollToBottom()
  
  try {
    const API_URL = API_BASE_URL;
    const res = await fetch(`${API_URL}/api/concierge/api/chat`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message: text, language: locale.value })
    })

    if (res.ok) {
      const data = await res.json()
      const replyContent = data.content || data.replyText
      
      const q = text.toLowerCase()
      const isTourQuery = data.intent === 'TourSearch' || data.intent === 1 ||
        q.includes('recommend') || q.includes('tour') || q.includes('trip') || q.includes('best') ||
        q.includes('ausflug') || q.includes('touren') || q.includes('excursion') || q.includes('visite') ||
        q.includes('тур') || q.includes('экскурси')

      if ((data.suggestedTours && data.suggestedTours.length > 0) || isTourQuery) {
        let toursToShow = data.suggestedTours
        if (!toursToShow || toursToShow.length === 0) {
          toursToShow = await fetchTours(text)
        }
        messages.value.push({
          role: 'assistant',
          text: replyContent || currentI18n.value.explorePrompt,
          type: 'tours',
          data: (toursToShow || []).slice(0, 4)
        })
      } else {
        messages.value.push({
          role: 'assistant',
          text: replyContent || currentI18n.value.welcome
        })
      }
    } else {
      messages.value.push({
        role: 'assistant',
        text: currentI18n.value.welcome
      })
    }
  } catch (err) {
    console.error('Chat processing error:', err)
    messages.value.push({
      role: 'assistant',
      text: currentI18n.value.welcome
    })
  } finally {
    isTyping.value = false
    scrollToBottom()
  }
}

const viewTour = (slug: string) => {
  closeChat()
  router.push(localizedPath('/tour/' + slug, locale.value))
}

const copyToClipboard = async (msg: any) => {
  try {
    await navigator.clipboard.writeText(msg.text)
    msg.copied = true
    setTimeout(() => { msg.copied = false }, 2000)
  } catch (err) {
    console.error('Failed to copy', err)
  }
}

const readAloud = (text: string) => {
  if (!soundEnabled.value) return
  if ('speechSynthesis' in window) {
    window.speechSynthesis.cancel();
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = locale.value === 'en' ? 'en-US' : locale.value;
    window.speechSynthesis.speak(utterance);
  }
}

watch(isMobile, (newVal) => {
  if (isOpen.value && !newVal) {
    document.body.style.overflow = ''
  } else if (isOpen.value && newVal) {
    document.body.style.overflow = 'hidden'
  }
})
</script>

<template>
  <!-- Mobile Backdrop Overlay -->
  <div 
    v-if="isMobile && isOpen" 
    v-motion
    :initial="{ opacity: 0 }"
    :enter="{ opacity: 1, transition: { duration: 250 } }"
    :leave="{ opacity: 0, transition: { duration: 200 } }"
    class="fixed inset-0 bg-black/60 backdrop-blur-sm z-[9990] pointer-events-auto"
    @click="closeChat"
  ></div>

  <!-- Chat Window (Desktop Floating Modal / Mobile Bottom Sheet Drawer) -->
  <div 
    v-show="isOpen" 
    v-motion
    :initial="isMobile ? { y: '100%' } : { opacity: 0, y: 30, scale: 0.95 }"
    :enter="isMobile ? { y: 0, transition: { type: 'spring', stiffness: 350, damping: 30 } } : { opacity: 1, y: 0, scale: 1, transition: { type: 'spring', stiffness: 350, damping: 25 } }"
    :leave="isMobile ? { y: '100%', transition: { type: 'spring', stiffness: 350, damping: 30 } } : { opacity: 0, y: 30, scale: 0.95, transition: { type: 'spring', stiffness: 350, damping: 25 } }"
    ref="chatSheet"
    class="bg-white shadow-[0_20px_50px_rgba(0,0,0,0.3)] flex flex-col overflow-hidden origin-bottom-right z-[9995] transition-all duration-300 font-sans"
    :class="[
      isMobile 
        ? 'fixed bottom-0 left-0 right-0 rounded-t-[28px] border-t border-[#c9a84c]/40 h-[88dvh] max-h-[88dvh] w-full' 
        : 'fixed bottom-24 right-6 rounded-2xl border border-[#e2e8f0]',
      !isMobile && isMinimized ? 'h-16 w-[420px]' : !isMobile ? 'w-[420px] h-[620px]' : ''
    ]"
  >
    <!-- Mobile Drag Handle Bar -->
    <div 
      v-if="isMobile" 
      class="w-full flex justify-center pt-3 pb-1 bg-[#062d4d] cursor-pointer touch-none" 
      @click="closeChat"
    >
      <div class="w-12 h-1.5 bg-white/30 rounded-full"></div>
    </div>
    
    <!-- Header -->
    <div 
      class="bg-[#062d4d] text-white p-4 flex items-center justify-between z-10 relative shrink-0 border-b border-white/10" 
      :class="isMobile ? 'pt-2' : ''"
    >
      <div class="flex items-center gap-3 cursor-pointer select-none" @click="!isMobile && toggleMinimize()">
        <div class="w-10 h-10 rounded-full bg-gradient-to-br from-[#c9a84c] to-[#a38030] flex items-center justify-center font-serif font-bold text-xl text-white shadow-[0_0_15px_rgba(201,168,76,0.5)]">
          S
        </div>
        <div>
          <h3 class="font-bold text-base tracking-wide">{{ currentI18n.title }}</h3>
          <p class="text-xs text-[#cbd5e1] flex items-center gap-1.5 font-medium">
            <span class="w-2 h-2 rounded-full bg-emerald-400 animate-pulse shadow-[0_0_5px_#34d399]"></span>
            {{ currentI18n.online }}
          </p>
        </div>
      </div>
      
      <div class="flex items-center gap-1.5">
        <button 
          v-if="!isMobile" 
          @click="soundEnabled = !soundEnabled" 
          class="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded-full transition-all" 
          title="Toggle Sound"
        >
          <svg v-if="soundEnabled" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.536 8.464a5 5 0 010 7.072m2.828-9.9a9 9 0 010 12.728M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z"></path></svg>
          <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z M17 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2"></path></svg>
        </button>
        <button 
          v-if="!isMobile" 
          @click="clearChat" 
          class="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded-full transition-all" 
          title="Clear Chat"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
        </button>
        <button 
          v-if="!isMobile" 
          @click="toggleMinimize" 
          class="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded-full transition-all"
        >
          <svg v-if="isMinimized" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8V4m0 0h4M4 4l5 5m11-1V4m0 0h-4m4 0l-5 5M4 16v4m0 0h4m-4 0l5-5m11 5l-5-5m5 5v-4m0 4h-4"></path></svg>
          <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4"></path></svg>
        </button>
        <button 
          @click="closeChat" 
          class="p-2 text-white/70 hover:text-white hover:bg-white/10 rounded-full transition-all active:scale-95"
          aria-label="Close Chat"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
        </button>
      </div>
    </div>
    
    <div v-show="!isMinimized" class="flex flex-col flex-1 overflow-hidden">
      <!-- Messages Container -->
      <div class="flex-1 overflow-y-auto p-4 bg-[#f8fafc] space-y-4" ref="chatContainer">
        <div 
          v-for="(msg, idx) in messages" 
          :key="idx" 
          v-motion 
          :initial="{ opacity: 0, y: 12, scale: 0.98 }" 
          :enter="{ opacity: 1, y: 0, scale: 1, transition: { type: 'spring', stiffness: 400, damping: 30 } }"
          :class="['flex', msg.role === 'user' ? 'justify-end' : 'justify-start group/msg']"
        >
          <div 
            :class="[
              'max-w-[88%] md:max-w-[85%] rounded-2xl p-3.5 text-[14px] md:text-[15px] shadow-sm relative leading-relaxed', 
              msg.role === 'user' 
                ? 'bg-[#062d4d] text-white rounded-br-sm' 
                : 'bg-white border border-[#e2e8f0] text-[#1e293b] rounded-bl-sm'
            ]"
          >
            <p class="whitespace-pre-wrap">{{ msg.text }}</p>
            
            <!-- Actions for Assistant Messages -->
            <div v-if="msg.role === 'assistant'" class="absolute -right-11 top-1 opacity-0 group-hover/msg:opacity-100 transition-opacity duration-200 flex flex-col gap-1 hidden md:flex">
              <button @click="copyToClipboard(msg)" class="p-1.5 bg-white text-gray-500 rounded-full shadow-sm hover:text-[#0284c7] hover:bg-gray-50 transition-colors border border-gray-100" title="Copy">
                <svg v-if="!msg.copied" class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"></path></svg>
                <svg v-else class="w-3.5 h-3.5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path></svg>
              </button>
              <button v-if="soundEnabled" @click="readAloud(msg.text)" class="p-1.5 bg-white text-gray-500 rounded-full shadow-sm hover:text-[#0284c7] hover:bg-gray-50 transition-colors border border-gray-100" title="Read Aloud">
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.536 8.464a5 5 0 010 7.072m2.828-9.9a9 9 0 010 12.728M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z"></path></svg>
              </button>
            </div>
            
            <!-- Interactive Tour Cards Carousel -->
            <div v-if="msg.type === 'tours' && msg.data && msg.data.length" class="mt-3 flex gap-3 overflow-x-auto snap-x snap-mandatory hide-scrollbar pb-2 overscroll-x-contain" style="-webkit-overflow-scrolling: touch; scroll-behavior: smooth;">
              <div v-for="tour in msg.data" :key="tour.slug" class="min-w-[175px] max-w-[175px] snap-center bg-white border border-[#e2e8f0] rounded-xl overflow-hidden cursor-pointer group hover:border-[#c9a84c] hover:shadow-lg transition-all duration-300 relative active:scale-95" @click="viewTour(tour.slug)">
                <div class="overflow-hidden h-24">
                  <img v-if="tour.mainImage" :src="tour.mainImage" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110" alt="Tour" />
                </div>
                <div class="p-2.5 relative">
                  <h4 class="font-bold text-xs text-[#0f172a] line-clamp-2 group-hover:text-[#c9a84c] transition-colors leading-tight h-8">{{ tour.names?.[locale] || tour.title }}</h4>
                  <div class="flex items-center gap-1 mt-1 text-[10px] text-gray-500 font-medium">
                    <span class="text-[#c9a84c]">★ 4.9</span>
                    <span>•</span>
                    <span>{{ tour.duration || '4 Hours' }}</span>
                  </div>
                  <div class="flex flex-col mt-1.5 gap-1.5">
                    <span class="font-black text-[14px] text-[#062d4d]">{{ formatPrice(tour.priceEur ?? tour.price) }}</span>
                    <button class="w-full text-[11px] bg-gradient-to-r from-[#c9a84c] to-[#e1c675] text-white py-1.5 rounded-lg font-bold shadow-xs hover:shadow-md transition-all">
                      {{ currentI18n.bookNow }}
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Handoff Form -->
            <div v-if="msg.type === 'handoff'" class="mt-3 flex flex-col gap-2.5">
              <input v-model="handoffForm.name" type="text" :placeholder="currentI18n.namePlaceholder" class="w-full bg-[#f8fafc] border border-[#e2e8f0] rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c]/20 transition-all placeholder:text-[#94a3b8]" />
              <input v-model="handoffForm.email" type="email" :placeholder="currentI18n.emailPlaceholder" class="w-full bg-[#f8fafc] border border-[#e2e8f0] rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c]/20 transition-all placeholder:text-[#94a3b8]" />
              <textarea v-model="handoffForm.message" :placeholder="currentI18n.messagePlaceholder" rows="2" class="w-full bg-[#f8fafc] border border-[#e2e8f0] rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-[#c9a84c] focus:ring-1 focus:ring-[#c9a84c]/20 transition-all placeholder:text-[#94a3b8] resize-none"></textarea>
              <button @click="submitHandoff" :disabled="isSubmittingHandoff || !handoffForm.name || !handoffForm.email" class="w-full text-sm bg-gradient-to-r from-[#062d4d] to-[#0f172a] text-white py-2 rounded-lg font-bold shadow-md hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed active:scale-98">
                {{ isSubmittingHandoff ? currentI18n.submitting : currentI18n.sendRequest }}
              </button>
            </div>
          </div>
        </div>
        
        <!-- Typing Indicator -->
        <div v-if="isTyping" class="flex justify-start">
          <div v-motion :initial="{ opacity: 0, y: 10, scale: 0.9 }" :enter="{ opacity: 1, y: 0, scale: 1, transition: { type: 'spring', stiffness: 400, damping: 25 } }" class="bg-white border border-[#e2e8f0] rounded-2xl rounded-bl-sm px-4 py-3 text-[#94a3b8] flex gap-1.5 items-center shadow-sm">
            <span class="w-2 h-2 bg-[#c9a84c] rounded-full animate-[wave_1.2s_ease-in-out_infinite] shadow-[0_0_4px_#c9a84c]"></span>
            <span class="w-2 h-2 bg-[#c9a84c] rounded-full animate-[wave_1.2s_ease-in-out_infinite] [animation-delay:0.2s] shadow-[0_0_4px_#c9a84c]"></span>
            <span class="w-2 h-2 bg-[#c9a84c] rounded-full animate-[wave_1.2s_ease-in-out_infinite] [animation-delay:0.4s] shadow-[0_0_4px_#c9a84c]"></span>
          </div>
        </div>
      </div>
      
      <!-- Quick Prompts -->
      <div class="bg-white px-4 py-2.5 border-t border-[#e2e8f0] flex gap-2 overflow-x-auto whitespace-nowrap hide-scrollbar">
        <button 
          v-for="prompt in currentOptions" 
          :key="prompt.key" 
          @click="handleMenuClick(prompt)" 
          class="text-[12px] md:text-[13px] bg-[#f8fafc] hover:bg-[#f1f5f9] active:bg-[#e2e8f0] text-[#475569] px-3.5 py-1.5 md:py-2 rounded-full transition-all border border-[#e2e8f0] hover:border-[#cbd5e1] active:scale-95 shadow-xs font-medium shrink-0"
        >
          {{ prompt.label }}
        </button>
      </div>
      
      <!-- Input -->
      <div class="p-3 md:p-4 bg-white border-t border-[#e2e8f0] flex gap-2.5 items-center relative z-10 pb-safe" :class="isMobile ? 'pb-5' : ''">
        <input 
          v-model="inputQuery" 
          @keyup.enter="handleSend(inputQuery)" 
          type="text" 
          :placeholder="currentI18n.placeholder" 
          class="flex-1 bg-[#f8fafc] border border-[#e2e8f0] rounded-full px-4 py-2.5 md:py-3 text-[14px] md:text-[15px] focus:outline-none focus:border-[#c9a84c] focus:ring-2 focus:ring-[#c9a84c]/20 transition-all placeholder:text-[#94a3b8] shadow-inner" 
        />
        <button 
          @click="handleSend(inputQuery)" 
          class="w-10 h-10 md:w-11 md:h-11 bg-gradient-to-br from-[#062d4d] to-[#0f172a] hover:from-[#0f172a] hover:to-[#1e293b] active:scale-90 text-white rounded-full flex items-center justify-center transition-all shadow-md group shrink-0"
          aria-label="Send message"
        >
          <svg class="w-4 h-4 md:w-5 md:h-5 translate-x-[1px] -translate-y-[1px] group-hover:translate-x-[2px] group-hover:-translate-y-[2px] transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"></path></svg>
        </button>
      </div>
    </div>
  </div>
  
  <!-- Draggable / Movable Floating Concierge Bubble -->
  <div 
    v-show="!isMobile || !isOpen"
    class="fixed z-[9999] select-none touch-none"
    :style="{
      left: bubblePos.x !== null ? `${bubblePos.x}px` : 'auto',
      top: bubblePos.y !== null ? `${bubblePos.y}px` : 'auto',
      right: bubblePos.x === null ? (isMobile ? '16px' : '24px') : 'auto',
      bottom: bubblePos.y === null ? '24px' : 'auto'
    }"
  >
    <button 
      @pointerdown="onBubblePointerDown"
      @pointermove="onBubblePointerMove"
      @pointerup="onBubblePointerUp"
      @pointercancel="onBubblePointerUp"
      class="concierge-bubble-btn group relative flex items-center justify-center rounded-full bg-[#062d4d] border border-[#c9a84c]/60 shadow-[0_10px_30px_rgba(6,45,77,0.5),0_0_20px_rgba(201,168,76,0.25)] select-none touch-none transition-transform duration-200"
      :class="[
        isMobile ? 'w-14 h-14' : 'w-16 h-16',
        isDragging ? 'scale-110 shadow-[0_15px_40px_rgba(6,45,77,0.7),0_0_30px_rgba(201,168,76,0.5)] border-[#c9a84c] cursor-grabbing' : 'hover:scale-105 active:scale-95 cursor-grab'
      ]"
      aria-label="Drag or Open AI Concierge"
    >
      <!-- Unread Notification Pulse Dot -->
      <div v-if="showNotification" class="absolute -top-1 -right-1 w-4 h-4 md:w-5 md:h-5 bg-red-500 border-2 border-white rounded-full flex items-center justify-center animate-pulse z-10 pointer-events-none"></div>
      
      <!-- Ambient Halo Glow -->
      <div v-show="!isOpen" class="absolute inset-0 rounded-full bg-[#c9a84c] opacity-0 group-hover:opacity-25 transition-opacity blur-md pointer-events-none"></div>
      
      <!-- Chat Icon -->
      <div 
        class="absolute inset-0 flex items-center justify-center text-[#c9a84c] transition-all duration-300 ease-[cubic-bezier(0.32,0.72,0,1)] pointer-events-none" 
        :class="isOpen && !isMinimized ? 'rotate-90 scale-0 opacity-0' : 'rotate-0 scale-100 opacity-100'"
      >
        <svg class="w-7 h-7 md:w-8 md:h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"></path>
        </svg>
      </div>

      <!-- Close Icon -->
      <div 
        class="absolute inset-0 flex items-center justify-center text-[#c9a84c] transition-all duration-300 ease-[cubic-bezier(0.32,0.72,0,1)] pointer-events-none" 
        :class="isOpen && !isMinimized ? 'rotate-0 scale-100 opacity-100' : '-rotate-90 scale-0 opacity-0'"
      >
        <svg class="w-7 h-7 md:w-8 md:h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
        </svg>
      </div>
    </button>
  </div>
</template>

<style scoped>
.hide-scrollbar::-webkit-scrollbar {
  display: none;
}
.hide-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

@keyframes wave {
  0%, 60%, 100% {
    transform: translateY(0);
  }
  30% {
    transform: translateY(-5px);
  }
}

/* Safe area padding for mobile input */
@supports (padding-bottom: env(safe-area-inset-bottom)) {
  .pb-safe {
    padding-bottom: calc(0.75rem + env(safe-area-inset-bottom));
  }
}
</style>
