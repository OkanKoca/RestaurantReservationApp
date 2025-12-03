namespace restaurant_reservation_system.Models.ViewModel
{
 public class AdminDashboardViewModel
 {
 public int TodayTotalReservations { get; set; }
 public int TodayUserReservations { get; set; }
 public int TodayGuestReservations { get; set; }
 public int TotalUsers { get; set; }
 public int PendingReservations { get; set; }
 public int ConfirmedReservations { get; set; }
 }
}
