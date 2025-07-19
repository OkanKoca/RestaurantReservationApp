using restaurant_reservation.Models.Abstracts;

namespace restaurant_reservation.Models
{
    public class Drink : Consumable
    {   
        public bool IsAlcoholic { get; set; }
        public bool ContainsCaffeine { get; set; }
        public bool ContainsSugar { get; set; }
    }
}
