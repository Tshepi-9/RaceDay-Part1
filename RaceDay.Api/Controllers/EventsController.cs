using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Filters;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        // GET: api/events
        // Both Organisers and Participants can view events.
        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(new
            {
                message = "Events retrieved successfully.",
                events = new object[] { }
            });
        }

    // POST: api/events
    // Only Organisers can create events.
    [HttpPost]
        [RoleAuthorize("Organiser")]
        public IActionResult CreateEvent()
        {
            return Ok(new
            {
                message = "The Organiser is authorised to create an event."
            });
        }

        // PUT: api/events/1
        // Only Organisers can edit events.
        [HttpPut("{id:int}")]
        [RoleAuthorize("Organiser")]
        public IActionResult UpdateEvent(int id)
        {
            return Ok(new
            {
                message = "The Organiser is authorised to edit the event.",
                eventId = id
            });
        }

        // DELETE: api/events/1
        // Only Organisers can delete events.
        [HttpDelete("{id:int}")]
        [RoleAuthorize("Organiser")]
        public IActionResult DeleteEvent(int id)
        {
            return Ok(new
            {
                message = "The Organiser is authorised to delete the event.",
                eventId = id
            });
        }
    }

}
