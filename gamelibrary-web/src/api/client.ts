const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080';

export class ApiError extends Error {
    status: number;

    constructor(message: string, status: number) {
        super(message);
        this.status = status;
    }
}

function extractErrorMessage(body: unknown, fallback: string): string {
    if (typeof body === 'string' && body.trim() !== '') {
        return body;
    }
    if (body && typeof body === 'object') {
        // ASP.NET ModelState validation errors: { field: string[] }
        const messages = Object.values(body as Record<string, unknown>)
            .flat()
            .filter((v): v is string => typeof v === 'string');
        if (messages.length > 0) {
            return messages.join(' ');
        }
    }
    return fallback;
}

interface RequestOptions {
    method?: string;
    body?: unknown;
    token?: string | null;
}

export async function apiFetch<T>(path: string, options: RequestOptions = {}): Promise<T> {
    const headers: Record<string, string> = {};
    if (options.body !== undefined) {
        headers['Content-Type'] = 'application/json';
    }
    if (options.token) {
        headers['Authorization'] = `Bearer ${options.token}`;
    }

    const res = await fetch(`${BASE_URL}${path}`, {
        method: options.method ?? 'GET',
        headers,
        body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
    });

    if (!res.ok) {
        const text = await res.text().catch(() => '');
        let parsedBody: unknown = text;
        try {
            parsedBody = JSON.parse(text);
        } catch {
            // not JSON, keep the raw text
        }
        throw new ApiError(extractErrorMessage(parsedBody, res.statusText), res.status);
    }

    if (res.status === 204) {
        return undefined as T;
    }

    return (await res.json()) as T;
}
