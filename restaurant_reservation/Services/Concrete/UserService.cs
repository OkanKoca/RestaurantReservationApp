using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Data;
using restaurant_reservation_api.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace restaurant_reservation.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RestaurantContext _context;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RestaurantContext context,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            return await _context.Users
            .Join(
            _context.UserRoles,
            user => user.Id,
            userRole => userRole.UserId,
            (user, userRole) => new { user, userRole }
            )
            .Join(
            _context.Roles,
                ur => ur.userRole.RoleId,
                role => role.Id,
                (ur, role) => new AdminUserDto
                {
                     Id = ur.user.Id,
                     FirstName = ur.user.FirstName,
                     LastName = ur.user.LastName,
                     Email = ur.user.Email!,
                     Phone = ur.user.PhoneNumber!,
                     Role = role.Name ?? "Customer"
                }
            )
            .ToListAsync();
        }

        public AppUser? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<object?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber
            };
        }

        public async Task<(bool Success, IEnumerable<string>? Errors)> RegisterUserAsync(UserDto userDto)
        {
            var user = new AppUser
            {
                UserName = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.Phone,
                Email = userDto.Email
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (result.Succeeded && roleResult.Succeeded)
            {
                return (true, null);
            }

            return (false, result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Success, string? Token, string? ErrorMessage)> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return (false, null, "Invalid email or password.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if (result.Succeeded)
            {
                var token = GenerateJwt(user);
                return (true, token, null);
            }

            return (false, null, "Invalid email or password.");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                       .Include(u => u.Reservations)
               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (user.Reservations != null && user.Reservations.Any())
                {
                    _context.Reservations.RemoveRange(user.Reservations);
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                }

                await _userManager.DeleteAsync(user);
                await transaction.CommitAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }

        public async Task<bool> ChangeUserRoleAsync(int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return false;
            }

            var isCustomer = await _userManager.IsInRoleAsync(user, "Customer");

            if (isCustomer)
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            return true;
        }

        private string GenerateJwt(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(
               _configuration.GetSection("AppSettings:SecretKey").Value ?? "");
            var userRoles = _userManager.GetRolesAsync(user);
            var userRole = userRoles.Result.FirstOrDefault() ?? "Customer";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Role, userRole),
                    new Claim("role", userRole)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
