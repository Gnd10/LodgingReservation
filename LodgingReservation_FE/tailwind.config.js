/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        primary: '#0F766E',      // Emerald green
        primaryHover: '#0D9488', // Hover State
        secondary: '#D87706',    // Amber/Gold
        background: '#F8FAFC',   // Off-white / Slate 50
        surface: '#FFFFFF',      // Clean-white
        textDark: '#1E293B',     // Slate 800
        textMuted: '#64748B',    // Slate 500
        border: '#E2E8F0',       // Slate 200
      }
    },
  },
  plugins: [],
}

