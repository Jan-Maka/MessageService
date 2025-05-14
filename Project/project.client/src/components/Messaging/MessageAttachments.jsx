import PropTypes from 'prop-types';
import { downloadAttachment } from '../../services/ChatService';
import ImageAttachment from "../Messaging/ImageAttachment";

const MessageAttachments = ({ message }) => {


    const handleDownloadAttachment = async (attachmentId, fileName) => {
        const blob = await downloadAttachment(message.id, attachmentId);
        const blobUrl = window.URL.createObjectURL(blob);

        const link = document.createElement("a");
        link.href = blobUrl;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(blobUrl);
    };

    return (
        <div>
            {
                message.attachments.length > 0 ? (
                    <>
                        <div className="message-images-container">
                            {message.attachments.filter(a => a.contentType.startsWith("image/")).map((a) => (
                                <div key={a.id} className="message-image-attachment" style={{ flex: "1 0 20%" }}>
                                    <ImageAttachment messageId={message.id} attachmentId={a.id} />
                                </div>
                            ))}
                        </div>

                        <div className="message-files-container">
                            {message.attachments.filter(a => !a.contentType.startsWith("image/")).map((a) => (
                                <div key={a.id} className="message-attachment">
                                    <a onClick={() => handleDownloadAttachment(a.id, a.fileName)} className="download-link">
                                        <i className="bi bi-file-earmark-arrow-down"></i>
                                        <p>{a.fileName}</p>
                                    </a>
                                </div>
                            ))}
                        </div>
                    </>
                ) : ""
            }
        </div>)
};

export default MessageAttachments;


MessageAttachments.propTypes = {
    message: PropTypes.shape({
        id: PropTypes.number.isRequired,
        attachments: PropTypes.arrayOf(PropTypes.shape({
            id: PropTypes.number.isRequired,
            fileName: PropTypes.string.isRequired,
            contentType: PropTypes.string.isRequired,
            uploadedAt: PropTypes.string.isRequired
        })).isRequired,
    })
};

