namespace Project.Server.DTOs
{
    public class ResetPasswordDTO
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public ResetPasswordDTO(string token, string newPassword)
        {
            Token = token;
            NewPassword = newPassword;
        }
    
    }

}