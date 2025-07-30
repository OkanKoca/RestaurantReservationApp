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
    public class FoodController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        public FoodController(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }
        
        [HttpGet]
        public List<Food> GetAllFoods()
        {
            return _foodRepository.Foods();
        }

        // GET api/<foodController>/5
        [HttpGet("{id}")]
        public Food GetFood(int id)
        {
            return _foodRepository.GetById(id);
        }

        // POST api/<foodController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(Food food)
        {
            if(food == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _foodRepository.Add(food);

            return CreatedAtAction(nameof(GetFood), new { id = food.Id }, food);
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
