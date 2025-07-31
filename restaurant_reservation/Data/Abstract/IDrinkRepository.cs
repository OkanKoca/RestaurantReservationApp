using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Abstract
{
    public interface IDrinkRepository
    {
        void Add(Drink drink);
        void Update(Drink drink);
        void Delete(int id);
        Drink GetById(int id);
        IQueryable<Drink> Drinks();
    }
}
