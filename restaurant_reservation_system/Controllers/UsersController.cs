using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using restaurant_reservation_system.Models.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication;
//using System.Security.Claims;

namespace restaurant_reservation_system.Controllers
{
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7284/api/");
                    var response = await client.PostAsJsonAsync("users/login", model);

                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

                        if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                        {
                            HttpContext.Session.SetString("token", tokenResponse.Token);

                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadJwtToken(tokenResponse.Token);
                            //var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Customer";

                            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role); // somehow it returns null 
                            var roleClaimRaw = jwtToken.Claims.FirstOrDefault(c => c.Type == "role"); // this is an alternative way if it is null

                            var role = roleClaim?.Value ?? roleClaimRaw?.Value ?? "Customer";

                            HttpContext.Session.SetString("role", role);

                            TempData["SuccessMessage"] = "Logged in successfully!!";
                            return RedirectToAction("Index", "Home");
                        }

                        ModelState.AddModelError("", "Token alınamadı.");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Login failed: {errorContent}");
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Session.GetString("token");

            if (!string.IsNullOrEmpty(token))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7284/api/");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    var response = await client.PostAsync("users/logout", null);

                    if (!response.IsSuccessStatusCode)
                    {

                    }
                }
            }

            HttpContext.Session.Remove("token");
            TempData["SuccessMessage"] = "Logged out successfully.";
            return RedirectToAction("Index", "Home");
        }



        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7284/api/");

                    var response = client.PostAsJsonAsync("users/register", model).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Registration successfully done! Please log in.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Registration failed! Please try again."; 
                        var errorContent = response.Content.ReadAsStringAsync().Result;
                        ModelState.AddModelError("", $"Registraion failed: {errorContent}");
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
