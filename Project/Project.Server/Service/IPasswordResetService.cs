using Project.Server.Models;

namespace Project.Server.Service
{
    public interface IPasswordResetService
    {
        Task<bool> SendResetPasswordEmailAsync(string email);
        Task<bool> ValidateResetTokenAsync(string resetToken);

        Task<bool> DeleteResetTokenAsync(int id);

        Task<bool> ResetPasswordAsync(string resetToken, string newPassword);
    }
}