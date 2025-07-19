using restaurant_reservation.Models.Abstracts;

namespace restaurant_reservation.Models
{
    public class Food : Consumable
    {
        public bool IsVegan { get; set; }
        public bool ContainsGluten { get; set; }
    }
}
