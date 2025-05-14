import "../styles/search.css";
import { useState } from 'react';
import UserCard from '../components/Search/UserCard';
import { fetchUsersFromSearchQuery } from '../services/UserSearchService';

function SearchPage() {
    const [search, SetSearch] = useState("");
    const [results, setResults] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setResults(await fetchUsersFromSearchQuery(search));
    };

    const handleSearchChange = (e) => {
        const newSearch = e.target.value;
        SetSearch(newSearch);
    };

    return (
        <div className="search-user-container fade-left">
            <form onSubmit={handleSubmit}>
                <input type="search" onChange={handleSearchChange} placeholder="Search User"/>
                <button type="submit"><i className="bi bi-search"></i></button>
            </form>
            {results && (
                <div id="searchUserResults" className="search-user-results fade-top">
                    {results.length > 0 ? (results.map((user) =>  <UserCard key={user.id} user={user} />)
                    ) : (
                        <h1>Users Not Found!</h1>
                    )}
                </div>
            )}
        </ div>
    );
}

export default SearchPage;