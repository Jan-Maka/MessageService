import PropTypes from 'prop-types';
import { useState, useRef, useEffect} from 'react';
import { formatDate } from '../../utils/CommonUtils';
import MessageAttachments from "../Messaging/MessageAttachments";


const MessageBubble = ({ message, isSender, updateMessageContent, deleteMessage}) => {
    const [editModeOn, setEditMode] = useState(false);
    const [messageContent, setMessageContent] = useState(message.content);
    const [editedMessage, setEditedMessage] = useState(message.content);
    const editableRef = useRef(null);

    useEffect(() => {
        setMessageContent(message.content);
    }, [message.content]);


    const handleMessageEditMessageSave = () => {
        const updatedMessage = {
            ...message,         
            content: editedMessage 
        };
        setMessageContent(editedMessage);
        updateMessageContent(updatedMessage);
        setEditMode(false);
    };

    const handleCancelEditMessage = () => {
        setEditedMessage(messageContent);
        if (editableRef.current) {
            editableRef.current.textContent = messageContent;
        }
        setEditMode(false);
    };

    const handleDeleteMessage = () => {
        const choice = confirm("Are you sure you want to delete this message?");
        if (choice) deleteMessage(message);
        else return;
    };


    return (
        <div className={`message-bubble ${isSender ? 'message-sender' : 'message-receiver'}`}>
            <p className="message-sendername">@{message.senderName}</p>
            <MessageAttachments message={message}/>
            <p 
                className={editModeOn ? "message-edit-box" : ""} 
                contentEditable={isSender && editModeOn ? "true" : "false"} 
                suppressContentEditableWarning={true}
                ref={editableRef}
                onInput={(e) => setEditedMessage(e.currentTarget.textContent)} 
            >
                {messageContent}
            </p>
            <div className="message-footer">
                <p className="message-sent">Sent At: {formatDate(message.sentAt)} {message.edited && `(Edited at: ${formatDate(message.editedAt)})`}</p>
                {isSender && !editModeOn ? (<a className="message-edit-btn" onClick={() => setEditMode(true)}><i className="bi bi-pencil-square"></i></a>) : ("")}
                {editModeOn && (
                    <div className="edit-btns">
                        <a className="save-message-btn" onClick={() => handleMessageEditMessageSave()}><i className="bi bi-save2"></i></a>
                        <a className="delete-message-btn" onClick={() => handleDeleteMessage() }><i className="bi bi-trash"></i></a>
                        <a className="cancel-edit-message-btn" onClick={() => handleCancelEditMessage()}><i className="bi bi-x-square"></i></a>
                    </div>
                )}
            </div>
        </div>);
};

MessageBubble.propTypes = {
    message: PropTypes.shape({
        id: PropTypes.number.isRequired,
        senderId: PropTypes.number.isRequired,
        senderName: PropTypes.string.isRequired,
        content: PropTypes.string.isRequired,
        sentAt: PropTypes.string.isRequired,
        attachments: PropTypes.arrayOf(PropTypes.shape({
            id: PropTypes.number.isRequired,
            fileName: PropTypes.string.isRequired,
            contentType: PropTypes.string.isRequired,
            uploadedAt: PropTypes.string.isRequired
        })).isRequired,
        edited: PropTypes.bool.isRequired,
        editedAt: PropTypes.string
    }),
    isSender: PropTypes.bool.isRequired,
    updateMessageContent: PropTypes.func.isRequired,
    deleteMessage: PropTypes.func.isRequired
};

export default MessageBubble;