using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;
using System.Text.Json;

namespace restaurant_reservation.Services.Concrete
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IDrinkRepository _drinkRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;

        private const string MenuCacheKey = "menu_list";

        public MenuService(
            IMenuRepository menuRepository,
            IDrinkRepository drinkRepository,
            IFoodRepository foodRepository,
            IDistributedCache distributedCache,
            IMapper mapper)
        {
            _menuRepository = menuRepository;
            _drinkRepository = drinkRepository;
            _foodRepository = foodRepository;
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        public async Task<List<MenuDto>> GetAllMenusAsync()
        {
            var cachedData = await _distributedCache.GetStringAsync(MenuCacheKey);

            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<List<MenuDto>>(cachedData) ?? new List<MenuDto>();
            }

            var menuList = await _menuRepository.Menus()
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    DrinkIds = m.Drinks.Select(d => d.Id).ToList(),
                    FoodIds = m.Foods.Select(f => f.Id).ToList()
                })
                .ToListAsync();

            await _distributedCache.SetStringAsync(MenuCacheKey,
                JsonSerializer.Serialize(menuList),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
                });

            return menuList;
        }

        public Menu? GetMenuById(int id)
        {
            return _menuRepository.GetById(id);
        }

        public List<DrinkDto> GetMenuDrinks(int id)
        {
            var menu = _menuRepository.GetById(id);
            if (menu?.Drinks == null)
            {
                return new List<DrinkDto>();
            }

            return _mapper.Map<List<DrinkDto>>(menu.Drinks);
        }

        public List<FoodDto> GetMenuFoods(int id)
        {
            var menu = _menuRepository.GetById(id);
            if (menu?.Foods == null)
            {
                return new List<FoodDto>();
            }

            return _mapper.Map<List<FoodDto>>(menu.Foods);
        }

        public async Task<Menu> CreateMenuAsync(MenuDto menuDto)
        {
            var drinks = GetDrinkList(menuDto.DrinkIds);
            var foods = GetFoodList(menuDto.FoodIds);

            var menu = new Menu
            {
                Name = menuDto.Name,
                Description = menuDto.Description,
                Drinks = drinks,
                Foods = foods
            };

            _menuRepository.Add(menu);

            await _distributedCache.RemoveAsync(MenuCacheKey);

            return menu;
        }

        public async void UpdateMenu(int id, MenuDto menuDto)
        {
            var drinks = GetDrinkList(menuDto.DrinkIds);
            var foods = GetFoodList(menuDto.FoodIds);

            var menuToUpdate = _menuRepository.GetById(id);
            if (menuToUpdate == null)
            {
                throw new KeyNotFoundException($"Menu with ID {id} not found.");
            }

            menuToUpdate.Name = menuDto.Name;
            menuToUpdate.Description = menuDto.Description;
            menuToUpdate.Drinks = drinks;
            menuToUpdate.Foods = foods;

            _menuRepository.Update(menuToUpdate);

            await _distributedCache.RemoveAsync(MenuCacheKey);
        }

        public async void DeleteMenu(int id)
        {
            _menuRepository.Delete(id);
            await _distributedCache.RemoveAsync(MenuCacheKey);
        }

        private List<Drink> GetDrinkList(List<int> drinkIds)
        {
            var drinks = new List<Drink>();
            foreach (var drinkId in drinkIds)
            {
                var drinkItem = _drinkRepository.GetById(drinkId);
                if (drinkItem != null)
                {
                    drinks.Add(drinkItem);
                }
            }
            return drinks;
        }

        private List<Food> GetFoodList(List<int> foodIds)
        {
            var foods = new List<Food>();
            foreach (var foodId in foodIds)
            {
                var foodItem = _foodRepository.GetById(foodId);
                if (foodItem != null)
                {
                    foods.Add(foodItem);
                }
            }
            return foods;
        }
    }
}
