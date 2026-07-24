import { useEffect, useState } from 'react';
import { getAll } from '../api/games';
import type { Game } from '../types';
import { GameCard } from '../components/GameCard';

export function GamesListPage() {
    const [games, setGames] = useState<Game[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        getAll()
            .then(setGames)
            .catch((err) => setError(err instanceof Error ? err.message : 'Erro ao carregar jogos.'))
            .finally(() => setLoading(false));
    }, []);

    if (loading) return <p className="status-message">Carregando jogos...</p>;
    if (error) return <p className="status-message form-error">{error}</p>;

    return (
        <div className="page" data-cy="games-list-page">
            <h1>Todos os jogos</h1>
            {games.length === 0 ? (
                <p className="status-message" data-cy="games-empty">
                    Nenhum jogo cadastrado ainda.
                </p>
            ) : (
                <div className="games-grid">
                    {games.map((game) => (
                        <GameCard key={game.id} game={game} />
                    ))}
                </div>
            )}
        </div>
    );
}
