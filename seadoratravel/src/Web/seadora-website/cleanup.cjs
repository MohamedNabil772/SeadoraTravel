const fs = require('fs');

const files = [
  'src/features/tours/components/Trips.vue',
  'src/features/tours/views/TourDetailsView.vue',
  'src/features/tours/views/ToursView.vue'
];

files.forEach(f => {
  if (!fs.existsSync(f)) return;
  let c = fs.readFileSync(f, 'utf8');
  
  // Remove inline getSlug definition
  c = c.replace(/const getSlug = \([\s\S]*?\}\n/, '');
  
  // Add import
  if (!c.includes('import { getSlug }')) {
    c = c.replace(/<script setup lang="ts">/, '<script setup lang="ts">\nimport { getSlug } from \'@/shared/utils/helpers\'');
  }
  
  fs.writeFileSync(f, c);
});
