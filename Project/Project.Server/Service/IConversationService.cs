using Project.Server.DTOs;
using Project.Server.Models;

namespace Project.Server.Service
{
    public interface IConversationService
    {
        List<Conversation> GetUserConversations(User user);
        Conversation GetUserConversationById(int id);

        ConversationDTO GetConversationDTO(Conversation conversation, User user);

        List<ConversationDTO> GetConversationDTOList(List<Conversation> conversations, User user);

        List<ConversationDTO> GetConversationsFromQuery(User user,  string query);
    }
}
