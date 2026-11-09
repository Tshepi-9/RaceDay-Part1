using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ManageResultsController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public ManageResultsController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("Role") == "Organiser"
                && !string.IsNullOrEmpty(
                    HttpContext.Session.GetString("UserId"));
        }

        public async Task<IActionResult> Index(int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var eventResponse = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/events/{eventId}");

            if (!eventResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not find this event.";
                return RedirectToAction("Index", "ManageEvents");
            }

            var eventItem = await eventResponse.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return RedirectToAction("Index", "ManageEvents");

            ViewBag.EventName = eventItem.EventName;
            ViewBag.EventId = eventId;

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/results/event/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    await response.Content.ReadAsStringAsync();

                return View(new List<ResultViewModel>());
            }

            var results = await response.Content
                .ReadFromJsonAsync<List<ResultViewModel>>();

            return View(results ?? new List<ResultViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/enrolments/event/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    "Could not load the event's enrolments.";

                return RedirectToAction(
                    "Index", "ManageResults",
                    new { eventId });
            }

            var enrolments = await response.Content
                .ReadFromJsonAsync<List<EnrolmentViewModel>>();

            ViewBag.EventId = eventId;

            return View(enrolments ?? new List<EnrolmentViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int eventId,
            int enrolmentId,
            string? finishTime,
            int? finishingPosition)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            TimeSpan? parsedFinishTime = null;

            if (!string.IsNullOrWhiteSpace(finishTime))
            {
                if (!TimeSpan.TryParse(finishTime, out TimeSpan parsed))
                {
                    ViewBag.Error =
                        "Enter a valid finishing time, for example 01:25:30.";

                    return await Create(eventId);
                }

                parsedFinishTime = parsed;
            }

            var resultData = new
            {
                EnrolmentID = enrolmentId,
                FinishTime = parsedFinishTime,
                FinishingPosition = finishingPosition
            };

            var response = await _apiClient.SendAsync(
                HttpMethod.Post,
                "api/results",
                resultData);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    await response.Content.ReadAsStringAsync();

                return RedirectToAction(
                    nameof(Create),
                    new { eventId });
            }

            TempData["Success"] = "Result recorded successfully!";

            return RedirectToAction(
                nameof(Index),
                new { eventId });
        }
    }
}