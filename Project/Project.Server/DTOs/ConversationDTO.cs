namespace Project.Server.DTOs
{
    public class ConversationDTO
    {
        public int Id { get; set; }

        public UserDTO OtherUser { get; set; }
        public MessageDTO LatestMessage { get; set; }

        public DateTime LastMessageReceived { get; set; }

        public ConversationDTO()
        {
            
        }
        
        public ConversationDTO(int id, UserDTO otherUser, DateTime lastMessageReceived)
        {
            Id = id;
            OtherUser = otherUser;
            LastMessageReceived = lastMessageReceived;
        }

        public ConversationDTO(int id, UserDTO otherUser, MessageDTO latestMessage, DateTime lastMessageReceived)
        {
            Id = id;
            OtherUser = otherUser;
            LatestMessage = latestMessage;
            LastMessageReceived = lastMessageReceived;
        }
    }
}
