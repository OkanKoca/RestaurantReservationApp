using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public FoodController(IFoodRepository foodRepository, IMenuRepository menuRepository, IMapper mapper)
        {
            _foodRepository = foodRepository;
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<FoodDto>> GetAllFoods()
        {
            var foods = _foodRepository.Foods()
               .Include(f => f.Menu)
                    .ToList();

            return Ok(_mapper.Map<List<FoodDto>>(foods));
        }

        // GET api/<foodController>/5
        [HttpGet("{id}")]
        public ActionResult<FoodDto> GetFood(int id)
        {
            var food = _foodRepository.GetById(id);
            if (food == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<FoodDto>(food));
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

            var food = _mapper.Map<Food>(foodDto);

            var menu = _menuRepository.Menus().FirstOrDefault(m => m.Id == food.MenuId);
            if (menu != null)
            {
                food.Menu = menu;
            }

            _foodRepository.Add(food);

            var createdFoodDto = _mapper.Map<FoodDto>(food);

            return CreatedAtAction(nameof(GetFood), new { id = food.Id }, createdFoodDto);
        }

        // PUT api/<foodController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Updatefood(int id, FoodDto foodDto)
        {
            if (foodDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var foodToUpdate = _foodRepository.GetById(id);
            if (foodToUpdate == null)
            {
                return NotFound();
            }

            _mapper.Map(foodDto, foodToUpdate);
            foodToUpdate.Menu = _menuRepository.GetById(foodDto.MenuId) ?? null;

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
