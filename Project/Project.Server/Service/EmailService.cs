using Microsoft.Extensions.Options;
using Project.Server.Components;
using Project.Server.Models;
using System.Net;
using System.Net.Mail;

namespace Project.Server.Service
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;

        public EmailService(IOptions<EmailSettings> options)
        {
            var settings = options.Value;
            _fromAddress = settings.From;
            _smtpClient = new SmtpClient(settings.SmtpHost, Int32.Parse(settings.SmtpPort))
            {
                Credentials = new NetworkCredential(settings.From, settings.Password),
                EnableSsl = true
            };
        }

        public void SendEmail(string to, string subject, string body)
        {
            var message = new MailMessage(_fromAddress, to, subject, body);
            try
            {
                _smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }
    }
}
