import AuthContainer from "../components/Authentication/AuthContainer";
import { useState } from "react";
import { checkEmailExists, sendResetPasswordEmail } from "../services/AuthService";

function ForgotPasswordPage() {
    const [email, setEmail] = useState("");
    const [isEmailValid, setIsEmailValid] = useState(false);

    const handleEmailChange = async (e) => {
        const newEmail = e.target.value;
        setEmail(newEmail);

        const emailExists = await checkEmailExists(newEmail);
        if (emailExists) {
            setIsEmailValid(true);
        } else {
            setIsEmailValid(false);
        }
    }

    const handleSubmit = async (e) => {
        e.preventDefault();

        emailSent = await sendResetPasswordEmail(email);
        if( emailSent ) {
            alert("Reset password email sent successfully!");
        }
        else {
            alert("Failed to send reset password email. Please try again.");
        }
        setEmail("");
        setIsEmailValid(false);
    }

    return (
        <AuthContainer title="Send Reset Password Email">
            <form onSubmit={handleSubmit}>
                <input value={email} type="email" onChange={(e) => handleEmailChange(e)} placeholder="Enter email of account you want to change password for." />
                <button className="btn btn-primary" type="submit" disabled={!isEmailValid}>Send Reset Password</button>
            </form>
        </AuthContainer>
    );
}

export default ForgotPasswordPage;