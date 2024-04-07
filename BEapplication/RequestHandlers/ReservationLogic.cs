using BEapplication.DBContexts;
using BEapplication.Interfaces;
using BEapplication.Models;

namespace BEapplication.RequestHandlers
{
    public class ReservationLogic : IReservationLogic
    {
        private ApplicationContext _context;

        public ReservationLogic(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddReservation(RequestNewReservation newReservation)
        {
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                Hour = newReservation.Hour,
                Day = newReservation.Day,
                Month = newReservation.Month,
                Year = newReservation.Year,
                Activity = newReservation.Activity,
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
