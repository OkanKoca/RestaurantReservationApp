using Microsoft.EntityFrameworkCore;

namespace restaurant_reservation.Models
{
    public class RestaurantContext : DbContext
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options)
        {

        }
     
        public DbSet<Table> Tables { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Food> Food { get; set; } = null!;
        public DbSet<Drink> Drinks { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;
    }
}
