const fs = require('fs');
const path = require('path');

const basePath = 'D:\\Seadora Travel\\seadoratravel\\src\\Web\\seadora-website';

const vueReplacements = [
  {
    files: [
      'src/components/Trips.vue',
      'src/features/tours/components/Trips.vue'
    ],
    replacements: [
      { from: /placeholder="Enter your name"/g, to: ':placeholder="$t(\'placeholders.fullName\')"' },
      { from: /placeholder="maria@example\.com"/g, to: ':placeholder="$t(\'placeholders.email\')"' }
    ]
  },
  {
    files: [
      'src/features/feedback/views/FeedbackView.vue'
    ],
    replacements: [
      { from: /placeholder="Enter your name"/g, to: ':placeholder="$t(\'placeholders.fullName\')"' },
      { from: /placeholder="name@example\.com"/g, to: ':placeholder="$t(\'placeholders.email\')"' },
      { from: /placeholder="Tell us about the highlights of your luxury journey\.\.\."/g, to: ':placeholder="$t(\'placeholders.message\')"' }
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
