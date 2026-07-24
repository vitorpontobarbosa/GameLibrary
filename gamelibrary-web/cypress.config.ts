import { defineConfig } from 'cypress';

export default defineConfig({
    e2e: {
        baseUrl: process.env.CYPRESS_BASE_URL ?? 'http://localhost:3000',
        supportFile: 'cypress/support/e2e.ts',
        specPattern: 'cypress/e2e/**/*.cy.ts',
        video: false,
        screenshotOnRunFailure: true,
    },
    env: {
        apiUrl: process.env.CYPRESS_API_URL ?? 'http://localhost:8080',
    },
});
