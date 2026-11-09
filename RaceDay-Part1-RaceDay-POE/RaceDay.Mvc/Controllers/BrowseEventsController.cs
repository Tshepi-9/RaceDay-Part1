using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class BrowseEventsController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public BrowseEventsController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // Check that the logged-in user is a Participant.
        private bool IsParticipant()
        {
            return HttpContext.Session.GetString("Role") == "Participant"
                && !string.IsNullOrEmpty(
                    HttpContext.Session.GetString("UserId"));
        }

        // Display all upcoming events.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsParticipant())
                return RedirectToAction("Login", "Auth");

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                "api/events");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Unable to load events. Please try again.";

                return View(new List<BrowseEventViewModel>());
            }

            var events = await _apiClient.ReadAsync<
                List<BrowseEventViewModel>>(response);

            var upcomingEvents =
                (events ?? new List<BrowseEventViewModel>())
                .Where(e => e.EventDate.Date >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToList();

            return View(upcomingEvents);
        }

        // Display the details and categories of one event.
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsParticipant())
                return RedirectToAction("Login", "Auth");

            var eventResponse = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/events/{id}");

            if (!eventResponse.IsSuccessStatusCode)
                return NotFound();

            var eventItem = await _apiClient.ReadAsync<
                BrowseEventViewModel>(eventResponse);

            if (eventItem == null)
                return NotFound();

            var categoryResponse = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/events/{id}/categories");

            if (!categoryResponse.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Unable to load event categories.";

                ViewBag.Categories =
                    new List<CategoryViewModel>();

                return View(eventItem);
            }

            var categories = await _apiClient.ReadAsync<
                List<CategoryViewModel>>(categoryResponse);

            ViewBag.Categories = categories ??
                new List<CategoryViewModel>();

            return View(eventItem);
        }

        // Allow a Participant to enter an event.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterEvent(
            int eventId,
            int categoryId)
        {
            if (!IsParticipant())
                return RedirectToAction("Login", "Auth");

            var enrolmentData = new
            {
                EventID = eventId,
                CategoryID = categoryId
            };

            var response = await _apiClient.SendAsync(
                HttpMethod.Post,
                "api/enrolments",
                enrolmentData);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Congratulations! You have successfully entered this event.";

                return RedirectToAction(
                    "Details",
                    new { id = eventId });
            }

            var errorText =
                await response.Content.ReadAsStringAsync();

            if (errorText.Contains(
                "already enrolled",
                StringComparison.OrdinalIgnoreCase))
            {
                TempData["SuccessMessage"] =
                    "You are already successfully enrolled in this event.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Event entry failed. " + errorText;
            }

            return RedirectToAction(
                "Details",
                new { id = eventId });
        }
    }
}