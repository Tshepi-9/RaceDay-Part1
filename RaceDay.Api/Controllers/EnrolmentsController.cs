using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/enrolments")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // Participants enrol themselves in an event and category.
        [HttpPost]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> CreateEnrolment(
            CreateEnrolmentRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == request.EventID);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.EventDate.Date < DateTime.Today)
                return BadRequest(new
                {
                    message = "You cannot enrol in an event that has already taken place."
                });

            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryID == request.CategoryID &&
                    c.EventID == request.EventID);

            if (category == null)
                return BadRequest(new
                {
                    message = "The selected category does not belong to this event."
                });

            bool alreadyEnrolled = await _context.Enrolments.AnyAsync(e =>
                e.ParticipantID == userId &&
                e.EventID == request.EventID);

            if (alreadyEnrolled)
                return BadRequest(new
                {
                    message = "You are already enrolled in this event."
                });

            var enrolment = new Enrolment
            {
                ParticipantID = userId,
                EventID = request.EventID,
                CategoryID = request.CategoryID,
                EnrolmentDate = DateTime.Now,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Enrolment created successfully.",
                enrolment.EnrolmentID,
                enrolment.EventID,
                enrolment.CategoryID,
                enrolment.Status
            });
        }

        // Participants can view their own enrolments.
        [HttpGet("mine")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var enrolments = await _context.Enrolments
                .Where(e => e.ParticipantID == userId)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.EventID,
                    EventName = e.Event == null ? "" : e.Event.EventName,
                    e.CategoryID,
                    CategoryName = e.Category == null
                        ? ""
                        : e.Category.CategoryName,
                    e.EnrolmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // Only the Organiser who owns the event can view its enrolments.
        [HttpGet("event/{eventId:int}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrganiserID != userId)
                return Forbid();

            var enrolments = await _context.Enrolments
                .Where(e => e.EventID == eventId)
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.EventID,
                    e.ParticipantID,
                    ParticipantName = e.Participant == null
                        ? ""
                        : e.Participant.FirstName + " " +
                          e.Participant.LastName,
                    e.CategoryID,
                    CategoryName = e.Category == null
                        ? ""
                        : e.Category.CategoryName,
                    e.EnrolmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // Only the Organiser who owns the event can change enrolment status.
        [HttpPut("{id:int}/status")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> UpdateEnrolmentStatus(
            int id,
            UpdateEnrolmentStatusRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == id);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            if (enrolment.Event == null ||
                enrolment.Event.OrganiserID != userId)
            {
                return Forbid();
            }

            string[] validStatuses =
                { "Pending", "Confirmed", "Cancelled" };

            string? status = validStatuses.FirstOrDefault(s =>
                s.Equals(
                    request.Status,
                    StringComparison.OrdinalIgnoreCase));

            if (status == null)
                return BadRequest(new
                {
                    message = "Status must be Pending, Confirmed or Cancelled."
                });

            enrolment.Status = status;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolment status updated successfully." });
        }
    }
}