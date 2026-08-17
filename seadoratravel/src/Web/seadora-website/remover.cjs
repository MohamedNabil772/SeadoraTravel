const fs = require('fs');

const files = [
  'src/features/tours/views/TourDetailsView.vue',
  'src/features/tours/views/ToursView.vue'
];

files.forEach(f => {
  let c = fs.readFileSync(f, 'utf8');
  
  // The exact string to remove
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
  
  fs.writeFileSync(f, c);
});
