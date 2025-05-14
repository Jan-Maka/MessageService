import PropTypes from 'prop-types';

const ChatSearch = ({searchQuery, setSearchQuery, onSearch }) => {
    return (
        <form className="search-form" onSubmit={onSearch}>
            <input type="search" value={searchQuery} placeholder="Chat" onChange={(e) => setSearchQuery(e.target.value)} />
            <button type="submit"><i className="bi bi-search"></i></button>
        </form>
    );
};

ChatSearch.propTypes = {
    searchQuery: PropTypes.string.isRequired,
    setSearchQuery: PropTypes.func.isRequired,
    onSearch: PropTypes.func.isRequired
};

export default ChatSearch;