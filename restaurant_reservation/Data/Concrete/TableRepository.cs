using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Concrete
{
    public class TableRepository : ITableRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public TableRepository(RestaurantContext restaurantContext) {
            _restaurantContext = restaurantContext;
        }

        public void Add(Table table)
        {
            _restaurantContext.Tables.Add(table);
            _restaurantContext.SaveChanges();
        }

        public void Delete(int id)
        {
            _restaurantContext.Tables.Remove(GetById(id));
            _restaurantContext.SaveChanges();
        }

        public List<Table> Tables()
        {
            return _restaurantContext.Tables.ToList();
        }

        public Table GetById(int id)
        {
            return _restaurantContext.Tables.Find(id) ?? throw new KeyNotFoundException($"Table with ID {id} not found.");
        }

        public Table GetByNumber(int number)
        {
            return _restaurantContext.Tables.FirstOrDefault(t => t.Number == number) 
                ?? throw new KeyNotFoundException($"Table with number {number} not found.");
        }

        public void Update(Table table)
        {
            _restaurantContext.Tables.Update(table);
            _restaurantContext.SaveChanges();
        }
    }
}
