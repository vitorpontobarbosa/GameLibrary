import type { Game } from '../types';

interface GameCardProps {
    game: Game;
    onEdit?: (game: Game) => void;
    onDelete?: (game: Game) => void;
}

export function GameCard({ game, onEdit, onDelete }: GameCardProps) {
    return (
        <article className="game-card">
            {game.coverImageUrl && (
                <img
                    src={game.coverImageUrl}
                    alt={game.name}
                    className="game-card-cover"
                    onError={(e) => {
                        e.currentTarget.style.display = 'none';
                    }}
                />
            )}
            <div className="game-card-body">
                <h3>{game.name}</h3>
                {game.studio && <p className="game-card-studio">{game.studio}</p>}
                <p className="game-card-description">{game.description}</p>
                <div className="game-card-meta">
                    {game.price != null && <span className="game-card-price">R$ {game.price.toFixed(2)}</span>}
                    {game.steamLink && (
                        <a href={game.steamLink} target="_blank" rel="noreferrer">
                            Steam
                        </a>
                    )}
                </div>
                {(onEdit || onDelete) && (
                    <div className="game-card-actions">
                        {onEdit && (
                            <button type="button" onClick={() => onEdit(game)}>
                                Editar
                            </button>
                        )}
                        {onDelete && (
                            <button type="button" className="danger" onClick={() => onDelete(game)}>
                                Excluir
                            </button>
                        )}
                    </div>
                )}
            </div>
        </article>
    );
}
