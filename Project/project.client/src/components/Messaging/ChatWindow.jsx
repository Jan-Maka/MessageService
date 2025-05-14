import MessageBubble from "./MessageBubble";
import PropTypes from "prop-types";


const ChatWindow = ({ messages, userId, chatContainerRef, attachmentsActive, updateMessageContent, deleteMessage }) => {
    return (
        <div id="chatMessages" className={`conversation-messages ${attachmentsActive ? "attachments-active":""}`} ref={chatContainerRef} >
                {messages.length === 0 ? (
                    <p className="text-grey message-error">No messages have been sent!</p>
                ) : (
                    messages.map((m) => (
                        <MessageBubble key={m.id} message={m} isSender={m.senderId === userId} updateMessageContent={updateMessageContent} deleteMessage={deleteMessage} />
                    ))
                )}
            </div>
    );
};

ChatWindow.propTypes = {
    messages: PropTypes.arrayOf(
        PropTypes.shape({
            id: PropTypes.number.isRequired,
            senderId: PropTypes.number.isRequired,
            content: PropTypes.string.isRequired,
            sentAt: PropTypes.string.isRequired,
        })
    ).isRequired,
    userId: PropTypes.number.isRequired,
    chatContainerRef: PropTypes.object.isRequired,
    attachmentsActive: PropTypes.bool.isRequired,
    updateMessageContent: PropTypes.func.isRequired,
    deleteMessage: PropTypes.func.isRequired
};

export default ChatWindow;