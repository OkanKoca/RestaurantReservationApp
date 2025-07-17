namespace restaurant_reservation.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; } // Masa numarası
        public int Seats { get; set; } // Oturma kapasitesi
        public bool IsReserved { get; set; } // Rezervasyon durumu
        public Reservation? Reservation { get; set; } // Rezervasyon bilgisi, eğer varsa
        public DateTime? ReservedUntil { get; set; } // Rezervasyonun bitiş zamanı, eğer varsa
        public Table(int number, int seats)
        {
            Number = number;
            Seats = seats;
            IsReserved = false;
            ReservedUntil = null;
        }
    }
}
