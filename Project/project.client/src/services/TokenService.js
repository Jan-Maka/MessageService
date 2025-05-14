export async function getCsrfToken() {

    try {
        const response = await fetch('/api/authentication/csrf-token', {
            method: 'GET',
            credentials: 'include',
        });

        if (response.ok) {
            const data = await response.json();
            return data.token;  // The CSRF token assigned to the current session
        }
    } catch (error) {
        console.error("Error fetching XRSF-TOKEN: ", error);
    }

};