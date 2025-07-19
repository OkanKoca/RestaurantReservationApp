using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrinkController : ControllerBase
    {
        private readonly IDrinkRepository _drinkRepository;
        public DrinkController(IDrinkRepository drinkRepository)
        {
            _drinkRepository = drinkRepository;
        }

        
        [HttpGet]
        public List<Drink> GetAllDrinks()
        {
            return _drinkRepository.Drinks();
        }

        // GET api/<DrinkController>/5
        [HttpGet("{id}")]
        public Drink GetDrink(int id)
        {
            return _drinkRepository.GetById(id);
        }

        // POST api/<DrinkController>
        [HttpPost]
        public IActionResult Post(DrinkDto drinkDto)
        {
            if(drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var drink = new Drink
            {
                Name = drinkDto.Name,
                Description = drinkDto.Description,
                Calories = drinkDto.Calories,
                Price = drinkDto.Price,
                IsAlcoholic = drinkDto.IsAlcoholic,
                ContainsCaffeine = drinkDto.ContainsCaffeine,
                ContainsSugar = drinkDto.ContainsSugar
            };

            _drinkRepository.Add(drink);

            return CreatedAtAction(nameof(GetDrink), new { id = drink.Id }, drink);
        }

        // PUT api/<DrinkController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateDrink(int id, DrinkDto drinkDto)
        {
            if (drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var drink = _drinkRepository.GetById(id);

            drink.Name = drinkDto.Name;
            drink.Description = drinkDto.Description;
            drink.Calories = drinkDto.Calories;
            drink.Price = drinkDto.Price;
            drink.IsAlcoholic = drinkDto.IsAlcoholic;
            drink.ContainsCaffeine = drinkDto.ContainsCaffeine;
            drink.ContainsSugar = drinkDto.ContainsSugar;

            _drinkRepository.Update(drink);

            return NoContent();
        }

        // DELETE api/<DrinkController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _drinkRepository.Delete(id);
        }
    }
}
