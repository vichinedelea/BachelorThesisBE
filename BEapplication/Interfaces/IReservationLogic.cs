using BEapplication.Models;

namespace BEapplication.Interfaces
{
    public interface IReservationLogic
    {
        public Task AddReservation(RequestNewReservation newReservation);

    }
}
