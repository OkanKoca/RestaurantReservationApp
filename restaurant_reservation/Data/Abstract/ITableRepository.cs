using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Abstract
{
    public interface ITableRepository
    {
        void Add(Table table);
        void Update(Table table);
        void Delete(int id);
        Table GetById(int id);
        Table GetByNumber(int number);
        IQueryable<Table> Tables(); 
    }
}
