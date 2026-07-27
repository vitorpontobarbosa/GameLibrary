import { apiFetch } from './client';
import type {
    AddToCollectionRequest,
    CreateGameRequest,
    Game,
    GameDetail,
    MyGame,
    UpdateGameRequest,
} from '../types';

export function getAll(): Promise<Game[]> {
    return apiFetch<Game[]>('/api/games');
}

export function getById(id: number): Promise<GameDetail> {
    return apiFetch<GameDetail>(`/api/games/${id}`);
}

export function getMyGames(token: string): Promise<MyGame[]> {
    return apiFetch<MyGame[]>('/api/games/me', { token });
}

export function create(payload: CreateGameRequest, token: string): Promise<Game> {
    return apiFetch<Game>('/api/games', { method: 'POST', body: payload, token });
}

export function update(id: number, payload: UpdateGameRequest, token: string): Promise<void> {
    return apiFetch<void>(`/api/games/${id}`, { method: 'PUT', body: payload, token });
}

export function remove(id: number, token: string): Promise<void> {
    return apiFetch<void>(`/api/games/${id}`, { method: 'DELETE', token });
}

export function addToCollection(id: number, payload: AddToCollectionRequest, token: string): Promise<Game> {
    return apiFetch<Game>(`/api/games/${id}/collection`, { method: 'POST', body: payload, token });
}
