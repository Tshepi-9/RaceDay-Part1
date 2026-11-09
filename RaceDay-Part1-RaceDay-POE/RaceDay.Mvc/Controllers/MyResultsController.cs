using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class MyResultsController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public MyResultsController(RaceDayApiClient apiClient)
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
                "api/results/mine");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Unable to load your results. Please try again.";

                return View(new List<ResultViewModel>());
            }

            var results = await _apiClient.ReadAsync<
                List<ResultViewModel>>(response);

            return View(results ?? new List<ResultViewModel>());
        }
    }
}