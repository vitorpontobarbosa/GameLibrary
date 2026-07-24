class MyGamesPage {
    visit() {
        cy.visit('/my-games');
        return this;
    }

    emptyMessage() {
        return cy.get('[data-cy=my-games-empty]');
    }

    errorMessage() {
        return cy.get('[data-cy=my-games-error]');
    }

    cards() {
        return cy.get('[data-cy=game-card]');
    }

    cardByName(name: string) {
        return cy.get(`[data-cy=game-card][data-cy-name="${name}"]`);
    }

    editButtonFor(name: string) {
        return this.cardByName(name).find('[data-cy=game-card-edit]');
    }

    deleteButtonFor(name: string) {
        return this.cardByName(name).find('[data-cy=game-card-delete]');
    }

    deleteGame(name: string) {
        cy.on('window:confirm', () => true);
        this.deleteButtonFor(name).click();
        return this;
    }
}

export const myGamesPage = new MyGamesPage();
