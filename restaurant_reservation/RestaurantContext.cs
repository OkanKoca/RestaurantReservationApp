using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Models;
using System.Reflection.Emit;

namespace restaurant_reservation
{
    public class RestaurantContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Seed data can be added here if needed
            // Example:
            // builder.Entity<Table>().HasData(
            //     new Table { Id = 1, Name = "Table 1", Capacity = 4 },
            //     new Table { Id = 2, Name = "Table 2", Capacity = 2 }
            // );

            builder.Entity<AppRole>().HasData(
                new AppRole { Id = 1, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "abc" },
                new AppRole { Id = 2, Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "def" }
            );

        }

        public DbSet<Table> Tables { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Food> Foods { get; set; } = null!;
        public DbSet<Drink> Drinks { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;

        
    }
}
