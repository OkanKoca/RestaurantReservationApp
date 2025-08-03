namespace restaurant_reservation.Dto
{
    public class TableDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Seats { get; set; }
        public bool IsReserved { get; set; } = false; // It will be changed based on hour of reservation that user selects.
    }
}
