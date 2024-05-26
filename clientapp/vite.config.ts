import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  base: '/app',
  server: {
    https: false,
    port: 5100,
    strictPort: true
  },
  plugins: [react()],
})
