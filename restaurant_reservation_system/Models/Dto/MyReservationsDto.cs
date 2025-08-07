namespace restaurant_reservation_system.Models.Dto
{
    public class MyReservationsDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<UserReservationDto> Reservations { get; set; } = new List<UserReservationDto>();
    }
}
