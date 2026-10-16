using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Filters;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/enrolments")]
    public class EnrolmentsController : ControllerBase
    {
        // POST: api/enrolments
        // Only Participants can enrol in events.
        [HttpPost]
        [RoleAuthorize("Participant")]
        public IActionResult CreateEnrolment()
        {
            return Ok(new
            {
                message = "The Participant is authorised to enrol in an event."
            });
        }
    }
}
