using BEapplication.Models;

public interface IEmailService
{
    Task SendReservationCreatedEmail(Reservation reservation);

    Task SendReservationCancelledEmail(Reservation reservation);
}
