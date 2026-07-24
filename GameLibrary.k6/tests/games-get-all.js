import { check } from 'k6';
import { getAll } from '../services/gamesService.js';

export const options = {
    vus: 1,
    iterations: 1,
    thresholds: {
        checks: ['rate==1.0'],
    },
};

export default function () {
    const res = getAll();

    check(res, {
        'GET /api/games retorna 200': (r) => r.status === 200,
        'GET /api/games retorna uma lista': (r) => Array.isArray(r.json()),
        'GET /api/games contem os jogos seedados': (r) => r.json().length >= 6,
    });
}
