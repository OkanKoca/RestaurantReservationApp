using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation_api.Data;

namespace restaurant_reservation.Data.Concrete
{
    public class TableRepository : ITableRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public TableRepository(RestaurantContext restaurantContext) {
            _restaurantContext = restaurantContext;
        }

        public async void Add(Table table)
        {
            _restaurantContext.Tables.Add(table);
            await _restaurantContext.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            _restaurantContext.Tables.Remove(GetById(id));
            _restaurantContext.SaveChanges();
        }

        public IQueryable<Table> Tables()
        {
            return _restaurantContext.Tables.Include(t=> t.UserReservations).Include(t=> t.GuestReservations);
        }

        public Table GetById(int id)
        {
            var table = _restaurantContext.Tables.Include(t=> t.UserReservations).Include(t => t.GuestReservations).FirstOrDefault(t => t.Id == id);

            if (table == null)
            {
                throw new KeyNotFoundException($"Table with ID {id} not found.");
            }

            return table;
        }

        public Table GetByNumber(int number)
        {
            return _restaurantContext.Tables.FirstOrDefault(t => t.Number == number) 
                ?? throw new KeyNotFoundException($"Table with number {number} not found.");
        }

        public async void Update(Table table)
        {
            _restaurantContext.Tables.Update(table);
            await _restaurantContext.SaveChangesAsync();
        }
    }
}
