import http from 'k6/http';
import { BASE_URL } from '../config.js';
import { jsonHeaders } from '../helpers/http.js';

export function getAll() {
    return http.get(`${BASE_URL}/api/games`, {
        headers: jsonHeaders(),
    });
}

export function getById(id, token) {
    return http.get(`${BASE_URL}/api/games/${id}`, {
        headers: jsonHeaders(token),
    });
}

export function getMyGames(token) {
    return http.get(`${BASE_URL}/api/games/me`, {
        headers: jsonHeaders(token),
    });
}

export function create(payload, token) {
    return http.post(`${BASE_URL}/api/games`, JSON.stringify(payload), {
        headers: jsonHeaders(token),
    });
}

export function update(id, payload, token) {
    return http.put(`${BASE_URL}/api/games/${id}`, JSON.stringify(payload), {
        headers: jsonHeaders(token),
    });
}

export function remove(id, token) {
    return http.del(`${BASE_URL}/api/games/${id}`, null, {
        headers: jsonHeaders(token),
    });
}

export function addToCollection(id, payload, token) {
    return http.post(`${BASE_URL}/api/games/${id}/collection`, JSON.stringify(payload), {
        headers: jsonHeaders(token),
    });
}
