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
    [Authorize]
    [ApiController]
    public class KitchenIngredientController : ControllerBase
    {
        private readonly KitchenIngredientService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;

        public KitchenIngredientController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new KitchenIngredientService(context);
            _userManager = userManager;
        }


        #region GET Methods

        // GET: api/KitchenIngredient
        [HttpGet]
        public async Task<ActionResult<List<KitchenIngredientDto>>> GetIngredientsForKitchen(long kitchenId)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                List<KitchenIngredient> ingredientsInKitchen = _service.GetKitchenIngredients(kitchenId, user);

                return Ok(KitchenIngredientDto.ToList(ingredientsInKitchen));
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
        }

        // GET: api/KitchenIngredient/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KitchenIngredientDto>> GetKitchenIngredient(long id)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                KitchenIngredientDto kitchenIngredient = new KitchenIngredientDto(_service.GetKitchenIngredientById(id, user));
                return Ok(kitchenIngredient);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (IngredientNotFoundException e)
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

        #endregion

        // PUT: api/KitchenIngredient/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKitchenIngredient(long id, KitchenIngredient kitchenIngredient)
        {
            return null;
        }

        // POST: api/KitchenIngredient
        [HttpPost]
        public async Task<ActionResult<KitchenIngredient>> PostKitchenIngredient(KitchenIngredient kitchenIngredient)
        {
            return null;
        }

        // DELETE: api/KitchenIngredient/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KitchenIngredient>> DeleteKitchenIngredient(long id)
        {
            return null;
        }

        private bool KitchenIngredientExists(long id)
        {
            return true;
        }
    }
}
