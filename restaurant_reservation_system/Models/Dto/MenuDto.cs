namespace restaurant_reservation_system.Models.Dto
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<int> DrinkIds { get; set; } = new List<int>();
        public List<int> FoodIds { get; set; } = new List<int>();

    }
}
