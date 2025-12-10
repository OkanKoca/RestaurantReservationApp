using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Models;
using restaurant_reservation.Dto;
using Microsoft.AspNetCore.Authorization;
using restaurant_reservation_api.Dto;
using restaurant_reservation.Services.Abstract;

namespace restaurant_reservation.Controllers
{
  [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;

   public TableController(ITableService tableService)
 {
  _tableService = tableService;
 }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<TableDto>> GetAllTables()
        {
            var tables = _tableService.GetAllTables();
 return Ok(tables);
        }

        [HttpGet("{id}")]
 [Authorize(Roles = "Admin")]
        public ActionResult<Table> GetTable(int id)
        {
            var table = _tableService.GetTableById(id);
       if (table == null)
            {
                return NotFound();
            }
      return Ok(table);
        }

        [HttpGet("{id}/UserReservations")]
        [Authorize(Roles = "Admin")]
   public ActionResult<List<UserReservationDto>> GetTableUserReservations(int id, [FromQuery] DateTime? dateTime = null)
        {
   var table = _tableService.GetTableById(id);
            if (table == null)
            {
    return NotFound();
          }

            var userReservations = _tableService.GetTableUserReservations(id, dateTime);
   return Ok(userReservations);
        }

        [HttpGet("{id}/GuestReservations")]
     [Authorize(Roles = "Admin")]
      public ActionResult<List<AdminGuestReservationTableDto>> GetTableGuestReservations(int id, [FromQuery] DateTime? dateTime = null)
        {
            var table = _tableService.GetTableById(id);
    if (table == null)
            {
         return NotFound();
            }

        var guestReservations = _tableService.GetTableGuestReservations(id, dateTime);
      return Ok(guestReservations);
   }

        [HttpGet("{id}/Occupancy")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTableOccupancy(int id, [FromQuery] DateTime dateTime)
        {
            var rate = _tableService.GetTableOccupancy(id, dateTime);
     return new JsonResult(rate);
        }

  [HttpPost]
     [Authorize(Roles = "Admin")]
        public IActionResult CreateTable(TableDto tableDto)
  {
         if (tableDto == null || !ModelState.IsValid)
    {
       return BadRequest(ModelState);
    }

var table = _tableService.CreateTable(tableDto);
        return CreatedAtAction(nameof(GetTable), new { id = table.Id }, table);
        }

      [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateTable(int id, TableDto tableDto)
 {
            if (tableDto == null || !ModelState.IsValid)
        {
      return BadRequest(ModelState);
            }

            try
            {
      _tableService.UpdateTable(id, tableDto);
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
   if (!_tableService.DeleteTable(id))
            {
      return NotFound($"Table with ID {id} not found.");
            }
         return NoContent();
        }
    }
}
