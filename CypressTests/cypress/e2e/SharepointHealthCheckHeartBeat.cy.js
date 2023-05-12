/// <reference types="Cypress" />

describe('Sharepoint API Testing', () => {

      let url = Cypress.config('url')
      let healthCheckEndpoint = '/api/HealthInternalStatusOnly'


      it('Sharepoint API Healthcheck / Heartbeat Test', () => {
        cy.request({
                method: 'GET',
                url: url + healthCheckEndpoint
        }).then((response) => {
          expect(response).to.have.property('status', 200)
        })



  })
})