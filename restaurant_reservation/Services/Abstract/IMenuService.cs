using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Services.Abstract
{
    public interface IMenuService
    {
        Task<List<MenuDto>> GetAllMenusAsync();
        Menu? GetMenuById(int id);
        List<DrinkDto> GetMenuDrinks(int id);
        List<FoodDto> GetMenuFoods(int id);
        Task<Menu> CreateMenuAsync(MenuDto menuDto);
        void UpdateMenu(int id, MenuDto menuDto);
        void DeleteMenu(int id);
    }
}
