using BEapplication.Models;

namespace BEapplication.Interfaces
{
    public interface IReservationLogic
    {
        // Creează o rezervare (după toate validările)
        Task AddReservation(RequestNewReservation request, string userEmail);

        // Verifică dacă mai sunt locuri libere într-un slot
        Task<bool> HasAvailability(DateOnly date, int hour, int people);

        // 🔽 pentru flow-ul: AN → LUNĂ
        Task<List<int>> GetAvailableMonths(int year);

        // 🔽 pentru flow-ul: LUNĂ → ZI
        Task<List<int>> GetAvailableDays(int year, int month);

        // 🔽 pentru flow-ul: ZI → ORĂ (+ locuri rămase)
        Task<List<HourAvailability>> GetAvailableHours(int year, int month, int day);

        Task DeleteReservation(Guid reservationId);

        Task<List<Reservation>> GetMyReservations(string userEmail);
    }
}
