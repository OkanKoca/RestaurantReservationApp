using restaurant_reservation_api.Dto;
using restaurant_reservation.Models;

namespace restaurant_reservation.Services.Abstract
{
    public interface IDrinkService
    {
        Task<List<DrinkDto>> GetAllDrinksAsync();
        List<DrinkDto> GetAllDrinks();
        DrinkDto? GetDrinkById(int id);
        Task<DrinkDto> CreateDrinkAsync(DrinkDto drinkDto);
        DrinkDto CreateDrink(DrinkDto drinkDto);
        Task UpdateDrinkAsync(int id, DrinkDto drinkDto);
        void UpdateDrink(int id, DrinkDto drinkDto);
        Task DeleteDrinkAsync(int id);
        void DeleteDrink(int id);
    }
}
