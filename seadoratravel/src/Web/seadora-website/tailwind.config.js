/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,js,ts,jsx,tsx,vue}",
  ],
  theme: {
    extend: {
      colors: {
        sea: {
          DEFAULT: '#0a5c8a',
          light: '#1a8bc4',
          deep: '#063a5c',
        },
        sun: {
          DEFAULT: '#e8820a',
          light: '#f5a435',
          pale: '#fdf3e0',
        },
        grass: {
          DEFAULT: '#2e7d4f',
          light: '#4caf78',
        },
        gold: '#c9a84c',
        cream: '#faf7f2',
        dark: '#0d1f2d',
        text: '#2a3f4f',
        muted: '#6b8a9a',
      },
      fontFamily: {
        serif: ['"Playfair Display"', 'serif'],
        cormorant: ['"Cormorant Garamond"', 'serif'],
        sans: ['Jost', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
