namespace restaurant_reservation_system.Models.Dto
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get => $"{FirstName} {LastName}"; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
