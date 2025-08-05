using System.ComponentModel.DataAnnotations;

namespace restaurant_reservation_system.Models.ViewModel
{
    public class UserReservationViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ReservationDateLocal => ReservationDate.ToLocalTime();
        public int NumberOfGuests { get; set; }
        public string Status { get; set; } = "Pending";
        [Required]
        [Display(Name = "Reservation Hour")]
        [FutureDateTime<UserReservationViewModel>]
        public string? ReservationHour { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedAtLocal => CreatedAt.ToLocalTime();
    }
}
