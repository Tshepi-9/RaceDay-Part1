using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // Organisers record results for enrolments in their own events.
        [HttpPost]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> CreateResult(
            CreateResultRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentID == request.EnrolmentID);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            if (enrolment.Event == null ||
                enrolment.Event.OrganiserID != userId)
            {
                return Forbid();
            }

            if (request.FinishingPosition.HasValue &&
                request.FinishingPosition.Value <= 0)
            {
                return BadRequest(new
                {
                    message = "Finishing position must be greater than zero."
                });
            }

            if (request.FinishTime.HasValue &&
                request.FinishTime.Value < TimeSpan.Zero)
            {
                return BadRequest(new
                {
                    message = "Finish time cannot be negative."
                });
            }

            bool resultExists = await _context.Results.AnyAsync(
                r => r.EnrolmentID == request.EnrolmentID);

            if (resultExists)
                return BadRequest(new
                {
                    message = "A result already exists for this enrolment."
                });

            var result = new Result
            {
                EnrolmentID = request.EnrolmentID,
                FinishTime = request.FinishTime,
                FinishingPosition = request.FinishingPosition,
                RecordedAt = DateTime.Now
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Result recorded successfully.",
                result.ResultID,
                result.EnrolmentID,
                result.FinishTime,
                result.FinishingPosition
            });
        }

        // Participants can view their own results.
        [HttpGet("mine")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Event)
                .Where(r => r.Enrolment != null &&
                            r.Enrolment.ParticipantID == userId)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    EventName = r.Enrolment!.Event == null
                        ? ""
                        : r.Enrolment.Event.EventName,
                    r.FinishTime,
                    r.FinishingPosition,
                    r.RecordedAt
                })
                .ToListAsync();

            return Ok(results);
        }

        // Organisers can view results for their own events.
        [HttpGet("event/{eventId:int}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrganiserID != userId)
                return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Participant)
                .Where(r => r.Enrolment != null &&
                            r.Enrolment.EventID == eventId)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    ParticipantName = r.Enrolment!.Participant == null
                        ? ""
                        : r.Enrolment.Participant.FirstName + " " +
                          r.Enrolment.Participant.LastName,
                    r.FinishTime,
                    r.FinishingPosition,
                    r.RecordedAt
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}