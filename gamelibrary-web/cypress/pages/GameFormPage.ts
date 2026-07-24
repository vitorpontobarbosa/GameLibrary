export interface GameFormData {
    name: string;
    studio?: string;
    coverImageUrl: string;
    price?: number;
    description: string;
    steamLink: string;
}

class GameFormPage {
    visitNew() {
        cy.visit('/games/new');
        return this;
    }

    nameInput() {
        return cy.get('[data-cy=game-name]');
    }

    studioInput() {
        return cy.get('[data-cy=game-studio]');
    }

    coverInput() {
        return cy.get('[data-cy=game-cover]');
    }

    priceInput() {
        return cy.get('[data-cy=game-price]');
    }

    descriptionInput() {
        return cy.get('[data-cy=game-description]');
    }

    steamLinkInput() {
        return cy.get('[data-cy=game-steamlink]');
    }

    submitButton() {
        return cy.get('[data-cy=game-submit]');
    }

    errorMessage() {
        return cy.get('[data-cy=game-form-error]');
    }

    fill(data: GameFormData) {
        this.nameInput().clear().type(data.name);
        if (data.studio !== undefined) {
            this.studioInput().clear().type(data.studio);
        }
        this.coverInput().clear().type(data.coverImageUrl);
        if (data.price !== undefined) {
            this.priceInput().clear().type(String(data.price));
        }
        this.descriptionInput().clear().type(data.description);
        this.steamLinkInput().clear().type(data.steamLink);
        return this;
    }

    submit() {
        this.submitButton().click();
        return this;
    }

    fillAndSubmit(data: GameFormData) {
        this.fill(data);
        this.submit();
        return this;
    }
}

export const gameFormPage = new GameFormPage();
