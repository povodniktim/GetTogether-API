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
        [FromQuery] string[]? activities = null,
        [FromQuery] int? maxParticipants = null,
        [FromQuery] string? visibility = null,
        [FromQuery] string? organizer = null,
        [FromQuery] string sortBy = "date",
        [FromQuery] string sortDirection = "asc")
        {
            try
            {
                if (dates == null)
                {
                    dates = Array.Empty<string>();
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
                    .Where(e => e.Date >= DateTime.Now && (e.MaxParticipants - (e.EventParticipants.Count + 1)) > 0)
                    .Select(e => new GetEventResponse
                    {
                        Id = e.Id,
                        OrganizerId = e.OrganizerId,
                        ActivityId = e.ActivityId,
                        Title = e.Title,
                        CreatedAt = e.CreatedAt,
                        Description = e.Description,
                        Date = e.Date,
                        Location = e.Location,
                        MaxParticipants = e.MaxParticipants,
                        PlacesLeft = (e.MaxParticipants - _context.EventParticipants.Count(ep => ep.EventId == e.Id)) - 1,
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
                        Participants = _context.EventParticipants
                            .Where(ep => ep.EventId == e.Id)
                            .Select(ep => new GetParticipantResponse
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

                if (activities != null && activities.Any())
                {
                    baseQuery = baseQuery.Where(e => activities.Contains(e.Activity.Name));
                }

                switch (sortBy.ToLower())
                {
                    case "date":
                        baseQuery = (sortDirection.ToLower() == "asc")
                            ? baseQuery.OrderBy(e => e.Date)
                            : baseQuery.OrderByDescending(e => e.Date);
                        break;
                    case "places-left":
                        baseQuery = (sortDirection.ToLower() == "asc")
                            ? baseQuery.OrderBy(e => e.PlacesLeft)
                            : baseQuery.OrderByDescending(e => e.PlacesLeft);
                        break;
                    case "date-created":
                        baseQuery = (sortDirection.ToLower() == "asc")
                            ? baseQuery.OrderBy(e => e.CreatedAt)
                            : baseQuery.OrderByDescending(e => e.CreatedAt);
                        break;
                    default:
                        break;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<GetEventRequest>> GetById(int id)
        {
            try
            {
                var eventEntity = await _context.Events
                    .Include(e => e.Organizer)
                    .Include(e => e.Activity)
                    .Include(e => e.EventParticipants)
                    .ThenInclude(ep => ep.Participant)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (eventEntity == null)
                {
                    return NotFound();
                }

                var getEventResponse = new GetEventResponse
                {
                    Id = eventEntity.Id,
                    OrganizerId = eventEntity.OrganizerId,
                    ActivityId = eventEntity.ActivityId,
                    Title = eventEntity.Title,
                    CreatedAt = eventEntity.CreatedAt,
                    Description = eventEntity.Description,
                    Date = eventEntity.Date,
                    Location = eventEntity.Location,
                    MaxParticipants = eventEntity.MaxParticipants,
                    PlacesLeft = (eventEntity.MaxParticipants - _context.EventParticipants.Count(ep => ep.EventId == eventEntity.Id)) - 1,
                    Visibility = eventEntity.Visibility ?? "public",
                    Organizer = new GetOrganizerResponse
                    {
                        Id = eventEntity.Organizer.Id,
                        FirstName = eventEntity.Organizer.FirstName,
                        LastName = eventEntity.Organizer.LastName,
                        Email = eventEntity.Organizer.Email,
                        CreatedAt = eventEntity.Organizer.CreatedAt,
                        ProfileImageUrl = eventEntity.Organizer.ProfileImageUrl
                    },
                    Activity = new GetActivityResponse
                    {
                        Id = eventEntity.Activity.Id,
                        Name = eventEntity.Activity.Name,
                        IconClassName = eventEntity.Activity.IconClassName
                    },
                    Participants = eventEntity.EventParticipants
                        .Select(ep => new GetParticipantResponse
                        {
                            Id = ep.ParticipantId,
                            FirstName = ep.Participant.FirstName,
                            LastName = ep.Participant.LastName,
                            ProfileImageUrl = ep.Participant.ProfileImageUrl
                        })
                        .ToList()
                };

                return Ok(new SuccessResponse<GetEventResponse>(getEventResponse, "Event details"));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorResponse<string>(new string[] { e.Message }, "Failed to get event details"));
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
        public async Task<IActionResult> Update(int id, [FromBody] PutEventRequest updatedEvent)
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
