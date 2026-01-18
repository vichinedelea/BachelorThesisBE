namespace BEapplication.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }

        public DateOnly ReservationDate { get; set; }

        public int ReservationHour { get; set; }

        public int People { get; set; }

        public string UserEmail { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
