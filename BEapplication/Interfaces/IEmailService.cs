using BEapplication.Models;

public interface IEmailService
{

    /// <summary>
    /// Sends an email notification when a reservation is created.  
    /// </summary>
    /// <param name="reservation"></param>
    /// <returns></returns>
    Task SendReservationCreatedEmail(Reservation reservation);

    /// <summary>
    /// Sends an email notification when a reservation is cancelled.
    /// </summary>
    /// <param name="reservation"></param>
    /// <returns></returns>
    Task SendReservationCancelledEmail(Reservation reservation);
}
