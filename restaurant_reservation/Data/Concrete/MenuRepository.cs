using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation_api.Data;

namespace restaurant_reservation.Data.Concrete
{
    public class MenuRepository : IMenuRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public MenuRepository(RestaurantContext restaurantContext) { 
            _restaurantContext = restaurantContext;
        }
        public async void Add(Menu menu)
        {
            _restaurantContext.Menus.Add(menu);
            await _restaurantContext.SaveChangesAsync();
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
        public async void Update(Menu menu)
        {
            if(menu == null)
            {
                throw new ArgumentNullException(nameof(Menu), "Menu cannot be null.");
            }

            _restaurantContext.Menus.Update(menu);
            await _restaurantContext.SaveChangesAsync();
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
