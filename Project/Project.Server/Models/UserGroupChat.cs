namespace Project.Server.Models
{
    public class UserGroupChat
    {
        public int UserId { get; set; }
        public  User User { get; set; }

        public int GroupChatId { get; set; }
        public  GroupChat GroupChat { get; set; }
    }
}
