using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Models;

namespace restaurant_reservation.Services.Concrete
{
    public class GuestReservationService : IGuestReservationService
    {
        private readonly IGuestReservationRepository _guestReservationRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IUserReservationRepository _userReservationRepository;

        public GuestReservationService(IGuestReservationRepository guestReservationRepository,ITableRepository tableRepository,IUserReservationRepository userReservationRepository)
        {
            _guestReservationRepository = guestReservationRepository;
            _tableRepository = tableRepository;
            _userReservationRepository = userReservationRepository;
        }

        public List<GuestReservation> GetAllGuestReservations()
        {
            var reservations = _guestReservationRepository.GuestReservations().ToList();

            foreach (var reservation in reservations)
            {
                if (IsOutdated(reservation) && !string.Equals(reservation.Status, ReservationStatus.Outdated.ToString()))
                {
                    reservation.Status = ReservationStatus.Outdated.ToString();
                    _guestReservationRepository.Update(reservation);
                }
            }

            return _guestReservationRepository.GuestReservations().ToList();
        }

        public GuestReservation? GetGuestReservationById(int id)
        {
            return _guestReservationRepository.GetById(id);
        }

        public async Task<(bool Success, string? ErrorMessage, GuestReservation? Reservation)> CreateGuestReservationAsync(GuestReservationDto dto)
        {
            var requestedDate = dto.ReservationDate.Date;
            var requestedHour = int.Parse(dto.ReservationHour.Split(':')[0]);

            var availableTables = _tableRepository.Tables()
                .Where(table => !table.GuestReservations.Any(r =>
                r.ReservationDate.Date == requestedDate &&
                r.ReservationDate.Hour == requestedHour) &&
                !table.UserReservations.Any(r =>
                    r.ReservationDate.Date == requestedDate &&
                    r.ReservationDate.Hour == requestedHour))
                .ToList();

            if (!availableTables.Any())
            {
                return (false, "No available tables for the requested date and time.", null);
            }

            var table = availableTables.First();
            var requestedDateWithHour = new DateTime(
            requestedDate.Year, requestedDate.Month, requestedDate.Day,requestedHour, 0, 0);

            var guestReservation = new GuestReservation
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                NumberOfGuests = dto.NumberOfGuests,
                ReservationDate = requestedDateWithHour,
                TableId = table.Id,
                Table = table,
                CreatedAt = DateTime.UtcNow
            };

            _guestReservationRepository.Add(guestReservation);

            return (true, null, guestReservation);
        }

        public void UpdateGuestReservation(int id, GuestReservation guestReservation)
        {
            var existingReservation = _guestReservationRepository.GetById(id);
            if (existingReservation == null)
            {
                throw new KeyNotFoundException($"Guest reservation with ID {id} not found.");
            }

            existingReservation.FullName = guestReservation.FullName;
            existingReservation.Email = guestReservation.Email;
            existingReservation.PhoneNumber = guestReservation.PhoneNumber;
            existingReservation.NumberOfGuests = guestReservation.NumberOfGuests;
            existingReservation.ReservationDate = guestReservation.ReservationDate;
            existingReservation.Table = _tableRepository.GetById(guestReservation.TableId);
            existingReservation.Status = ReservationStatus.Confirmed.ToString();

            existingReservation.Table?.GuestReservations.Remove(existingReservation);
            existingReservation.Table?.GuestReservations.Add(existingReservation);

            _guestReservationRepository.Update(existingReservation);
        }

        public (bool Success, GuestReservation? Reservation) DeleteGuestReservation(int id)
        {
            var guestReservation = _guestReservationRepository.GetById(id);
            if (guestReservation == null)
            {
                return (false, null);
            }

            _guestReservationRepository.Delete(id);
            return (true, guestReservation);
        }

        public bool ToggleReservationStatus(int id)
        {
            var existingReservation = _guestReservationRepository.GetById(id);
            if (existingReservation == null)
            {
                return false;
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
            return true;
        }

        public async Task<List<string>> GetAvailableHoursAsync(DateTime date)
        {
            var existingReservations = await _guestReservationRepository.GuestReservations()
                .Where(r => r.ReservationDate.Date == date.Date)
                .ToListAsync();

            var userReservations = await _userReservationRepository.UserReservations()
            .Where(r => r.ReservationDate.Date == date.Date)
                    .ToListAsync();

            foreach (var reservation in userReservations)
            {
                var guestReservation = new GuestReservation
                {
                    FullName = reservation.Customer?.FullName ?? "Made by Unknown User",
                    Email = reservation.Customer?.Email ?? "Made by Unknown User",
                    PhoneNumber = reservation.Customer?.PhoneNumber ?? "Made by Unknown User",
                    ReservationDate = reservation.ReservationDate,
                    TableId = reservation.TableId
                };
                existingReservations.Add(guestReservation);
            }

            var allTables = await _tableRepository.Tables()
                .Include(t => t.GuestReservations)
                .Include(t => t.UserReservations)
                .ToListAsync();

            var availableHours = WorkingHours.Hours.Where(hour =>
            {
                var timeSlot = DateTime.Parse(hour);

                var reservationsAtTime = existingReservations
                .Count(r => r.ReservationDate.Hour == timeSlot.Hour);

                return reservationsAtTime < allTables.Count;
            }).ToList();

            return availableHours;
        }

        private bool IsOutdated(GuestReservation reservation)
        {
            return reservation.ReservationDate < DateTime.UtcNow.ToLocalTime();
        }
    }
}
