using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Concrete
{
    public class MenuRepository : IMenuRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public MenuRepository(RestaurantContext restaurantContext) { 
            _restaurantContext = restaurantContext;
        }
        public void Add(Menu menu)
        {
            _restaurantContext.Menus.Add(menu);
            _restaurantContext.SaveChanges();
        }
        public Menu GetById(int id)
        {
            var menu = _restaurantContext.Menus.Include(f => f.Foods)
                .Include(d => d.Drinks)
                .FirstOrDefault(m => m.Id == id);

            if (menu == null)
            {
                throw new KeyNotFoundException($"Menu with ID {id} not found.");
            }

            return menu;
        }
        public void Update(Menu menu)
        {
            if(menu == null)
            {
                throw new ArgumentNullException(nameof(Menu), "Menu cannot be null.");
            }

            _restaurantContext.Menus.Update(menu);
            _restaurantContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var menuToDelete = GetById(id);

            _restaurantContext.Menus.Remove(menuToDelete);
            _restaurantContext.SaveChanges();
        }

        public IQueryable<Menu> Menus()
        {
            return _restaurantContext.Menus.Include(f => f.Foods)
                .Include(d => d.Drinks);
        }


    }
}
