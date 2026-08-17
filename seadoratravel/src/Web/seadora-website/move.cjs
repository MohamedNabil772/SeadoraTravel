const fs = require('fs');
const path = require('path');

const srcDir = 'D:/Seadora Travel/seadoratravel/src/Web/seadora-website/src';

const moves = [
  ['store/auth.ts', 'features/auth/store/auth.ts'],
  ['store/contact.ts', 'features/contact/store/contact.ts'],
  ['components/Contact.vue', 'features/contact/components/Contact.vue'],
  ['components/Destinations.vue', 'features/destinations/components/Destinations.vue'],
  ['views/FeedbackView.vue', 'features/feedback/views/FeedbackView.vue'],
  ['views/ToursView.vue', 'features/tours/views/ToursView.vue'],
  ['views/TourDetailsView.vue', 'features/tours/views/TourDetailsView.vue'],
  ['components/TourDetailsModal.vue', 'features/tours/components/TourDetailsModal.vue'],
  ['components/Trips.vue', 'features/tours/components/Trips.vue'],
  ['components/Navbar.vue', 'shared/components/Navbar.vue'],
  ['components/Footer.vue', 'shared/components/Footer.vue'],
  ['components/Hero.vue', 'shared/components/Hero.vue'],
  ['components/WhyChoose.vue', 'shared/components/WhyChoose.vue'],
  ['components/Testimonials.vue', 'shared/components/Testimonials.vue'],
];

for (const [oldPath, newPath] of moves) {
  const fullOld = path.join(srcDir, oldPath);
  const fullNew = path.join(srcDir, newPath);
  if (fs.existsSync(fullOld)) {
    const newDir = path.dirname(fullNew);
    if (!fs.existsSync(newDir)) {
      fs.mkdirSync(newDir, { recursive: true });
    }
    fs.renameSync(fullOld, fullNew);
    console.log(`Moved ${oldPath} to ${newPath}`);
  } else {
    console.log(`Not found: ${oldPath}`);
  }
}
