using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Data;
using restaurant_reservation_api.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RestaurantContext _context;
        private readonly IConfiguration _configuration;
        public UsersApiController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RestaurantContext restaurantContext, IConfiguration configuration) {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = restaurantContext;
            _configuration = configuration;
        }
        // GET: api/<UsersController>
        
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
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
                    Email = ur.user.Email,
                    Phone = ur.user.PhoneNumber,
                    Role = role.Name ?? "Customer"
                }
            )
            .ToListAsync();

            return Ok(users);
        }

        // GET api/<UsersController>/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public AppUser Get(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id) ?? throw new KeyNotFoundException("User not found.");
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber
            });
        }

        // POST api/<UsersController>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                return StatusCode(StatusCodes.Status201Created);
            }

            return BadRequest(new
            {
                Errors = result.Errors.Select(e => e.Description)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(user == null)
            {
                return BadRequest(new { Message = "Invalid email or password." });
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if(result.Succeeded)
            {
                return Ok(new {token = GenerateJWT(user)}); 
            }

            return Unauthorized(new { Message = "Invalid email or password." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully." });
        }


        // DELETE api/<UsersController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.Reservations) 
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
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
                return Ok(new { Message = "User and related data deleted successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "An error occurred while deleting the user.", Error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("ChangeUserRole/{id}")]
        public async Task<IActionResult> AuthorizeUser(int id)
        {
            var user = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            var isUserRoleCustomer = await _userManager.IsInRoleAsync(user, "Customer");

            if (isUserRoleCustomer)
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            return Ok(new { Message = "User role changed successfully." });
        }

        private object GenerateJWT(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value ?? "");
            var userRoles = _userManager.GetRolesAsync(user);
            var userRole = userRoles.Result.FirstOrDefault() ?? "Customer"; // Default to Customer if no roles assigned

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName ?? ""),
                        new Claim(ClaimTypes.Role, userRole.ToString() ?? ""),
                        new Claim("role", userRole)
                    }
                ),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
