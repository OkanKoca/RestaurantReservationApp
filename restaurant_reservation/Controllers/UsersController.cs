using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace restaurant_reservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RestaurantContext _context;
        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RestaurantContext restaurantContext) {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = restaurantContext;
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
                return Ok(); // jwt token generation can be added here
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
        public void Delete(int id)
        {
            _context.Users.Remove(_context.Users.FirstOrDefault(u => u.Id == id) ?? throw new KeyNotFoundException("User not found."));
        }
    }
}
