import { check } from 'k6';
import { login } from '../services/authService.js';
import { create } from '../services/gamesService.js';

export const options = {
    vus: 1,
    iterations: 1,
    thresholds: {
        checks: ['rate==1.0'],
    },
};

export function setup() {
    const res = login({ email: 'jogador3@k6.test', password: 'senha123' });
    check(res, { 'login retornou 200': (r) => r.status === 200 });
    return { token: res.json('token') };
}

export default function (data) {
    const res = create(
        {
            name: 'Jogo criado no teste de POST /api/games',
            studio: 'k6 Studio',
            coverImageUrl: 'https://example.com/covers/create-teste.jpg',
            price: 59.9,
            description: 'Jogo criado para validar o endpoint de criacao de jogos.',
            steamLink: 'https://store.steampowered.com/app/000001',
        },
        data.token
    );

    check(res, {
        'POST /api/games retorna 201': (r) => r.status === 201,
        'resposta contem o id do jogo criado': (r) => !!r.json('id'),
        'resposta contem o nome enviado': (r) => r.json('name') === 'Jogo criado no teste de POST /api/games',
    });
}
