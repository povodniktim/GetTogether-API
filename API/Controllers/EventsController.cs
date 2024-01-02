using API.Helpers;
using API.Models;
using API.Models.Requests.Event;
using API.Models.Responses.Activity;
using API.Models.Responses.Event;
using API.Models.Responses.User;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private readonly GetTogetherContext _context;

        public EventsController(GetTogetherContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEventRequest>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? title = null,
        [FromQuery] string? location = null,
        [FromQuery] string[]? dates = null,
        [FromQuery] int? maxParticipants = null,
        [FromQuery] string? visibility = null,
        [FromQuery] string? organizer = null)
        {
            try
            {
                if (dates == null)
                {
                    dates = new string[] { };
                }
                else if (!dates.All(DateHelper.IsValidShortDateFormat))
                {
                    throw new Exception("INVALID_DATES");
                }

                if (page < 1)
                {
                    page = 1;
                }

                IQueryable<GetEventResponse> baseQuery = _context.Events
                    .Include(e => e.Organizer)
                    .Where(e => e.Date >= DateTime.Now)
                    .OrderBy(e => e.Date)
                    .Select(e => new GetEventResponse
                    {
                        Id = e.Id,
                        Title = e.Title,
                        CreatedAt = e.CreatedAt,
                        Description = e.Description,
                        Date = e.Date,
                        Location = e.Location,
                        MaxParticipants = e.MaxParticipants,
                        Visibility = e.Visibility ?? "public",
                        Organizer = new GetOrganizerResponse
                        {
                            Id = e.Organizer.Id,
                            FirstName = e.Organizer.FirstName,
                            LastName = e.Organizer.LastName,
                            Email = e.Organizer.Email,
                            CreatedAt = e.Organizer.CreatedAt,
                            ProfileImageUrl = e.Organizer.ProfileImageUrl
                        },
                        Activity = new GetActivityResponse
                        {
                            Id = e.Activity.Id,
                            Name = e.Activity.Name,
                            IconClassName = e.Activity.IconClassName
                        },
                        Attendees = _context.EventParticipants
                            .Where(ep => ep.EventId == e.Id)
                            .Select(ep => new GetAttendeeResponse
                            {
                                Id = ep.ParticipantId,
                                FirstName = ep.Participant.FirstName,
                                LastName = ep.Participant.LastName,
                                ProfileImageUrl = ep.Participant.ProfileImageUrl
                            })
                            .ToList()
                    });

                if (!string.IsNullOrWhiteSpace(title))
                {
                    baseQuery = baseQuery.Where(e => e.Title.ToLower().Contains(title.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    baseQuery = baseQuery.Where(e => e.Location.ToLower().Contains(location.ToLower()));
                }

                if (dates.Any())
                {
                    baseQuery = baseQuery.Where(e => dates.Contains(e.Date.Date.ToString()));
                }

                if (maxParticipants.HasValue)
                {
                    baseQuery = baseQuery.Where(e => e.MaxParticipants <= maxParticipants);
                }

                if (!string.IsNullOrWhiteSpace(visibility))
                {
                    baseQuery = baseQuery.Where(e => e.Visibility.ToLower() == visibility.ToLower());
                }

                if (!string.IsNullOrWhiteSpace(organizer))
                {
                    baseQuery = baseQuery.Where(e =>
                        (e.Organizer.FirstName.ToLower() + " " + e.Organizer.LastName.ToLower()).Contains(organizer.ToLower()) ||
                        e.Organizer.Email.ToLower().Contains(organizer.ToLower()));
                }

                int count = await baseQuery.CountAsync();

                var events = await baseQuery
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .ToListAsync();


                return Ok(
                    new SuccessResponse<GetMultipleResponse<GetEventResponse>>(
                        new GetMultipleResponse<GetEventResponse>
                        {
                            Count = count,
                            Page = page,
                            PerPage = perPage,
                            Collection = events
                        },
                        "List of events"
                    )
                );

            }
            catch (Exception e)
            {
                return BadRequest(
                    new ErrorResponse<string>(
                        new string[] { e.Message },
                        e.Message == "INVALID_DATES" ? "Invalid dates" : "Failed to get events"
                    )
                );
            }

        }


        [HttpPost]
        public async Task<ActionResult<Event>> Create([FromBody] CreateEventRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrganizer = await _context.Users
                .Include(u => u.Events)
                .FirstOrDefaultAsync(u => u.Id == request.OrganizerId);

            var existingActivity = await _context.Activities
                .Include(a => a.Events)
                .FirstOrDefaultAsync(a => a.Id == request.ActivityId);

            if (existingOrganizer == null || existingActivity == null)
            {
                return NotFound();
            }

            var newEvent = new Event
            {
                Title = request.Title,
                CreatedAt = request.CreatedAt,
                Description = request.Description,
                Date = request.Date,
                Location = request.Location,
                MaxParticipants = (int)request.MaxParticipants,
                Visibility = request.Visibility,
                Organizer = existingOrganizer,
                Activity = existingActivity
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();


            return Ok(
                new SuccessResponse<CreateEventResponse>(
                    new CreateEventResponse
                    {
                        Id = newEvent.Id,
                        Title = newEvent.Title,
                        OrganizerId = newEvent.OrganizerId,
                        ActivityId = newEvent.ActivityId,
                        CreatedAt = newEvent.CreatedAt,
                        Description = newEvent.Description,
                        Date = newEvent.Date,
                        Location = newEvent.Location,
                        MaxParticipants = newEvent.MaxParticipants,
                        Visibility = newEvent.Visibility
                    }
                )
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PutEventResponse updatedEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.Title = updatedEvent.Title;
            existingEvent.OrganizerId = updatedEvent.OrganizerId;
            existingEvent.ActivityId = updatedEvent.ActivityId;
            existingEvent.CreatedAt = updatedEvent.CreatedAt;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.Date = updatedEvent.Date;
            existingEvent.Location = updatedEvent.Location;
            existingEvent.MaxParticipants = (int)updatedEvent.MaxParticipants;
            existingEvent.Visibility = updatedEvent.Visibility;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var _event = await _context.Events.FindAsync(id);
            if (_event == null)
            {
                return NotFound();
            }

            _context.Events.Remove(_event);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
