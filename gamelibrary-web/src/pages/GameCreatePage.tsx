import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { addToCollection, create, getAll } from '../api/games';
import type { CreateGameRequest, Game } from '../types';
import { GameForm } from '../components/GameForm';
import { useAuth } from '../context/AuthContext';

function toFormValues(game: Game): CreateGameRequest {
    return {
        name: game.name,
        studio: game.studio ?? '',
        coverImageUrl: game.coverImageUrl ?? '',
        price: undefined,
        description: game.description,
        steamLink: game.steamLink,
    };
}

const CATALOG_READ_ONLY_FIELDS: Array<keyof CreateGameRequest> = [
    'name',
    'studio',
    'coverImageUrl',
    'description',
    'steamLink',
];

export function GameCreatePage() {
    const { token } = useAuth();
    const navigate = useNavigate();
    const [existingGames, setExistingGames] = useState<Game[]>([]);
    const [selectedGameId, setSelectedGameId] = useState('');

    useEffect(() => {
        getAll()
            .then(setExistingGames)
            .catch(() => setExistingGames([]));
    }, []);

    const selectedGame = existingGames.find((game) => game.id === Number(selectedGameId)) ?? null;

    async function handleSubmit(payload: CreateGameRequest) {
        if (!token) return;
        if (selectedGame) {
            await addToCollection(selectedGame.id, { pricePaid: payload.price }, token);
            navigate(`/games/${selectedGame.id}`);
        } else {
            await create(payload, token);
            navigate('/my-games');
        }
    }

    return (
        <div className="page">
            <h1>Novo jogo</h1>
            <label>
                Esse jogo já existe na base?
                <select
                    data-cy="game-existing-select"
                    value={selectedGameId}
                    onChange={(e) => setSelectedGameId(e.target.value)}
                >
                    <option value="">-- Cadastrar um jogo novo --</option>
                    {existingGames.map((game) => (
                        <option key={game.id} value={game.id}>
                            {game.name}
                        </option>
                    ))}
                </select>
            </label>
            <GameForm
                key={selectedGame ? selectedGame.id : 'new'}
                initialValues={selectedGame ? toFormValues(selectedGame) : undefined}
                readOnlyFields={selectedGame ? CATALOG_READ_ONLY_FIELDS : undefined}
                priceLabel={selectedGame ? 'Quanto você pagou?' : 'Preço'}
                submitLabel={selectedGame ? 'Adicionar à minha coleção' : 'Criar jogo'}
                onSubmit={handleSubmit}
            />
        </div>
    );
}
