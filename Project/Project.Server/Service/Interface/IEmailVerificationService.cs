using Project.Server.Models;

namespace Project.Server.Service
{
    public interface IEmailVerificationService
    {
        Task SendEmailChangeCode(User user);
        Task<bool> VerifyCode(User user, string submittedCode, string newEmail);
    }
}
