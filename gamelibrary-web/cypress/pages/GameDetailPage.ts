class GameDetailPage {
    visit(gameId: number) {
        cy.visit(`/games/${gameId}`);
        return this;
    }

    ownersList() {
        return cy.get('[data-cy=game-owners-list]');
    }

    ownerRows() {
        return cy.get('[data-cy=game-owner-row]');
    }

    pricePaidInput() {
        return cy.get('[data-cy=add-to-collection-price]');
    }

    submitButton() {
        return cy.get('[data-cy=add-to-collection-submit]');
    }

    addToCollection(price: number) {
        this.pricePaidInput().clear().type(String(price));
        this.submitButton().click();
        return this;
    }

    alreadyInCollectionMessage() {
        return cy.get('[data-cy=already-in-collection]');
    }
}

export const gameDetailPage = new GameDetailPage();
