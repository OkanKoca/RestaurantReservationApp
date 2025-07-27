namespace restaurant_reservation.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending.ToString();
        public AppUser? Customer { get; set; }
        public Table? Table { get; set; }
        public int TableId { get; set; }    
        public required DateTime ReservationDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }

    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}
