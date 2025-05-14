import '../styles/messages.css';
import ChatList from "../components/Messaging/ChatList";
import ChatSearch from "../components/Messaging/ChatSearch";
import ChatWindow from "../components/Messaging/ChatWindow";
import MessageInput from "../components/Messaging/MessageInput";
import CreateGroupChatModal from "../components/Messaging/CreateGroupChatModal";
import { useState, useEffect, useRef } from 'react';
import { startConnection, setupCallbacks, joinChat, leaveChat, sendMessage, updateMessage, deleteMessage,stopConnection } from '../services/signalRMessagingService';
import { fetchAllChats, searchChats, createAttachments  } from '../services/ChatService';

function MessagesPage() {
    const [chats, setChats] = useState([]);
    const [currentChat, setCurrentChat] = useState(null);
    const [messages, setMessages] = useState([]);
    const [messageInput, setMessageInput] = useState("");
    const [isConversation, setIsConversation] = useState(false);
    const [searchQuery, setSearchQuery] = useState("");
    const [searched, setSearched] = useState(false);
    const [attachments, setAttachments] = useState([]);
    const [chatChanged, setChatChanged] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const chatContainerRef = useRef(null);
    const messagesRef = useRef([]);

    useEffect(() => {
        const setUp = async () => {
            await loadChats();
            await startConnection();
        }
        setUp();

        return () => { stopConnection()};
    }, []);

    useEffect(() => {
        const onMessageReceived = async (message) => {
            if ((message.conversationId === currentChat.id && isConversation) ||
                (message.groupChatId === currentChat.id && !isConversation)) {
                setMessages((prevMessags) => [...prevMessags, message]);
            }
        };

        const onChatListUpdated = (message) => {
            setChats((chats) => {
                const isGroup = message.groupChatId !== null;
                const chatId = message.conversationId || message.groupChatId;
                const chat = chats.find(chat => chat.id === chatId && ((isGroup && !chat.otherUser) || (!isGroup && chat.otherUser)));
                if (!chat) {
                    return chats;
                }
                const updatedChat = {
                    ...chat,
                    lastMessageReceived: message.sentAt,
                    messages: [...chat.messages, message]
                };
                const updatedChats = chats.filter(c => c !== chat);
                return [updatedChat, ...updatedChats];
            });
        }

        const onMessageUpdated = (message) => {
            if ((message.conversationId === currentChat.id && isConversation) ||
                (message.groupChatId === currentChat.id && !isConversation)) {
                setMessages((prevMessages) => [...prevMessages.map(msg =>
                    msg.id === message.id ? { ...msg, ...message } : msg
                )]);
            }

            setChats((chats) => {
                const isGroup = message.groupChatId !== null;
                const chatId = message.conversationId || message.groupChatId;

                const chat = chats.find(chat =>
                    chat.id === chatId && ((isGroup && !chat.otherUser) || (!isGroup && chat.otherUser))
                ); 
                if (!chat) return chats; 

                const messageIndex = chat.messages.findIndex(msg => msg.id === message.id);

                if (messageIndex === -1) return chats;

                chat.messages[messageIndex] = message;

                return chats;
            });
        };

        const onDeleteMessage = (message) => {
            if ((message.conversationId === currentChat.id && isConversation) ||
                (message.groupChatId === currentChat.id && !isConversation)) {
                setMessages((prevMessages) => prevMessages.filter(msg => msg.id !== message.id));
                messagesRef.current = messages;
            }

            setChats((chats) => {
                const isGroup = message.groupChatId !== null;
                const chatId = message.conversationId || message.groupChatId;

                const updatedChats = chats.map(chat => {
                    if (chat.id === chatId && ((isGroup && !chat.otherUser) || (!isGroup && chat.otherUser))) {
                        const updatedMessages = chat.messages.filter(msg => msg.id !== message.id);
                        return {
                            ...chat, 
                            messages: updatedMessages
                        };
                    }
                    return chat; 
                });

                return updatedChats;
            });

        };

        setupCallbacks(onMessageReceived, onChatListUpdated, onMessageUpdated, onDeleteMessage);

    }, [currentChat, isConversation]);

    useEffect(() => {
        if (messagesRef.current.length === messages.length) return;
        else if (messagesRef.current.length > messages.length) {
            messagesRef.current = messages;
            return;
        }

        messagesRef.current = messages;

        const intervalId = setInterval(() => {
            if (chatContainerRef.current) {
                chatContainerRef.current.scrollTop = chatContainerRef.current.scrollHeight;
                clearInterval(intervalId); 
            }
        }, 250); 

        return () => clearInterval(intervalId);
    }, [messages]);

    useEffect(() => {
        if (searchQuery.trim() === "") handleSearched(); loadChats();
    }, [searchQuery]);

    const handleSearched = () => {
        setSearched(true);
        setTimeout(() => setSearched(false), 1000);
    };

    const handleChatChanged = () => {
        setChatChanged(true);
        setTimeout(() => setChatChanged(false), 1000);
    };

    const loadChats = async () => {
        const data = await fetchAllChats();
        setChats(data);
    };

    const handleChatSearch = async (e) => {
        e.preventDefault();
        const data = await searchChats(searchQuery);
        setChats(data);
        handleSearched();
    };

    const renderChat = (chat) => {
        if (!chat) return;

        if (currentChat) {
            leaveChat(currentChat.id, isConversation);
        }

        const isConv = chat.otherUser !== undefined;
        if (currentChat?.id !== chat.id) {
            setCurrentChat(chat);
        }
        if (isConversation !== isConv) {
            setIsConversation(isConv);
        }
        if (messages !== chat.messages) {
            setMessages(chat.messages);
        }
        setAttachments([]);

        handleChatChanged();

        if (currentChat?.id !== chat.id) {
            joinChat(chat.id, isConv);
        }
    };

    const handleSendMessage = async (e) => {
        e.preventDefault();
        if (messageInput.trim() === "") return;
        let files = [];
        if (attachments.length > 0) {
            files = await createAttachments(attachments);
        }
        setAttachments([]);
        const message = {
            Id:0,
            Content: messageInput,
            SenderId: parseInt(localStorage.getItem("user")),
            SenderName: localStorage.getItem("username"),
            SentAt: new Date(),
            ConversationId: isConversation ? currentChat.id : null,
            GroupChatId: !isConversation ? currentChat.id : null,
            Attachments: files,
            Edited: false,
            EditedAt: null
        };
        await sendMessage(message, isConversation);
        setMessageInput("");
    };

    const handleMessageContentUpdate = async (updatedMessage) => {
        if (updatedMessage.content.trim() === "") return;
        updateMessage(currentChat.id, updatedMessage, isConversation);
    };

    const handleDeleteMessage = async (message) => {
        await deleteMessage(currentChat.id, isConversation, message);
    };

    const handleOnCreateGroup = (chat) => {
        setChats(prevChats => [...prevChats, chat]);
    };

    return (
        <div className="page-container fade-left">
            <div className="chats-container">
                <div className="chat-list-header">
                    <h1><i className="bi bi-chat-left-text-fill"></i> Messages</h1>
                    <button onClick={() => setIsModalOpen(true)}><i className="bi bi-plus-square"></i></button>
                </div>
                <div className="chat-list-container">
                    <ChatSearch searchQuery={searchQuery} setSearchQuery={setSearchQuery} onSearch={handleChatSearch} />
                    <ChatList chats={chats} currentChat={currentChat} renderChat={renderChat} isSearching={searched} />
                </div>
            </div>
            <div className="chat-container">
                <div id="chatHeader" className={`conversation-header ${chatChanged ? "fade-top" : ""}`}>
                    <h1>{currentChat ? (isConversation ? `@${currentChat.otherUser.username}` : currentChat.name) : "Direct Messaging"}</h1>
                </div>
                <div className={`conversation-container ${chatChanged ? "fade-top" : ""}`}>
                    <ChatWindow messages={messages} userId={parseInt(localStorage.getItem("user"))} chatContainerRef={chatContainerRef} attachmentsActive={attachments.length > 0} updateMessageContent={handleMessageContentUpdate} deleteMessage={handleDeleteMessage} />
                    <MessageInput messageInput={messageInput} setMessageInput={setMessageInput} onSendMessage={handleSendMessage} isDisabled={!currentChat} attachments={attachments} setAttachments={setAttachments} />
                </div>
            </div>
            <CreateGroupChatModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} onGroupCreated={handleOnCreateGroup} />
        </div>
    );
}

export default MessagesPage;