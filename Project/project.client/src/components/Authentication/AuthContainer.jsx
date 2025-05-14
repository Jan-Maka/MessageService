//import React from 'react';
import PropTypes from 'prop-types';
import '../../styles/auth-container.css';

const AuthContainer = ({ title, children }) => {
    return (
        <div className="auth-page">
            <div className="auth-container">
                <div className="form-container">
                    <h1>{title}</h1>
                    <div className="auth-form">
                        {children}
                    </div>
                </div>
            </div>
        </div>
    );
};

AuthContainer.propTypes = {
    title: PropTypes.string.isRequired,
    children: PropTypes.node.isRequired,
}

export default AuthContainer;