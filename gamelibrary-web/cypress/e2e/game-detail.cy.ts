import { gameDetailPage } from '../pages/GameDetailPage';
import { gamesListPage } from '../pages/GamesListPage';
import { myGamesPage } from '../pages/MyGamesPage';

describe('Página de detalhes do jogo', () => {
    it('mostra quem já tem o jogo e o preço pago', function () {
        cy.apiRegister('detail-owner').then((owner) => {
            cy.apiCreateGame(owner, { name: `Jogo Compartilhado ${Date.now()}`, price: 59.9 }).then((game) => {
                gameDetailPage.visit(game.id as number);

                gameDetailPage.ownerRows().should('have.length', 1);
                gameDetailPage.ownerRows().first().should('contain.text', owner.email);
                gameDetailPage.ownerRows().first().should('contain.text', 'R$ 59.90');
            });
        });
    });

    it('permite que um segundo usuário adicione um jogo já existente à própria coleção', function () {
        cy.apiRegister('detail-creator').then((creator) => {
            const name = `Jogo Para Adicionar ${Date.now()}`;
            cy.apiCreateGame(creator, { name, price: 40 }).then((game) => {
                cy.apiRegister('detail-second').then((second) => {
                    cy.loginAs(second);

                    gamesListPage.visit();
                    gamesListPage.openGame(name);

                    cy.url().should('include', `/games/${game.id as number}`);
                    gameDetailPage.ownerRows().should('have.length', 1);

                    gameDetailPage.addToCollection(19.9);

                    gameDetailPage.ownerRows().should('have.length', 2);
                    gameDetailPage.alreadyInCollectionMessage().should('be.visible');

                    myGamesPage.visit();
                    myGamesPage.cardByName(name).should('be.visible');
                });
            });
        });
    });
});
