using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public AuthController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // LOGIN PAGE
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // PROCESS LOGIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginData = new
            {
                Email = model.Email,
                Password = model.Password
            };

            var response = await _apiClient.SendAsync(
                HttpMethod.Post,
                "api/auth/login",
                loginData);

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    GetMessage(responseText, "Login failed. Check your details."));

                return View(model);
            }

            using var document = JsonDocument.Parse(responseText);

            var root = document.RootElement;

            string userId = GetProperty(root, "userId");
            string role = GetProperty(root, "role");
            string firstName = GetProperty(root, "firstName");
            string lastName = GetProperty(root, "lastName");

            if (string.IsNullOrWhiteSpace(userId) ||
                (role != "Organiser" && role != "Participant"))
            {
                ModelState.AddModelError(
                    "",
                    "The API returned incomplete account information.");

                return View(model);
            }

            // Store logged-in user information in MVC session.
            HttpContext.Session.SetString("UserId", userId);
            HttpContext.Session.SetString("Role", role);

            HttpContext.Session.SetString(
                "DisplayName",
                $"{firstName} {lastName}".Trim());

            // Redirect users according to their role.
            if (role == "Organiser")
            {
                return RedirectToAction("Index", "Organiser");
            }

            return RedirectToAction("Index", "Participant");
        }

        // REGISTRATION PAGE
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // PROCESS REGISTRATION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registerData = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role,
                Phone = model.Phone
            };

            var response = await _apiClient.SendAsync(
                HttpMethod.Post,
                "api/auth/register",
                registerData);

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    GetMessage(responseText, "Registration failed."));

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Registration successful. Please log in.";

            return RedirectToAction("Login");
        }

        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (!string.IsNullOrEmpty(
                HttpContext.Session.GetString("ApiSessionCookie")))
            {
                await _apiClient.SendAsync(
                    HttpMethod.Post,
                    "api/auth/logout");
            }

            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        // READ JSON PROPERTY
        private string GetProperty(
            JsonElement element,
            string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var value))
            {
                return value.ToString();
            }

            return string.Empty;
        }

        // READ API ERROR MESSAGE
        private string GetMessage(
            string responseText,
            string defaultMessage)
        {
            try
            {
                using var document = JsonDocument.Parse(responseText);

                if (document.RootElement.TryGetProperty(
                    "message", out var message))
                {
                    return message.GetString() ?? defaultMessage;
                }
            }
            catch
            {
                // Use the default message if the response is not JSON.
            }

            return defaultMessage;
        }
    }
}