using restaurant_reservation.Models;

namespace restaurant_reservation_system.Models.Dto
{
    public class AdminGuestReservationDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending.ToString();
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservationDate { get; set; }
        public string? ReservationHour => ReservationDate.Hour.ToString();
        public DateTime CreatedAt { get; set; }
    }
}
