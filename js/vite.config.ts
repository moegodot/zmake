import { defineConfig } from 'vite'

export default defineConfig({
  build: {
    target: "es2024",
    lib: {
      entry: ['./src/zmake.ts'],
      fileName: (format, entryName) => `${entryName}.js`,
      formats: ["es"]
    },
  },
})
