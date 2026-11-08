using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserController : Controller
    {
        public IActionResult Index()
        {
            // Check whether the user is logged in.
            string? userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Check whether the user is an Organiser.
            string? role = HttpContext.Session.GetString("Role");

            if (role != "Organiser")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}