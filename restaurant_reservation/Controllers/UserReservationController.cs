using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Data.Concrete;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;
using System.Collections.Generic;
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
        public List<AdminUserReservationDto> GetAllUserReservations()
        {
            List <AdminUserReservationDto> userReservations = new List<AdminUserReservationDto>();

            foreach (var reservation in _userReservationRepository.UserReservations().ToList())
            {
                if(reservation.Customer == null)
                {
                    continue;
                }

                if(isOutdated(reservation.Id))
                {
                    reservation.Status = ReservationStatus.Outdated.ToString();
                    _userReservationRepository.Update(reservation);
                }

                userReservations.Add(new AdminUserReservationDto
                {
                    Id = reservation.Id,
                    CustomerId = reservation.Customer.Id,
                    CustomerName = $"{reservation.Customer.FirstName} {reservation.Customer.LastName}",
                    CustomerEmail = reservation.Customer.Email,
                    CustomerPhone = reservation.Customer.PhoneNumber,
                    NumberOfGuests = reservation.NumberOfGuests,
                    ReservationDate = reservation.ReservationDate,
                    ReservationHour = reservation.ReservationDate.Hour.ToString(),
                    Status = reservation.Status.ToString(),
                    CreatedAt = reservation.CreatedAt
                });
            }
            
            return userReservations;
        }
        private bool isOutdated(int id)
        {
            var reservation = _userReservationRepository.GetById(id);

            if (reservation == null)
            {
                return false;
            }

            if (reservation.ReservationDate < DateTime.UtcNow.ToLocalTime())
            {
                return true;
            }

            return false;
        }

        // GET api/<UserReservationController>/5
        [Authorize(Roles = "Admin")]   
        [HttpGet("{id}")]
        public UserReservation GetUserReservation(int id)
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

            var userReservation = new UserReservation
            {
                Customer = customer,
                NumberOfGuests = userReservationDto.NumberOfGuests,
                ReservationDate = requestedDateWithHour,
                TableId = table.Id,
                Table = table,
                CreatedAt = DateTime.UtcNow,
            };

            _userReservationRepository.Add(userReservation);

            return CreatedAtAction(nameof(GetUserReservation), new { id = userReservation.Id }, userReservationDto);
        }

        // PUT api/<UserReservationController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateReservation(int id, UserReservationDto userReservationDto)
        {
            if (userReservationDto == null)
            {
                return BadRequest("Invalid reservation data.");
            }

            var existingReservation = _userReservationRepository.GetById(id);

            if (existingReservation == null)
            {
                return NotFound($"User reservation with ID {id} not found.");
            }

            existingReservation.NumberOfGuests = userReservationDto.NumberOfGuests;
            existingReservation.ReservationDate = userReservationDto.ReservationDate;
            existingReservation.ReservationDate = new DateTime(existingReservation.ReservationDate.Year, existingReservation.ReservationDate.Month, existingReservation.ReservationDate.Day, int.Parse(userReservationDto.ReservationHour.Split(':')[0]), 0, 0);
            //existingReservation.TableId = userReservation.TableId;
            //existingReservation.Table = _tableRepository.GetById(userReservation.TableId);
            existingReservation.Status = userReservationDto.Status;

            existingReservation.Table.UserReservations.Remove(existingReservation);

            existingReservation.Table.UserReservations.Add(existingReservation);

            _userReservationRepository.Update(existingReservation);

            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateReservationStatus(int id)
        {

            var existingReservation = _userReservationRepository.GetById(id);

            if (existingReservation == null)
            {
                return NotFound($"User reservation with ID {id} not found.");
            }

            if(existingReservation.Status == ReservationStatus.Confirmed.ToString())
            {
                existingReservation.Status = ReservationStatus.Pending.ToString();
            } 
            else if(existingReservation.Status == ReservationStatus.Pending.ToString())
            {
                existingReservation.Status = ReservationStatus.Confirmed.ToString();
            }

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
                    Id = reservation.Id,
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

        [Authorize]
        [HttpDelete("myreservations/{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var reservationToCancel = _userReservationRepository.UserReservations()
            .Where(r => r.Customer.Id == int.Parse(userId))
            .Include(r => r.Table)
            .FirstOrDefault(r => r.Id == id);

            if (reservationToCancel == null)
            {
                return NotFound();
            }

            _userReservationRepository.Delete(id);

            return NoContent();
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
