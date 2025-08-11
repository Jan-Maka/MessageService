import '../styles/settings.css';
import defaultPfp from '../assets/defaultPfp.jpg';
import { useState, useEffect, useContext } from 'react';
import { fetchLoggedInUser, updateUser, sendUpdateEmailVerificationCode, verifyAndUpdateUserEmail} from '../services/UserService';
import { checkPasswordAgainstEmail, checkUsernameExists, checkEmailExists } from '../services/AuthService';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../utils/AuthContext';
import ChangePasswordInput from '../components/Authentication/ChangePasswordInput';


function SettingsPage() {
    const [toggleChangeEmail, setToggleChangeEmail] = useState(false);
    const [user, setUser] = useState({});
    const [username, setUsername] = useState("");
    const [isUsernameValid, setIsUsernameValid] = useState(true);
    const [confirmPassword, setConfirmPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [rePassword, setRePassword] = useState("");
    const [passwordMatch, setPasswordsMatch] = useState(false);
    const [isPassword, setIsPassword] = useState(false);
    const [verificationCode, setVerificationCode] = useState("");
    const [newEmail, setNewEmail] = useState("");
    const [emailExists, setEmailExists] = useState(false);
    const [emailChangeFail, setEmailChangeFail] = useState(false);

    const navigate = useNavigate();

    const { logout } = useContext(AuthContext);

    useEffect(() => {
        const getUser = async () => {
            const data = await fetchLoggedInUser();
            setUser(data);
            setUsername(data.username);
        }
        getUser();
    }, []);

    const handleUsernameChange = async (e) => {
        const newUsername = e.target.value;
        setUsername(newUsername);
        const usernameExists = await checkUsernameExists(newUsername);
        if (usernameExists && newUsername !== user.username) setIsUsernameValid(false);
        else setIsUsernameValid(true);
    };

    const handleNewPasswordChange = ({newPassword, rePassword, passwordMatch}) => {
        setNewPassword(newPassword);
        setRePassword(rePassword);
        setPasswordsMatch(passwordMatch);
    };

    const handleConfirmPasswordChange = async (e) => {
        const pass = e.target.value;
        setConfirmPassword(pass);
        const isPass = await checkPasswordAgainstEmail(user.email, pass);
        setIsPassword(isPass);
    };

    const handlChangeEmailClick = async () => {
        await sendUpdateEmailVerificationCode();
        setToggleChangeEmail(true);
    };

    const handleNewEmailChange = async (e) => {
        const email = e.target.value;
        setNewEmail(email);
        setEmailExists(await checkEmailExists(email));
    };

    const handleChangeVerificationCode = async (e) => {
        const code = e.target.value;
        setVerificationCode(code);
    };

    const onSetNewEmail = async () => {
        const emailChanged = verifyAndUpdateUserEmail(newEmail, verificationCode);
        if (emailChanged) { logout(); navigate("/login"); }
        else {
            setEmailChangeFail(true);
        } 
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const updatedUser = {
            Username: username,
            Password: newPassword
        };

        const response = await updateUser(updatedUser);
        if (response && newPassword.length !== 0) {
            logout();
            navigate("/login");
        } else if (response) {
            navigate(0);
        }
    };



    return (
        <div className="settings-container fade-top">
            <div className="settings-form-container">
                <div>
                    <h1>Your Account</h1>
                </div>
                <hr/>
                <form onSubmit={handleSubmit}>  
                    <div className="profile-container">
                        <img src={defaultPfp} />
                        <h1>@{user.username}</h1>
                    </div>
                    <hr />

                    <div className="settings-field">
                        <h2>Email: {user.email}</h2>
                        {!toggleChangeEmail ? <button className="change-email-btn" type="button" onClick={() => handlChangeEmailClick()}>Change Email</button> :
                            <div className="emails-field">
                            <p>Note: After changing your email you will be logged out and prompted to login again.</p>
                                <p>Enter new email and verification code sent!</p>
                            {emailChangeFail && <p className="error-text">Verification code inputted incorrect</p>}
                            {(emailExists && newEmail.length !== 0)&& <p className="error-text">Email already in use for another account.</p> }
                            <input type="text" value={newEmail} placeholder="Email@example.com" onChange={(e) => handleNewEmailChange(e)} />
                            <input type="text" name="code" pattern="^\d{6}$" value={verificationCode} placeholder="Verification Code" maxLength="6" onChange={(e) => handleChangeVerificationCode(e)} />
                            <button className="submit-btn" type="button" disabled={!emailExists && verificationCode.length !== 6 && newEmail.length !== 0} onClick={() => onSetNewEmail()}>Set New Email</button>
                        </div>
                    }
                    </div>
                    <hr />

                    
                    <div className="settings-field">
                        <h2>Username</h2>
                        {!isUsernameValid && <p className="error-text">Error: Username already in use</p>}
                        <input type="text" placeholder="Username" value={username} onChange={(e) => handleUsernameChange(e)} />
                    </div>
                    <hr />

                    <div className="settings-field">
                        <h2>Change Password</h2>
                        <ChangePasswordInput onChange={handleNewPasswordChange} />
                    </div>
                    <hr />

                    <div className="settings-field">
                        <h2>Confirm Changes</h2>
                        <input type="password" value={confirmPassword} placeholder="Enter Password" onChange={(e) => handleConfirmPasswordChange(e)}></input>
                        <button type="submit" className="submit-btn" disabled={!isPassword || !isUsernameValid || (!passwordMatch && (newPassword.length !== 0 && rePassword.length !== 0))}>Save Changes</button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default SettingsPage;