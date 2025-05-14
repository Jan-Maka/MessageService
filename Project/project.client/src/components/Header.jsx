import '../styles/header.css';
import { useContext } from 'react';
import { AuthContext } from '../utils/AuthContext';
function Header() {
    const { isLoggedIn, logout } = useContext(AuthContext);
    return (
        <header>
            <h1><i className="bi bi-envelope-fill"></i> Message Service</h1>
            <nav>
                <ul className="ul-start">
                    {isLoggedIn && (
                        <>
                            <li><a href="/search">Search</a></li>
                            <li><a href="/messages">Messages</a></li>
                            <li><a href="/settings">Settings</a></li>
                        </>
                    )}
                </ul>
                <ul className="ul-end">
                    {isLoggedIn ? (
                        <li><a href="#" onClick={logout}>Logout</a></li>
                    ) : (
                        <>
                            <li><a href="/login">Login</a></li>
                            <li><a href="/sign-up">Sign-Up</a></li>
                        </>
                    )}
                </ul>
            </nav>
        </header>
    );
}

export default Header;