namespace Project.Server.Models
{
    public class UserConversation
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public UserConversation(){
            
        }

        public UserConversation(int userId, User user, int conversationId, Conversation conversation){
            UserId = userId;
            User = user;
            ConversationId = conversationId;
            Conversation = conversation;
        }
    }
}
