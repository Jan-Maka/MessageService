
const checkEmailExists = async (email) => {
    try {
        const response = await fetch(`api/authentication/check/email?email=${email}`);
        if (!response.ok) {
            throw new Error("Email check failed!");
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Error checking email:", error);
    }
};

const checkUsernameExists = async (username) => {
    try {
        const response = await fetch(`api/authentication/check/username?username=${username}`);
        if (!response.ok) {
            throw new Error("Username check failed!");
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Error checking username:", error);
    }
};

const checkPasswordAgainstEmail = async (email, password) => {
    try {
        const response = await fetch(`api/authentication/check/password/email`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({Email: email, Password: password})
        });
        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Error checking Password against email: ", error);
    }
};

const createAccount = async (user) => {
    try {
        const response = await fetch(`api/authentication/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(user),
        });
        if (!response.ok) {
            throw new Error("User creation failed!");
        }
    } catch (error) {
        console.error("Error creating user:", error);
    }
};

const loginUser = async (user) => {
    try {
        const response = await fetch(`api/authentication/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(user),
        });
        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Error authenticating user login:", error);
    }
};


export {
    checkEmailExists,
    checkUsernameExists,
    checkPasswordAgainstEmail,
    createAccount,
    loginUser,
};