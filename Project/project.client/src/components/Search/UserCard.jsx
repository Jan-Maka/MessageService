import PropTypes from 'prop-types';
import { useState } from 'react';
import '../../styles/user-search-card.css';
import { sendFriendRequest, removeFriend, deleteFriendRequest, acceptFriendRequest } from '../../services/UserFriendService';

const UserCardContainer = ({ user }) => {

    const [userState, setUserState] = useState(user);

    const handleSendFriendRequest = async (userId) => {
        const requestId = await sendFriendRequest(userId);
        setUserState((prevState) => ({
            ...prevState,
            sentFriendRequest: true,
            friendRequestId: requestId
        }));
    };

    const handleRemoveFriend = async (userId) => {
        if (await removeFriend(userId)) {
            setUserState((prevState) => ({
                ...prevState,
                sentFriendRequest: false,
                receivedFriendRequest: false,
                isFriend: false
            }));
        }
    };

    const handleDeleteFriendRequest = async (requestId) => {
        if (await deleteFriendRequest(requestId)) {
            setUserState((prevState) => ({
                ...prevState,
                sentFriendRequest: false,
                receivedFriendRequest: false,
            }));
        }

    };
    const handleAcceptFriendRequest = async (requestId) => {
        if (await acceptFriendRequest(requestId)) {
            setUserState((prevState) => ({
                ...prevState,
                receivedFriendRequest: false,
                isFriend: true,
            }));
        }
    };

    const handleButton = () => {
        if (userState.id === parseInt(localStorage.getItem("user"))) {
            return (<></>);
        } else if (userState.isFriend) {
            return (<button className="danger-btn" onClick={() => handleRemoveFriend(userState.id) }>Remove Friend</button>);
        } else if (userState.sentFriendRequest) {
            return (<button className="danger-btn" onClick={() => handleDeleteFriendRequest(userState.friendRequestId)}>Cancel Friend Request</button>);
        } else if (userState.receivedFriendRequest) {
            return (<div>
                        <button className="success-btn" onClick={() => handleAcceptFriendRequest(userState.friendRequestId) }>Accept Friend Request</button>
                        <button className="danger-btn" onClick={() => handleDeleteFriendRequest(userState.friendRequestId)}>Deny Friend Request</button>
                    </div>);
        }
        return (<button className="success-btn" onClick={() => handleSendFriendRequest(userState.id)}>Send Friend Request</button>);
    }


    return (
        <div className="user-card-container">
            <div className="user-card-details">
                <h1>{user.username}</h1>
                {handleButton()}
            </div>
        </div>
    );
};

UserCardContainer.propTypes = {
    user: PropTypes.shape({
        id: PropTypes.number.isRequired,
        username: PropTypes.string.isRequired,
        isFriend: PropTypes.bool.isRequired,
        sentFriendRequest: PropTypes.bool.isRequired,
        receivedFriendRequest: PropTypes.bool.isRequired,
        friendRequestId: PropTypes.oneOfType([PropTypes.number, PropTypes.oneOf([null])]),
    }).isRequired,
};

export default UserCardContainer;