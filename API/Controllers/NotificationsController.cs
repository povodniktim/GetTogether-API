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
                     .Include(u => u.User)
                     .Where(n => (n.UserId == userId && n.Status == "joined") || (n.ParticipantId == userId && n.Status == "updated"))
                     .Select(n => new GetNotificationResponse
                     {
                        UserId = n.UserId,
                        EventId = n.EventId,
                        ParticipantId = n.ParticipantId,
                        Status = n.Status,
                        User = _context.Users.Where(u => u.Id == n.UserId).Select(u => new GetUserResponse
                        {
                            Id = u.Id,
                            FirstName = u.FirstName
                        }).FirstOrDefault(),
                        Participant = _context.Users.Where(u => u.Id == n.ParticipantId).Select(u => new GetUserResponse
                        {
                            Id = u.Id,
                            FirstName = u.FirstName
                        }).FirstOrDefault()
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
