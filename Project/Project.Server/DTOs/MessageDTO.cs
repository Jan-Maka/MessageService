using Project.Server.Models;

namespace Project.Server.DTOs
{
    public class MessageDTO
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public required int SenderId { get; set; }

        public required string SenderName { get; set; }

        public List<AttachmentDTO> Attachments { get; set; } = new List<AttachmentDTO>();

        public int? ConversationId { get; set; }

        public int? GroupChatId { get; set; }

        public DateTime SentAt { get; set; }

        public bool Edited { get; set; }

        public DateTime? EditedAt { get; set; }

        public MessageDTO()
        {
            
        }

        public MessageDTO(int id, string content, int senderId, string senderName ,DateTime sent, int? conversationId, int? groupChatId, bool edited, DateTime? editedAt)
        {
            Id = id;
            Content = content;
            SenderId = senderId;
            SenderName = senderName;
            SentAt = sent;
            ConversationId = conversationId;
            GroupChatId = groupChatId;
            Edited = edited;
            EditedAt = editedAt;
        }

    }
}
