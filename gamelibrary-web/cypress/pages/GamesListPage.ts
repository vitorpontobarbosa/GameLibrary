class GamesListPage {
    visit() {
        cy.visit('/');
        return this;
    }

    emptyMessage() {
        return cy.get('[data-cy=games-empty]');
    }

    cards() {
        return cy.get('[data-cy=game-card]');
    }

    cardByName(name: string) {
        return cy.get(`[data-cy=game-card][data-cy-name="${name}"]`);
    }

    openGame(name: string) {
        this.cardByName(name).find('[data-cy=game-card-link]').click();
        return this;
    }
}

export const gamesListPage = new GamesListPage();
