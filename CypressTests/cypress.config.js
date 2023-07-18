const { defineConfig } = require("cypress");
require("dotenv").config(); // Load the .env file

module.exports = defineConfig({
  e2e: {
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
  },
  env: {
    url: process.env.url,
    video: false
  },
});
