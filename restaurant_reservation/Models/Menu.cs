namespace restaurant_reservation.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<Food> Foods { get; set; } = new List<Food>();
        public List<Drink> Drinks { get; set; } = new List<Drink>();
    }
}
