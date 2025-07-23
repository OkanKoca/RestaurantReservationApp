using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Abstract
{
    public interface IUserRepository
    {
        void Add();
        void Update(Food food);
        void Delete(int id);
        Food GetById(int id);
        List<Food> Foods();
    }
}
