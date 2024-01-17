using API.Models.Requests.Notifications;
using API.Models.Response.User;
using API.Models.Responses.Activity;
using API.Models.Responses.Event;
using API.Models.Responses.Notifications;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : Controller
    {
        private readonly GetTogetherContext _context;

        public NotificationsController(GetTogetherContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<ActionResult<IEnumerable<GetNotificationRequest>>> Get
        (
            [FromRoute] int userId,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10
        )
        {
            try
            {
                var query = _context.Notifications
                     .Include(p => p.Participant)
                     .Include(o => o.Organizer)
                     .Where(n => (n.OrganizerId == userId && n.Status == "joined") || (n.ParticipantId == userId && n.Status == "updated") || (n.ParticipantId == userId && n.Status == "left"))
                     .Select(n => new GetNotificationResponse
                     {
                         OrganizerId = n.OrganizerId,
                         EventId = n.EventId,
                         ParticipantId = n.ParticipantId,
                         Status = n.Status,
                         CreatedAt = n.CreatedAt,
                         Organizer = _context.Users
                        .Where(o => o.Id == n.OrganizerId)
                        .Select(o => new GetUserResponse
                        {
                            Id = o.Id,
                            FirstName = o.FirstName,
                            LastName = o.LastName,
                            Email = o.Email,
                            CreatedAt = o.CreatedAt,
                            ProfileImageUrl = o.ProfileImageUrl
                        })
                        .FirstOrDefault() ?? new GetUserResponse(),

                         Participant = _context.Users
                        .Where(u => u.Id == n.ParticipantId)
                        .Select(u => new GetUserResponse
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            CreatedAt = u.CreatedAt,
                            ProfileImageUrl = u.ProfileImageUrl
                        })
                        .FirstOrDefault() ?? new GetUserResponse(),

                         Event = _context.Events
                        .Where(e => e.Id == n.EventId)
                        .Select(e => new GetEventSimpleResponse
                        {
                            Id = e.Id,
                            Title = e.Title,
                            Activity = _context.Activities
                            .Where(a => a.Id == e.ActivityId)
                            .Select(a => new GetActivityResponse
                            {
                                Id = a.Id,
                                Name = a.Name,
                                IconClassName = a.IconClassName
                            })
                            .FirstOrDefault() ?? new GetActivityResponse()
                        })
                        .FirstOrDefault() ?? new GetEventSimpleResponse()
                     });

                var notifications = await query
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .ToListAsync();

                return Ok
                (
                    new SuccessResponse<GetMultipleResponse<GetNotificationResponse>>
                    (
                        new GetMultipleResponse<GetNotificationResponse>
                        {
                            Count = await _context.Notifications.CountAsync(),
                            Page = page,
                            PerPage = perPage,
                            Collection = notifications
                        },
                        "List of Notifications"
                    )
                );
            }
            catch (Exception e)
            {
                return BadRequest($"Failed to fetch notifications: {e.Message}");
            }
        }
    }
}
