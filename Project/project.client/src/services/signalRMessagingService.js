import * as signalR from "@microsoft/signalr";

let connection = null;

export const startConnection = async () => {
    if (connection) return;

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/chat/messages",{
            accessTokenFactory: () => localStorage.getItem("jwtToken")
        })
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect()
        .build();

    try {
        await connection.start();
        console.log("Connected to SignalR server");

        const userId = localStorage.getItem("user");
        if (userId) {
            await connection.invoke("RegisterUser", userId).then(console.log("Registred User"));

        }

    } catch (error) {
        console.error("SignalR connection error:", error);
        setTimeout(() => startConnection(), 5000);
    }
}

export const setupCallbacks = (onMessageReceived, onChatListUpdated, onMessageUpdated, onDeleteMessage) => {
    if (connection) {
        connection.off("UpdateChatList");
        connection.off("ReceiveMessage");
        connection.off("UpdateMessage");

        connection.on("UpdateChatList", (message) => {
            if (onChatListUpdated) {
                onChatListUpdated(message);
            }
        });

        connection.on("ReceiveMessage", (message) => {
            if (onMessageReceived) {
                onMessageReceived(message);
            }
        });

        connection.on("UpdateMessage", (message) => {
            if (onMessageUpdated) {
                onMessageUpdated(message);
            }
        });

        connection.on("DeleteMessage", (message) => {
            if (onDeleteMessage) {
                onDeleteMessage(message);
            }
        });
    } else {
        console.error("No SignalR connection available to register callbacks.");
    }
};


export const joinChat = async (chatId, isConversation) => {
    if (connection) {
        await connection.invoke("JoinChat", chatId, isConversation)
            .then(() => console.log(`Joined chat ${chatId}`))
            .catch(err => console.error("Error joining conversation:", err));
    }
};

export const leaveChat = async (chatId, isConversation) => {
    if (connection) {
        await connection.invoke("LeaveChat", chatId, isConversation)
            .then(() => console.log(`Left chat ${chatId}`))
            .catch(err => console.error("Error leaving conversation:", err));
    }
}

export const sendMessage = async (message, isConversation) => {
    if (connection) {
        if (isConversation) await connection.invoke("SendMessageToConversation", message.ConversationId, message).catch(err => console.error("Error sending message: ", err));
        else await connection.invoke("SendMessageToGroupChat", message.GroupChatId, message).catch(err => console.error("Error sending message: ", err));
    }
};

export const updateMessage = async (chatId ,message, isConversation) => {
    if (connection) {
        await connection.invoke("UpdateMessageContent", chatId, isConversation, message).catch(err => console.error("Error updating message content: ", err));
    }
};

export const deleteMessage = async (chatId, isConversation, message) => {
    if (connection) {
        await connection.invoke("DeleteMessage", chatId, isConversation, message).catch(err => console.error("Error deleting message: ", err));
    }
};


export const stopConnection = async () => {
    if (connection) {
        await connection.stop();
        connection = null;
        console.log("SignalR connection stopped");
    }
};