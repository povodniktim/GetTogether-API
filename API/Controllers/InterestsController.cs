using API.Models;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterestsController : Controller
    {
        private readonly GetTogetherContext _context;

        public InterestsController(GetTogetherContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Interest>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10,
            [FromQuery] string? filter = null
        )
        {

            if (page < 1)
            {
                page = 1;
            }

            var interests = await _context.Interest.ToListAsync();
            var count = interests.Count();

            return Ok(
                new SuccessResponse<GetMultipleResponse<Interest>>(
                    new GetMultipleResponse<Interest>
                    {
                        Count = count,
                        Page = page,
                        PerPage = perPage,
                        Collection = interests
                    },
                    "List of all interests"
                )
            );
        }
    }
}
