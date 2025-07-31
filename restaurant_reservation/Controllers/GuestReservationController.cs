using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestReservationController : ControllerBase
    {
        private readonly IGuestReservationRepository _guestReservationRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IUserReservationRepository _userReservationRepository;
        public GuestReservationController(IGuestReservationRepository guestReservationRepository, ITableRepository tableRepository, IUserReservationRepository userReservationRepository)
        {
            _guestReservationRepository = guestReservationRepository;
            _tableRepository = tableRepository;
            _userReservationRepository = userReservationRepository;
        }

        // GET: api/<GuestReservationController>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public List<GuestReservation> GetAllGuestReservations()
        {
            // made a status check here to see if a reservation is outdated

            var reservations = _guestReservationRepository.GuestReservations().ToList();

            foreach (var reservation in reservations)
            {
                if (isOutdated(reservation.Id) && !string.Equals(reservation.Status.ToString(), ReservationStatus.Outdated.ToString()))
                {
                    reservation.Status = ReservationStatus.Outdated.ToString();
                    _guestReservationRepository.Update(reservation);
                }
            }

            return _guestReservationRepository.GuestReservations().ToList();
        }
        private bool isOutdated(int id)
        {
            var reservation = _guestReservationRepository.GetById(id);

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

        // GET api/<GuestReservationController>/5
        [Authorize(Roles = "Admin")]   
        [HttpGet("{id}")]
        public GuestReservation GetGuestReservation(int id)
        {
            return _guestReservationRepository.GetById(id);
        }

        // POST api/<GuestReservationController>
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

            var requestedDate = guestReservationDto.ReservationDate.Date;
            var requestedHour = int.Parse(guestReservationDto.ReservationHour.Split(':')[0]);


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

            var guestReservation = new GuestReservation
            {
                FullName = guestReservationDto.FullName,
                Email = guestReservationDto.Email,
                PhoneNumber = guestReservationDto.PhoneNumber,
                NumberOfGuests = guestReservationDto.NumberOfGuests,
                ReservationDate = requestedDateWithHour,
                TableId = table.Id,
                Table = table,
                CreatedAt = DateTime.UtcNow
            };

            _guestReservationRepository.Add(guestReservation);

            return CreatedAtAction(nameof(GetGuestReservation), new { id = guestReservation.Id }, guestReservationDto);
        }

        // PUT api/<GuestReservationController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateGuestReservation(int id, GuestReservation guestReservation)
        {
            if (guestReservation == null || guestReservation.Id != id)
            {
                return BadRequest("Invalid guest reservation data.");
            }
            var existingReservation = _guestReservationRepository.GetById(id);
            if (existingReservation == null)
            {
                return NotFound($"Guest reservation with ID {id} not found.");
            }

            existingReservation.FullName = guestReservation.FullName;
            existingReservation.Email = guestReservation.Email;
            existingReservation.PhoneNumber = guestReservation.PhoneNumber;
            existingReservation.NumberOfGuests = guestReservation.NumberOfGuests;
            existingReservation.ReservationDate = guestReservation.ReservationDate;
            existingReservation.Table = _tableRepository.GetById(guestReservation.TableId);
            existingReservation.Status = ReservationStatus.Confirmed.ToString();

            existingReservation.Table.GuestReservations.Remove(existingReservation);    

            existingReservation.Table.GuestReservations.Add(existingReservation);

            _guestReservationRepository.Update(existingReservation);

            return NoContent();
        }

        // DELETE api/<GuestReservationController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var guestReservation = _guestReservationRepository.GetById(id);

            if (guestReservation == null)
            {
                return NotFound($"Guest reservation with ID {id} not found.");
            }
            _guestReservationRepository.Delete(id);

            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateReservationStatus(int id)
        {
            var existingReservation = _guestReservationRepository.GetById(id);

            if (existingReservation == null)
            {
                return NotFound($"Guest reservation with ID {id} not found.");
            }

            if (existingReservation.Status == ReservationStatus.Confirmed.ToString())
            {
                existingReservation.Status = ReservationStatus.Pending.ToString();
            }
            else if (existingReservation.Status == ReservationStatus.Pending.ToString())
            {
                existingReservation.Status = ReservationStatus.Confirmed.ToString();
            }

            _guestReservationRepository.Update(existingReservation);

            return NoContent();
        }

        [HttpGet("available-hours")]
        public async Task<ActionResult<List<string>>> GetAvailableHours(DateTime date)
        {
            var existingReservations = await _guestReservationRepository.GuestReservations()
                .Where(r => r.ReservationDate.Date == date.Date).ToListAsync();

            var userReservations = await _userReservationRepository.UserReservations()
                .Where(r => r.ReservationDate.Date == date.Date).ToListAsync();

            foreach (var reservation in userReservations)
            {
                if (reservation.Customer == null)
                {
                    existingReservations.Add(
                        new GuestReservation
                        {
                            FullName = "Made by Unknown User",
                            Email = "Made by Unknown User",
                            PhoneNumber = "Made by Unknown User",
                            ReservationDate = reservation.ReservationDate,
                            TableId = reservation.TableId
                        });
                }

                else
                {
                    existingReservations.Add(
                        new GuestReservation
                        {
                            FullName = reservation.Customer.FullName ?? "",
                            Email = reservation.Customer.Email ?? "",
                            PhoneNumber = reservation.Customer.PhoneNumber ?? "",
                            ReservationDate = reservation.ReservationDate,
                            TableId = reservation.TableId
                        });
                }
            }

            var allTables = await _tableRepository.Tables()
                .Include(t => t.GuestReservations)
                .Include(t=> t.UserReservations)
                .ToListAsync();

            var workingHours = new[]
            {
                "12:00", "13:00", "14:00", "15:00", "16:00", 
                "17:00", "18:00", "19:00", "20:00", "21:00"
            };

            var availableHours = workingHours.Where(hour =>
            {
                var timeSlot = DateTime.Parse(hour);
                var reservationsAtTime = existingReservations
                    .Count(r => r.ReservationDate.Hour == timeSlot.Hour);

                return reservationsAtTime < allTables.Count;
            }).ToList();

            return availableHours;
        }
    }
}
