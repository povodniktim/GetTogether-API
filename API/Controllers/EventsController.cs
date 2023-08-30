using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private readonly niktopler_getTogetherContext _context;

        public EventsController(niktopler_getTogetherContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Users>> Get()
        {
            return Ok(_context.Events);
        }

        [HttpPost]
        public async Task<ActionResult<Users>> Create([FromBody] Events _event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Events.Add(_event);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = _event.ID }, _event);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Events _event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != _event.ID)
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
            return _context.Events.Any(e => e.ID == id);
        }
    }
}
