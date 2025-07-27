using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Models;
using restaurant_reservation_system.Models;
using System.Net.Http.Headers;

namespace restaurant_reservation_system.Controllers
{
    public class UserReservationController : Controller
    {

        //private async Task<CustomerInfo> GetCustomerInfo()
        //{
        //    var token = HttpContext.Session.GetString("token");
        //    if (string.IsNullOrEmpty(token))
        //        return null;

        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        //    if (jsonToken == null) return null;

        //    return new CustomerInfo
        //    {
        //        Id = int.Parse(jsonToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value),
        //        Email = jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value,
        //        Role = jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value
        //    };
        //}

        //private async Task<CustomerInfo?> GetCustomerInfo()
        //{
        //    var token = HttpContext.Session.GetString("token");
        //    if (string.IsNullOrEmpty(token))
        //        return null;

        //    try
        //    {
        //        var handler = new JwtSecurityTokenHandler();
        //        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        //        if (jsonToken == null) return null;

        //        var idClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
        //        var emailClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
        //        var roleClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

        //        if (idClaim == null || emailClaim == null || roleClaim == null)
        //            return null;

        //        return new CustomerInfo
        //        {
        //            Id = int.Parse(idClaim.Value),
        //            Email = emailClaim.Value,
        //            Role = roleClaim.Value
        //        };
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        public async Task<IActionResult> Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("token")))
            {
                return RedirectToAction("Login", "Users");
            }

            using (var client = new HttpClient()) 
            {
                client.BaseAddress = new Uri("https://localhost:7284/api/");
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                var response = await client.GetAsync("users/profile");

                if (response.IsSuccessStatusCode)
                {
                    var customerDetails = await response.Content.ReadFromJsonAsync<AppUser>();
                    var viewModel = new UserReservationViewModel
                    {
                        Id = customerDetails.Id,
                        CustomerName = $"{customerDetails.FirstName} {customerDetails.LastName}",
                        CustomerEmail = customerDetails.Email,
                        CustomerPhone = customerDetails.PhoneNumber
                    };
                    return View(viewModel);
                }
                
                TempData["ErrorMessage"] = "User information couldn't be loaded.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BookReservation(UserReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all the fields.";
                return View("Create", model);
            }

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("token")))
            {
                return RedirectToAction("Login", "Users");
            }

            var reservationDto = new UserReservationDto
            {
                Id = model.Id,
                ReservationDate = model.ReservationDate,
                ReservationHour = model.ReservationHour,
                NumberOfGuests = model.NumberOfGuests
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7284/api/");
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                var response = await client.PostAsJsonAsync("userreservation", reservationDto);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Your reservation is successfully done!";
                    return RedirectToAction("Index", "Home");
                }
                
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Reservation couldn't be booked: {error}";
                return View("Create", model);
            }
        }

    }
    public class CustomerInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
