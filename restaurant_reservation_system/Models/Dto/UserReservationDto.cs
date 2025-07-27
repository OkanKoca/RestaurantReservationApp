namespace restaurant_reservation_system.Models.Dto
{
    public class UserReservationDto
    {
        public int CustomerId { get; set; }
        public required DateTime ReservationDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public string? ReservationHour { get; set; }
        public string Status { get; set; } = "Pending"; // Default status is "Pending"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
