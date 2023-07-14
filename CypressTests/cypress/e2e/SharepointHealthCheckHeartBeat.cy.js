/// <reference types="Cypress" />

describe('Sharepoint API Testing', () => {

      let url = Cypress.env('url')
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