using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IDrinkRepository _drinkRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;

        public MenuController(IMenuRepository menuRepository, IDrinkRepository drinkRepository, IFoodRepository foodRepository, IDistributedCache distributedCache, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _drinkRepository = drinkRepository;
            _foodRepository = foodRepository;
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            var cacheKey = "menu_list";
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);

            if (cachedData != null)
            {
                var menus = JsonSerializer.Deserialize<List<MenuDto>>(cachedData);
                return Ok(menus);
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

            await _distributedCache.SetStringAsync(cacheKey,
                JsonSerializer.Serialize(menuList),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
                });

            return Ok(menuList);

        }

        // GET api/<MenuController>/5
        [HttpGet("{id}")]
        public Menu GetMenu(int id)
        {
            return _menuRepository.GetById(id);
        }

        [HttpGet("{id}/drinks")]
        public List<DrinkDto> GetMenuDrinks(int id)
        {
            var menu = _menuRepository.GetById(id);
            if (menu?.Drinks == null)
            {
                return new List<DrinkDto>();
            }

            return _mapper.Map<List<DrinkDto>>(menu.Drinks);
        }

        [HttpGet("{id}/foods")]
        public List<FoodDto> GetMenuFood(int id)
        {
            var menu = _menuRepository.GetById(id);
            if (menu?.Foods == null)
            {
                return new List<FoodDto>();
            }

            return _mapper.Map<List<FoodDto>>(menu.Foods);
        }

        // POST api/<MenuController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(MenuDto menuDto)
        {
            if (menuDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Drink> drinks = GetDrinkList(menuDto.DrinkIds);
            List<Food> foods = GetFoodList(menuDto.FoodIds);

            var menu = new Menu
            {
                Name = menuDto.Name,
                Description = menuDto.Description,
                Drinks = drinks,
                Foods = foods
            };

            _menuRepository.Add(menu);

            await _distributedCache.RemoveAsync("menu_list");

            return CreatedAtAction(nameof(GetMenu), new { id = menu.Id }, menu);
        }

        // PUT api/<MenuController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateMenu(int id, MenuDto menuDto)
        {
            if (menuDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Drink> drinks = GetDrinkList(menuDto.DrinkIds);
            List<Food> foods = GetFoodList(menuDto.FoodIds);

            var menuToUpdate = _menuRepository.GetById(id);
            menuToUpdate.Name = menuDto.Name;
            menuToUpdate.Description = menuDto.Description;
            menuToUpdate.Drinks = drinks;
            menuToUpdate.Foods = foods;

            _menuRepository.Update(menuToUpdate);

            return NoContent();
        }

        // DELETE api/<MenuController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public void Delete(int id)
        {
            _menuRepository.Delete(id);
        }

        private List<Drink> GetDrinkList(List<int> drinkIds)
        {
            List<Drink> drinks = new List<Drink>();
            foreach (int drinkId in drinkIds)
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
            List<Food> foods = new List<Food>();
            foreach (int foodId in foodIds)
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

