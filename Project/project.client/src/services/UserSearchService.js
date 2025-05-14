const API_BASE_URL = "/api/user";
const token = localStorage.getItem("jwtToken");

export const fetchUsersFromSearchQuery = async (query) => {
    try {
        const response = await fetch(`${API_BASE_URL}/search?query=${query}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
        });
        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Error Occurred: ", error);
        return [];
    }
};

export const fetchUserFriends = async () => {
    try {
        const response = await fetch(`${API_BASE_URL}/user-friends`, {
            method: "GET",
            headers: {
                "Content-Type": 'application/json',
                "Authorization": `Bearer ${token}`,
            },
        });
        const data = await response.json();
        return data;

    } catch (error) {
        console.error("Error occurred fetching user friends: ", error);
        return [];
    }
};

export const fetchUserFriendsFromQuery = async (query) => {
    try {
        const response = await fetch(`${API_BASE_URL}/user-friends-search?query=${query}`, {
            method: "GET",
            headers: {
                "Content-Type": 'application/json',
                "Authorization": `Bearer ${token}`,
            },
        });
        return await response.json();
    } catch (error) {
        console.error("Error occurred fetching user friends: ", error);
        return [];
    }
};