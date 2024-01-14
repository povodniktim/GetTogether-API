using API.Models.Responses.Activity;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : Controller
    {
        private readonly GetTogetherContext _context;

        public ActivitiesController(GetTogetherContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetActivityResponse>>> Get
        (
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10
        )
        {
            IQueryable<GetActivityResponse> query = _context.Activities
                .Select(a => new GetActivityResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    IconClassName = a.IconClassName
                });

            int skip = (page - 1) * perPage;

            var activities = await query
                .Skip(skip)
                .Take(perPage)
                .ToListAsync();

            return Ok
            (
                new SuccessResponse<GetMultipleResponse<GetActivityResponse>>
                (
                    new GetMultipleResponse<GetActivityResponse>
                    {
                        Count = await _context.Activities.CountAsync(),
                        Collection = activities,
                        Page = page,
                        PerPage = perPage
                    },
                    "List of all activities"
                )
            );
        }
    }
}
