class NavBar {
    gamesLink() {
        return cy.get('[data-cy=nav-games]');
    }

    myGamesLink() {
        return cy.get('[data-cy=nav-my-games]');
    }

    newGameLink() {
        return cy.get('[data-cy=nav-new-game]');
    }

    emailLabel() {
        return cy.get('[data-cy=nav-email]');
    }

    loginLink() {
        return cy.get('[data-cy=nav-login]');
    }

    registerLink() {
        return cy.get('[data-cy=nav-register]');
    }

    logoutButton() {
        return cy.get('[data-cy=nav-logout]');
    }

    logout() {
        this.logoutButton().click();
        return this;
    }
}

export const navBar = new NavBar();
