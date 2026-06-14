using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace Website_forum.Models
{
    public class EmailMessage
    {
        public string Destination { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class EmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var smtpSection = configuration.GetSection("Smtp");
            var host = smtpSection["Host"];
            var port = int.Parse(smtpSection["Port"] ?? "465");
            var from = smtpSection["User"];
            var pass = smtpSection["Password"];

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(message.Destination));
            email.Subject = message.Subject;
            email.Body = new TextPart("html") { Text = message.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(from, pass);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
    }
}