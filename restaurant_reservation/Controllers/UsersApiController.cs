using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;
using System.Security.Claims;

namespace restaurant_reservation.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersApiController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<UsersController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET api/<UsersController>/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public ActionResult<AppUser> Get(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var profile = await _userService.GetUserProfileAsync(userId);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        // POST api/<UsersController>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUserAsync(userDto);

            if (result.Success)
            {
                return StatusCode(StatusCodes.Status201Created);
            }

            return BadRequest(new { Errors = result.Errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);

            if (result.Success)
            {
                return Ok(new { token = result.Token });
            }

            return Unauthorized(new { Message = result.ErrorMessage });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return Ok(new { message = "Logged out successfully." });
        }

        // DELETE api/<UsersController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.Success)
            {
                if (result.ErrorMessage == "User not found.")
                {
                    return NotFound(new { Message = result.ErrorMessage });
                }
                return StatusCode(500, new { Message = "An error occurred while deleting the user.", Error = result.ErrorMessage });
            }

            return Ok(new { Message = "User and related data deleted successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("ChangeUserRole/{id}")]
        public async Task<IActionResult> AuthorizeUser(int id)
        {
            if (!await _userService.ChangeUserRoleAsync(id))
            {
                return NotFound();
            }

            return Ok(new { Message = "User role changed successfully." });
        }
    }
}
