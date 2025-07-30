using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using restaurant_reservation_system.Models.Dto;
using restaurant_reservation_system.Models.ViewModel;
using System.Diagnostics;

namespace restaurant_reservation_system.Controllers
{
    public class AdminController : Controller
    {

        private readonly HttpClient client;

        public AdminController(IConfiguration config)
        {
            var baseUrl = config["ApiSettings:BaseUrl"];

            client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public IActionResult Index()
        {
            return View();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (HttpContext.Session.GetString("role") != "Admin")
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                TempData["ErrorMessage"] = "You are not authorized to access this page.";
                return;
            }

            ViewData["Layout"] = "_AdminLayout";
        }

        #region Reservations

        public async Task<IActionResult> Reservations()
        {
            //List<UserReservationDto> userReservations = new List<UserReservationDto>();
            var reservationsViewModel = new AdminReservationsViewModel
            {
                UserReservations = new List<UserReservationViewModel>(),
                GuestReservations = new List<AdminGuestReservationViewModel>()
            };

            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var uReservationResponse = await client.GetAsync("UserReservation");
                var gReservationResponse = await client.GetAsync("GuestReservation");

            if (uReservationResponse.IsSuccessStatusCode) {
                var uReservations = await uReservationResponse.Content.ReadFromJsonAsync<List<AdminUserReservationDto>>();

                if(uReservations != null)
                {
                    foreach (var reservation in uReservations)
                    {
                        reservationsViewModel.UserReservations.Add(new UserReservationViewModel
                        {
                            Id = reservation.Id,
                            CustomerId = reservation.CustomerId,
                            CustomerName = reservation.CustomerName,
                            CustomerEmail = reservation.CustomerEmail,
                            CustomerPhone = reservation.CustomerPhone,
                            ReservationDate = reservation.ReservationDate,
                            Status = reservation.Status,
                            NumberOfGuests = reservation.NumberOfGuests,
                            ReservationHour = reservation.ReservationHour,
                            CreatedAt = reservation.CreatedAt
                        });
                    }
                }
            }
            else{
                TempData["ErrorMessage"] = $"Failed to load user reservations. Status: {uReservationResponse.StatusCode}";
            }

            if(gReservationResponse.IsSuccessStatusCode) {
                var gReservations = await gReservationResponse.Content.ReadFromJsonAsync<List<AdminGuestReservationDto>>();
                if(gReservations != null)
                {
                    foreach (var reservation in gReservations)
                    {
                        reservationsViewModel.GuestReservations.Add(new AdminGuestReservationViewModel
                        {
                            Id = reservation.Id,
                            FullName = reservation.FullName,
                            Email = reservation.Email,
                            PhoneNumber = reservation.PhoneNumber,
                            ReservationDate = reservation.ReservationDate,
                            NumberOfGuests = reservation.NumberOfGuests,
                            Status = reservation.Status,
                            CreatedAt = reservation.CreatedAt,
                            ReservationHour = reservation.ReservationHour
                        });
                    }
                }
            }
            else{
                TempData["ErrorMessage"] = $"Failed to load guest reservations. Status: {gReservationResponse.StatusCode}";
            }

            return View(reservationsViewModel);
        }

        [HttpPost]
        public IActionResult DeleteUserReservation(int id)
        {
                var token = HttpContext.Session.GetString("token");

                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = client.DeleteAsync($"UserReservation/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Reservation deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to delete reservation. {response.StatusCode}";
                }

            return RedirectToAction("Reservations");
        }

        [HttpPost]
        public IActionResult DeleteGuestReservation(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7284/api/");
                var token = HttpContext.Session.GetString("token");

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = client.DeleteAsync($"GuestReservation/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Reservation deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to delete reservation. Status: {response.StatusCode}";
                }
            }

            return RedirectToAction("Reservations");
        }

        [HttpPost]
        public IActionResult UpdateUserReservationStatus(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7284/api/");

                var token = HttpContext.Session.GetString("token");

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = client.PutAsync($"UserReservation/{id}/status", null).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Reservation status updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to update reservation status. Status: {response.StatusCode}";
                }
            }
            return RedirectToAction("Reservations");
        }

        #endregion

        #region Users
        public IActionResult Users()
        {
            return View();
        }
        #endregion

        #region  Foods

        public IActionResult Foods()
        {
            List<FoodDto> foods = new List<FoodDto>();

            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.GetFromJsonAsync<List<FoodDto>>("Food").Result;

            if(response != null)
            {
                foods.AddRange(response);
            }

            ViewBag.Foods = foods;

            return View();
        }

        [HttpPost]
        public IActionResult CreateFood(FoodDto foodDto)
        {

            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.PostAsJsonAsync("Food", foodDto).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Food item created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to create food item. Status: {response.StatusCode}";
            }

            return RedirectToAction("Foods");
        }

        [HttpPost]
        public IActionResult DeleteFood(int id)
        {
            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.DeleteAsync($"Food/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Food item deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete food item. Status: {response.StatusCode}";
            }

            return RedirectToAction("Foods");
        }

        #endregion

        #region  Drinks

        public IActionResult Drinks()
        {
            List<DrinkDto> drinks = new List<DrinkDto>();

            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.GetFromJsonAsync<List<DrinkDto>>("Drink").Result;

            if (response != null)
            {
                drinks.AddRange(response);
            }

            ViewBag.Drinks = drinks;

            return View();
        }

        [HttpPost]
        public IActionResult CreateDrink(DrinkDto drinkDto)
        {

            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.PostAsJsonAsync("Drink", drinkDto).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Drink item created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to create drink item. Status: {response.StatusCode}";
            }

            return RedirectToAction("Drinks");
        }

        [HttpPost]
        public IActionResult DeleteDrink(int id)
        {
            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.DeleteAsync($"Drink/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Drink item deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete drink item. Status: {response.StatusCode}";
            }

            return RedirectToAction("Drinks");
        }

        #endregion

        #region Menus
        public IActionResult Menus()
        {
            return View();
        }

        public IActionResult CreateMenu()
        {
            return View();
        }
        #endregion

    }
}
