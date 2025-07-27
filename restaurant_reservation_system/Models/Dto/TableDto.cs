namespace restaurant_reservation_system.Models.Dto
{
    public class TableDto
    {
        public int Number { get; set; }
        public int Seats { get; set; }
        public bool IsReserved { get; set; } = false; // It will be changed based on hour of reservation that user selects.

    }
}
