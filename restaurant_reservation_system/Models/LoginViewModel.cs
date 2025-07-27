using System.ComponentModel.DataAnnotations;

namespace restaurant_reservation_system.Models
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
