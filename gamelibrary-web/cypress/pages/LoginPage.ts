class LoginPage {
    visit() {
        cy.visit('/login');
        return this;
    }

    emailInput() {
        return cy.get('[data-cy=login-email]');
    }

    passwordInput() {
        return cy.get('[data-cy=login-password]');
    }

    submitButton() {
        return cy.get('[data-cy=login-submit]');
    }

    errorMessage() {
        return cy.get('[data-cy=login-error]');
    }

    login(email: string, password: string) {
        this.emailInput().type(email);
        this.passwordInput().type(password);
        this.submitButton().click();
        return this;
    }
}

export const loginPage = new LoginPage();
