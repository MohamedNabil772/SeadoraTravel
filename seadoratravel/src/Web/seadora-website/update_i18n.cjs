const fs = require('fs');
const path = require('path');

const locales = ['en', 'de', 'fr', 'it', 'ru'];
const basePath = 'D:\\Seadora Travel\\seadoratravel\\src\\Web\\seadora-website';

const placeholders = {
  en: {
    fullName: "e.g. John Smith",
    email: "name@example.com",
    whatsapp: "+20 100 123 4567",
    hotelName: "e.g. Sunrise Diamond Beach Resort",
    roomNumber: "e.g. 2104",
    specialRequests: "Optional: Vegetarian meals, anniversary celebration, child booster seat...",
    search: "Search tours...",
    date: "Select Date",
    message: "Tell us about your requirements..."
  },
  de: {
    fullName: "z.B. Maximilian Klein",
    email: "name@beispiel.de",
    whatsapp: "+49 170 1234567",
    hotelName: "z.B. Sunrise Arabian Beach Resort",
    roomNumber: "z.B. 204",
    specialRequests: "Optional: Vegetarisches Essen, Jubiläumsfeier, Kindersitz...",
    search: "Touren suchen...",
    date: "Datum auswählen",
    message: "Erzählen Sie uns von Ihren Anforderungen..."
  },
  fr: {
    fullName: "ex. Jean Dupont",
    email: "nom@exemple.fr",
    whatsapp: "+33 6 12 34 56 78",
    hotelName: "ex. Sunrise Resort",
    roomNumber: "ex. 2104",
    specialRequests: "Optionnel : Repas végétariens, célébration...",
    search: "Rechercher...",
    date: "Sélectionner la date",
    message: "Parlez-nous de vos besoins..."
  },
  it: {
    fullName: "es. Mario Rossi",
    email: "nome@esempio.it",
    whatsapp: "+39 312 3456789",
    hotelName: "es. Resort Sunrise",
    roomNumber: "es. 2104",
    specialRequests: "Opzionale: Pasti vegetariani, celebrazioni...",
    search: "Cerca tour...",
    date: "Seleziona data",
    message: "Raccontaci le tue esigenze..."
  },
  ru: {
    fullName: "напр. Иван Иванов",
    email: "имя@пример.ру",
    whatsapp: "+7 900 123 45 67",
    hotelName: "напр. Sunrise Resort",
    roomNumber: "напр. 2104",
    specialRequests: "Необязательно: Вегетарианское питание, праздник...",
    search: "Поиск...",
    date: "Выберите дату",
    message: "Расскажите нам о ваших требованиях..."
  }
};

locales.forEach(lang => {
  const p = path.join(basePath, 'src', 'locales', `${lang}.json`);
  let content = JSON.parse(fs.readFileSync(p, 'utf8'));
  
  if (!content.placeholders) {
    content.placeholders = {};
  }
  
  content.placeholders = { ...content.placeholders, ...placeholders[lang] };
  
  fs.writeFileSync(p, JSON.stringify(content, null, 2) + '\n', 'utf8');
  console.log(`Updated ${lang}.json`);
});

const vueReplacements = [
  {
    files: ['src/features/tours/views/TourDetailsView.vue', 'src/views/TourDetailsView.vue'],
    replacements: [
      { from: /placeholder="e\.g\. Maximilian Klein"/g, to: ':placeholder="$t(\'placeholders.fullName\')"' },
      { from: /placeholder="e\.g\. name@example\.com"/g, to: ':placeholder="$t(\'placeholders.email\')"' },
      { from: /placeholder="\+49 170 1234567"/g, to: ':placeholder="$t(\'placeholders.whatsapp\')"' },
      { from: /placeholder="e\.g\. Sunrise Arabian Beach Resort"/g, to: ':placeholder="$t(\'placeholders.hotelName\')"' },
      { from: /placeholder="e\.g\. 204"/g, to: ':placeholder="$t(\'placeholders.roomNumber\')"' },
      { from: /placeholder="Optional: Vegetarian meals, anniversary celebration, child booster seat\.\.\."/g, to: ':placeholder="$t(\'placeholders.specialRequests\')"' }
    ]
  },
  {
    files: ['src/features/tours/views/ToursView.vue'],
    replacements: [
      { from: /:placeholder="searchPlaceholder"/g, to: ':placeholder="$t(\'placeholders.search\')"' },
      { from: /placeholder="Enter your full name"/g, to: ':placeholder="$t(\'placeholders.fullName\')"' },
      { from: /placeholder="maria\.vance@example\.com"/g, to: ':placeholder="$t(\'placeholders.email\')"' },
      { from: /placeholder="\+1 555-0199 \(WhatsApp\)"/g, to: ':placeholder="$t(\'placeholders.whatsapp\')"' },
      { from: /placeholder="e\.g\. Steigenberger Resort"/g, to: ':placeholder="$t(\'placeholders.hotelName\')"' },
      { from: /placeholder="402"/g, to: ':placeholder="$t(\'placeholders.roomNumber\')"' }
    ]
  },
  {
    files: ['src/shared/components/Navbar.vue'],
    replacements: [
      { from: /placeholder="e\.g\. Amr Nabil"/g, to: ':placeholder="$t(\'placeholders.fullName\')"' },
      { from: /placeholder="e\.g\. guest@seadoratravel\.com"/g, to: ':placeholder="$t(\'placeholders.email\')"' }
    ]
  }
];

vueReplacements.forEach(group => {
  group.files.forEach(f => {
    const fullPath = path.join(basePath, f);
    if (fs.existsSync(fullPath)) {
      let content = fs.readFileSync(fullPath, 'utf8');
      let originalContent = content;
      group.replacements.forEach(r => {
        content = content.replace(r.from, r.to);
      });
      if (content !== originalContent) {
        fs.writeFileSync(fullPath, content, 'utf8');
        console.log(`Updated ${f}`);
      }
    }
  });
});
