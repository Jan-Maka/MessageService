using Azure.Core;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Server.Data;
using Project.Server.Models;
using Project.Server.Service;

public class PasswordResetService : IPasswordResetService
{
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _db;
    private readonly string _frontendBaseUrl;

    private readonly IPasswordHasher _passwordHasher;

    public PasswordResetService(IEmailService emailService, ApplicationDbContext db, IConfiguration configuration, IPasswordHasher passwordHasher)
    {
        _emailService = emailService;
        _db = db;
        _frontendBaseUrl = configuration["FrontendBasedUrl"] ?? throw new ArgumentNullException("FrontendBasedUrl configuration is missing.");
        _passwordHasher = passwordHasher;

    }

    public async Task<bool> DeleteResetTokenAsync(int id)
    {
        PasswordResetToken? token = await _db.PasswordResetTokens.FindAsync(id);
        if (token == null) return false;

        _db.PasswordResetTokens.Remove(token);
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> SendResetPasswordEmailAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;

        var code = GenerateVerificationCode();
        var resetToken = new PasswordResetToken(user.Id, code);
        _db.PasswordResetTokens.Add(resetToken);
        await _db.SaveChangesAsync();

        string resetUrl = $"{_frontendBaseUrl}/reset-password?token={resetToken.Token}";
        string content = $"Hi, {resetToken.User.Username}!\n\n" +
                            "You requested a password reset." +"\n" +
                            $"You can reset your password by clicking this link: {resetUrl}\n\n" +
                            "If you did not request this, please ignore this email.";


        _emailService.SendEmail(email, "Password Reset", content);
        return true;
    }

    public Task<bool> ValidateResetTokenAsync(string resetToken)
    {
        PasswordResetToken? token = _db.PasswordResetTokens.FirstOrDefault(t => t.Token == resetToken);

        if (token == null) return Task.FromResult(false);

        if (token.Expiration < DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }


    public Task<bool> ResetPasswordAsync(string resetToken, string newPassword)
    {
        PasswordResetToken? token = _db.PasswordResetTokens.FirstOrDefault(t => t.Token == resetToken);
        if (token == null || token.Expiration < DateTime.UtcNow) return Task.FromResult(false);
        

        User? user = _db.Users.Find(token.UserId);
        if (user == null) return Task.FromResult(false);

        user.Password = _passwordHasher.HashPassword(newPassword);
        _db.Users.Update(user);
        _db.PasswordResetTokens.Remove(token);
        _db.SaveChanges();

        return Task.FromResult(true);
    }


    private string GenerateVerificationCode()
    {
        // Implementation for generating a secure random code
        return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    }
}