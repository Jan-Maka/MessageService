import PropTypes from 'prop-types';
import { useRef } from 'react';


const MessageInput = ({ messageInput, setMessageInput, onSendMessage, isDisabled, attachments, setAttachments }) => {
    const fileInputRef = useRef(null);

    const handleFileSelect = (e) => {
        const files = Array.from(e.target.files);
        let totalAttachSize = 0;
        attachments.forEach((f) => totalAttachSize += f.size);

        let newFileSize = 0;
        files.forEach((f) => newFileSize += f.size);

        if ((totalAttachSize + newFileSize) > 10 * 1024 * 1024) return;

        setAttachments((prevAttachments) => [...prevAttachments, ...files]);
    };

    const removeAttachment = (index) => {
        setAttachments((prevAttachments) => prevAttachments.filter((_, i) => i !== index));
    };

    return (
        <div>
            {attachments.length > 0 && (
                <div className="message-attachments">
                    {attachments.map((a, index) => (
                        <div className="attachment" key={index}>
                            <p>{a.name}</p>
                            <button 
                                type="button" 
                                className="remove-attach-btn" 
                                onClick={() => removeAttachment(index)}
                            >
                                <i className="bi bi-x"></i>
                            </button>
                        </div>
                    ))}
                </div>
            )}
            <form className="message-form" onSubmit={onSendMessage}>
                <input
                    type="file"
                    ref={fileInputRef}
                    style={{ display: "none" }}
                    onChange={handleFileSelect}
                    multiple
                />
                <input
                    type="text"
                    placeholder="Enter a message..."
                    value={messageInput}
                    onChange={(e) => setMessageInput(e.target.value)}
                    disabled={isDisabled}
                />
                <button type="button" className="btn" onClick={() => fileInputRef.current.click()} disabled={isDisabled}><i className="bi bi-file-plus"></i></button>
                <button type="submit" disabled={isDisabled}><i className="bi bi-send"></i></button>
            </form>
        </div>
    );
};

MessageInput.propTypes = {
    messageInput: PropTypes.string.isRequired,
    setMessageInput: PropTypes.func.isRequired,
    onSendMessage: PropTypes.func.isRequired,
    isDisabled: PropTypes.bool.isRequired,
    attachments: PropTypes.arrayOf(PropTypes.object).isRequired,
    setAttachments: PropTypes.func.isRequired,
};

export default MessageInput;