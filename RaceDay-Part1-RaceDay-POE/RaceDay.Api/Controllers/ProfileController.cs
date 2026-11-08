using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ProfileController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> GetMyProfile()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "Profile not found." });

            return Ok(new
            {
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Phone,
                user.Role
            });
        }

        [HttpPut("me")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> UpdateMyProfile(
            UpdateProfileRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "Profile not found." });

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    message = "First name, last name and email are required."
                });
            }

            string email = request.Email.Trim();

            bool emailExists = await _context.Users.AnyAsync(
                u => u.Email == email && u.UserID != userId);

            if (emailExists)
                return BadRequest(new { message = "Email is already in use." });

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = email;
            user.Phone = request.Phone?.Trim();

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully." });
        }
    }
}