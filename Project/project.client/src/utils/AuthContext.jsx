import { createContext, useState, useEffect } from 'react';
import PropTypes from 'prop-types';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [user, setUser] = useState(null);

    useEffect(() => {
        const token = localStorage.getItem("jwtToken");
        setIsLoggedIn(!!token);
    }, []);


    const login = (data) => {
        localStorage.setItem("jwtToken", data.token);
        localStorage.setItem("user", data.user.id);
        localStorage.setItem("username", data.user.username);
        setUser(data.user);
        setIsLoggedIn(true);
    };

    const logout = () => {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem("user");
        setIsLoggedIn(false);
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ isLoggedIn, user, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

AuthProvider.propTypes = {
    children: PropTypes.node.isRequired,
};