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
        public IActionResult Post(FoodDto foodDto)
        {
            if(foodDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var food = new Food
            {
                Name = foodDto.Name,
                Description = foodDto.Description,
                Calories = foodDto.Calories,
                Price = foodDto.Price,
                IsVegan = foodDto.IsVegan,
                ContainsGluten = foodDto.ContainsGluten
            };

            _foodRepository.Add(food);

            return CreatedAtAction(nameof(GetFood), new { id = food.Id }, food);
        }

        // PUT api/<foodController>/5
        [HttpPut("{id}")]
        public IActionResult Updatefood(int id, FoodDto foodDto)
        {
            if (foodDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var food = _foodRepository.GetById(id);

            food.Name = foodDto.Name;
            food.Description = foodDto.Description;
            food.Calories = foodDto.Calories;
            food.Price = foodDto.Price;
            food.IsVegan = foodDto.IsVegan;
            food.ContainsGluten = foodDto.ContainsGluten;

            _foodRepository.Update(food);

            return NoContent();
        }

        // DELETE api/<foodController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _foodRepository.Delete(id);
        }
    }
}
