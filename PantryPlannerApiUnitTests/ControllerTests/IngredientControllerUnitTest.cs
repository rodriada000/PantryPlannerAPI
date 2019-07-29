using System;
using Xunit;
using PantryPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Moq;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using PantryPlanner.DTOs;
using PantryPlanner.Controllers;

namespace PantryPlannerApiUnitTests
{
    public class IngredientControllerUnitTest
    {
        PantryPlannerContext _context;
        IngredientController _controller;
        FakeUserManager _userManager;

        public IngredientControllerUnitTest()
        {
            _userManager = new FakeUserManager();
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _userManager.TestUser, insertIngredientData: true);
            _controller = new IngredientController(_context, _userManager);
        }

        #region Get Test Methods

        #endregion


        #region Add Test Method

        [Fact]
        public async Task AddIngredient_ValidNewIngredient_ReturnsNewIngredient()
        {
            Ingredient ingredientToAdd = new Ingredient()
            {
                Name = "My Fresh Spice Of Bel-Air",
                Description = "Will Smith approved"
            };
            ActionResult<IngredientDto> addResult = await _controller.AddIngredient(ingredientToAdd);

            Assert.IsType<ObjectResult>(addResult.Result);
            Assert.Equal(StatusCodes.Status201Created, (addResult.Result as ObjectResult).StatusCode);


            var addedIngredient = (addResult.Result as ObjectResult).Value;

            Assert.IsType<IngredientDto>(addedIngredient);
            Assert.Equal(new IngredientDto(ingredientToAdd).ToString(), (addedIngredient as IngredientDto).ToString());
        }

        #endregion


        #region Delete Test Methods

        [Fact]
        public async Task DeleteIngredient_ValidIngredient_ReturnsIngredientDeleted()
        {
            Ingredient ingredientToDelete = _context.Ingredient.Where(i => i.AddedByUserId == _userManager.TestUser.Id).FirstOrDefault();
            ActionResult<IngredientDto> deleteResult = await _controller.DeleteIngredient(ingredientToDelete.IngredientId);

            Assert.IsType<OkObjectResult>(deleteResult.Result);

            var deletedIngredient = (deleteResult.Result as OkObjectResult).Value;

            Assert.IsType<IngredientDto>(deletedIngredient);
            Assert.Equal(new IngredientDto(ingredientToDelete).ToString(), (deletedIngredient as IngredientDto).ToString());
            Assert.False(_context.Ingredient.Any(i => i.IngredientId == ingredientToDelete.IngredientId));
        }

        [Fact]
        public async Task DeleteIngredient_UnknownIngredient_ReturnsNotFound()
        {
            ActionResult<IngredientDto> deleteResult = await _controller.DeleteIngredient(-5);

            Assert.IsType<NotFoundObjectResult>(deleteResult.Result);
        }

        #endregion


        #region Update Test Methods

        [Fact]
        public async Task UpdateIngredient_ValidIngredient_ReturnsOKAndContextIsUpdated()
        {
            Ingredient ingredientToUpdate = _context.Ingredient.Where(i => i.AddedByUserId == _userManager.TestUser.Id).FirstOrDefault();

            string expectedName = "My New Name!!";
            string expectedDescription = "morty summer, it's me. its regular rick";

            ingredientToUpdate.Name = expectedName;
            ingredientToUpdate.Description = expectedDescription;

            ActionResult updateResult = await _controller.UpdateIngredient(ingredientToUpdate.IngredientId, ingredientToUpdate);

            Assert.IsType<OkResult>(updateResult);

            Ingredient updatedIngredient = _context.Ingredient.Where(i => i.IngredientId == ingredientToUpdate.IngredientId).FirstOrDefault();
            Assert.Equal(expectedName, updatedIngredient.Name);
            Assert.Equal(expectedDescription, updatedIngredient.Description);
        }


        #endregion
    }
}
