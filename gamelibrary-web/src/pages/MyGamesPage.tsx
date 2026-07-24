import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getMyGames, remove } from '../api/games';
import type { Game } from '../types';
import { GameCard } from '../components/GameCard';
import { useAuth } from '../context/AuthContext';

export function MyGamesPage() {
    const { token } = useAuth();
    const navigate = useNavigate();
    const [games, setGames] = useState<Game[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!token) return;
        getMyGames(token)
            .then(setGames)
            .catch((err) => setError(err instanceof Error ? err.message : 'Erro ao carregar seus jogos.'))
            .finally(() => setLoading(false));
    }, [token]);

    async function handleDelete(game: Game) {
        if (!token) return;
        if (!confirm(`Excluir "${game.name}"?`)) return;
        try {
            await remove(game.id, token);
            setGames((current) => current.filter((g) => g.id !== game.id));
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Erro ao excluir o jogo.');
        }
    }

    if (loading) return <p className="status-message">Carregando seus jogos...</p>;

    return (
        <div className="page" data-cy="my-games-page">
            <h1>Meus jogos</h1>
            {error && (
                <p className="form-error" data-cy="my-games-error">
                    {error}
                </p>
            )}
            {games.length === 0 ? (
                <p className="status-message" data-cy="my-games-empty">
                    Você ainda não cadastrou nenhum jogo.
                </p>
            ) : (
                <div className="games-grid">
                    {games.map((game) => (
                        <GameCard
                            key={game.id}
                            game={game}
                            onEdit={(g) => navigate(`/games/${g.id}/edit`)}
                            onDelete={handleDelete}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}
