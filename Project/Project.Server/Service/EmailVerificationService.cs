
using Project.Server.Models;

namespace Project.Server.Service
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly IVerificationStore _verificationStore;
        private readonly IUserService _userService;
        public EmailVerificationService(IEmailService emailService, IVerificationStore verificationStore, IUserService userService)
        {
            _emailService = emailService;
            _verificationStore = verificationStore;
            _userService = userService;
        }
        public async Task SendEmailChangeCode(User user)
        {
            string code = GenerateCode(6);
            await _verificationStore.SaveVerificationCode(user.Id.ToString(), user.Email, code, TimeSpan.FromMinutes(15));
            string subject = "Email Change Verification Code";
            string body = $"Your verification code is: {code}";

            _emailService.SendEmail(user.Email, subject, body);

        }

        public async Task<bool> VerifyCode(User user, string submittedCode, string newEmail)
        {
            var isValid = await _verificationStore.VerifiyCode(user.Id.ToString(), user.Email, submittedCode);
            if (isValid) {
                await _userService.UpdateUserEmail(user, newEmail);
            }

            return isValid;
        }

        private string GenerateCode(int length) {
            var rd = new Random();
            return string.Concat(Enumerable.Range(0, length).Select(_ => rd.Next(0, 10)));
        }
    }
}
