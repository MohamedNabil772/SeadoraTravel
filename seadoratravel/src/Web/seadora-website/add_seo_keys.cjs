/**
 * One-off locale patch: adds the `seo.*` metadata keys to every locale file
 * (for localized per-route titles/descriptions) and reports any keys that
 * exist in en.json but are missing in the other locales.
 *
 * Usage: node add_seo_keys.cjs
 */
const fs = require('fs')
const path = require('path')

const dir = path.join(__dirname, 'src', 'locales')

const SEO = {
  en: {
    home: {
      title: 'Seadora Travel – Luxury Egypt Experiences & Red Sea Tours',
      description: 'Luxury Egypt tours with Seadora Travel: Red Sea cruises, diving, Nile voyages and private Cairo & Luxor experiences. VIP concierge, hotel pickup and expert local guides.'
    },
    tours: {
      title: 'Egypt Tours & Excursions | Seadora Travel',
      description: 'Browse curated Egypt tours: snorkeling & diving in Hurghada, Pyramids day trips, Nile cruises, desert safaris and private luxury experiences.'
    },
    comingSoon: {
      title: 'Something Extraordinary Is Coming | Seadora Travel',
      description: 'A new luxury travel experience from Seadora Travel is on the way. Stay tuned.'
    },
    feedback: {
      title: 'Share Your Experience | Seadora Travel',
      description: 'Tell us about your Seadora Travel tour. Your feedback helps us craft extraordinary journeys.'
    }
  },
  fr: {
    home: {
      title: 'Seadora Travel – Expériences de luxe en Égypte & croisières en mer Rouge',
      description: 'Voyages de luxe en Égypte avec Seadora Travel : croisières en mer Rouge, plongée, croisières sur le Nil et expériences privées au Caire & à Louxor. Concierge VIP, transfert hôtel et guides locaux experts.'
    },
    tours: {
      title: 'Circuits & Excursions en Égypte | Seadora Travel',
      description: 'Découvrez nos circuits Égypte : snorkeling & plongée à Hurghada, excursions aux Pyramides, croisières sur le Nil, safaris dans le désert et expériences de luxe privées.'
    },
    comingSoon: {
      title: "Quelque chose d'extraordinaire arrive | Seadora Travel",
      description: 'Une nouvelle expérience de voyage de luxe signée Seadora Travel arrive bientôt. Restez à l’écoute.'
    },
    feedback: {
      title: 'Partagez votre expérience | Seadora Travel',
      description: 'Racontez votre circuit Seadora Travel. Vos retours nous aident à créer des voyages extraordinaires.'
    }
  },
  de: {
    home: {
      title: 'Seadora Travel – Luxusreisen Ägypten & Rotes Meer Touren',
      description: 'Luxusreisen in Ägypten mit Seadora Travel: Rote-Meer-Kreuzfahrten, Tauchen, Nilkreuzfahrten und private Kairo- & Luxor-Erlebnisse. VIP-Concierge, Hotelabholung und erfahrene lokale Guides.'
    },
    tours: {
      title: 'Ägypten Touren & Ausflüge | Seadora Travel',
      description: 'Entdecken Sie kuratierte Ägypten-Touren: Schnorcheln & Tauchen in Hurghada, Pyramiden-Tagestrips, Nilkreuzfahrten, Wüstensafaris und private Luxus-Erlebnisse.'
    },
    comingSoon: {
      title: 'Etwas Außergewöhnliches kommt | Seadora Travel',
      description: 'Ein neues Luxusreiseerlebnis von Seadora Travel ist auf dem Weg. Bleiben Sie gespannt.'
    },
    feedback: {
      title: 'Teilen Sie Ihre Erfahrung | Seadora Travel',
      description: 'Erzählen Sie uns von Ihrer Seadora-Travel-Reise. Ihr Feedback hilft uns, außergewöhnliche Reisen zu gestalten.'
    }
  },
  it: {
    home: {
      title: 'Seadora Travel – Esperienze di lusso in Egitto e tour sul Mar Rosso',
      description: 'Viaggi di lusso in Egitto con Seadora Travel: crociere sul Mar Rosso, immersioni, crociere sul Nilo ed esperienze private al Cairo e a Luxor. Concierge VIP, transfer in hotel e guide locali esperte.'
    },
    tours: {
      title: 'Tour ed escursioni in Egitto | Seadora Travel',
      description: 'Scopri i tour in Egitto selezionati: snorkeling e immersioni a Hurghada, gite alle Piramidi, crociere sul Nilo, safari nel deserto ed esperienze di lusso private.'
    },
    comingSoon: {
      title: 'Sta arrivando qualcosa di straordinario | Seadora Travel',
      description: 'Una nuova esperienza di viaggio di lusso firmata Seadora Travel è in arrivo. Restate sintonizzati.'
    },
    feedback: {
      title: 'Condividi la tua esperienza | Seadora Travel',
      description: 'Raccontaci il tuo tour Seadora Travel. Il tuo feedback ci aiuta a creare viaggi straordinari.'
    }
  },
  ru: {
    home: {
      title: 'Seadora Travel — Роскошные туры в Египет и на Красное море',
      description: 'Роскошные туры в Египет с Seadora Travel: круизы по Красному морю, дайвинг, круизы по Нилу и частные экскурсии в Каире и Луксоре. VIP-консьерж, трансфер из отеля и опытные местные гиды.'
    },
    tours: {
      title: 'Туры и экскурсии в Египте | Seadora Travel',
      description: 'Отобранные туры по Египту: снорклинг и дайвинг в Хургаде, экскурсии к пирамидам, круизы по Нилу, сафари в пустыне и частные роскошные впечатления.'
    },
    comingSoon: {
      title: 'Впереди нечто удивительное | Seadora Travel',
      description: 'Новое роскошное туристическое впечатление от Seadora Travel уже скоро. Оставайтесь с нами.'
    },
    feedback: {
      title: 'Поделитесь впечатлениями | Seadora Travel',
      description: 'Расскажите о своём туре с Seadora Travel. Ваш отзыв помогает нам создавать невероятные путешествия.'
    }
  }
}

function leafPaths(obj, prefix = '') {
  const out = []
  for (const [k, v] of Object.entries(obj || {})) {
    const key = prefix ? `${prefix}.${k}` : k
    if (v && typeof v === 'object') out.push(...leafPaths(v, key))
    else out.push(key)
  }
  return out
}

const locales = ['en', 'fr', 'de', 'it', 'ru']
const messages = {}
for (const l of locales) {
  messages[l] = JSON.parse(fs.readFileSync(path.join(dir, `${l}.json`), 'utf8'))
  if (!messages[l].seo) messages[l].seo = SEO[l]
  fs.writeFileSync(path.join(dir, `${l}.json`), JSON.stringify(messages[l], null, 2) + '\n', 'utf8')
  console.log(`${l}.json: seo keys ${messages[l].seo === SEO[l] ? 'added' : 'already present'}`)
}

const enKeys = new Set(leafPaths(messages.en))
for (const l of locales.filter(x => x !== 'en')) {
  const keys = new Set(leafPaths(messages[l]))
  const missing = [...enKeys].filter(k => !keys.has(k))
  console.log(`\n${l}: ${missing.length} missing key(s) vs en`)
  if (missing.length) console.log(missing.map(k => `  - ${k}`).join('\n'))
}
console.log('\nDone.')