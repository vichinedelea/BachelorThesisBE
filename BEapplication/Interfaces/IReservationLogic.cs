using BEapplication.Models;

namespace BEapplication.Interfaces
{
    public interface IReservationLogic
    {
        /// <summary>
        /// Adds a new reservation for the user identified by userEmail.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        Task AddReservation(RequestNewReservation request, string userEmail);

        /// <summary>
        /// Gets the list of available days in a given month and year.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<List<int>> GetAvailableDays(int year, int month);

        /// <summary>
        /// Gets the list of available hours for a specific date.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<List<HourAvailability>> GetAvailableHours(int year, int month, int day);

        /// <summary>
        /// Deletes a reservation by its ID.
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        Task DeleteReservation(Guid reservationId);

        /// <summary>
        /// Gets the reservations for the user identified by userEmail.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        Task<List<Reservation>> GetMyReservations(string userEmail);
    }
}
