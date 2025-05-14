import PropTypes from 'prop-types';
import '../../styles/chat-card.css';
import { formatDate } from '../../utils/CommonUtils';

const ChatCardContainer = ({ chat, onClick, disabled }) => {
    const isConversation = !!chat.otherUser;
    const chatTitle = isConversation ? `@${chat.otherUser.username}` : chat.name;
    const lastMessage = chat.messages.length > 0 ? chat.messages[chat.messages.length - 1] : null;
    const lastMessageText = lastMessage ? `${lastMessage.senderName}: ${lastMessage.content.slice(0,20)}...` : "No messages have been sent!" ;

    return (
        <div className={`chat-card-container ${disabled ? 'disabled' : ''}`} onClick={!disabled ? onClick : undefined}>
            <div className="chat-card">
                <h2>{chatTitle}</h2>
                <p className="latest-message-sent">{lastMessageText}</p>
                <p className="last-message-sent">Sent: {formatDate(chat.lastMessageReceived)}</p>
            </div>
        </div>
    );
};

ChatCardContainer.propTypes = {
    chat: PropTypes.shape({
        id: PropTypes.number.isRequired,
        name: PropTypes.string,
        otherUser: PropTypes.shape({
            username: PropTypes.string.isRequired
        }),
        messages: PropTypes.arrayOf(
            PropTypes.shape({
                id: PropTypes.number.isRequired,
                senderId: PropTypes.number.isRequired,
                senderName: PropTypes.string.isRequired,
                content: PropTypes.string.isRequired,
                sentAt: PropTypes.string.isRequired,
            })
        ).isRequired,
        lastMessageReceived: PropTypes.string.isRequired,
    }).isRequired,
    onClick: PropTypes.func.isRequired,
    disabled: PropTypes.bool.isRequired
};

export default ChatCardContainer;
