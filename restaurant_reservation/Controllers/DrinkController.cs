using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Data.Concrete;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrinkController : ControllerBase
    {
        private readonly IDrinkRepository _drinkRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public DrinkController(IDrinkRepository drinkRepository, IMenuRepository menuRepository, IMapper mapper)
        {
            _drinkRepository = drinkRepository;
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<DrinkDto>> GetAllDrinks()
        {
            var drinks = _drinkRepository.Drinks()
                .Include(d => d.Menu)
                .ToList();

            return Ok(_mapper.Map<List<DrinkDto>>(drinks));
        }

        // GET api/<DrinkController>/5
        [HttpGet("{id}")]
        public ActionResult<DrinkDto> GetDrink(int id)
        {
            var drink = _drinkRepository.GetById(id);

            if (drink == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DrinkDto>(drink));
        }

        // POST api/<DrinkController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(DrinkDto drinkDto)
        {
            if (drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var drink = _mapper.Map<Drink>(drinkDto);

            var menu = _menuRepository.Menus().FirstOrDefault(m => m.Id == drinkDto.MenuId);
            if (menu != null)
            {
                drink.Menu = menu;
            }

            _drinkRepository.Add(drink);

            return CreatedAtAction(nameof(GetDrink), new { id = drink.Id }, _mapper.Map<DrinkDto>(drink));
        }

        // PUT api/<DrinkController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateDrink(int id, DrinkDto drinkDto)
        {
            if (drinkDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var drinkToUpdate = _drinkRepository.GetById(id);
            if (drinkToUpdate == null)
            {
                return NotFound();
            }

            _mapper.Map(drinkDto, drinkToUpdate);
            drinkToUpdate.Menu = _menuRepository.GetById(drinkDto.MenuId) ?? null;

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
