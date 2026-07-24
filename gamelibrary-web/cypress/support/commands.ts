const TOKEN_STORAGE_KEY = 'gamelibrary.token';

export interface ApiUser {
    email: string;
    password: string;
    token: string;
}

Cypress.Commands.add('apiRegister', (emailPrefix = 'cypress') => {
    const email = `${emailPrefix}-${Date.now()}-${Math.floor(Math.random() * 10000)}@teste.com`;
    const password = 'senha123';

    return cy
        .request('POST', `${Cypress.env('apiUrl')}/api/auth/register`, { email, password })
        .then((res) => {
            const user: ApiUser = { email, password, token: res.body.token };
            return user;
        });
});

// localStorage is per-origin, so the app must have been visited at least once
// before we can write to it; we then revisit so AuthProvider re-reads it on mount.
Cypress.Commands.add('loginAs', (user: ApiUser) => {
    cy.visit('/');
    cy.window().then((win) => {
        win.localStorage.setItem(TOKEN_STORAGE_KEY, user.token);
    });
    cy.visit('/');
});

Cypress.Commands.add('apiCreateGame', (user: ApiUser, overrides = {}) => {
    const payload = {
        name: 'Jogo de apoio',
        studio: 'Cypress Studio',
        coverImageUrl: 'https://example.com/covers/cypress.jpg',
        price: 29.9,
        description: 'Jogo criado via API para dar suporte a um teste de UI.',
        steamLink: 'https://store.steampowered.com/app/000000',
        ...overrides,
    };

    return cy
        .request({
            method: 'POST',
            url: `${Cypress.env('apiUrl')}/api/games`,
            headers: { Authorization: `Bearer ${user.token}` },
            body: payload,
        })
        .then((res) => res.body);
});

declare global {
    // eslint-disable-next-line @typescript-eslint/no-namespace
    namespace Cypress {
        interface Chainable {
            apiRegister(emailPrefix?: string): Chainable<ApiUser>;
            loginAs(user: ApiUser): Chainable<void>;
            apiCreateGame(user: ApiUser, overrides?: Record<string, unknown>): Chainable<Record<string, unknown>>;
        }
    }
}

export {};
