import AuthContainer from '../components/Authentication/AuthContainer';
import { useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { checkEmailExists, checkPasswordAgainstEmail, loginUser } from '../services/AuthService';
import { AuthContext } from '../utils/AuthContext';

function Login() {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [emailError, setEmailError] = useState('');
    const [passwordError, setPasswordError] = useState('');
    const [emailValid, setIsEmailValid] = useState(false);
    const [passwordValid, setIsPasswordValid] = useState(false);
    const [formValid, setIsFormValid] = useState(false);
    const { login } = useContext(AuthContext);


    const handleOnSubmit = async (e) => {
        e.preventDefault();
        const data = await loginUser({
            Email: email,
            Password: password
        });
        login(data);
        navigate("/messages");
    };

    useEffect(() => {
        setIsFormValid(emailValid && passwordValid);
    }, [emailValid, passwordValid]); 

    const handleEmailChange = async (e) => {
        const newEmail = e.target.value;
        setEmail(newEmail);

        const emailExists = await checkEmailExists(newEmail);
        if (!emailExists) {
            setEmailError("Account with email doesn't exist!");
            setIsEmailValid(false);
        } else {
            setEmailError("");
            setIsEmailValid(true);
        }

    };

    const handlePasswordChange = async (e) => {
        const newPassword = e.target.value;
        setPassword(newPassword);

        const passwordValidAgainstEmail = await checkPasswordAgainstEmail(email, newPassword);
        if (passwordValidAgainstEmail) {
            setIsPasswordValid(true);
            setPasswordError("");
        } else {
            setIsPasswordValid(false);
            setPasswordError("Password doesn't match with email for account!");
        }
    };


    return (
        <AuthContainer title="Login">
            <form onSubmit={handleOnSubmit}>
                {emailError && <p className="error">{emailError}</p>}
                <input type="email" placeholder="Enter email" value={email} onChange={handleEmailChange}></input>
                {passwordError && <p className="error">{passwordError}</p>}
                <input type="password" placeholder="Enter password" value={password} onChange={handlePasswordChange} disabled={!emailValid}></input>
                <a className='link' href="forgot-password">Forgot Password?</a>
                <button className='btn btn-success' disabled={!formValid}>Login</button>
            </form>
        </AuthContainer>);
}

export default Login;