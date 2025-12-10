using restaurant_reservation_api.Dto;
using restaurant_reservation.Models;

namespace restaurant_reservation.Services.Abstract
{
    public interface IDrinkService
    {
        List<DrinkDto> GetAllDrinks();
        DrinkDto? GetDrinkById(int id);
        DrinkDto CreateDrink(DrinkDto drinkDto);
        void UpdateDrink(int id, DrinkDto drinkDto);
        void DeleteDrink(int id);
    }
}
