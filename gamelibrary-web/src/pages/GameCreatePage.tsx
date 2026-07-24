import { useNavigate } from 'react-router-dom';
import { create } from '../api/games';
import type { CreateGameRequest } from '../types';
import { GameForm } from '../components/GameForm';
import { useAuth } from '../context/AuthContext';

export function GameCreatePage() {
    const { token } = useAuth();
    const navigate = useNavigate();

    async function handleSubmit(payload: CreateGameRequest) {
        if (!token) return;
        await create(payload, token);
        navigate('/my-games');
    }

    return (
        <div className="page">
            <h1>Novo jogo</h1>
            <GameForm submitLabel="Criar jogo" onSubmit={handleSubmit} />
        </div>
    );
}
