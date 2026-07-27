import { useEffect, useState, type FormEvent } from 'react';
import { useParams } from 'react-router-dom';
import { addToCollection, getById } from '../api/games';
import type { GameDetail } from '../types';
import { useAuth } from '../context/AuthContext';

export function GameDetailPage() {
    const { token, email, isAuthenticated } = useAuth();
    const { id } = useParams<{ id: string }>();
    const gameId = Number(id);

    const [game, setGame] = useState<GameDetail | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [pricePaid, setPricePaid] = useState<string>('');
    const [submitting, setSubmitting] = useState(false);
    const [addError, setAddError] = useState<string | null>(null);

    function loadGame() {
        if (Number.isNaN(gameId)) return;
        setLoading(true);
        getById(gameId)
            .then(setGame)
            .catch((err) => setError(err instanceof Error ? err.message : 'Erro ao carregar o jogo.'))
            .finally(() => setLoading(false));
    }

    useEffect(loadGame, [gameId]);

    async function handleAddToCollection(e: FormEvent) {
        e.preventDefault();
        if (!token) return;
        setAddError(null);
        setSubmitting(true);
        try {
            await addToCollection(gameId, { pricePaid: pricePaid === '' ? undefined : Number(pricePaid) }, token);
            setPricePaid('');
            loadGame();
        } catch (err) {
            setAddError(err instanceof Error ? err.message : 'Erro ao adicionar à coleção.');
        } finally {
            setSubmitting(false);
        }
    }

    if (loading) return <p className="status-message">Carregando jogo...</p>;
    if (error) return <p className="status-message form-error">{error}</p>;
    if (!game) return <p className="status-message">Jogo não encontrado.</p>;

    const alreadyOwned = isAuthenticated && game.owners.some((owner) => owner.email === email);

    return (
        <div className="page" data-cy="game-detail-page">
            <h1 data-cy="game-detail-name">{game.name}</h1>
            {game.coverImageUrl && (
                <img src={game.coverImageUrl} alt={game.name} className="game-card-cover" style={{ maxWidth: 320 }} />
            )}
            {game.studio && <p className="game-card-studio">{game.studio}</p>}
            <p className="game-card-description" data-cy="game-detail-description">
                {game.description}
            </p>
            {game.steamLink && (
                <a href={game.steamLink} target="_blank" rel="noreferrer">
                    Steam
                </a>
            )}

            <h2>Quem já tem este jogo</h2>
            {game.owners.length === 0 ? (
                <p className="status-message" data-cy="game-owners-empty">
                    Ninguém adicionou este jogo à coleção ainda.
                </p>
            ) : (
                <ul data-cy="game-owners-list">
                    {game.owners.map((owner) => (
                        <li key={owner.userId} data-cy="game-owner-row">
                            {owner.email}
                            {owner.pricePaid != null && ` — pagou R$ ${owner.pricePaid.toFixed(2)}`}
                        </li>
                    ))}
                </ul>
            )}

            {isAuthenticated && !alreadyOwned && (
                <form onSubmit={handleAddToCollection} data-cy="add-to-collection-form">
                    <h2>Adicionar à minha coleção</h2>
                    {addError && (
                        <p className="form-error" data-cy="add-to-collection-error">
                            {addError}
                        </p>
                    )}
                    <label>
                        Quanto você pagou?
                        <input
                            type="number"
                            min={0}
                            max={1000}
                            step="0.01"
                            data-cy="add-to-collection-price"
                            value={pricePaid}
                            onChange={(e) => setPricePaid(e.target.value)}
                        />
                    </label>
                    <button type="submit" data-cy="add-to-collection-submit" disabled={submitting}>
                        {submitting ? 'Adicionando...' : 'Adicionar à minha coleção'}
                    </button>
                </form>
            )}
            {isAuthenticated && alreadyOwned && (
                <p className="status-message" data-cy="already-in-collection">
                    Este jogo já está na sua coleção.
                </p>
            )}
        </div>
    );
}
