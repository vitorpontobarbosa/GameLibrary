export interface Game {
    id: number;
    name: string;
    studio?: string | null;
    coverImageUrl?: string | null;
    price?: number | null;
    description: string;
    steamLink: string;
    createdByUserId: number;
}

export interface GameOwner {
    userId: number;
    email: string;
    pricePaid?: number | null;
    addedAt: string;
}

export interface GameDetail extends Game {
    owners: GameOwner[];
}

export interface MyGame extends Game {
    pricePaid?: number | null;
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

export interface AddToCollectionRequest {
    pricePaid?: number | null;
}

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
