namespace restaurant_reservation.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Seats { get; set; }
        public bool IsReserved { get; set; } = false;
        public List<Reservation>? UserReservations { get; set; }  = new List<Reservation>(); 
        public List<GuestReservation>? GuestReservations { get; set; }  = new List<GuestReservation>();
        public DateTime? ReservedUntil { get; set; } 
    }
}
