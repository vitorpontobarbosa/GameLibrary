import { createContext, useContext, useMemo, useState, type ReactNode } from 'react';
import { login as loginRequest, register as registerRequest } from '../api/auth';
import type { LoginRequest, RegisterRequest } from '../types';

const STORAGE_KEY = 'gamelibrary.token';

interface DecodedToken {
    email?: string;
    sub?: string;
    nameid?: string;
    [key: string]: unknown;
}

function decodeToken(token: string): DecodedToken | null {
    try {
        const payload = token.split('.')[1];
        const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
        const json = decodeURIComponent(
            atob(base64)
                .split('')
                .map((c) => `%${c.charCodeAt(0).toString(16).padStart(2, '0')}`)
                .join('')
        );
        return JSON.parse(json) as DecodedToken;
    } catch {
        return null;
    }
}

function emailFromToken(token: string | null): string | null {
    if (!token) return null;
    const decoded = decodeToken(token);
    if (!decoded) return null;
    return (
        (decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] as string | undefined) ??
        decoded.email ??
        null
    );
}

interface AuthContextValue {
    token: string | null;
    email: string | null;
    isAuthenticated: boolean;
    login: (payload: LoginRequest) => Promise<void>;
    register: (payload: RegisterRequest) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [token, setToken] = useState<string | null>(() => localStorage.getItem(STORAGE_KEY));

    const value = useMemo<AuthContextValue>(
        () => ({
            token,
            email: emailFromToken(token),
            isAuthenticated: token !== null,
            login: async (payload) => {
                const res = await loginRequest(payload);
                localStorage.setItem(STORAGE_KEY, res.token);
                setToken(res.token);
            },
            register: async (payload) => {
                const res = await registerRequest(payload);
                localStorage.setItem(STORAGE_KEY, res.token);
                setToken(res.token);
            },
            logout: () => {
                localStorage.removeItem(STORAGE_KEY);
                setToken(null);
            },
        }),
        [token]
    );

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
    const ctx = useContext(AuthContext);
    if (!ctx) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return ctx;
}
