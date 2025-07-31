using restaurant_reservation.Models;

namespace restaurant_reservation_system.Models.ViewModel
{
    public class AdminGuestReservationViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ReservationDateLocal => ReservationDate.ToLocalTime();
        public string? ReservationHour { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedAtLocal => CreatedAt.ToLocalTime();
    }
}
