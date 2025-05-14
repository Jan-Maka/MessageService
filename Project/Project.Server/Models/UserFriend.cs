using System.Reflection;

namespace Project.Server.Models
{
    public class UserFriend
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int FriendId { get; set; }

        public  User Friend { get; set; }

        public UserFriend()
        {
            
        }

        public UserFriend(User user, User friend)
        {
            User = user;
            UserId = user.Id;
            Friend = friend;
            FriendId = friend.Id;
        }
    }
}
