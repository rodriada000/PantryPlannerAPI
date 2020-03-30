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
using PantryPlanner.Extensions;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Cookies,Bearer")]
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
        public async Task<ActionResult<List<KitchenUserDto>>> GetAllUsersForKitchen(long kitchenId)
        {
            PantryPlannerUser user;
            List<KitchenUserDto> kitchenUsers;

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
                kitchenUsers = KitchenUserDto.ToList(_service.GetAllUsersForKitchenById(kitchenId, user));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (KitchenNotFoundException e)
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

            if (kitchenUsers == null)
            {
                return NotFound("No user found for kitchen");
            }

            return kitchenUsers;
        }

        // GET: api/KitchenUser/Accepted
        [HttpGet]
        [Route("Accepted")]
        public async Task<ActionResult<List<KitchenUserDto>>> GetAcceptedUsersForKitchen(long kitchenId)
        {
            PantryPlannerUser user;
            List<KitchenUserDto> kitchenUsers;

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
                kitchenUsers = KitchenUserDto.ToList(_service.GetAcceptedUsersForKitchenById(kitchenId, user));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (KitchenNotFoundException e)
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

            if (kitchenUsers == null)
            {
                return NotFound("No users found for kitchen");
            }

            return kitchenUsers;
        }

        // GET: api/KitchenUser/Accepted
        [HttpGet]
        [Route("NotAccepted")]
        public async Task<ActionResult<List<KitchenUserDto>>> GetNotAcceptedUsersForKitchen(long kitchenId)
        {
            PantryPlannerUser user;
            List<KitchenUserDto> kitchenUsers;

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
                kitchenUsers = KitchenUserDto.ToList(_service.GetUsersThatHaveNotAcceptedInviteByKitchenId(kitchenId, user));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (KitchenNotFoundException e)
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

            if (kitchenUsers == null)
            {
                return NotFound("No users found for kitchen");
            }

            return kitchenUsers;
        }

        // GET: api/KitchenUser/Invite
        [HttpGet]
        [Route("Invite")]
        public async Task<ActionResult<List<KitchenUserDto>>> GetKitchenInvitesForLoggedInUser()
        {
            PantryPlannerUser user;
            List<KitchenUserDto> myInvites;

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
                myInvites = KitchenUserDto.ToList(_service.GetMyInvites(user));
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

            if (myInvites == null)
            {
                return NotFound("No invites found for user");
            }

            return myInvites;
        }



        // POST: api/KitchenUser/Invite
        [HttpPost]
        [Route("Invite")]
        public async Task<ActionResult> InviteUserToKitchen(string username, long kitchenId)
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
                _service.InviteUserToKitchenByUsername(username, kitchenId, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (KitchenNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionsException e)
            {
                return Unauthorized(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok();
        }

        // PUT: api/KitchenUser/Invite
        [HttpPut]
        [Route("Invite")]
        public async Task<ActionResult> AcceptKitchenInvite(long kitchenId)
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
                _service.AcceptInviteToKitchenByKitchenId(kitchenId, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (KitchenNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InviteNotFoundException e)
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
            
            return Ok();
        }

        // DELETE: api/KitchenUser/Invite
        [HttpDelete]
        [Route("Invite")]
        public async Task<ActionResult> DenyKitchenInvite(long kitchenId)
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
                _service.DenyInviteToKitchen(kitchenId, user);
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
            catch (InviteNotFoundException e)
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

            return Ok();
        }



        // DELETE: api/KitchenUser/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KitchenUserDto>> DeleteKitchenUserByKitchenUserId(long id)
        {
            PantryPlannerUser user;
            KitchenUser kitchenUser;

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
                kitchenUser = _service.OwnerDeleteKitchenUserByKitchenUserId(id, user);
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
                return StatusCode(StatusCodes.Status422UnprocessableEntity, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok(new KitchenUserDto(kitchenUser));
        }

        // DELETE: api/KitchenUser
        [HttpDelete]
        public async Task<ActionResult<KitchenUserDto>> DeleteLoggedInUserFromKitchen(long kitchenId)
        {
            PantryPlannerUser user;
            KitchenUser kitchenUser;

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
                kitchenUser = _service.DeleteMyselfFromKitchen(kitchenId, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (KitchenNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (KitchenUserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionsException e)
            {
                return Unauthorized(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return new KitchenUserDto(kitchenUser);
        }
    }
}
