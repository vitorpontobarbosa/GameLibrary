import http from 'k6/http';
import { BASE_URL } from '../config.js';
import { jsonHeaders } from '../helpers/http.js';

export function register(payload) {
    return http.post(`${BASE_URL}/api/auth/register`, JSON.stringify(payload), {
        headers: jsonHeaders(),
    });
}

export function login(payload) {
    return http.post(`${BASE_URL}/api/auth/login`, JSON.stringify(payload), {
        headers: jsonHeaders(),
    });
}
