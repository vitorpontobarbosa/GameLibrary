import { check } from 'k6';
import { login } from '../services/authService.js';

export const options = {
    vus: 1,
    iterations: 1,
    thresholds: {
        checks: ['rate==1.0'],
    },
};

export default function () {
    const res = login({
        email: 'jogador1@k6.test',
        password: 'senha123',
    });

    check(res, {
        'POST /api/auth/login retorna 200': (r) => r.status === 200,
        'resposta contem token': (r) => !!r.json('token'),
    });
}
