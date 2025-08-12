namespace Project.Server.DTOs
{
    public class GroupChatDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<UserDTO> Users { get; set; }

        public MessageDTO LatestMessage { get; set; }

        public DateTime LastMessageReceived { get; set; }

        public GroupChatDTO()
        {
            
        }

        public GroupChatDTO(int id, string name, List<UserDTO> users, DateTime lastMessageReceived)
        {
            Id = id;
            Name = name;
            Users = users;
            LastMessageReceived = lastMessageReceived;
        }

        public GroupChatDTO(int id, string name, List<UserDTO> users, MessageDTO latestMessage, DateTime lastMessageReceived)
        {
            Id = id;
            Name = name;
            Users = users;
            LatestMessage = latestMessage;
            LastMessageReceived = lastMessageReceived;
        }
    }
}
