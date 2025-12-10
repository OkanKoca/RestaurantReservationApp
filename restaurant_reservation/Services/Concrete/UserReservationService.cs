using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Services.Concrete
{
    public class UserReservationService : IUserReservationService
    {
        private readonly IUserReservationRepository _userReservationRepository;
        private readonly ITableRepository _tableRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserReservationService(
            IUserReservationRepository userReservationRepository,
            ITableRepository tableRepository,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _userReservationRepository = userReservationRepository;
            _tableRepository = tableRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public List<AdminUserReservationDto> GetAllUserReservations()
        {
            var userReservations = new List<AdminUserReservationDto>();

            foreach (var reservation in _userReservationRepository.UserReservations().ToList())
            {
                if (reservation.Customer == null)
                {
                    continue;
                }

                if (IsOutdated(reservation))
                {
                    reservation.Status = ReservationStatus.Outdated.ToString();
                    _userReservationRepository.Update(reservation);
                }

                var reservationDto = _mapper.Map<AdminUserReservationDto>(reservation);
                userReservations.Add(reservationDto);
            }

            return userReservations;
        }

        public UserReservation? GetUserReservationById(int id)
        {
            return _userReservationRepository.GetById(id);
        }

        public async Task<(bool Success, string? ErrorMessage, UserReservation? Reservation)> CreateUserReservationAsync(UserReservationDto dto)
        {
            var requestedDate = dto.ReservationDate.Date;
            var requestedHour = int.Parse(dto.ReservationHour!.Split(':')[0]);

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
                requestedDate.Year, requestedDate.Month, requestedDate.Day,
                requestedHour, 0, 0);

            var customer = await _userManager.FindByIdAsync(dto.CustomerId.ToString());

            var userReservation = new UserReservation
            {
                Customer = customer,
                NumberOfGuests = dto.NumberOfGuests,
                ReservationDate = requestedDateWithHour,
                TableId = table.Id,
                Table = table,
                CreatedAt = DateTime.UtcNow,
            };

            _userReservationRepository.Add(userReservation);

            return (true, null, userReservation);
        }

        public void UpdateUserReservation(int id, UserReservationDto dto)
        {
            var existingReservation = _userReservationRepository.GetById(id);
            if (existingReservation == null)
            {
                throw new KeyNotFoundException($"User reservation with ID {id} not found.");
            }

            existingReservation.NumberOfGuests = dto.NumberOfGuests;
            existingReservation.ReservationDate = dto.ReservationDate;
            existingReservation.ReservationDate = new DateTime(
                existingReservation.ReservationDate.Year,
                existingReservation.ReservationDate.Month,
                existingReservation.ReservationDate.Day,
                int.Parse(dto.ReservationHour!.Split(':')[0]), 0, 0);
            existingReservation.Status = dto.Status;

            existingReservation.Table!.UserReservations.Remove(existingReservation);
            existingReservation.Table.UserReservations.Add(existingReservation);

            _userReservationRepository.Update(existingReservation);
        }

        public bool ToggleReservationStatus(int id)
        {
            var existingReservation = _userReservationRepository.GetById(id);
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

            _userReservationRepository.Update(existingReservation);
            return true;
        }

        public async Task<List<UserReservationDto>> GetUserReservationsAsync(int userId)
        {
            var reservationsDb = await _userReservationRepository.UserReservations()
                .Where(r => r.Customer!.Id == userId)
                .Include(r => r.Table)
                .ToListAsync();

            return _mapper.Map<List<UserReservationDto>>(reservationsDb);
        }

        public async Task<(bool Success, UserReservation? Reservation)> CancelUserReservationAsync(int reservationId, int userId)
        {
            var reservationToCancel = await _userReservationRepository.UserReservations()
                .Where(r => r.Customer!.Id == userId)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservationToCancel == null)
            {
                return (false, null);
            }

            _userReservationRepository.Delete(reservationId);
            return (true, reservationToCancel);
        }

        public (bool Success, UserReservation? Reservation) DeleteUserReservation(int id)
        {
            var userReservation = _userReservationRepository.GetById(id);
            if (userReservation == null)
            {
                return (false, null);
            }

            _userReservationRepository.Delete(id);
            return (true, userReservation);
        }

        private bool IsOutdated(UserReservation reservation)
        {
            return reservation.ReservationDate < DateTime.UtcNow.ToLocalTime();
        }
    }
}
