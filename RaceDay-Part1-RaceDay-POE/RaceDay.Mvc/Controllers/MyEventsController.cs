using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class MyEventsController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public MyEventsController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            string? userId =
                HttpContext.Session.GetString("UserId");

            string? role =
                HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            if (role != "Participant")
                return RedirectToAction("Index", "Home");

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                "api/enrolments/mine");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Unable to load your event entries. Please try again.";

                return View(new List<EnrolmentViewModel>());
            }

            var enrolments = await _apiClient.ReadAsync<
                List<EnrolmentViewModel>>(response);

            return View(enrolments ??
                new List<EnrolmentViewModel>());
        }
    }
}