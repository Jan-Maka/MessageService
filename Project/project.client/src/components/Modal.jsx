import PropTypes from 'prop-types';
import '../styles/modal.css';

const ModalContainer = ({ isOpen,onClose,header, body, footer }) => {
    if (!isOpen) return null;
    return (
        <div className="modal">
            <div className="modal-content fade-top">
                <div className="modal-header">
                    {header}
                    <button className="modal-close-btn" onClick={onClose}><i className="bi bi-x"></i></button>
                </div>
                <div className="modal-body">
                    {body}
                </div>
                {footer && <div className="modal-footer">
                    {footer}
                </div>
                }
            </div>
        </div>
    );
};

ModalContainer.propTypes = {
    isOpen: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    header: PropTypes.node.isRequired,
    body: PropTypes.node.isRequired,
    footer: PropTypes.node
};

export default ModalContainer;