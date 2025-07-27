using restaurant_reservation.Models;

namespace restaurant_reservation.Dto
{
    public class UserReservationDto // login needed reservation
    {
        //public AppUser? Customer { get; set; }
        public int CustomerId { get; set; }
        public required DateTime ReservationDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public string? ReservationHour { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending.ToString();
        public DateTime CreatedAt { get; set; }
    }
}
