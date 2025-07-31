using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Data.Concrete;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrinkController : ControllerBase
    {
        private readonly IDrinkRepository _drinkRepository;
        private readonly IMenuRepository _menuRepository;
        public DrinkController(IDrinkRepository drinkRepository, IMenuRepository menuRepository)
        {
            _drinkRepository = drinkRepository;
            _menuRepository = menuRepository;
        }
        
        [HttpGet]
        public ActionResult<List<DrinkDto>> GetAllDrinks()
        {
            var drinks = _drinkRepository.Drinks().Include(d => d.Menu)
                .Select(d => new DrinkDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Calories = d.Calories,
                    Price = d.Price,
                    MenuId = d.MenuId,
                    IsAlcoholic = d.IsAlcoholic,
                    ContainsCaffeine = d.ContainsCaffeine,
                    ContainsSugar = d.ContainsSugar
                }).ToList();

            return Ok(drinks);
        }

        // GET api/<DrinkController>/5
        [HttpGet("{id}")]
        public ActionResult<DrinkDto> GetDrink(int id)
        {
            var drink = _drinkRepository.GetById(id);

            if(drink == null)
            {
                return NotFound();
            }

            var drinkDto = new DrinkDto
            {
                Id = drink.Id,
                Name = drink.Name,
                Description = drink.Description,
                Calories = drink.Calories,
                Price = drink.Price,
                MenuId = drink.MenuId,
                IsAlcoholic = drink.IsAlcoholic,
                ContainsCaffeine = drink.ContainsCaffeine,
                ContainsSugar = drink.ContainsSugar
            };

            return Ok(drinkDto);
        }

        // POST api/<DrinkController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(DrinkDto drinkDto)
        {
            if(drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var drink = new Drink
            {
                Id = drinkDto.Id,
                Name = drinkDto.Name,
                Description = drinkDto.Description,
                Calories = drinkDto.Calories,
                Price = drinkDto.Price,
                MenuId = drinkDto.MenuId,
                IsAlcoholic = drinkDto.IsAlcoholic,
                ContainsCaffeine = drinkDto.ContainsCaffeine,
                ContainsSugar = drinkDto.ContainsSugar
            };

            var menu = _menuRepository.Menus().FirstOrDefault(m=> m.Id == drinkDto.MenuId);

            if(menu != null)
            {
                drink.Menu = menu;
            }

            _drinkRepository.Add(drink);

            return CreatedAtAction(nameof(GetDrink), new { id = drink.Id }, new DrinkDto
            {
                Id = drinkDto.Id,
                Name = drinkDto.Name,
                Description = drinkDto.Description,
                Price= drinkDto.Price,
                Calories= drinkDto.Calories,
                MenuId= drinkDto.MenuId,
                IsAlcoholic = drinkDto.IsAlcoholic,
                ContainsCaffeine= drinkDto.ContainsCaffeine,
                ContainsSugar = drinkDto.ContainsSugar
            });
        }

        // PUT api/<DrinkController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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
            drinkToUpdate.MenuId = drink.MenuId;
            drinkToUpdate.Menu = _menuRepository.GetById(id) ?? null;
            drinkToUpdate.IsAlcoholic = drink.IsAlcoholic;
            drinkToUpdate.ContainsCaffeine = drink.ContainsCaffeine;
            drinkToUpdate.ContainsSugar = drink.ContainsSugar;

            _drinkRepository.Update(drinkToUpdate);

            return NoContent();
        }

        // DELETE api/<DrinkController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public void Delete(int id)
        {
            _drinkRepository.Delete(id);
        }
    }
}
