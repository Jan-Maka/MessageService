namespace Project.Server.Service
{
    public interface IVerificationStore
    {

        Task SaveVerificationCode(string userId, string email,string code, TimeSpan expiration);
        Task<bool> VerifiyCode(string userId, string email, string code);
    }
}
