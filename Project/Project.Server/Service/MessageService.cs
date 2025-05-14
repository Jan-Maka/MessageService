using Microsoft.EntityFrameworkCore;
using Project.Server.Data;
using Project.Server.DTOs;
using Project.Server.Models;

namespace Project.Server.Service
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAttachmentService _attachmentService;

        public MessageService(ApplicationDbContext db, IAttachmentService attachmentService)
        {
            _db = db;
            _attachmentService = attachmentService;
        }

        public Message GetMessageById(int id)
        {
            return _db.Messages.Include(m => m.Attachments).Include(m => m.Sender).AsSplitQuery().SingleOrDefault(m => m.Id == id);
        }

        public MessageDTO GetMessageDTO(Message message)
        {
            return new MessageDTO
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                SenderName = message.Sender.Username,
                SentAt = message.SentAt,
                GroupChatId = message.GroupChatId, 
                ConversationId = message.ConversationId,
                Attachments = _attachmentService.CreateAttachmentDTOList(message.Attachments),
                Edited = message.Edited,
                EditedAt = message.EditedAt,
            };
        }

        public List<MessageDTO> GetMessagesDTOList(List<Message> messages)
        {
            return messages.Select(m => GetMessageDTO(m)).ToList();
        }

        public async Task<MessageDTO> CreateMessageForConversation(MessageDTO messageDTO)
        {
            Message message = new Message(messageDTO.Content, messageDTO.SenderId, messageDTO.ConversationId, messageDTO.SentAt);
            _db.Messages.Add(message);
            var conversation = await _db.Conversations.FindAsync(messageDTO.ConversationId);
            if (conversation != null) conversation.LastMessageReceived = message.SentAt;
            await _db.SaveChangesAsync();
            messageDTO.Id = message.Id;
            foreach (var a in messageDTO.Attachments){
                var attachment = await _db.Attachments.FirstOrDefaultAsync(attach => attach.Id == a.Id);
                if (attachment != null){
                    attachment.MessageId = message.Id;
                    await _db.SaveChangesAsync();
                }
            }

            return messageDTO;
        }

        public async Task<MessageDTO> CreateMessageForGroupChat(MessageDTO messageDTO){
            Message message = new Message(messageDTO.Content, messageDTO.SenderId, messageDTO.SentAt, messageDTO.GroupChatId);
            _db.Messages.Add(message);
            var groupChat = await _db.GroupChats.FindAsync(messageDTO.GroupChatId);
            if (groupChat != null) groupChat.LastMessageReceived = message.SentAt;
            await _db.SaveChangesAsync();
            messageDTO.Id = message.Id;
            foreach (var a in messageDTO.Attachments){
                var attachment = await _db.Attachments.FirstOrDefaultAsync(attach => attach.Id == a.Id);
                if (attachment != null){
                    attachment.MessageId = message.Id;
                    await _db.SaveChangesAsync();
                }
            }

            return messageDTO;
        }

        public async Task<MessageDTO> UpdateMessageContent(Message message, string updatedContent)
        {
            message.Content = updatedContent;
            message.Edited = true;
            message.EditedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return GetMessageDTO(message);
        }

        public async Task DeleteMessage(int id)
        {
            var message = await _db.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message == null) throw new KeyNotFoundException($"Message with {id} not found");
            foreach(var attachment in message.Attachments) _db.Attachments.Remove(attachment);
            _db.Messages.Remove(message);
            await _db.SaveChangesAsync();
        }
    }
}
