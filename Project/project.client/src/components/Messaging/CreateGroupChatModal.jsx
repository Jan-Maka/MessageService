import Modal from '../../components/Modal';
import '../../styles/groupChatModal.css';
import PropTypes from "prop-types";
import { useState, useEffect } from 'react';
import { fetchUserFriends, fetchUserFriendsFromQuery } from '../../services/UserSearchService';
import { createGroupChat } from '../../services/ChatService';

const CreateGroupChatModal = ({ isOpen, onClose, onGroupCreated }) => {
    const [friends, setFriends] = useState([]);
    const [filteredFriends, setFilteredFriends] = useState([]);
    const [selectedFriends, setSelectedFriends] = useState([]);
    const [groupName, setGroupName] = useState("");
    const [userSearch, setUserSearch] = useState("");

    useEffect(() => {
        if (isOpen) {
            const loadFriends = async () => {
                const data = await fetchUserFriends();
                setFriends(data);
                setFilteredFriends(data);
                setSelectedFriends([]);
            };
            loadFriends();
        }
    }, [isOpen]);

    useEffect(() => {
        if (userSearch === "") setFilteredFriends(friends.filter((u) => !selectedFriends.find((uf) => uf.id === u.id)));
    }, [userSearch]);

    const handleUserSearch = async () => {
        if (userSearch.trim() === "") {
            setFilteredFriends(friends);
            return;
        }
        let data = await fetchUserFriendsFromQuery(userSearch);
        data = data.filter((u) => !selectedFriends.find((uf) => uf.id === u.id));
        setFilteredFriends(data);
    };

    const handleUserSelect = (user) => {
        setSelectedFriends((prev) =>
            prev.some((u) => u.id === user.id) ? prev.filter((u) => u.id !== user.id) : [...prev, user]
        );
        setFilteredFriends((prev) =>
            prev.some((u) => u.id === user.id) ? prev.filter((u) => u.id !== user.id) : [...prev, user]
        );
    };

    const handleGroupCreate = async (e) => {
        e.preventDefault();
        if (!groupName.trim() || selectedFriends.length < 2) return;
        const group = await createGroupChat({
            name: groupName,
            users: selectedFriends,
        });

        if (group) {
            onGroupCreated(group);
            onClose();
        }
    };


    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            header={
            <h2>Create Group-Chat</h2>
        }
            body={
                <form onSubmit={handleGroupCreate}>
                    <input className="form-chat-input" type="text" placeholder="Group Name" value={groupName} onChange={(e) => setGroupName(e.target.value)}></input>
                <h3>Users in chat</h3>
                <div className="users-list">
                        {selectedFriends.map((u) => (
                            <div className="user-friend" key={u.id}> 
                                <p>@{u.username}</p>
                                <a className="btn btn-danger" onClick={() => handleUserSelect(u)}><i className="bi bi-person-dash"></i></a>
                            </div>
                        ))}
                </div>

                <h3>Users to add</h3>
                <div className="friend-search">
                    <input className="form-chat-input" type="search" placeholder="Search User" value={userSearch} onChange={(e) => setUserSearch(e.target.value) }></input>
                    <a className="user-search-btn btn" onClick={handleUserSearch}><i className="bi bi-search"/></a>
                </div>
                <div className="users-list">
                    {filteredFriends.map((u) => (
                            <div className="user-friend" key={u.id}> 
                                <p>@{u.username}</p>
                                <a className="btn btn-success" onClick={() => handleUserSelect(u)}><i className="bi bi-person-add"></i></a>
                            </div>
                    ))}
                </div>
                <button className="btn btn-success" type="submit">Create Chat</button>
            </form>
        } />
    );
};

CreateGroupChatModal.propTypes = {
    isOpen: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    onGroupCreated: PropTypes.func.isRequired
};

export default CreateGroupChatModal;