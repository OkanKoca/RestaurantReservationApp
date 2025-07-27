namespace restaurant_reservation.Models
{
    public class GuestReservation
    {
        public int Id { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending.ToString();
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public int NumberOfGuests { get; set; }
        public int TableId { get; set; }
        public Table? Table { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;

    }
}
