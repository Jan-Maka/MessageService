using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.Server.Data;
using Project.Server.DTOs;
using Project.Server.Models;
using Project.Server.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService _userService) { 
            this._userService = _userService;
        }


        [Authorize]
        [HttpGet("logged-in")]
        public ActionResult<UserDTO> GetLoggedInUser() {
            User user = _userService.GetLoggedInUser(User);
            UserDTO userDTO = _userService.GetUserDTO(user);
            return Ok(userDTO);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<List<UserDTO>>> SearchUsers([FromQuery(Name = "query")] string query) {
            User user = _userService.GetLoggedInUser(User);
            if(user == null) return Unauthorized();

            List<UserDTO> users = await _userService.GetUsersFromSearch(query, user);
            if (users.Count == 0) return NotFound(new List<UserDTO>());

            return Ok(users);
        }

        [Authorize]
        [HttpPost("send-friend-request")]
        public async Task<ActionResult> SendFriendRequest([FromQuery(Name = "userId")] int receiverId)
        {

            User sender = _userService.GetLoggedInUser(User);
            if (sender == null) return Unauthorized();

            User receiver = _userService.GetUserById(receiverId);

            if (receiver == null || sender == null) return NotFound();
            if (sender.Id == receiverId) return BadRequest();


            var existingRequest = _userService.DoesFriendRequestExist(sender, receiver);
            if (existingRequest) return BadRequest();

            FriendRequest request = await _userService.SendUserFriendRequest(sender, receiver);

            return Ok(request.Id);
        }

        [Authorize]
        [HttpPost("accept-friend-request")]
        public IActionResult AcceptFriendRequest([FromQuery(Name = "requestId")] int requestId)
        {
            User receiver = _userService.GetLoggedInUser(User);
            if (receiver == null) return Unauthorized();

            FriendRequest friendRequest = _userService.GetFriendRequestById(requestId);
            if (friendRequest == null) return NotFound();

            if (friendRequest.ReceiverId != receiver.Id) return Unauthorized();

            User sender = friendRequest.Sender;
            if (sender == null) return NotFound();

            if (sender.Id == receiver.Id) return BadRequest("A user cannot be friends with themselves.");

            _userService.AcceptUserFriendRequest(sender, receiver, friendRequest);

            return Ok();
        }

        [Authorize]
        [HttpDelete("delete-friend-request")]
        public async Task<ActionResult> DeleteFriendRequest([FromQuery(Name = "requestId")] int requestId) {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();

            FriendRequest friendRequest = null;
            friendRequest = _userService.GetFriendRequestById(requestId);
            if (friendRequest.SenderId != user.Id && friendRequest.ReceiverId != user.Id) return Unauthorized();
            if (friendRequest == null) return NotFound();
            await _userService.DeleteFriendRequest(friendRequest);
            return Ok();
        }

        [Authorize]
        [HttpDelete("remove-friend")]
        public async Task<ActionResult> RemoveFriend([FromQuery(Name = "userId")] int userId) {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();

            User friend = _userService.GetUserById(userId);
            if (friend == null) return NotFound();

            UserFriend userFriend = _userService.GetUserFriendByUsers(user,friend);
            UserFriend userFriend2 = _userService.GetUserFriendByUsers(friend, user);
            if (userFriend == null && userFriend2 == null) return NotFound();

            await _userService.RemoveFriend(user, friend);

            return Ok();    
        }

        [Authorize]
        [HttpGet("user-friends")]
        public async Task<ActionResult<List<UserDTO>>> GetUserFriends() {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            List<UserDTO> friends = _userService.GetUserFriends(user);
            if (friends.Count == 0) return NotFound(new List<UserDTO>());
            return Ok(friends);
        }

        [Authorize]
        [HttpGet("user-friends-search")]
        public async Task<ActionResult<List<UserDTO>>> GetUserFriendsFromSearch([FromQuery(Name = "query")] string query) {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            List<UserDTO> result = _userService.GetUserFriendsFromSearch(user,query);
            if (result.Count == 0) return NotFound(new List<UserDTO>());
            return Ok(result);
        }
    }
}
