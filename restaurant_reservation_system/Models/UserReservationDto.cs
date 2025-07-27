namespace restaurant_reservation_system.Models
{
    public class UserReservationDto
    {
        public int Id { get; set; }
        public required DateTime ReservationDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public string? ReservationHour { get; set; }
    }
}
