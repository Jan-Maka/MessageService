using System.ComponentModel.DataAnnotations;

namespace Project.Server.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }

        public  ICollection<UserFriend> Friends { get; set; } = new List<UserFriend>();

        public  ICollection<UserConversation> Conversations { get; set; } = new List<UserConversation>();

        public  ICollection<UserGroupChat> GroupChats { get; set; } = new List<UserGroupChat>();

        public ICollection<FriendRequest> FriendRequestsSent { get; set; } = new List<FriendRequest>();

        public ICollection<FriendRequest> FriendRequestsReceived { get; set; } = new List<FriendRequest>();

    }
}
