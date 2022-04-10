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
    public class ListIngredientController : ControllerBase
    {
        private readonly ListIngredientService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;

        public ListIngredientController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new ListIngredientService(context);
            _userManager = userManager;
        }


        #region GET Methods

        // GET: api/ListIngredient
        [HttpGet]
        public async Task<ActionResult<List<ListIngredientDto>>> GetIngredientsForKitchenList(long kitchenListId)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                List<KitchenListIngredient> ingredientsInKitchen = _service.GetKitchenListIngredients(kitchenListId, user);

                return Ok(ListIngredientDto.ToList(ingredientsInKitchen));
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


        // GET: api/ListIngredient/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ListIngredientDto>> GetKitchenListIngredient(long id)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                ListIngredientDto kitchenIngredient = new ListIngredientDto(_service.GetKitchenListIngredientById(id, user));
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


        #region POST & PUT Methods (Add Ingredients to Kitchen List; Update Ingredients in Kitchen List)

        // POST: api/ListIngredient
        [HttpPost]
        public async Task<ActionResult<ListIngredientDto>> AddKitchenIngredientAsync([FromBody] ListIngredientDto kitchenIngredient)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                KitchenListIngredient newIngredient = _service.AddKitchenListIngredient(kitchenIngredient, user);
                return Ok(new ListIngredientDto(newIngredient));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message);
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

        // POST: api/ListIngredient
        [HttpPost("Id")]
        public async Task<ActionResult<ListIngredientDto>> AddIngredientToKitchenListAsync(long kitchenListId, long ingredientId)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                KitchenListIngredient newIngredient = _service.AddIngredientToKitchenList(ingredientId, kitchenListId, user);
                return Ok(new ListIngredientDto(newIngredient));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (KitchenNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (IngredientNotFoundException e)
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
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }

        // PUT: api/KitchenIngredient/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateKitchenIngredientAsync(long id, [FromBody] ListIngredientDto listIngredient)
        {
            PantryPlannerUser user;

            try
            {
                if (id != listIngredient?.Id)
                {
                    return BadRequest("The ID specified does not match");
                }

                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);


                await _service.UpdateKitchenListIngredientAsync(listIngredient, user);
                return Ok();
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


        #region DELETE Methods

        // DELETE: api/KitchenIngredient/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KitchenIngredient>> DeleteKitchenIngredientAsync(long id)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                KitchenListIngredient deletedIngredient = _service.DeleteKitchenListIngredient(id, user);
                return Ok(new ListIngredientDto(deletedIngredient));
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





    }
}
