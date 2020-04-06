using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Models;
using PantryPlanner.Services;
using PantryPlanner.Migrations;
using PantryPlanner.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Cookies,Bearer")]
    [ApiController]
    public class RecipeIngredientController : ControllerBase
    {
        private readonly RecipeIngredientService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;


        public RecipeIngredientController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new RecipeIngredientService(context);
            _userManager = userManager;
        }

        // GET: api/RecipeIngredient
        [HttpGet]
        public async Task<ActionResult<List<RecipeIngredientDto>>> GetIngredientsForRecipeAsync(long recipeId)
        {
            List<RecipeIngredientDto> ingredients = null;

            try
            {
                PantryPlannerUser user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                await Task.Run(() =>
                {
                    ingredients = RecipeIngredientDto.ToList(_service.GetIngredientsForRecipe(recipeId, user));
                }).ConfigureAwait(false);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok(ingredients);
        }


        // GET: api/RecipeIngredient/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeIngredientDto>> GetRecipeIngredientByIdAsync(long id)
        {
            PantryPlannerUser user;
            RecipeIngredient ingredient;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                ingredient = _service.GetRecipeIngredientById(id, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecipeNotFoundException e)
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

            return Ok(new RecipeIngredientDto(ingredient));
        }

        // PUT: api/RecipeIngredient/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecipeIngredientAsync(long id, [FromBody] RecipeIngredientDto ingredient)
        {
            if (id != ingredient.RecipeIngredientId)
            {
                return BadRequest($"id {id} does not match the recipe ingredient to update");
            }


            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                await _service.UpdateRecipeIngredientAsync(ingredient, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecipeNotFoundException e)
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

        // POST: api/RecipeIngredient
        [HttpPost]
        public async Task<ActionResult<RecipeIngredientDto>> AddRecipeIngredientAsync([FromBody] RecipeIngredientDto ingredient)
        {
            PantryPlannerUser user;
            RecipeIngredientDto addedIngredient;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                addedIngredient = new RecipeIngredientDto(_service.AddRecipeIngredient(ingredient, user));
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
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return StatusCode(StatusCodes.Status201Created, addedIngredient);
        }

        // DELETE: api/RecipeIngredient/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecipeIngredientDto>> DeleteRecipeIngredientAsync(long id)
        {
            PantryPlannerUser user;
            RecipeIngredient deletedIngredient;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                deletedIngredient = _service.DeleteRecipeIngredient(id, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecipeNotFoundException e)
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

            return Ok(new RecipeIngredientDto(deletedIngredient));
        }

    }
}
