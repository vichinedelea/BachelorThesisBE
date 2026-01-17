using BEapplication.Interfaces;
using BEapplication.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BEapplication.RequestHandlers
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendReservationEmail(
            string clientEmail,
            DateTime date,
            int hour,
            int people)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _settings.SenderName,
                "no-reply@mailtrap.io")); // IMPORTANT

            message.To.Add(new MailboxAddress(
                "Admin",
                "admin@fermanedelea.test"));

            message.Subject = "Rezervare nouă - Ferma Nedelea";

            message.Body = new TextPart("plain")
            {
                Text =
$@"A fost creată o nouă rezervare:

📅 Data: {date:dd.MM.yyyy}
⏰ Ora: {hour}:00
👥 Număr persoane: {people}
📧 Client: {clientEmail}

— Ferma Nedelea"
            };

            using var client = new SmtpClient();

            // 🔥 DEV ONLY – acceptă certificatul SSL
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(
                _settings.SmtpServer,
                587,
                SecureSocketOptions.StartTls);

            // 🔑 FIX CRITIC pentru Mailtrap
            client.AuthenticationMechanisms.Remove("CRAM-MD5");

            await client.AuthenticateAsync(
                _settings.Username,
                _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
