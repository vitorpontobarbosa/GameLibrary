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
                <Link to="/" data-cy="nav-games">
                    Jogos
                </Link>
                {isAuthenticated && (
                    <Link to="/my-games" data-cy="nav-my-games">
                        Meus jogos
                    </Link>
                )}
                {isAuthenticated && (
                    <Link to="/games/new" data-cy="nav-new-game">
                        Novo jogo
                    </Link>
                )}
            </nav>
            <div className="navbar-auth">
                {isAuthenticated ? (
                    <>
                        <span className="navbar-email" data-cy="nav-email">
                            {email}
                        </span>
                        <button type="button" data-cy="nav-logout" onClick={handleLogout}>
                            Sair
                        </button>
                    </>
                ) : (
                    <>
                        <Link to="/login" data-cy="nav-login">
                            Entrar
                        </Link>
                        <Link to="/register" data-cy="nav-register">
                            Criar conta
                        </Link>
                    </>
                )}
            </div>
        </header>
    );
}
