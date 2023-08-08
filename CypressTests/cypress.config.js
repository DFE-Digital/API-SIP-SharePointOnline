const { defineConfig } = require("cypress");
require("dotenv").config(); // Load the .env file
import { generateZapReport } from './cypress/plugins/generateZapReport'

module.exports = defineConfig({
  e2e: {
    setupNodeEvents(on, config) {
      on('before:run', () => {
        // Map cypress env vars to process env vars for usage outside of Cypress run
        process.env = config.env
      })

      on('after:run', async () => {
        if(process.env.ZAP) {
          await generateZapReport()
        }
      })
    },
  },
  env: {
    url: process.env.url,
    video: false
  },
});
