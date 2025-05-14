using System.ComponentModel.DataAnnotations;

namespace Project.Server.Models
{
    public class GroupChat
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserGroupChat> Users { get; set; } = new List<UserGroupChat>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public  DateTime LastMessageReceived { get; set; } = DateTime.Today;


        public GroupChat()
        {
            
        }

        public GroupChat(string name)
        {
            Name = name;
        }

    }
}
