using Microsoft.AspNetCore.Mvc;
using restaurant_reservation.Models;
using restaurant_reservation_system.Models.Dto;
using restaurant_reservation_system.Models.ViewModel;
using System.Net.Http.Headers;

namespace restaurant_reservation_system.Controllers
{
    public class UserReservationController : Controller
    {
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
                        CustomerId = customerDetails.Id,
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
                CustomerId = model.CustomerId,
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

        public async Task<IActionResult> MyReservations()
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

                var response = await client.GetAsync("UserReservation/MyReservations");

                if (response.IsSuccessStatusCode)
                {
                    var customerDetails = await response.Content.ReadFromJsonAsync<MyReservationsDto>();
                    var reservations = new List<MyReservationsViewModel>();
                    
                    foreach(var reservation in customerDetails.Reservations)
                    {
                        reservations.Add(new MyReservationsViewModel
                        {
                            Id = reservation.CustomerId,
                            Status = reservation.Status,
                            ReservationDate = reservation.ReservationDate,
                            ReservationHour = reservation.ReservationDate.Hour.ToString(),
                            NumberOfGuests = reservation.NumberOfGuests,
                            CreatedAt = reservation.CreatedAt
                        }); 
                    }

                    return View(reservations);
                }

                TempData["ErrorMessage"] = "Reservation informations couldn't be loaded.";
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
