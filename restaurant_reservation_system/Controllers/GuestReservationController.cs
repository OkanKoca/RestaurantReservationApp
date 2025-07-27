using Microsoft.AspNetCore.Mvc;
using restaurant_reservation_system.Models;

namespace restaurant_reservation_system.Controllers
{
    public class GuestReservationController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BookReservation(GuestReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7284/api/");
                    var response = await client.PostAsJsonAsync("guestreservation", model);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Your reservation is successfully done!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"An error occurred: {error}";
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please check your input and try again.";
            }

            return View("Create", model);
        }
    }
}
