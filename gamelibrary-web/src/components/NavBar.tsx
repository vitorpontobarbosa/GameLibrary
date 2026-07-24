import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export function NavBar() {
    const { isAuthenticated, email, logout } = useAuth();
    const navigate = useNavigate();

    function handleLogout() {
        logout();
        navigate('/');
    }

    return (
        <header className="navbar">
            <Link to="/" className="navbar-brand">
                GameLibrary
            </Link>
            <nav className="navbar-links">
                <Link to="/">Jogos</Link>
                {isAuthenticated && <Link to="/my-games">Meus jogos</Link>}
                {isAuthenticated && <Link to="/games/new">Novo jogo</Link>}
            </nav>
            <div className="navbar-auth">
                {isAuthenticated ? (
                    <>
                        <span className="navbar-email">{email}</span>
                        <button type="button" onClick={handleLogout}>
                            Sair
                        </button>
                    </>
                ) : (
                    <>
                        <Link to="/login">Entrar</Link>
                        <Link to="/register">Criar conta</Link>
                    </>
                )}
            </div>
        </header>
    );
}
