using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
      _menuService = menuService;
        }

     [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
          var menus = await _menuService.GetAllMenusAsync();
            return Ok(menus);
        }

        [HttpGet("{id}")]
        public IActionResult GetMenu(int id)
        {
   var menu = _menuService.GetMenuById(id);
            if (menu == null)
            {
       return NotFound();
    }
        return Ok(menu);
        }

        [HttpGet("{id}/drinks")]
        public ActionResult<List<DrinkDto>> GetMenuDrinks(int id)
 {
            var drinks = _menuService.GetMenuDrinks(id);
  return Ok(drinks);
        }

        [HttpGet("{id}/foods")]
    public ActionResult<List<FoodDto>> GetMenuFoods(int id)
        {
            var foods = _menuService.GetMenuFoods(id);
            return Ok(foods);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
public async Task<IActionResult> Post(MenuDto menuDto)
        {
            if (menuDto == null || !ModelState.IsValid)
  {
  return BadRequest(ModelState);
        }

     var menu = await _menuService.CreateMenuAsync(menuDto);
            return CreatedAtAction(nameof(GetMenu), new { id = menu.Id }, menu);
 }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
    public IActionResult UpdateMenu(int id, MenuDto menuDto)
     {
            if (menuDto == null || !ModelState.IsValid)
      {
              return BadRequest(ModelState);
            }

  try
       {
    _menuService.UpdateMenu(id, menuDto);
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
  _menuService.DeleteMenu(id);
            return NoContent();
    }
    }
}

