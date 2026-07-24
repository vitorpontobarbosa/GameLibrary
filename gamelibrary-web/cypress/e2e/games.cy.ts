import { gameFormPage } from '../pages/GameFormPage';
import { gamesListPage } from '../pages/GamesListPage';
import { myGamesPage } from '../pages/MyGamesPage';
import type { ApiUser } from '../support/commands';

describe('Jogos', () => {
    beforeEach(function () {
        cy.apiRegister('games').then((user) => {
            cy.loginAs(user);
            cy.wrap(user).as('user');
        });
    });

    it('cria um jogo e exibe em "Meus jogos" e na listagem pública', function () {
        const name = `Jogo Cypress ${Date.now()}`;

        gameFormPage.visitNew().fillAndSubmit({
            name,
            studio: 'Estúdio Cypress',
            coverImageUrl: 'https://example.com/covers/cypress.jpg',
            price: 49.9,
            description: 'Jogo criado durante um teste end-to-end com Cypress.',
            steamLink: 'https://store.steampowered.com/app/111111',
        });

        cy.url().should('include', '/my-games');
        myGamesPage.cardByName(name).should('be.visible');

        gamesListPage.visit();
        gamesListPage.cardByName(name).should('be.visible');
    });

    it('edita um jogo existente', function () {
        const user = this.user as ApiUser;
        const originalName = `Jogo Original ${Date.now()}`;
        const editedName = `Jogo Editado ${Date.now()}`;

        cy.apiCreateGame(user, { name: originalName }).then(() => {
            myGamesPage.visit();
            myGamesPage.editButtonFor(originalName).click();

            cy.url().should('match', /\/games\/\d+\/edit$/);
            gameFormPage.nameInput().should('have.value', originalName);
            gameFormPage.fillAndSubmit({
                name: editedName,
                coverImageUrl: 'https://example.com/covers/cypress-edit.jpg',
                description: 'Descrição atualizada durante um teste end-to-end com Cypress.',
                steamLink: 'https://store.steampowered.com/app/222222',
            });

            cy.url().should('include', '/my-games');
            myGamesPage.cardByName(editedName).should('be.visible');
            myGamesPage.cardByName(originalName).should('not.exist');
        });
    });

    it('exclui um jogo', function () {
        const user = this.user as ApiUser;
        const name = `Jogo Para Excluir ${Date.now()}`;

        cy.apiCreateGame(user, { name }).then(() => {
            myGamesPage.visit();
            myGamesPage.cardByName(name).should('be.visible');

            myGamesPage.deleteGame(name);

            myGamesPage.cardByName(name).should('not.exist');
        });
    });
});
