import AuthContainer from '../components/Authentication/AuthContainer';
import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { checkPasswordResetToken, resetUserPassword } from "../services/AuthService";
import ChangePasswordInput from '../components/Authentication/ChangePasswordInput';

function ResetPasswordPage() {
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token");
    const [newPassword, setNewPassword] = useState("");
    const [rePassword, setRePassword] = useState("");
    const [passwordsMatch, setPasswordsMatch] = useState(false);
    const [isFormValid, setIsFormValid] = useState(false);
    const navigate = useNavigate();

    const handleNewPasswordChange = ({newPassword, rePassword, passwordMatch}) => {
        setNewPassword(newPassword);
        setRePassword(rePassword);
        setPasswordsMatch(passwordMatch);
    };

    useEffect(() => {
        if(passwordsMatch && newPassword.length > 6 && rePassword.length > 6){
            setIsFormValid(true);
        }else{
            setIsFormValid(false);
        }
    },[passwordsMatch])

    useEffect(() => {
        if (!token) {
            navigate("/login");
        }

        const checkTokenValidity = async () => {
            const isTokenValid = await checkPasswordResetToken(token);
            if (!isTokenValid) navigate("/login");
        }

        checkTokenValidity();
    }, [token]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        const result = await resetUserPassword({
            Token: token,
            NewPassword: newPassword
        });
        if (result) {
            navigate("/login");
        } else {
            alert("Password reset failed due to an invalid token or server error.");
            navigate("/login");
        }
    
    }

    return (
        <AuthContainer title="Reset Password">
            <form onSubmit={handleSubmit}>
                <ChangePasswordInput onChange={handleNewPasswordChange} />
                <button className="btn btn-success" type="submit" disabled={!isFormValid}>Reset Password</button>
            </form>
        </AuthContainer>
    );
}

export default ResetPasswordPage;