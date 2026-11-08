using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ManageEventsController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public ManageEventsController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("Role") == "Organiser"
                && !string.IsNullOrEmpty(
                    HttpContext.Session.GetString("UserId"));
        }

        // DISPLAY EVENTS
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                "api/events/mine");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Could not load events: " +
                    await response.Content.ReadAsStringAsync();

                return View(new List<EventViewModel>());
            }

            var events = await response.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            return View(events ?? new List<EventViewModel>());
        }

        // DISPLAY CREATE FORM
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            return View(new EventViewModel());
        }

        // CREATE EVENT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.Error =
                    "Please correct the validation errors below.";

                return View(model);
            }

            var eventData = new
            {
                EventName = model.EventName,
                Description = model.Description,
                EventDate = model.EventDate,
                Location = model.Location,
                Distance = model.Distance,
                EventType = model.EventType,
                RouteID = model.RouteID
            };

            var response = await _apiClient.SendAsync(
                HttpMethod.Post,
                "api/events",
                eventData);

            var responseText =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    $"API Error ({(int)response.StatusCode}): {responseText}";

                return View(model);
            }

            TempData["Success"] = "Event created successfully!";

            return RedirectToAction(nameof(Index));
        }

        // DISPLAY EDIT FORM
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/events/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var eventItem = await response.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return RedirectToAction(nameof(Index));

            return View(eventItem);
        }

        // UPDATE EVENT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            EventViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiClient.SendAsync(
                HttpMethod.Put,
                $"api/events/{id}",
                model);

            var responseText =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    $"API Error ({(int)response.StatusCode}): {responseText}";

                return View(model);
            }

            TempData["Success"] = "Event updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        // DELETE EVENT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var response = await _apiClient.SendAsync(
                HttpMethod.Delete,
                $"api/events/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event deleted successfully!";
            }
            else
            {
                TempData["Error"] =
                    await response.Content.ReadAsStringAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}