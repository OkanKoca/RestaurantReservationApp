using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserReservationController : ControllerBase
    {
        private readonly IUserReservationRepository _userReservationRepository;
        private readonly ITableRepository _tableRepository;
        private readonly UserManager<AppUser> _userManager;
        public UserReservationController(IUserReservationRepository userReservationRepository, ITableRepository tableRepository, UserManager<AppUser> userManager)
        {
            _userReservationRepository = userReservationRepository;
            _tableRepository = tableRepository;
            _userManager = userManager;
        }
        // GET: api/<UserReservationController>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public List<Reservation> GetAllUserReservations()
        {
            return _userReservationRepository.UserReservations().ToList();
        }

        // GET api/<UserReservationController>/5
        [Authorize(Roles = "Admin")]   
        [HttpGet("{id}")]
        public Reservation GetUserReservation(int id)
        {
            return _userReservationRepository.GetById(id);
        }



        // POST api/<UserReservationController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(UserReservationDto userReservationDto)
        {
            if (userReservationDto == null)
            {
                return BadRequest("User reservation data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requestedDate = userReservationDto.ReservationDate.Date;
            var requestedHour = int.Parse(userReservationDto.ReservationHour.Split(':')[0]);


            var available_tables = await _tableRepository.Tables()
                .Where(table => !table.GuestReservations.Any(r =>
                    r.ReservationDate.Date == requestedDate &&
                    r.ReservationDate.Hour == requestedHour) &&
                    !table.UserReservations.Any(r =>
                    r.ReservationDate.Date == requestedDate &&
                    r.ReservationDate.Hour == requestedHour))
                .ToListAsync();


            if (!available_tables.Any())
            {
                return BadRequest("No available tables for the requested date and time.");
            }

            var table = available_tables.First();

            DateTime requestedDateWithHour = new DateTime(requestedDate.Year, requestedDate.Month, requestedDate.Day, requestedHour, 0, 0);
            
            var customer = await _userManager.FindByIdAsync(userReservationDto.CustomerId.ToString());

            var userReservation = new Reservation
            {
                Customer = customer,
                NumberOfGuests = userReservationDto.NumberOfGuests,
                ReservationDate = requestedDateWithHour,
                TableId = table.Id,
                Table = table
            };

            _userReservationRepository.Add(userReservation);

            return CreatedAtAction(nameof(GetUserReservation), new { id = userReservation.Id }, userReservationDto);
        }

        // PUT api/<UserReservationController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult ConfirmUserReservation(int id, Reservation userReservation)
        {
            if (userReservation == null || userReservation.Id != id)
            {
                return BadRequest("Invalid reservation data or ID mismatch.");
            }
            var existingReservation = _userReservationRepository.GetById(id);

            if (existingReservation == null)
            {
                return NotFound($"Guest reservation with ID {id} not found.");
            }

            existingReservation.Customer = userReservation.Customer;    
            existingReservation.NumberOfGuests = userReservation.NumberOfGuests;
            existingReservation.ReservationDate = userReservation.ReservationDate;
            existingReservation.TableId = userReservation.TableId;
            existingReservation.Table = _tableRepository.GetById(userReservation.TableId);
            existingReservation.Status = ReservationStatus.Confirmed.ToString();

            existingReservation.Table.UserReservations.Add(existingReservation);

            _userReservationRepository.Update(existingReservation);

            return NoContent();
        }

        [Authorize]
        [HttpGet("myreservations")]
        public async Task<IActionResult> GetUserReservations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);


            if (user == null)
                return NotFound();

            var reservationsDb = await _userReservationRepository.UserReservations()
                .Where(r => r.Customer.Id == int.Parse(userId))
                .Include(r => r.Table)
                .ToListAsync();

            List<UserReservationDto> reservations = new List<UserReservationDto>();

            foreach(var reservation in reservationsDb)
            {
                var reservationDto = new UserReservationDto
                {
                    CustomerId = reservation.Customer.Id,
                    NumberOfGuests = reservation.NumberOfGuests,
                    ReservationDate = reservation.ReservationDate,
                    ReservationHour = reservation.ReservationDate.Hour.ToString(),
                    Status = reservation.Status.ToString(),
                    CreatedAt = reservation.CreatedAt
                };

                reservations.Add(reservationDto);
            }
            

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                reservations
            });
        }

        // DELETE api/<UserReservationController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var UserReservation = _userReservationRepository.GetById(id);

            if (UserReservation == null)
            {
                return NotFound($"Guest reservation with ID {id} not found.");
            }

            _userReservationRepository.Delete(id);

            return NoContent();
        }
    }
}
