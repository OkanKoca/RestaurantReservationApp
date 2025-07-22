using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Abstract
{
    public interface IMenuRepository
    {
        void Add(Menu menu);
        void Update(Menu menu);
        void Delete(int id);
        Menu GetById(int id);
        List<Menu> Menus();
    }
}
