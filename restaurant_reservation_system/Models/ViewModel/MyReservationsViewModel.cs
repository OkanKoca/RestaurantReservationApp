using restaurant_reservation.Models;

namespace restaurant_reservation_system.Models.ViewModel
{
    public class MyReservationsViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }
        public DateTime ReservationDateLocal => ReservationDate.ToLocalTime();
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedAtLocal => CreatedAt.ToLocalTime();
        public string ReservationHour { get; set; } = string.Empty;
        public int NumberOfGuests { get; set; }
    }
}
