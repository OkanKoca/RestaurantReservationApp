namespace restaurant_reservation.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Seats { get; set; }
        public bool IsReserved { get; set; } = false;
        public Reservation? Reservation { get; set; }
        public DateTime? ReservedUntil { get; set; } 
    }
}
