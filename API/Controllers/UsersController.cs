using API.Models;
using API.Models.Requests.Activity;
using API.Models.Requests.User;
using API.Models.Response.User;
using API.Models.Responses.Activity;
using API.Models.Responses.Event;
using API.Models.Responses.User;
using API.Responses;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly GetTogetherContext _context;
        private readonly UserService _userService;

        public UsersController(GetTogetherContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserRequest>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10,
            [FromQuery] string? filter = null)
        {
            if (page < 1)
            {
                page = 1;
            }

            IQueryable<GetUserResponse> query = _context.Users
                .Where(u => string.IsNullOrWhiteSpace(filter) ||
                            u.FirstName.ToLower().Contains(filter.ToLower()) ||
                            u.LastName.ToLower().Contains(filter.ToLower()) ||
                            u.Email.ToLower().Contains(filter.ToLower()))
                .OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(u => new GetUserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                    ProfileImageUrl = u.ProfileImageUrl,
                    CreatedEventsCount = _context.Events.Count(e => e.OrganizerId == u.Id),
                    AttendedEventsCount = _context.EventParticipants.Count(ep => ep.ParticipantId == u.Id)
                });

            int count = await _context.Users.CountAsync();

            var users = await query.ToListAsync();

            return Ok(
                new SuccessResponse<GetMultipleResponse<GetUserResponse>>(
                    new GetMultipleResponse<GetUserResponse>
                    {
                        Count = count,
                        Page = page,
                        PerPage = perPage,
                        Collection = users
                    },
                    "List of all users"
                )
            );
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<GetOrganizerRequest>> GetById([FromRoute] int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new GetOrganizerResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                    ProfileImageUrl = u.ProfileImageUrl
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(
                    new ErrorResponse<string>(
                        new string[] { "User not found" },
                        "Invalid user data"
                    )
                );
            }

            return Ok(
                new SuccessResponse<GetOrganizerResponse>(
                    user,
                    "User with id=" + user.Id
                )
            );
        }


        [HttpGet]
        [Route("{id}/events")]
        public async Task<ActionResult<IEnumerable<GetEventResponse>>> GetUserEvents(
            [FromRoute] int id,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {

            if (page < 1)
            {
                page = 1;
            }

            try
            {

                IQueryable<GetEventResponse> baseQuery = _context.Events
                    .Include(e => e.Organizer)
                    .Where(e => e.Date >= DateTime.Now && e.Organizer.Id == id)
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
                        "List of events created by the user"
                    )
                );

            }
            catch (Exception e)
            {
                return BadRequest(
                   new ErrorResponse<string>(
                       new string[] { e.Message },
                        "Failed to get events for user with id=" + id
                   )
               );
            }

        }

        [HttpGet]
        [Route("{id}/interests")]
        public async Task<ActionResult<IEnumerable<GetActivityRequest>>> GetUserInterests(int id)
        {
            try
            {
                var userInterests = await _context.UserActivities
                   .Where(ua => ua.UserId == id)
                   .Select(ua => new GetActivityResponse
                   {
                       Id = ua.Activity.Id,
                       Name = ua.Activity.Name,
                       IconClassName = ua.Activity.IconClassName
                   })
                   .ToListAsync();

                return Ok(
                    new SuccessResponse<IEnumerable<GetActivityResponse>>(
                        userInterests,
                        "User interests"
                    )
                );
            }
            catch (Exception e)
            {
                return BadRequest(
                   new ErrorResponse<string>(
                       new string[] { e.Message },
                        "Failed to get interests for user with id=" + id
                   )
               );
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(
                    new ErrorResponse<string>(
                        new string[] { "User not found" },
                        "Invalid user data"
                    )
                );
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(
                new SuccessResponse<User>(
                    user,
                    "User with id=" + user.Id + " has been deleted"
                )
            );
        }


    }
}