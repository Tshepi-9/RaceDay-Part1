using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ManageEnrolmentsController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public ManageEnrolmentsController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("Role") == "Organiser"
                && !string.IsNullOrEmpty(
                    HttpContext.Session.GetString("UserId"));
        }

        // DISPLAY ENROLMENTS FOR AN EVENT
        public async Task<IActionResult> Index(int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            // Get event details.
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

            // Get enrolments for this event.
            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/enrolments/event/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    await response.Content.ReadAsStringAsync();

                return View(new List<EnrolmentViewModel>());
            }

            var enrolments = await response.Content
                .ReadFromJsonAsync<List<EnrolmentViewModel>>();

            return View(enrolments ?? new List<EnrolmentViewModel>());
        }

        // UPDATE ENROLMENT STATUS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            int eventId,
            string status)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var requestData = new
            {
                Status = status
            };

            var response = await _apiClient.SendAsync(
                HttpMethod.Put,
                $"api/enrolments/{id}/status",
                requestData);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Enrolment status updated successfully.";
            }
            else
            {
                TempData["Error"] =
                    await response.Content.ReadAsStringAsync();
            }

            return RedirectToAction(
                nameof(Index),
                new { eventId });
        }
    }
}