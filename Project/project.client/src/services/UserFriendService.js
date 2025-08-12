import { getCsrfToken } from '../services/TokenService';

const API_BASE_URL = "/api/user";
const token = localStorage.getItem("jwtToken");
const csrfToken = await getCsrfToken();

export const sendFriendRequest = async (userId) => {
    try {
        const response = await fetch(`${API_BASE_URL}/send-friend-request?userId=${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
                'X-XSRF-TOKEN': csrfToken
            },
            credentials: 'include'
        });
        if (response.ok) return await response.json();
        
    } catch (error) {
        console.error("Error Occurred: ", error);
        return null;
    }
};

export const removeFriend = async (userId) => {
    try {
        const response = await fetch(`${API_BASE_URL}/remove-friend?userId=${userId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
                'X-XSRF-TOKEN': csrfToken
            },
            credentials: 'include'
        });

        if (response.ok)  return true;
        
    } catch (error) {
        console.error("Error Occurred: ", error);
        return false;
    }
};

export const deleteFriendRequest = async (requestId) => {
    try {
        const response = await fetch(`${API_BASE_URL}/delete-friend-request?requestId=${requestId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
                'X-XSRF-TOKEN': csrfToken
            },
            credentials: 'include'
        });
        

        if (response.ok) return true;
    } catch (error) {
        console.error("Error Occurred: ", error);
        return false;
    }
};

export const acceptFriendRequest = async (requestId) => {
    try {
        const response = await fetch(`${API_BASE_URL}/accept-friend-request?requestId=${requestId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
                'X-XSRF-TOKEN': csrfToken
            },
            credentials: 'include'
        });

        if (response.ok) return true;
    } catch (error) {
        console.error("Error Occurred: ", error);
        return false;
    }
};

