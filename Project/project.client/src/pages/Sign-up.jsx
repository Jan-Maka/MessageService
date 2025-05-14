import { useState, useEffect } from 'react';
import AuthContainer from '../components/Authentication/AuthContainer';
import { checkEmailExists, checkUsernameExists, createAccount } from '../services/AuthService';
import { useNavigate } from 'react-router-dom';


function SignUp() {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [username, setUsername] = useState('');
    const [repPassword, setRepPassword] = useState('');
    const [emailError, setEmailError] = useState('');
    const [usernameError, setUsernameError] = useState('');
    const [passwordError, setPasswordError] = useState('');
    const [isEmailValid, setIsEmailValid] = useState(false);
    const [isUsernameValid, setIsUsernameValid] = useState(false);
    const [isPasswordValid, setIsPasswordValid] = useState(false);
    const [isPasswordMatch, setIsPasswordMatch] = useState(false);
    const [isFormValid, setIsFormValid] = useState(false);

    useEffect(() => {
        setIsFormValid(isEmailValid && isUsernameValid && isPasswordValid && isPasswordMatch);
    }, [isEmailValid, isUsernameValid, isPasswordValid, isPasswordMatch]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        createAccount({
            Email: email,
            Username: username,
            Password: password
        });
        navigate("/login");
    };

    const setError = (errorType, message) => {
        if (errorType === 'email') {
            setEmailError(message);
            setIsEmailValid(false);
        } else if (errorType === 'username') {
            setUsernameError(message);
            setIsUsernameValid(false);
        } else if (errorType === 'password') {
            setPasswordError(message);
            setIsPasswordValid(false);
        }
    };

    const handleEmailChange = async (e) => {
        const newEmail = e.target.value;
        setEmail(newEmail);

        const emailExists = await checkEmailExists(newEmail);
        if (emailExists) {
            setError('email', 'Account with email already exists!');
            setIsEmailValid(false);
        } else {
            setEmailError('');
            setIsEmailValid(true);
        }
        
    };

    const handleUsernameChange = async (e) => {
        const newUsername = e.target.value;
        setUsername(newUsername);

        const usernameExists = await checkUsernameExists(newUsername);
        if (usernameExists) {
            setError('username', 'Account with username already exists!');
            setIsUsernameValid(false);
        } else {
            setUsernameError('');
            setIsUsernameValid(true);
        }
    };

    const handlePasswordChange = (e) => {
        const newPassword = e.target.value;
        setPassword(newPassword);

        if (newPassword.length < 6) {
            setPasswordError('Password must be at least 6 characters!');
            setIsPasswordValid(false);
        }else {
            setPasswordError('');
            setIsPasswordValid(true);
        }

        if (newPassword != repPassword && repPassword.length !== 0) {
            setPasswordError('Error: Passwords do not match!');
            setIsPasswordMatch(false);
        } else {
            setPasswordError('');
            setIsPasswordMatch(true);
        }
    };

    const handleRepPasswordChange = (e) => {
        const newRepPassword = e.target.value;
        setRepPassword(newRepPassword);

        if (password && newRepPassword !== password) {
            setPasswordError('Error: Passwords do not match!');
            setIsPasswordMatch(false);
        } else {
            setPasswordError('');
            setIsPasswordMatch(true);
        }
    };

    return (
        <AuthContainer title="Sign-Up">
            <form onSubmit={handleSubmit}>
                {emailError && <p className="error">{emailError}</p>}
                <input type="email" placeholder="Enter Email" value={email} onChange={handleEmailChange} required/>

                {usernameError && <p className="error">{usernameError}</p>}
                <input type="text" placeholder="Enter Username" value={username} onChange={handleUsernameChange} required />

                {passwordError && <p className="error">{passwordError}</p>}
                <input type="password" placeholder="Enter Password" value={password} onChange={handlePasswordChange} required />

                <input type="password" placeholder="Repeat Password" value={repPassword} onChange={handleRepPasswordChange} required />

                <button type="submit" disabled={!isFormValid}>Create Account</button>
            </form>
        </AuthContainer>
    );
}

export default SignUp;
