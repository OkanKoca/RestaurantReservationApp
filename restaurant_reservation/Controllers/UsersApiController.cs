using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
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
        public List<AppUser> GetAllUsers()
        {
            return _context.Users.ToList(); // This will load all users from the database
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

        // PUT api/<UsersController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if(user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            _context.Users.Remove(user);
            return Ok();
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
