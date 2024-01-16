using API.Models.Requests.Notifications;
using API.Models.Response.User;
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
        public async Task<ActionResult<IEnumerable<GetNotificationRequest>>> Get
        (
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10
        )
        {
            try
            {
                if (page < 1)
                {
                    page = 1;
                }

                IQueryable<GetNotificationResponse> query = _context.Notifications
                    .Include(n => n.Participant)
                    .Select(n => new GetNotificationResponse
                    {
                        UserId = n.UserId,
                        EventId = n.EventId,
                        ParticipantId = n.ParticipantId,
                        Status = n.Status,
                        User = new GetUserResponse
                        {
                            Id = n.User.Id,
                            FirstName = n.User.FirstName,
                            LastName = n.User.LastName,
                            Email = n.User.Email,
                            CreatedAt = n.User.CreatedAt,
                            ProfileImageUrl = n.User.ProfileImageUrl
                        },
                        Participant = new GetUserResponse
                        {
                            Id = n.Participant.Id,
                            FirstName = n.Participant.Participant.FirstName
                        }
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
