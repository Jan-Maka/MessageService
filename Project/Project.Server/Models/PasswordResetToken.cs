using System.ComponentModel.DataAnnotations;

namespace Project.Server.Models
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; } = DateTime.UtcNow.AddHours(1);

        public PasswordResetToken()
        {

        }
        public PasswordResetToken(int userId, string token)
        {
            UserId = userId;
            Token = token;
        }
    }
}