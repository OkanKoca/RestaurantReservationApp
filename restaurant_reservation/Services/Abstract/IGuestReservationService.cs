using restaurant_reservation.Dto;
using restaurant_reservation.Models;

namespace restaurant_reservation.Services.Abstract
{
    public interface IGuestReservationService
    {
        List<GuestReservation> GetAllGuestReservations();
        GuestReservation? GetGuestReservationById(int id);
        Task<(bool Success, string? ErrorMessage, GuestReservation? Reservation)> CreateGuestReservationAsync(GuestReservationDto dto);
        void UpdateGuestReservation(int id, GuestReservation guestReservation);
        (bool Success, GuestReservation? Reservation) DeleteGuestReservation(int id);
        Task<bool> ToggleReservationStatusAsync(int id);
        Task<List<string>> GetAvailableHoursAsync(DateTime date);
    }
}
