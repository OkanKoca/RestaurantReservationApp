namespace restaurant_reservation.Models.Abstracts
{
    public abstract class Consumable
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int Calories { get; set; }
        public float Price { get; set; }
    }
}
