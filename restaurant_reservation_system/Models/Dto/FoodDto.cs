namespace restaurant_reservation_system.Models.Dto
{
    public class FoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Calories { get; set; } = 0;
        public float Price { get; set; }
        public bool IsVegan { get; set; }
        public bool ContainsGluten { get; set; }
    }
}
