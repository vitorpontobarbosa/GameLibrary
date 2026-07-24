import { useState, type FormEvent } from 'react';
import type { CreateGameRequest } from '../types';

interface GameFormProps {
    initialValues?: CreateGameRequest;
    submitLabel: string;
    onSubmit: (payload: CreateGameRequest) => Promise<void>;
}

const emptyValues: CreateGameRequest = {
    name: '',
    studio: '',
    coverImageUrl: '',
    price: undefined,
    description: '',
    steamLink: '',
};

export function GameForm({ initialValues, submitLabel, onSubmit }: GameFormProps) {
    const [values, setValues] = useState<CreateGameRequest>(initialValues ?? emptyValues);
    const [error, setError] = useState<string | null>(null);
    const [submitting, setSubmitting] = useState(false);

    async function handleSubmit(e: FormEvent) {
        e.preventDefault();
        setError(null);
        setSubmitting(true);
        try {
            await onSubmit(values);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Erro ao salvar o jogo.');
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <form className="game-form" onSubmit={handleSubmit} data-cy="game-form">
            {error && (
                <p className="form-error" data-cy="game-form-error">
                    {error}
                </p>
            )}

            <label>
                Nome
                <input
                    required
                    minLength={2}
                    data-cy="game-name"
                    value={values.name}
                    onChange={(e) => setValues({ ...values, name: e.target.value })}
                />
            </label>

            <label>
                Estúdio
                <input
                    data-cy="game-studio"
                    value={values.studio ?? ''}
                    onChange={(e) => setValues({ ...values, studio: e.target.value })}
                />
            </label>

            <label>
                URL da capa
                <input
                    type="url"
                    required
                    data-cy="game-cover"
                    value={values.coverImageUrl ?? ''}
                    onChange={(e) => setValues({ ...values, coverImageUrl: e.target.value })}
                />
            </label>

            <label>
                Preço
                <input
                    type="number"
                    min={0}
                    max={1000}
                    step="0.01"
                    data-cy="game-price"
                    value={values.price ?? ''}
                    onChange={(e) =>
                        setValues({ ...values, price: e.target.value === '' ? undefined : Number(e.target.value) })
                    }
                />
            </label>

            <label>
                Descrição
                <textarea
                    required
                    minLength={5}
                    data-cy="game-description"
                    value={values.description}
                    onChange={(e) => setValues({ ...values, description: e.target.value })}
                />
            </label>

            <label>
                Link da Steam
                <input
                    type="url"
                    required
                    data-cy="game-steamlink"
                    value={values.steamLink}
                    onChange={(e) => setValues({ ...values, steamLink: e.target.value })}
                />
            </label>

            <button type="submit" data-cy="game-submit" disabled={submitting}>
                {submitting ? 'Salvando...' : submitLabel}
            </button>
        </form>
    );
}
