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

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IngredientService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;


        public IngredientController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new IngredientService(context);
            _userManager = userManager;
        }

        // GET: api/Ingredient
        [HttpGet]
        public async Task<ActionResult<List<IngredientDto>>> GetIngredientByName(string name)
        {
            PantryPlannerUser user = await _userManager.GetUserAsync(this.User);
            List<IngredientDto> ingredients = null;

            try
            {
                ingredients = IngredientDto.ToList(_service.GetIngredientByName(name));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return ingredients;
        }

        // GET: api/Ingredient
        //[HttpGet]
        //public async Task<ActionResult<List<IngredientDto>>> GetIngredientByNameAndCategory(string name, string categoryName)
        //{
        //    PantryPlannerUser user = await _userManager.GetUserAsync(this.User);
        //    List<IngredientDto> ingredients = null;

        //    try
        //    {
        //        ingredients = IngredientDto.ToList(_service.GetIngredientByNameAndCategory(name, categoryName));
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //    catch (CategoryNotFoundException e)
        //    {
        //        return NotFound(e.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        //    }

        //    return ingredients;
        //}

        // GET: api/Ingredient/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientDto>> GetIngredient(long id)
        {
            PantryPlannerUser user = await _userManager.GetUserAsync(this.User);
            Ingredient ingredient = null;

            try
            {
                ingredient = _service.GetIngredientById(id, user);
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

            return new IngredientDto(ingredient);
        }

        // PUT: api/Ingredient/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(long id, Ingredient ingredient)
        {
            PantryPlannerUser user = await _userManager.GetUserAsync(this.User);

            try
            {
                _service.UpdateIngredient(ingredient, user);
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

        // POST: api/Ingredient
        [HttpPost]
        public async Task<ActionResult<IngredientDto>> AddIngredient(Ingredient ingredient)
        {
            PantryPlannerUser user = await _userManager.GetUserAsync(this.User);

            try
            {
                _service.AddIngredient(ingredient, user);
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

            return CreatedAtAction(nameof(AddIngredient), new { id = ingredient.IngredientId }, new IngredientDto(ingredient));
        }

        // DELETE: api/Ingredient/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IngredientDto>> DeleteIngredient(long id)
        {
            PantryPlannerUser user = await _userManager.GetUserAsync(this.User);
            Ingredient ingredient = null;

            try
            {
                ingredient = _service.DeleteIngredient(id, user);
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

            return new IngredientDto(ingredient);
        }

    }
}
