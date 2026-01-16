namespace BEapplication.Models
{
    public class RequestNewReservation
    {
        public DateOnly ReservationDate { get; set; }
        public int ReservationHour { get; set; }
        public int People { get; set; }
    }
}