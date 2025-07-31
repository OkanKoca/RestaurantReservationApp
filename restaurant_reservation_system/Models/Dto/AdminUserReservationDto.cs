using restaurant_reservation.Models;

namespace restaurant_reservation_system.Models.Dto
{
    public class AdminUserReservationDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public required DateTime ReservationDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public string? ReservationHour { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending.ToString();
        public DateTime CreatedAt { get; set; }
    }
}
