using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Hubs;

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestReservationController : ControllerBase
    {
        private readonly IGuestReservationService _guestReservationService;
        private readonly ITableRepository _tableRepository;
        private readonly IHubContext<AdminHub> _hubContext;

        public GuestReservationController(
          IGuestReservationService guestReservationService,
               ITableRepository tableRepository,
              IHubContext<AdminHub> hubContext)
        {
            _guestReservationService = guestReservationService;
            _tableRepository = tableRepository;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<GuestReservation>> GetAllGuestReservations()
        {
            var reservations = _guestReservationService.GetAllGuestReservations();
            return Ok(reservations);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public ActionResult<GuestReservation> GetGuestReservation(int id)
        {
            var reservation = _guestReservationService.GetGuestReservationById(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> Post(GuestReservationDto guestReservationDto)
        {
            if (guestReservationDto == null)
            {
                return BadRequest("Guest reservation data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _guestReservationService.CreateGuestReservationAsync(guestReservationDto);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            var reservation = result.Reservation!;
            await _hubContext.Clients.All.SendAsync("NewReservation", new
            {
                ReservationTable = reservation.Table!.Number,
                CustomerName = reservation.FullName,
                ReservationDate = reservation.ReservationDate.ToShortDateString(),
                ReservationHour = reservation.ReservationDate.ToShortTimeString()
            });

            return CreatedAtAction(nameof(GetGuestReservation), new { id = reservation.Id }, guestReservationDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateGuestReservation(int id, GuestReservation guestReservation)
        {
            if (guestReservation == null || guestReservation.Id != id)
            {
                return BadRequest("Invalid guest reservation data.");
            }

            try
            {
                _guestReservationService.UpdateGuestReservation(id, guestReservation);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = _guestReservationService.DeleteGuestReservation(id);

            if (!result.Success)
            {
                return NotFound($"Guest reservation with ID {id} not found.");
            }

            var reservation = result.Reservation!;
            await _hubContext.Clients.All.SendAsync("ReservationCanceled", new
            {
                ReservationTable = _tableRepository.GetById(reservation.TableId)?.Number ?? 0,
                CustomerName = reservation.FullName,
                ReservationDate = reservation.ReservationDate.ToShortDateString(),
                ReservationHour = reservation.ReservationDate.ToShortTimeString()
            });

            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateReservationStatus(int id)
        {
            if (!await _guestReservationService.ToggleReservationStatusAsync(id))
            {
                return NotFound($"Guest reservation with ID {id} not found.");
            }
            return NoContent();
        }

        [HttpGet("available-hours")]
        public async Task<ActionResult<List<string>>> GetAvailableHours(DateTime date)
        {
            var availableHours = await _guestReservationService.GetAvailableHoursAsync(date);
            return Ok(availableHours);
        }
    }
}
