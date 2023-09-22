using API.Models;
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
        public async Task<ActionResult<IEnumerable<Event>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10,
            [FromQuery] string? filter = null
        )
        {
            IQueryable<Event> query = _context.Events;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var filterLower = filter.ToLower();

                query = query.Where(e =>
                    e.Title.ToLower().Contains(filterLower) ||
                    e.Description.ToLower().Contains(filterLower) ||
                    e.Date.ToString().Contains(filterLower) ||
                    e.Location.ToLower().Contains(filterLower)
                );
            }

            int count = await query.CountAsync();

            int skip = (page - 1) * perPage;
            query = query.Skip(skip).Take(perPage);

            var events = query.ToList();

            return Ok(
                new SuccessResponse<GetMultipleResponse<Event>>(
                    new GetMultipleResponse<Event>
                    {
                        Count = count,
                        Page = page,
                        PerPage = perPage,
                        Collection = events
                    },
                    "List of all events"
                )
            );

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Event>> GetById([FromRoute] int id)
        {

            var _event = await _context.Events.FindAsync(id);

            if (_event == null)
                return NotFound();

            return Ok(_event);
        }

        [HttpPost]
        public async Task<ActionResult<Event>> Create([FromBody] Event _event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Events.Add(_event);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = _event.Id }, _event);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Event _event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != _event.Id)
            {
                return BadRequest();
            }

            _context.Entry(_event).State = EntityState.Modified;

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
