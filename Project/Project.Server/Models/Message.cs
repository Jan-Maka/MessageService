using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Server.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public  string Content { get; set; }

        [Required]
        public  int SenderId { get; set; }
        public  User Sender { get; set; }

        public int? ConversationId { get; set; }
        public Conversation? Conversation { get; set; }

        public int? GroupChatId { get; set; }

        public GroupChat? GroupChat { get; set; }

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        public DateTime SentAt { get; set; }

        public bool Edited { get; set; } = false;

        public DateTime? EditedAt { get; set; }

        public Message()
        {
            
        }

        public Message(string content, int senderId, int? conversationId, DateTime sentAt){
            Content = content;
            SenderId = senderId;
            ConversationId = conversationId;
            SentAt = sentAt;
        }

        public Message(string content, int senderId, DateTime sentAt, int? groupChatId) { 
            Content = content;
            SenderId = senderId;
            SentAt= sentAt;
            GroupChatId = groupChatId;
        }
    }
}
