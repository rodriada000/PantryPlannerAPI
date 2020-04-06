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
    public class RecipeStepController : ControllerBase
    {
        private readonly RecipeStepService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;


        public RecipeStepController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new RecipeStepService(context);
            _userManager = userManager;
        }

        // GET: api/RecipeStep
        [HttpGet]
        public async Task<ActionResult<List<RecipeStepDto>>> GetStepsForRecipeAsync(long recipeId)
        {
            List<RecipeStepDto> steps = null;

            try
            {
                PantryPlannerUser user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                await Task.Run(() =>
                {
                    steps = RecipeStepDto.ToList(_service.GetStepsForRecipe(recipeId, user));
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

            return Ok(steps);
        }

        // GET: api/RecipeStep/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeStepDto>> GetRecipeStepByIdAsync(long id)
        {
            PantryPlannerUser user;
            RecipeStep step;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                step = _service.GetRecipeStepById(id, user);
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

            return Ok(new RecipeStepDto(step));
        }

        // PUT: api/RecipeStep/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecipeStepAsync(long id, [FromBody] RecipeStepDto updatedStep)
        {
            if (id != updatedStep.RecipeStepId)
            {
                return BadRequest($"id {id} does not match the recipe ingredient to update");
            }


            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                await _service.UpdateRecipeStepAsync(updatedStep, user);
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

        // POST: api/RecipeStep
        [HttpPost]
        public async Task<ActionResult<RecipeStepDto>> AddRecipeStepAsync([FromBody] RecipeStepDto newStep)
        {
            PantryPlannerUser user;
            RecipeStepDto addedStep;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                addedStep = new RecipeStepDto(_service.AddRecipeStep(newStep, user));
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

            return StatusCode(StatusCodes.Status201Created, addedStep);
        }

        // DELETE: api/RecipeStep/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecipeStepDto>> DeleteRecipeStepAsync(long id)
        {
            PantryPlannerUser user;
            RecipeStep deletedStep;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                deletedStep = _service.DeleteRecipeStep(id, user);
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

            return Ok(new RecipeStepDto(deletedStep));
        }

    }
}
