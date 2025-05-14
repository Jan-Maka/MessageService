using Azure.Core;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Server.Data;
using Project.Server.DTOs;
using Project.Server.Models;
using System.Security.Claims;

namespace Project.Server.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher passwordHasher;


        public UserService(ApplicationDbContext _db, IPasswordHasher passwordHasher)
        {
            this._db = _db;
            this.passwordHasher = passwordHasher;
        }

        public User GetLoggedInUser(ClaimsPrincipal user)
        {
            if (user.Identity is ClaimsIdentity identity)
            {
                var userEmail = identity.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    return GetUserByEmail(userEmail);
                }
            }

            return null;
        }

        public User GetUserById(int id) {
            return _db.Users.SingleOrDefault(u => u.Id == id);
        }

        public UserDTO GetUserDTO(User user) { 
            return new UserDTO (user.Id,user.Email,user.Username);
        }

        public List<UserDTO> GetUsersDTOList(List<User> users) {
            return users.Select(u => GetUserDTO(u)).ToList();
        }

        public List<UserDTO> GetUserDTOsFromUserGroupChat(List<UserGroupChat> users)
        {
            return users.Select(u => GetUserDTO(u.User)).ToList();
        }

        public List<UserDTO> GetUserFriends(User user){
            List<User> friends = user.Friends.Select(uf => uf.Friend).ToList();
            return GetUsersDTOList(friends);
        }

        public List<UserDTO> GetUserFriendsFromSearch(User user, string query)
        {
            string likeQuery = $"%{query}%";    
            List<User> results = _db.Users
                .Where(u => u.Friends.Any(uf => uf.UserId != user.Id) &&
                        EF.Functions.Like(u.Username, likeQuery)).ToList();
            return GetUsersDTOList(results);
        }

        public bool CheckEmailExistsForAUser(string email)
        {
            return _db.Users.Any(u => u.Email == email);
        }

        public bool CheckUserNameExists(string username)
        {
            return _db.Users.Any(u => u.Username == username);
        }

        public bool CheckPasswordMatchesAgainstEmail(UserLoginDTO userDTO, User user)
        {
            return passwordHasher.VerifyHashedPassword(user.Password, userDTO.Password) == Microsoft.AspNet.Identity.PasswordVerificationResult.Success;
        }

        public User GetUserByEmail(string email)
        {
            return _db.Users
                            .Include(u => u.Friends)
                            .ThenInclude(uf => uf.Friend)
                            .AsSplitQuery()
                            .SingleOrDefault(u => u.Email == email);
        }

        public async Task CreateUser(UserDTO userDTO)
        {
            var user = new User
            {
                Username = userDTO.Username,
                Email = userDTO.Email,
                Password = passwordHasher.HashPassword(userDTO.Password),
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            //For when users send messages with file attachments
            string userDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", user.Id.ToString());
            Directory.CreateDirectory(userDir);
        }

        public async Task<List<UserDTO>> GetUsersFromSearch(string query, User user)
        {
            List<User> users = await _db.Users.Where(u => EF.Functions.Like(u.Username.ToLower(), $"%{query.ToLower()}%")).ToListAsync();
            List<UserDTO> userDTOs = users.Select(u => {
                var isFriend = _db.UserFriends.Any(uf => (uf.UserId == user.Id && uf.FriendId == u.Id) ||
                                                         (uf.UserId == u.Id && uf.FriendId == user.Id));
                var friendRequest = _db.FriendRequests.FirstOrDefault(fr => (fr.SenderId == user.Id && fr.ReceiverId == u.Id) ||
                                                                 (fr.ReceiverId == user.Id && fr.SenderId == u.Id));

                int? friendRequestId = friendRequest?.Id;

                var sentRequest = friendRequest != null && friendRequest.SenderId == user.Id;
                var receivedRequest = friendRequest != null && friendRequest.ReceiverId == user.Id;

                return new UserDTO(u.Id, u.Email, u.Username, isFriend, sentRequest, receivedRequest, friendRequestId);
            }).ToList();

            return userDTOs;
        }

        public bool DoesFriendRequestExist(User sender, User receiver)
        {
            return _db.FriendRequests.Any(fr => (fr.SenderId == sender.Id && fr.ReceiverId == receiver.Id) || (fr.SenderId == receiver.Id && fr.ReceiverId == sender.Id));
        }

        public async Task<FriendRequest> SendUserFriendRequest(User sender, User receiver)
        {
            FriendRequest request = new FriendRequest(sender, receiver);
            _db.FriendRequests.Add(request);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task AcceptUserFriendRequest(User sender, User receiver, FriendRequest request)
        {
            UserFriend userFriend = new UserFriend(sender, receiver);
            UserFriend userFriend1 = new UserFriend(receiver, sender);
            _db.UserFriends.Add(userFriend);
            _db.UserFriends.Add(userFriend1);
            _db.FriendRequests.Remove(request);

            var conversationExists = _db.Conversations.Any(c => c.Users.Count == 2 && c.Users.Any(u => u.UserId == sender.Id) && c.Users.Any(u => u.UserId == receiver.Id));
            if (!conversationExists)
            {
                Conversation conversation = new Conversation();
                _db.Conversations.Add(conversation);

                UserConversation userConversation = new UserConversation(sender.Id, sender, conversation.Id, conversation);
                UserConversation userConversation1 = new UserConversation(receiver.Id, receiver, conversation.Id, conversation);
                _db.UserConversations.Add(userConversation);
                _db.UserConversations.Add(userConversation1);
            }

            await _db.SaveChangesAsync();
        }

        public FriendRequest GetFriendRequestById(int id)
        {
            return _db.FriendRequests.Include(fr => fr.Sender).SingleOrDefault(fr => fr.Id == id);
        }

        public async Task DeleteFriendRequest(FriendRequest request)
        {
            _db.FriendRequests.Remove(request);
            await _db.SaveChangesAsync();
        }

        public UserFriend GetUserFriendByUsers(User user1, User user2)
        {
            return _db.UserFriends.FirstOrDefault(uf => (uf.UserId == user1.Id && uf.FriendId == user2.Id) ||
                                                                         (uf.UserId == user2.Id && uf.FriendId == user1.Id));
        }

        public async Task RemoveFriend(User user, User friend)
        {
            if(user == null || friend == null) return;

            var userFriends = await _db.UserFriends
                .Where(uf => (uf.UserId == user.Id && uf.FriendId == friend.Id) ||
                             (uf.UserId == friend.Id && uf.FriendId == user.Id))
                .ToListAsync();

            if (userFriends.Any())  
            {
                _db.UserFriends.RemoveRange(userFriends);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateUser(User user, UpdateUserDTO updatedUser)
        {
            user.Username = updatedUser.Username;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateUserPassword(User user, string password)
        {
            user.Password = passwordHasher.HashPassword(password);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateUserEmail(User user, string newEmail)
        {
            user.Email = newEmail;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
