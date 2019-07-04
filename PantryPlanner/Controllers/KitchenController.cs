using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenController : ControllerBase
    {
        private readonly PantryPlannerContext _context;

        public KitchenController(PantryPlannerContext context)
        {
            _context = context;
        }

        // GET: api/Kitchen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kitchen>>> GetKitchen()
        {
            return await _context.Kitchen.ToListAsync();
        }

        // GET: api/Kitchen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kitchen>> GetKitchen(long id)
        {
            var kitchen = await _context.Kitchen.FindAsync(id);

            if (kitchen == null)
            {
                return NotFound();
            }

            return kitchen;
        }

        // PUT: api/Kitchen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKitchen(long id, Kitchen kitchen)
        {
            if (id != kitchen.KitchenId)
            {
                return BadRequest();
            }

            _context.Entry(kitchen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KitchenExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Kitchen
        [HttpPost]
        public async Task<ActionResult<Kitchen>> PostKitchen(Kitchen kitchen)
        {
            _context.Kitchen.Add(kitchen);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (KitchenExists(kitchen.KitchenId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetKitchen", new { id = kitchen.KitchenId }, kitchen);
        }

        // DELETE: api/Kitchen/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Kitchen>> DeleteKitchen(long id)
        {
            var kitchen = await _context.Kitchen.FindAsync(id);
            if (kitchen == null)
            {
                return NotFound();
            }

            _context.Kitchen.Remove(kitchen);
            await _context.SaveChangesAsync();

            return kitchen;
        }

        private bool KitchenExists(long id)
        {
            return _context.Kitchen.Any(e => e.KitchenId == id);
        }
    }
}
