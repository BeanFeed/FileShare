/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors:{
        slate:{
          850: "#090F21",
          "900-light": "#878B95"
        }
      },
      screens: {
        '2.5xl': '1597px'
      }
    },
  },
  plugins: []
}

