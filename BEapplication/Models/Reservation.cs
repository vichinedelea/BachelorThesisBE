namespace BEapplication.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }

        // Data rezervării (fără oră)
        public DateOnly ReservationDate { get; set; }

        // Ora rezervării (10–16)
        public int ReservationHour { get; set; }

        // Număr persoane din această rezervare
        public int People { get; set; }

        // Email utilizator (din login)
        public string UserEmail { get; set; } = string.Empty;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
