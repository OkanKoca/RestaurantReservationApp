using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Services.Abstract
{
    public interface IUserReservationService
    {
        List<AdminUserReservationDto> GetAllUserReservations();
        UserReservation? GetUserReservationById(int id);
        Task<(bool Success, string? ErrorMessage, UserReservation? Reservation)> CreateUserReservationAsync(UserReservationDto dto);
        void UpdateUserReservation(int id, UserReservationDto dto);
        bool ToggleReservationStatus(int id);
        Task<List<UserReservationDto>> GetUserReservationsAsync(int userId);
        Task<(bool Success, UserReservation? Reservation)> CancelUserReservationAsync(int reservationId, int userId);
        (bool Success, UserReservation? Reservation) DeleteUserReservation(int id);
    }
}
