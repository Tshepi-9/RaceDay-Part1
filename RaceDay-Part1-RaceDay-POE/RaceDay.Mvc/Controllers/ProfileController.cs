using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public ProfileController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("UserId"));
        }

        // Display the user's profile.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                "api/profile/me");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to load your profile.";
                return View(new ProfileViewModel());
            }

            var profile = await _apiClient.ReadAsync<
                ProfileViewModel>(response);

            if (profile == null)
                return NotFound();

            return View(profile);
        }

        // Update the user's profile.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var updateData = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone
            };

            var response = await _apiClient.SendAsync(
                HttpMethod.Put,
                "api/profile/me",
                updateData);

            var responseText =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    using var document =
                        JsonDocument.Parse(responseText);

                    if (document.RootElement.TryGetProperty(
                        "message", out var message))
                    {
                        ModelState.AddModelError(
                            "",
                            message.GetString() ??
                            "Unable to update profile.");
                    }
                    else
                    {
                        ModelState.AddModelError(
                            "",
                            "Unable to update profile.");
                    }
                }
                catch
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to update profile.");
                }

                return View(model);
            }

            HttpContext.Session.SetString(
                "DisplayName",
                $"{model.FirstName} {model.LastName}");

            TempData["SuccessMessage"] =
                "Your profile was updated successfully.";

            return RedirectToAction("Index");
        }
    }
}