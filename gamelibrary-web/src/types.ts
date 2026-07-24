export interface Game {
    id: number;
    name: string;
    studio?: string | null;
    coverImageUrl?: string | null;
    price?: number | null;
    description: string;
    steamLink: string;
    ownerId: number;
}

export interface CreateGameRequest {
    name: string;
    studio?: string | null;
    coverImageUrl?: string | null;
    price?: number | null;
    description: string;
    steamLink: string;
}

export type UpdateGameRequest = Partial<CreateGameRequest>;

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
}

export interface AuthResponse {
    token: string;
    message: string;
}
