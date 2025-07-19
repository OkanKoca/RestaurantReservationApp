using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Models;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;

        public TableController(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        [HttpGet]
        public List<Table> GetAllTables()
        {
            return _tableRepository.Tables();
        }

        // GET api/<TableController>/5
        [HttpGet("{id}")]
        public Table GetTable(int id)
        {
            return _tableRepository.GetById(id);
        }

        // POST api/<TableController>
        [HttpPost]
        public IActionResult CreateTable(TableDto tableDto)
        {
            if (tableDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var table = new Table
            {
                Number = tableDto.Number,
                Seats = tableDto.Seats
            };

            _tableRepository.Add(table);
            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, table);
        }

        // PUT api/<TableController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateTable(int id, TableDto tableDto)
        {
            if (tableDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var table = _tableRepository.GetById(id);

            table.Number = tableDto.Number;
            table.Seats = tableDto.Seats;

            _tableRepository.Update(table);

            return NoContent(); 
        }

        // DELETE api/<TableController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_tableRepository.GetById(id) == null)
            {
                return NotFound($"Table with ID {id} not found.");
            }
            
            _tableRepository.Delete(id);
            return NoContent();
        }
    }
}
