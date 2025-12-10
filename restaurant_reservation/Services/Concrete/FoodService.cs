using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;
using System.Text.Json;

namespace restaurant_reservation.Services.Concrete
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;

        private const string FoodsCacheKey = "foods_list";

        public FoodService(
            IFoodRepository foodRepository,
            IMenuRepository menuRepository,
            IDistributedCache distributedCache,
            IMapper mapper)
        {
            _foodRepository = foodRepository;
            _menuRepository = menuRepository;
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        public async Task<List<FoodDto>> GetAllFoodsAsync()
        {
            var cachedData = await _distributedCache.GetStringAsync(FoodsCacheKey);

            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<List<FoodDto>>(cachedData) ?? new List<FoodDto>();
            }

            var foods = _foodRepository.Foods()
                .Include(f => f.Menu)
                .ToList();

            var foodDtos = _mapper.Map<List<FoodDto>>(foods);

            await _distributedCache.SetStringAsync(FoodsCacheKey,
                JsonSerializer.Serialize(foodDtos),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
                });

            return foodDtos;
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

        public async Task<FoodDto> CreateFoodAsync(FoodDto foodDto)
        {
            var food = _mapper.Map<Food>(foodDto);

            var menu = _menuRepository.Menus().FirstOrDefault(m => m.Id == food.MenuId);
            if (menu != null)
            {
                food.Menu = menu;
            }

            _foodRepository.Add(food);
            await _distributedCache.RemoveAsync(FoodsCacheKey);

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

        public async Task UpdateFoodAsync(int id, FoodDto foodDto)
        {
            var foodToUpdate = _foodRepository.GetById(id);
            if (foodToUpdate == null)
            {
                throw new KeyNotFoundException($"Food with ID {id} not found.");
            }

            _mapper.Map(foodDto, foodToUpdate);
            foodToUpdate.Menu = _menuRepository.GetById(foodDto.MenuId) ?? null;

            _foodRepository.Update(foodToUpdate);
            await _distributedCache.RemoveAsync(FoodsCacheKey);
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

        public async Task DeleteFoodAsync(int id)
        {
            _foodRepository.Delete(id);
            await _distributedCache.RemoveAsync(FoodsCacheKey);
        }

        public void DeleteFood(int id)
        {
            _foodRepository.Delete(id);
        }
    }
}
