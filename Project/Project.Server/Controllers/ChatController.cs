using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Server.Data;
using Project.Server.DTOs;
using Project.Server.Models;
using Project.Server.Service;

namespace Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConversationService _conversationService;
        private readonly IGroupChatService _groupChatService;

        public ChatController(IUserService _userService, IConversationService _conversationService, IGroupChatService _groupChatService)
        {
            this._userService = _userService;
            this._conversationService = _conversationService;
            this._groupChatService = _groupChatService;
        }

        [Authorize]
        [HttpGet("get-conversations")]
        public ActionResult<List<ConversationDTO>> GetUserConversations()
        {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            if (user.Conversations.Count == 0) return NoContent();
            List<Conversation> userConversations = _conversationService.GetUserConversations(user);
            List<ConversationDTO> conversations = _conversationService.GetConversationDTOList(userConversations, user);
            return Ok(conversations);
        }

        [Authorize]
        [HttpGet("get-conversation")]
        public ActionResult<ConversationDTO> GetConversation([FromQuery(Name = "id")] int id)
        {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            Conversation conversation = _conversationService.GetUserConversationById(id);
            if (conversation == null) return NotFound();
            if (!conversation.Users.Any(u => u.UserId == user.Id)) return Unauthorized();
            return Ok(_conversationService.GetConversationDTO(conversation, user));
        }

        [Authorize]
        [HttpGet("get-group-chats")]
        public ActionResult<List<GroupChatDTO>> GetUserGroupChats()
        {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            if (user.GroupChats.Count == 0) return NoContent();
            List<GroupChat> userGroupChats = _groupChatService.GetUserChats(user);
            List<GroupChatDTO> groupChats = _groupChatService.GetGroupChatList(userGroupChats);
            return Ok(groupChats);
        }

        [Authorize]
        [HttpGet("get-group-chat")]
        public ActionResult<GroupChatDTO> GetGroupChat([FromQuery(Name = "id")] int id)
        {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            GroupChat chat = _groupChatService.GetGroupChatById(id);
            if (chat == null) return NotFound();
            if (!chat.Users.Any(u => u.UserId == user.Id)) return Unauthorized();
            return Ok(_groupChatService.GetGroupChatDTO(chat));
        }

        private List<Object> OrderByLastMessageReceived(List<Object> chats)
        {
            var orderedChats = chats.OrderByDescending(chat =>
            {
                if (chat is ConversationDTO conversation) return conversation.LastMessageReceived;
                if (chat is GroupChatDTO groupChat) return groupChat.LastMessageReceived;
                return DateTime.MinValue;
            }).ToList();

            return orderedChats;
        }

        [Authorize]
        [HttpGet("get-all-chats")]
        public ActionResult<List<Object>> GetAllUserChats()
        {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            List<Conversation> userConversations = _conversationService.GetUserConversations(user);
            List<GroupChat> userGroupChats = _groupChatService.GetUserChats(user);
            List<ConversationDTO> conversations = _conversationService.GetConversationDTOList(userConversations, user);
            List<GroupChatDTO> groupChats = _groupChatService.GetGroupChatList(userGroupChats);
            List<Object> allChats = new List<Object>();
            allChats.AddRange(conversations);
            allChats.AddRange(groupChats);
            if (allChats.Count == 0) return NoContent();

            List<Object> orderedChats = OrderByLastMessageReceived(allChats);

            return Ok(orderedChats);
        }

        [Authorize]
        [HttpGet("get-chats")]
        public ActionResult<List<Object>> GetChatsFromSearch([FromQuery(Name = "query")] string search) {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            List<ConversationDTO> matchedConversations = _conversationService.GetConversationsFromQuery(user, search);
            List<GroupChatDTO> matchedGroupChats = _groupChatService.GetGroupChatListFromQuery(user, search);
            List<Object> matchedChats = new List<Object>();
            matchedChats.AddRange(matchedConversations);
            matchedChats.AddRange(matchedGroupChats);
            if (matchedChats.Count == 0) return NoContent();

            return Ok(OrderByLastMessageReceived(matchedChats));
        }

        [Authorize]
        [HttpPost("create-group-chat")]
        public async Task<ActionResult<GroupChatDTO>> CreateGroupChat(GroupChatDTO groupChat) {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();
            GroupChatDTO groupChatDTO = await _groupChatService.CreateGroupChat(groupChat, user);
            return Ok(groupChatDTO);
        }

    }
}
