using BEapplication.DBContexts;
using BEapplication.Interfaces;
using BEapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BEapplication.RequestHandlers
{
    public class ReservationLogic : IReservationLogic
    {
        private const int MAX_PEOPLE_PER_HOUR = 20;
        private const int START_HOUR = 10;
        private const int END_HOUR = 16;

        private readonly ApplicationContext _context;
        private readonly IEmailService _emailService;

        public ReservationLogic(ApplicationContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ==================================================
        // ADD RESERVATION
        // ==================================================
        public async Task AddReservation(RequestNewReservation request, string userEmail)
        {
            // zi lucrătoare
            if (request.ReservationDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                throw new Exception("Rezervările sunt disponibile doar luni–vineri.");

            // oră validă
            if (request.ReservationHour < START_HOUR || request.ReservationHour > END_HOUR)
                throw new Exception("Rezervările sunt disponibile între 10 și 16.");

            // persoane valide
            if (request.People < 1 || request.People > MAX_PEOPLE_PER_HOUR)
                throw new Exception("Număr invalid de persoane.");

            // verificare disponibilitate
            var usedPeople = await _context.Reservations
                .Where(r =>
                    r.ReservationDate == request.ReservationDate &&
                    r.ReservationHour == request.ReservationHour)
                .SumAsync(r => r.People);

            if (usedPeople + request.People > MAX_PEOPLE_PER_HOUR)
                throw new Exception("Nu mai sunt locuri disponibile la această oră.");

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                ReservationDate = request.ReservationDate,
                ReservationHour = request.ReservationHour,
                People = request.People,
                UserEmail = userEmail
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var reservationDateTime =
                request.ReservationDate.ToDateTime(TimeOnly.MinValue);

            await _emailService.SendReservationEmail(
                userEmail,
                reservationDateTime,
                request.ReservationHour,
                request.People);
        }

        // ==================================================
        // CHECK AVAILABILITY (20 pers / oră)
        // ==================================================
        public async Task<bool> HasAvailability(DateOnly date, int hour, int people)
        {
            var usedPeople = await _context.Reservations
                .Where(r => r.ReservationDate == date && r.ReservationHour == hour)
                .SumAsync(r => r.People);

            return usedPeople + people <= MAX_PEOPLE_PER_HOUR;
        }

        // ==================================================
        // YEAR → AVAILABLE MONTHS
        // ==================================================
        public async Task<List<int>> GetAvailableMonths(int year)
        {
            var result = new List<int>();

            for (int month = 1; month <= 12; month++)
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);

                for (int day = 1; day <= daysInMonth; day++)
                {
                    var date = new DateOnly(year, month, day);

                    if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                        continue;

                    for (int hour = START_HOUR; hour <= END_HOUR; hour++)
                    {
                        var used = await _context.Reservations
                            .Where(r => r.ReservationDate == date && r.ReservationHour == hour)
                            .SumAsync(r => r.People);

                        if (used < MAX_PEOPLE_PER_HOUR)
                        {
                            result.Add(month);
                            goto NextMonth;
                        }
                    }
                }

            NextMonth:;
            }

            return result.Distinct().OrderBy(m => m).ToList();
        }

        // ==================================================
        // MONTH → AVAILABLE DAYS
        // ==================================================
        public async Task<List<int>> GetAvailableDays(int year, int month)
        {
            var result = new List<int>();
            var daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateOnly(year, month, day);

                if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    continue;

                for (int hour = START_HOUR; hour <= END_HOUR; hour++)
                {
                    var used = await _context.Reservations
                        .Where(r => r.ReservationDate == date && r.ReservationHour == hour)
                        .SumAsync(r => r.People);

                    if (used < MAX_PEOPLE_PER_HOUR)
                    {
                        result.Add(day);
                        break;
                    }
                }
            }

            return result;
        }

        // ==================================================
        // DAY → AVAILABLE HOURS (+ locuri rămase)
        // ==================================================
        public async Task<List<HourAvailability>> GetAvailableHours(int year, int month, int day)
        {
            var date = new DateOnly(year, month, day);
            var result = new List<HourAvailability>();

            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return result;

            for (int hour = START_HOUR; hour <= END_HOUR; hour++)
            {
                var used = await _context.Reservations
                    .Where(r => r.ReservationDate == date && r.ReservationHour == hour)
                    .SumAsync(r => r.People);

                if (used < MAX_PEOPLE_PER_HOUR)
                {
                    result.Add(new HourAvailability
                    {
                        Hour = hour,
                        AvailableSpots = MAX_PEOPLE_PER_HOUR - used
                    });
                }
            }

            return result;
        }

        public async Task DeleteReservation(Guid reservationId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                throw new Exception("Rezervarea nu există.");

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Reservation>> GetMyReservations(string userEmail)
        {
            return await _context.Reservations
                .Where(r => r.UserEmail == userEmail)
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationHour)
                .ToListAsync();
        }

    }
}
