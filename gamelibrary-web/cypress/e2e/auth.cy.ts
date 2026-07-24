import { loginPage } from '../pages/LoginPage';
import { registerPage } from '../pages/RegisterPage';
import { navBar } from '../pages/NavBar';
import { myGamesPage } from '../pages/MyGamesPage';

function uniqueEmail(prefix: string) {
    return `${prefix}-${Date.now()}-${Math.floor(Math.random() * 10000)}@teste.com`;
}

describe('Autenticação', () => {
    it('registra um novo usuário e redireciona para a listagem de jogos autenticado', () => {
        const email = uniqueEmail('registro');

        registerPage.visit().register(email, 'senha123');

        cy.url().should('eq', Cypress.config().baseUrl + '/');
        navBar.emailLabel().should('have.text', email);
    });

    it('faz login com um usuário existente', () => {
        cy.apiRegister('login').then((user) => {
            loginPage.visit().login(user.email, user.password);

            cy.url().should('eq', Cypress.config().baseUrl + '/');
            navBar.emailLabel().should('have.text', user.email);
        });
    });

    it('mostra erro ao tentar logar com credenciais inválidas', () => {
        loginPage.visit().login('nao-existe@teste.com', 'senhaerrada');

        loginPage.errorMessage().should('be.visible');
        cy.url().should('include', '/login');
    });

    it('redireciona para /login ao acessar uma rota protegida sem autenticação', () => {
        myGamesPage.visit();

        cy.url().should('include', '/login');
    });

    it('desloga e volta para o estado não autenticado', () => {
        cy.apiRegister('logout').then((user) => {
            cy.loginAs(user);

            navBar.logout();

            navBar.loginLink().should('be.visible');
            navBar.registerLink().should('be.visible');
        });
    });
});
