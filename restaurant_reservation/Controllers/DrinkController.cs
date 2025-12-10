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
        public ActionResult<List<DrinkDto>> GetAllDrinks()
        {
            var drinks = _drinkService.GetAllDrinks();
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
        public IActionResult Post(DrinkDto drinkDto)
        {
            if (drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdDrink = _drinkService.CreateDrink(drinkDto);
            return CreatedAtAction(nameof(GetDrink), new { id = createdDrink.Id }, createdDrink);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateDrink(int id, DrinkDto drinkDto)
        {
            if (drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _drinkService.UpdateDrink(id, drinkDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _drinkService.DeleteDrink(id);
            return NoContent();
        }
    }
}
