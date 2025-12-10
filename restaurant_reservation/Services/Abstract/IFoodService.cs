using restaurant_reservation_api.Dto;
using restaurant_reservation.Models;

namespace restaurant_reservation.Services.Abstract
{
    public interface IFoodService
    {
        Task<List<FoodDto>> GetAllFoodsAsync();
        List<FoodDto> GetAllFoods();
        FoodDto? GetFoodById(int id);
        Task<FoodDto> CreateFoodAsync(FoodDto foodDto);
        FoodDto CreateFood(FoodDto foodDto);
        Task UpdateFoodAsync(int id, FoodDto foodDto);
        void UpdateFood(int idk, FoodDto foodDto);
        Task DeleteFoodAsync(int id);
        void DeleteFood(int id);
    }
}
