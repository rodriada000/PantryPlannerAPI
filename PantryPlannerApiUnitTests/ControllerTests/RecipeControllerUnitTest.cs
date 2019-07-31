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
    public class RecipeControllerUnitTest
    {
        PantryPlannerContext _context;
        RecipeController _controller;
        FakeUserManager _userManager;

        public RecipeControllerUnitTest()
        {
            _userManager = new FakeUserManager();
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _userManager.TestUser, insertIngredientData: true);
            _controller = new RecipeController(_context, _userManager);
        }

        #region Get Test Methods

        [Fact]
        public async Task GetRecipe_ValidRecipeId_ReturnsOkAndCorrectRecipeDtoAsync()
        {
            Recipe expectedRecipe = _userManager.TestUser.Recipe.FirstOrDefault();

            ActionResult<RecipeDto> getResult = await _controller.GetRecipeAsync(expectedRecipe.RecipeId);
            Assert.IsType<OkObjectResult>(getResult.Result);

            var foundRecipe = (getResult.Result as ObjectResult).Value;

            Assert.IsType<RecipeDto>(foundRecipe);
            Xunit.Asserts.Compare.DeepAssert.Equals(new RecipeDto(expectedRecipe), (foundRecipe as RecipeDto));
        }

        [Fact]
        public async Task GetRecipe_InvalidRecipeId_ReturnsNotFound()
        {
            ActionResult<RecipeDto> getResult = await _controller.GetRecipeAsync(-5);
            Assert.IsType<NotFoundObjectResult>(getResult.Result);
        }

        #endregion


        #region Add Test Method

        [Fact]
        public async Task AddRecipe_ValidRecipeDtoAndUser_Returns201AndAddedRecipe()
        {
            string expectedName = "Ultimate Recipe Burrito";
            string expectedDescription = "this is actually a pizza recipe...";
            int expectedCookTime = 30;
            RecipeDto recipeToAdd = new RecipeDto()
            {
                Name = expectedName,
                Description = expectedDescription,
                CookTime = expectedCookTime
            };

            ActionResult<RecipeDto> addResult = await _controller.AddRecipeAsync(recipeToAdd);

            Assert.IsType<ObjectResult>(addResult.Result);
            Assert.Equal(StatusCodes.Status201Created, (addResult.Result as ObjectResult).StatusCode);


            var addedRecipe = (addResult.Result as ObjectResult).Value;
            Assert.IsType<RecipeDto>(addedRecipe);

            Assert.Equal(expectedName, (addedRecipe as RecipeDto).Name);
            Assert.Equal(expectedDescription, (addedRecipe as RecipeDto).Description);
            Assert.Equal(expectedCookTime, (addedRecipe as RecipeDto).CookTime);
        }

        [Fact]
        public async Task AddRecipe_RecipeDtoMissingName_Returns405MethodNotAllowed()
        {
            RecipeDto recipeToAdd = new RecipeDto()
            {
                Description = "blah blah"
            };

            ActionResult<RecipeDto> addResult = await _controller.AddRecipeAsync(recipeToAdd);

            Assert.IsType<ObjectResult>(addResult.Result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, (addResult.Result as ObjectResult).StatusCode);
        }

        #endregion


        #region Delete Test Methods

        [Fact]
        public async Task DeleteRecipe_ValidRecipe_ReturnsOkAndRecipeDeleted()
        {
            Recipe recipeToDelete = _userManager.TestUser.Recipe.FirstOrDefault();
            ActionResult<RecipeDto> deleteResult = await _controller.DeleteRecipeAsync(recipeToDelete.RecipeId);

            Assert.IsType<OkObjectResult>(deleteResult.Result);

            var deletedRecipe = (deleteResult.Result as OkObjectResult).Value;

            Assert.IsType<RecipeDto>(deletedRecipe);
            Assert.Equal(recipeToDelete.RecipeId, (deletedRecipe as RecipeDto).RecipeId);
        }

        [Fact]
        public async Task DeleteRecipe_RecipeNotExists_ReturnsNotFound()
        {
            ActionResult<RecipeDto> deleteResult = await _controller.DeleteRecipeAsync(-5);

            Assert.IsType<NotFoundObjectResult>(deleteResult.Result);
        }

        [Fact]
        public async Task DeleteRecipe_UserWithNoRights_ReturnsUnauthorized()
        {
            PantryPlannerUser randomUser = InMemoryDataGenerator.AddNewRandomUser(_context);
            Recipe notMyRecipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, randomUser);


            ActionResult<RecipeDto> deleteResult = await _controller.DeleteRecipeAsync(notMyRecipe.RecipeId);

            Assert.IsType<UnauthorizedObjectResult>(deleteResult.Result);
        }

        #endregion


        #region Update Test Methods

        [Fact]
        public async Task UpdateRecipe_ValidRecipeAndUser_ReturnsOk()
        {
            Recipe recipeToUpdate = _userManager.TestUser.Recipe.FirstOrDefault();
            string expectedDescription = "I copy and pasted this code :)";
            int expectedCookTime = 30;

            recipeToUpdate.Description = expectedDescription;
            recipeToUpdate.CookTime = expectedCookTime;

            ActionResult updateResult = await _controller.UpdateRecipeAsync(recipeToUpdate.RecipeId, new RecipeDto(recipeToUpdate));

            Assert.IsType<OkResult>(updateResult);
        }

        [Fact]
        public async Task UpdateRecipe_RecipeIdsDoNotMatch_ReturnBadRequest()
        {
            Recipe recipeToUpdate = _userManager.TestUser.Recipe.FirstOrDefault();

            ActionResult updateResult = await _controller.UpdateRecipeAsync(99999, new RecipeDto(recipeToUpdate));

            Assert.IsType<BadRequestObjectResult>(updateResult);
        }

        [Fact]
        public async Task UpdateRecipe_RecipeNotExists_ReturnNotFound()
        {
            ActionResult updateResult = await _controller.UpdateRecipeAsync(99999, new RecipeDto() { RecipeId = 99999, Name = "phony recipe"});

            Assert.IsType<NotFoundObjectResult>(updateResult);
        }


        [Fact]
        public async Task UpdateRecipe_NoRightsToRecipe_ReturnUnauthorized()
        {
            PantryPlannerUser randomUser = InMemoryDataGenerator.AddNewRandomUser(_context);
            Recipe notMyRecipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, randomUser);

            notMyRecipe.Description = "this won't update";

            ActionResult updateResult = await _controller.UpdateRecipeAsync(notMyRecipe.RecipeId, new RecipeDto(notMyRecipe));

            Assert.IsType<UnauthorizedObjectResult>(updateResult);
        }


        #endregion
    }
}
