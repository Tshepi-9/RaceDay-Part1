using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantController : Controller
    {
        public IActionResult Index()
        {
            string? userId = HttpContext.Session.GetString("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (role != "Participant")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}