using Microsoft.EntityFrameworkCore;
using Project.Server.Data;
using Project.Server.DTOs;
using Project.Server.Models;

namespace Project.Server.Service
{
    public class ConversationService : IConversationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        public ConversationService(ApplicationDbContext db, IUserService userService, IMessageService messageService)
        {
            _db = db;
            _userService = userService;
            _messageService = messageService;
        }

        public Conversation GetUserConversationById(int id)
        {
            return _db.Conversations
                .Include(c => c.Users)
                .ThenInclude(uc => uc.User)
                .Include(c => c.Messages)
                .ThenInclude(m => m.Attachments)
                .AsSplitQuery()
                .SingleOrDefault(c => c.Id == id);
        }

        public List<Conversation> GetUserConversations(User user)
        {

            return user.Conversations.Select(uc => GetUserConversationById(uc.ConversationId)).ToList();
        }

        public ConversationDTO GetConversationDTO(Conversation conversation, User user) {
            UserConversation otherUser = conversation.Users.FirstOrDefault(u => u.UserId != user.Id);
            Message? latestMessage = conversation.Messages.Count > 0 ? conversation.Messages.Last() : null;
            if (latestMessage == null) return new ConversationDTO(conversation.Id, _userService.GetUserDTO(otherUser.User), conversation.LastMessageReceived);
            return new ConversationDTO(conversation.Id,
                                       _userService.GetUserDTO(otherUser.User),
                                       _messageService.GetMessageDTO(latestMessage),
                                       conversation.LastMessageReceived);
        }

        public List<ConversationDTO> GetConversationDTOList(List<Conversation> conversations, User user)
        {
            return conversations.Select(c => GetConversationDTO(c,user)).ToList();
        }

        public List<ConversationDTO> GetConversationsFromQuery(User user, string query)
        {
            string likeQuery = $"%{query}%";
            List<Conversation> matchedConversations = _db.Conversations
                .Include(convo => convo.Users)
                .ThenInclude(uc => uc.User)
                .Include(convo => convo.Messages)
                .Where(convo => 
                    convo.Users.Any(uc => uc.UserId ==  user.Id) &&(
                    convo.Users.Any(uc => EF.Functions.Like(uc.User.Username, likeQuery)))).ToList();
            return GetConversationDTOList(matchedConversations, user);
        }
    }
}
