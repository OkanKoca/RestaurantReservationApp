namespace restaurant_reservation_system.Models
{
    public class UserReservationViewModel
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime ReservationDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? ReservationHour { get; set; }
    }
}
