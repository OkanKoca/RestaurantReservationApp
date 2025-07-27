using Microsoft.AspNetCore.Mvc;

namespace restaurant_reservation_system.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Menu()
        {
            return View();
        }

    }
}
