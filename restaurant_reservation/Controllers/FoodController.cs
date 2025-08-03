using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMenuRepository _menuRepository;
        public FoodController(IFoodRepository foodRepository, IMenuRepository menuRepository)
        {
            _foodRepository = foodRepository;
            _menuRepository = menuRepository;
        }
        
        [HttpGet]
        public ActionResult<List<FoodDto>> GetAllFoods()
        {
            var foods = _foodRepository.Foods()
                .Include(f => f.Menu)
                .Select(f => new FoodDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Calories = f.Calories,
                    Price = f.Price,
                    MenuId = f.MenuId,
                    IsVegan = f.IsVegan,
                    ContainsGluten = f.ContainsGluten
                })
                .ToList();

            return Ok(foods);
        }

        // GET api/<foodController>/5
        [HttpGet("{id}")]
        public ActionResult<FoodDto> GetFood(int id)
        {
            var food = _foodRepository.GetById(id);
            if (food == null)
            {
                return NotFound();
            }

            var foodDto = new FoodDto
            {
                Id = food.Id,
                Name = food.Name,
                Description = food.Description,
                Calories = food.Calories,
                Price = food.Price,
                MenuId = food.MenuId,
                IsVegan = food.IsVegan,
                ContainsGluten = food.ContainsGluten
            };

            return Ok(foodDto);
        }

        // POST api/<foodController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(FoodDto foodDto)
        {
            if (foodDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var food = new Food
            {
                Name = foodDto.Name,
                Description = foodDto.Description,
                Calories = foodDto.Calories,
                Price = foodDto.Price,
                MenuId = foodDto.MenuId,
                IsVegan = foodDto.IsVegan,
                ContainsGluten = foodDto.ContainsGluten
            };

            var menu = _menuRepository.Menus().FirstOrDefault(m => m.Id == food.MenuId);

            if (menu != null)
            {
                food.Menu = menu;
            }

            _foodRepository.Add(food);

            var createdFoodDto = new FoodDto
            {
                Id = food.Id,
                Name = food.Name,
                Description = food.Description,
                Calories = food.Calories,
                Price = food.Price,
                MenuId = food.MenuId,
                IsVegan = food.IsVegan,
                ContainsGluten = food.ContainsGluten
            };

            return CreatedAtAction(nameof(GetFood), new { id = food.Id }, createdFoodDto);
        }

        // PUT api/<foodController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Updatefood(int id, Food food)
        {
            if (food == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var foodToUpdate = _foodRepository.GetById(id);

            foodToUpdate.Name = food.Name;
            foodToUpdate.Description = food.Description;
            foodToUpdate.Calories = food.Calories;
            foodToUpdate.Price = food.Price;
            foodToUpdate.MenuId = food.MenuId;
            foodToUpdate.Menu = _menuRepository.GetById(food.MenuId) ?? null;
            foodToUpdate.IsVegan = food.IsVegan;
            foodToUpdate.ContainsGluten = food.ContainsGluten;

            _foodRepository.Update(foodToUpdate);

            return NoContent();
        }

        // DELETE api/<foodController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public void Delete(int id)
        {
            _foodRepository.Delete(id);
        }
    }
}
