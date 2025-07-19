using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Abstract
{
    public interface IFoodRepository
    {
        void Add(Food food);
        void Update(Food food);
        void Delete(int id);
        Food GetById(int id);
        List<Food> Foods();
    }
}
