import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getById, update } from '../api/games';
import type { CreateGameRequest, Game } from '../types';
import { GameForm } from '../components/GameForm';
import { useAuth } from '../context/AuthContext';

function toFormValues(game: Game): CreateGameRequest {
    return {
        name: game.name,
        studio: game.studio ?? '',
        coverImageUrl: game.coverImageUrl ?? '',
        price: game.price ?? undefined,
        description: game.description,
        steamLink: game.steamLink,
    };
}

export function GameEditPage() {
    const { token } = useAuth();
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const gameId = Number(id);

    const [game, setGame] = useState<Game | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!token || Number.isNaN(gameId)) return;
        getById(gameId, token)
            .then(setGame)
            .catch((err) => setError(err instanceof Error ? err.message : 'Erro ao carregar o jogo.'))
            .finally(() => setLoading(false));
    }, [token, gameId]);

    async function handleSubmit(payload: CreateGameRequest) {
        if (!token) return;
        await update(gameId, payload, token);
        navigate('/my-games');
    }

    if (loading) return <p className="status-message">Carregando jogo...</p>;
    if (error) return <p className="status-message form-error">{error}</p>;
    if (!game) return <p className="status-message">Jogo não encontrado.</p>;

    return (
        <div className="page">
            <h1>Editar jogo</h1>
            <GameForm initialValues={toFormValues(game)} submitLabel="Salvar alterações" onSubmit={handleSubmit} />
        </div>
    );
}
