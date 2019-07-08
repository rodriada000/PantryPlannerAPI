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
    public class KitchenUserController : ControllerBase
    {
        private readonly KitchenUserService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;

        public KitchenUserController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new KitchenUserService(context);
            _userManager = userManager;
        }

        // GET: api/KitchenUser
        [HttpGet]
        public ActionResult<IEnumerable<KitchenUser>> GetKitchenUser()
        {
            return null;
            //return _service.GetUsersForKitchen()
        }

        // GET: api/KitchenUser/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KitchenUser>> GetKitchenUser(long id)
        {
            return await _service.Context.KitchenUser.FindAsync(id);
        }

        // GET: api/KitchenUser?kitchenId
        public async Task<ActionResult<List<KitchenUserDto>>> GetAllKitchenUsersForKitchen(long kitchenId)
        {
            PantryPlannerUser user = await _userManager.GetUserAsync(this.User);
            List<KitchenUserDto> kitchenUsers = null;

            try
            {
                kitchenUsers = _service.GetUsersForKitchenById(kitchenId, user);
            }
            catch (ArgumentNullException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (PermissionsException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }

            if (kitchenUsers == null)
            {
                return NotFound();
            }

            return kitchenUsers;
        }

        // PUT: api/KitchenUser/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKitchenUser(long id, KitchenUser kitchenUser)
        {
            if (id != kitchenUser.KitchenUserId)
            {
                return BadRequest();
            }

            PantryPlannerUser user = await _userManager.GetUserAsync(this.User);

            try
            {
                _service.UpdateKitchenUser(kitchenUser, user);
            }
            catch(ArgumentNullException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (PermissionsException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return StatusCode(StatusCodes.Status200OK);
        }

        // POST: api/KitchenUser
        //[HttpPost]
        //public async Task<ActionResult<KitchenUser>> PostKitchenUser(KitchenUser kitchenUser)
        //{
        //    PantryPlannerUser user = await _userManager.GetUserAsync(this.User);

        //    try
        //    {
        //        _ser
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (KitchenUserExists(kitchenUser.KitchenUserId))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetKitchenUser", new { id = kitchenUser.KitchenUserId }, kitchenUser);
        //}

        // DELETE: api/KitchenUser/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<KitchenUserDto>> DeleteKitchenUserByKitchenUserId(long id)
        {
            PantryPlannerUser user = await _userManager.GetUserAsync(this.User);
            KitchenUser kitchenUser = null;

            try
            {
                kitchenUser = _service.DeleteKitchenUserById(id, user);
            }
            catch (PermissionsException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (ArgumentNullException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return new KitchenUserDto(kitchenUser);
        }
    }
}
