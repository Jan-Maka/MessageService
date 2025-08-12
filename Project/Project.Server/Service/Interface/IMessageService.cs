using Project.Server.DTOs;
using Project.Server.Models;

namespace Project.Server.Service
{
    public interface IMessageService
    {

        Task<MessageDTO> CreateMessageForConversation(MessageDTO messageDTO);

        Task<MessageDTO> CreateMessageForGroupChat(MessageDTO messageDTO);
        Message GetMessageById(int id);

        MessageDTO GetMessageDTO(Message message);

        List<MessageDTO> GetMessagesDTOList(List<Message> messages);

        Task<MessageDTO> UpdateMessageContent(Message message, string updatedContent);

        Task DeleteMessage(int id);

    }
}
