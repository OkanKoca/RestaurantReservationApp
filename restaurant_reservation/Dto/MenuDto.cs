namespace restaurant_reservation.Dto
{
    public class MenuDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<int> DrinkIds { get; set; } = new List<int>();
        public List<int> FoodIds { get; set; } = new List<int>();
    }
}
