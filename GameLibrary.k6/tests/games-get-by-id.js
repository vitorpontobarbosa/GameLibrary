import { check } from 'k6';
import { login } from '../services/authService.js';
import { create, getById } from '../services/gamesService.js';

export const options = {
    vus: 1,
    iterations: 1,
    thresholds: {
        checks: ['rate==1.0'],
    },
};

export function setup() {
    const loginRes = login({ email: 'jogador1@k6.test', password: 'senha123' });
    check(loginRes, { 'login retornou 200': (r) => r.status === 200 });
    const token = loginRes.json('token');

    const createRes = create(
        {
            name: 'Jogo criado no teste de GET /api/games/{id}',
            studio: 'k6 Studio',
            coverImageUrl: 'https://example.com/covers/get-by-id-teste.jpg',
            price: 19.9,
            description: 'Jogo criado para validar o endpoint de busca por id.',
            steamLink: 'https://store.steampowered.com/app/000002',
        },
        token
    );
    check(createRes, { 'jogo de apoio criado com sucesso': (r) => r.status === 201 });

    return { token, gameId: createRes.json('id') };
}

export default function (data) {
    const res = getById(data.gameId, data.token);

    check(res, {
        'GET /api/games/{id} retorna 200': (r) => r.status === 200,
        'GET /api/games/{id} retorna o jogo esperado': (r) =>
            r.json('name') === 'Jogo criado no teste de GET /api/games/{id}',
    });
}
