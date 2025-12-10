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
        public async Task<ActionResult<List<FoodDto>>> GetAllFoods()
        {
            var foods = await _foodService.GetAllFoodsAsync();
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
        public async Task<IActionResult> Post(FoodDto foodDto)
        {
            if (foodDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdFood = await _foodService.CreateFoodAsync(foodDto);
            return CreatedAtAction(nameof(GetFood), new { id = createdFood.Id }, createdFood);
        }

        // PUT api/<foodController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFood(int id, FoodDto foodDto)
        {
            if (foodDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _foodService.UpdateFoodAsync(id, foodDto);
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
        public async Task<IActionResult> Delete(int id)
        {
            await _foodService.DeleteFoodAsync(id);
            return NoContent();
        }
    }
}
