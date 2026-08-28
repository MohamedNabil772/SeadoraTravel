/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Luxury Core Palette
        primary: {
          DEFAULT: '#0A192F', // Deep Navy
          light: '#172A45',
          dark: '#020C1B',
        },
        secondary: {
          DEFAULT: '#D4AF37', // Refined Gold
          light: '#F4D03F',
          dark: '#997A00',
          // ponytail: text/icon-safe gold. Brand token above is unchanged; this one is
          // only for gold text or icons on white/light surfaces (7.1:1 on white).
          text: '#6B5310',
        },
        surface: {
          DEFAULT: '#fdfff5',
          sunken: '#f4f6e8',
          elevated: '#fdfff5',
        },
        text: {
          main: '#1A202C',
          muted: '#718096',
          inverse: '#F7FAFC',
        },
        border: {
          DEFAULT: '#E2E8F0',
          light: '#EDF2F7',
        },
        // Legacy colors to prevent immediate breaking
        stroke: '#E2E8F0',
        strokedark: '#2E3A47',
        dark: '#1C2434',
        boxdark: '#24303F',
        boxdark2: '#1A222F',
        body: '#64748B',
        bodydark1: '#DEE4EE',
        bodydark2: '#8A99AD',
        whiten: '#F9FAFB',
        whiter: '#F5F7FD',
        'meta-1': '#D34053',
        'meta-3': '#10B981',
        'meta-4': '#313D4A',
        'meta-5': '#259AE6',
        'meta-6': '#FFBA00',
        
        sea: { DEFAULT: '#0a5c8a', light: '#1a8bc4', deep: '#063a5c' },
        sun: { DEFAULT: '#e8820a', light: '#f5a435', pale: '#fdf3e0' },
        grass: { DEFAULT: '#2e7d4f', light: '#4caf78' },
        gold: '#c9a84c',
        cream: '#faf7f2',
        
        navy: {
          50: '#f0f4f8',
          100: '#d9e2ec',
          200: '#bcccdc',
          300: '#9fb3c8',
          400: '#829ab1',
          500: '#627d98',
          600: '#486581',
          700: '#334e68',
          800: '#102a43',
          900: '#061c33',
          950: '#041224',
        },
      },
      fontFamily: {
        serif: ['"Playfair Display"', 'serif'],
        cormorant: ['"Cormorant Garamond"', 'serif'],
        sans: ['Jost', 'system-ui', 'sans-serif'],
      },
      boxShadow: {
        default: '0px 2px 4px rgba(0, 0, 0, 0.04), 0px 4px 6px rgba(0, 0, 0, 0.04)',
        card: '0 4px 20px rgba(0,0,0,0.03)',
        'card-hover': '0 8px 30px rgba(0,0,0,0.06)',
        glass: '0 8px 32px 0 rgba(31, 38, 135, 0.07)',
      },
      animation: {
        'fade-in': 'fadeIn 300ms ease-out',
        'slide-up': 'slideUp 400ms cubic-bezier(0.16, 1, 0.3, 1)',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '10%': { opacity: '1' },
        },
        slideUp: {
          '0%': { opacity: '0', transform: 'translateY(10px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        }
      }
    },
  },
  plugins: [],
}
