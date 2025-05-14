using Microsoft.AspNetCore.Identity.UI.Services;
using Project.Server.DTOs;
using Project.Server.Models;
using System.Security.Claims;

namespace Project.Server.Service
{
    public interface IUserService
    {
        Task CreateUser(UserDTO userDTO);


        User GetLoggedInUser(ClaimsPrincipal user);

        User GetUserById(int id);

        UserDTO GetUserDTO(User user);

        List<UserDTO> GetUsersDTOList(List<User> users);


        List<UserDTO> GetUserDTOsFromUserGroupChat(List<UserGroupChat> users);

        List<UserDTO> GetUserFriends(User user);

        List<UserDTO> GetUserFriendsFromSearch(User user, string query);

        bool CheckEmailExistsForAUser(string email);

        bool CheckUserNameExists(string username);

        bool CheckPasswordMatchesAgainstEmail(UserLoginDTO userDTO, User user);

        User GetUserByEmail(string email);

        Task<List<UserDTO>> GetUsersFromSearch(string query, User user);

        bool DoesFriendRequestExist(User sender, User receiver);

        Task<FriendRequest> SendUserFriendRequest(User sender, User receiver);

        Task AcceptUserFriendRequest(User sender,User receiver, FriendRequest request);


        FriendRequest GetFriendRequestById(int id);

        Task DeleteFriendRequest(FriendRequest request);

        UserFriend GetUserFriendByUsers(User user1, User user2);


        Task RemoveFriend(User user, User friend);

        Task UpdateUser(User user, UpdateUserDTO updatedUser);

        Task UpdateUserPassword(User user, string password);

        Task UpdateUserEmail(User user, string newEmail);

    }
}
