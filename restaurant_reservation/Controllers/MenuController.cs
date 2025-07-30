using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;

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
        public MenuController(IMenuRepository menuRepository, IDrinkRepository drinkRepository, IFoodRepository foodRepository)
        {
            _menuRepository = menuRepository;
            _drinkRepository = drinkRepository;
            _foodRepository = foodRepository;
        }

        [HttpGet]
        public List<Menu> GetAllMenus()
        {
            return _menuRepository.Menus();
        }

        // GET api/<MenuController>/5
        [HttpGet("{id}")]
        public Menu GetMenu(int id)
        {
            return _menuRepository.GetById(id);
        }

        // POST api/<MenuController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(MenuDto menuDto)
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

