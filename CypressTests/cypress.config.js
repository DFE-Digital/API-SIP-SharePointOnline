const { defineConfig } = require("cypress");
require("dotenv").config(); // Load the .env file
const { generateZapReport } = require('./cypress/plugins/generateZapReport')

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
  userAgent: 'SipApiSharePointOnline/1.0 Cypress',
});
