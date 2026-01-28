/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      colors: {
        primary: '#6D28D9',
        primaryHover: '#5B21B6',
        accent: '#A78BFA'
      }
    }
  },
  plugins: []
};
