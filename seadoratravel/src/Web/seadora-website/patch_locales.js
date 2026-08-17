import fs from 'fs'
import path from 'path'

const localesPath = 'D:\\Seadora Travel\\seadoratravel\\src\\Web\\seadora-website\\src\\locales'

const translations = {
  en: {
    welcome: 'Welcome to Seadora Travel! How may I assist you with your luxury Red Sea experience today?',
    prompts: {
      recommend: '🏝️ Recommend Best Tours',
      weekend: '📅 Check Weekend Availability',
      payment: '💳 Payment & Cancellation',
      passport: '🛂 Why Passport Upload?'
    },
    recommendationFallback: 'Here are some of our top-rated experiences I recommend for you:',
    connectionError: 'I am sorry, I am having trouble connecting to the server.',
    processingError: 'I am sorry, an error occurred while processing your request.',
    title: 'Seadora Concierge',
    online: 'Online',
    copied: 'Copied!',
    bookNow: 'Book Now',
    placeholder: 'Message Concierge...'
  },
  de: {
    welcome: 'Willkommen bei Seadora Travel! Wie kann ich Ihnen heute bei Ihrem Luxus-Erlebnis am Roten Meer helfen?',
    prompts: {
      recommend: '🏝️ Beste Touren empfehlen',
      weekend: '📅 Wochenend-Verfügbarkeit',
      payment: '💳 Zahlung & Stornierung',
      passport: '🛂 Warum Reisepass hochladen?'
    },
    recommendationFallback: 'Hier sind einige unserer bestbewerteten Erlebnisse, die ich Ihnen empfehle:',
    connectionError: 'Es tut mir leid, ich habe Probleme, eine Verbindung zum Server herzustellen.',
    processingError: 'Es tut mir leid, bei der Bearbeitung Ihrer Anfrage ist ein Fehler aufgetreten.',
    title: 'Seadora Concierge',
    online: 'Online',
    copied: 'Kopiert!',
    bookNow: 'Jetzt buchen',
    placeholder: 'Nachricht an Concierge...'
  },
  fr: {
    welcome: 'Bienvenue chez Seadora Travel ! Comment puis-je vous aider avec votre expérience de luxe en mer Rouge aujourd\'hui ?',
    prompts: {
      recommend: '🏝️ Recommander les meilleures visites',
      weekend: '📅 Disponibilité le week-end',
      payment: '💳 Paiement et annulation',
      passport: '🛂 Pourquoi télécharger le passeport ?'
    },
    recommendationFallback: 'Voici quelques-unes de nos expériences les mieux notées que je vous recommande :',
    connectionError: 'Je suis désolé, je n\'arrive pas à me connecter au serveur.',
    processingError: 'Je suis désolé, une erreur s\'est produite lors du traitement de votre demande.',
    title: 'Concierge Seadora',
    online: 'En ligne',
    copied: 'Copié !',
    bookNow: 'Réserver',
    placeholder: 'Message au Concierge...'
  },
  it: {
    welcome: 'Benvenuti a Seadora Travel! Come posso aiutarvi oggi con la vostra esperienza di lusso sul Mar Rosso?',
    prompts: {
      recommend: '🏝️ Consiglia i migliori tour',
      weekend: '📅 Disponibilità per il fine settimana',
      payment: '💳 Pagamento e cancellazione',
      passport: '🛂 Perché caricare il passaporto?'
    },
    recommendationFallback: 'Ecco alcune delle nostre esperienze più votate che ti consiglio:',
    connectionError: 'Mi dispiace, ho problemi di connessione al server.',
    processingError: 'Mi dispiace, si è verificato un errore durante l\'elaborazione della tua richiesta.',
    title: 'Concierge Seadora',
    online: 'In linea',
    copied: 'Copiato!',
    bookNow: 'Prenota Ora',
    placeholder: 'Messaggio al Concierge...'
  },
  ru: {
    welcome: 'Добро пожаловать в Seadora Travel! Чем я могу помочь вам сегодня с вашим роскошным отдыхом на Красном море?',
    prompts: {
      recommend: '🏝️ Посоветовать лучшие туры',
      weekend: '📅 Доступность на выходных',
      payment: '💳 Оплата и отмена',
      passport: '🛂 Зачем загружать паспорт?'
    },
    recommendationFallback: 'Вот несколько наших самых рейтинговых впечатлений, которые я вам рекомендую:',
    connectionError: 'К сожалению, у меня проблемы с подключением к серверу.',
    processingError: 'К сожалению, при обработке вашего запроса произошла ошибка.',
    title: 'Консьерж Seadora',
    online: 'В сети',
    copied: 'Скопировано!',
    bookNow: 'Забронировать',
    placeholder: 'Сообщение консьержу...'
  }
}

for (const lang of Object.keys(translations)) {
  const filePath = path.join(localesPath, `${lang}.json`)
  if (fs.existsSync(filePath)) {
    const data = JSON.parse(fs.readFileSync(filePath, 'utf8'))
    data.concierge = translations[lang]
    fs.writeFileSync(filePath, JSON.stringify(data, null, 2))
    console.log(`Updated ${lang}.json`)
  }
}
