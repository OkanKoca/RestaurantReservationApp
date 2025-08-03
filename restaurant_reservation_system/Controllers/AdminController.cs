using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using restaurant_reservation.Models;
using restaurant_reservation_system.Models.Dto;
using restaurant_reservation_system.Models.ViewModel;

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

        [HttpPost]
        public IActionResult UpdateGuestReservationStatus(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7284/api/");

                var token = HttpContext.Session.GetString("token");

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = client.PutAsync($"GuestReservation/{id}/status", null).Result;

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
            List<MenuDto> menus = new List<MenuDto>();

            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var foodResponse = client.GetFromJsonAsync<List<FoodDto>>("Food").Result;
            var menuResponse = client.GetFromJsonAsync<List<MenuDto>>("Menu").Result;

            if (foodResponse != null)
            {
                foods.AddRange(foodResponse);
            }

            if (menuResponse != null) { 
                menus.AddRange(menuResponse);
            }

            ViewBag.Foods = foods;
            ViewBag.Menus = menus;

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

        [HttpPost]
        public async Task<IActionResult> UpdateFood(FoodDto foodDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Food fields are not filled correctly.";
                return RedirectToAction("Foods");
            }

            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.PutAsJsonAsync($"Food/{foodDto.Id}", foodDto);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Food updated successfully.";
                    return RedirectToAction("Foods");
                }
                TempData["ErrorMessage"] = "Food update failed.";
                return RedirectToAction("Foods");
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not get API response.";
                return RedirectToAction("Foods");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFoodUpdateForm(int id)
        {
            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var food = await client.GetFromJsonAsync<FoodDto>($"Food/{id}");
                if (food == null)
                {
                    return NotFound();
                }

                var menus = await client.GetFromJsonAsync<List<MenuDto>>("Menu") ?? new List<MenuDto>();

                ViewBag.Menus = menus;

                return PartialView("_FoodUpdateForm", food);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to load food data" });
            }
        }

        #endregion

        #region  Drinks

        public IActionResult Drinks()
        {
            List<DrinkDto> drinks = new List<DrinkDto>();
            List<MenuDto> menus = new List<MenuDto>();

            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var drinkResponse = client.GetFromJsonAsync<List<DrinkDto>>("Drink").Result;
            var menuResponse = client.GetFromJsonAsync<List<MenuDto>>("Menu").Result;

            if (drinkResponse != null)
            {
                drinks.AddRange(drinkResponse);
            }

            if(menuResponse != null)
            {
                menus.AddRange(menuResponse);
            }

            ViewBag.Drinks = drinks;
            ViewBag.Menus = menus;

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

        [HttpPost]
        public async Task<IActionResult> UpdateDrink(DrinkDto drinkDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Drink fields are not filled correctly.";
                return RedirectToAction("Drinks");
            }

            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.PutAsJsonAsync($"Drink/{drinkDto.Id}", drinkDto);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Drink updated successfully.";
                    return RedirectToAction("Drinks");
                }
                TempData["ErrorMessage"] = "Drink update failed.";
                return RedirectToAction("Drinks");
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not get API response.";
                return RedirectToAction("Drinks");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDrinkUpdateForm(int id)
        {
            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var drink = await client.GetFromJsonAsync<DrinkDto>($"Drink/{id}");
                if (drink == null)
                {
                    return NotFound();
                }

                var menus = await client.GetFromJsonAsync<List<MenuDto>>("Menu") ?? new List<MenuDto>();
                
                ViewBag.Menus = menus;
                
                return PartialView("_DrinkUpdateForm", drink);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to load drink data" });
            }
        }

        #endregion

        #region Menus
        public IActionResult Menus(int? id)
        {
            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var menus = client.GetFromJsonAsync<List<MenuDto>>("Menu").Result ?? new List<MenuDto>();

            if (id.HasValue)
            {
                var drinkResponse = client.GetFromJsonAsync<List<DrinkDto>>($"Menu/{id}/drinks").Result;
                var foodResponse = client.GetFromJsonAsync<List<FoodDto>>($"Menu/{id}/foods").Result;

                ViewBag.Drinks = drinkResponse;
                ViewBag.Foods = foodResponse;
                ViewBag.SelectedMenuId = id;
            }

            return View(menus);
        }

        [HttpPost]
        public IActionResult CreateMenu()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteMenu(int id)
        {
            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.DeleteAsync($"Menu/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Menu item deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete menu item. Status: {response.StatusCode}";
            }

            return RedirectToAction("Menus");
        }

        #endregion

        #region Tables

        public IActionResult Tables(int? id, DateTime? date = null)
        {
            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var selectedDate = date ?? DateTime.Today; // Tarih seçilmemişse bugünü kullan
            var tables = client.GetFromJsonAsync<List<TableDto>>("Table").Result ?? new List<TableDto>();

            // Her masa için doluluk oranını al
            foreach (var table in tables)
            {
                try
                {
                    var response = client.GetAsync($"Table/{table.Id}/Occupancy?dateTime={selectedDate:yyyy-MM-dd}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var occupancyRate = response.Content.ReadFromJsonAsync<double>().Result;
                        ViewData[$"TableOccupancy_{table.Id}"] = occupancyRate;
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunda varsayılan değer 0 olarak kalacak
                    ViewData[$"TableOccupancy_{table.Id}"] = 0;
                }
            }

            if (id.HasValue)
            {
                var userReservations = client.GetFromJsonAsync<List<UserReservationDto>>($"Table/{id}/UserReservations").Result ?? new List<UserReservationDto>();
                var guestReservations = client.GetFromJsonAsync<List<AdminGuestReservationDto>>($"Table/{id}/GuestReservations").Result ?? new List<AdminGuestReservationDto>();

                ViewBag.UserReservations = userReservations;
                ViewBag.GuestReservations = guestReservations;
                ViewBag.SelectedTableId = id;
            }

            ViewBag.SelectedDate = selectedDate; // View'da kullanmak için tarihi ViewBag'e ekle
            return View(tables);
        }

        [HttpPost]
        public IActionResult CreateTable(TableDto tableDto)
        {
            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.PostAsJsonAsync<TableDto>("Table", tableDto).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Table item created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to create table item. Status: {response.StatusCode}";
            }

            return RedirectToAction("Tables");
        }

        [HttpPost]
        public IActionResult DeleteTable(int id)
        {
            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = client.DeleteAsync($"Table/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Table item deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete table item. Status: {response.StatusCode}";
            }

            return RedirectToAction("Tables");
        }

        #endregion

    }
}
