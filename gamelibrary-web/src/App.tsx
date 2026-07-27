import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { NavBar } from './components/NavBar';
import { GamesListPage } from './pages/GamesListPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { MyGamesPage } from './pages/MyGamesPage';
import { GameCreatePage } from './pages/GameCreatePage';
import { GameEditPage } from './pages/GameEditPage';
import { GameDetailPage } from './pages/GameDetailPage';

function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <NavBar />
                <main className="app-main">
                    <Routes>
                        <Route path="/" element={<GamesListPage />} />
                        <Route path="/login" element={<LoginPage />} />
                        <Route path="/register" element={<RegisterPage />} />
                        <Route
                            path="/my-games"
                            element={
                                <ProtectedRoute>
                                    <MyGamesPage />
                                </ProtectedRoute>
                            }
                        />
                        <Route
                            path="/games/new"
                            element={
                                <ProtectedRoute>
                                    <GameCreatePage />
                                </ProtectedRoute>
                            }
                        />
                        <Route
                            path="/games/:id/edit"
                            element={
                                <ProtectedRoute>
                                    <GameEditPage />
                                </ProtectedRoute>
                            }
                        />
                        <Route path="/games/:id" element={<GameDetailPage />} />
                    </Routes>
                </main>
            </AuthProvider>
        </BrowserRouter>
    );
}

export default App;
