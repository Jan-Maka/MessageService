import {useState} from 'react';
import PropTypes from 'prop-types';

const ChangePasswordInput = ({ onChange}) => {
    const [newPassword, setNewPassword] = useState("");
    const [rePassword, setRePassword] = useState("");
    const [passwordMatch, setPasswordsMatch] = useState(false);

    const handleNewPasswordChange = (e) => {
        const newPass = e.target.value;
        const match = rePassword && newPass === rePassword;
        setNewPassword(newPass);
        setPasswordsMatch(match);
        onChange({ newPassword: newPass, rePassword, passwordMatch: match });
    };

    const handleRePasswordChange = (e) => {
        const rePass = e.target.value;
        const match = newPassword && rePass === newPassword;
        setRePassword(rePass);
        setPasswordsMatch(match);
        onChange({ newPassword, rePassword: rePass, passwordMatch: match });
    };


    return (
    <>
        {(!passwordMatch && newPassword.length !== 0) && <p className="error-text">Error: Passwords dont match</p>}
        <input type="password" value={newPassword} placeholder="New Password" onChange={handleNewPasswordChange} />
        <input type="password" value={rePassword} placeholder="Repeat Password" onChange={handleRePasswordChange} />
    </>
    );
}

ChangePasswordInput.propTypes = {
    onChange: PropTypes.func.isRequired,
};

export default ChangePasswordInput;