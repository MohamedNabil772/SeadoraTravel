import { createI18n } from 'vue-i18n'

const messages = {
  en: {
    concierge: {
      title: 'Seadora AI Concierge',
      status: {
        online: 'Online'
      },
      welcome: 'Hello! I am your Seadora AI Concierge. How can I help you plan your perfect trip today?',
      placeholder: 'Ask me anything...'
    },
    actions: {
      sound: 'Toggle Sound',
      maximize: 'Toggle Maximize',
      clear: 'Clear Chat',
      close: 'Close'
    },
    quickActions: {
      topRated: 'Top Rated',
      waterSports: 'Water Sports',
      desertSafari: 'Desert Safari',
      weekend: 'Weekend Availability',
      permits: 'Permits & Passports',
      whatsapp: 'WhatsApp Concierge'
    }
  },
  de: {
    concierge: {
      title: 'Seadora KI-Concierge',
      status: {
        online: 'Online'
      },
      welcome: 'Hallo! Ich bin Ihr Seadora KI-Concierge. Wie kann ich Ihnen heute bei der Planung Ihrer perfekten Reise helfen?',
      placeholder: 'Fragen Sie mich etwas...'
    },
    actions: {
      sound: 'Ton umschalten',
      maximize: 'Maximieren umschalten',
      clear: 'Chat leeren',
      close: 'Schließen'
    },
    quickActions: {
      topRated: 'Am besten bewertet',
      waterSports: 'Wassersport',
      desertSafari: 'Wüstensafari',
      weekend: 'Wochenendverfügbarkeit',
      permits: 'Genehmigungen & Pässe',
      whatsapp: 'WhatsApp Concierge'
    }
  },
  fr: {
    concierge: {
      title: 'Concierge IA Seadora',
      status: {
        online: 'En ligne'
      },
      welcome: 'Bonjour ! Je suis votre Concierge IA Seadora. Comment puis-je vous aider à planifier votre voyage parfait aujourd\'hui ?',
      placeholder: 'Posez-moi n\'importe quelle question...'
    },
    actions: {
      sound: 'Basculer le son',
      maximize: 'Agrandir',
      clear: 'Effacer le chat',
      close: 'Fermer'
    },
    quickActions: {
      topRated: 'Les mieux notés',
      waterSports: 'Sports nautiques',
      desertSafari: 'Safari dans le désert',
      weekend: 'Disponibilité le week-end',
      permits: 'Permis et passeports',
      whatsapp: 'Concierge WhatsApp'
    }
  },
  it: {
    concierge: {
      title: 'Concierge IA Seadora',
      status: {
        online: 'Online'
      },
      welcome: 'Ciao! Sono il tuo Concierge IA Seadora. Come posso aiutarti a pianificare il tuo viaggio perfetto oggi?',
      placeholder: 'Chiedimi qualsiasi cosa...'
    },
    actions: {
      sound: 'Attiva/Disattiva audio',
      maximize: 'Ingrandisci',
      clear: 'Cancella chat',
      close: 'Chiudi'
    },
    quickActions: {
      topRated: 'I più votati',
      waterSports: 'Sport acquatici',
      desertSafari: 'Safari nel deserto',
      weekend: 'Disponibilità nel weekend',
      permits: 'Permessi e passaporti',
      whatsapp: 'Concierge WhatsApp'
    }
  },
  ru: {
    concierge: {
      title: 'ИИ-Консьерж Seadora',
      status: {
        online: 'В сети'
      },
      welcome: 'Здравствуйте! Я ваш ИИ-Консьерж Seadora. Как я могу помочь вам спланировать идеальное путешествие сегодня?',
      placeholder: 'Спросите меня о чем угодно...'
    },
    actions: {
      sound: 'Переключить звук',
      maximize: 'Развернуть',
      clear: 'Очистить чат',
      close: 'Закрыть'
    },
    quickActions: {
      topRated: 'Лучшие рейтинги',
      waterSports: 'Водные виды спорта',
      desertSafari: 'Сафари в пустыне',
      weekend: 'Наличие на выходные',
      permits: 'Разрешения и паспорта',
      whatsapp: 'WhatsApp Консьерж'
    }
  }
}

export const i18n = createI18n({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
  messages
})
