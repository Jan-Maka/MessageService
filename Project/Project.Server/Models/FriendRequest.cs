using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Server.Models
{
    public class FriendRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        [Required]
        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }

        public DateTime TimeSent { get; set; }

        public FriendRequest()
        {
            TimeSent = DateTime.UtcNow; // Default value for TimeSent
        }

        public FriendRequest(User sender, User receiver) {
            SenderId = sender.Id;
            ReceiverId = receiver.Id;
            Sender = sender;
            Receiver = receiver;
            TimeSent = DateTime.UtcNow;
        }

    }
}
