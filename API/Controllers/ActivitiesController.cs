using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : Controller
    {
        private readonly niktopler_getTogetherContext _context;

        public ActivitiesController(niktopler_getTogetherContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Activities>> Get()
        {
            return Ok(_context.Activities);
        }

        [HttpPost]
        public async Task<ActionResult<Activities>> Create([FromBody] Activities activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = activity.ID }, activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Activities activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activity.ID)
            {
                return BadRequest();
            }

            _context.Entry(activity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

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
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.Activities.Any(e => e.ID == id);
        }

    }
}
