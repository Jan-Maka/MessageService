import ChatCard from '../Messaging/ChatCard';
import PropTypes from 'prop-types';
    
const ChatList = ({ chats, currentChat, renderChat, isSearching }) => {

    return (
        <ul className={`chat-list ${isSearching ? "fade-left":""}`}>
            {chats.length > 0 ? chats.map((c) => {
                const isSelected = currentChat?.id === c.id;
                return <ChatCard key={c.id} chat={c} onClick={() => { if (!isSelected) renderChat(c); }} disabled={isSelected} />;
            }) : (
                <p>No chats found.</p>
            )}
        </ul>
    )
};


ChatList.propTypes = {
    chats: PropTypes.arrayOf(
        PropTypes.shape({
            id: PropTypes.number.isRequired,
            name: PropTypes.string,
            otherUser: PropTypes.shape({
                username: PropTypes.string
            }),
        })
    ).isRequired,
    currentChat: PropTypes.shape({
        id: PropTypes.number.isRequired
    }),
    renderChat: PropTypes.func.isRequired,
    isSearching: PropTypes.bool.isRequired
};
export default ChatList;