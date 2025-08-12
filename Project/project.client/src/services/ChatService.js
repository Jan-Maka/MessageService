import { getCsrfToken } from '../services/TokenService';


const API_BASE_URL = "/api/chat";
const API_MESSAGE_URL = "/api/message";
const token = localStorage.getItem("jwtToken");
const csrfToken = await getCsrfToken();

export const fetchAllChats = async () => {
    try {
        const response = await fetch(`${API_BASE_URL}/get-all-chats`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
        });
        return response.ok ? response.json() : [];
    } catch (error) {
        console.error("Error fetching chats:", error);
        return [];
    }
};

export const fetchChatMessagesById = async (chatId, isConversation) => {
   const CALL = isConversation ? "/get-conversation" : "/get-group-chat";
   try {
       const response = await fetch(`${API_BASE_URL}${CALL}/${chatId}/messages`, {
           method: "GET",
           headers: {
               "Content-Type": "application/json",
               "Authorization": `Bearer ${token}`,
           },
       });

       return response.ok ? response.json() : {};
   } catch (error) {
       console.error("Error fetching chat:", error);
   }
}

export const searchChats = async (query) => {
    if (!query.trim()) return fetchAllChats();

    try {
        const response = await fetch(`${API_BASE_URL}/get-chats?query=${query}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
        });

        return response.status === 204 ? [] : response.json();
    } catch (error) {
        console.error("Error searching chats:", error);
        return [];
    }
};

export const createGroupChat = async (gc) => {
    try {
        const response = await fetch(`${API_BASE_URL}/create-group-chat`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
                'X-XSRF-TOKEN': csrfToken
            },
            body: JSON.stringify(gc),
            credentials: 'include'
        });
        return await response.json();

    } catch (error) {
        console.error("Error creating chat:", error);
        return null;
    }
};

export const createAttachments = async (attachments) => {
    try {
        const formData = new FormData();
        for (let i = 0; i < attachments.length; i++) {
            formData.append("files", attachments[i], attachments[i].name);
        }
        const response = await fetch(`${API_MESSAGE_URL}/create/attachments`, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                'X-XSRF-TOKEN': csrfToken
            },
            body: formData,
            credentials: 'include'
        });
        return await response.json();
    } catch (error) {
        console.error("Error attaching files:", error);
    }

};

export const downloadAttachment = async (messageId, attachmentId) => {
    try {
        const response = await fetch(`${API_MESSAGE_URL}/download/attachment?messageId=${messageId}&attachmentId=${attachmentId}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
            },
        });

        if (!response.ok) throw new Error("Failed to download attachment");

        return await response.blob();

    } catch (error) {
        console.error("Error downloading attachment:", error);
    }

};

export const viewImageAttachment = async (messageId, attachmentId) => {
    try {
        const response = await fetch(`${API_MESSAGE_URL}/view/attachment?messageId=${messageId}&attachmentId=${attachmentId}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
            },
        });
        if (!response.ok) throw new Error("Failed to load image");
        const blob = await response.blob();
        return URL.createObjectURL(blob);
    } catch (error) {
        console.error("Error getting image:", error);
    }
};