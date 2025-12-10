using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Services.Abstract
{
    public interface IUserService
    {
        Task<List<AdminUserDto>> GetAllUsersAsync();
        AppUser? GetUserById(int id);
        Task<object?> GetUserProfileAsync(string userId);
        Task<(bool Success, IEnumerable<string>? Errors)> RegisterUserAsync(UserDto userDto);
        Task<(bool Success, string? Token, string? ErrorMessage)> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
        Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(int id);
        Task<bool> ChangeUserRoleAsync(int id);
    }
}
