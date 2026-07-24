import { check } from 'k6';
import { login } from '../services/authService.js';
import { create, remove, getById } from '../services/gamesService.js';

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
            name: 'Jogo criado no teste de DELETE /api/games/{id}',
            studio: 'k6 Studio',
            coverImageUrl: 'https://example.com/covers/delete-teste.jpg',
            price: 9.9,
            description: 'Jogo criado para validar o endpoint de exclusao de jogos.',
            steamLink: 'https://store.steampowered.com/app/000004',
        },
        token
    );
    check(createRes, { 'jogo de apoio criado com sucesso': (r) => r.status === 201 });

    return { token, gameId: createRes.json('id') };
}

export default function (data) {
    const deleteRes = remove(data.gameId, data.token);
    check(deleteRes, {
        'DELETE /api/games/{id} retorna 204': (r) => r.status === 204,
    });

    const getRes = getById(data.gameId, data.token);
    check(getRes, {
        'GET apos delete retorna 404': (r) => r.status === 404,
    });
}
