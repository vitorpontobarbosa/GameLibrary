import { apiFetch } from './client';
import type { AuthResponse, LoginRequest, RegisterRequest } from '../types';

export function register(payload: RegisterRequest): Promise<AuthResponse> {
    return apiFetch<AuthResponse>('/api/auth/register', { method: 'POST', body: payload });
}

export function login(payload: LoginRequest): Promise<AuthResponse> {
    return apiFetch<AuthResponse>('/api/auth/login', { method: 'POST', body: payload });
}
