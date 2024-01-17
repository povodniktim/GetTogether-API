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
        public async Task<ActionResult<IEnumerable<GetEventRequest>>> Get
        (
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
            [FromQuery] string sortDirection = "asc"
        )
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

                return Ok
                (
                    new SuccessResponse<GetMultipleResponse<GetEventResponse>>
                    (
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
                return BadRequest
                (
                    new ErrorResponse<string>
                    (
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
                    return NotFound
                    (
                        new ErrorResponse<string>
                        (
                            new string[] { $"Event with ID {id} not found" },
                            "Invalid event ID"
                        )
                    );
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

                return Ok
                (
                    new SuccessResponse<GetEventResponse>
                    (
                        getEventResponse,
                        "Event details"
                    )
                );
            }
            catch (Exception e)
            {
                return BadRequest
                (
                    new ErrorResponse<string>
                    (
                        new string[] { e.Message },
                        "Failed to get event details"
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

            if (existingOrganizer == null)
            {
                return NotFound
                (
                    new ErrorResponse<string>
                    (
                        new string[] { $"User with ID {request.OrganizerId} not found" },
                        "Invalid user ID"
                    )
                );
            }

            var existingActivity = await _context.Activities
                .Include(a => a.Events)
                .FirstOrDefaultAsync(a => a.Id == request.ActivityId);

            if (existingActivity == null)
            {
                return NotFound
                (
                    new ErrorResponse<string>
                    (
                        new string[] { $"Activity with ID {request.ActivityId} not found" },
                        "Invalid activity ID"
                    )
                );
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Conflict
                (
                    new ErrorResponse<string>
                    (
                        new string[] { e.Message },
                        "Failed to create the event. Please check your request and try again."
                    )
                );
            }


            return Ok
            (
                new SuccessResponse<CreateEventResponse>
                (
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

        [HttpPost("{id}/join")]
        public async Task<ActionResult> JoinEvent(int id, [FromBody] JoinEventRequest request)
        {
            try
            {
                var eventToJoin = await _context.Events
                    .Include(e => e.EventParticipants)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (eventToJoin == null)
                {
                    return NotFound(
                        new ErrorResponse<string>(
                            new string[] { $"Event with ID {id} not found" },
                            "Invalid event ID"
                        )
                    );
                }

                var doesUserExist = await _context.Users.AnyAsync(u => u.Id == request.UserId);

                if (!doesUserExist)
                {
                    return NotFound(
                        new ErrorResponse<string>(
                            new string[] { $"User with ID {request.UserId} not found" },
                            "Invalid user ID"
                        )
                    );
                }

                var isAlreadyParticipant = eventToJoin.EventParticipants
                    .Any(ep => ep.ParticipantId == request.UserId);

                if (isAlreadyParticipant)
                {
                    return Conflict(
                        new ErrorResponse<string>(
                            new string[] { "You are already a participant in this event" },
                            "Duplicate participant registration"
                        )
                    );
                }

                var eventParticipant = new EventParticipant
                {
                    ParticipantId = request.UserId,
                    EventId = id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                eventToJoin.EventParticipants.Add(eventParticipant);

                await _context.SaveChangesAsync();

                // --- Notifications ---
                var organizerId = eventToJoin.OrganizerId;

                if (organizerId != request.UserId)
                {
                    var notification = new Notification
                    {
                        OrganizerId = organizerId,
                        EventId = id,
                        ParticipantId = request.UserId,
                        CreatedAt = DateTime.UtcNow,
                        Status = "joined"
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                return Ok(
                    new SuccessResponse<JoinEventResponse>(
                         new JoinEventResponse
                         {
                             Id = eventToJoin.Id,
                             Title = eventToJoin.Title,
                             OrganizerId = eventToJoin.OrganizerId,
                             ActivityId = eventToJoin.ActivityId,
                             CreatedAt = eventToJoin.CreatedAt,
                             Description = eventToJoin.Description,
                             Date = eventToJoin.Date,
                             Location = eventToJoin.Location,
                             MaxParticipants = eventToJoin.MaxParticipants,
                             Visibility = eventToJoin.Visibility
                         },
                        "Successfully joined the event"
                    )
                );
            }
            catch (Exception e)
            {
                return BadRequest(
                    new ErrorResponse<string>(
                        new string[] { e.Message },
                        "Failed to join the event"
                    )
                );
            }
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
                return NotFound
                (
                    new ErrorResponse<string>
                    (
                        new string[] { $"Event with ID {id} not found" },
                        "Invalid event ID"
                    )
                );
            }

            existingEvent.Title = updatedEvent.Title;
            existingEvent.OrganizerId = updatedEvent.OrganizerId;
            existingEvent.ActivityId = updatedEvent.ActivityId;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.Date = updatedEvent.Date;
            existingEvent.Location = updatedEvent.Location;
            existingEvent.MaxParticipants = (int)updatedEvent.MaxParticipants;
            existingEvent.Visibility = updatedEvent.Visibility ?? "public";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Conflict
                (
                    new ErrorResponse<string>
                    (
                        new string[] { e.Message },
                        "Failed to update the event. Please check your request and try again."
                    )
                );
            }

            return Ok
            (
                new SuccessResponse<string>
                (
                    "Event updated successfully"
                )
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var _event = await _context.Events
                .Include(e => e.EventParticipants)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (_event == null)
            {
                return NotFound
                (
                    new ErrorResponse<string>
                    (
                        new string[] { $"Event with ID {id} not found" },
                        "Invalid event ID"
                    )
                );
            }

            if (_event.EventParticipants.Any())
            {
                foreach (var participant in _event.EventParticipants)
                {
                    _context.EventParticipants.Remove(participant);
                }
            }

            _context.Events.Remove(_event);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Conflict
                (
                    new ErrorResponse<string>
                    (
                        new string[] { e.Message },
                        "Failed to delete the event. Please check your request and try again."
                    )
                );
            }

            return Ok
            (
                new SuccessResponse<string>
                (
                    "Event deleted successfully"
                )
            );
        }

        [HttpDelete("{id}/leave")]
        public async Task<ActionResult> LeaveEvent(int id, [FromBody] LeaveEventRequest request)
        {
            try
            {
                var eventToLeave = await _context.Events
                    .Include(e => e.EventParticipants)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (eventToLeave == null)
                {
                    return NotFound
                    (
                        new ErrorResponse<string>
                        (
                            new string[] { $"Event with ID {id} not found" },
                            "Invalid event ID"
                        )
                    );
                }

                var doesUserExist = await _context.Users.AnyAsync(u => u.Id == request.UserId);

                if (!doesUserExist)
                {
                    return NotFound
                    (
                        new ErrorResponse<string>
                        (
                            new string[] { $"User with ID {request.UserId} not found" },
                            "Invalid user ID"
                        )
                    );
                }

                var eventParticipant = eventToLeave.EventParticipants
                    .FirstOrDefault(ep => ep.ParticipantId == request.UserId);

                if (eventParticipant == null)
                {
                    return NotFound
                    (
                        new ErrorResponse<string>
                        (
                            new string[] { "You are not a participant in this event" },
                            "The specified participant was not found in the event"
                        )
                    );
                }

                eventToLeave.EventParticipants.Remove(eventParticipant);

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    // --- Notifications ---
                    var organizerId = eventToLeave.OrganizerId;

                    if (organizerId != request.UserId)
                    {
                        var notification = new Notification
                        {
                            OrganizerId = organizerId,
                            EventId = id,
                            ParticipantId = request.UserId,
                            CreatedAt = DateTime.UtcNow,
                            Status = "left"
                        };

                        _context.Notifications.Add(notification);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }

                return Ok
                (
                    new SuccessResponse<string>
                    (
                        "Successfully left the event"
                    )
                );
            }
            catch (Exception e)
            {
                return BadRequest
                (
                    new ErrorResponse<string>
                    (
                        new string[] { e.Message },
                        "Failed to leave the event"
                    )
                );
            }
        }
    }
}
