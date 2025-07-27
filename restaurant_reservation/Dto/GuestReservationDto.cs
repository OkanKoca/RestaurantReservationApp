namespace restaurant_reservation.Dto
{
    public class GuestReservationDto
    {        
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        //public int TableId { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservationDate { get; set; }
        public string ReservationHour { get; set; }
    }
}
