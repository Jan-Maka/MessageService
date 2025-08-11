import { useState, useEffect } from 'react';
import AuthContainer from '../components/Authentication/AuthContainer';
import { checkEmailExists, checkUsernameExists, createAccount } from '../services/AuthService';
import { useNavigate } from 'react-router-dom';
import ChangePasswordInput from '../components/Authentication/ChangePasswordInput';


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

    useEffect(() => {
        if(isPasswordMatch && password.length > 6 && repPassword.length > 6) setIsPasswordValid(true);
        else if(isPasswordMatch && (password.length < 6 || repPassword.length < 6)) {
            setIsPasswordValid(false);
            const error = 'Password must be at least 6 characters!';
            setPasswordError(error);
            
        }else setPasswordError('');

    },[isPasswordMatch]);

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

    const handlePasswordChange = ({newPassword, rePassword, passwordMatch}) => {
        setPassword(newPassword);
        setRepPassword(rePassword);
        setIsPasswordMatch(passwordMatch);
    }

    return (
        <AuthContainer title="Sign-Up">
            <form onSubmit={handleSubmit}>
                {emailError && <p className="error">{emailError}</p>}
                <input type="email" placeholder="Enter Email" value={email} onChange={handleEmailChange} required/>

                {usernameError && <p className="error">{usernameError}</p>}
                <input type="text" placeholder="Enter Username" value={username} onChange={handleUsernameChange} required />

                {passwordError && <p className="error">{passwordError}</p>}
                <ChangePasswordInput onChange={handlePasswordChange} />

                <button className='btn btn-success' type="submit" disabled={!isFormValid}>Create Account</button>
            </form>
        </AuthContainer>
    );
}

export default SignUp;
