using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;
using restaurant_reservation_api.Hubs;
using System.Security.Claims;

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserReservationController : ControllerBase
    {
        private readonly IUserReservationService _userReservationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<AdminHub> _hubContext;

        public UserReservationController(
            IUserReservationService userReservationService,
            UserManager<AppUser> userManager,
            IHubContext<AdminHub> hubContext)
        {
            _userReservationService = userReservationService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<AdminUserReservationDto>> GetAllUserReservations()
        {
            var reservations = _userReservationService.GetAllUserReservations();
            return Ok(reservations);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public ActionResult<UserReservation> GetUserReservation(int id)
        {
            var reservation = _userReservationService.GetUserReservationById(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

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

            var result = await _userReservationService.CreateUserReservationAsync(userReservationDto);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            var reservation = result.Reservation!;
            await _hubContext.Clients.All.SendAsync("NewReservation", new
            {
                ReservationTable = reservation.Table!.Number,
                CustomerName = $"{reservation.Customer!.FirstName} {reservation.Customer.LastName}",
                ReservationDate = reservation.ReservationDate.ToShortDateString(),
                ReservationHour = reservation.ReservationDate.ToShortTimeString()
            });

            return CreatedAtAction(nameof(GetUserReservation), new { id = reservation.Id }, userReservationDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateReservation(int id, UserReservationDto userReservationDto)
        {
            if (userReservationDto == null)
            {
                return BadRequest("Invalid reservation data.");
            }

            try
            {
                _userReservationService.UpdateUserReservation(id, userReservationDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateReservationStatus(int id)
        {
            if (!_userReservationService.ToggleReservationStatus(id))
            {
                return NotFound($"User reservation with ID {id} not found.");
            }
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

            var reservations = await _userReservationService.GetUserReservationsAsync(int.Parse(userId));

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

            var result = await _userReservationService.CancelUserReservationAsync(id, int.Parse(userId));

            if (!result.Success)
            {
                return NotFound();
            }

            var reservation = result.Reservation!;
            await _hubContext.Clients.All.SendAsync("ReservationCanceled", new
            {
                ReservationTable = reservation.Table!.Number,
                CustomerName = $"{reservation.Customer!.FirstName} {reservation.Customer.LastName}",
                ReservationDate = reservation.ReservationDate.ToShortDateString(),
                ReservationHour = reservation.ReservationDate.ToShortTimeString()
            });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = _userReservationService.DeleteUserReservation(id);

            if (!result.Success)
            {
                return NotFound($"User reservation with ID {id} not found.");
            }

            var reservation = result.Reservation!;
            await _hubContext.Clients.All.SendAsync("ReservationCanceled", new
            {
                ReservationTable = reservation.Table!.Number,
                CustomerName = $"{reservation.Customer!.FirstName} {reservation.Customer.LastName}",
                ReservationDate = reservation.ReservationDate.ToShortDateString(),
                ReservationHour = reservation.ReservationDate.ToShortTimeString()
            });

            return NoContent();
        }
    }
}
