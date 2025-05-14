using Project.Server.Models;

namespace Project.Server.Service
{
    public interface IEmailService
    {
        public void SendEmail(string to, string subject, string body);
    }
}
