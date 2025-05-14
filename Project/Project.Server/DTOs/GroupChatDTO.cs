namespace Project.Server.DTOs
{
    public class GroupChatDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<UserDTO> Users { get; set; }

        public List<MessageDTO> Messages { get; set; } = new List<MessageDTO>();

        public DateTime LastMessageReceived { get; set; }

        public GroupChatDTO()
        {
            
        }

        public GroupChatDTO(int id, string name, List<UserDTO> users, List<MessageDTO> messages, DateTime lastMessageReceived)
        {
            Id = id;
            Name = name;
            Users = users;
            Messages = messages;
            LastMessageReceived = lastMessageReceived;
        }
    }
}
