import { check } from 'k6';
import { login } from '../services/authService.js';
import { create, update, getById } from '../services/gamesService.js';

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
            name: 'Jogo criado no teste de PUT /api/games/{id}',
            studio: 'k6 Studio',
            coverImageUrl: 'https://example.com/covers/update-teste.jpg',
            price: 29.9,
            description: 'Jogo criado para validar o endpoint de atualizacao de jogos.',
            steamLink: 'https://store.steampowered.com/app/000003',
        },
        token
    );
    check(createRes, { 'jogo de apoio criado com sucesso': (r) => r.status === 201 });

    return { token, gameId: createRes.json('id') };
}

export default function (data) {
    const updateRes = update(
        data.gameId,
        {
            name: 'Jogo atualizado no teste de PUT /api/games/{id}',
            studio: 'k6 Studio Atualizado',
            coverImageUrl: 'https://example.com/covers/update-teste-v2.jpg',
            price: 34.9,
            description: 'Descricao atualizada para validar o endpoint de atualizacao de jogos.',
            steamLink: 'https://store.steampowered.com/app/000003',
        },
        data.token
    );

    check(updateRes, {
        'PUT /api/games/{id} (payload completo) retorna 204': (r) => r.status === 204,
    });

    const getRes = getById(data.gameId, data.token);
    check(getRes, {
        'GET apos update retorna 200': (r) => r.status === 200,
        'GET apos update reflete os novos dados': (r) =>
            r.json('name') === 'Jogo atualizado no teste de PUT /api/games/{id}' && r.json('price') === 34.9,
    });

    // Update parcial: so envia price. Antes do fix no CreateMap<UpdateGameRequest, Game>
    // (GameProfile.cs), isso zerava Name/Description (null) e quebrava a constraint
    // NOT NULL do SQLite (500). Agora os campos ausentes devem ser preservados.
    const partialUpdateRes = update(data.gameId, { price: 39.9 }, data.token);
    check(partialUpdateRes, {
        'PUT /api/games/{id} (payload parcial) retorna 204': (r) => r.status === 204,
    });

    const getAfterPartialRes = getById(data.gameId, data.token);
    check(getAfterPartialRes, {
        'GET apos update parcial mantem o nome': (r) =>
            r.json('name') === 'Jogo atualizado no teste de PUT /api/games/{id}',
        'GET apos update parcial aplica o novo preco': (r) => r.json('price') === 39.9,
    });
}
