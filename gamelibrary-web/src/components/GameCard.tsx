import { Link } from 'react-router-dom';
import type { Game, MyGame } from '../types';

interface GameCardProps {
    game: Game | MyGame;
    onEdit?: (game: Game) => void;
    onDelete?: (game: Game) => void;
}

function isMyGame(game: Game | MyGame): game is MyGame {
    return 'pricePaid' in game;
}

export function GameCard({ game, onEdit, onDelete }: GameCardProps) {
    const pricePaid = isMyGame(game) ? game.pricePaid : null;

    return (
        <article className="game-card" data-cy="game-card" data-cy-name={game.name}>
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
                <h3 data-cy="game-card-name">
                    <Link to={`/games/${game.id}`} data-cy="game-card-link">
                        {game.name}
                    </Link>
                </h3>
                {game.studio && <p className="game-card-studio">{game.studio}</p>}
                <p className="game-card-description">{game.description}</p>
                <div className="game-card-meta">
                    {pricePaid != null ? (
                        <span className="game-card-price" data-cy="game-card-price-paid">
                            Paguei: R$ {pricePaid.toFixed(2)}
                        </span>
                    ) : (
                        game.price != null && <span className="game-card-price">R$ {game.price.toFixed(2)}</span>
                    )}
                    {game.steamLink && (
                        <a href={game.steamLink} target="_blank" rel="noreferrer">
                            Steam
                        </a>
                    )}
                </div>
                {(onEdit || onDelete) && (
                    <div className="game-card-actions">
                        {onEdit && (
                            <button type="button" data-cy="game-card-edit" onClick={() => onEdit(game)}>
                                Editar
                            </button>
                        )}
                        {onDelete && (
                            <button
                                type="button"
                                className="danger"
                                data-cy="game-card-delete"
                                onClick={() => onDelete(game)}
                            >
                                Excluir
                            </button>
                        )}
                    </div>
                )}
            </div>
        </article>
    );
}
