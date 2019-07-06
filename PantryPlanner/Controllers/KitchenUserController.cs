using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenUserController : ControllerBase
    {
        private readonly PantryPlannerContext _context;

        public KitchenUserController(PantryPlannerContext context)
        {
            _context = context;
        }

        // GET: api/KitchenUser
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<KitchenUser>>> GetKitchenUser()
        {
            return await _context.KitchenUser.ToListAsync();
        }

        // GET: api/KitchenUser/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<KitchenUser>> GetKitchenUser(long id)
        {
            var kitchenUser = await _context.KitchenUser.FindAsync(id);

            if (kitchenUser == null)
            {
                return NotFound();
            }

            return kitchenUser;
        }

        // PUT: api/KitchenUser/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutKitchenUser(long id, KitchenUser kitchenUser)
        {
            if (id != kitchenUser.KitchenUserId)
            {
                return BadRequest();
            }

            _context.Entry(kitchenUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KitchenUserExists(id))
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

        // POST: api/KitchenUser
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<KitchenUser>> PostKitchenUser(KitchenUser kitchenUser)
        {
            _context.KitchenUser.Add(kitchenUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (KitchenUserExists(kitchenUser.KitchenUserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetKitchenUser", new { id = kitchenUser.KitchenUserId }, kitchenUser);
        }

        // DELETE: api/KitchenUser/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<KitchenUser>> DeleteKitchenUser(long id)
        {
            var kitchenUser = await _context.KitchenUser.FindAsync(id);
            if (kitchenUser == null)
            {
                return NotFound();
            }

            _context.KitchenUser.Remove(kitchenUser);
            await _context.SaveChangesAsync();

            return kitchenUser;
        }

        private bool KitchenUserExists(long id)
        {
            return _context.KitchenUser.Any(e => e.KitchenUserId == id);
        }
    }
}
