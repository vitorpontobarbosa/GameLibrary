import { check } from 'k6';
import { register } from '../services/authService.js';

export const options = {
    vus: 1,
    iterations: 1,
    thresholds: {
        checks: ['rate==1.0'],
    },
};

export default function () {
    const res = register({
        email: `novo-usuario-${Date.now()}@k6.test`,
        password: 'senha123',
    });

    check(res, {
        'POST /api/auth/register retorna 200': (r) => r.status === 200,
        'resposta contem token': (r) => !!r.json('token'),
    });
}
