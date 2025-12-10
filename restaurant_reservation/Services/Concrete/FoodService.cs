using AutoMapper;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Services.Concrete
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public FoodService(IFoodRepository foodRepository,IMenuRepository menuRepository,IMapper mapper)
        {
            _foodRepository = foodRepository;
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public List<FoodDto> GetAllFoods()
        {
            var foods = _foodRepository.Foods()
                .Include(f => f.Menu)
                .ToList();

            return _mapper.Map<List<FoodDto>>(foods);
        }

        public FoodDto? GetFoodById(int id)
        {
            var food = _foodRepository.GetById(id);
            if (food == null)
            {
                return null;
            }

            return _mapper.Map<FoodDto>(food);
        }

        public FoodDto CreateFood(FoodDto foodDto)
        {
            var food = _mapper.Map<Food>(foodDto);

            var menu = _menuRepository.Menus().FirstOrDefault(m => m.Id == food.MenuId);
            if (menu != null)
            {
                food.Menu = menu;
            }

            _foodRepository.Add(food);

            return _mapper.Map<FoodDto>(food);
        }

        public void UpdateFood(int id, FoodDto foodDto)
        {
            var foodToUpdate = _foodRepository.GetById(id);
            if (foodToUpdate == null)
            {
                throw new KeyNotFoundException($"Food with ID {id} not found.");
            }

            _mapper.Map(foodDto, foodToUpdate);
            foodToUpdate.Menu = _menuRepository.GetById(foodDto.MenuId) ?? null;

            _foodRepository.Update(foodToUpdate);
        }

        public void DeleteFood(int id)
        {
            _foodRepository.Delete(id);
        }
    }
}
