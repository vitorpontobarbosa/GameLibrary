class RegisterPage {
    visit() {
        cy.visit('/register');
        return this;
    }

    emailInput() {
        return cy.get('[data-cy=register-email]');
    }

    passwordInput() {
        return cy.get('[data-cy=register-password]');
    }

    submitButton() {
        return cy.get('[data-cy=register-submit]');
    }

    errorMessage() {
        return cy.get('[data-cy=register-error]');
    }

    register(email: string, password: string) {
        this.emailInput().type(email);
        this.passwordInput().type(password);
        this.submitButton().click();
        return this;
    }
}

export const registerPage = new RegisterPage();
