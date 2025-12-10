using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        public ActionResult<List<FoodDto>> GetAllFoods()
        {
            var foods = _foodService.GetAllFoods();
            return Ok(foods);
        }

        // GET api/<foodController>/5
        [HttpGet("{id}")]
        public ActionResult<FoodDto> GetFood(int id)
        {
            var food = _foodService.GetFoodById(id);
            if (food == null)
            {
                return NotFound();
            }
            return Ok(food);
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

            var createdFood = _foodService.CreateFood(foodDto);
            return CreatedAtAction(nameof(GetFood), new { id = createdFood.Id }, createdFood);
        }

        // PUT api/<foodController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateFood(int id, FoodDto foodDto)
        {
            if (foodDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _foodService.UpdateFood(id, foodDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE api/<foodController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _foodService.DeleteFood(id);
            return NoContent();
        }
    }
}
