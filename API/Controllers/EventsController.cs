using API.Models;
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
        public async Task<ActionResult<IEnumerable<Event>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string filter = "")
        {
            IQueryable<Event> query = _context.Events;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(e =>
                    e.Title.Contains(filter) ||
                    e.Description.Contains(filter) ||
                    e.Date.ToString().Contains(filter) ||
                    e.Location.Contains(filter)
                );
            }

            int totalItems = await query.CountAsync();

            int skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            var events = await query.ToListAsync();

            var response = new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = events
            };

            return Ok(response);
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
