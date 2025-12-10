using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Models;
using System.Reflection.Emit;

namespace restaurant_reservation_api.Data
{
    public class RestaurantContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppRole>().HasData(
                new AppRole { Id = 1, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "abc" },
                new AppRole { Id = 2, Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "def" }
            );
        }

        public DbSet<Table> Tables { get; set; } = null!;
        public DbSet<UserReservation> Reservations { get; set; } = null!;
        public DbSet<GuestReservation> GuestReservations { get; set; } = null!;
        public DbSet<Food> Foods { get; set; } = null!;
        public DbSet<Drink> Drinks { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;

        
    }
}
