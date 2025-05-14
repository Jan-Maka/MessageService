const API_BASE_URL = "/api/user";
const token = localStorage.getItem("jwtToken");
import { getCsrfToken } from '../services/TokenService';

const csrfToken = await getCsrfToken();

export const fetchLoggedInUser = async () => {
    try {
        const response = await fetch(`${API_BASE_URL}/logged-in`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
        });
        return await response.json();
    } catch (error) {
        console.error("Error retrieving logged in user: ", error);
    }
};

export const updateUser = async (user) => {
    try {
        const response = await fetch(`api/authentication/update/user`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                "Authorization": `Bearer ${token}`,
                'X-XRSF-TOKEN': csrfToken
            },
            body: JSON.stringify(user),
            credentials: 'include'
        });

        if (await response.ok) return true;
    } catch (error) {
        console.error("Error updating user details: ", error);
        return false;
    }
};

export const sendUpdateEmailVerificationCode = async () => {
    try {
        const response = await fetch(`api/authentication/send/user-verification-code`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                "Authorization": `Bearer ${token}`,
                'X-XRSF-TOKEN': csrfToken
            },
            credentials: 'include'
        });
        if (await response.ok) return;
    } catch (error) {
        console.error("Error sending email verification code: ", error);
    }
};

export const verifyAndUpdateUserEmail = async (newEmail, code) => {
    try {
        const response = await fetch(`api/authentication/verify/change-email?code=${code}&newEmail=${newEmail}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                "Authorization": `Bearer ${token}`,
                'X-XRSF-TOKEN': csrfToken
            },
            credentials: 'include'
        });

        if (response.ok) return true;
        else return false;
    } catch (error) {
        console.error("Error verifying token:  ", error);
        return false;
    }
};
