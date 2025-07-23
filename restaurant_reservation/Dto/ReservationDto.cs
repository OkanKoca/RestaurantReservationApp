using restaurant_reservation.Models;

namespace restaurant_reservation.Dto
{
    public class ReservationDto // login gerekli (üyelere özel reservation)
    {
        public AppUser Customer { get; set; } = null!;
        public required DateTime ReservationDate { get; set; }
        public required int NumberOfGuests { get; set; }
    }
}
