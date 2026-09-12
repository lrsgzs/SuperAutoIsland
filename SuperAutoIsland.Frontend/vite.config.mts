import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [react()],
    base: './',
    build: {
        outDir: 'dist',
        emptyOutDir: true,
        target: 'esnext',
        sourcemap: true,
        chunkSizeWarningLimit: 6000,
        rolldownOptions: {
            output: {
                manualChunks(id) {
                    if (id.includes('node_modules/prettier')) return 'prettier';
                    if (id.includes('node_modules/blockly')) return 'blockly';
                },
            },
        },
    },
    server: {
        port: 8080,
        host: true,
    },
});
