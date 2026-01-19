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

        /// <summary>
        /// Inheritance
        /// </summary>
        public async Task SendReservationCreatedEmail(Reservation reservation)
        {
            var message = BuildMessage(
                subject: "New booking - Nedelea Farm",
                body:
$@"A new reservation has been created.:

📅 Date: {reservation.ReservationDate:dd.MM.yyyy}
⏰ Hour: {reservation.ReservationHour}:00
👥 Number of people: {reservation.People}
📧 Client: {reservation.UserEmail}

— Nedelea Farm"
            );

            await SendAsync(message);
        }

        public async Task SendReservationCancelledEmail(Reservation reservation)
        {
            var message = BuildMessage(
                subject: "Booking cancelled - Nedelea Farm",
                body:
$@"A reservation has been canceled:

📅 Date: {reservation.ReservationDate:dd.MM.yyyy}
⏰ Hour: {reservation.ReservationHour}:00
📧 Client: {reservation.UserEmail}

— Nedelea Farm"
            );

            await SendAsync(message);
        }

        /// <summary>
        /// Inheritance
        /// </summary>
        protected virtual async Task SendAsync(MimeMessage message)
        {
            using var client = new SmtpClient();

            // DEV only
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(
                _settings.SmtpServer,
                587,
                SecureSocketOptions.StartTls);

            client.AuthenticationMechanisms.Remove("CRAM-MD5");

            await client.AuthenticateAsync(
                _settings.Username,
                _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

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

    }
}
