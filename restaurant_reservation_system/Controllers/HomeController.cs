using Microsoft.AspNetCore.Mvc;
using restaurant_reservation_system.Models.Dto;

namespace restaurant_reservation_system.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client;
        public HomeController(IConfiguration config)
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


        public IActionResult Menus(int? id)
        {
            List<MenuDto> menus = new List<MenuDto>();

                var token = HttpContext.Session.GetString("token");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var menusResponse = client.GetFromJsonAsync<List<MenuDto>>("Menu").Result ?? new List<MenuDto>();
                menus.AddRange(menusResponse);

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

    }
}
