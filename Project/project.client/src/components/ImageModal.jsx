import PropTypes from 'prop-types';


const ImageModal = ({ isOpen, onCloseModal, image }) => {
    if (!isOpen) return null;
    return (
        <div className="image-modal" onClick={onCloseModal}>
            <div className="image-content fade-top">
                <img
                    src={image}
                    alt="Full size"
                />
            </div>
        </div>);

};

ImageModal.propTypes = {
    isOpen: PropTypes.bool.isRequired,
    onCloseModal: PropTypes.func.isRequired,
    image: PropTypes.string
}

export default ImageModal;