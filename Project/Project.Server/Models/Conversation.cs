using System.ComponentModel.DataAnnotations;

namespace Project.Server.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }

        public  ICollection<UserConversation> Users { get; set; } = new List<UserConversation>();


        public  ICollection<Message> Messages { get; set; } = new List<Message>();

        public DateTime LastMessageReceived { get; set; } = DateTime.Today;

        public Conversation()
        {
            
        }

    }
}
