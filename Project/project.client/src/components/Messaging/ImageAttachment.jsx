import { useEffect, useState } from "react";
import { viewImageAttachment } from "../../services/ChatService";
import ImageModal from "../../components/ImageModal";
import PropTypes from 'prop-types';
import '../../styles/image-modal.css';

const ImageAttachment = ({ messageId, attachmentId }) => {
    const [imageUrl, setImageUrl] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedImage, setSelectedImage] = useState(null);

    useEffect(() => {
        const fetchImage = async () => {
            const url = await viewImageAttachment(messageId, attachmentId);
            if (url) setImageUrl(url);
        };
        fetchImage();
    }, [messageId, attachmentId]);

    const openModal = () => {
        setSelectedImage(imageUrl);
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setIsModalOpen(false);
        setSelectedImage(null);
    };

    return (
        <>
            {imageUrl && (
                <img
                    src={imageUrl}
                    alt="Attachment"
                    onClick={() => openModal(imageUrl)}
                    style={{
                        width: "100%",
                        height: "auto",
                        borderRadius: "5px",
                        objectFit: "contain",
                        display: "block",
                    }}
                />
            )}

            <ImageModal isOpen={isModalOpen} onCloseModal={closeModal} image={selectedImage} />
        </>
    );
        
};

ImageAttachment.propTypes = {
    messageId: PropTypes.number.isRequired,
    attachmentId: PropTypes.number.isRequired
};

export default ImageAttachment;