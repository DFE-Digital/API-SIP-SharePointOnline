# Sharepoint API Healthcheck Test

This is the healthcheck test for the Sharepoint API service. This has been written using [Cypress](https://cypress.io) in JavaScript.

## Setup

### Installation

You will need to install all dependencies using `npm install`.

### Environment setup

There is the 'url' variable you will need to set to be able to run the test locally. This can be added to a `.env` file within the CypressTests folder, or in your terminal session:

```
url
```

## Running the tests

There are two ways Cypress provides to run tests - through the Cypress runner and headless in command line.

To run through the Cypress runner, use `npm run cy:open`

To run through the command line, use `npm run cy:run`