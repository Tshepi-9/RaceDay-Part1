using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ManageCategoriesController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public ManageCategoriesController(RaceDayApiClient apiClient)
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

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/events/{eventId}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index", "ManageEvents");

            var eventItem = await response.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return RedirectToAction("Index", "ManageEvents");

            ViewBag.EventName = eventItem.EventName;
            ViewBag.EventId = eventId;

            var categoryResponse = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/events/{eventId}/categories");

            if (!categoryResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load categories.";
                return View(new List<CategoryViewModel>());
            }

            var categories = await categoryResponse.Content
                .ReadFromJsonAsync<List<CategoryViewModel>>();

            return View(categories ?? new List<CategoryViewModel>());
        }

        [HttpGet]
        public IActionResult Create(int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            return View(new CategoryViewModel
            {
                EventID = eventId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiClient.SendAsync(
                HttpMethod.Post,
                $"api/events/{model.EventID}/categories",
                model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }

            TempData["Success"] = "Category created successfully.";

            return RedirectToAction(
                nameof(Index),
                new { eventId = model.EventID });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var response = await _apiClient.SendAsync(
                HttpMethod.Get,
                $"api/events/{eventId}/categories");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index), new { eventId });

            var categories = await response.Content
                .ReadFromJsonAsync<List<CategoryViewModel>>();

            var category = categories?
                .FirstOrDefault(c => c.CategoryID == id);

            if (category == null)
                return RedirectToAction(nameof(Index), new { eventId });

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiClient.SendAsync(
                HttpMethod.Put,
                $"api/categories/{model.CategoryID}",
                model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }

            TempData["Success"] = "Category updated successfully.";

            return RedirectToAction(
                nameof(Index),
                new { eventId = model.EventID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var response = await _apiClient.SendAsync(
                HttpMethod.Delete,
                $"api/categories/{id}");

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Category deleted successfully.";
            else
                TempData["Error"] =
                    await response.Content.ReadAsStringAsync();

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}