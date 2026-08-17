const fs = require('fs');

const f = 'src/features/tours/components/Trips.vue';
let c = fs.readFileSync(f, 'utf8');

const toRemove = `const getSlug = (name: string) => {
  return name
    .toLowerCase()
    .replace(/[^\\w\\s-]/g, '')
    .replace(/\\s+/g, '-')
    .replace(/-+/g, '-')
    .trim()
}`;

const toRemoveWindows = toRemove.replace(/\n/g, '\r\n');

c = c.replace(toRemove, '');
c = c.replace(toRemoveWindows, '');

if (!c.includes('import { getSlug }')) {
  c = c.replace(/<script setup lang="ts">/, '<script setup lang="ts">\nimport { getSlug } from \'@/shared/utils/helpers\'');
}

fs.writeFileSync(f, c);
