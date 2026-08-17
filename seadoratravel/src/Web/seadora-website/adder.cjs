const fs = require('fs');

const files = [
  'src/features/tours/views/TourDetailsView.vue',
  'src/features/tours/views/ToursView.vue'
];

files.forEach(f => {
  let c = fs.readFileSync(f, 'utf8');
  if (!c.includes('import { getSlug }')) {
    c = c.replace(/<script setup lang="ts">/, '<script setup lang="ts">\nimport { getSlug } from \'@/shared/utils/helpers\'');
    fs.writeFileSync(f, c);
  }
});
