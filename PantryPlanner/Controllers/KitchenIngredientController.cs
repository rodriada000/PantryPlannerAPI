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
    public class KitchenIngredientController : ControllerBase
    {
        private readonly PantryPlannerContext _context;

        public KitchenIngredientController(PantryPlannerContext context)
        {
            _context = context;
        }

        // GET: api/KitchenIngredient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KitchenIngredient>>> GetKitchenIngredient()
        {
            return await _context.KitchenIngredient.ToListAsync();
        }

        // GET: api/KitchenIngredient/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KitchenIngredient>> GetKitchenIngredient(long id)
        {
            var kitchenIngredient = await _context.KitchenIngredient.FindAsync(id);

            if (kitchenIngredient == null)
            {
                return NotFound();
            }

            return kitchenIngredient;
        }

        // PUT: api/KitchenIngredient/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKitchenIngredient(long id, KitchenIngredient kitchenIngredient)
        {
            if (id != kitchenIngredient.KitchenIngredientId)
            {
                return BadRequest();
            }

            _context.Entry(kitchenIngredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KitchenIngredientExists(id))
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

        // POST: api/KitchenIngredient
        [HttpPost]
        public async Task<ActionResult<KitchenIngredient>> PostKitchenIngredient(KitchenIngredient kitchenIngredient)
        {
            _context.KitchenIngredient.Add(kitchenIngredient);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (KitchenIngredientExists(kitchenIngredient.KitchenIngredientId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetKitchenIngredient", new { id = kitchenIngredient.KitchenIngredientId }, kitchenIngredient);
        }

        // DELETE: api/KitchenIngredient/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KitchenIngredient>> DeleteKitchenIngredient(long id)
        {
            var kitchenIngredient = await _context.KitchenIngredient.FindAsync(id);
            if (kitchenIngredient == null)
            {
                return NotFound();
            }

            _context.KitchenIngredient.Remove(kitchenIngredient);
            await _context.SaveChangesAsync();

            return kitchenIngredient;
        }

        private bool KitchenIngredientExists(long id)
        {
            return _context.KitchenIngredient.Any(e => e.KitchenIngredientId == id);
        }
    }
}
