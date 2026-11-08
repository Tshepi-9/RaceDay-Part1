using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // Both roles can view all events.
        [HttpGet]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Include(e => e.Organiser)
                .OrderBy(e => e.EventDate)
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.OrganiserID,
                    OrganiserName = e.Organiser == null
                        ? ""
                        : e.Organiser.FirstName + " " +
                          e.Organiser.LastName
                })
                .ToListAsync();

            return Ok(events);
        }

        // Both roles can view one event.
        [HttpGet("{id:int}")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Where(e => e.EventID == id)
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.OrganiserID,
                    e.RouteID
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            return Ok(eventItem);
        }

        // Organisers can view their own events.
        [HttpGet("mine")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> GetMyEvents()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var events = await _context.Events
                .Where(e => e.OrganiserID == userId)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return Ok(events);
        }

        // Only Organisers can create events.
        [HttpPost]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> CreateEvent(
            CreateEventRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (string.IsNullOrWhiteSpace(request.EventName) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                string.IsNullOrWhiteSpace(request.Location))
            {
                return BadRequest(new
                {
                    message = "Event name, description and location are required."
                });
            }

            if (request.EventDate.Date < DateTime.Today)
                return BadRequest(new { message = "Event date cannot be in the past." });

            if (request.Distance <= 0)
                return BadRequest(new { message = "Distance must be greater than zero." });

            if (!IsValidEventType(request.EventType))
                return BadRequest(new
                {
                    message = "Event type must be Run, Walk or Cycle."
                });

            if (request.RouteID.HasValue)
            {
                bool routeExists = await _context.Routes.AnyAsync(
                    r => r.RouteID == request.RouteID.Value);

                if (!routeExists)
                    return BadRequest(new { message = "Route does not exist." });
            }

            var eventItem = new Event
            {
                OrganiserID = userId,
                RouteID = request.RouteID,
                EventName = request.EventName.Trim(),
                Description = request.Description.Trim(),
                EventDate = request.EventDate,
                Location = request.Location.Trim(),
                Distance = request.Distance,
                EventType = request.EventType.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventID },
                new
                {
                    message = "Event created successfully.",
                    eventItem.EventID,
                    eventItem.EventName
                });
        }

        // An Organiser can update only their own events.
        [HttpPut("{id:int}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            UpdateEventRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrganiserID != userId)
                return Forbid();

            if (string.IsNullOrWhiteSpace(request.EventName) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                string.IsNullOrWhiteSpace(request.Location))
            {
                return BadRequest(new
                {
                    message = "Event name, description and location are required."
                });
            }

            if (request.EventDate.Date < DateTime.Today)
                return BadRequest(new { message = "Event date cannot be in the past." });

            if (request.Distance <= 0)
                return BadRequest(new { message = "Distance must be greater than zero." });

            if (!IsValidEventType(request.EventType))
                return BadRequest(new
                {
                    message = "Event type must be Run, Walk or Cycle."
                });

            if (request.RouteID.HasValue)
            {
                bool routeExists = await _context.Routes.AnyAsync(
                    r => r.RouteID == request.RouteID.Value);

                if (!routeExists)
                    return BadRequest(new { message = "Route does not exist." });
            }

            eventItem.EventName = request.EventName.Trim();
            eventItem.Description = request.Description.Trim();
            eventItem.EventDate = request.EventDate;
            eventItem.Location = request.Location.Trim();
            eventItem.Distance = request.Distance;
            eventItem.EventType = request.EventType.Trim();
            eventItem.RouteID = request.RouteID;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully." });
        }

        // An Organiser can delete only their own events.
        [HttpDelete("{id:int}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrganiserID != userId)
                return Forbid();

            bool hasCategories = await _context.Categories
                .AnyAsync(c => c.EventID == id);

            bool hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.EventID == id);

            if (hasCategories || hasEnrolments)
            {
                return BadRequest(new
                {
                    message = "This event has categories or enrolments and cannot be deleted."
                });
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event deleted successfully." });
        }

        private static bool IsValidEventType(string? eventType)
        {
            return eventType != null &&
                new[] { "Run", "Walk", "Cycle" }
                    .Contains(eventType.Trim(), StringComparer.OrdinalIgnoreCase);
        }
    }
}