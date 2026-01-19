using BEapplication.DBContexts;
using BEapplication.Models;
using BEapplication.RequestHandlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BachelorThesisBETests.RequestHandlers
{
    [TestClass]
    public class ReservationLogicTests
    {
        private ApplicationContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }

        // AddReservation

        [TestMethod]
        public async Task AddReservation_CreatesReservation_WhenValid()
        {
            var context = CreateContext();
            var emailMock = new Mock<IEmailService>();
            var logic = new ReservationLogic(context, emailMock.Object);

            var request = new RequestNewReservation
            {
                ReservationDate = new DateOnly(2026, 5, 6),
                ReservationHour = 12,
                People = 4
            };

            await logic.AddReservation(request, "user@test.com");

            (await context.Reservations.CountAsync()).Should().Be(1);
        }

        [TestMethod]
        public async Task AddReservation_SendsEmail_WhenCreated()
        {
            var context = CreateContext();
            var emailMock = new Mock<IEmailService>();
            var logic = new ReservationLogic(context, emailMock.Object);

            var request = new RequestNewReservation
            {
                ReservationDate = new DateOnly(2026, 5, 6),
                ReservationHour = 12,
                People = 4
            };

            await logic.AddReservation(request, "user@test.com");

            emailMock.Verify(
                e => e.SendReservationCreatedEmail(It.IsAny<Reservation>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddReservation_ThrowsException_WhenWeekend()
        {
            var context = CreateContext();
            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var request = new RequestNewReservation
            {
                ReservationDate = new DateOnly(2026, 5, 9), // Saturday
                ReservationHour = 12,
                People = 2
            };

            Func<Task> act = () => logic.AddReservation(request, "a@test.com");

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Reservations are only available Monday–Friday.");
        }

        [TestMethod]
        public async Task AddReservation_ThrowsException_WhenHourInvalid()
        {
            var context = CreateContext();
            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var request = new RequestNewReservation
            {
                ReservationDate = new DateOnly(2026, 5, 6),
                ReservationHour = 8,
                People = 2
            };

            Func<Task> act = () => logic.AddReservation(request, "a@test.com");

            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task AddReservation_ThrowsException_WhenPeopleInvalid()
        {
            var context = CreateContext();
            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var request = new RequestNewReservation
            {
                ReservationDate = new DateOnly(2026, 5, 6),
                ReservationHour = 12,
                People = 25
            };

            Func<Task> act = () => logic.AddReservation(request, "a@test.com");

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Invalid number of people.");
        }

        [TestMethod]
        public async Task AddReservation_ThrowsException_WhenCapacityExceeded()
        {
            var context = CreateContext();
            context.Reservations.Add(new Reservation
            {
                ReservationDate = new DateOnly(2026, 5, 6),
                ReservationHour = 12,
                People = 20
            });
            await context.SaveChangesAsync();

            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var request = new RequestNewReservation
            {
                ReservationDate = new DateOnly(2026, 5, 6),
                ReservationHour = 12,
                People = 1
            };

            Func<Task> act = () => logic.AddReservation(request, "a@test.com");

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Nu mai sunt locuri disponibile la această oră.");
        }

        // GetAvailableDays

        [TestMethod]
        public async Task GetAvailableDays_ReturnsOnlyWeekdays()
        {
            var context = CreateContext();
            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var days = await logic.GetAvailableDays(2026, 5);

            days.Should().NotContain(9); // Saturday
            days.Should().NotContain(10); // Sunday
        }

        [TestMethod]
        public async Task GetAvailableDays_ExcludesFullyBookedDay()
        {
            // Arrange
            var context = CreateContext();

            var fullyBookedDate = new DateOnly(2026, 5, 5);

            for (int hour = 10; hour <= 16; hour++)
            {
                context.Reservations.Add(new Reservation
                {
                    ReservationDate = fullyBookedDate,
                    ReservationHour = hour,
                    People = 20
                });
            }

            await context.SaveChangesAsync();

            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            // Act
            var days = await logic.GetAvailableDays(2026, 5);

            // Assert
            days.Should().NotContain(5);
        }

        // GetAvailableHours

        [TestMethod]
        public async Task GetAvailableHours_ReturnsAvailableHours()
        {
            var context = CreateContext();
            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var hours = await logic.GetAvailableHours(2026, 5, 6);

            hours.Should().NotBeEmpty();
            hours.Should().Contain(h => h.Hour == 10 && h.AvailableSpots == 20);
        }

        [TestMethod]
        public async Task GetAvailableHours_ReturnsEmpty_WhenWeekend()
        {
            var context = CreateContext();
            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var hours = await logic.GetAvailableHours(2026, 5, 9);

            hours.Should().BeEmpty();
        }

        // DeleteReservation

        [TestMethod]
        public async Task DeleteReservation_RemovesReservation()
        {
            var context = CreateContext();
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                ReservationDate = new DateOnly(2026, 5, 6),
                ReservationHour = 12,
                People = 2,
                UserEmail = "a@test.com"
            };
            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();

            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            await logic.DeleteReservation(reservation.Id);

            (await context.Reservations.AnyAsync()).Should().BeFalse();
        }

        [TestMethod]
        public async Task DeleteReservation_SendsCancellationEmail()
        {
            var context = CreateContext();
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                ReservationDate = new DateOnly(2026, 5, 6),
                ReservationHour = 12,
                People = 2,
                UserEmail = "a@test.com"
            };
            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();

            var emailMock = new Mock<IEmailService>();
            var logic = new ReservationLogic(context, emailMock.Object);

            await logic.DeleteReservation(reservation.Id);

            emailMock.Verify(
                e => e.SendReservationCancelledEmail(It.IsAny<Reservation>()),
                Times.Once);
        }

        // GetMyReservations

        [TestMethod]
        public async Task GetMyReservations_ReturnsOnlyUserReservations()
        {
            var context = CreateContext();
            context.Reservations.AddRange(
                new Reservation { ReservationDate = new DateOnly(2026, 5, 6), ReservationHour = 12, UserEmail = "a@test.com" },
                new Reservation { ReservationDate = new DateOnly(2026, 5, 6), ReservationHour = 10, UserEmail = "b@test.com" }
            );
            await context.SaveChangesAsync();

            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var result = await logic.GetMyReservations("a@test.com");

            result.Should().HaveCount(1);
            result[0].UserEmail.Should().Be("a@test.com");
        }

        [TestMethod]
        public async Task GetMyReservations_ReturnsOrderedReservations()
        {
            var context = CreateContext();
            context.Reservations.AddRange(
                new Reservation { ReservationDate = new DateOnly(2026, 5, 7), ReservationHour = 14, UserEmail = "a@test.com" },
                new Reservation { ReservationDate = new DateOnly(2026, 5, 6), ReservationHour = 12, UserEmail = "a@test.com" }
            );
            await context.SaveChangesAsync();

            var logic = new ReservationLogic(context, new Mock<IEmailService>().Object);

            var result = await logic.GetMyReservations("a@test.com");

            result[0].ReservationDate.Should().Be(new DateOnly(2026, 5, 6));
            result[1].ReservationDate.Should().Be(new DateOnly(2026, 5, 7));
        }
    }
}
