namespace restaurant_reservation.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending.ToString();
        public User Customer { get; set; } = null!;
        public required DateTime ReservationDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
public enum ReservationStatus
{
    Pending,  
    Confirmed, 
    Rejected   
}

