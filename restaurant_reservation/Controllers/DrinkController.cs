using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrinkController : ControllerBase
    {
        private readonly IDrinkService _drinkService;

        public DrinkController(IDrinkService drinkService)
        {
            _drinkService = drinkService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DrinkDto>>> GetAllDrinks()
        {
            var drinks = await _drinkService.GetAllDrinksAsync();
            return Ok(drinks);
        }

        [HttpGet("{id}")]
        public ActionResult<DrinkDto> GetDrink(int id)
        {
            var drink = _drinkService.GetDrinkById(id);
            if (drink == null)
            {
                return NotFound();
            }
            return Ok(drink);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(DrinkDto drinkDto)
        {
            if (drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdDrink = await _drinkService.CreateDrinkAsync(drinkDto);
            return CreatedAtAction(nameof(GetDrink), new { id = createdDrink.Id }, createdDrink);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDrink(int id, DrinkDto drinkDto)
        {
            if (drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _drinkService.UpdateDrinkAsync(id, drinkDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _drinkService.DeleteDrinkAsync(id);
            return NoContent();
        }
    }
}
