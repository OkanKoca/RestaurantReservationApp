namespace restaurant_reservation_system.Models.ViewModel
{
    public class AdminReservationsViewModel
    {
        public List<UserReservationViewModel> UserReservations { get; set; } = new List<UserReservationViewModel>();
        public List<AdminGuestReservationViewModel> GuestReservations { get; set; } = new List<AdminGuestReservationViewModel>(); 
    }
}
