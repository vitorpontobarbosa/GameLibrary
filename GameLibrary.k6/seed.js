import { check, fail } from 'k6';
import { register } from './services/authService.js';
import { create } from './services/gamesService.js';

const usuarios = JSON.parse(open('./data/usuarios.json'));
const games = JSON.parse(open('./data/games.json'));

export default function () {
    const tokenByEmail = {};

    usuarios.forEach((usuario) => {
        const res = register(usuario);
        const ok = check(res, {
            [`usuario ${usuario.email} registrado`]: (r) => r.status === 200,
        });
        if (!ok) {
            fail(`Falha ao registrar ${usuario.email}: status ${res.status} - ${res.body}`);
        }
        tokenByEmail[usuario.email] = res.json('token');
    });

    games.forEach((game) => {
        const { owner, ...payload } = game;
        const token = tokenByEmail[owner];
        if (!token) {
            fail(`Nenhum token disponivel para o owner "${owner}" do jogo "${game.name}"`);
        }

        const res = create(payload, token);
        check(res, {
            [`jogo ${game.name} criado`]: (r) => r.status === 201,
        });
    });

    console.log(`Seed concluido: ${usuarios.length} usuarios e ${games.length} jogos inseridos.`);
}
