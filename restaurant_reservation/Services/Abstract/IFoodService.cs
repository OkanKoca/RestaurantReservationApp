using restaurant_reservation_api.Dto;
using restaurant_reservation.Models;

namespace restaurant_reservation.Services.Abstract
{
    public interface IFoodService
    {
        List<FoodDto> GetAllFoods();
        FoodDto? GetFoodById(int id);
        FoodDto CreateFood(FoodDto foodDto);
        void UpdateFood(int idk, FoodDto foodDto);
        void DeleteFood(int id);
    }
}
