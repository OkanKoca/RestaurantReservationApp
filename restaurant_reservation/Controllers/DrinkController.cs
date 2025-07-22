using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Data.Abstract;
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
        public IActionResult Post(Drink drink)
        {
            if(drink == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _drinkRepository.Add(drink);

            return CreatedAtAction(nameof(GetDrink), new { id = drink.Id }, drink);
        }

        // PUT api/<DrinkController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateDrink(int id, Drink drink)
        {
            if (drink == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var drinkToUpdate = _drinkRepository.GetById(id);

            drinkToUpdate.Name = drink.Name;
            drinkToUpdate.Description = drink.Description;
            drinkToUpdate.Calories = drink.Calories;
            drinkToUpdate.Price = drink.Price;
            drinkToUpdate.IsAlcoholic = drink.IsAlcoholic;
            drinkToUpdate.ContainsCaffeine = drink.ContainsCaffeine;
            drinkToUpdate.ContainsSugar = drink.ContainsSugar;

            _drinkRepository.Update(drinkToUpdate);

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
