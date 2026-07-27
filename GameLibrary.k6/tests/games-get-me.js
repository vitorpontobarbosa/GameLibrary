import { check } from 'k6';
import { login } from '../services/authService.js';
import { getMyGames } from '../services/gamesService.js';

export const options = {
    vus: 1,
    iterations: 1,
    thresholds: {
        checks: ['rate==1.0'],
    },
};

export function setup() {
    const res = login({ email: 'jogador2@k6.test', password: 'senha123' });
    check(res, { 'login retornou 200': (r) => r.status === 200 });
    return { token: res.json('token') };
}

export default function (data) {
    const res = getMyGames(data.token);

    check(res, {
        'GET /api/games/me retorna 200': (r) => r.status === 200,
        'GET /api/games/me retorna os jogos seedados do usuario': (r) => r.json().length >= 2,
        'GET /api/games/me retorna o preco pago por cada jogo': (r) =>
            r.json().every((game) => typeof game.pricePaid === 'number'),
    });
}
