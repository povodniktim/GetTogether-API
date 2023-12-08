using API.Models;
using API.Models.Requests.User;
using API.Models.Response.User;
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
                    ProfileImageUrl = u.ProfileImageUrl
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
        public async Task<ActionResult<GetUserResponse>> GetById([FromRoute] int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new GetUserResponse
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
                new SuccessResponse<GetUserResponse>(
                    user,
                    "User with id=" + user.Id
                )
            );
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

        private bool UserExists(int id)
        {
            return _context.Users.Any(u => u.Id == id);
        }
    }
}