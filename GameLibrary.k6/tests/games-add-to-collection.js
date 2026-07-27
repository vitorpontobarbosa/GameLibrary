import { check } from 'k6';
import { login } from '../services/authService.js';
import { addToCollection, create, getById, getMyGames } from '../services/gamesService.js';

export const options = {
    vus: 1,
    iterations: 1,
    thresholds: {
        checks: ['rate==1.0'],
    },
};

export function setup() {
    const ownerLoginRes = login({ email: 'jogador1@k6.test', password: 'senha123' });
    check(ownerLoginRes, { 'login do dono retornou 200': (r) => r.status === 200 });
    const ownerToken = ownerLoginRes.json('token');

    const createRes = create(
        {
            name: 'Jogo criado no teste de POST /api/games/{id}/collection',
            studio: 'k6 Studio',
            coverImageUrl: 'https://example.com/covers/add-to-collection-teste.jpg',
            price: 49.9,
            description: 'Jogo criado para validar o endpoint de adicionar a colecao.',
            steamLink: 'https://store.steampowered.com/app/000005',
        },
        ownerToken
    );
    check(createRes, { 'jogo de apoio criado com sucesso': (r) => r.status === 201 });

    const secondLoginRes = login({ email: 'jogador2@k6.test', password: 'senha123' });
    check(secondLoginRes, { 'login do segundo usuario retornou 200': (r) => r.status === 200 });

    return { gameId: createRes.json('id'), secondToken: secondLoginRes.json('token') };
}

export default function (data) {
    const addRes = addToCollection(data.gameId, { pricePaid: 19.9 }, data.secondToken);
    check(addRes, {
        'POST /api/games/{id}/collection retorna 200': (r) => r.status === 200,
    });

    const duplicateRes = addToCollection(data.gameId, { pricePaid: 19.9 }, data.secondToken);
    check(duplicateRes, {
        'POST /api/games/{id}/collection duplicado retorna 400': (r) => r.status === 400,
    });

    const detailRes = getById(data.gameId);
    check(detailRes, {
        'GET /api/games/{id} apos adicionar mostra os dois donos': (r) => r.json('owners').length === 2,
        'GET /api/games/{id} mostra o preco pago pelo segundo usuario': (r) =>
            r.json('owners').some((owner) => owner.pricePaid === 19.9),
    });

    const myGamesRes = getMyGames(data.secondToken);
    check(myGamesRes, {
        'GET /api/games/me do segundo usuario inclui o jogo adicionado': (r) =>
            r.json().some((game) => game.id === data.gameId && game.pricePaid === 19.9),
    });
}
