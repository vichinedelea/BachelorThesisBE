using BEapplication.Models;
using BEapplication.RequestHandlers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;

namespace BachelorThesisBETests.RequestHandlers
{
    [TestClass]
    public class EmailServiceTests
    {
        private class TestEmailService : EmailService
        {
            public bool SendAsyncCalled { get; private set; }
            public MimeMessage? SentMessage { get; private set; }

            public TestEmailService(IOptions<EmailSettings> settings)
                : base(settings)
            {
            }

            protected override Task SendAsync(MimeMessage message)
            {
                SendAsyncCalled = true;
                SentMessage = message;
                return Task.CompletedTask;
            }
        }

        private static IOptions<EmailSettings> GetFakeSettings()
        {
            return Options.Create(new EmailSettings
            {
                SmtpServer = "fake.smtp",
                Username = "fake",
                Password = "fake",
                SenderName = "Test Sender"
            });
        }

        [TestMethod]
        public async Task SendReservationCreatedEmail_CallsSendAsync()
        {
            // Arrange
            var service = new TestEmailService(GetFakeSettings());

            var reservation = new Reservation
            {
                ReservationDate = new DateOnly(2026, 5, 10),
                ReservationHour = 12,
                People = 4,
                UserEmail = "test@test.com"
            };

            // Act
            await service.SendReservationCreatedEmail(reservation);

            // Assert
            service.SendAsyncCalled.Should().BeTrue();
            service.SentMessage.Should().NotBeNull();
            service.SentMessage!.Subject.Should().Contain("New booking");
        }

        [TestMethod]
        public async Task SendReservationCancelledEmail_CallsSendAsync()
        {
            // Arrange
            var service = new TestEmailService(GetFakeSettings());

            var reservation = new Reservation
            {
                ReservationDate = new DateOnly(2026, 6, 15),
                ReservationHour = 14,
                UserEmail = "cancel@test.com"
            };

            // Act
            await service.SendReservationCancelledEmail(reservation);

            // Assert
            service.SendAsyncCalled.Should().BeTrue();
            service.SentMessage.Should().NotBeNull();
            service.SentMessage!.Subject.Should().Contain("Booking cancelled");
        }
    }
}
