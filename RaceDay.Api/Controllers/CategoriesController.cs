using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // Both roles can view categories for an event.
        [HttpGet("events/{eventId:int}/categories")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
                return NotFound(new { message = "Event not found." });

            var categories = await _context.Categories
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            return Ok(categories);
        }

        // Only the Organiser who owns the event can add categories.
        [HttpPost("events/{eventId:int}/categories")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> CreateCategory(
            int eventId,
            CreateCategoryRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrganiserID != userId)
                return Forbid();

            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return BadRequest(new { message = "Category name is required." });

            if (request.MinimumAge < 0 || request.MaximumAge < 0)
                return BadRequest(new { message = "Age cannot be negative." });

            if (request.MinimumAge.HasValue &&
                request.MaximumAge.HasValue &&
                request.MinimumAge > request.MaximumAge)
            {
                return BadRequest(new
                {
                    message = "Minimum age cannot exceed maximum age."
                });
            }

            if (request.CategoryDistance.HasValue &&
                request.CategoryDistance.Value <= 0)
            {
                return BadRequest(new
                {
                    message = "Category distance must be greater than zero."
                });
            }

            bool duplicate = await _context.Categories.AnyAsync(c =>
                c.EventID == eventId &&
                c.CategoryName == request.CategoryName.Trim());

            if (duplicate)
                return BadRequest(new
                {
                    message = "This category already exists for the event."
                });

            var category = new Category
            {
                EventID = eventId,
                CategoryName = request.CategoryName.Trim(),
                MinimumAge = request.MinimumAge,
                MaximumAge = request.MaximumAge,
                CategoryDistance = request.CategoryDistance
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Category created successfully.",
                category.CategoryID,
                category.EventID,
                category.CategoryName
            });
        }

        // Only the Organiser who owns the event can update a category.
        [HttpPut("categories/{id:int}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            CreateCategoryRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            if (category.Event == null ||
                category.Event.OrganiserID != userId)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return BadRequest(new { message = "Category name is required." });

            if (request.MinimumAge < 0 || request.MaximumAge < 0)
                return BadRequest(new { message = "Age cannot be negative." });

            if (request.MinimumAge.HasValue &&
                request.MaximumAge.HasValue &&
                request.MinimumAge > request.MaximumAge)
            {
                return BadRequest(new
                {
                    message = "Minimum age cannot exceed maximum age."
                });
            }

            if (request.CategoryDistance.HasValue &&
                request.CategoryDistance.Value <= 0)
            {
                return BadRequest(new
                {
                    message = "Category distance must be greater than zero."
                });
            }

            bool duplicate = await _context.Categories.AnyAsync(c =>
                c.EventID == category.EventID &&
                c.CategoryID != id &&
                c.CategoryName == request.CategoryName.Trim());

            if (duplicate)
                return BadRequest(new
                {
                    message = "This category already exists for the event."
                });

            category.CategoryName = request.CategoryName.Trim();
            category.MinimumAge = request.MinimumAge;
            category.MaximumAge = request.MaximumAge;
            category.CategoryDistance = request.CategoryDistance;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully." });
        }

        // Only the Organiser who owns the event can delete a category.
        [HttpDelete("categories/{id:int}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            if (category.Event == null ||
                category.Event.OrganiserID != userId)
            {
                return Forbid();
            }

            bool hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.CategoryID == id);

            if (hasEnrolments)
                return BadRequest(new
                {
                    message = "This category has enrolments and cannot be deleted."
                });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully." });
        }
    }
}