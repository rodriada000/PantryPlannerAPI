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
                return KitchenDto.ToList(_service.GetAllKitchensForUser(user));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        // GET: api/Kitchen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KitchenDto>> GetKitchenAsync(long id)
        {
            PantryPlannerUser user = await _userManager?.GetUserAsync(this.User);
            Kitchen kitchen = null;

            try
            {
                kitchen = _service.GetKitchenById(id, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (KitchenNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionsException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            if (kitchen == null)
            {
                return NotFound($"Kitchen with ID {id} not found");
            }

            return new KitchenDto(kitchen);
        }

        // PUT: api/Kitchen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKitchen(long id, Kitchen kitchen)
        {
            PantryPlannerUser user = await _userManager?.GetUserAsync(this.User);

            try
            {
                if (id != kitchen.KitchenId)
                {
                    return BadRequest();
                }

                await _service.UpdateKitchenAsync(kitchen, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (PermissionsException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }



            return NoContent();
        }

        // POST: api/Kitchen
        [HttpPost]
        public async Task<ActionResult<KitchenDto>> AddNewKitchen(Kitchen kitchen)
        {
            PantryPlannerUser user = await _userManager?.GetUserAsync(this.User);

            try
            {
                _service.AddKitchen(kitchen, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }


            return new KitchenDto(kitchen);
        }

        // DELETE: api/Kitchen/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KitchenDto>> DeleteKitchenAsync(long id)
        {
            PantryPlannerUser user = await _userManager?.GetUserAsync(this.User); ;

            try
            {
                Kitchen deletedKitchen = _service.DeleteKitchen(id, user);

                if (deletedKitchen == null)
                {
                    return NotFound();
                }

                return new KitchenDto(deletedKitchen);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (PermissionsException e)
            {
                return Unauthorized(e.Message);
            }
            catch (KitchenNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return UnprocessableEntity(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
