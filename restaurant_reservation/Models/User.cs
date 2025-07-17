namespace restaurant_reservation.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get => $"{FirstName} {LastName}"; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public string Role { get; set; } = UserRole.Customer.ToString(); 
    }

    public enum UserRole
    {
        Customer, 
        Admin   
    }
}
