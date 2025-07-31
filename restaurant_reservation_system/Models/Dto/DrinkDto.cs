using restaurant_reservation.Migrations;

namespace restaurant_reservation_system.Models.Dto
{
    public class DrinkDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Calories { get; set; } = 0;
        public float Price { get; set; }
        public int MenuId { get; set; }
        public bool IsAlcoholic { get; set; }
        public bool ContainsCaffeine { get; set; }
        public bool ContainsSugar { get; set; }
    }
}
