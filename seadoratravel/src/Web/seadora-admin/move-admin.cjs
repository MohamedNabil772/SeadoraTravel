const fs = require('fs');
const path = require('path');

const srcDir = 'D:/Seadora Travel/seadoratravel/src/Web/seadora-admin/src';

const moves = [
  ['views/LoginView.vue', 'features/auth/views/LoginView.vue'],
  ['stores/auth.ts', 'features/auth/store/auth.ts'],
  ['views/ToursView.vue', 'features/tours/views/ToursView.vue'],
  ['views/TourEditView.vue', 'features/tours/views/TourEditView.vue'],
  ['views/DestinationsView.vue', 'features/destinations/views/DestinationsView.vue'],
  ['views/CategoriesView.vue', 'features/categories/views/CategoriesView.vue'],
  ['views/BookingsView.vue', 'features/bookings/views/BookingsView.vue'],
  ['views/BookingDetailsView.vue', 'features/bookings/views/BookingDetailsView.vue'],
  ['views/FeedbackView.vue', 'features/feedback/views/FeedbackView.vue'],
  ['views/UsersView.vue', 'features/users/views/UsersView.vue'],
  ['views/ReportsView.vue', 'features/reports/views/ReportsView.vue'],
  ['views/SuppliersView.vue', 'features/suppliers/views/SuppliersView.vue']
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
