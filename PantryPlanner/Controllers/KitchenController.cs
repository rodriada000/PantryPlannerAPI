using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
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
            PantryPlannerUser user = null;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }



            try
            {
                List<Kitchen> kitchens = _service.GetAllKitchensForUser(user);
                return Ok(KitchenDto.ToList(kitchens));
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
            Kitchen kitchen;
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            try
            {
                kitchen = _service.GetKitchenById(id, user);
                return Ok(new KitchenDto(kitchen));

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
        }

        // PUT: api/Kitchen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKitchen(long id, Kitchen kitchen)
        {
            PantryPlannerUser user = null;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

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
            PantryPlannerUser user = null;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }


            try
            {
                await _service.AddKitchenAsync(kitchen, user);
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


            return StatusCode(StatusCodes.Status201Created, new KitchenDto(kitchen));
        }

        // DELETE: api/Kitchen/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KitchenDto>> DeleteKitchenAsync(long id)
        {
            PantryPlannerUser user = null;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }


            try
            {
                Kitchen deletedKitchen = _service.DeleteKitchen(id, user);

                if (deletedKitchen == null)
                {
                    return NotFound();
                }

                return Ok(new KitchenDto(deletedKitchen));
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
