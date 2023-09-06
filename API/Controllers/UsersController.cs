using API.Models;
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
        public async Task<ActionResult<User>> Get()
        {
            return Ok(
                new SuccessResponse<GetMultipleResponse<User>>(
                    new GetMultipleResponse<User>
                    {
                        Count = await _context.Users.CountAsync(),
                        Collection = await _context.Users.ToListAsync()
                    },
                    "List of all users"
                )
            );
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ErrorResponse<object>(
                        ModelState.Values.ToArray(),
                        "Invalid user data"
                    )
                );
            }

            _userService.Create(user);

            return CreatedAtAction(
                nameof(Get),
                new { id = user.Id },
                new SuccessResponse<User>(
                    user,
                    "User created successfully"
                )
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ErrorResponse<object>(
                        ModelState.Values.ToArray(),
                        "Invalid user data"
                    )
                );
            }

            if (id != user.Id)
            {
                return BadRequest(
                    new ErrorResponse<string>(
                        new string[] { "Invalid ID" },
                        "Invalid user data"
                    )
                );
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(
                        new ErrorResponse<string>(
                            new string[] { "User does not exist" },
                            "Invalid user data"
                        )
                    );
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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