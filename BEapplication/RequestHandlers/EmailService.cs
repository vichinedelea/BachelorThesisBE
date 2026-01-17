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

        // 📩 EMAIL LA CREARE REZERVARE
        public async Task SendReservationCreatedEmail(Reservation reservation)
        {
            var message = BuildMessage(
                subject: "Rezervare nouă - Ferma Nedelea",
                body:
$@"A fost creată o nouă rezervare:

📅 Data: {reservation.ReservationDate:dd.MM.yyyy}
⏰ Ora: {reservation.ReservationHour}:00
👥 Număr persoane: {reservation.People}
📧 Client: {reservation.UserEmail}

— Ferma Nedelea"
            );

            await SendAsync(message);
        }

        // ❌ EMAIL LA ANULARE REZERVARE
        public async Task SendReservationCancelledEmail(Reservation reservation)
        {
            var message = BuildMessage(
                subject: "Rezervare anulată - Ferma Nedelea",
                body:
$@"O rezervare a fost anulată:

📅 Data: {reservation.ReservationDate:dd.MM.yyyy}
⏰ Ora: {reservation.ReservationHour}:00
📧 Client: {reservation.UserEmail}

— Ferma Nedelea"
            );

            await SendAsync(message);
        }

        // 🔧 Helper comun (fără duplicare de cod)
        private MimeMessage BuildMessage(string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _settings.SenderName,
                "no-reply@mailtrap.io"));

            message.To.Add(new MailboxAddress(
                "Admin",
                "admin@fermanedelea.test"));

            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            return message;
        }

        // 🔌 Trimitere SMTP
        private async Task SendAsync(MimeMessage message)
        {
            using var client = new SmtpClient();

            // DEV only
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(
                _settings.SmtpServer,
                587,
                SecureSocketOptions.StartTls);

            // 🔑 FIX Mailtrap
            client.AuthenticationMechanisms.Remove("CRAM-MD5");

            await client.AuthenticateAsync(
                _settings.Username,
                _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
