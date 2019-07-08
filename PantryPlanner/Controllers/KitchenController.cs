using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class KitchenController : ControllerBase
    {
        private readonly KitchenService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;

        public KitchenController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new KitchenService(context);
            _userManager = userManager;
        }

        // GET: api/Kitchen
        [HttpGet]
        public async Task<ActionResult<List<KitchenDto>>> GetKitchenAsync()
        {
            var user = await _userManager.GetUserAsync(this.User);

            try
            {
                return _service.GetAllKitchensForUser(user);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        // GET: api/Kitchen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kitchen>> GetKitchenAsync(long id)
        {
            PantryPlannerUser user = await _userManager?.GetUserAsync(this.User); ;
            Kitchen kitchen = null;

            try
            {
                kitchen = _service.GetKitchenById(id, user);
            }
            catch (PermissionsException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

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

            PantryPlannerUser user = await _userManager?.GetUserAsync(this.User);

            try
            {
                await _service.UpdateKitchenAsync(kitchen, user);
            }
            catch (Exception e)
            {

                return new UnprocessableEntityResult();
            }


            return NoContent();
        }

        // POST: api/Kitchen
        [HttpPost]
        public async Task<ActionResult<Kitchen>> PostKitchenAsync(Kitchen kitchen)
        {
            PantryPlannerUser user = await _userManager?.GetUserAsync(this.User);

            try
            {
                _service.AddKitchen(kitchen, user);
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e.Message);
            }

            return CreatedAtAction("PostKitchen", new { id = kitchen.KitchenId }, kitchen);
        }

        // DELETE: api/Kitchen/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Kitchen>> DeleteKitchenAsync(long id)
        {
            PantryPlannerUser user = await _userManager?.GetUserAsync(this.User); ;

            try
            {
                Kitchen deletedKitchen = _service.DeleteKitchenById(id, user);

                if (deletedKitchen == null)
                {
                    return NotFound();
                }

                return deletedKitchen;
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e.Message);
            }
        }

    }
}
