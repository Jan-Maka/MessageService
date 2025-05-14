namespace Project.Server.DTOs
{
    public class ConversationDTO
    {
        public int Id { get; set; }

        public UserDTO OtherUser { get; set; }
        public List<MessageDTO> Messages { get; set; }

        public DateTime LastMessageReceived { get; set; }

        public ConversationDTO()
        {
            
        }

        public ConversationDTO(int id,UserDTO otherUser, List<MessageDTO> messages, DateTime lastMessageReceived)
        {
            Id = id;
            OtherUser = otherUser;
            Messages = messages;
            LastMessageReceived = lastMessageReceived;
        }
    }
}
