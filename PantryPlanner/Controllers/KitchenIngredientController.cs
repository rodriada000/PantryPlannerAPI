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

        // GET: api/KitchenIngredient/ByName
        [HttpGet("ByName")]
        public async Task<ActionResult<List<KitchenIngredientDto>>> GetIngredientsForKitchenByName(long kitchenId, string ingredientName)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                List<KitchenIngredient> ingredientsInKitchen = _service.GetKitchenIngredientsByName(kitchenId, ingredientName, user);

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

        // GET: api/KitchenIngredient/ByCategory
        [HttpGet("ByCategory")]
        public async Task<ActionResult<List<KitchenIngredientDto>>> GetIngredientsForKitchenByCategory(long kitchenId, long categoryId)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                List<KitchenIngredient> ingredientsInKitchen = _service.GetKitchenIngredientsByCategory(kitchenId, categoryId, user);

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
            catch (CategoryNotFoundException e)
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

        // GET: api/KitchenIngredient/ByCategoryName
        [HttpGet("ByCategoryName")]
        public async Task<ActionResult<List<KitchenIngredientDto>>> GetIngredientsForKitchenByCategoryName(long kitchenId, string categoryName)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                List<KitchenIngredient> ingredientsInKitchen = _service.GetKitchenIngredientsByCategoryName(kitchenId, categoryName, user);

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
            catch (CategoryNotFoundException e)
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


        #region POST & PUT Methods (Add Ingredients to Kitchen; Update Ingredients in Kitchen)

        // POST: api/KitchenIngredient
        [HttpPost]
        public async Task<ActionResult<KitchenIngredientDto>> AddKitchenIngredientAsync([FromBody] KitchenIngredientDto kitchenIngredient)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                KitchenIngredient kitchenIngredientToAdd = KitchenIngredientDto.Create(kitchenIngredient);

                KitchenIngredient newIngredient = _service.AddKitchenIngredient(kitchenIngredientToAdd, user);
                return Ok(new KitchenIngredientDto(newIngredient));
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

        // POST: api/KitchenIngredient
        [HttpPost("Id")]
        public async Task<ActionResult<KitchenIngredientDto>> AddIngredientToKitchenAsync(long kitchenId, long ingredientId)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                KitchenIngredient newIngredient = _service.AddIngredientToKitchen(ingredientId, kitchenId, user);
                return Ok(new KitchenIngredientDto(newIngredient));
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
        public async Task<ActionResult> UpdateKitchenIngredientAsync(long id, [FromBody] KitchenIngredientDto kitchenIngredient)
        {
            PantryPlannerUser user;

            try
            {
                if (id != kitchenIngredient?.KitchenIngredientId)
                {
                    return BadRequest("The ID specified does not match");
                }

                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);


                await _service.UpdateKitchenIngredientAsync(kitchenIngredient, user);
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

                KitchenIngredient deletedIngredient = _service.DeleteKitchenIngredient(id, user);
                return Ok(new KitchenIngredientDto(deletedIngredient));
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
