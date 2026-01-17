public interface IEmailService
{
    Task SendReservationEmail(
        string clientEmail,
        DateTime date,
        int hour,
        int people
    );
}
