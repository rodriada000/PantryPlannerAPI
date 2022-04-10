using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Cookies,Bearer")]
    [ApiController]
    public class KitchenListController : ControllerBase
    {
        private readonly KitchenListService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;

        public KitchenListController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new KitchenListService(context, new KitchenService(context));
            _userManager = userManager;
        }

        // GET: api/KitchenList
        [HttpGet]
        public async Task<ActionResult<List<KitchenDto>>> GetKitchenListAsync()
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
                List<KitchenList> lists = await _service.GetAllKitchenListsForUser(user);
                return Ok(KitchenListDto.ToList(lists));
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

        // GET: api/KitchenList/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KitchenListDto>> GetKitchenListAsync(long id)
        {
            KitchenList kitchenList;
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
                kitchenList = _service.GetKitchenListById(id, user);
                return Ok(new KitchenListDto(kitchenList));

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

        // POST: api/KitchenList
        [HttpPost]
        public async Task<ActionResult<KitchenListDto>> AddNewKitchenList(KitchenList newList)
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
                await _service.AddKitchenListAsync(newList, user);
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


            return StatusCode(StatusCodes.Status201Created, new KitchenListDto(newList));
        }

        // DELETE: api/KitchenList/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KitchenListDto>> DeleteKitchenListAsync(long id)
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
                KitchenList deletedList = _service.DeleteKitchenList(id, user);

                if (deletedList == null)
                {
                    return NotFound();
                }

                return Ok(new KitchenListDto(deletedList));
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
