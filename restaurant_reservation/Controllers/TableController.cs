using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Models;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using Microsoft.AspNetCore.Authorization;
using restaurant_reservation_api.Dto;
using restaurant_reservation_api.Models;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;

        public TableController(ITableRepository tableRepository, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public List<TableDto> GetAllTables()
        {
            var tables = _tableRepository.Tables().ToList();
            return _mapper.Map<List<TableDto>>(tables);
        }

        // GET api/<TableController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public Table GetTable(int id)
        {
            return _tableRepository.GetById(id);
        }

        [HttpGet("{id}/UserReservations")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<UserReservationDto>> GetTableUserReservations(int id, [FromQuery] DateTime? dateTime = null)
        {
            var table = _tableRepository.GetById(id);
            if (table == null) return NotFound();

            var query = table.UserReservations.AsQueryable();
            if (dateTime.HasValue)
            {
                var d = dateTime.Value.Date;
                query = query.Where(r => r.ReservationDate.Date == d);
            }

            var userReservations = _mapper.Map<List<UserReservationDto>>(query.ToList());

            if (userReservations == null)
            {
                return NotFound();
            }

            return userReservations;
        }

        [HttpGet("{id}/GuestReservations")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<AdminGuestReservationTableDto>> GetTableGuestReservations(int id, [FromQuery] DateTime? dateTime = null)
        {
            var table = _tableRepository.GetById(id);
            if (table == null) return NotFound();

            var query = table.GuestReservations.AsQueryable();
            if (dateTime.HasValue)
            {
                var d = dateTime.Value.Date;
                query = query.Where(g => g.ReservationDate.Date == d);
            }

            var guestReservations = _mapper.Map<List<AdminGuestReservationTableDto>>(query.ToList());

            if (guestReservations == null)
            {
                return NotFound();
            }

            return guestReservations;
        }

        [HttpGet("{id}/Occupancy")]

        [Authorize(Roles = "Admin")]
        public IActionResult GetTableOccupancy(int id, [FromQuery] DateTime dateTime)
        {
            var countOfWorkingHours = WorkingHours.Hours.Length;

            var userReservations = _tableRepository.GetById(id).UserReservations.Where(u => u.ReservationDate.Date == dateTime.Date).ToList();
            var guestReservations = _tableRepository.GetById(id).GuestReservations.Where(g => g.ReservationDate.Date == dateTime.Date).ToList();

            int countOfReservationsAtDate = userReservations.Count + guestReservations.Count;

            var rate = ((double)countOfReservationsAtDate / countOfWorkingHours) * 100;

            return new JsonResult(rate);
        }


        // POST api/<TableController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTable(TableDto tableDto)
        {
            if (tableDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var table = _mapper.Map<Table>(tableDto);

            _tableRepository.Add(table);
            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, table);
        }

        // PUT api/<TableController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateTable(int id, TableDto tableDto)
        {
            if (tableDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var table = _tableRepository.GetById(id);

            _mapper.Map(tableDto, table);

            _tableRepository.Update(table);

            return NoContent();
        }

        // DELETE api/<TableController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
