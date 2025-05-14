namespace Project.Server.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; } 
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsFriend { get; set; }

        public string? Password { get; set; }

        public bool SentFriendRequest { get; set; }

        public bool ReceivedFriendRequest { get; set; }
        public int? FriendRequestId { get; set; }

        public UserDTO()
        {
            
        }
        public UserDTO(int Id,string Email, string Username, bool IsFriend, bool sentFriendRequsest, bool receivedFriendRequest, int? friendRequestId)
        {
            this.Id = Id;
            this.Email = Email;
            this.Username = Username;
            this.IsFriend = IsFriend;
            SentFriendRequest = sentFriendRequsest;
            ReceivedFriendRequest = receivedFriendRequest;
            FriendRequestId = friendRequestId;
        }

        public UserDTO(int id, string email ,string username) {
            Id = id;
            Email = email;
            Username = username;
        }

    }
}
