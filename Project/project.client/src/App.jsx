import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import Header from './components/Header';
import SignUp from './pages/Sign-up';
import Login from './pages/Login';
import MessagesPage from './pages/MessagesPage';
import SearchPage from './pages/SearchPage';
import SettingsPage from './pages/SettingsPage';
import ForgotPasswordPage from './pages/ForgotPasswordPage';
import ResetPasswordPage from './pages/ResetPaswordPage';
import { AuthProvider } from './utils/AuthContext';
import { SocketProvider } from './utils/SocketContext';
function App() {
    return (
        <AuthProvider>
            <SocketProvider>
                <Router>
                <Header />
                <Routes>
                    <Route path="/sign-up" element={<SignUp />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/messages" element={<MessagesPage />} />
                    <Route path="/search" element={<SearchPage />} />
                    <Route path="/settings" element={<SettingsPage/>} />
                    <Route path="/forgot-password" element={<ForgotPasswordPage />}/>
                    <Route path="/reset-password" element={<ResetPasswordPage />} />

                </Routes>
            </Router>
            </SocketProvider>
        </AuthProvider>
    );
}

export default App;