using restaurant_reservation.Models;

namespace restaurant_reservation_api.Dto
{
    public class AdminGuestReservationTableDto
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required DateTime ReservationDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public string? ReservationHour { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending.ToString();
        public DateTime CreatedAt { get; set; }
    }
}
